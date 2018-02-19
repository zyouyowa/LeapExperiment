using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project{
	public class ShotSphere : MonoBehaviour {
		Vector3 startPos;
		Quaternion startRot;
		Rigidbody rb;

		void Start () {
			rb = GetComponent<Rigidbody> ();
			startPos = transform.position;
			startRot = transform.rotation;
		}

		public void Shot(){
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
			rb.position = startPos;
			rb.rotation = startRot;
		}
	}
}
