using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour {

	// Attack state is basically melee attack.
	// -> when the player moves towards enemy.
	// --> spells/and use items doesn't use states.
	public enum NextMoveState { Move, Attack, Pass, Stuck, None };

	public string actorName = "";

	// default to pass state.
	[HideInInspector] public NextMoveState myNextState = NextMoveState.Pass;
	[HideInInspector] public Vector2 position;

	protected Vector2 moveTargetPosition;
	private Color startColor;

	[Header("Actor settings")]
	public int defaultDamage = 1;
	public int defaultArmor = 0;
	public int defaultCritChance = 5;
	public float defaultCritMultiplier = 2f;
	public int defaultMissChance = 25;
	public int expAmount = 15;

	void Awake() { startColor = GetComponentInChildren<SpriteRenderer>().color; }

	public void Hide() { 
		GetComponentInChildren<SpriteRenderer>().color = Color.clear; 
		if(GetComponent<shadowController>() != null) GetComponent<shadowController>().Hide();
	}
	public void Show() { 
		GetComponentInChildren<SpriteRenderer>().color = startColor; 
		if(GetComponent<shadowController>() != null) GetComponent<shadowController>().Show();
	}

	public void Attack() {
		bool crit = false;
		int totalDamage = defaultDamage;

		GameObject target = DungeonGenerator.instance.GetTileAtPos(moveTargetPosition).GetComponent<Tile>().actor;

		// calculate weapon damage
		if(GetComponent<Inventory>() != null) {
			if(GetComponent<Inventory>().currentWeapon != null) {
				Weapon weapon = GetComponent<Inventory>().currentWeapon.GetComponent<Weapon>();
				totalDamage += Random.Range(weapon.minDamage, weapon.maxDamage);
			}
		} else {
			// use default damage.
		}

		// calculate miss
		if(Random.Range(0, 100) > 100 - defaultMissChance) {
			GUIManager.instance.CreatePopUpEntry("MISS", target.GetComponent<Actor>().position, GUIManager.PopUpType.Miss);
			GUIManager.instance.CreateJournalEntry(actorName + " missed.", GUIManager.JournalType.Combat);
			return;
		}

		// calculate critical hit
		if(Random.Range(0, 100) > 100 - defaultCritChance) {
			totalDamage = Mathf.FloorToInt(totalDamage * defaultCritMultiplier);
			crit = true;
		}

		target.GetComponent<Health>().TakeDamage(totalDamage, crit);

		// degenerate armor.
		if(target.GetComponent<Inventory>() != null) {
			if(target.GetComponent<Inventory>().currentArmor != null) {
				target.GetComponent<Inventory>().currentArmor.GetComponent<Armor>().SubtractArmor();
			} else if(target.GetComponent<Actor>().defaultArmor > 0) {
				if(GameMaster.instance.attacksSubtractDefaultArmor) target.GetComponent<Actor>().defaultArmor --;
			}
		} else {
			if(GameMaster.instance.attacksSubtractDefaultArmor) target.GetComponent<Actor>().defaultArmor --;
		}

	}

	public void Move() {
		// first reset the tile actor field.
		DungeonGenerator.instance.UpdateTileActor(position, null);

		// calculate z-level.
		int zLevel = 0;
		if(GetComponent<Player>() != null) zLevel = GameMaster.instance.playerZLevel;
		else if(GetComponent<Enemy>() != null) zLevel = GameMaster.instance.enemyZLevel;

		// move the actor
		Vector3 target = new Vector3(moveTargetPosition.x, moveTargetPosition.y, zLevel);
		transform.position = target;

		position = new Vector2(transform.position.x, transform.position.y);

		GameObject tileGo = DungeonGenerator.instance.GetTileAtPos(position);

		// if we stepped on a trap.
		if(tileGo.GetComponent<Trap>() != null) {
			tileGo.GetComponent<Trap>().Activate();
		}

		// update next tile's actor field.
		DungeonGenerator.instance.UpdateTileActor(moveTargetPosition, this.gameObject);
	}


}
