using SimpleTools.AudioManager;
using SimpleTools.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour{

	[SerializeField] Animator anim;
	[SerializeField] Animator menu;
	
	public void PlayAnimation() {
		menu.SetTrigger("Disable");
		AudioManager.instance.FadeOut("menu", 1f);
		AudioManager.instance.Play("cinematica");
		StartCoroutine(DelayCutscene());
	}

	IEnumerator DelayCutscene(){
		yield return new WaitForSeconds(2.5f);
		anim.SetBool("cinematica", true);
	}

	public void EndAnimation(){
		Loader.Load("Main");
	}
}
