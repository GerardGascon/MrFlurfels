using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using SimpleTools.AudioManager;

public class ButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler {

	public void OnPointerEnter(PointerEventData ped) {
		AudioManager.instance.PlayOneShot("hover_menu");
	}

	public void OnPointerDown(PointerEventData ped) {
		AudioManager.instance.PlayOneShot("seleccion_menu");
	}
}