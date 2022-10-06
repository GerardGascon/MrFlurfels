using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Notepad : MonoBehaviour{

	public static Notepad instance;
	
	[HideInInspector] public bool opened;

	[SerializeField] TMP_InputField inputField;

	Animator anim;

	// Start is called before the first frame update
	void Awake(){
		instance = this;
		
		anim = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update(){
		if (Input.GetKeyDown(KeyCode.E) && Time.timeScale > .5f && !inputField.isFocused) {
			if (!opened)
				Open();
			else
				Close();
		}

		if(Input.GetKeyDown(KeyCode.Escape) && Time.timeScale > .5f && opened)
			Close();
	}

	void Open(){
		anim.SetTrigger("In");
	}

	public void Close(){
		anim.SetTrigger("Out");
	}

	public void Opened() {
		opened = true;
		inputField.Select();
		inputField.ActivateInputField();
	}
	public void Closed(){
		opened = false;
	}
}
