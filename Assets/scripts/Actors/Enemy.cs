using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Actor {

	[Header("Enemy specific settings")]
	public string enemyDescription = "";
    public bool canDropItems = true;

	[HideInInspector] public bool isActive = false;
	[HideInInspector] public Vector2 targetPosition = Vector2.zero;

	/// <summary>
	/// Finds the next tile.
	/// This is really simplified pathfinding 
	/// and should be upgraded to atleast to use
	/// pathfinding.cs (breadth-first search).
	/// </summary>
	/// <returns>The find next tile.</returns>
	private GameObject PathFindNextTile() {

		// Simple pathfinding algorithm
		// 1. get the tiles around the enemy.
		// 2. get candidate tiles (allowed to walk etc.)
		// 3. get the closest tile to player
		// 4. return that tile

		float shortestDistance = Mathf.Infinity;
		GameObject bestTile = null;

		foreach(GameObject tileGo in DungeonGenerator.instance.GetTilesAroundPosition(position, 1)) {
			Tile tile = tileGo.GetComponent<Tile>();

			// if the enemy is set to be static
			// i.e. can't move but can attack.
			// then it just sits still and attacks
			// player when there is one nearby.
			if(isStatic) {
				if(tile.actor != null) {
					if (tile.actor.GetComponent<Player>() != null) {
						myNextState = NextMoveState.Attack;
						bestTile = tileGo;
					}
				}
			} else {

				// if the player is near 
				// prioritize attacking.
				if(tile.actor != null) {
					if(tile.actor.GetComponent<Player>() != null) {
						bestTile = tileGo;
						myNextState = NextMoveState.Attack;
						break;
					}
				}

				// tile is walkable
				if(tile.myType == Tile.TileType.Floor ||
					tile.myType == Tile.TileType.DoorOpen ||
					tile.myType == Tile.TileType.FloorSpecialItem) {

					// tile is not occupied.
					if(tile.actor == null) {

						float distance = Vector2.Distance(tile.position, targetPosition);

						if(distance < shortestDistance) {
							bestTile = tileGo;
							shortestDistance = distance;
							myNextState = NextMoveState.Move;
						}

					} 

					// if the player is standing on an exit tile
					// and this enemy can reach it -> attack.
				} else if(tile.myType == Tile.TileType.Exit) {

					if(tile.actor != null) {
						if(tile.actor.GetComponent<Player>() != null) {
							bestTile = tileGo;
							myNextState = NextMoveState.Attack;
							break;
						}
					}
				}
			}
		}
		return bestTile;
	}

    private void HandleAbility(Ability myAbility)
    {
            // get our boss component.
            Boss boss = GetComponent<Boss>();
            
            // player distance
            Player player = PrefabManager.instance.GetPlayerInstance().GetComponent<Player>();

            // target position.
            Vector2 targetPos = Vector2.zero;

            List<GameObject> tiles = new List<GameObject>();
            Tile chosenTile = null;
            float distance = Mathf.Infinity;

            // get the target according to the ability.
            switch(myAbility.myAbilityType) {

                case Ability.Abilities.Summon:

                    tiles = DungeonGenerator.instance.GetTilesAroundPosition(position, 2);

                    // get's the closest tile near player and
                    // summons an enemy there.
                    foreach(GameObject tileGo in tiles) {
                        Tile tile = tileGo.GetComponent<Tile>();

                        if (tile.actor != null)
                        {
                            continue;
                        }

                        if(tile.myType == Tile.TileType.DoorOpen ||
                            tile.myType == Tile.TileType.Exit ||
                            tile.myType == Tile.TileType.Floor || 
                            tile.myType == Tile.TileType.FloorSpecialItem) {

                            float calcDist = Vector2.Distance(tile.position, player.position);

                            if(calcDist < distance) {
                                chosenTile = tile;
                                distance = calcDist;
                            }
                        }
                    }

                    targetPos = chosenTile.position;
                    break;

                default:
                    Debug.LogError("Error: No such ability type!");
                    break;
            }

            // do ability.
            boss.DoAbility(myAbility, targetPos);
    }

    private void TryUseAbility(Boss boss)
    {
        // get available abilities
        Ability[] availableAbilities = boss.myAbilities;

        // convert them to a queue.
        Queue<Ability> abilityQueue = new Queue<Ability>(availableAbilities);

        // try to use any ability that is not on cooldown.
        while (abilityQueue.Count > 0)
        {

            // get one ability from the ability queue.
            Ability currentAbility = abilityQueue.Dequeue();

            // test ability cooldown.
            if (currentAbility.currentCooldown <= 0)
            {

                // try to use ability.
                HandleAbility(currentAbility);

                // reset cooldown.
                currentAbility.currentCooldown = currentAbility.cooldown + 1;

                // reduce the cooldown of every ability.
                boss.ReduceCooldownAllAbilities();

                break;
            }
        }

    }

	private void CalculateNextStep() {

        // TODO:
        // 1. normal monsters can have abilities too!

        if (GetComponent<Boss>() != null) {

            // Basic boss AI.
            // 1. tries to use all abilities first
            // 2. when all are on cooldown -> use melee attack

            // get boss component.
            Boss boss = GetComponent<Boss>();
            
            // tries to use all abilities first
            TryUseAbility(boss);

            // move & attack % pass
            // are handled here.
            GameObject bestTile = PathFindNextTile();
            if (bestTile != null) moveTargetPosition = bestTile.GetComponent<Tile>().position;
            else myNextState = NextMoveState.Pass;

            // reduce cooldowns.
            boss.ReduceCooldownAllAbilities();

        } else {
            GameObject bestTile = PathFindNextTile();
            if (bestTile != null) moveTargetPosition = bestTile.GetComponent<Tile>().position;
            else myNextState = NextMoveState.Pass;
        }
	}

	public void DecideNextStep() {
		CalculateNextStep();

		if(myNextState == NextMoveState.Attack) {
			Attack();
		} else if(myNextState == NextMoveState.Move) {
			Move();
		} else if(myNextState == NextMoveState.Pass) {
			// pass the turn.
		}
	}

}
