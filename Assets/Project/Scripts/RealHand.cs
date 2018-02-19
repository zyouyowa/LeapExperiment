using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;

namespace Project{
	[RequireComponent(typeof(MoveRecorder))]
	[RequireComponent(typeof(MovePlayer))]
	public class RealHand : MonoBehaviour {
		public bool directConnect = true;

		public CapsuleHand capsuleHand;
		public Transform palm;
		public RealFinger[] fingers;

		private MoveRecorder recorder;
		private MovePlayer player;

		void Start(){
			recorder = GetComponent<MoveRecorder> ();
			player = GetComponent<MovePlayer> ();
		}

		void FixedUpdate(){
			if (directConnect) {
				//追従対象の手がないときは非アクティブにする
				palm.gameObject.SetActive (capsuleHand.gameObject.activeSelf);
				for (int f = 0; f < fingers.Length; ++f) {
					if (fingers [f] != null) {
						fingers [f].gameObject.SetActive (capsuleHand.gameObject.activeSelf);
					}
				}
				//追従対象がないときは何もしない
				if (!capsuleHand.gameObject.activeSelf) {
					return;
				}

				Hand leapHand = capsuleHand.GetLeapHand ();
				palm.position = leapHand.PalmPosition.ToVector3 ();
				palm.rotation = leapHand.Rotation.ToQuaternion ();

				if (recorder.recording) {
					string[] data = { 
						palm.position.x.ToString(),
						palm.position.y.ToString(),
						palm.position.z.ToString(),
						palm.rotation.x.ToString(),
						palm.rotation.y.ToString(),
						palm.rotation.z.ToString(),
						palm.rotation.w.ToString()
					};
					recorder.Record (data);
				}

				for (int f = 0; f < fingers.Length; ++f) {
					if (fingers [f] != null) {
						fingers [f].FixedUpdateBones (leapHand.Fingers [f]);
					}
				}
			} else {
				//再生してないときは非アクティブにする
				palm.gameObject.SetActive (player.playing);
				for (int f = 0; f < fingers.Length; ++f) {
					if (fingers [f] != null) {
						fingers [f].gameObject.SetActive (player.playing);
					}
				}
				if (!player.playing) {
					return;
				}

				float[] data = player.OnFixedUpdate ();
				if (data == null) {
					return;
				}

				palm.position = new Vector3 (data [0], data [1], data [2]);
				palm.rotation = new Quaternion (data [3], data [4], data [5], data [6]);

				for (int f = 0; f < fingers.Length; ++f) {
					if (fingers [f] != null) {
						fingers [f].FixedUpdateBones ();
					}
				}
			}
		}
	}
}