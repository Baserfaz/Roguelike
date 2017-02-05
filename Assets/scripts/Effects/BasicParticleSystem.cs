using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicParticleSystem : MonoBehaviour {

	void Start () {
		transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		Destroy(this.gameObject, GetComponent<ParticleSystem>().main.duration);
	}
}
