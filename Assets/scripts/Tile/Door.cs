using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

	public void OpenDoor() {
		
		SpriteManager.SpriteType mySpriteType = GetComponent<Tile>().mySpriteType;

		GetComponent<Tile>().myType = Tile.TileType.DoorOpen;

		if(mySpriteType == SpriteManager.SpriteType.DoorHorizontalClosed) {
			GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.DoorHorizontalOpen);
		} else if(mySpriteType == SpriteManager.SpriteType.DoorVerticalClosed) {
			GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.DoorVerticalOpen);
		}

	}


}
