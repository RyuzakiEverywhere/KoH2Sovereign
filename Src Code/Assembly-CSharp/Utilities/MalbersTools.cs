using System;
using System.Collections;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace MalbersAnimations.Utilities
{
	// Token: 0x02000474 RID: 1140
	public static class MalbersTools
	{
		// Token: 0x06003B8C RID: 15244 RVA: 0x001C75BD File Offset: 0x001C57BD
		public static bool CollidersLayer(Collider collider, LayerMask layerMask)
		{
			return layerMask == (layerMask | 1 << collider.gameObject.layer);
		}

		// Token: 0x06003B8D RID: 15245 RVA: 0x001C75DE File Offset: 0x001C57DE
		public static bool Layer_in_LayerMask(int layer, LayerMask layerMask)
		{
			return layerMask == (layerMask | 1 << layer);
		}

		// Token: 0x06003B8E RID: 15246 RVA: 0x001C75F8 File Offset: 0x001C57F8
		public static T GetInstance<T>(string name) where T : ScriptableObject
		{
			return default(T);
		}

		// Token: 0x06003B8F RID: 15247 RVA: 0x001C7610 File Offset: 0x001C5810
		public static void DebugCross(Vector3 center, float radius, Color color)
		{
			Debug.DrawLine(center - new Vector3(0f, radius, 0f), center + new Vector3(0f, radius, 0f), color);
			Debug.DrawLine(center - new Vector3(radius, 0f, 0f), center + new Vector3(radius, 0f, 0f), color);
			Debug.DrawLine(center - new Vector3(0f, 0f, radius), center + new Vector3(0f, 0f, radius), color);
		}

		// Token: 0x06003B90 RID: 15248 RVA: 0x001C76B4 File Offset: 0x001C58B4
		public static void DebugPlane(Vector3 center, float radius, Color color, bool cross = false)
		{
			Debug.DrawLine(center - new Vector3(radius, 0f, 0f), center + new Vector3(0f, 0f, -radius), color);
			Debug.DrawLine(center - new Vector3(radius, 0f, 0f), center + new Vector3(0f, 0f, radius), color);
			Debug.DrawLine(center + new Vector3(0f, 0f, radius), center - new Vector3(-radius, 0f, 0f), color);
			Debug.DrawLine(center - new Vector3(0f, 0f, radius), center + new Vector3(radius, 0f, 0f), color);
			if (cross)
			{
				Debug.DrawLine(center - new Vector3(radius, 0f, 0f), center + new Vector3(radius, 0f, 0f), color);
				Debug.DrawLine(center - new Vector3(0f, 0f, radius), center + new Vector3(0f, 0f, radius), color);
			}
		}

		// Token: 0x06003B91 RID: 15249 RVA: 0x001C77F4 File Offset: 0x001C59F4
		public static void DebugTriangle(Vector3 center, float radius, Color color)
		{
			Debug.DrawLine(center - new Vector3(radius, 0f, 0f), center + new Vector3(radius, 0f, 0f), color);
			Debug.DrawLine(center - new Vector3(0f, 0f, radius), center + new Vector3(0f, 0f, radius), color);
			Debug.DrawLine(center - new Vector3(0f, -radius, 0f), center + new Vector3(radius, 0f, 0f), color);
			Debug.DrawLine(center - new Vector3(0f, -radius, 0f), center + new Vector3(-radius, 0f, 0f), color);
			Debug.DrawLine(center - new Vector3(0f, -radius, 0f), center + new Vector3(0f, 0f, radius), color);
			Debug.DrawLine(center - new Vector3(0f, -radius, 0f), center + new Vector3(0f, 0f, -radius), color);
			Debug.DrawLine(center - new Vector3(radius, 0f, 0f), center + new Vector3(0f, 0f, -radius), color);
			Debug.DrawLine(center - new Vector3(radius, 0f, 0f), center + new Vector3(0f, 0f, radius), color);
			Debug.DrawLine(center + new Vector3(0f, 0f, radius), center - new Vector3(-radius, 0f, 0f), color);
			Debug.DrawLine(center - new Vector3(0f, 0f, radius), center + new Vector3(radius, 0f, 0f), color);
		}

		// Token: 0x06003B92 RID: 15250 RVA: 0x001C7A00 File Offset: 0x001C5C00
		public static void SetLayer(Transform root, int layer)
		{
			root.gameObject.layer = layer;
			foreach (object obj in root)
			{
				MalbersTools.SetLayer((Transform)obj, layer);
			}
		}

		// Token: 0x06003B93 RID: 15251 RVA: 0x001C7A60 File Offset: 0x001C5C60
		public static Vector3 DirectionTarget(Transform origin, Transform Target, bool normalized = true)
		{
			if (normalized)
			{
				return (Target.position - origin.position).normalized;
			}
			return Target.position - origin.position;
		}

		// Token: 0x06003B94 RID: 15252 RVA: 0x001C7A9C File Offset: 0x001C5C9C
		public static string Serialize<T>(this T toSerialize)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
			StringWriter stringWriter = new StringWriter();
			xmlSerializer.Serialize(stringWriter, toSerialize);
			return stringWriter.ToString();
		}

		// Token: 0x06003B95 RID: 15253 RVA: 0x001C7AD0 File Offset: 0x001C5CD0
		public static bool IsBitActive(int IntValue, int index)
		{
			return (IntValue & 1 << index) != 0;
		}

		// Token: 0x06003B96 RID: 15254 RVA: 0x001C7AE0 File Offset: 0x001C5CE0
		public static T Deserialize<T>(this string toDeserialize)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
			StringReader textReader = new StringReader(toDeserialize);
			return (T)((object)xmlSerializer.Deserialize(textReader));
		}

		// Token: 0x06003B97 RID: 15255 RVA: 0x001C7B10 File Offset: 0x001C5D10
		public static Vector3 DirectionTarget(Vector3 origin, Vector3 Target, bool normalized = true)
		{
			if (normalized)
			{
				return (Target - origin).normalized;
			}
			return Target - origin;
		}

		// Token: 0x06003B98 RID: 15256 RVA: 0x001C7B38 File Offset: 0x001C5D38
		public static float HorizontalAngle(Vector3 From, Vector3 To, Vector3 Up)
		{
			float num = Mathf.Atan2(Vector3.Dot(Up, Vector3.Cross(From, To)), Vector3.Dot(From, To));
			num *= 57.29578f;
			if (Mathf.Abs(num) < 0.0001f)
			{
				num = 0f;
			}
			return num;
		}

		// Token: 0x06003B99 RID: 15257 RVA: 0x001C7B7C File Offset: 0x001C5D7C
		public static Vector3 DirectionFromCamera(Transform origin, float x, float y, out RaycastHit hit, LayerMask hitmask)
		{
			Camera main = Camera.main;
			hit = default(RaycastHit);
			Ray ray = main.ScreenPointToRay(new Vector2(x * (float)main.pixelWidth, y * (float)main.pixelHeight));
			Vector3 result = ray.direction;
			hit.distance = float.MaxValue;
			foreach (RaycastHit raycastHit in Physics.RaycastAll(ray, 100f, hitmask))
			{
				if (!(raycastHit.transform.root == origin.transform.root) && Vector3.Distance(main.transform.position, raycastHit.point) >= Vector3.Distance(main.transform.position, origin.position) && hit.distance > raycastHit.distance)
				{
					hit = raycastHit;
				}
			}
			if (hit.distance != 3.4028235E+38f)
			{
				result = (hit.point - origin.position).normalized;
			}
			return result;
		}

		// Token: 0x06003B9A RID: 15258 RVA: 0x001C7C88 File Offset: 0x001C5E88
		public static Vector3 DirectionFromCamera(Transform origin, Vector3 ScreenPoint, out RaycastHit hit, LayerMask hitmask)
		{
			Camera main = Camera.main;
			Ray ray = main.ScreenPointToRay(ScreenPoint);
			Vector3 result = ray.direction;
			hit = new RaycastHit
			{
				distance = float.MaxValue,
				point = ray.GetPoint(100f)
			};
			foreach (RaycastHit raycastHit in Physics.RaycastAll(ray, 100f, hitmask))
			{
				if (!(raycastHit.transform.root == origin.transform.root) && Vector3.Distance(main.transform.position, raycastHit.point) >= Vector3.Distance(main.transform.position, origin.position) && hit.distance > raycastHit.distance)
				{
					hit = raycastHit;
				}
			}
			if (hit.distance != 3.4028235E+38f)
			{
				result = (hit.point - origin.position).normalized;
			}
			return result;
		}

		// Token: 0x06003B9B RID: 15259 RVA: 0x001C7D98 File Offset: 0x001C5F98
		public static Vector3 DirectionFromCamera(Transform origin)
		{
			RaycastHit raycastHit;
			return MalbersTools.DirectionFromCamera(origin, 0.5f * (float)Screen.width, 0.5f * (float)Screen.height, out raycastHit, -1);
		}

		// Token: 0x06003B9C RID: 15260 RVA: 0x001C7DCC File Offset: 0x001C5FCC
		public static Vector3 DirectionFromCamera(Transform origin, LayerMask layerMask)
		{
			RaycastHit raycastHit;
			return MalbersTools.DirectionFromCamera(origin, 0.5f * (float)Screen.width, 0.5f * (float)Screen.height, out raycastHit, layerMask);
		}

		// Token: 0x06003B9D RID: 15261 RVA: 0x001C7DFC File Offset: 0x001C5FFC
		public static Vector3 DirectionFromCamera(Transform origin, Vector3 ScreenCenter)
		{
			RaycastHit raycastHit;
			return MalbersTools.DirectionFromCamera(origin, ScreenCenter, out raycastHit, -1);
		}

		// Token: 0x06003B9E RID: 15262 RVA: 0x001C7E18 File Offset: 0x001C6018
		public static RaycastHit RayCastHitToCenter(Transform origin, Vector3 ScreenCenter, int layerMask = -1)
		{
			Camera main = Camera.main;
			RaycastHit result = default(RaycastHit);
			Ray ray = main.ScreenPointToRay(ScreenCenter);
			Vector3 direction = ray.direction;
			result.distance = float.MaxValue;
			foreach (RaycastHit raycastHit in Physics.RaycastAll(ray, 100f, layerMask))
			{
				if (!(raycastHit.transform.root == origin.transform.root) && Vector3.Distance(main.transform.position, raycastHit.point) >= Vector3.Distance(main.transform.position, origin.position) && result.distance > raycastHit.distance)
				{
					result = raycastHit;
				}
			}
			return result;
		}

		// Token: 0x06003B9F RID: 15263 RVA: 0x001C7EDC File Offset: 0x001C60DC
		public static Vector3 DirectionFromCameraNoRayCast(Vector3 ScreenCenter)
		{
			return Camera.main.ScreenPointToRay(ScreenCenter).direction;
		}

		// Token: 0x06003BA0 RID: 15264 RVA: 0x001C7EFC File Offset: 0x001C60FC
		public static RaycastHit RayCastHitToCenter(Transform origin)
		{
			return MalbersTools.RayCastHitToCenter(origin, new Vector3(0.5f * (float)Screen.width, 0.5f * (float)Screen.height), -1);
		}

		// Token: 0x06003BA1 RID: 15265 RVA: 0x001C7F22 File Offset: 0x001C6122
		public static RaycastHit RayCastHitToCenter(Transform origin, LayerMask layerMask)
		{
			return MalbersTools.RayCastHitToCenter(origin, new Vector3(0.5f * (float)Screen.width, 0.5f * (float)Screen.height), layerMask);
		}

		// Token: 0x06003BA2 RID: 15266 RVA: 0x001C7F4D File Offset: 0x001C614D
		public static RaycastHit RayCastHitToCenter(Transform origin, int layerMask)
		{
			return MalbersTools.RayCastHitToCenter(origin, new Vector3(0.5f * (float)Screen.width, 0.5f * (float)Screen.height), layerMask);
		}

		// Token: 0x06003BA3 RID: 15267 RVA: 0x001C7F74 File Offset: 0x001C6174
		public static float AngleAroundAxis(Vector3 dirA, Vector3 dirB, Vector3 axis)
		{
			dirA -= Vector3.Project(dirA, axis);
			dirB -= Vector3.Project(dirB, axis);
			return Vector3.Angle(dirA, dirB) * (float)((Vector3.Dot(axis, Vector3.Cross(dirA, dirB)) < 0f) ? -1 : 1);
		}

		// Token: 0x06003BA4 RID: 15268 RVA: 0x001C7FC0 File Offset: 0x001C61C0
		public static Vector3 ClosestPointOnLine(Vector3 vA, Vector3 vB, Vector3 vPoint)
		{
			Vector3 rhs = vPoint - vA;
			Vector3 normalized = (vB - vA).normalized;
			float num = Vector3.Distance(vA, vB);
			float num2 = Vector3.Dot(normalized, rhs);
			if (num2 <= 0f)
			{
				return vA;
			}
			if (num2 >= num)
			{
				return vB;
			}
			Vector3 b = normalized * num2;
			return vA + b;
		}

		// Token: 0x06003BA5 RID: 15269 RVA: 0x001C8017 File Offset: 0x001C6217
		public static IEnumerator AlignTransform_Position(Transform t1, Vector3 NewPosition, float time, AnimationCurve curve = null)
		{
			float elapsedTime = 0f;
			Vector3 CurrentPos = t1.position;
			while (time > 0f && elapsedTime <= time)
			{
				float t2 = (curve != null) ? curve.Evaluate(elapsedTime / time) : (elapsedTime / time);
				t1.position = Vector3.LerpUnclamped(CurrentPos, NewPosition, t2);
				elapsedTime += Time.deltaTime;
				yield return null;
			}
			t1.position = NewPosition;
			yield break;
		}

		// Token: 0x06003BA6 RID: 15270 RVA: 0x001C803B File Offset: 0x001C623B
		public static IEnumerator AlignTransform_Rotation(Transform t1, Quaternion NewRotation, float time, AnimationCurve curve = null)
		{
			float elapsedTime = 0f;
			Quaternion CurrentRot = t1.rotation;
			while (time > 0f && elapsedTime <= time)
			{
				float t2 = (curve != null) ? curve.Evaluate(elapsedTime / time) : (elapsedTime / time);
				t1.rotation = Quaternion.LerpUnclamped(CurrentRot, NewRotation, t2);
				elapsedTime += Time.deltaTime;
				yield return null;
			}
			t1.rotation = NewRotation;
			yield break;
		}

		// Token: 0x06003BA7 RID: 15271 RVA: 0x001C8060 File Offset: 0x001C6260
		public static Vector3 Quaternion_to_AngularVelocity(Quaternion quaternion)
		{
			float d;
			Vector3 a;
			quaternion.ToAngleAxis(out d, out a);
			return a * d * 0.017453292f / Time.deltaTime;
		}

		// Token: 0x06003BA8 RID: 15272 RVA: 0x001C8093 File Offset: 0x001C6293
		public static IEnumerator AlignTransformsC(Transform t1, Transform t2, float time, bool Position = true, bool Rotation = true, AnimationCurve curve = null)
		{
			float elapsedTime = 0f;
			Vector3 CurrentPos = t1.position;
			Quaternion CurrentRot = t1.rotation;
			while (time > 0f && elapsedTime <= time)
			{
				float t3 = (curve != null) ? curve.Evaluate(elapsedTime / time) : (elapsedTime / time);
				if (Position)
				{
					t1.position = Vector3.LerpUnclamped(CurrentPos, t2.position, t3);
				}
				if (Rotation)
				{
					t1.rotation = Quaternion.SlerpUnclamped(CurrentRot, t2.rotation, t3);
				}
				elapsedTime += Time.deltaTime;
				yield return null;
			}
			if (Position)
			{
				t1.position = t2.position;
			}
			if (Rotation)
			{
				t1.rotation = t2.rotation;
			}
			yield break;
		}

		// Token: 0x06003BA9 RID: 15273 RVA: 0x001C80C7 File Offset: 0x001C62C7
		public static IEnumerator AlignTransformsC(Transform t1, Quaternion rotation, float time, AnimationCurve curve = null)
		{
			float elapsedTime = 0f;
			Quaternion CurrentRot = t1.rotation;
			while (time > 0f && elapsedTime <= time)
			{
				float t2 = (curve != null) ? curve.Evaluate(elapsedTime / time) : (elapsedTime / time);
				t1.rotation = Quaternion.SlerpUnclamped(CurrentRot, rotation, t2);
				elapsedTime += Time.deltaTime;
				yield return null;
			}
			t1.rotation = rotation;
			yield break;
		}

		// Token: 0x06003BAA RID: 15274 RVA: 0x001C80EB File Offset: 0x001C62EB
		public static IEnumerator AlignLookAtTransform(Transform t1, Transform t2, float time, AnimationCurve curve = null)
		{
			float elapsedTime = 0f;
			Quaternion CurrentRot = t1.rotation;
			Vector3 normalized = (t2.position - t1.position).normalized;
			normalized.y = t1.forward.y;
			Quaternion FinalRot = Quaternion.LookRotation(normalized);
			while (time > 0f && elapsedTime <= time)
			{
				float t3 = (curve != null) ? curve.Evaluate(elapsedTime / time) : (elapsedTime / time);
				t1.rotation = Quaternion.SlerpUnclamped(CurrentRot, FinalRot, t3);
				elapsedTime += Time.deltaTime;
				yield return null;
			}
			t1.rotation = FinalRot;
			yield break;
		}

		// Token: 0x06003BAB RID: 15275 RVA: 0x001C8110 File Offset: 0x001C6310
		public static bool FindAnimatorParameter(Animator animator, AnimatorControllerParameterType type, string ParameterName)
		{
			foreach (AnimatorControllerParameter animatorControllerParameter in animator.parameters)
			{
				if (animatorControllerParameter.type == type && animatorControllerParameter.name == ParameterName)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06003BAC RID: 15276 RVA: 0x001C8150 File Offset: 0x001C6350
		public static bool FindAnimatorParameter(Animator animator, AnimatorControllerParameterType type, int hash)
		{
			foreach (AnimatorControllerParameter animatorControllerParameter in animator.parameters)
			{
				if (animatorControllerParameter.type == type && animatorControllerParameter.nameHash == hash)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x04002B51 RID: 11089
		public static Vector3 NullVector = new Vector3(float.MinValue, float.MinValue, float.MinValue);
	}
}
