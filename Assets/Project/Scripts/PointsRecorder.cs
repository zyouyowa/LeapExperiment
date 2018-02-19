using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project{
	[RequireComponent(typeof(MoveRecorder))]
	public class PointsRecorder : MonoBehaviour {

		MoveRecorder recorder;

		public CollisionInform[] collisions;

		public MovePlayer player;

		void Start(){
			recorder = GetComponent<MoveRecorder> ();
			recorder.SwitchState ();
		}

		void FixedUpdate(){
			if (player.playing) {
				int count = 0;
				foreach (var col in collisions) {
					count += (col.isColliding) ? 1 : 0;
				}
				recorder.Record (new string[]{count.ToString()});
			}
		}
	}
}
