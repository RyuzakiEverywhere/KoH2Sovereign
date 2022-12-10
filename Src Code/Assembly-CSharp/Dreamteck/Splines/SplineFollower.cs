using System;
using UnityEngine;

namespace Dreamteck.Splines
{
	// Token: 0x020004C0 RID: 1216
	[AddComponentMenu("Dreamteck/Splines/Users/Spline Follower")]
	public class SplineFollower : SplineTracer
	{
		// Token: 0x1700047D RID: 1149
		// (get) Token: 0x06004030 RID: 16432 RVA: 0x001E8ECC File Offset: 0x001E70CC
		// (set) Token: 0x06004031 RID: 16433 RVA: 0x001E8ED4 File Offset: 0x001E70D4
		public float followSpeed
		{
			get
			{
				return this._followSpeed;
			}
			set
			{
				if (this._followSpeed != value)
				{
					if (value < 0f)
					{
						value = 0f;
					}
					this._followSpeed = value;
				}
			}
		}

		// Token: 0x1700047E RID: 1150
		// (get) Token: 0x06004032 RID: 16434 RVA: 0x001E8EF5 File Offset: 0x001E70F5
		// (set) Token: 0x06004033 RID: 16435 RVA: 0x001E8EFD File Offset: 0x001E70FD
		public double startPosition
		{
			get
			{
				return this._startPosition;
			}
			set
			{
				if (value != this._startPosition)
				{
					this._startPosition = DMath.Clamp01(value);
					if (!this.followStarted)
					{
						this.SetPercent(this._startPosition, false, false);
					}
				}
			}
		}

		// Token: 0x1700047F RID: 1151
		// (get) Token: 0x06004034 RID: 16436 RVA: 0x001E8F2A File Offset: 0x001E712A
		// (set) Token: 0x06004035 RID: 16437 RVA: 0x001E8F32 File Offset: 0x001E7132
		public float followDuration
		{
			get
			{
				return this._followDuration;
			}
			set
			{
				if (this._followDuration != value)
				{
					if (value < 0f)
					{
						value = 0f;
					}
					this._followDuration = value;
				}
			}
		}

		// Token: 0x14000043 RID: 67
		// (add) Token: 0x06004036 RID: 16438 RVA: 0x001E8F54 File Offset: 0x001E7154
		// (remove) Token: 0x06004037 RID: 16439 RVA: 0x001E8F8C File Offset: 0x001E718C
		public event SplineReachHandler onEndReached;

		// Token: 0x14000044 RID: 68
		// (add) Token: 0x06004038 RID: 16440 RVA: 0x001E8FC4 File Offset: 0x001E71C4
		// (remove) Token: 0x06004039 RID: 16441 RVA: 0x001E8FFC File Offset: 0x001E71FC
		public event SplineReachHandler onBeginningReached;

		// Token: 0x0600403A RID: 16442 RVA: 0x001E9034 File Offset: 0x001E7234
		protected override void Start()
		{
			base.Start();
			if (this.follow && this.autoStartPosition)
			{
				this.SetPercent(base.spline.Project(this.GetTransform().position, 0.0, 1.0).percent, false, false);
			}
		}

		// Token: 0x0600403B RID: 16443 RVA: 0x001E908C File Offset: 0x001E728C
		protected override void LateRun()
		{
			base.LateRun();
			if (this.follow)
			{
				this.Follow();
			}
		}

		// Token: 0x0600403C RID: 16444 RVA: 0x001E90A2 File Offset: 0x001E72A2
		protected override void PostBuild()
		{
			base.PostBuild();
			if (base.sampleCount == 0)
			{
				return;
			}
			base.Evaluate(this._result.percent, this._result);
			if (this.follow && !this.autoStartPosition)
			{
				base.ApplyMotion();
			}
		}

		// Token: 0x0600403D RID: 16445 RVA: 0x001E90E0 File Offset: 0x001E72E0
		private void Follow()
		{
			if (!this.followStarted)
			{
				if (this.autoStartPosition)
				{
					this.Project(this.GetTransform().position, this.evalResult, 0.0, 1.0);
					this.SetPercent(this.evalResult.percent, false, false);
				}
				else
				{
					this.SetPercent(this._startPosition, false, false);
				}
			}
			this.followStarted = true;
			SplineFollower.FollowMode followMode = this.followMode;
			if (followMode == SplineFollower.FollowMode.Uniform)
			{
				this.Move(Time.deltaTime * this._followSpeed);
				return;
			}
			if (followMode != SplineFollower.FollowMode.Time)
			{
				return;
			}
			if ((double)this._followDuration == 0.0)
			{
				this.Move(0.0);
				return;
			}
			this.Move((double)Time.deltaTime / (double)this._followDuration);
		}

		// Token: 0x0600403E RID: 16446 RVA: 0x001E91A8 File Offset: 0x001E73A8
		public void Restart(double startPosition = 0.0)
		{
			this.followStarted = false;
			this.SetPercent(startPosition, false, false);
		}

		// Token: 0x0600403F RID: 16447 RVA: 0x001E91BA File Offset: 0x001E73BA
		public override void SetPercent(double percent, bool checkTriggers = false, bool handleJuncitons = false)
		{
			base.SetPercent(percent, checkTriggers, handleJuncitons);
			this.lastClippedPercent = percent;
		}

		// Token: 0x06004040 RID: 16448 RVA: 0x001E91CC File Offset: 0x001E73CC
		public override void SetDistance(float distance, bool checkTriggers = false, bool handleJuncitons = false)
		{
			base.SetDistance(distance, checkTriggers, handleJuncitons);
			this.lastClippedPercent = base.ClipPercent(this._result.percent);
			if (base.samplesAreLooped && base.clipFrom == base.clipTo && distance > 0f && this.lastClippedPercent == 0.0)
			{
				this.lastClippedPercent = 1.0;
			}
		}

		// Token: 0x06004041 RID: 16449 RVA: 0x001E9238 File Offset: 0x001E7438
		public void Move(double percent)
		{
			if (percent == 0.0)
			{
				return;
			}
			if (base.sampleCount <= 1)
			{
				if (base.sampleCount == 1)
				{
					this._result.CopyFrom(base.GetSampleRaw(0));
					base.ApplyMotion();
				}
				return;
			}
			base.Evaluate(this._result.percent, this._result);
			double num = this._result.percent;
			if (this.wrapMode == SplineFollower.Wrap.Default && this.lastClippedPercent >= 1.0 && num == 0.0)
			{
				num = 1.0;
			}
			double num2 = num + ((this._direction == Spline.Direction.Forward) ? percent : (-percent));
			bool flag = false;
			bool flag2 = false;
			this.lastClippedPercent = num2;
			if (this._direction == Spline.Direction.Forward && num2 >= 1.0)
			{
				if (this.onEndReached != null && num < 1.0)
				{
					flag = true;
				}
				switch (this.wrapMode)
				{
				case SplineFollower.Wrap.Default:
					num2 = 1.0;
					break;
				case SplineFollower.Wrap.Loop:
					base.CheckTriggers(num, 1.0);
					base.CheckNodes(num, 1.0);
					while (num2 > 1.0)
					{
						num2 -= 1.0;
					}
					num = 0.0;
					break;
				case SplineFollower.Wrap.PingPong:
					num2 = DMath.Clamp01(1.0 - (num2 - 1.0));
					num = 1.0;
					this._direction = Spline.Direction.Backward;
					break;
				}
			}
			else if (this._direction == Spline.Direction.Backward && num2 <= 0.0)
			{
				if (this.onBeginningReached != null && num > 0.0)
				{
					flag2 = true;
				}
				switch (this.wrapMode)
				{
				case SplineFollower.Wrap.Default:
					num2 = 0.0;
					break;
				case SplineFollower.Wrap.Loop:
					base.CheckTriggers(num, 0.0);
					base.CheckNodes(num, 0.0);
					while (num2 < 0.0)
					{
						num2 += 1.0;
					}
					num = 1.0;
					break;
				case SplineFollower.Wrap.PingPong:
					num2 = DMath.Clamp01(-num2);
					num = 0.0;
					this._direction = Spline.Direction.Forward;
					break;
				}
			}
			base.CheckTriggers(num, num2);
			base.CheckNodes(num, num2);
			base.Evaluate(num2, this._result);
			base.ApplyMotion();
			if (flag)
			{
				this.onEndReached();
			}
			else if (flag2)
			{
				this.onBeginningReached();
			}
			base.InvokeTriggers();
			base.InvokeNodes();
		}

		// Token: 0x06004042 RID: 16450 RVA: 0x001E94CC File Offset: 0x001E76CC
		public void Move(float distance)
		{
			bool flag = false;
			bool flag2 = false;
			float num = 0f;
			double percent = this._result.percent;
			this._result.percent = base.Travel(this._result.percent, distance, this._direction, out num);
			if (percent != this._result.percent)
			{
				base.CheckTriggers(percent, this._result.percent);
				base.CheckNodes(percent, this._result.percent);
			}
			if (num < distance)
			{
				if (base.direction == Spline.Direction.Forward)
				{
					if (this._result.percent >= 1.0)
					{
						SplineFollower.Wrap wrap = this.wrapMode;
						if (wrap != SplineFollower.Wrap.Loop)
						{
							if (wrap == SplineFollower.Wrap.PingPong)
							{
								this._direction = Spline.Direction.Backward;
								this._result.percent = base.Travel(1.0, distance - num, this._direction, out num);
								base.CheckTriggers(1.0, this._result.percent);
								base.CheckNodes(1.0, this._result.percent);
							}
						}
						else
						{
							this._result.percent = base.Travel(0.0, distance - num, this._direction, out num);
							base.CheckTriggers(0.0, this._result.percent);
							base.CheckNodes(0.0, this._result.percent);
						}
						if (percent < this._result.percent)
						{
							flag = true;
						}
					}
				}
				else if (this._result.percent <= 0.0)
				{
					SplineFollower.Wrap wrap = this.wrapMode;
					if (wrap != SplineFollower.Wrap.Loop)
					{
						if (wrap == SplineFollower.Wrap.PingPong)
						{
							this._direction = Spline.Direction.Forward;
							this._result.percent = base.Travel(0.0, distance - num, this._direction, out num);
							base.CheckTriggers(0.0, this._result.percent);
							base.CheckNodes(0.0, this._result.percent);
						}
					}
					else
					{
						this._result.percent = base.Travel(1.0, distance - num, this._direction, out num);
						base.CheckTriggers(1.0, this._result.percent);
						base.CheckNodes(1.0, this._result.percent);
					}
					if (percent > this._result.percent)
					{
						flag2 = true;
					}
				}
			}
			base.Evaluate(this._result.percent, this._result);
			base.ApplyMotion();
			if (flag && this.onEndReached != null)
			{
				this.onEndReached();
			}
			else if (flag2 && this.onBeginningReached != null)
			{
				this.onBeginningReached();
			}
			base.InvokeTriggers();
			base.InvokeNodes();
		}

		// Token: 0x04002D09 RID: 11529
		[HideInInspector]
		public SplineFollower.Wrap wrapMode;

		// Token: 0x04002D0A RID: 11530
		[HideInInspector]
		public SplineFollower.FollowMode followMode;

		// Token: 0x04002D0B RID: 11531
		[HideInInspector]
		public bool autoStartPosition;

		// Token: 0x04002D0C RID: 11532
		[HideInInspector]
		public bool follow = true;

		// Token: 0x04002D0F RID: 11535
		[SerializeField]
		[HideInInspector]
		private float _followSpeed = 1f;

		// Token: 0x04002D10 RID: 11536
		[SerializeField]
		[HideInInspector]
		private float _followDuration = 1f;

		// Token: 0x04002D11 RID: 11537
		[SerializeField]
		[HideInInspector]
		[Range(0f, 1f)]
		private double _startPosition;

		// Token: 0x04002D12 RID: 11538
		private double lastClippedPercent = -1.0;

		// Token: 0x04002D13 RID: 11539
		private bool followStarted;

		// Token: 0x0200099E RID: 2462
		public enum FollowMode
		{
			// Token: 0x040044BB RID: 17595
			Uniform,
			// Token: 0x040044BC RID: 17596
			Time
		}

		// Token: 0x0200099F RID: 2463
		public enum Wrap
		{
			// Token: 0x040044BE RID: 17598
			Default,
			// Token: 0x040044BF RID: 17599
			Loop,
			// Token: 0x040044C0 RID: 17600
			PingPong
		}
	}
}
