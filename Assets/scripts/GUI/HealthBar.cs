using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

	private GameObject myHealthBar;
	private Vector3 offset = new Vector3(0f, 0.7f, 0f);

	void Start() { 
		myHealthBar = (GameObject) Instantiate(GUIManager.instance.healthBarPrefab);
		myHealthBar.transform.SetParent(this.transform);
		myHealthBar.transform.localPosition = Vector3.zero + offset;
		Hide();
	}

	public void Show() { myHealthBar.GetComponent<CanvasGroup>().alpha = 1f; }
	public void Hide() { myHealthBar.GetComponent<CanvasGroup>().alpha = 0f; }

	public void UpdateHPBar() {

		Slider hpSlider = myHealthBar.GetComponent<Slider>();
		Health health = GetComponent<Health>();

		Show();

		hpSlider.maxValue = health.maxHealth;
		hpSlider.value = health.currentHealth;
	}


}
