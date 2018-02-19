using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project{
	[RequireComponent(typeof(MoveRecorder))]
	public class AngleRecorder : MonoBehaviour {

		MoveRecorder recorder;

		public Transform[] aR;
		public Transform[] bR;

		public Transform[] aV;
		public Transform[] bV;

		public MovePlayer player;

		void Start(){
			recorder = GetComponent<MoveRecorder> ();
			recorder.SwitchState ();
		}

		void FixedUpdate(){
			if (player.playing) {
				string[] angleDiffs = new string[aR.Length];
				for (int i = 0; i < aR.Length; i++) {
					float angleR = Vector3.Angle (aR[i].forward, bR[i].forward);
					float angleV = Vector3.Angle (aV[i].forward, bV[i].forward);
					angleDiffs [i] = Mathf.Abs (angleR - angleV).ToString ();
				}
				recorder.Record (angleDiffs);
			}
		}
	}
}
