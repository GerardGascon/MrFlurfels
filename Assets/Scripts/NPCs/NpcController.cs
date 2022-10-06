using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class NpcController : WaypointEntity
{
	[SerializeField] private List<Keyframe> _keyframes = new List<Keyframe>();
	[SerializeField] Animator _anim;
	Vector2 _startPos;

	[System.Serializable]
	public class Keyframe
	{
		public int Frame;
		public Vector2 LocalPosition;

		public Keyframe(Vector2 a_localPosition)
		{
			LocalPosition = a_localPosition;
		}
	}

	// Start is called before the first frame update
	void Awake(){
		_startPos = transform.position;
		_keyframes = _keyframes.OrderBy(a_keyframe => a_keyframe.Frame).ToList();
	}

	void Start() {
		GoToWaypoint(100);
	}

	public override void OnPresent() {
		//_anim.SetTrigger("Present"); o lo que sea
	}

	public override void GoToWaypoint(int a_desiredFrame)
	{
		for (int i = 0; i < _keyframes.Count; i++)
		{
			Keyframe keyframe = _keyframes[i];

			//Frame exacto deseado
			if (keyframe.Frame == a_desiredFrame)
			{
				transform.position = new Vector3((_startPos + keyframe.LocalPosition).x, (_startPos + keyframe.LocalPosition).y, (_startPos + keyframe.LocalPosition).y / 50f);
				break;
			}

			//Frame inferior al deseado
			if (keyframe.Frame < a_desiredFrame)
			{
				if (i != _keyframes.Count - 1) continue;
				//No hay más keyframes hacia adelante
				transform.position = new Vector3((_startPos + keyframe.LocalPosition).x, (_startPos + keyframe.LocalPosition).y, (_startPos + keyframe.LocalPosition).y / 50f);
				break;
			}

			//Frame superior al deseado
			Vector2 prevKeyframePos;
			int     prevKeyframeFrame;

			if (i == 0)
			{
				prevKeyframePos   = _startPos;
				prevKeyframeFrame = 0;
			}
			else
			{
				Keyframe prevKeyframe = _keyframes[i - 1];
				prevKeyframePos = new Vector3((_startPos + prevKeyframe.LocalPosition).x, (_startPos + prevKeyframe.LocalPosition).y, (_startPos + prevKeyframe.LocalPosition).y / 50f);
				prevKeyframeFrame = prevKeyframe.Frame;
			}

			float   t      = Mathf.InverseLerp(prevKeyframeFrame, keyframe.Frame, a_desiredFrame);
			Vector2 midPos = Vector2.Lerp(prevKeyframePos, new Vector3((_startPos + keyframe.LocalPosition).x, (_startPos + keyframe.LocalPosition).y, (_startPos + keyframe.LocalPosition).y / 50f), t);
			transform.position = midPos;
			break;
		}
	}

#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		Color initialColor = Color.blue;
		Color finalColor = Color.red;

		Vector2 pos = transform.position;
		Keyframe[] sortedKeyframes = _keyframes.OrderBy(a_keyframe => a_keyframe.Frame).ToArray();

		Vector2 lastPos = pos;
		int lastFrame = 0;

		foreach (Keyframe keyframe in sortedKeyframes)
		{
			Vector2 newPos   = pos + keyframe.LocalPosition;
			int     newFrame = keyframe.Frame;

			Gizmos.color = Color.green;
			Gizmos.DrawLine(lastPos, newPos);

			Gizmos.color = Color.yellow;
			for (int i = lastFrame + 1; i < newFrame; i++)
			{
				float   t      = Mathf.InverseLerp(lastFrame, newFrame, i);
				Vector2 midPos = Vector2.Lerp(lastPos, newPos, t);

				float miniSpherePercent = Mathf.InverseLerp(0, 100, i);
				Gizmos.color = Color.Lerp(initialColor, finalColor, miniSpherePercent);
				Gizmos.DrawSphere(midPos, .1f);
			}

			float percent = Mathf.InverseLerp(0, 100, keyframe.Frame);
			Gizmos.color = Color.Lerp(initialColor, finalColor, percent);
			Gizmos.DrawSphere(newPos, .25f);
			Handles.Label(newPos, _keyframes.IndexOf(keyframe).ToString());

			float   distance = Vector2.Distance(newPos, lastPos);
			GUI.contentColor = Color.red;
			if (distance > 0 && newFrame != lastFrame)
			{
				float   speed   = distance / (newFrame - lastFrame);
				Vector2 textPos = Vector2.Lerp(lastPos, newPos, 0.5f);
				textPos += Vector2.Perpendicular(newPos - lastPos).normalized * 1f;

				Handles.Label(textPos, speed + "Mpf");
			}

			lastPos   = newPos;
			lastFrame = newFrame;
		}
	}
#endif
}
