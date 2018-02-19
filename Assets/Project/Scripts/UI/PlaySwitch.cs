using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Project{
	public class PlaySwitch : MonoBehaviour {
		public MovePlayer player;

		private Text text;

		void Start(){
			text = GetComponent<Text> ();
		}

		void Update () {
			text.text = player.playing ? "NowPlaying" : "Start Play";
		}
	}
}
