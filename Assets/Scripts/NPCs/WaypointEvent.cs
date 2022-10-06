using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.Rendering.DebugUI;

[System.Serializable] public class WpEvent : UnityEvent<WpEvent> { }

public class WaypointEvent : WaypointEntity{

	[System.Serializable]
	public class Waypoint {
		public int frame;
		public WpEvent wpEvent;
		public Waypoint(int a_frame, WpEvent a_wpEvent) {
			frame = a_frame;
			wpEvent = a_wpEvent;
		}
	}
	[SerializeField] Waypoint[] waypoints;

	// Start is called before the first frame update
	void Start(){
		
	}

	// Update is called once per frame
	void Update() {

	}

	int lastIndex;
	public override void GoToWaypoint(int index) {
		int closestEvent = int.MaxValue, closestEventDifference = int.MaxValue;
		for (int i = 0; i < waypoints.Length; i++) {
			if(waypoints[i].frame <= index && waypoints[i].frame >= lastIndex || waypoints[i].frame >= index && waypoints[i].frame <= lastIndex){
				if (Mathf.Abs(waypoints[i].frame - index) < closestEventDifference) {
					closestEvent = i;
					closestEventDifference = Mathf.Abs(waypoints[i].frame - index);
				}
			}
		}

		if(closestEvent != int.MaxValue){
			waypoints[closestEvent].wpEvent.Invoke(waypoints[closestEvent].wpEvent);
		}
		lastIndex = index;
	}

	public override void OnPresent() {
		GoToWaypoint(100);
	}
}
