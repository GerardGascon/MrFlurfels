using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterAnimation : MonoBehaviour{

	[SerializeField] Sprite[] sprites;
	SpriteRenderer renderer;
	ulong currentFrame;

	void Awake() {
		renderer = GetComponent<SpriteRenderer>();
	}

	void Start() {
		StartCoroutine(UpdateSprite());	
	}

	// Update is called once per frame
	void Update(){
		if (TimeBar.instance.currentTime != 100) renderer.sprite = sprites[TimeBar.instance.currentTime % 2];
	}

	IEnumerator UpdateSprite(){
		while (true) {
			if (TimeBar.instance.currentTime != 100) yield return null;
			else{
				renderer.sprite = sprites[++currentFrame % 2];
				yield return new WaitForSeconds(0.5f);
			}
			
		}
	}
}
