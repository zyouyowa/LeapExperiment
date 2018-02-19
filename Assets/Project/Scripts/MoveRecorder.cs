using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Project{
	public class MoveRecorder : MonoBehaviour {

		public string fileName;
		public bool recording{ get; private set; }

		private StreamWriter sw;
		public FileInfo fi{ get; private set; }

		void Start () {
			recording = false;
			fi = new FileInfo (Application.dataPath + "/" + fileName);
		}

		public void Record(string[] data){
			if(recording)
				sw.WriteLine (string.Join (",", data));
		}

		public void SwitchState(){
			if (!recording) {
				recording = true;
				sw = fi.CreateText ();
			} else {
				recording = false;
				sw.Flush();
				sw.Close ();
			}
		}

		void OnApplicationQuit(){
			try{
				sw.Flush ();
				sw.Close ();
			} catch (Exception e){
				Debug.Log (e);
			}

		}
	}
}
