using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dreamteck.Splines
{
	// Token: 0x020004C7 RID: 1223
	[ExecuteInEditMode]
	public class SplineUser : MonoBehaviour
	{
		// Token: 0x1700048F RID: 1167
		// (get) Token: 0x060040B7 RID: 16567 RVA: 0x001EC286 File Offset: 0x001EA486
		// (set) Token: 0x060040B8 RID: 16568 RVA: 0x001EC290 File Offset: 0x001EA490
		public SplineComputer spline
		{
			get
			{
				return this._spline;
			}
			set
			{
				if (value != this._spline)
				{
					if (this._spline != null)
					{
						this._spline.Unsubscribe(this);
					}
					this._spline = value;
					if (this._spline != null)
					{
						this._spline.Subscribe(this);
						this.Rebuild();
					}
					this.OnSplineChanged();
				}
			}
		}

		// Token: 0x17000490 RID: 1168
		// (get) Token: 0x060040B9 RID: 16569 RVA: 0x001EC2F2 File Offset: 0x001EA4F2
		// (set) Token: 0x060040BA RID: 16570 RVA: 0x001EC300 File Offset: 0x001EA500
		public double clipFrom
		{
			get
			{
				return this.sampleCollection.clipFrom;
			}
			set
			{
				if (value != this.sampleCollection.clipFrom)
				{
					this.animClipFrom = (float)this.sampleCollection.clipFrom;
					this.sampleCollection.clipFrom = DMath.Clamp01(value);
					if (this.sampleCollection.clipFrom > this.sampleCollection.clipTo && !this._spline.isClosed)
					{
						this.sampleCollection.clipTo = this.sampleCollection.clipFrom;
					}
					this.getSamples = true;
					this.Rebuild();
				}
			}
		}

		// Token: 0x17000491 RID: 1169
		// (get) Token: 0x060040BB RID: 16571 RVA: 0x001EC386 File Offset: 0x001EA586
		// (set) Token: 0x060040BC RID: 16572 RVA: 0x001EC394 File Offset: 0x001EA594
		public double clipTo
		{
			get
			{
				return this.sampleCollection.clipTo;
			}
			set
			{
				if (value != this.sampleCollection.clipTo)
				{
					this.animClipTo = (float)this.sampleCollection.clipTo;
					this.sampleCollection.clipTo = DMath.Clamp01(value);
					if (this.sampleCollection.clipTo < this.sampleCollection.clipFrom && !this._spline.isClosed)
					{
						this.sampleCollection.clipFrom = this.sampleCollection.clipTo;
					}
					this.getSamples = true;
					this.Rebuild();
				}
			}
		}

		// Token: 0x17000492 RID: 1170
		// (get) Token: 0x060040BD RID: 16573 RVA: 0x001EC41A File Offset: 0x001EA61A
		// (set) Token: 0x060040BE RID: 16574 RVA: 0x001EC422 File Offset: 0x001EA622
		public bool autoUpdate
		{
			get
			{
				return this._autoUpdate;
			}
			set
			{
				if (value != this._autoUpdate)
				{
					this._autoUpdate = value;
					if (value)
					{
						this.Rebuild();
					}
				}
			}
		}

		// Token: 0x17000493 RID: 1171
		// (get) Token: 0x060040BF RID: 16575 RVA: 0x001EC43D File Offset: 0x001EA63D
		// (set) Token: 0x060040C0 RID: 16576 RVA: 0x001EC44C File Offset: 0x001EA64C
		public bool loopSamples
		{
			get
			{
				return this.sampleCollection.loopSamples;
			}
			set
			{
				if (value != this.sampleCollection.loopSamples)
				{
					this.sampleCollection.loopSamples = value;
					if (!this.sampleCollection.loopSamples && this.sampleCollection.clipTo < this.sampleCollection.clipFrom)
					{
						double clipTo = this.sampleCollection.clipTo;
						this.sampleCollection.clipTo = this.sampleCollection.clipFrom;
						this.sampleCollection.clipFrom = clipTo;
					}
					this.Rebuild();
				}
			}
		}

		// Token: 0x17000494 RID: 1172
		// (get) Token: 0x060040C1 RID: 16577 RVA: 0x001EC4CC File Offset: 0x001EA6CC
		public double span
		{
			get
			{
				if (this.samplesAreLooped)
				{
					return 1.0 - this.sampleCollection.clipFrom + this.sampleCollection.clipTo;
				}
				return this.sampleCollection.clipTo - this.sampleCollection.clipFrom;
			}
		}

		// Token: 0x17000495 RID: 1173
		// (get) Token: 0x060040C2 RID: 16578 RVA: 0x001EC51A File Offset: 0x001EA71A
		public bool samplesAreLooped
		{
			get
			{
				return this.sampleCollection.samplesAreLooped;
			}
		}

		// Token: 0x17000496 RID: 1174
		// (get) Token: 0x060040C3 RID: 16579 RVA: 0x001EC527 File Offset: 0x001EA727
		public RotationModifier rotationModifier
		{
			get
			{
				return this._rotationModifier;
			}
		}

		// Token: 0x17000497 RID: 1175
		// (get) Token: 0x060040C4 RID: 16580 RVA: 0x001EC52F File Offset: 0x001EA72F
		public OffsetModifier offsetModifier
		{
			get
			{
				return this._offsetModifier;
			}
		}

		// Token: 0x17000498 RID: 1176
		// (get) Token: 0x060040C5 RID: 16581 RVA: 0x001EC537 File Offset: 0x001EA737
		public ColorModifier colorModifier
		{
			get
			{
				return this._colorModifier;
			}
		}

		// Token: 0x17000499 RID: 1177
		// (get) Token: 0x060040C6 RID: 16582 RVA: 0x001EC53F File Offset: 0x001EA73F
		public SizeModifier sizeModifier
		{
			get
			{
				return this._sizeModifier;
			}
		}

		// Token: 0x1700049A RID: 1178
		// (get) Token: 0x060040C7 RID: 16583 RVA: 0x001EC547 File Offset: 0x001EA747
		public int sampleCount
		{
			get
			{
				return this._sampleCount;
			}
		}

		// Token: 0x060040C8 RID: 16584 RVA: 0x001EC550 File Offset: 0x001EA750
		protected virtual void Awake()
		{
			this.trs = base.transform;
			if (this.spline == null)
			{
				this.spline = base.GetComponent<SplineComputer>();
			}
			else if (!this.spline.IsSubscribed(this))
			{
				this.spline.Subscribe(this);
			}
			if (this.buildOnAwake)
			{
				this.RebuildImmediate();
			}
		}

		// Token: 0x060040C9 RID: 16585 RVA: 0x000023FD File Offset: 0x000005FD
		protected virtual void Reset()
		{
		}

		// Token: 0x060040CA RID: 16586 RVA: 0x001EC5AD File Offset: 0x001EA7AD
		protected virtual void OnEnable()
		{
			if (this.spline != null)
			{
				this.spline.Subscribe(this);
			}
			if (this.buildOnEnable)
			{
				this.RebuildImmediate();
			}
		}

		// Token: 0x060040CB RID: 16587 RVA: 0x001EC5D7 File Offset: 0x001EA7D7
		protected virtual void OnDisable()
		{
			if (this.spline != null)
			{
				this.spline.Unsubscribe(this);
			}
		}

		// Token: 0x060040CC RID: 16588 RVA: 0x000023FD File Offset: 0x000005FD
		protected virtual void OnDestroy()
		{
		}

		// Token: 0x060040CD RID: 16589 RVA: 0x001EC5F4 File Offset: 0x001EA7F4
		protected virtual void OnDidApplyAnimationProperties()
		{
			bool flag = false;
			if (this.sampleCollection.clipFrom != (double)this.animClipFrom || this.sampleCollection.clipTo != (double)this.animClipTo)
			{
				flag = true;
			}
			this.sampleCollection.clipFrom = (double)this.animClipFrom;
			this.sampleCollection.clipTo = (double)this.animClipTo;
			this.Rebuild();
			if (flag)
			{
				this.GetSamples();
			}
		}

		// Token: 0x060040CE RID: 16590 RVA: 0x001EC660 File Offset: 0x001EA860
		public SplineSample GetSampleRaw(int index)
		{
			if (index >= this._sampleCount)
			{
				index = this._sampleCount - 1;
			}
			if (this.sampleCollection.samplesAreLooped)
			{
				int num;
				double num2;
				this.sampleCollection.GetSamplingValues(this.clipFrom, out num, out num2);
				int num3;
				this.sampleCollection.GetSamplingValues(this.clipTo, out num3, out num2);
				if (index == 0)
				{
					return this.clipFromSample;
				}
				int num4 = num3;
				if (num2 > 0.0)
				{
					num4++;
				}
				if (index == this._sampleCount - 1)
				{
					return this.clipToSample;
				}
				int num5 = num + index;
				if (num5 >= this.sampleCollection.Count)
				{
					num5 -= this.sampleCollection.Count;
				}
				return this.sampleCollection.samples[num5];
			}
			else
			{
				if (index == 0)
				{
					return this.clipFromSample;
				}
				if (index == this._sampleCount - 1)
				{
					return this.clipToSample;
				}
				return this.sampleCollection.samples[this.startSampleIndex + index];
			}
		}

		// Token: 0x060040CF RID: 16591 RVA: 0x001EC74B File Offset: 0x001EA94B
		public void GetSample(int index, SplineSample target)
		{
			this.ModifySample(this.GetSampleRaw(index), target);
		}

		// Token: 0x060040D0 RID: 16592 RVA: 0x001EC75C File Offset: 0x001EA95C
		public virtual void Rebuild()
		{
			if (!this.autoUpdate)
			{
				return;
			}
			this.rebuild = (this.getSamples = true);
		}

		// Token: 0x060040D1 RID: 16593 RVA: 0x001EC784 File Offset: 0x001EA984
		public virtual void RebuildImmediate()
		{
			try
			{
				this.GetSamples();
				this.Build();
				this.PostBuild();
			}
			catch (Exception ex)
			{
				Debug.Log(ex.Message);
			}
			this.rebuild = false;
			this.getSamples = false;
		}

		// Token: 0x060040D2 RID: 16594 RVA: 0x001EC7D0 File Offset: 0x001EA9D0
		private void Update()
		{
			if (this.updateMethod == SplineUser.UpdateMethod.Update)
			{
				this.Run();
				this.RunUpdate();
				this.LateRun();
			}
		}

		// Token: 0x060040D3 RID: 16595 RVA: 0x001EC7EC File Offset: 0x001EA9EC
		private void LateUpdate()
		{
			if (this.updateMethod == SplineUser.UpdateMethod.LateUpdate)
			{
				this.Run();
				this.RunUpdate();
				this.LateRun();
			}
		}

		// Token: 0x060040D4 RID: 16596 RVA: 0x001EC809 File Offset: 0x001EAA09
		private void FixedUpdate()
		{
			if (this.updateMethod == SplineUser.UpdateMethod.FixedUpdate)
			{
				this.Run();
				this.RunUpdate();
				this.LateRun();
			}
		}

		// Token: 0x060040D5 RID: 16597 RVA: 0x001EC828 File Offset: 0x001EAA28
		private void RunUpdate()
		{
			if (this.rebuild)
			{
				if (this.multithreaded)
				{
					if (this.getSamples)
					{
						SplineThreading.Run(new SplineThreading.EmptyHandler(this.ResampleAndBuildThreaded));
					}
					else
					{
						SplineThreading.Run(new SplineThreading.EmptyHandler(this.BuildThreaded));
					}
				}
				else
				{
					if (this.getSamples || this.spline.sampleMode == SplineComputer.SampleMode.Optimized)
					{
						this.GetSamples();
					}
					this.Build();
					this.postBuild = true;
				}
				this.rebuild = false;
			}
			if (this.postBuild)
			{
				this.PostBuild();
				this.postBuild = false;
			}
		}

		// Token: 0x060040D6 RID: 16598 RVA: 0x001EC8BA File Offset: 0x001EAABA
		private void BuildThreaded()
		{
			this.Build();
			this.postBuild = true;
		}

		// Token: 0x060040D7 RID: 16599 RVA: 0x001EC8C9 File Offset: 0x001EAAC9
		private void ResampleAndBuildThreaded()
		{
			this.GetSamples();
			this.Build();
			this.postBuild = true;
		}

		// Token: 0x060040D8 RID: 16600 RVA: 0x000023FD File Offset: 0x000005FD
		protected virtual void Run()
		{
		}

		// Token: 0x060040D9 RID: 16601 RVA: 0x000023FD File Offset: 0x000005FD
		protected virtual void LateRun()
		{
		}

		// Token: 0x060040DA RID: 16602 RVA: 0x000023FD File Offset: 0x000005FD
		protected virtual void Build()
		{
		}

		// Token: 0x060040DB RID: 16603 RVA: 0x000023FD File Offset: 0x000005FD
		protected virtual void PostBuild()
		{
		}

		// Token: 0x060040DC RID: 16604 RVA: 0x000023FD File Offset: 0x000005FD
		protected virtual void OnSplineChanged()
		{
		}

		// Token: 0x060040DD RID: 16605 RVA: 0x001EC8DE File Offset: 0x001EAADE
		public void ModifySample(SplineSample source, SplineSample destination)
		{
			destination.CopyFrom(source);
			this.ModifySample(destination);
		}

		// Token: 0x060040DE RID: 16606 RVA: 0x001EC8EE File Offset: 0x001EAAEE
		public void ModifySample(SplineSample sample)
		{
			this.offsetModifier.Apply(sample);
			this._rotationModifier.Apply(sample);
			this._colorModifier.Apply(sample);
			this._sizeModifier.Apply(sample);
		}

		// Token: 0x060040DF RID: 16607 RVA: 0x001EC920 File Offset: 0x001EAB20
		public void SetClipRange(double from, double to)
		{
			if (!this._spline.isClosed && to < from)
			{
				to = from;
			}
			this.sampleCollection.clipFrom = DMath.Clamp01(from);
			this.sampleCollection.clipTo = DMath.Clamp01(to);
			this.GetSamples();
			this.Rebuild();
		}

		// Token: 0x060040E0 RID: 16608 RVA: 0x001EC970 File Offset: 0x001EAB70
		private void GetSamples()
		{
			if (this.spline == null)
			{
				return;
			}
			this.getSamples = false;
			this.spline.GetSamples(this.sampleCollection);
			this.sampleCollection.Evaluate(0.0, this.clipFromSample);
			this.sampleCollection.Evaluate(1.0, this.clipToSample);
			int num;
			int num2;
			this._sampleCount = this.sampleCollection.GetClippedSampleCount(out num, out num2);
			double num3;
			this.sampleCollection.GetSamplingValues(this.sampleCollection.clipFrom, out this.startSampleIndex, out num3);
		}

		// Token: 0x060040E1 RID: 16609 RVA: 0x001ECA0B File Offset: 0x001EAC0B
		public double ClipPercent(double percent)
		{
			this.ClipPercent(ref percent);
			return percent;
		}

		// Token: 0x060040E2 RID: 16610 RVA: 0x001ECA16 File Offset: 0x001EAC16
		public void ClipPercent(ref double percent)
		{
			this.sampleCollection.ClipPercent(ref percent);
		}

		// Token: 0x060040E3 RID: 16611 RVA: 0x001ECA24 File Offset: 0x001EAC24
		public double UnclipPercent(double percent)
		{
			this.UnclipPercent(ref percent);
			return percent;
		}

		// Token: 0x060040E4 RID: 16612 RVA: 0x001ECA2F File Offset: 0x001EAC2F
		public void UnclipPercent(ref double percent)
		{
			this.sampleCollection.UnclipPercent(ref percent);
		}

		// Token: 0x060040E5 RID: 16613 RVA: 0x001ECA3D File Offset: 0x001EAC3D
		private int GetSampleIndex(double percent)
		{
			return DMath.FloorInt(percent * (double)(this.sampleCollection.Count - 1));
		}

		// Token: 0x060040E6 RID: 16614 RVA: 0x001ECA54 File Offset: 0x001EAC54
		public Vector3 EvaluatePosition(double percent)
		{
			return this.sampleCollection.EvaluatePosition(percent);
		}

		// Token: 0x060040E7 RID: 16615 RVA: 0x001ECA62 File Offset: 0x001EAC62
		public void Evaluate(double percent, SplineSample result)
		{
			this.sampleCollection.Evaluate(percent, result);
			result.percent = DMath.Clamp01(percent);
		}

		// Token: 0x060040E8 RID: 16616 RVA: 0x001ECA80 File Offset: 0x001EAC80
		public SplineSample Evaluate(double percent)
		{
			SplineSample splineSample = new SplineSample();
			this.Evaluate(percent, splineSample);
			splineSample.percent = DMath.Clamp01(percent);
			return splineSample;
		}

		// Token: 0x060040E9 RID: 16617 RVA: 0x001ECAA8 File Offset: 0x001EACA8
		public void Evaluate(ref SplineSample[] results, double from = 0.0, double to = 1.0)
		{
			this.sampleCollection.Evaluate(ref results, from, to);
			for (int i = 0; i < results.Length; i++)
			{
				this.ClipPercent(ref results[i].percent);
			}
		}

		// Token: 0x060040EA RID: 16618 RVA: 0x001ECAE1 File Offset: 0x001EACE1
		public void EvaluatePositions(ref Vector3[] positions, double from = 0.0, double to = 1.0)
		{
			this.sampleCollection.EvaluatePositions(ref positions, from, to);
		}

		// Token: 0x060040EB RID: 16619 RVA: 0x001ECAF4 File Offset: 0x001EACF4
		public double Travel(double start, float distance, Spline.Direction direction, out float moved)
		{
			moved = 0f;
			if (direction == Spline.Direction.Forward && start >= 1.0)
			{
				return 1.0;
			}
			if (direction == Spline.Direction.Backward && start <= 0.0)
			{
				return 0.0;
			}
			if (distance == 0f)
			{
				return DMath.Clamp01(start);
			}
			double percent = this.sampleCollection.Travel(start, distance, direction, out moved);
			return this.ClipPercent(percent);
		}

		// Token: 0x060040EC RID: 16620 RVA: 0x001ECB64 File Offset: 0x001EAD64
		public double Travel(double start, float distance, Spline.Direction direction = Spline.Direction.Forward)
		{
			float num;
			return this.Travel(start, distance, direction, out num);
		}

		// Token: 0x060040ED RID: 16621 RVA: 0x001ECB7C File Offset: 0x001EAD7C
		public virtual void Project(Vector3 position, SplineSample result, double from = 0.0, double to = 1.0)
		{
			if (this._spline == null)
			{
				return;
			}
			this.sampleCollection.Project(position, this._spline.pointCount, result, from, to);
			this.ClipPercent(ref result.percent);
		}

		// Token: 0x060040EE RID: 16622 RVA: 0x001ECBB4 File Offset: 0x001EADB4
		public float CalculateLength(double from = 0.0, double to = 1.0)
		{
			return this.sampleCollection.CalculateLength(from, to);
		}

		// Token: 0x04002D4F RID: 11599
		[HideInInspector]
		public SplineUser.UpdateMethod updateMethod;

		// Token: 0x04002D50 RID: 11600
		[SerializeField]
		[HideInInspector]
		[FormerlySerializedAs("_computer")]
		private SplineComputer _spline;

		// Token: 0x04002D51 RID: 11601
		[SerializeField]
		[HideInInspector]
		private bool _autoUpdate = true;

		// Token: 0x04002D52 RID: 11602
		[SerializeField]
		[HideInInspector]
		protected RotationModifier _rotationModifier = new RotationModifier();

		// Token: 0x04002D53 RID: 11603
		[SerializeField]
		[HideInInspector]
		protected OffsetModifier _offsetModifier = new OffsetModifier();

		// Token: 0x04002D54 RID: 11604
		[SerializeField]
		[HideInInspector]
		protected ColorModifier _colorModifier = new ColorModifier();

		// Token: 0x04002D55 RID: 11605
		[SerializeField]
		[HideInInspector]
		protected SizeModifier _sizeModifier = new SizeModifier();

		// Token: 0x04002D56 RID: 11606
		[SerializeField]
		[HideInInspector]
		private SampleCollection sampleCollection = new SampleCollection();

		// Token: 0x04002D57 RID: 11607
		[SerializeField]
		[HideInInspector]
		private SplineSample clipFromSample = new SplineSample();

		// Token: 0x04002D58 RID: 11608
		[SerializeField]
		[HideInInspector]
		private SplineSample clipToSample = new SplineSample();

		// Token: 0x04002D59 RID: 11609
		[SerializeField]
		[HideInInspector]
		private float animClipFrom;

		// Token: 0x04002D5A RID: 11610
		[SerializeField]
		[HideInInspector]
		private float animClipTo = 1f;

		// Token: 0x04002D5B RID: 11611
		private bool rebuild;

		// Token: 0x04002D5C RID: 11612
		private bool getSamples;

		// Token: 0x04002D5D RID: 11613
		private bool postBuild;

		// Token: 0x04002D5E RID: 11614
		protected Transform trs;

		// Token: 0x04002D5F RID: 11615
		[SerializeField]
		[HideInInspector]
		private int _sampleCount;

		// Token: 0x04002D60 RID: 11616
		[SerializeField]
		[HideInInspector]
		private int startSampleIndex;

		// Token: 0x04002D61 RID: 11617
		protected SplineSample evalResult = new SplineSample();

		// Token: 0x04002D62 RID: 11618
		[HideInInspector]
		public volatile bool multithreaded;

		// Token: 0x04002D63 RID: 11619
		[HideInInspector]
		public bool buildOnAwake;

		// Token: 0x04002D64 RID: 11620
		[HideInInspector]
		public bool buildOnEnable;

		// Token: 0x020009A9 RID: 2473
		public enum UpdateMethod
		{
			// Token: 0x04004504 RID: 17668
			Update,
			// Token: 0x04004505 RID: 17669
			FixedUpdate,
			// Token: 0x04004506 RID: 17670
			LateUpdate
		}
	}
}
