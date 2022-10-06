using Assets.Scripts.TestScenes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(ObjectDepthSetter))]
public class DialoguePrompterManagerEditor : Editor {
	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		ObjectDepthSetter myScript = (ObjectDepthSetter)target;
		if (GUILayout.Button("Set Depth")) {
			myScript.SetDepth();
			Debug.Log("Depth Setted.");
		}
	}
}
#endif

public class ObjectDepthSetter : MonoBehaviour{
	public void SetDepth(){
		transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y / 50);
	}

	[SerializeField] bool updateInRealtime;
	void Update() {
		if(updateInRealtime)
			transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y / 50);
	}
}
