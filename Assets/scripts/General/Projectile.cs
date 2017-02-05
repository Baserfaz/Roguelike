using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	private Vector3 start = Vector3.zero;
	private Vector3 end = Vector3.zero;

	public void StartMe(Vector2 _target, Vector2 start) {
		start = new Vector3(start.x, start.y, GameMaster.instance.playerZLevel - 0.1f);
		end = new Vector3(_target.x, _target.y, GameMaster.instance.playerZLevel - 0.1f);
		StartCoroutine(Move());
	}

	private IEnumerator Move() {

		float currentTime = 0f;
		float maxTime = 0.5f;

		while(true) {
		
			currentTime += Time.deltaTime;

			transform.position = Vector3.Lerp(start, end, currentTime/maxTime);

			yield return null;
		}

		Destroy(this.gameObject);
	}

	//void Start() {
	//	start = new Vector3(startPoint.x, startPoint.y, GameMaster.instance.playerZLevel - 0.1f);
	//	end = new Vector3(target.x, target.y, GameMaster.instance.playerZLevel - 0.1f);
	//}

	//void Update() {
	//	transform.position = Vector3.Lerp(start, end, Time.deltaTime);
	//	if(Vector3.Distance(transform.position, end) < 0.1f) Destroy(this.gameObject);
	//}
}
