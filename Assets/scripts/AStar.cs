using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour {


	public List<Tile> FindPath(Tile goal) {

		// player
		Player player = GetComponent<Player>();

		// create lists
		List<Tile> closedSet = new List<Tile>();
		List<Tile> openSet = new List<Tile>();

		// get the start tile.
		Tile start = DungeonGenerator.instance.GetTileAtPos(player.position).GetComponent<Tile>();

		// set the start score to zero.
		start.GetComponent<Node>().gScore = 0f;
		start.GetComponent<Node>().fScore = 0f;

		// set the start position
		openSet.Add(start);

		while(openSet.Count > 0) {

			Tile current = null;

			// get the node that has the lowest fscore.
			foreach(Tile tile in openSet) {
				if(current == null) {
					current = tile;
					continue;
				}
				if(tile.GetComponent<Node>().fScore < current.GetComponent<Node>().fScore) {
					current = tile;
				}
			}

			// if we are at the goal position
			// construct path and return it.
			if(current.position == goal.position) {
				List<Tile> completePath = ConstructPath(current, player);
				//DrawPath(completePath);
				return completePath;
			}

			// remove current tile from openset 
			// and add it to closedset.
			openSet.Remove(current);
			closedSet.Add(current);

			// loop through all neighbors of current tile.
			foreach(GameObject n in DungeonGenerator.instance.GetTilesAroundPosition(current.position, 1)) {

				Tile neighbor = n.GetComponent<Tile>();

				if(neighbor.myType == Tile.TileType.DoorClosed || 
					neighbor.myType == Tile.TileType.OuterWall ||
					neighbor.myType == Tile.TileType.Wall) continue;

				if(neighbor.actor != null) continue;

				// first check if the tile is same 
				// as current... 
				// because GetTilesAroundPosition() returns it too.
				if(neighbor.position == current.position) continue;

				// ignore already evaluated tile.
				if(closedSet.Contains(neighbor)) continue;

				// calculate score based on distance.
				float tentative_gScore = current.GetComponent<Node>().gScore + Vector2.Distance(current.position, neighbor.position);

				// Discover a new tile
				if(openSet.Contains(neighbor) == false) {
					openSet.Add(neighbor);
				} else if (tentative_gScore >= neighbor.GetComponent<Node>().gScore) {
					continue;
				}

				Node neighborNode = neighbor.GetComponent<Node>();

				neighborNode.parentNode = current;
				neighborNode.gScore = tentative_gScore;
				neighborNode.fScore = neighborNode.gScore + HeuristicCostEstimate(neighbor, goal);
			}
		}

		// failure.
		return null;
	}

	private void DrawPath(List<Tile> _path) {
		foreach(Tile tile in _path) { 
			tile.GetComponentInChildren<SpriteRenderer>().color = Color.red;
		}
	}

	private float HeuristicCostEstimate(Tile neighbor, Tile goal) {

		// TODO

		return 0f;
	}

	private Tile GetParentNode(Tile t) {
		return t.GetComponent<Node>().parentNode;
	}

	private List<Tile> ConstructPath(Tile current, Player player) {
		List<Tile> totalPath = new List<Tile>();
		totalPath.Add(current);

		Tile parent = GetParentNode(current);
		totalPath.Add(parent);

		int counter = 0;

		while(true) {

			// safety first ;)
			if(counter > 200) {
				Debug.Log("Loop error: ConstructPath");
				break;
			}

			parent = GetParentNode(parent);

			totalPath.Add(parent);

			// if we are back at the player position
			// -> break.
			if(parent.position == player.position) break;

			counter ++;

		}

		// remove the player current position from the list.
		totalPath.RemoveAt(totalPath.Count - 1);

		return totalPath;
	}


}
