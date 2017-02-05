using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GuiHoverElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

	private GameObject hoverTextobj = null;

	public void OnPointerEnter(PointerEventData data) {

		// if we are casting spell
		// then we want to disable hover.
        if (MouseController.instance.GetState() == MouseController.State.CastSpell) return;

		// change state to hovering on gui 
		// -> can't click tiles under the gui element.
        MouseController.instance.ChangeState(MouseController.State.OnGui);

		// get the gui item info script.
		GUIItemInfo gii = data.pointerEnter.transform.parent.GetComponentInParent<GUIItemInfo>();

		// if we dont have a hover text instantiated yet..
		// -> instantgiate the text
		if(hoverTextobj == null) hoverTextobj = (GameObject) Instantiate(GUIManager.instance.hoverTextPrefab);

		// show text.
		hoverTextobj.GetComponent<CanvasGroup>().alpha = 1f;

		// which element we are currently
		// hovering on.
		// -> set the correct text.
		if(gii.myEffect != null) {

			hoverTextobj.GetComponent<Text>().text =
				"<size=\"32\">[Effect]: " + gii.myEffect.type.ToString() + "</size>" +
				"\nAmount: " + gii.myEffect.amount + 
				"\nDuration: " + gii.myEffect.duration;
			
		} else if(gii.myItem != null) {

			if(gii.myItem.GetComponent<Spell>() != null) { 

				hoverTextobj.GetComponent<Text>().text = 
					"<size=\"32\">[Spell]: " +
					gii.myItem.GetComponent<Spell>().spellType.ToString() +
					"</size>" + "\n" + gii.myItem.GetComponent<Spell>().itemDescription;

			} else {

				hoverTextobj.GetComponent<Text>().text =
					"<size=\"32\">[Item]: " + gii.myItem.GetComponent<Item>().itemName + "</size>" +
					"\n" + gii.myItem.GetComponent<Item>().itemDescription;
				
			}

		} else {
			
			// we dont have anything
			switch(gii.myType) {
			case GUIItemInfo.ElementType.SpellSlot:
				hoverTextobj.GetComponent<Text>().text = "<size=\"32\">[Empty Spell Slot]</size>";
				break;

			case GUIItemInfo.ElementType.ItemSlot:
				hoverTextobj.GetComponent<Text>().text = "<size=\"32\">[Empty Item Slot]</size>";
				break;

			case GUIItemInfo.ElementType.EffectSlot:
				hoverTextobj.GetComponent<Text>().text = "<size=\"32\">[Empty Effect Slot]</size>";
				break;
			}
		}

		// parent the text to canvas.
		hoverTextobj.transform.SetParent(GUIManager.instance.gameGUI.transform);

		// calculate the correct position
		// -> if on the right side of the screen, then move text left.
		// -> if on the left side of the screen, then move thext right.
		Vector2 mousePos = Input.mousePosition;
		Vector2 offset = Vector2.zero;

		// modify by using x-axel
		if(mousePos.x > Screen.width/2f) {
			offset = new Vector2(200f, 0f);
		}

		// modify by using y-axel.
		if(mousePos.y > Screen.height/2f) {
			offset += new Vector2(0f, 100f);
		} else {
			offset -= new Vector2(0f, 100f);
		}

		// set the position.
		hoverTextobj.transform.position = mousePos - offset;
	}

	public void OnPointerExit(PointerEventData data) {
		
		// change the state back to normal state.
		if(MouseController.instance.GetState() == MouseController.State.OnGui) {
			MouseController.instance.ChangeState(MouseController.State.Normal);
		}

		// dont destroy the gameobject
		// -> modify its alpha value.
		hoverTextobj.GetComponent<CanvasGroup>().alpha = 0f;
	}

	public void OnPointerClick(PointerEventData data) {

		// only accept mouse left click.
        if (data.button == PointerEventData.InputButton.Right ||
			data.button == PointerEventData.InputButton.Middle) return;

		// get reference
		GUIItemInfo gii = data.pointerEnter.transform.parent.GetComponentInParent<GUIItemInfo>();
		Player player = PrefabManager.instance.GetPlayerInstance().GetComponent<Player>();

		// which one we are clicking on.
		switch(gii.myType) {
		case GUIItemInfo.ElementType.ItemSlot:

			// mouse state to use item.
			GameObject useItem = player.GetComponent<Inventory>().currentUseItem;
			if(useItem != null) {
				if(useItem.GetComponent<UseItem>() != null) {
					useItem.GetComponent<UseItem>().Use();
				}
			}
			break;

		case GUIItemInfo.ElementType.SpellSlot:
	
			// mouse state to cast spell.
			GameObject spell = player.GetComponent<Inventory>().currentSpell;
			if(spell != null) {
				MouseController.instance.ChangeState(MouseController.State.CastSpell);
				MouseController.instance.ChangeCrosshairSprite(CrosshairManager.instance.crosshairSpell);
			}
			break;

		default:
			break;
		}
	}
}
