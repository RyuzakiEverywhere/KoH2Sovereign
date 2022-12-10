using System;
using System.Collections;
using UnityEngine;

namespace MalbersAnimations.SA
{
	// Token: 0x02000453 RID: 1107
	public class MProtectCameraFromWallClip : MonoBehaviour
	{
		// Token: 0x170003E7 RID: 999
		// (get) Token: 0x06003AC9 RID: 15049 RVA: 0x001C3FF4 File Offset: 0x001C21F4
		// (set) Token: 0x06003ACA RID: 15050 RVA: 0x001C3FFC File Offset: 0x001C21FC
		public bool protecting { get; private set; }

		// Token: 0x06003ACB RID: 15051 RVA: 0x001C4008 File Offset: 0x001C2208
		private void Start()
		{
			this.m_Cam = base.GetComponentInChildren<Camera>().transform;
			this.m_Pivot = this.m_Cam.parent;
			this.m_OriginalDist = this.m_Cam.localPosition.magnitude;
			this.m_CurrentDist = this.m_OriginalDist;
			this.m_RayHitComparer = new MProtectCameraFromWallClip.RayHitComparer();
		}

		// Token: 0x06003ACC RID: 15052 RVA: 0x001C4068 File Offset: 0x001C2268
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
				if (!array[i].isTrigger && (!(array[i].attachedRigidbody != null) || !array[i].attachedRigidbody.CompareTag(this.dontClipTag)))
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				this.m_Ray.origin = this.m_Ray.origin + this.m_Pivot.forward * this.sphereCastRadius;
				this.m_Hits = Physics.RaycastAll(this.m_Ray, this.m_OriginalDist - this.sphereCastRadius);
			}
			else
			{
				this.m_Hits = Physics.SphereCastAll(this.m_Ray, this.sphereCastRadius, this.m_OriginalDist + this.sphereCastRadius);
			}
			Array.Sort(this.m_Hits, this.m_RayHitComparer);
			float num2 = float.PositiveInfinity;
			for (int j = 0; j < this.m_Hits.Length; j++)
			{
				if (this.m_Hits[j].distance < num2 && !this.m_Hits[j].collider.isTrigger && (!(this.m_Hits[j].collider.attachedRigidbody != null) || !this.m_Hits[j].collider.attachedRigidbody.CompareTag(this.dontClipTag)))
				{
					num2 = this.m_Hits[j].distance;
					num = -this.m_Pivot.InverseTransformPoint(this.m_Hits[j].point).z;
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

		// Token: 0x04002A82 RID: 10882
		public float clipMoveTime = 0.05f;

		// Token: 0x04002A83 RID: 10883
		public float returnTime = 0.4f;

		// Token: 0x04002A84 RID: 10884
		public float sphereCastRadius = 0.1f;

		// Token: 0x04002A85 RID: 10885
		public bool visualiseInEditor;

		// Token: 0x04002A86 RID: 10886
		public float closestDistance = 0.5f;

		// Token: 0x04002A88 RID: 10888
		public string dontClipTag = "Player";

		// Token: 0x04002A89 RID: 10889
		private Transform m_Cam;

		// Token: 0x04002A8A RID: 10890
		private Transform m_Pivot;

		// Token: 0x04002A8B RID: 10891
		private float m_OriginalDist;

		// Token: 0x04002A8C RID: 10892
		private float m_MoveVelocity;

		// Token: 0x04002A8D RID: 10893
		private float m_CurrentDist;

		// Token: 0x04002A8E RID: 10894
		private Ray m_Ray;

		// Token: 0x04002A8F RID: 10895
		private RaycastHit[] m_Hits;

		// Token: 0x04002A90 RID: 10896
		private MProtectCameraFromWallClip.RayHitComparer m_RayHitComparer;

		// Token: 0x0200093E RID: 2366
		public class RayHitComparer : IComparer
		{
			// Token: 0x0600530F RID: 21263 RVA: 0x00242780 File Offset: 0x00240980
			public int Compare(object x, object y)
			{
				return ((RaycastHit)x).distance.CompareTo(((RaycastHit)y).distance);
			}
		}
	}
}
