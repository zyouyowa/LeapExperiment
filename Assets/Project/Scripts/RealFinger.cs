using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;

namespace Project{
	[RequireComponent(typeof(MoveRecorder))]
	[RequireComponent(typeof(MovePlayer))]
	public class RealFinger : MonoBehaviour {
		public Transform[] bones;
		public float[] boneWidths;
		public float[] boneLengths;

		private MoveRecorder recorder;
		private MovePlayer player;

		void Start(){
			recorder = GetComponent<MoveRecorder> ();
			player = GetComponent<MovePlayer> ();
		}

		public void FixedUpdateBones(Finger leapFinger){
			string[] data = new string[bones.Length];
			for (int b = 0; b < bones.Length; ++b) {
				boneWidths [b] = leapFinger.bones [b].Width;
				boneLengths [b] = leapFinger.bones [b].Length;
				if (bones [b] != null) {
					bones [b].position = leapFinger.bones [b].Center.ToVector3 ();
					bones [b].rotation = leapFinger.bones [b].Rotation.ToQuaternion ();
				}
				if (recorder.recording) {
					string[] oneBoneData = new string[9];
					if (bones [b] != null) {
						oneBoneData [0] = bones [b].position.x.ToString ();
						oneBoneData [1] = bones [b].position.y.ToString ();
						oneBoneData [2] = bones [b].position.z.ToString ();
						oneBoneData [3] = bones [b].rotation.x.ToString ();
						oneBoneData [4] = bones [b].rotation.y.ToString ();
						oneBoneData [5] = bones [b].rotation.z.ToString ();
						oneBoneData [6] = bones [b].rotation.w.ToString ();
					} else {

						oneBoneData [0] = "0";
						oneBoneData [1] = "0";
						oneBoneData [2] = "0";
						oneBoneData [3] = "0";
						oneBoneData [4] = "0";
						oneBoneData [5] = "0";
						oneBoneData [6] = "0";
					}

					oneBoneData [7] = boneWidths [b].ToString ();
					oneBoneData [8] = boneLengths [b].ToString ();
					data [b] = string.Join (",", oneBoneData);
				}
			}
			if (recorder.recording) {
				recorder.Record (data);
			}
		}

		public void FixedUpdateBones(){
			float[] data = player.OnFixedUpdate ();
			if (data == null) {
				return;
			}
			for (int b = 0; b < bones.Length; ++b) {
				if (bones [b] != null) {
					bones [b].position = new Vector3(data[0 + 9*b], data[1 + 9*b], data[2 + 9*b]);
					bones [b].rotation = new Quaternion (data [3 + 9*b], data [4 + 9*b], data [5 + 9*b], data [6 + 9*b]);
				}
				boneWidths [b] = data [7 + 9*b];
				boneLengths [b] = data [8 + 9*b];
			}
		}
	}
}