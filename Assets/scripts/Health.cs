using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

	public int currentHealth = 0;
	public int maxHealth = 100;
	public bool isDead = false;

	void Awake() { UpdateCurrentHealth(); }
		
	public void UpdateCurrentHealth() {
		currentHealth = maxHealth;
	}

	/// <summary>
	/// Use this for attacks from other actors.
	/// </summary>
	/// <param name="amount">Amount.</param>
	/// <param name="isCrit">If set to <c>true</c> is crit.</param>
	public void TakeDamage(int amount, bool isCrit) {

		// get the armor rating
		// and simply subtract damage from it.
		int actualDmg = amount;

		if(GetComponent<Inventory>().currentArmor != null) {
			actualDmg -= GetComponent<Inventory>().currentArmor.GetComponent<Armor>().GetArmorRating();
		} else {
			actualDmg -= GetComponent<Actor>().defaultArmor;
		}

		if(actualDmg <= 0) actualDmg = 0;

		currentHealth -= actualDmg;

		if(isCrit) GUIManager.instance.CreateJournalEntry("Critical hit! " + GetComponent<Actor>().actorName + " took " + actualDmg + " damage.", GUIManager.JournalType.Combat);
		else GUIManager.instance.CreateJournalEntry(GetComponent<Actor>().actorName + " took " + actualDmg + " damage.", GUIManager.JournalType.Combat);

		if(GetComponent<Player>() != null) {
			GUIManager.instance.UpdateAllElements();
		}

		if(currentHealth <= 0) {
			currentHealth = 0;
			Die();
		}
	}

	/// <summary>
	/// Use this to take damage from potions etc.
	/// </summary>
	/// <param name="amount">Amount.</param>
	public void TakeDamageSimple(int amount) {

		currentHealth -= amount;

		GUIManager.instance.CreateJournalEntry(GetComponent<Actor>().actorName + " took " + amount + " damage.", GUIManager.JournalType.Combat);

		if(GetComponent<Player>() != null) {
			GUIManager.instance.UpdateAllElements();
		}

		if(currentHealth <= 0) {
			currentHealth = 0;
			Die();
		}
	}

	public void HealDamage(int amount) {
		currentHealth += amount;
		if(currentHealth > maxHealth) currentHealth = maxHealth;

		if(GetComponent<Player>() != null) {
			GUIManager.instance.UpdateAllElements();
		}
		GUIManager.instance.CreateJournalEntry(GetComponent<Actor>().actorName + " healed " + amount + " health.", GUIManager.JournalType.Combat);
	}

	private void Die() {

		isDead = true;

		// update tile's actor = null;
		DungeonGenerator.instance.UpdateTileActor(GetComponent<Actor>().position, null);

		// if its enemy.. drop loot.
		if(GetComponent<Enemy>() != null) {

			GameObject targetTile = DungeonGenerator.instance.GetTileAtPos(GetComponent<Actor>().position);

			// drop item under the 
			if(targetTile.GetComponent<Tile>().item == null) {
				ItemDropController.instance.DropItem(targetTile.GetComponent<Tile>().position);
			} else {

				// if tile has an item already.
				// -> calculate new position.
				targetTile = DungeonGenerator.instance.GetFirstFreeTileNearPosition(GetComponent<Actor>().position);
				if(targetTile != null) {
					ItemDropController.instance.DropItem(targetTile.GetComponent<Tile>().position);
				}
			}

			GetComponent<Enemy>().isActive = false;

			// deactivate actor
			gameObject.SetActive(false);

			GUIManager.instance.CreateJournalEntry(GetComponent<Actor>().actorName + " died.", GUIManager.JournalType.Combat);


		} 
	}


}
