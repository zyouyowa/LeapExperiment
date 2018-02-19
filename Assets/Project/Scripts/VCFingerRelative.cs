using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;

namespace Project{
	public class VCFingerRelative: VCFinger {
		//位置がずれる、なぜ....
		public override void FixedUpdateBones(RealHand targetHand, VCHand vcHand, int f) {
			RealFinger finger = targetHand.fingers[f];

			Vector3 targetLocalPos = finger.bones [bones.Length - 1].position - targetHand.palm.position;
			Vector3 localPos = bones [bones.Length - 1].position - targetHand.palm.position;
			Vector3 localVelocity = boneBodys [bones.Length - 1].velocity - vcHand.palmBody.velocity; 
			Vector3 force = posSD.GetForce (localPos - targetLocalPos, localVelocity);

			CapsuleCollider boneCol = boneCols [bones.Length - 1];
			Vector3 I = CapsuleI (boneCol.radius, boneCol.height, posSD.m);
			Quaternion targetLocalRot = finger.bones [bones.Length - 1].rotation * Quaternion.Inverse (targetHand.palm.rotation);
			Quaternion boneLocalRot = bones[bones.Length-1].rotation * Quaternion.Inverse (targetHand.palm.rotation);
			Vector3 localAngularVelocity = boneBodys [bones.Length - 1].angularVelocity - vcHand.palmBody.angularVelocity;
			Vector3 torque = rotSD.GetTorque (boneLocalRot, targetLocalRot, localAngularVelocity, I);

			MoveBones (vcHand, finger, force, torque);
		}
	}
}