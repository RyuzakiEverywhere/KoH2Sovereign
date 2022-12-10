using System;
using UnityEngine;

namespace Dreamteck.Splines
{
	// Token: 0x020004C3 RID: 1219
	[AddComponentMenu("Dreamteck/Splines/Users/Spline Positioner")]
	public class SplinePositioner : SplineTracer
	{
		// Token: 0x17000481 RID: 1153
		// (get) Token: 0x0600406C RID: 16492 RVA: 0x001EAFF1 File Offset: 0x001E91F1
		// (set) Token: 0x0600406D RID: 16493 RVA: 0x001EB00E File Offset: 0x001E920E
		public GameObject targetObject
		{
			get
			{
				if (this._targetObject == null)
				{
					return base.gameObject;
				}
				return this._targetObject;
			}
			set
			{
				if (value != this._targetObject)
				{
					this._targetObject = value;
					base.RefreshTargets();
					this.Rebuild();
				}
			}
		}

		// Token: 0x17000482 RID: 1154
		// (get) Token: 0x0600406E RID: 16494 RVA: 0x001EB031 File Offset: 0x001E9231
		// (set) Token: 0x0600406F RID: 16495 RVA: 0x001EB03C File Offset: 0x001E923C
		public double position
		{
			get
			{
				return this._position;
			}
			set
			{
				if (value != this._position)
				{
					this.animPosition = (float)value;
					this._position = value;
					if (this.mode == SplinePositioner.Mode.Distance)
					{
						this.SetDistance((float)this._position, true, false);
						return;
					}
					this.SetPercent(this._position, true, false);
				}
			}
		}

		// Token: 0x17000483 RID: 1155
		// (get) Token: 0x06004070 RID: 16496 RVA: 0x001EB088 File Offset: 0x001E9288
		// (set) Token: 0x06004071 RID: 16497 RVA: 0x001EB090 File Offset: 0x001E9290
		public SplinePositioner.Mode mode
		{
			get
			{
				return this._mode;
			}
			set
			{
				if (value != this._mode)
				{
					this._mode = value;
					this.Rebuild();
				}
			}
		}

		// Token: 0x06004072 RID: 16498 RVA: 0x001EB0A8 File Offset: 0x001E92A8
		protected override void OnDidApplyAnimationProperties()
		{
			if ((double)this.animPosition != this._position)
			{
				this.position = (double)this.animPosition;
			}
			base.OnDidApplyAnimationProperties();
		}

		// Token: 0x06004073 RID: 16499 RVA: 0x001EB0CC File Offset: 0x001E92CC
		protected override Transform GetTransform()
		{
			return this.targetObject.transform;
		}

		// Token: 0x06004074 RID: 16500 RVA: 0x001EB0D9 File Offset: 0x001E92D9
		protected override Rigidbody GetRigidbody()
		{
			return this.targetObject.GetComponent<Rigidbody>();
		}

		// Token: 0x06004075 RID: 16501 RVA: 0x001EB0E6 File Offset: 0x001E92E6
		protected override Rigidbody2D GetRigidbody2D()
		{
			return this.targetObject.GetComponent<Rigidbody2D>();
		}

		// Token: 0x06004076 RID: 16502 RVA: 0x001EB0F3 File Offset: 0x001E92F3
		protected override void PostBuild()
		{
			base.PostBuild();
			if (this.mode == SplinePositioner.Mode.Distance)
			{
				this.SetDistance((float)this._position, true, false);
				return;
			}
			this.SetPercent(this._position, true, false);
		}

		// Token: 0x06004077 RID: 16503 RVA: 0x001EB122 File Offset: 0x001E9322
		public override void SetPercent(double percent, bool checkTriggers = false, bool handleJuncitons = false)
		{
			base.SetPercent(percent, checkTriggers, handleJuncitons);
			this._position = percent;
		}

		// Token: 0x06004078 RID: 16504 RVA: 0x001EB134 File Offset: 0x001E9334
		public override void SetDistance(float distance, bool checkTriggers = false, bool handleJuncitons = false)
		{
			base.SetDistance(distance, checkTriggers, handleJuncitons);
			this._position = (double)distance;
		}

		// Token: 0x04002D26 RID: 11558
		[SerializeField]
		[HideInInspector]
		private GameObject _targetObject;

		// Token: 0x04002D27 RID: 11559
		[SerializeField]
		[HideInInspector]
		private double _position;

		// Token: 0x04002D28 RID: 11560
		[SerializeField]
		[HideInInspector]
		private float animPosition;

		// Token: 0x04002D29 RID: 11561
		[SerializeField]
		[HideInInspector]
		private SplinePositioner.Mode _mode;

		// Token: 0x020009A4 RID: 2468
		public enum Mode
		{
			// Token: 0x040044F8 RID: 17656
			Percent,
			// Token: 0x040044F9 RID: 17657
			Distance
		}
	}
}
