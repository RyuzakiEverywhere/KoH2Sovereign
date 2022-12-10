using System;
using UnityEngine;

namespace BezierSolution
{
	// Token: 0x02000355 RID: 853
	public class BezierPoint : MonoBehaviour
	{
		// Token: 0x1700029F RID: 671
		// (get) Token: 0x0600332C RID: 13100 RVA: 0x0019D7F9 File Offset: 0x0019B9F9
		// (set) Token: 0x0600332D RID: 13101 RVA: 0x0019D806 File Offset: 0x0019BA06
		public Vector3 localPosition
		{
			get
			{
				return base.transform.localPosition;
			}
			set
			{
				base.transform.localPosition = value;
			}
		}

		// Token: 0x170002A0 RID: 672
		// (get) Token: 0x0600332E RID: 13102 RVA: 0x0019D814 File Offset: 0x0019BA14
		// (set) Token: 0x0600332F RID: 13103 RVA: 0x0019D82F File Offset: 0x0019BA2F
		public Vector3 position
		{
			get
			{
				if (base.transform.hasChanged)
				{
					this.Revalidate();
				}
				return this.m_position;
			}
			set
			{
				base.transform.position = value;
			}
		}

		// Token: 0x170002A1 RID: 673
		// (get) Token: 0x06003330 RID: 13104 RVA: 0x0019D83D File Offset: 0x0019BA3D
		// (set) Token: 0x06003331 RID: 13105 RVA: 0x0019D84A File Offset: 0x0019BA4A
		public Quaternion localRotation
		{
			get
			{
				return base.transform.localRotation;
			}
			set
			{
				base.transform.localRotation = value;
			}
		}

		// Token: 0x170002A2 RID: 674
		// (get) Token: 0x06003332 RID: 13106 RVA: 0x0019D858 File Offset: 0x0019BA58
		// (set) Token: 0x06003333 RID: 13107 RVA: 0x0019D865 File Offset: 0x0019BA65
		public Quaternion rotation
		{
			get
			{
				return base.transform.rotation;
			}
			set
			{
				base.transform.rotation = value;
			}
		}

		// Token: 0x170002A3 RID: 675
		// (get) Token: 0x06003334 RID: 13108 RVA: 0x0019D873 File Offset: 0x0019BA73
		// (set) Token: 0x06003335 RID: 13109 RVA: 0x0019D880 File Offset: 0x0019BA80
		public Vector3 localEulerAngles
		{
			get
			{
				return base.transform.localEulerAngles;
			}
			set
			{
				base.transform.localEulerAngles = value;
			}
		}

		// Token: 0x170002A4 RID: 676
		// (get) Token: 0x06003336 RID: 13110 RVA: 0x0019D88E File Offset: 0x0019BA8E
		// (set) Token: 0x06003337 RID: 13111 RVA: 0x0019D89B File Offset: 0x0019BA9B
		public Vector3 eulerAngles
		{
			get
			{
				return base.transform.eulerAngles;
			}
			set
			{
				base.transform.eulerAngles = value;
			}
		}

		// Token: 0x170002A5 RID: 677
		// (get) Token: 0x06003338 RID: 13112 RVA: 0x0019D8A9 File Offset: 0x0019BAA9
		// (set) Token: 0x06003339 RID: 13113 RVA: 0x0019D8B6 File Offset: 0x0019BAB6
		public Vector3 localScale
		{
			get
			{
				return base.transform.localScale;
			}
			set
			{
				base.transform.localScale = value;
			}
		}

		// Token: 0x170002A6 RID: 678
		// (get) Token: 0x0600333A RID: 13114 RVA: 0x0019D8C4 File Offset: 0x0019BAC4
		// (set) Token: 0x0600333B RID: 13115 RVA: 0x0019D8CC File Offset: 0x0019BACC
		public Vector3 precedingControlPointLocalPosition
		{
			get
			{
				return this.m_precedingControlPointLocalPosition;
			}
			set
			{
				this.m_precedingControlPointLocalPosition = value;
				this.m_precedingControlPointPosition = base.transform.TransformPoint(value);
				if (this.m_handleMode == BezierPoint.HandleMode.Aligned)
				{
					this.m_followingControlPointLocalPosition = -this.m_precedingControlPointLocalPosition.normalized * this.m_followingControlPointLocalPosition.magnitude;
					this.m_followingControlPointPosition = base.transform.TransformPoint(this.m_followingControlPointLocalPosition);
					return;
				}
				if (this.m_handleMode == BezierPoint.HandleMode.Mirrored)
				{
					this.m_followingControlPointLocalPosition = -this.m_precedingControlPointLocalPosition;
					this.m_followingControlPointPosition = base.transform.TransformPoint(this.m_followingControlPointLocalPosition);
				}
			}
		}

		// Token: 0x170002A7 RID: 679
		// (get) Token: 0x0600333C RID: 13116 RVA: 0x0019D96A File Offset: 0x0019BB6A
		// (set) Token: 0x0600333D RID: 13117 RVA: 0x0019D988 File Offset: 0x0019BB88
		public Vector3 precedingControlPointPosition
		{
			get
			{
				if (base.transform.hasChanged)
				{
					this.Revalidate();
				}
				return this.m_precedingControlPointPosition;
			}
			set
			{
				this.m_precedingControlPointPosition = value;
				this.m_precedingControlPointLocalPosition = base.transform.InverseTransformPoint(value);
				if (base.transform.hasChanged)
				{
					this.m_position = base.transform.position;
					base.transform.hasChanged = false;
				}
				if (this.m_handleMode == BezierPoint.HandleMode.Aligned)
				{
					this.m_followingControlPointPosition = this.m_position - (this.m_precedingControlPointPosition - this.m_position).normalized * (this.m_followingControlPointPosition - this.m_position).magnitude;
					this.m_followingControlPointLocalPosition = base.transform.InverseTransformPoint(this.m_followingControlPointPosition);
					return;
				}
				if (this.m_handleMode == BezierPoint.HandleMode.Mirrored)
				{
					this.m_followingControlPointPosition = 2f * this.m_position - this.m_precedingControlPointPosition;
					this.m_followingControlPointLocalPosition = base.transform.InverseTransformPoint(this.m_followingControlPointPosition);
				}
			}
		}

		// Token: 0x170002A8 RID: 680
		// (get) Token: 0x0600333E RID: 13118 RVA: 0x0019DA82 File Offset: 0x0019BC82
		// (set) Token: 0x0600333F RID: 13119 RVA: 0x0019DA8C File Offset: 0x0019BC8C
		public Vector3 followingControlPointLocalPosition
		{
			get
			{
				return this.m_followingControlPointLocalPosition;
			}
			set
			{
				this.m_followingControlPointLocalPosition = value;
				this.m_followingControlPointPosition = base.transform.TransformPoint(value);
				if (this.m_handleMode == BezierPoint.HandleMode.Aligned)
				{
					this.m_precedingControlPointLocalPosition = -this.m_followingControlPointLocalPosition.normalized * this.m_precedingControlPointLocalPosition.magnitude;
					this.m_precedingControlPointPosition = base.transform.TransformPoint(this.m_precedingControlPointLocalPosition);
					return;
				}
				if (this.m_handleMode == BezierPoint.HandleMode.Mirrored)
				{
					this.m_precedingControlPointLocalPosition = -this.m_followingControlPointLocalPosition;
					this.m_precedingControlPointPosition = base.transform.TransformPoint(this.m_precedingControlPointLocalPosition);
				}
			}
		}

		// Token: 0x170002A9 RID: 681
		// (get) Token: 0x06003340 RID: 13120 RVA: 0x0019DB2A File Offset: 0x0019BD2A
		// (set) Token: 0x06003341 RID: 13121 RVA: 0x0019DB48 File Offset: 0x0019BD48
		public Vector3 followingControlPointPosition
		{
			get
			{
				if (base.transform.hasChanged)
				{
					this.Revalidate();
				}
				return this.m_followingControlPointPosition;
			}
			set
			{
				this.m_followingControlPointPosition = value;
				this.m_followingControlPointLocalPosition = base.transform.InverseTransformPoint(value);
				if (base.transform.hasChanged)
				{
					this.m_position = base.transform.position;
					base.transform.hasChanged = false;
				}
				if (this.m_handleMode == BezierPoint.HandleMode.Aligned)
				{
					this.m_precedingControlPointPosition = this.m_position - (this.m_followingControlPointPosition - this.m_position).normalized * (this.m_precedingControlPointPosition - this.m_position).magnitude;
					this.m_precedingControlPointLocalPosition = base.transform.InverseTransformPoint(this.m_precedingControlPointPosition);
					return;
				}
				if (this.m_handleMode == BezierPoint.HandleMode.Mirrored)
				{
					this.m_precedingControlPointPosition = 2f * this.m_position - this.m_followingControlPointPosition;
					this.m_precedingControlPointLocalPosition = base.transform.InverseTransformPoint(this.m_precedingControlPointPosition);
				}
			}
		}

		// Token: 0x170002AA RID: 682
		// (get) Token: 0x06003342 RID: 13122 RVA: 0x0019DC42 File Offset: 0x0019BE42
		// (set) Token: 0x06003343 RID: 13123 RVA: 0x0019DC4A File Offset: 0x0019BE4A
		public BezierPoint.HandleMode handleMode
		{
			get
			{
				return this.m_handleMode;
			}
			set
			{
				this.m_handleMode = value;
				if (value == BezierPoint.HandleMode.Aligned || value == BezierPoint.HandleMode.Mirrored)
				{
					this.precedingControlPointLocalPosition = this.m_precedingControlPointLocalPosition;
				}
			}
		}

		// Token: 0x06003344 RID: 13124 RVA: 0x0019DC67 File Offset: 0x0019BE67
		private void Awake()
		{
			base.transform.hasChanged = true;
		}

		// Token: 0x06003345 RID: 13125 RVA: 0x0019DC78 File Offset: 0x0019BE78
		public void CopyTo(BezierPoint other)
		{
			other.transform.localPosition = base.transform.localPosition;
			other.transform.localRotation = base.transform.localRotation;
			other.transform.localScale = base.transform.localScale;
			other.m_handleMode = this.m_handleMode;
			other.m_precedingControlPointLocalPosition = this.m_precedingControlPointLocalPosition;
			other.m_followingControlPointLocalPosition = this.m_followingControlPointLocalPosition;
		}

		// Token: 0x06003346 RID: 13126 RVA: 0x0019DCEC File Offset: 0x0019BEEC
		private void Revalidate()
		{
			this.m_position = base.transform.position;
			this.m_precedingControlPointPosition = base.transform.TransformPoint(this.m_precedingControlPointLocalPosition);
			this.m_followingControlPointPosition = base.transform.TransformPoint(this.m_followingControlPointLocalPosition);
			base.transform.hasChanged = false;
		}

		// Token: 0x06003347 RID: 13127 RVA: 0x0019DD44 File Offset: 0x0019BF44
		public void Reset()
		{
			this.localPosition = Vector3.zero;
			this.localRotation = Quaternion.identity;
			this.localScale = Vector3.one;
			this.precedingControlPointLocalPosition = Vector3.left;
			this.followingControlPointLocalPosition = Vector3.right;
			base.transform.hasChanged = true;
		}

		// Token: 0x0400229B RID: 8859
		[SerializeField]
		[HideInInspector]
		private Vector3 m_position;

		// Token: 0x0400229C RID: 8860
		[SerializeField]
		[HideInInspector]
		private Vector3 m_precedingControlPointLocalPosition = Vector3.left;

		// Token: 0x0400229D RID: 8861
		[SerializeField]
		[HideInInspector]
		private Vector3 m_precedingControlPointPosition;

		// Token: 0x0400229E RID: 8862
		[SerializeField]
		[HideInInspector]
		private Vector3 m_followingControlPointLocalPosition = Vector3.right;

		// Token: 0x0400229F RID: 8863
		[SerializeField]
		[HideInInspector]
		private Vector3 m_followingControlPointPosition;

		// Token: 0x040022A0 RID: 8864
		[SerializeField]
		[HideInInspector]
		private BezierPoint.HandleMode m_handleMode = BezierPoint.HandleMode.Mirrored;

		// Token: 0x02000897 RID: 2199
		public enum HandleMode
		{
			// Token: 0x04004042 RID: 16450
			Free,
			// Token: 0x04004043 RID: 16451
			Aligned,
			// Token: 0x04004044 RID: 16452
			Mirrored
		}
	}
}
