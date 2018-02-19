using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;

namespace Project{
	public class SkinVC : MonoBehaviour {

		public bool drawGizmos = true;
		public bool pullBone = true;

		public SpringDamper posSD = new SpringDamper();
		public SpringDamper rotSD = new SpringDamper();

		public float kPosMax = 10000f;
		public float kPosMin = 1000f;
		public float angleMax = 10f;
		public float kRotMax = 1000f;
		public float kRotMin = 100f;

		CapsuleCollider capsuleCol;
		BoxCollider boxCol;
		public Rigidbody rb{ get; private set; }

		void Start () {
			capsuleCol = GetComponent<CapsuleCollider> ();
			boxCol = GetComponent<BoxCollider> ();
			rb = GetComponent<Rigidbody> ();
			rb.mass = posSD.m;
			rb.isKinematic = false;
			rb.maxAngularVelocity = Mathf.Infinity;
			rb.maxDepenetrationVelocity = Mathf.Infinity;
		}

		public void FixedUpdateSkin(Transform bone, Rigidbody boneBody, CapsuleCollider targetCol){
			capsuleCol.height = targetCol.height;
			capsuleCol.radius = targetCol.radius;
			float tPos = Mathf.Clamp01 (Vector3.Distance (bone.position, transform.position) / targetCol.radius);
			posSD.k = kPosMin + tPos * (kPosMax - kPosMin);
			float tRot = Mathf.Clamp01(Quaternion.Angle (bone.rotation, transform.rotation) / angleMax);
			rotSD.k = kRotMin + tRot * (kRotMax - kRotMin);
			FixedUpdateSkin (bone, boneBody, VCFinger.CapsuleI(targetCol.radius, targetCol.height, posSD.m));
		}

		public void FixedUpdateSkin(Transform bone, Rigidbody boneBody, BoxCollider targetCol){
			boxCol.size = targetCol.size;
			float tPos = Mathf.Clamp01 (Vector3.Distance (bone.position, transform.position) / (boxCol.size * 0.5f).magnitude);
			posSD.k = kPosMin + tPos * tPos * (kPosMax - kPosMin);
			float tRot = Mathf.Clamp01(Quaternion.Angle (bone.rotation, transform.rotation) / angleMax);
			rotSD.k = kRotMin + tRot * tRot * (kRotMax - kRotMin);
			FixedUpdateSkin (bone, boneBody, VCHand.BoxI(targetCol.size * 0.5f, posSD.m));
		}

		protected void FixedUpdateSkin(Transform bone, Rigidbody boneBody, Vector3 I){
			Vector3 skinForce = posSD.GetForce (transform.position - bone.position, rb.velocity);
			rb.AddForce (skinForce);

			Vector3 skinTorque = rotSD.GetTorque (transform.rotation, bone.rotation, rb.angularVelocity, I);
			rb.AddTorque (skinTorque);
			if (!pullBone)
				return;
			//バネお式考え直した方がよさそう、原点がごこなのか
			Vector3 boneForce = posSD.GetForce (bone.position - transform.position, boneBody.velocity);
			boneBody.AddForce (boneForce);
			Vector3 boneTorque = rotSD.GetTorque (bone.rotation, transform.rotation, boneBody.angularVelocity, I);
			boneBody.AddTorque (boneTorque);
		}
	}
}
