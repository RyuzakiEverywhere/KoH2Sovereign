using System;
using UnityEngine;
using UnityEngine.Events;

namespace BezierSolution
{
	// Token: 0x02000358 RID: 856
	public class BezierWalkerWithTime : MonoBehaviour
	{
		// Token: 0x170002AF RID: 687
		// (get) Token: 0x06003368 RID: 13160 RVA: 0x0019F295 File Offset: 0x0019D495
		// (set) Token: 0x06003369 RID: 13161 RVA: 0x0019F29D File Offset: 0x0019D49D
		public float NormalizedT
		{
			get
			{
				return this.progress;
			}
			set
			{
				this.progress = value;
			}
		}

		// Token: 0x0600336A RID: 13162 RVA: 0x0019F2A6 File Offset: 0x0019D4A6
		private void Awake()
		{
			this.cachedTransform = base.transform;
		}

		// Token: 0x0600336B RID: 13163 RVA: 0x0019F2B4 File Offset: 0x0019D4B4
		private void Update()
		{
			this.cachedTransform.position = Vector3.Lerp(this.cachedTransform.position, this.spline.GetPoint(this.progress), this.movementLerpModifier * Time.deltaTime);
			if (this.lookForward)
			{
				Quaternion b;
				if (this.isGoingForward)
				{
					b = Quaternion.LookRotation(this.spline.GetTangent(this.progress));
				}
				else
				{
					b = Quaternion.LookRotation(-this.spline.GetTangent(this.progress));
				}
				this.cachedTransform.rotation = Quaternion.Lerp(this.cachedTransform.rotation, b, this.rotationLerpModifier * Time.deltaTime);
			}
			if (this.isGoingForward)
			{
				this.progress += Time.deltaTime / this.travelTime;
				if (this.progress <= 1f)
				{
					this.onPathCompletedCalledAt1 = false;
					return;
				}
				if (!this.onPathCompletedCalledAt1)
				{
					this.onPathCompleted.Invoke();
					this.onPathCompletedCalledAt1 = true;
				}
				if (this.travelMode == BezierWalkerWithTime.TravelMode.Once)
				{
					this.progress = 1f;
					return;
				}
				if (this.travelMode == BezierWalkerWithTime.TravelMode.Loop)
				{
					this.progress -= 1f;
					return;
				}
				this.progress = 2f - this.progress;
				this.isGoingForward = false;
				return;
			}
			else
			{
				this.progress -= Time.deltaTime / this.travelTime;
				if (this.progress >= 0f)
				{
					this.onPathCompletedCalledAt0 = false;
					return;
				}
				if (!this.onPathCompletedCalledAt0)
				{
					this.onPathCompleted.Invoke();
					this.onPathCompletedCalledAt0 = true;
				}
				if (this.travelMode == BezierWalkerWithTime.TravelMode.Once)
				{
					this.progress = 0f;
					return;
				}
				if (this.travelMode == BezierWalkerWithTime.TravelMode.Loop)
				{
					this.progress += 1f;
					return;
				}
				this.progress = -this.progress;
				this.isGoingForward = true;
				return;
			}
		}

		// Token: 0x040022B3 RID: 8883
		private Transform cachedTransform;

		// Token: 0x040022B4 RID: 8884
		public BezierSpline spline;

		// Token: 0x040022B5 RID: 8885
		public BezierWalkerWithTime.TravelMode travelMode;

		// Token: 0x040022B6 RID: 8886
		public float travelTime = 5f;

		// Token: 0x040022B7 RID: 8887
		private float progress;

		// Token: 0x040022B8 RID: 8888
		public float movementLerpModifier = 10f;

		// Token: 0x040022B9 RID: 8889
		public float rotationLerpModifier = 10f;

		// Token: 0x040022BA RID: 8890
		public bool lookForward = true;

		// Token: 0x040022BB RID: 8891
		private bool isGoingForward = true;

		// Token: 0x040022BC RID: 8892
		public UnityEvent onPathCompleted = new UnityEvent();

		// Token: 0x040022BD RID: 8893
		private bool onPathCompletedCalledAt1;

		// Token: 0x040022BE RID: 8894
		private bool onPathCompletedCalledAt0;

		// Token: 0x02000899 RID: 2201
		public enum TravelMode
		{
			// Token: 0x0400404A RID: 16458
			Once,
			// Token: 0x0400404B RID: 16459
			Loop,
			// Token: 0x0400404C RID: 16460
			PingPong
		}
	}
}
