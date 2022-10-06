using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RiverTravelAnim : MonoBehaviour{
	public void GoToRiver() {
		FindObjectOfType<RiverTravel>().GoToRiver();
	}
}
