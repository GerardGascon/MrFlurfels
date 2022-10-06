using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleTools.AudioManager;

public class PlaySound : MonoBehaviour{
	
	public void PlayConfeti(){
		AudioManager.instance.PlayOneShot("confeti");
	}
}
