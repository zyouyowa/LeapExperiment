using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project{
	public class CollisionInform : MonoBehaviour {

		public bool isColliding{ get; private set; }

		void Start () {

		}

		void Update () {

		}

		void OnCollisionEnter(Collision col){
			if(col.gameObject.name == "Sphere"){
				isColliding = true;
			}
		}

		void OnCollisionExit(Collision col){
			if (col.gameObject.name == "Sphere") {
				isColliding = false;
			}
		}
	}
}