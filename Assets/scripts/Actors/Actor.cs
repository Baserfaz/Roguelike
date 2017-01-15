using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour {

	// Attack state is basically melee attack.
	// -> when the player moves towards enemy.
	// --> spells/and use items doesn't use states.
	public enum NextMoveState { Move, Attack, Pass, Stuck, None, Stunned };

	public string actorName = "";
	public bool canFly = false;
	public bool isStatic = false;

	// default to pass state.
	[HideInInspector] public NextMoveState myNextState = NextMoveState.Pass;
	[HideInInspector] public Vector2 position;

	// list of current status effects.
	// these will proc on each turn.
	private List<StatusEffect> myStatusEffects = new List<StatusEffect>();

	protected Vector2 moveTargetPosition;
	private Color startColor;

	[Header("Actor settings")]
	public int defaultDamage = 1;
	public int defaultArmor = 0;
	public int defaultCritChance = 5;
	public float defaultCritMultiplier = 2f;
	public int defaultMissChance = 25;
	public int expAmount = 15;

	// status effects modify these and 
	// these can't be reduced by anything.
	[HideInInspector] public int buffedDamage = 0;
	[HideInInspector] public int buffedArmor = 0;
	[HideInInspector] public float buffedExpMultiplier = 1;

	void Awake() { 
		startColor = GetComponentInChildren<SpriteRenderer>().color; 
		buffedDamage = 0;
		buffedArmor = 0;
		buffedExpMultiplier = 1f;
	}

	public void SetMoveTargetPosition(Vector2 pos) { moveTargetPosition = pos; }
	public Vector2 GetMoveTargetPosition() { return moveTargetPosition; }

	public void Hide() { 
		GetComponentInChildren<SpriteRenderer>().color = Color.clear; 
		if(GetComponent<shadowController>() != null) GetComponent<shadowController>().Hide();
	}
	public void Show() { 
		GetComponentInChildren<SpriteRenderer>().color = startColor; 
		if(GetComponent<shadowController>() != null) GetComponent<shadowController>().Show();
	}

	public void AddStatusEffect(StatusEffect effect) {
		myStatusEffects.Add(effect);
		if(GetComponent<Player>() != null) {
			GUIManager.instance.CreateStatusBarElement(effect);
		}
	}

	public StatusEffect[] GetStatusEffects() {
		return myStatusEffects.ToArray();
	}

	public void RemoveStatusEffect(StatusEffect e) {
		myStatusEffects.Remove(e);
	}

	public void Attack() {
		bool crit = false;
		int totalDamage = defaultDamage + buffedDamage;

		GameObject target = DungeonGenerator.instance.GetTileAtPos(moveTargetPosition).GetComponent<Tile>().actor;

		// calculate weapon damage
		if(GetComponent<Inventory>() != null) {
			if(GetComponent<Inventory>().currentWeapon != null) {
				Weapon weapon = GetComponent<Inventory>().currentWeapon.GetComponent<Weapon>();
				totalDamage += Random.Range(weapon.minDamage, weapon.maxDamage);
			}
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

		// do damage to the target.
		target.GetComponent<Health>().TakeDamage(totalDamage, crit, this.gameObject);

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

	private IEnumerator MoveCharacterSmooth(Vector3 target) {

		float currentTime = 0f;
        float maxTime = 1f;

		Vector3 startPos = position;
		Vector3 endPos = target;

		while(currentTime < maxTime) {
			currentTime += Time.deltaTime;
            // WAS transform.position not startPOS
            transform.position = Vector3.Lerp(transform.position, endPos, currentTime / maxTime);
			yield return null;
		}

		transform.position = target;
	}

	public void StopCoroutines() { StopAllCoroutines(); }

	public void Move() {
		
		// first reset the tile actor field.
		DungeonGenerator.instance.UpdateTileActor(position, null);

		// calculate correct z-level.
		int zLevel = 0;
		if(GetComponent<Player>() != null) zLevel = GameMaster.instance.playerZLevel;
		else if(GetComponent<Enemy>() != null) zLevel = GameMaster.instance.enemyZLevel;

		// create target vector3
		Vector3 target = new Vector3(moveTargetPosition.x, moveTargetPosition.y, zLevel);

		// smoothly move the actor.
		if(GameMaster.instance.allowSmoothMovement) {
			StopAllCoroutines();
			if(this.gameObject.activeInHierarchy == true) StartCoroutine(MoveCharacterSmooth(target));
			else return;
		} else {
			transform.position = target;
		}

		// save the position.
		position = moveTargetPosition;

		// get the current tile we just stepped.
		GameObject tileGo = DungeonGenerator.instance.GetTileAtPos(position);
		Tile tile = tileGo.GetComponent<Tile>();

		// pick up gold automatically.
		if(GameMaster.instance.pickupGoldAutomatically) {
			if(tile.item != null) {
				if(tile.item.GetComponent<Gold>() != null && GetComponent<Player>() != null) {
					GetComponent<Player>().PickUpItem(tile.item);
				}
			}
		}

		// if we stepped on a trap.
		if(tileGo.GetComponent<Trap>() != null) {
			if(canFly == false) tileGo.GetComponent<Trap>().Activate();
		}

		// update next tile's actor field.
		DungeonGenerator.instance.UpdateTileActor(moveTargetPosition, this.gameObject);
	}


}
