using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverTravel : MonoBehaviour{

	bool right;
	[SerializeField] Vector3 posRight, posLeft;
	[SerializeField] Vector3 boatPosRight, boatPosLeft;
	[SerializeField] GameObject boat;
	[SerializeField] Animator uiAnim;

	PlayerController player;
	HelperController helper;

	// Start is called before the first frame update
	void Awake(){
		player = FindObjectOfType<PlayerController>();
		helper = FindObjectOfType<HelperController>();
	}

	public void GoToRiver() {
		player.transform.position = helper.transform.position = new Vector3(!right ? posLeft.x : posRight.x, !right ? posLeft.y : posRight.y, !right ? posLeft.y : posRight.y / 50f);
		boat.transform.position = new Vector3(!right ? boatPosLeft.x : boatPosRight.x, !right ? boatPosLeft.y : boatPosRight.y, boat.transform.position.z);
		right ^= true;
	}
	
	public void ChangeSide(){
		uiAnim.SetTrigger("Change");
	}
	
	void OnDrawGizmos(){
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(posRight, 0.5f);
		Gizmos.DrawWireSphere(posLeft, 0.5f);
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(boatPosRight, 0.5f);
		Gizmos.DrawWireSphere(boatPosLeft, 0.5f);
	}
}
