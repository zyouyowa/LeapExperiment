using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project{
	[System.Serializable]
	public class SpringDamper{
		public float zeta = 0.707f;
		public float k = 1000f;
		public float m = 0.3f;

		public Vector3 GetForce(Vector3 springLocalPos, Vector3 velocity){
			float c = zeta * 2 * Mathf.Sqrt (m * k);
			return -k * springLocalPos - c * velocity;
		}

		public Vector3 GetTorque(Quaternion start, Quaternion target, Vector3 angularVelocity, Vector3 I){
			Quaternion rot = target * Quaternion.Inverse (start);
			if (rot.w < 0f) {
				rot.x = -rot.x;
				rot.y = -rot.y;
				rot.z = -rot.z;
				rot.w = -rot.w;
			}
			Vector3 springLocalAngle = rot.eulerAngles;
			if (springLocalAngle.x >= 180f)
				springLocalAngle.x -= 360f;
			if (springLocalAngle.y >= 180f)
				springLocalAngle.y -= 360f;
			if (springLocalAngle.z >= 180f)
				springLocalAngle.z -= 360f;
			//Debug.LogFormat("theta:{0}, omega:{1}", springLocalAngle, angularVelocity);
			//springLocalAngleは負の方向になっているので-kではなくkをかける
			Vector3 cv = zeta * 2 * new Vector3 (
				             Mathf.Sqrt (I.x * k) * angularVelocity.x,
				             Mathf.Sqrt (I.y * k) * angularVelocity.y,
				             Mathf.Sqrt (I.z * k) * angularVelocity.z
			             );
			return k * springLocalAngle - cv;
		}

		/*
		public Vector3 GetMovement(Vector3 springOrigin, Vector3 position){
			Vector3 displacement = position + springOrigin;
			Vector3 springEffect = -k / m * Time.deltaTime * Time.deltaTime * springOrigin;
			Vector3 damperEffect = -c / m * Time.deltaTime * springOrigin;
			return springEffect + damperEffect;
		}

		public Quaternion GetRotation(Quaternion springOrigin, Quaternion rotation){
			Quaternion displacement = rotation * springOrigin;
			Vector3 springEffect = -k / m * Time.deltaTime * Time.deltaTime * springOrigin.eulerAngles;
			Vector3 damperEffect = -c / m * Time.deltaTime * springOrigin.eulerAngles;
			return Quaternion.Euler (springEffect + damperEffect);
		}
		*/
	}
}
