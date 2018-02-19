using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Project{
	public class RecordSwitch : MonoBehaviour {
		public MoveRecorder recorder;

		private Text text;

		void Start(){
			text = GetComponent<Text> ();
		}

		void Update () {
			text.text = recorder.recording ? "Stop Record" : "Start Record";
		}
	}
}
