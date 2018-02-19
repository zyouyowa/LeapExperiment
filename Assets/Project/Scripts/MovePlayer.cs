using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Project{
	[RequireComponent(typeof(MoveRecorder))]
	public class MovePlayer : MonoBehaviour {

		MoveRecorder recorder;
		public bool playing{ get; private set; }

		private StreamReader sr;

		void Start () {
			recorder = GetComponent<MoveRecorder> ();

		}

		public float[] OnFixedUpdate(){
			if (playing) {
				string s = sr.ReadLine();
				if (s == null) {
					playing = false;
					Debug.Log ("Finish play movement");
					sr.Close ();
					return null;
				}
				string[] sdata = s.Split (',');
				float[] data = new float[sdata.Length];
				for (int i = 0; i < sdata.Length; i++) {
					data [i] = float.Parse (sdata [i]);
				}
				return data;
			}
			return null;
		}

		public void Play(){
			if (recorder.recording) {
				Debug.Log ("Now recording, so I cannot play");
				return;
			}
			playing = true;
			sr = recorder.fi.OpenText ();
			Debug.Log ("Play movement");
		}
	}

}
