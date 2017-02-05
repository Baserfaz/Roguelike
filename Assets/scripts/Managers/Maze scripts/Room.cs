using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room {

	public string roomName = "";
	public List<Tile> roomTiles = new List<Tile>();

	public Room(string _name, List<Tile> tiles) {
		roomName = _name;
		roomTiles = tiles;
	}
}
