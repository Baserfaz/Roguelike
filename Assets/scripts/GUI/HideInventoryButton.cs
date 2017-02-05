using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HideInventoryButton : MonoBehaviour {
	
	void Start () {
		// add click listener.
		GetComponent<Button>().onClick.AddListener(() => HideInv());
	}

	private void HideInv() {
		GUIManager.instance.HideInventory();
		MouseController.instance.ChangeState(MouseController.State.Normal);
		GUIManager.instance.setIsInventoryOpen(false);
		GUIManager.instance.ResetChosenEffectOnInvGuiItem();
	}

}
