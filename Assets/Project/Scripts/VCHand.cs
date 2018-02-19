using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;

namespace Project{
	public class VCHand : MonoBehaviour {
		public bool drawGizmos = true;

		public SpringDamper posSD = new SpringDamper();
		public SpringDamper rotSD = new SpringDamper();

		public Transform palm;
		public Rigidbody palmBody{ get; private set; }
		public BoxCollider palmCol{ get; private set; }
		public VCFinger[] vcFingers = new VCFinger[5];

		public bool useSkin = false;
		public SkinVC palmSkin;

		public RealHand targetHand;

		void Start(){
			palmBody = palm.GetComponent<Rigidbody> ();
			palmBody.mass = posSD.m;
			palmCol = palm.GetComponent<BoxCollider> ();
			palmBody.isKinematic = false;
			palmBody.maxAngularVelocity = Mathf.Infinity;
			palmBody.maxDepenetrationVelocity = Mathf.Infinity;
		}

		public static Vector3 BoxI(Vector3 halfSize, float m){
			Vector3 halfPow2 = new Vector3 (
				halfSize.x * halfSize.x,
				halfSize.y * halfSize.y,
				halfSize.z * halfSize.z
			);
			//直方体の慣性モーメント
			return new Vector3 (
				halfPow2.y + halfPow2.z,
				halfPow2.x + halfPow2.z,
				halfPow2.x + halfPow2.y
			) * m / 3f;
		}

		protected virtual void FixedUpdate (){
			//追従対象の手がないときは非アクティブにする
			palm.gameObject.SetActive (targetHand.palm.gameObject.activeSelf);
			for (int f = 0; f < vcFingers.Length; ++f) {
				if (vcFingers [f] != null) {
					vcFingers [f].gameObject.SetActive (targetHand.palm.gameObject.activeSelf);
				}
			}
			//追従対象がないときは何もしない
			if (!targetHand.palm.gameObject.activeSelf) {
				return;
			}

			//手の追従
			if (palm != null && palmBody) {
				//位置のバネダンパ
				Vector3 displacement = palm.position - targetHand.palm.position;
				//Debug.DrawLine (palm.position, hand.PalmPosition.ToVector3 (), Color.red, Time.fixedDeltaTime);
				Vector3 force = posSD.GetForce (displacement, palmBody.velocity);
				palmBody.AddForce (force);

				Vector3 I = BoxI(palmCol.size * 0.5f, posSD.m);
				//姿勢のバネダンパ
				Quaternion targetRot = targetHand.palm.rotation;
				Quaternion palmRot = palm.rotation;
				Vector3 torque = rotSD.GetTorque (palmRot, targetRot, palmBody.angularVelocity, I);
				palmBody.AddTorque (torque);

				if (useSkin) {
					palmSkin.FixedUpdateSkin (palm, palmBody, palmCol);
				}
			}

			//指を動かす
			for (int f = 0; f < vcFingers.Length; ++f) {
				if (vcFingers [f] != null) {
					vcFingers [f].FixedUpdateBones (targetHand, this, f);
				}
			}
		}

		void OnDrawGizmos(){
			if (!drawGizmos) {
				return;
			}

			if (palmCol == null) {
				if (palm == null) {
					return;
				}
				palmCol = palm.GetComponent<BoxCollider> ();
			}

			DebugExtension.DrawLocalCube (palm, palmCol.size, Color.red, palmCol.center);
		}
	}
}
