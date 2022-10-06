using SimpleTools.AudioManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour{

	bool extrasMenu, creditsMenu;
	[SerializeField] Animator anim;

	void Start() {
		AudioManager.instance.Play("menu");
	}

	// Update is called once per frame
	void Update(){
		if(Input.GetKeyDown(KeyCode.Escape)){
			if(extrasMenu){
				if(creditsMenu){
					creditsMenu = false;
					anim.SetBool("Credits", false);
				} else{
					extrasMenu = false;
					anim.SetBool("Extras", false);
				}
			}
		}
	}

	public void Extras(){
		extrasMenu = true;
		anim.SetBool("Extras", true);
	}
	public void Credits(){
		creditsMenu = true;
		anim.SetBool("Credits", true);
	}
}
