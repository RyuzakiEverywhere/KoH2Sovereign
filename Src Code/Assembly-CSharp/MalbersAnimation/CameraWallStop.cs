using System;
using System.Collections;
using MalbersAnimations.Utilities;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x020003E3 RID: 995
	public class CameraWallStop : MonoBehaviour
	{
		// Token: 0x17000342 RID: 834
		// (get) Token: 0x06003790 RID: 14224 RVA: 0x001B81DA File Offset: 0x001B63DA
		// (set) Token: 0x06003791 RID: 14225 RVA: 0x001B81E2 File Offset: 0x001B63E2
		public bool protecting { get; private set; }

		// Token: 0x06003792 RID: 14226 RVA: 0x001B81EC File Offset: 0x001B63EC
		private void Start()
		{
			this.m_Cam = base.GetComponentInChildren<Camera>().transform;
			this.m_Pivot = this.m_Cam.parent;
			this.m_OriginalDist = this.m_Cam.localPosition.magnitude;
			this.m_CurrentDist = this.m_OriginalDist;
			this.m_RayHitComparer = new CameraWallStop.RayHitComparer();
		}

		// Token: 0x06003793 RID: 14227 RVA: 0x001B824C File Offset: 0x001B644C
		private void LateUpdate()
		{
			float num = this.m_OriginalDist;
			this.m_Ray.origin = this.m_Pivot.position + this.m_Pivot.forward * this.sphereCastRadius;
			this.m_Ray.direction = -this.m_Pivot.forward;
			Collider[] array = Physics.OverlapSphere(this.m_Ray.origin, this.sphereCastRadius);
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].isTrigger && !MalbersTools.CollidersLayer(array[i], this.dontClip))
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				this.m_Ray.origin = this.m_Ray.origin + this.m_Pivot.forward * this.sphereCastRadius;
				this.hits = Physics.RaycastAll(this.m_Ray, this.m_OriginalDist - this.sphereCastRadius);
			}
			else
			{
				this.hits = Physics.SphereCastAll(this.m_Ray, this.sphereCastRadius, this.m_OriginalDist + this.sphereCastRadius);
			}
			Array.Sort(this.hits, this.m_RayHitComparer);
			float num2 = float.PositiveInfinity;
			for (int j = 0; j < this.hits.Length; j++)
			{
				if (this.hits[j].distance < num2 && !this.hits[j].collider.isTrigger && !MalbersTools.CollidersLayer(this.hits[j].collider, this.dontClip))
				{
					num2 = this.hits[j].distance;
					num = -this.m_Pivot.InverseTransformPoint(this.hits[j].point).z;
					flag2 = true;
				}
			}
			if (flag2)
			{
				Debug.DrawRay(this.m_Ray.origin, -this.m_Pivot.forward * (num + this.sphereCastRadius), Color.red);
			}
			this.protecting = flag2;
			this.m_CurrentDist = Mathf.SmoothDamp(this.m_CurrentDist, num, ref this.m_MoveVelocity, (this.m_CurrentDist > num) ? this.clipMoveTime : this.returnTime);
			this.m_CurrentDist = Mathf.Clamp(this.m_CurrentDist, this.closestDistance, this.m_OriginalDist);
			this.m_Cam.localPosition = -Vector3.forward * this.m_CurrentDist;
		}

		// Token: 0x040027B7 RID: 10167
		public float clipMoveTime = 0.05f;

		// Token: 0x040027B8 RID: 10168
		public float returnTime = 0.4f;

		// Token: 0x040027B9 RID: 10169
		public float sphereCastRadius = 0.15f;

		// Token: 0x040027BA RID: 10170
		public bool visualiseInEditor;

		// Token: 0x040027BB RID: 10171
		public float closestDistance = 0.5f;

		// Token: 0x040027BD RID: 10173
		public LayerMask dontClip = 1048576;

		// Token: 0x040027BE RID: 10174
		private Transform m_Cam;

		// Token: 0x040027BF RID: 10175
		private Transform m_Pivot;

		// Token: 0x040027C0 RID: 10176
		private float m_OriginalDist;

		// Token: 0x040027C1 RID: 10177
		private float m_MoveVelocity;

		// Token: 0x040027C2 RID: 10178
		private float m_CurrentDist;

		// Token: 0x040027C3 RID: 10179
		private Ray m_Ray;

		// Token: 0x040027C4 RID: 10180
		private RaycastHit[] hits;

		// Token: 0x040027C5 RID: 10181
		private CameraWallStop.RayHitComparer m_RayHitComparer;

		// Token: 0x0200091B RID: 2331
		public class RayHitComparer : IComparer
		{
			// Token: 0x06005284 RID: 21124 RVA: 0x002413DC File Offset: 0x0023F5DC
			public int Compare(object x, object y)
			{
				return ((RaycastHit)x).distance.CompareTo(((RaycastHit)y).distance);
			}
		}
	}
}
