using SimpleTools.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour{

	Animator anim;
	bool paused;

	// Start is called before the first frame update
	void Awake(){
		anim = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update(){
		if (Input.GetKeyDown(KeyCode.Escape) && !Notepad.instance.opened) {
			if (Time.timeScale == 1 && !paused) {
				paused = true;
				Time.timeScale = 0;
				anim.SetTrigger("Pause");
			} else if (paused){
				paused = false;
				anim.SetTrigger("Unpause");
			}
		}
	}

	public void EndMenuAnimation(){
		Time.timeScale = paused ? 0 : 1;
	}

	public void Leave(){
		Loader.Load(0);
		Time.timeScale = 1;
	}
	public void Continue(){
		if (!Notepad.instance.opened && paused) {
			paused = false;
			anim.SetTrigger("Unpause");
		}
	}
}
