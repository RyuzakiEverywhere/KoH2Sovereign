using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dreamteck.Splines
{
	// Token: 0x020004C6 RID: 1222
	public class SplineTracer : SplineUser
	{
		// Token: 0x1700048A RID: 1162
		// (get) Token: 0x0600409C RID: 16540 RVA: 0x001EBB1E File Offset: 0x001E9D1E
		// (set) Token: 0x0600409D RID: 16541 RVA: 0x001EBB26 File Offset: 0x001E9D26
		public SplineTracer.PhysicsMode physicsMode
		{
			get
			{
				return this._physicsMode;
			}
			set
			{
				this._physicsMode = value;
				this.RefreshTargets();
			}
		}

		// Token: 0x1700048B RID: 1163
		// (get) Token: 0x0600409E RID: 16542 RVA: 0x001EBB35 File Offset: 0x001E9D35
		public TransformModule motion
		{
			get
			{
				if (this._motion == null)
				{
					this._motion = new TransformModule();
				}
				return this._motion;
			}
		}

		// Token: 0x1700048C RID: 1164
		// (get) Token: 0x0600409F RID: 16543 RVA: 0x001EBB50 File Offset: 0x001E9D50
		public SplineSample result
		{
			get
			{
				return this._result;
			}
		}

		// Token: 0x1700048D RID: 1165
		// (get) Token: 0x060040A0 RID: 16544 RVA: 0x001EBB58 File Offset: 0x001E9D58
		public SplineSample modifiedResult
		{
			get
			{
				return this._finalResult;
			}
		}

		// Token: 0x1700048E RID: 1166
		// (get) Token: 0x060040A1 RID: 16545 RVA: 0x001EBB60 File Offset: 0x001E9D60
		// (set) Token: 0x060040A2 RID: 16546 RVA: 0x001EBB68 File Offset: 0x001E9D68
		public Spline.Direction direction
		{
			get
			{
				return this._direction;
			}
			set
			{
				if (value != this._direction)
				{
					this._direction = value;
					this.ApplyMotion();
				}
			}
		}

		// Token: 0x14000047 RID: 71
		// (add) Token: 0x060040A3 RID: 16547 RVA: 0x001EBB80 File Offset: 0x001E9D80
		// (remove) Token: 0x060040A4 RID: 16548 RVA: 0x001EBBB8 File Offset: 0x001E9DB8
		public event SplineTracer.JunctionHandler onNode;

		// Token: 0x14000048 RID: 72
		// (add) Token: 0x060040A5 RID: 16549 RVA: 0x001EBBF0 File Offset: 0x001E9DF0
		// (remove) Token: 0x060040A6 RID: 16550 RVA: 0x001EBC28 File Offset: 0x001E9E28
		public event EmptySplineHandler onMotionApplied;

		// Token: 0x060040A7 RID: 16551 RVA: 0x001EBC5D File Offset: 0x001E9E5D
		protected virtual void Start()
		{
			this.RefreshTargets();
		}

		// Token: 0x060040A8 RID: 16552 RVA: 0x001EBC68 File Offset: 0x001E9E68
		public virtual void SetPercent(double percent, bool checkTriggers = false, bool handleJunctions = false)
		{
			if (base.sampleCount == 0)
			{
				return;
			}
			double percent2 = this._result.percent;
			base.Evaluate(percent, this._result);
			this.ApplyMotion();
			if (checkTriggers)
			{
				this.CheckTriggers(percent2, percent);
				this.InvokeTriggers();
			}
			if (handleJunctions)
			{
				this.CheckNodes(percent2, percent);
			}
		}

		// Token: 0x060040A9 RID: 16553 RVA: 0x001EBCBC File Offset: 0x001E9EBC
		public virtual void SetDistance(float distance, bool checkTriggers = false, bool handleJunctions = false)
		{
			if (base.sampleCount == 0)
			{
				return;
			}
			double percent = this._result.percent;
			base.Evaluate(base.Travel(0.0, distance, Spline.Direction.Forward), this._result);
			this.ApplyMotion();
			if (checkTriggers)
			{
				this.CheckTriggers(percent, this._result.percent);
				this.InvokeTriggers();
			}
			if (handleJunctions)
			{
				this.CheckNodes(percent, this._result.percent);
			}
		}

		// Token: 0x060040AA RID: 16554 RVA: 0x001EBD31 File Offset: 0x001E9F31
		protected override void PostBuild()
		{
			if (this.setPercentOnRebuild)
			{
				this.SetPercent(this.targetPercentOnRebuild, false, false);
				this.setPercentOnRebuild = false;
			}
		}

		// Token: 0x060040AB RID: 16555 RVA: 0x001EBD50 File Offset: 0x001E9F50
		protected virtual Rigidbody GetRigidbody()
		{
			return base.GetComponent<Rigidbody>();
		}

		// Token: 0x060040AC RID: 16556 RVA: 0x001EBD58 File Offset: 0x001E9F58
		protected virtual Rigidbody2D GetRigidbody2D()
		{
			return base.GetComponent<Rigidbody2D>();
		}

		// Token: 0x060040AD RID: 16557 RVA: 0x000742D9 File Offset: 0x000724D9
		protected virtual Transform GetTransform()
		{
			return base.transform;
		}

		// Token: 0x060040AE RID: 16558 RVA: 0x001EBD60 File Offset: 0x001E9F60
		protected void ApplyMotion()
		{
			base.ModifySample(this._result, this._finalResult);
			this.motion.targetUser = this;
			this.motion.splineResult = this._finalResult;
			if (this.applyDirectionRotation)
			{
				this.motion.direction = this._direction;
			}
			else
			{
				this.motion.direction = Spline.Direction.Forward;
			}
			switch (this._physicsMode)
			{
			case SplineTracer.PhysicsMode.Transform:
				if (this.targetTransform == null)
				{
					this.RefreshTargets();
				}
				if (this.targetTransform == null)
				{
					return;
				}
				this.motion.ApplyTransform(this.targetTransform);
				if (this.onMotionApplied != null)
				{
					this.onMotionApplied();
					return;
				}
				break;
			case SplineTracer.PhysicsMode.Rigidbody:
				if (this.targetRigidbody == null)
				{
					this.RefreshTargets();
					if (this.targetRigidbody == null)
					{
						throw new MissingComponentException("There is no Rigidbody attached to " + base.name + " but the Physics mode is set to use one.");
					}
				}
				this.motion.ApplyRigidbody(this.targetRigidbody);
				if (this.onMotionApplied != null)
				{
					this.onMotionApplied();
					return;
				}
				break;
			case SplineTracer.PhysicsMode.Rigidbody2D:
				if (this.targetRigidbody2D == null)
				{
					this.RefreshTargets();
					if (this.targetRigidbody2D == null)
					{
						throw new MissingComponentException("There is no Rigidbody2D attached to " + base.name + " but the Physics mode is set to use one.");
					}
				}
				this.motion.ApplyRigidbody2D(this.targetRigidbody2D);
				if (this.onMotionApplied != null)
				{
					this.onMotionApplied();
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x060040AF RID: 16559 RVA: 0x001EBEEC File Offset: 0x001EA0EC
		protected void CheckNodes(double from, double to)
		{
			if (this.onNode == null)
			{
				return;
			}
			if (from == to)
			{
				return;
			}
			base.UnclipPercent(ref from);
			base.UnclipPercent(ref to);
			Spline.FormatFromTo(ref from, ref to, true);
			int num = base.spline.PercentToPointIndex(from, this._direction);
			int num2 = base.spline.PercentToPointIndex(to, this._direction);
			if (num == num2)
			{
				if (from < 1E-06 && to > from)
				{
					SplineTracer.NodeConnection junction = this.GetJunction(0);
					if (junction != null)
					{
						this.nodeConnectionQueue.Add(junction);
						return;
					}
				}
				else if (to > 0.999999 && from < to)
				{
					SplineTracer.NodeConnection junction2 = this.GetJunction(base.spline.pointCount - 1);
					if (junction2 != null)
					{
						this.nodeConnectionQueue.Add(junction2);
					}
				}
				return;
			}
			if (this._direction == Spline.Direction.Forward)
			{
				for (int i = num + 1; i <= num2; i++)
				{
					SplineTracer.NodeConnection junction3 = this.GetJunction(i);
					if (junction3 != null)
					{
						this.nodeConnectionQueue.Add(junction3);
					}
				}
				return;
			}
			for (int j = num2 - 1; j >= num; j--)
			{
				SplineTracer.NodeConnection junction4 = this.GetJunction(j);
				if (junction4 != null)
				{
					this.nodeConnectionQueue.Add(junction4);
				}
			}
		}

		// Token: 0x060040B0 RID: 16560 RVA: 0x001EC009 File Offset: 0x001EA209
		protected void InvokeNodes()
		{
			if (this.nodeConnectionQueue.Count > 0)
			{
				this.onNode(this.nodeConnectionQueue);
				this.nodeConnectionQueue.Clear();
			}
		}

		// Token: 0x060040B1 RID: 16561 RVA: 0x001EC038 File Offset: 0x001EA238
		protected void CheckTriggers(double from, double to)
		{
			if (!this.useTriggers)
			{
				return;
			}
			if (from == to)
			{
				return;
			}
			base.UnclipPercent(ref from);
			base.UnclipPercent(ref to);
			if (this.triggerGroup < 0 || this.triggerGroup >= base.spline.triggerGroups.Length)
			{
				return;
			}
			for (int i = 0; i < base.spline.triggerGroups[this.triggerGroup].triggers.Length; i++)
			{
				if (base.spline.triggerGroups[this.triggerGroup].triggers[i] != null && base.spline.triggerGroups[this.triggerGroup].triggers[i].Check(from, to))
				{
					this.AddTriggerToQueue(base.spline.triggerGroups[this.triggerGroup].triggers[i]);
				}
			}
		}

		// Token: 0x060040B2 RID: 16562 RVA: 0x001EC104 File Offset: 0x001EA304
		private SplineTracer.NodeConnection GetJunction(int pointIndex)
		{
			Node node = base.spline.GetNode(pointIndex);
			if (node == null)
			{
				return null;
			}
			return new SplineTracer.NodeConnection(node, pointIndex);
		}

		// Token: 0x060040B3 RID: 16563 RVA: 0x001EC130 File Offset: 0x001EA330
		protected void InvokeTriggers()
		{
			for (int i = 0; i < this.addTriggerIndex; i++)
			{
				if (this.triggerInvokeQueue[i] != null)
				{
					this.triggerInvokeQueue[i].Invoke(this);
				}
			}
			this.addTriggerIndex = 0;
		}

		// Token: 0x060040B4 RID: 16564 RVA: 0x001EC170 File Offset: 0x001EA370
		protected void RefreshTargets()
		{
			switch (this._physicsMode)
			{
			case SplineTracer.PhysicsMode.Transform:
				this.targetTransform = this.GetTransform();
				return;
			case SplineTracer.PhysicsMode.Rigidbody:
				this.targetRigidbody = this.GetRigidbody();
				return;
			case SplineTracer.PhysicsMode.Rigidbody2D:
				this.targetRigidbody2D = this.GetRigidbody2D();
				return;
			default:
				return;
			}
		}

		// Token: 0x060040B5 RID: 16565 RVA: 0x001EC1C0 File Offset: 0x001EA3C0
		private void AddTriggerToQueue(SplineTrigger trigger)
		{
			if (this.addTriggerIndex >= this.triggerInvokeQueue.Length)
			{
				SplineTrigger[] array = new SplineTrigger[this.triggerInvokeQueue.Length + base.spline.triggerGroups[this.triggerGroup].triggers.Length];
				this.triggerInvokeQueue.CopyTo(array, 0);
				this.triggerInvokeQueue = array;
			}
			this.triggerInvokeQueue[this.addTriggerIndex] = trigger;
			this.addTriggerIndex++;
		}

		// Token: 0x04002D3C RID: 11580
		private SplineTrigger[] triggerInvokeQueue = new SplineTrigger[0];

		// Token: 0x04002D3D RID: 11581
		private List<SplineTracer.NodeConnection> nodeConnectionQueue = new List<SplineTracer.NodeConnection>();

		// Token: 0x04002D3E RID: 11582
		private int addTriggerIndex;

		// Token: 0x04002D3F RID: 11583
		[HideInInspector]
		public bool applyDirectionRotation = true;

		// Token: 0x04002D40 RID: 11584
		[HideInInspector]
		public bool useTriggers;

		// Token: 0x04002D41 RID: 11585
		[HideInInspector]
		public int triggerGroup;

		// Token: 0x04002D42 RID: 11586
		[SerializeField]
		[HideInInspector]
		protected Spline.Direction _direction = Spline.Direction.Forward;

		// Token: 0x04002D43 RID: 11587
		[SerializeField]
		[HideInInspector]
		protected SplineTracer.PhysicsMode _physicsMode;

		// Token: 0x04002D44 RID: 11588
		[SerializeField]
		[HideInInspector]
		protected TransformModule _motion;

		// Token: 0x04002D45 RID: 11589
		[SerializeField]
		[HideInInspector]
		protected Rigidbody targetRigidbody;

		// Token: 0x04002D46 RID: 11590
		[SerializeField]
		[HideInInspector]
		protected Rigidbody2D targetRigidbody2D;

		// Token: 0x04002D47 RID: 11591
		[SerializeField]
		[HideInInspector]
		protected Transform targetTransform;

		// Token: 0x04002D48 RID: 11592
		[SerializeField]
		[HideInInspector]
		protected SplineSample _result = new SplineSample();

		// Token: 0x04002D49 RID: 11593
		[SerializeField]
		[HideInInspector]
		protected SplineSample _finalResult = new SplineSample();

		// Token: 0x04002D4A RID: 11594
		private bool setPercentOnRebuild;

		// Token: 0x04002D4B RID: 11595
		private double targetPercentOnRebuild;

		// Token: 0x04002D4E RID: 11598
		private const double MIN_DELTA = 1E-06;

		// Token: 0x020009A6 RID: 2470
		public class NodeConnection
		{
			// Token: 0x060054AB RID: 21675 RVA: 0x00247318 File Offset: 0x00245518
			public NodeConnection(Node node, int point)
			{
				this.node = node;
				this.point = point;
			}

			// Token: 0x040044FD RID: 17661
			public Node node;

			// Token: 0x040044FE RID: 17662
			public int point;
		}

		// Token: 0x020009A7 RID: 2471
		public enum PhysicsMode
		{
			// Token: 0x04004500 RID: 17664
			Transform,
			// Token: 0x04004501 RID: 17665
			Rigidbody,
			// Token: 0x04004502 RID: 17666
			Rigidbody2D
		}

		// Token: 0x020009A8 RID: 2472
		// (Invoke) Token: 0x060054AD RID: 21677
		public delegate void JunctionHandler(List<SplineTracer.NodeConnection> passed);
	}
}
