using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour {

	public enum State { Active, Inactive }

	[SerializeField]
	private State myState = State.Inactive;

	public int activatedOnTurn = 0;

	public State GetActiveStatus() { return myState; }

	public void Activate() {
		if(myState == State.Inactive) {
			myState = State.Active;
			activatedOnTurn = GameMaster.instance.turnCount;
		}
	}

	public void UpdateGraphics() {
		if(myState == State.Active) {
			GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(
				SpriteManager.SpriteType.TrapOn);
		} else if(myState == State.Inactive) {
			GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(
				SpriteManager.SpriteType.TrapOff);
		}
	}

	public void Deactivate() { myState = State.Inactive; }

}
