using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project{
	[RequireComponent(typeof(MoveRecorder))]
	public class RelativePosRecorder : MonoBehaviour {

		MoveRecorder recorder;
		public Transform hand;
		public Transform sphere;

		public MovePlayer player;

		void Start(){
			recorder = GetComponent<MoveRecorder> ();
			recorder.SwitchState ();
		}

		void FixedUpdate(){
			if (player.playing) {
				float dist = Vector3.Distance (hand.position, sphere.position);
				recorder.Record (new string[]{dist.ToString()});
			}
		}
	}
}
