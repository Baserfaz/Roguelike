using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GuiHoverElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

	private GameObject hoverTextobj = null;

	public void OnPointerEnter(PointerEventData data) {
		GUIItemInfo gii = data.pointerEnter.transform.parent.GetComponentInParent<GUIItemInfo>();

		gii.SetCanvasGroupAlpha(1f);

		if(hoverTextobj == null) {
			hoverTextobj = (GameObject) Instantiate(GUIManager.instance.hoverTextPrefab);

			if(gii.myEffect != null) {

				hoverTextobj.GetComponent<Text>().text =
					"<size=\"32\">[Effect]: " + gii.myEffect.type.ToString() + "</size>";
				
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

			hoverTextobj.transform.SetParent(GUIManager.instance.mainGUI.transform);
			hoverTextobj.transform.position = Input.mousePosition;
		}
	}

	public void OnPointerExit(PointerEventData data) {
		GUIItemInfo gii = data.pointerEnter.transform.parent.GetComponentInParent<GUIItemInfo>();
		gii.ResetCanvasGroupAlpha();
		Destroy(hoverTextobj);
	}

	public void OnPointerClick(PointerEventData data) {
	
		GUIItemInfo gii = data.pointerEnter.transform.parent.GetComponentInParent<GUIItemInfo>();

		Player player = PrefabManager.instance.GetPlayerInstance().GetComponent<Player>();

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
