using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour {

	Tile target = null;

	bool finished = false;

	List<Tile> visitedTiles = new List<Tile>();
	List<Tile> path = new List<Tile>();

	Queue<Tile> foundTiles = new Queue<Tile>();

	/// <summary>
	/// Breadth-first search.
	/// </summary>
	/// <returns>The path to target.</returns>
	/// <param name="_target">Target.</param>
	public List<Tile> CalculatePathToTarget(Tile _target) {

		// reset tile parent tiles
		foreach(GameObject t in DungeonGenerator.instance.GetTiles()) {
			Node node = t.GetComponent<Node>();
			node.parentNode = null;
		}

		// get the player
		Player player = PrefabManager.instance.GetPlayerInstance().GetComponent<Player>();

		// set the target.
		target = _target;

		// reset
		foundTiles.Clear();
		visitedTiles.Clear();
		path.Clear();
		finished = false;

		// get the first tile.
		Tile startTile = DungeonGenerator.instance.GetTileAtPos(player.position).GetComponent<Tile>();

		// enqueue it.
		foundTiles.Enqueue(startTile);

		// start algorithm.
		FindPath(startTile);

		// draw the path.
		//DrawPath();

		// return it.
		return path;
	}

	private void DrawPath() {
		foreach(Tile tile in path) { tile.GetComponentInChildren<SpriteRenderer>().color = Color.red; }
	}


	private Tile GetParent(Tile tile) {
		return tile.GetComponent<Node>().parentNode;
	}

	private void SearchTiles(Tile _tile) {

		GameObject[] ts = DungeonGenerator.instance.GetTilesAroundPosition(_tile.position, 1).ToArray();

		if(ts.Length == 0) return;

		// check if we can see our target.
		foreach(GameObject t in ts) {
			Tile tile = t.GetComponent<Tile>();

			if(tile.position == target.position) {
				tile.GetComponent<Node>().parentNode = _tile;
				CreatePath(tile);
				finished = true;
				return;
			}
		}

		// add all tiles to found list
		foreach(GameObject t in ts) {
			Tile tile = t.GetComponent<Tile>();

			if(foundTiles.Contains(tile) || visitedTiles.Contains(tile) || tile.isDiscovered == false) continue;

			if(tile.actor != null) continue;

			if(tile.myType == Tile.TileType.Floor ||
				tile.myType == Tile.TileType.DoorOpen ||
				tile.myType == Tile.TileType.Exit ||
				tile.myType == Tile.TileType.FloorSpecialItem) {

				tile.GetComponent<Node>().parentNode = _tile;

				foundTiles.Enqueue(tile);
			}
		}

	}

	private void CreatePath(Tile targetTile) {

		Tile a = targetTile;
		path.Add(a);

		int loop = 0;

		while(true) {

			if(loop > 200) {
				Debug.LogError("Loop error: CreatePath");
				break;
			}

			a = GetParent(a);

			if(a == null) {
				break;
			}

			path.Add(a);

			loop++;
		}
	}

	private void FindPath(Tile _tile) {

		while(foundTiles.Count > 0) {

			if(finished) return;

			Tile a = foundTiles.Dequeue();
				
			if(visitedTiles.Contains(a) == false) {
				visitedTiles.Add(a);
				SearchTiles(a);
			}
		}
	}
}
