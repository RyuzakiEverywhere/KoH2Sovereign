using System;
using UnityEngine;
using UnityEngine.Events;

namespace BezierSolution
{
	// Token: 0x02000357 RID: 855
	public class BezierWalkerWithSpeed : MonoBehaviour
	{
		// Token: 0x170002AE RID: 686
		// (get) Token: 0x06003363 RID: 13155 RVA: 0x0019F054 File Offset: 0x0019D254
		// (set) Token: 0x06003364 RID: 13156 RVA: 0x0019F05C File Offset: 0x0019D25C
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

		// Token: 0x06003365 RID: 13157 RVA: 0x0019F065 File Offset: 0x0019D265
		private void Awake()
		{
			this.cachedTransform = base.transform;
		}

		// Token: 0x06003366 RID: 13158 RVA: 0x0019F074 File Offset: 0x0019D274
		private void Update()
		{
			float num = this.isGoingForward ? this.speed : (-this.speed);
			Vector3 position = this.spline.MoveAlongSpline(ref this.progress, num * Time.deltaTime, 3);
			this.cachedTransform.position = position;
			bool flag = this.speed > 0f == this.isGoingForward;
			if (this.lookForward)
			{
				Quaternion b;
				if (flag)
				{
					b = Quaternion.LookRotation(this.spline.GetTangent(this.progress));
				}
				else
				{
					b = Quaternion.LookRotation(-this.spline.GetTangent(this.progress));
				}
				this.cachedTransform.rotation = Quaternion.Lerp(this.cachedTransform.rotation, b, this.rotationLerpModifier * Time.deltaTime);
			}
			if (flag)
			{
				if (this.progress < 1f - this.relaxationAtEndPoints)
				{
					this.onPathCompletedCalledAt1 = false;
					return;
				}
				if (!this.onPathCompletedCalledAt1)
				{
					this.onPathCompleted.Invoke();
					this.onPathCompletedCalledAt1 = true;
				}
				if (this.travelMode == BezierWalkerWithSpeed.TravelMode.Once)
				{
					this.progress = 1f;
					return;
				}
				if (this.travelMode == BezierWalkerWithSpeed.TravelMode.Loop)
				{
					this.progress -= 1f;
					return;
				}
				this.progress = 2f - this.progress;
				this.isGoingForward = !this.isGoingForward;
				return;
			}
			else
			{
				if (this.progress > this.relaxationAtEndPoints)
				{
					this.onPathCompletedCalledAt0 = false;
					return;
				}
				if (!this.onPathCompletedCalledAt0)
				{
					this.onPathCompleted.Invoke();
					this.onPathCompletedCalledAt0 = true;
				}
				if (this.travelMode == BezierWalkerWithSpeed.TravelMode.Once)
				{
					this.progress = 0f;
					return;
				}
				if (this.travelMode == BezierWalkerWithSpeed.TravelMode.Loop)
				{
					this.progress += 1f;
					return;
				}
				this.progress = -this.progress;
				this.isGoingForward = !this.isGoingForward;
				return;
			}
		}

		// Token: 0x040022A7 RID: 8871
		private Transform cachedTransform;

		// Token: 0x040022A8 RID: 8872
		public BezierSpline spline;

		// Token: 0x040022A9 RID: 8873
		public BezierWalkerWithSpeed.TravelMode travelMode;

		// Token: 0x040022AA RID: 8874
		public float speed = 5f;

		// Token: 0x040022AB RID: 8875
		private float progress;

		// Token: 0x040022AC RID: 8876
		[Range(0f, 0.06f)]
		public float relaxationAtEndPoints = 0.01f;

		// Token: 0x040022AD RID: 8877
		public float rotationLerpModifier = 10f;

		// Token: 0x040022AE RID: 8878
		public bool lookForward = true;

		// Token: 0x040022AF RID: 8879
		private bool isGoingForward = true;

		// Token: 0x040022B0 RID: 8880
		public UnityEvent onPathCompleted = new UnityEvent();

		// Token: 0x040022B1 RID: 8881
		private bool onPathCompletedCalledAt1;

		// Token: 0x040022B2 RID: 8882
		private bool onPathCompletedCalledAt0;

		// Token: 0x02000898 RID: 2200
		public enum TravelMode
		{
			// Token: 0x04004046 RID: 16454
			Once,
			// Token: 0x04004047 RID: 16455
			Loop,
			// Token: 0x04004048 RID: 16456
			PingPong
		}
	}
}
