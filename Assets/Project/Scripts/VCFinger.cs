using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;

namespace Project{
	public class VCFinger : MonoBehaviour {

		public bool drawGizmos = true;

		public SpringDamper posSD = new SpringDamper();
		public SpringDamper rotSD = new SpringDamper();

		//0番目は何もいれない
		public Transform[] bones = new Transform[4];
		public SkinVC[] fingerSkins = new SkinVC[4];
		//public Transform[] joints = new Transform[3];

		public Rigidbody[] boneBodys{ get; private set; }
		public CapsuleCollider[] boneCols{ get; private set; }
		public HingeJoint[] boneJoints{ get; private set; }
		public HingeJoint[] baseJoints{ get; private set; }
		//private Rigidbody[] jointBodys;



		void Start(){
			baseJoints = transform.Find ("joint").GetComponents<HingeJoint> ();

			boneBodys = new Rigidbody[bones.Length];
			boneCols = new CapsuleCollider[bones.Length];
			boneJoints = new HingeJoint[bones.Length];
			for (int b = 0; b < boneBodys.Length; ++b) {
				if (bones [b] != null) {
					boneJoints [b] = bones [b].GetComponent<HingeJoint> ();
					boneCols [b] = bones [b].GetComponent<CapsuleCollider> ();
					boneBodys [b] = bones [b].GetComponent<Rigidbody> ();
					boneBodys [b].mass = posSD.m;
					boneBodys [b].isKinematic = false;
					boneBodys [b].maxAngularVelocity = Mathf.Infinity;
					boneBodys [b].maxDepenetrationVelocity = Mathf.Infinity;
				}
			}

			/*
			jointBodys = new Rigidbody[joints.Length];
			for (int j = 0; j < jointBodys.Length; ++j) {
				if (joints [j] != null) {
					jointBodys [j] = joints [j].GetComponent<Rigidbody> ();
				}
			}
			*/
		}

		protected void MoveBones(VCHand vcHand, RealFinger finger, Vector3 force, Vector3 torque){
			boneBodys [bones.Length-1].AddForce (force);
			boneBodys[bones.Length-1].AddTorque (torque);

			for (int b = 0; b < bones.Length; ++b) {
				if (bones [b] != null && boneBodys [b]) {
					boneCols [b].radius = finger.boneWidths[b] / 2f;
					boneCols [b].height = finger.boneLengths[b] + finger.boneWidths [b];


					if (vcHand.useSkin && fingerSkins[b]) {
						fingerSkins[b].FixedUpdateSkin (bones[b], boneBodys[b], boneCols[b]);
					}

					/*
					Vector3 displacement = bones [b].position - finger.bones [b].Center.ToVector3 ();
					//Debug.DrawRay (finger.bones [b].Center.ToVector3 (), finger.bones[b].Direction.ToVector3(), Color.green, Time.fixedDeltaTime);
					Vector3 force = posSD.GetForce (displacement, boneBodys [b].velocity);
					boneBodys [b].AddForce (force);

					Quaternion targetRot = finger.bones[b].Rotation.ToQuaternion();
					Quaternion boneRot = bones[b].rotation;
					Vector3 torque = rotSD.GetTorque (boneRot, targetRot, boneBodys[b].angularVelocity);
					boneBodys[b].AddTorque (torque);
					*/

					//回転軸の描画
					/*
					if (b >= 2) {//Element0は空なので
						float axisStartZ = -boneCols [b].height / 2f + boneCols [b].radius;
						Vector3 start = bones [b].position + bones [b].forward * axisStartZ;
						Vector3 rotAxis = Vector3.Cross (bones [b].forward, bones [b - 1].forward);
						Debug.DrawRay (start, rotAxis, Color.red);
					}
					*/
				}
			}
		}
			
		public static Vector3 CapsuleI(float r, float h, float m){
			float rP2 = r * r;
			//円柱の体積
			float v1 = Mathf.PI * rP2 * h;
			//半球の体積
			float v2 = 2 / 3 * Mathf.PI * rP2 * r;

			//円柱の質量
			float m1 = v1 * m / (v1 + v2);
			//半球お質量
			float m2 = v2 * m / (v1 + v2);

			//x, y軸を回転軸としたときの慣性モーメント
			//円柱
			float Ix1 = (rP2 * 0.25f + h * h / 12f) * m1;
			//半球二つ
			float Ix2 = (2357 / 13120 * rP2 + r * h * 0.375f + h * h * 0.25f) * m2;
			//カプセル
			float Ix = Ix1 + Ix2;

			//z軸を回転軸としたときの慣性モーメント
			float Iz = m1 * rP2 * 0.5f + m2 * rP2 * 0.8f;

			return new Vector3 (
				Ix,
				Ix,
				Iz
			);
		}

		public virtual void FixedUpdateBones(RealHand targetHand, VCHand vcHand, int f) {
			RealFinger finger = targetHand.fingers[f];

			Vector3 displacement = bones [bones.Length - 1].position - finger.bones [bones.Length - 1].position;
			Vector3 force = posSD.GetForce (displacement, boneBodys [bones.Length-1].velocity);

			CapsuleCollider boneCol = boneCols [bones.Length - 1];
			Vector3 I = CapsuleI (boneCol.radius, boneCol.height, posSD.m);

			Quaternion targetRot = finger.bones[bones.Length-1].rotation;
			Quaternion boneRot = bones[bones.Length-1].rotation;
			Vector3 torque = rotSD.GetTorque (boneRot, targetRot, boneBodys[bones.Length-1].angularVelocity, I);

			MoveBones (vcHand, finger, force, torque);
		}

		void OnDrawGizmos(){
			if (!drawGizmos)
				return;
			
			if (boneCols == null) {
				boneCols = new CapsuleCollider[bones.Length];
			}
			for (int b = 0; b < bones.Length; ++b) {
				if (bones [b] == null) {
					continue;
				}
				if (boneCols [b] == null) {
					boneCols [b] = bones [b].GetComponent<CapsuleCollider> ();
				} else {
					Vector3 direction = bones[b].forward;
					switch (boneCols [b].direction) {
						case 0: direction = bones [b].right; break;
						case 1: direction = bones [b].up; break;
						case 2: direction = bones [b].forward; break;
					}
					Vector3 center = boneCols [b].center + bones [b].transform.position;
					Vector3 half = direction * boneCols [b].height * 0.5f;
					DebugExtension.DrawCapsule (center + half, center - half, Color.red, boneCols [b].radius);
				}
			}
		}
	}
}