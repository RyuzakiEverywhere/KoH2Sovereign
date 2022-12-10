using System;
using UnityEngine;

namespace Dreamteck.Splines
{
	// Token: 0x020004C4 RID: 1220
	[ExecuteInEditMode]
	[AddComponentMenu("Dreamteck/Splines/Users/Spline Projector")]
	public class SplineProjector : SplineTracer
	{
		// Token: 0x17000484 RID: 1156
		// (get) Token: 0x0600407A RID: 16506 RVA: 0x001EB14F File Offset: 0x001E934F
		// (set) Token: 0x0600407B RID: 16507 RVA: 0x001EB157 File Offset: 0x001E9357
		public SplineProjector.Mode mode
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

		// Token: 0x17000485 RID: 1157
		// (get) Token: 0x0600407C RID: 16508 RVA: 0x001EB16F File Offset: 0x001E936F
		// (set) Token: 0x0600407D RID: 16509 RVA: 0x001EB177 File Offset: 0x001E9377
		public bool autoProject
		{
			get
			{
				return this._autoProject;
			}
			set
			{
				if (value != this._autoProject)
				{
					this._autoProject = value;
					if (this._autoProject)
					{
						this.Rebuild();
					}
				}
			}
		}

		// Token: 0x17000486 RID: 1158
		// (get) Token: 0x0600407E RID: 16510 RVA: 0x001EB197 File Offset: 0x001E9397
		// (set) Token: 0x0600407F RID: 16511 RVA: 0x001EB19F File Offset: 0x001E939F
		public int subdivide
		{
			get
			{
				return this._subdivide;
			}
			set
			{
				if (value != this._subdivide)
				{
					this._subdivide = value;
					if (this._mode == SplineProjector.Mode.Accurate)
					{
						this.Rebuild();
					}
				}
			}
		}

		// Token: 0x17000487 RID: 1159
		// (get) Token: 0x06004080 RID: 16512 RVA: 0x001EB1BF File Offset: 0x001E93BF
		// (set) Token: 0x06004081 RID: 16513 RVA: 0x001EB1DC File Offset: 0x001E93DC
		public Transform projectTarget
		{
			get
			{
				if (this._projectTarget == null)
				{
					return base.transform;
				}
				return this._projectTarget;
			}
			set
			{
				if (value != this._projectTarget)
				{
					this._projectTarget = value;
					this.Rebuild();
				}
			}
		}

		// Token: 0x17000488 RID: 1160
		// (get) Token: 0x06004082 RID: 16514 RVA: 0x001EB1FC File Offset: 0x001E93FC
		// (set) Token: 0x06004083 RID: 16515 RVA: 0x001EB24A File Offset: 0x001E944A
		public GameObject targetObject
		{
			get
			{
				if (this._targetObject == null && this.applyTarget != null)
				{
					this._targetObject = this.applyTarget.gameObject;
					this.applyTarget = null;
					return this._targetObject;
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

		// Token: 0x14000045 RID: 69
		// (add) Token: 0x06004084 RID: 16516 RVA: 0x001EB270 File Offset: 0x001E9470
		// (remove) Token: 0x06004085 RID: 16517 RVA: 0x001EB2A8 File Offset: 0x001E94A8
		public event SplineReachHandler onEndReached;

		// Token: 0x14000046 RID: 70
		// (add) Token: 0x06004086 RID: 16518 RVA: 0x001EB2E0 File Offset: 0x001E94E0
		// (remove) Token: 0x06004087 RID: 16519 RVA: 0x001EB318 File Offset: 0x001E9518
		public event SplineReachHandler onBeginningReached;

		// Token: 0x06004088 RID: 16520 RVA: 0x001EB34D File Offset: 0x001E954D
		protected override void Reset()
		{
			base.Reset();
			this._projectTarget = base.transform;
		}

		// Token: 0x06004089 RID: 16521 RVA: 0x001EB361 File Offset: 0x001E9561
		protected override Transform GetTransform()
		{
			if (this.targetObject == null)
			{
				return null;
			}
			return this.targetObject.transform;
		}

		// Token: 0x0600408A RID: 16522 RVA: 0x001EB37E File Offset: 0x001E957E
		protected override Rigidbody GetRigidbody()
		{
			if (this.targetObject == null)
			{
				return null;
			}
			return this.targetObject.GetComponent<Rigidbody>();
		}

		// Token: 0x0600408B RID: 16523 RVA: 0x001EB39B File Offset: 0x001E959B
		protected override Rigidbody2D GetRigidbody2D()
		{
			if (this.targetObject == null)
			{
				return null;
			}
			return this.targetObject.GetComponent<Rigidbody2D>();
		}

		// Token: 0x0600408C RID: 16524 RVA: 0x001EB3B8 File Offset: 0x001E95B8
		protected override void LateRun()
		{
			base.LateRun();
			if (this.autoProject && this.projectTarget && this.lastPosition != this.projectTarget.position)
			{
				this.lastPosition = this.projectTarget.position;
				this.CalculateProjection();
			}
		}

		// Token: 0x0600408D RID: 16525 RVA: 0x001EB40F File Offset: 0x001E960F
		protected override void PostBuild()
		{
			base.PostBuild();
			this.CalculateProjection();
		}

		// Token: 0x0600408E RID: 16526 RVA: 0x001EB420 File Offset: 0x001E9620
		protected override void OnSplineChanged()
		{
			if (base.spline != null)
			{
				if (this._mode == SplineProjector.Mode.Accurate)
				{
					base.spline.Project(this._result, this._projectTarget.position, base.clipFrom, base.clipTo, SplineComputer.EvaluateMode.Calculate, this.subdivide);
				}
				else
				{
					base.spline.Project(this._result, this._projectTarget.position, base.clipFrom, base.clipTo, SplineComputer.EvaluateMode.Cached, 4);
				}
				this._result.percent = base.ClipPercent(this._result.percent);
			}
		}

		// Token: 0x0600408F RID: 16527 RVA: 0x001EB4BC File Offset: 0x001E96BC
		private void Project()
		{
			if (this._mode == SplineProjector.Mode.Accurate && base.spline != null)
			{
				base.spline.Project(this._result, this._projectTarget.position, base.clipFrom, base.clipTo, SplineComputer.EvaluateMode.Calculate, this.subdivide);
				this._result.percent = base.ClipPercent(this._result.percent);
				return;
			}
			this.Project(this._projectTarget.position, this._result, 0.0, 1.0);
		}

		// Token: 0x06004090 RID: 16528 RVA: 0x001EB554 File Offset: 0x001E9754
		public void CalculateProjection()
		{
			if (this._projectTarget == null)
			{
				return;
			}
			double num = this._result.percent;
			this.Project();
			if (this.onBeginningReached != null && this._result.percent <= base.clipFrom)
			{
				if (!Mathf.Approximately((float)num, (float)this._result.percent))
				{
					this.onBeginningReached();
					if (base.samplesAreLooped)
					{
						base.CheckTriggers(num, 0.0);
						base.CheckNodes(num, 0.0);
						num = 1.0;
					}
				}
			}
			else if (this.onEndReached != null && this._result.percent >= base.clipTo && !Mathf.Approximately((float)num, (float)this._result.percent))
			{
				this.onEndReached();
				if (base.samplesAreLooped)
				{
					base.CheckTriggers(num, 1.0);
					base.CheckNodes(num, 1.0);
					num = 0.0;
				}
			}
			base.CheckTriggers(num, this._result.percent);
			base.CheckNodes(num, this._result.percent);
			if (this.targetObject != null)
			{
				base.ApplyMotion();
			}
			base.InvokeTriggers();
			base.InvokeNodes();
			this.lastPosition = this.projectTarget.position;
		}

		// Token: 0x04002D2A RID: 11562
		[SerializeField]
		[HideInInspector]
		private SplineProjector.Mode _mode = SplineProjector.Mode.Cached;

		// Token: 0x04002D2B RID: 11563
		[SerializeField]
		[HideInInspector]
		private bool _autoProject = true;

		// Token: 0x04002D2C RID: 11564
		[SerializeField]
		[HideInInspector]
		[Range(3f, 8f)]
		private int _subdivide = 4;

		// Token: 0x04002D2D RID: 11565
		[SerializeField]
		[HideInInspector]
		private Transform _projectTarget;

		// Token: 0x04002D2E RID: 11566
		[SerializeField]
		[HideInInspector]
		private Transform applyTarget;

		// Token: 0x04002D2F RID: 11567
		[SerializeField]
		[HideInInspector]
		private GameObject _targetObject;

		// Token: 0x04002D30 RID: 11568
		[SerializeField]
		[HideInInspector]
		public Vector2 _offset;

		// Token: 0x04002D31 RID: 11569
		[SerializeField]
		[HideInInspector]
		public Vector3 _rotationOffset = Vector3.zero;

		// Token: 0x04002D34 RID: 11572
		[SerializeField]
		[HideInInspector]
		private Vector3 lastPosition = Vector3.zero;

		// Token: 0x020009A5 RID: 2469
		public enum Mode
		{
			// Token: 0x040044FB RID: 17659
			Accurate,
			// Token: 0x040044FC RID: 17660
			Cached
		}
	}
}
