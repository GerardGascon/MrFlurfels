using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomShadows : MonoBehaviour{

	SpriteRenderer[] shadows;
	float transparencyVelocity;
	bool isTransparent;

	// Start is called before the first frame update
	void Awake(){
		shadows = GetComponentsInChildren<SpriteRenderer>();
	}

	// Update is called once per frame
	void Update(){
		foreach (SpriteRenderer shadow in shadows) {
			shadow.color = new Color(shadow.color.r, shadow.color.g, shadow.color.b, Mathf.SmoothDamp(shadow.color.a, isTransparent ? 0 : .8f, ref transparencyVelocity, 0.5f));
		}
	}

	void OnTriggerStay2D(Collider2D collision) {
		if (collision.CompareTag("Player")) {
			isTransparent = true;
		}
	}
	void OnTriggerExit2D(Collider2D collision) {
		if (collision.CompareTag("Player")) {
			isTransparent = false;
		}
	}
}
