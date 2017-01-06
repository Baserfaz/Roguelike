using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonItemManager : MonoBehaviour {

	[Header("Dungeon item settings")]
	public bool allowItemSpawns = true;
	[Range(0, 100)] public int chanceToSpawnChest = 10;
	[Range(0, 100)] public int chanceToSpawnGold = 10;



}
