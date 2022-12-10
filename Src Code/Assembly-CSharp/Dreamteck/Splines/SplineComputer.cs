using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dreamteck.Splines
{
	// Token: 0x020004BE RID: 1214
	[AddComponentMenu("Dreamteck/Splines/Spline Computer")]
	[ExecuteInEditMode]
	public class SplineComputer : MonoBehaviour
	{
		// Token: 0x17000467 RID: 1127
		// (get) Token: 0x06003FAD RID: 16301 RVA: 0x001E63BA File Offset: 0x001E45BA
		// (set) Token: 0x06003FAE RID: 16302 RVA: 0x001E63C4 File Offset: 0x001E45C4
		public SplineComputer.Space space
		{
			get
			{
				return this._space;
			}
			set
			{
				if (value != this._space)
				{
					SplinePoint[] points = this.GetPoints(SplineComputer.Space.World);
					this._space = value;
					if (this._space == SplineComputer.Space.Local)
					{
						this._transformedSamples = new SplineSample[this._rawSamples.Length];
						for (int i = 0; i < this._transformedSamples.Length; i++)
						{
							this._transformedSamples[i] = new SplineSample();
						}
					}
					this.SetPoints(points, SplineComputer.Space.World);
					this.Rebuild(true);
				}
			}
		}

		// Token: 0x17000468 RID: 1128
		// (get) Token: 0x06003FAF RID: 16303 RVA: 0x001E6434 File Offset: 0x001E4634
		// (set) Token: 0x06003FB0 RID: 16304 RVA: 0x001E6441 File Offset: 0x001E4641
		public Spline.Type type
		{
			get
			{
				return this.spline.type;
			}
			set
			{
				if (value != this.spline.type)
				{
					this.spline.type = value;
					this.Rebuild(true);
				}
			}
		}

		// Token: 0x17000469 RID: 1129
		// (get) Token: 0x06003FB1 RID: 16305 RVA: 0x001E6464 File Offset: 0x001E4664
		// (set) Token: 0x06003FB2 RID: 16306 RVA: 0x001E6471 File Offset: 0x001E4671
		public bool linearAverageDirection
		{
			get
			{
				return this.spline.linearAverageDirection;
			}
			set
			{
				if (value != this.spline.linearAverageDirection)
				{
					this.spline.linearAverageDirection = value;
					this.Rebuild(true);
				}
			}
		}

		// Token: 0x1700046A RID: 1130
		// (get) Token: 0x06003FB3 RID: 16307 RVA: 0x001E6494 File Offset: 0x001E4694
		// (set) Token: 0x06003FB4 RID: 16308 RVA: 0x001E649C File Offset: 0x001E469C
		public bool is2D
		{
			get
			{
				return this._is2D;
			}
			set
			{
				if (value != this._is2D)
				{
					this._is2D = value;
					this.SetPoints(this.GetPoints(SplineComputer.Space.World), SplineComputer.Space.World);
				}
			}
		}

		// Token: 0x1700046B RID: 1131
		// (get) Token: 0x06003FB5 RID: 16309 RVA: 0x001E64BC File Offset: 0x001E46BC
		// (set) Token: 0x06003FB6 RID: 16310 RVA: 0x001E64C9 File Offset: 0x001E46C9
		public int sampleRate
		{
			get
			{
				return this.spline.sampleRate;
			}
			set
			{
				if (value != this.spline.sampleRate)
				{
					if (value < 2)
					{
						value = 2;
					}
					this.spline.sampleRate = value;
					this.Rebuild(true);
				}
			}
		}

		// Token: 0x1700046C RID: 1132
		// (get) Token: 0x06003FB7 RID: 16311 RVA: 0x001E64F3 File Offset: 0x001E46F3
		// (set) Token: 0x06003FB8 RID: 16312 RVA: 0x001E64FB File Offset: 0x001E46FB
		public float optimizeAngleThreshold
		{
			get
			{
				return this._optimizeAngleThreshold;
			}
			set
			{
				if (value != this._optimizeAngleThreshold)
				{
					if (value < 0.001f)
					{
						value = 0.001f;
					}
					this._optimizeAngleThreshold = value;
					if (this._sampleMode == SplineComputer.SampleMode.Optimized)
					{
						this.Rebuild(true);
					}
				}
			}
		}

		// Token: 0x1700046D RID: 1133
		// (get) Token: 0x06003FB9 RID: 16313 RVA: 0x001E652C File Offset: 0x001E472C
		// (set) Token: 0x06003FBA RID: 16314 RVA: 0x001E6534 File Offset: 0x001E4734
		public SplineComputer.SampleMode sampleMode
		{
			get
			{
				return this._sampleMode;
			}
			set
			{
				if (value != this._sampleMode)
				{
					this._sampleMode = value;
					this.Rebuild(true);
				}
			}
		}

		// Token: 0x1700046E RID: 1134
		// (get) Token: 0x06003FBB RID: 16315 RVA: 0x001E654D File Offset: 0x001E474D
		// (set) Token: 0x06003FBC RID: 16316 RVA: 0x001E655A File Offset: 0x001E475A
		public AnimationCurve customValueInterpolation
		{
			get
			{
				return this.spline.customValueInterpolation;
			}
			set
			{
				this.spline.customValueInterpolation = value;
				this.Rebuild(false);
			}
		}

		// Token: 0x1700046F RID: 1135
		// (get) Token: 0x06003FBD RID: 16317 RVA: 0x001E656F File Offset: 0x001E476F
		// (set) Token: 0x06003FBE RID: 16318 RVA: 0x001E657C File Offset: 0x001E477C
		public AnimationCurve customNormalInterpolation
		{
			get
			{
				return this.spline.customNormalInterpolation;
			}
			set
			{
				this.spline.customNormalInterpolation = value;
				this.Rebuild(false);
			}
		}

		// Token: 0x17000470 RID: 1136
		// (get) Token: 0x06003FBF RID: 16319 RVA: 0x001E6591 File Offset: 0x001E4791
		public int iterations
		{
			get
			{
				return this.spline.iterations;
			}
		}

		// Token: 0x17000471 RID: 1137
		// (get) Token: 0x06003FC0 RID: 16320 RVA: 0x001E659E File Offset: 0x001E479E
		public double moveStep
		{
			get
			{
				return this.spline.moveStep;
			}
		}

		// Token: 0x17000472 RID: 1138
		// (get) Token: 0x06003FC1 RID: 16321 RVA: 0x001E65AB File Offset: 0x001E47AB
		public bool isClosed
		{
			get
			{
				return this.spline.isClosed;
			}
		}

		// Token: 0x17000473 RID: 1139
		// (get) Token: 0x06003FC2 RID: 16322 RVA: 0x001E65B8 File Offset: 0x001E47B8
		public int pointCount
		{
			get
			{
				return this.spline.points.Length;
			}
		}

		// Token: 0x17000474 RID: 1140
		// (get) Token: 0x06003FC3 RID: 16323 RVA: 0x001E65C7 File Offset: 0x001E47C7
		public SplineSample[] samples
		{
			get
			{
				return this.sampleCollection.samples;
			}
		}

		// Token: 0x17000475 RID: 1141
		// (get) Token: 0x06003FC4 RID: 16324 RVA: 0x001E65D4 File Offset: 0x001E47D4
		public int sampleCount
		{
			get
			{
				return this._sampleCount;
			}
		}

		// Token: 0x17000476 RID: 1142
		// (get) Token: 0x06003FC5 RID: 16325 RVA: 0x001E65DC File Offset: 0x001E47DC
		public SplineSample[] rawSamples
		{
			get
			{
				return this._rawSamples;
			}
		}

		// Token: 0x17000477 RID: 1143
		// (get) Token: 0x06003FC6 RID: 16326 RVA: 0x001E65E4 File Offset: 0x001E47E4
		public Vector3 position
		{
			get
			{
				return this.lastPosition;
			}
		}

		// Token: 0x17000478 RID: 1144
		// (get) Token: 0x06003FC7 RID: 16327 RVA: 0x001E65EC File Offset: 0x001E47EC
		public Quaternion rotation
		{
			get
			{
				return this.lastRotation;
			}
		}

		// Token: 0x17000479 RID: 1145
		// (get) Token: 0x06003FC8 RID: 16328 RVA: 0x001E65F4 File Offset: 0x001E47F4
		public Vector3 scale
		{
			get
			{
				return this.lastScale;
			}
		}

		// Token: 0x1700047A RID: 1146
		// (get) Token: 0x06003FC9 RID: 16329 RVA: 0x001E65FC File Offset: 0x001E47FC
		public int subscriberCount
		{
			get
			{
				return this.subscribers.Length;
			}
		}

		// Token: 0x1700047B RID: 1147
		// (get) Token: 0x06003FCA RID: 16330 RVA: 0x001E6606 File Offset: 0x001E4806
		public Transform trs
		{
			get
			{
				if (!this._trsCheck)
				{
					this._trs = base.transform;
				}
				return this._trs;
			}
		}

		// Token: 0x14000042 RID: 66
		// (add) Token: 0x06003FCB RID: 16331 RVA: 0x001E6624 File Offset: 0x001E4824
		// (remove) Token: 0x06003FCC RID: 16332 RVA: 0x001E665C File Offset: 0x001E485C
		public event EmptySplineHandler onRebuild;

		// Token: 0x1700047C RID: 1148
		// (get) Token: 0x06003FCD RID: 16333 RVA: 0x001E6691 File Offset: 0x001E4891
		private bool useMultithreading
		{
			get
			{
				return this.multithreaded;
			}
		}

		// Token: 0x06003FCE RID: 16334 RVA: 0x001E6699 File Offset: 0x001E4899
		private void Awake()
		{
			if (this.rebuildOnAwake)
			{
				this.RebuildImmediate(true, true);
			}
			this.ResampleTransform();
		}

		// Token: 0x06003FCF RID: 16335 RVA: 0x001E66B1 File Offset: 0x001E48B1
		private void FixedUpdate()
		{
			if (this.updateMode == SplineComputer.UpdateMode.FixedUpdate || this.updateMode == SplineComputer.UpdateMode.AllUpdate)
			{
				this.RunUpdate();
			}
		}

		// Token: 0x06003FD0 RID: 16336 RVA: 0x001E66CB File Offset: 0x001E48CB
		private void LateUpdate()
		{
			if (this.updateMode == SplineComputer.UpdateMode.LateUpdate || this.updateMode == SplineComputer.UpdateMode.AllUpdate)
			{
				this.RunUpdate();
			}
		}

		// Token: 0x06003FD1 RID: 16337 RVA: 0x001E66E5 File Offset: 0x001E48E5
		private void Update()
		{
			if (this.updateMode == SplineComputer.UpdateMode.Update || this.updateMode == SplineComputer.UpdateMode.AllUpdate)
			{
				this.RunUpdate();
			}
		}

		// Token: 0x06003FD2 RID: 16338 RVA: 0x001E6700 File Offset: 0x001E4900
		private void RunUpdate()
		{
			bool flag = this.TransformHasChanged();
			if (flag)
			{
				this.ResampleTransform();
				if (this.nodes.Length != 0)
				{
					this.UpdateConnectedNodes();
				}
			}
			if (this.useMultithreading && this.queueRebuild)
			{
				this.RebuildUsers();
			}
			if (this.queueResample)
			{
				if (this.useMultithreading)
				{
					if (!flag)
					{
						SplineThreading.Run(new SplineThreading.EmptyHandler(this.CalculateAndTransformSamples));
					}
					else
					{
						SplineThreading.Run(new SplineThreading.EmptyHandler(this.CalculateSamples));
					}
				}
				else
				{
					this.CalculateSamples();
					if (!flag)
					{
						this.TransformSamples(false);
					}
				}
			}
			if (flag)
			{
				this.SetPointsDirty();
				if (this.useMultithreading)
				{
					SplineThreading.Run(new SplineThreading.EmptyHandler(this.TransformSamplesThreaded));
				}
				else
				{
					this.TransformSamples(true);
				}
			}
			if (!this.useMultithreading && this.queueRebuild)
			{
				this.RebuildUsers();
			}
		}

		// Token: 0x06003FD3 RID: 16339 RVA: 0x001E67CC File Offset: 0x001E49CC
		private void TransformSamplesThreaded()
		{
			this.TransformSamples(true);
		}

		// Token: 0x06003FD4 RID: 16340 RVA: 0x001E67D5 File Offset: 0x001E49D5
		private void CalculateAndTransformSamples()
		{
			this.CalculateSamples();
			this.TransformSamples(false);
		}

		// Token: 0x06003FD5 RID: 16341 RVA: 0x001E67E4 File Offset: 0x001E49E4
		private bool TransformHasChanged()
		{
			return this.lastPosition != this.trs.position || this.lastRotation != this.trs.rotation || this.lastScale != this.trs.lossyScale;
		}

		// Token: 0x06003FD6 RID: 16342 RVA: 0x001E6839 File Offset: 0x001E4A39
		private void OnEnable()
		{
			if (this.rebuildPending)
			{
				this.rebuildPending = false;
				this.Rebuild(false);
			}
		}

		// Token: 0x06003FD7 RID: 16343 RVA: 0x001E6851 File Offset: 0x001E4A51
		public void GetSamples(SampleCollection collection)
		{
			collection.samples = this.sampleCollection.samples;
			collection.optimizedIndices = this.sampleCollection.optimizedIndices;
			collection.sampleMode = this._sampleMode;
		}

		// Token: 0x06003FD8 RID: 16344 RVA: 0x001E6884 File Offset: 0x001E4A84
		public void ResampleTransform()
		{
			this.transformMatrix.SetTRS(this.trs.position, this.trs.rotation, this.trs.lossyScale);
			this.inverseTransformMatrix = this.transformMatrix.inverse;
			this.lastPosition = this.trs.position;
			this.lastRotation = this.trs.rotation;
			this.lastScale = this.trs.lossyScale;
			this.uniformScale = (this.lastScale.x == this.lastScale.y && this.lastScale.y == this.lastScale.z);
		}

		// Token: 0x06003FD9 RID: 16345 RVA: 0x001E693C File Offset: 0x001E4B3C
		public void Subscribe(SplineUser input)
		{
			for (int i = 0; i < this.subscribers.Length; i++)
			{
				if (this.subscribers[i] == input)
				{
					return;
				}
			}
			ArrayUtility.Add<SplineUser>(ref this.subscribers, input);
		}

		// Token: 0x06003FDA RID: 16346 RVA: 0x001E697C File Offset: 0x001E4B7C
		public void Unsubscribe(SplineUser input)
		{
			for (int i = 0; i < this.subscribers.Length; i++)
			{
				if (this.subscribers[i] == input)
				{
					ArrayUtility.RemoveAt<SplineUser>(ref this.subscribers, i);
					return;
				}
			}
		}

		// Token: 0x06003FDB RID: 16347 RVA: 0x001E69BC File Offset: 0x001E4BBC
		public bool IsSubscribed(SplineUser user)
		{
			for (int i = 0; i < this.subscribers.Length; i++)
			{
				if (this.subscribers[i] == user)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06003FDC RID: 16348 RVA: 0x001E69F0 File Offset: 0x001E4BF0
		public SplineUser[] GetSubscribers()
		{
			SplineUser[] array = new SplineUser[this.subscribers.Length];
			this.subscribers.CopyTo(array, 0);
			return array;
		}

		// Token: 0x06003FDD RID: 16349 RVA: 0x001E6A1C File Offset: 0x001E4C1C
		public SplinePoint[] GetPoints(SplineComputer.Space getSpace = SplineComputer.Space.World)
		{
			SplinePoint[] array = new SplinePoint[this.spline.points.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = this.spline.points[i];
				if (this._space == SplineComputer.Space.Local && getSpace == SplineComputer.Space.World)
				{
					array[i].position = this.TransformPoint(array[i].position);
					array[i].tangent = this.TransformPoint(array[i].tangent);
					array[i].tangent2 = this.TransformPoint(array[i].tangent2);
					array[i].normal = this.TransformDirection(array[i].normal);
				}
			}
			return array;
		}

		// Token: 0x06003FDE RID: 16350 RVA: 0x001E6AF0 File Offset: 0x001E4CF0
		public SplinePoint GetPoint(int index, SplineComputer.Space getSpace = SplineComputer.Space.World)
		{
			if (index < 0 || index >= this.spline.points.Length)
			{
				return default(SplinePoint);
			}
			if (this._space == SplineComputer.Space.Local && getSpace == SplineComputer.Space.World)
			{
				SplinePoint splinePoint = this.spline.points[index];
				splinePoint.position = this.TransformPoint(splinePoint.position);
				splinePoint.tangent = this.TransformPoint(splinePoint.tangent);
				splinePoint.tangent2 = this.TransformPoint(splinePoint.tangent2);
				splinePoint.normal = this.TransformDirection(splinePoint.normal);
				return splinePoint;
			}
			return this.spline.points[index];
		}

		// Token: 0x06003FDF RID: 16351 RVA: 0x001E6B98 File Offset: 0x001E4D98
		public Vector3 GetPointPosition(int index, SplineComputer.Space getSpace = SplineComputer.Space.World)
		{
			if (this._space == SplineComputer.Space.Local && getSpace == SplineComputer.Space.World)
			{
				return this.TransformPoint(this.spline.points[index].position);
			}
			return this.spline.points[index].position;
		}

		// Token: 0x06003FE0 RID: 16352 RVA: 0x001E6BE4 File Offset: 0x001E4DE4
		public Vector3 GetPointNormal(int index, SplineComputer.Space getSpace = SplineComputer.Space.World)
		{
			if (this._space == SplineComputer.Space.Local && getSpace == SplineComputer.Space.World)
			{
				return this.TransformDirection(this.spline.points[index].normal).normalized;
			}
			return this.spline.points[index].normal;
		}

		// Token: 0x06003FE1 RID: 16353 RVA: 0x001E6C38 File Offset: 0x001E4E38
		public Vector3 GetPointTangent(int index, SplineComputer.Space getSpace = SplineComputer.Space.World)
		{
			if (this._space == SplineComputer.Space.Local && getSpace == SplineComputer.Space.World)
			{
				return this.TransformPoint(this.spline.points[index].tangent);
			}
			return this.spline.points[index].tangent;
		}

		// Token: 0x06003FE2 RID: 16354 RVA: 0x001E6C84 File Offset: 0x001E4E84
		public Vector3 GetPointTangent2(int index, SplineComputer.Space getSpace = SplineComputer.Space.World)
		{
			if (this._space == SplineComputer.Space.Local && getSpace == SplineComputer.Space.World)
			{
				return this.TransformPoint(this.spline.points[index].tangent2);
			}
			return this.spline.points[index].tangent2;
		}

		// Token: 0x06003FE3 RID: 16355 RVA: 0x001E6CD0 File Offset: 0x001E4ED0
		public float GetPointSize(int index, SplineComputer.Space getSpace = SplineComputer.Space.World)
		{
			return this.spline.points[index].size;
		}

		// Token: 0x06003FE4 RID: 16356 RVA: 0x001E6CE8 File Offset: 0x001E4EE8
		public Color GetPointColor(int index, SplineComputer.Space getSpace = SplineComputer.Space.World)
		{
			return this.spline.points[index].color;
		}

		// Token: 0x06003FE5 RID: 16357 RVA: 0x001E6D00 File Offset: 0x001E4F00
		private void Make2D(ref SplinePoint point)
		{
			point.normal = Vector3.back;
			point.position.z = 0f;
			point.tangent.z = 0f;
			point.tangent2.z = 0f;
		}

		// Token: 0x06003FE6 RID: 16358 RVA: 0x001E6D40 File Offset: 0x001E4F40
		public void SetPoints(SplinePoint[] points, SplineComputer.Space setSpace = SplineComputer.Space.World)
		{
			bool flag = false;
			if (points.Length != this.spline.points.Length)
			{
				flag = true;
				if (points.Length < 4)
				{
					this.Break();
				}
				this.spline.points = new SplinePoint[points.Length];
				this.SetPointsDirty();
			}
			for (int i = 0; i < points.Length; i++)
			{
				SplinePoint splinePoint = points[i];
				if (this._space == SplineComputer.Space.Local && setSpace == SplineComputer.Space.World)
				{
					splinePoint.position = this.InverseTransformPoint(points[i].position);
					splinePoint.tangent = this.InverseTransformPoint(points[i].tangent);
					splinePoint.tangent2 = this.InverseTransformPoint(points[i].tangent2);
					splinePoint.normal = this.InverseTransformDirection(points[i].normal);
				}
				if (this._is2D)
				{
					this.Make2D(ref splinePoint);
				}
				if (SplinePoint.AreDifferent(ref splinePoint, ref this.spline.points[i]))
				{
					this.SetDirty(i);
					flag = true;
				}
				this.spline.points[i] = splinePoint;
			}
			if (this.isClosed)
			{
				this.spline.points[this.spline.points.Length - 1] = this.spline.points[0];
			}
			if (flag)
			{
				this.Rebuild(false);
				this.UpdateConnectedNodes(points);
			}
		}

		// Token: 0x06003FE7 RID: 16359 RVA: 0x001E6EA4 File Offset: 0x001E50A4
		public void SetPointPosition(int index, Vector3 pos, SplineComputer.Space setSpace = SplineComputer.Space.World)
		{
			if (index < 0)
			{
				return;
			}
			if (index >= this.spline.points.Length)
			{
				this.AppendPoints(index + 1 - this.spline.points.Length);
			}
			Vector3 vector = pos;
			if (this._space == SplineComputer.Space.Local && setSpace == SplineComputer.Space.World)
			{
				vector = this.InverseTransformPoint(pos);
			}
			if (vector != this.spline.points[index].position)
			{
				this.SetDirty(index);
				this.spline.points[index].position = vector;
				this.Rebuild(false);
				this.SetNodeForPoint(index, this.GetPoint(index, SplineComputer.Space.World));
			}
		}

		// Token: 0x06003FE8 RID: 16360 RVA: 0x001E6F48 File Offset: 0x001E5148
		public void SetPointTangents(int index, Vector3 tan1, Vector3 tan2, SplineComputer.Space setSpace = SplineComputer.Space.World)
		{
			if (index < 0)
			{
				return;
			}
			if (index >= this.spline.points.Length)
			{
				this.AppendPoints(index + 1 - this.spline.points.Length);
			}
			Vector3 vector = tan1;
			Vector3 vector2 = tan2;
			if (this._space == SplineComputer.Space.Local && setSpace == SplineComputer.Space.World)
			{
				vector = this.InverseTransformPoint(tan1);
				vector2 = this.InverseTransformPoint(tan2);
			}
			bool flag = false;
			if (vector2 != this.spline.points[index].tangent2)
			{
				flag = true;
				this.spline.points[index].SetTangent2Position(vector2);
			}
			if (vector != this.spline.points[index].tangent)
			{
				flag = true;
				this.spline.points[index].SetTangentPosition(vector);
			}
			if (this._is2D)
			{
				this.Make2D(ref this.spline.points[index]);
			}
			if (flag)
			{
				this.SetDirty(index);
				this.Rebuild(false);
				this.SetNodeForPoint(index, this.GetPoint(index, SplineComputer.Space.World));
			}
		}

		// Token: 0x06003FE9 RID: 16361 RVA: 0x001E7054 File Offset: 0x001E5254
		public void SetPointNormal(int index, Vector3 nrm, SplineComputer.Space setSpace = SplineComputer.Space.World)
		{
			if (index < 0)
			{
				return;
			}
			if (index >= this.spline.points.Length)
			{
				this.AppendPoints(index + 1 - this.spline.points.Length);
			}
			Vector3 vector = nrm;
			if (this._space == SplineComputer.Space.Local && setSpace == SplineComputer.Space.World)
			{
				vector = this.InverseTransformDirection(nrm);
			}
			if (vector != this.spline.points[index].normal)
			{
				this.SetDirty(index);
				this.spline.points[index].normal = vector;
				if (this._is2D)
				{
					this.Make2D(ref this.spline.points[index]);
				}
				this.Rebuild(false);
				this.SetNodeForPoint(index, this.GetPoint(index, SplineComputer.Space.World));
			}
		}

		// Token: 0x06003FEA RID: 16362 RVA: 0x001E7114 File Offset: 0x001E5314
		public void SetPointSize(int index, float size)
		{
			if (index < 0)
			{
				return;
			}
			if (index >= this.spline.points.Length)
			{
				this.AppendPoints(index + 1 - this.spline.points.Length);
			}
			if (size != this.spline.points[index].size)
			{
				this.SetDirty(index);
				this.spline.points[index].size = size;
				this.Rebuild(false);
				this.SetNodeForPoint(index, this.GetPoint(index, SplineComputer.Space.World));
			}
		}

		// Token: 0x06003FEB RID: 16363 RVA: 0x001E719C File Offset: 0x001E539C
		public void SetPointColor(int index, Color color)
		{
			if (index < 0)
			{
				return;
			}
			if (index >= this.spline.points.Length)
			{
				this.AppendPoints(index + 1 - this.spline.points.Length);
			}
			if (color != this.spline.points[index].color)
			{
				this.SetDirty(index);
				this.spline.points[index].color = color;
				this.Rebuild(false);
				this.SetNodeForPoint(index, this.GetPoint(index, SplineComputer.Space.World));
			}
		}

		// Token: 0x06003FEC RID: 16364 RVA: 0x001E7228 File Offset: 0x001E5428
		public void SetPoint(int index, SplinePoint point, SplineComputer.Space setSpace = SplineComputer.Space.World)
		{
			if (index < 0)
			{
				return;
			}
			if (index >= this.spline.points.Length)
			{
				this.AppendPoints(index + 1 - this.spline.points.Length);
			}
			bool flag = false;
			SplinePoint splinePoint = point;
			if (this._space == SplineComputer.Space.Local && setSpace == SplineComputer.Space.World)
			{
				splinePoint.position = this.InverseTransformPoint(point.position);
				splinePoint.tangent = this.InverseTransformPoint(point.tangent);
				splinePoint.tangent2 = this.InverseTransformPoint(point.tangent2);
				splinePoint.normal = this.InverseTransformDirection(point.normal);
			}
			if (this._is2D)
			{
				this.Make2D(ref splinePoint);
			}
			if (SplinePoint.AreDifferent(ref splinePoint, ref this.spline.points[index]))
			{
				flag = true;
			}
			if (flag)
			{
				this.SetDirty(index);
				this.spline.points[index] = splinePoint;
				this.Rebuild(false);
				this.SetNodeForPoint(index, point);
			}
		}

		// Token: 0x06003FED RID: 16365 RVA: 0x001E7314 File Offset: 0x001E5514
		private void AppendPoints(int count)
		{
			SplinePoint[] array = new SplinePoint[this.spline.points.Length + count];
			this.spline.points.CopyTo(array, 0);
			this.spline.points = array;
			this.Rebuild(true);
		}

		// Token: 0x06003FEE RID: 16366 RVA: 0x001E735C File Offset: 0x001E555C
		public double GetPointPercent(int pointIndex)
		{
			double num = DMath.Clamp01((double)pointIndex / (double)(this.pointCount - 1));
			if (this._sampleMode != SplineComputer.SampleMode.Uniform)
			{
				return num;
			}
			if (this.originalSamplePercents.Length <= 1)
			{
				return 0.0;
			}
			for (int i = this.originalSamplePercents.Length - 2; i >= 0; i--)
			{
				if (this.originalSamplePercents[i] < num)
				{
					double t = DMath.InverseLerp(this.originalSamplePercents[i], this.originalSamplePercents[i + 1], num);
					return DMath.Lerp(this.sampleCollection.samples[i].percent, this.sampleCollection.samples[i + 1].percent, t);
				}
			}
			return 0.0;
		}

		// Token: 0x06003FEF RID: 16367 RVA: 0x001E740C File Offset: 0x001E560C
		public int PercentToPointIndex(double percent, Spline.Direction direction = Spline.Direction.Forward)
		{
			if (this._sampleMode == SplineComputer.SampleMode.Uniform)
			{
				int num;
				double num2;
				this.GetSamplingValues(percent, out num, out num2);
				if (num2 > 0.0)
				{
					num2 = DMath.Lerp(this.originalSamplePercents[num], this.originalSamplePercents[num + 1], num2);
					if (direction == Spline.Direction.Forward)
					{
						return DMath.FloorInt(num2 * (double)(this.pointCount - 1));
					}
					return DMath.CeilInt(num2 * (double)(this.pointCount - 1));
				}
				else
				{
					if (direction == Spline.Direction.Forward)
					{
						return DMath.FloorInt(this.originalSamplePercents[num] * (double)(this.pointCount - 1));
					}
					return DMath.CeilInt(this.originalSamplePercents[num] * (double)(this.pointCount - 1));
				}
			}
			else
			{
				if (direction == Spline.Direction.Forward)
				{
					return DMath.FloorInt(percent * (double)(this.pointCount - 1));
				}
				return DMath.CeilInt(percent * (double)(this.pointCount - 1));
			}
		}

		// Token: 0x06003FF0 RID: 16368 RVA: 0x001E74D4 File Offset: 0x001E56D4
		public Vector3 EvaluatePosition(double percent)
		{
			return this.EvaluatePosition(percent, SplineComputer.EvaluateMode.Cached);
		}

		// Token: 0x06003FF1 RID: 16369 RVA: 0x001E74DE File Offset: 0x001E56DE
		public Vector3 EvaluatePosition(double percent, SplineComputer.EvaluateMode mode = SplineComputer.EvaluateMode.Cached)
		{
			if (mode == SplineComputer.EvaluateMode.Calculate)
			{
				return this.TransformPoint(this.spline.EvaluatePosition(percent));
			}
			return this.sampleCollection.EvaluatePosition(percent);
		}

		// Token: 0x06003FF2 RID: 16370 RVA: 0x001E7503 File Offset: 0x001E5703
		public Vector3 EvaluatePosition(int pointIndex, SplineComputer.EvaluateMode mode = SplineComputer.EvaluateMode.Cached)
		{
			return this.EvaluatePosition(this.GetPointPercent(pointIndex), mode);
		}

		// Token: 0x06003FF3 RID: 16371 RVA: 0x001E7513 File Offset: 0x001E5713
		public SplineSample Evaluate(double percent)
		{
			return this.Evaluate(percent, SplineComputer.EvaluateMode.Cached);
		}

		// Token: 0x06003FF4 RID: 16372 RVA: 0x001E7520 File Offset: 0x001E5720
		public SplineSample Evaluate(double percent, SplineComputer.EvaluateMode mode = SplineComputer.EvaluateMode.Cached)
		{
			SplineSample result = new SplineSample();
			this.Evaluate(percent, result, mode);
			return result;
		}

		// Token: 0x06003FF5 RID: 16373 RVA: 0x001E7540 File Offset: 0x001E5740
		public SplineSample Evaluate(int pointIndex)
		{
			SplineSample result = new SplineSample();
			this.Evaluate(pointIndex, result);
			return result;
		}

		// Token: 0x06003FF6 RID: 16374 RVA: 0x001E755C File Offset: 0x001E575C
		public void Evaluate(int pointIndex, SplineSample result)
		{
			this.Evaluate(this.GetPointPercent(pointIndex), result);
		}

		// Token: 0x06003FF7 RID: 16375 RVA: 0x001E756C File Offset: 0x001E576C
		public void Evaluate(double percent, SplineSample result)
		{
			this.Evaluate(percent, result, SplineComputer.EvaluateMode.Cached);
		}

		// Token: 0x06003FF8 RID: 16376 RVA: 0x001E7577 File Offset: 0x001E5777
		public void Evaluate(double percent, SplineSample result, SplineComputer.EvaluateMode mode = SplineComputer.EvaluateMode.Cached)
		{
			if (mode == SplineComputer.EvaluateMode.Calculate)
			{
				this.spline.Evaluate(result, percent);
				this.TransformResult(result);
				return;
			}
			this.sampleCollection.Evaluate(percent, result);
		}

		// Token: 0x06003FF9 RID: 16377 RVA: 0x001E759F File Offset: 0x001E579F
		public void Evaluate(ref SplineSample[] results, double from = 0.0, double to = 1.0)
		{
			this.sampleCollection.Evaluate(ref results, from, to);
		}

		// Token: 0x06003FFA RID: 16378 RVA: 0x001E75AF File Offset: 0x001E57AF
		public void EvaluatePositions(ref Vector3[] positions, double from = 0.0, double to = 1.0)
		{
			this.sampleCollection.EvaluatePositions(ref positions, from, to);
		}

		// Token: 0x06003FFB RID: 16379 RVA: 0x001E75BF File Offset: 0x001E57BF
		public double Travel(double start, float distance, out float moved, Spline.Direction direction = Spline.Direction.Forward)
		{
			return this.sampleCollection.Travel(start, distance, direction, out moved);
		}

		// Token: 0x06003FFC RID: 16380 RVA: 0x001E75D4 File Offset: 0x001E57D4
		public double Travel(double start, float distance, Spline.Direction direction = Spline.Direction.Forward)
		{
			float num;
			return this.Travel(start, distance, out num, direction);
		}

		// Token: 0x06003FFD RID: 16381 RVA: 0x001E75EC File Offset: 0x001E57EC
		public void Project(SplineSample result, Vector3 position, double from = 0.0, double to = 1.0, SplineComputer.EvaluateMode mode = SplineComputer.EvaluateMode.Cached, int subdivisions = 4)
		{
			if (mode == SplineComputer.EvaluateMode.Calculate)
			{
				position = this.InverseTransformPoint(position);
				double percent = this.spline.Project(position, subdivisions, from, to);
				this.spline.Evaluate(result, percent);
				this.TransformResult(result);
				return;
			}
			this.sampleCollection.Project(position, this.pointCount, result, from, to);
		}

		// Token: 0x06003FFE RID: 16382 RVA: 0x001E7644 File Offset: 0x001E5844
		public SplineSample Project(Vector3 point, double from = 0.0, double to = 1.0)
		{
			SplineSample result = new SplineSample();
			this.Project(result, point, from, to, SplineComputer.EvaluateMode.Cached, 4);
			return result;
		}

		// Token: 0x06003FFF RID: 16383 RVA: 0x001E7664 File Offset: 0x001E5864
		public float CalculateLength(double from = 0.0, double to = 1.0)
		{
			if (!this.hasSamples)
			{
				return 0f;
			}
			return this.sampleCollection.CalculateLength(from, to);
		}

		// Token: 0x06004000 RID: 16384 RVA: 0x001E7684 File Offset: 0x001E5884
		private void TransformResult(SplineSample result)
		{
			result.position = this.TransformPoint(result.position);
			result.forward = this.TransformDirection(result.forward);
			result.up = this.TransformDirection(result.up);
			if (!this.uniformScale)
			{
				result.forward.Normalize();
				result.up.Normalize();
			}
		}

		// Token: 0x06004001 RID: 16385 RVA: 0x001E76E5 File Offset: 0x001E58E5
		public void Rebuild(bool forceUpdateAll = false)
		{
			if (forceUpdateAll)
			{
				this.SetPointsDirty();
			}
			this.queueResample = true;
			if (this.updateMode == SplineComputer.UpdateMode.None)
			{
				this.queueResample = false;
			}
		}

		// Token: 0x06004002 RID: 16386 RVA: 0x001E7707 File Offset: 0x001E5907
		public void RebuildImmediate(bool calculateSamples = true, bool forceUpdateAll = false)
		{
			if (calculateSamples)
			{
				this.queueResample = true;
				if (forceUpdateAll)
				{
					this.SetPointsDirty();
				}
			}
			else
			{
				this.queueResample = false;
			}
			this.RunUpdate();
		}

		// Token: 0x06004003 RID: 16387 RVA: 0x001E772C File Offset: 0x001E592C
		private void RebuildUsers()
		{
			for (int i = this.subscribers.Length - 1; i >= 0; i--)
			{
				if (this.subscribers[i] != null)
				{
					if (this.subscribers[i].spline != this)
					{
						ArrayUtility.RemoveAt<SplineUser>(ref this.subscribers, i);
					}
					else
					{
						this.subscribers[i].Rebuild();
					}
				}
				else
				{
					ArrayUtility.RemoveAt<SplineUser>(ref this.subscribers, i);
				}
			}
			if (this.onRebuild != null)
			{
				this.onRebuild();
			}
			this.queueRebuild = false;
		}

		// Token: 0x06004004 RID: 16388 RVA: 0x001E77B8 File Offset: 0x001E59B8
		private void UnsetPointsDirty()
		{
			if (this.pointsDirty.Length != this.spline.points.Length)
			{
				this.pointsDirty = new bool[this.spline.points.Length];
			}
			for (int i = 0; i < this.pointsDirty.Length; i++)
			{
				this.pointsDirty[i] = false;
			}
		}

		// Token: 0x06004005 RID: 16389 RVA: 0x001E7810 File Offset: 0x001E5A10
		private void SetPointsDirty()
		{
			if (this.pointsDirty.Length != this.spline.points.Length)
			{
				this.pointsDirty = new bool[this.spline.points.Length];
			}
			for (int i = 0; i < this.pointsDirty.Length; i++)
			{
				this.pointsDirty[i] = true;
			}
		}

		// Token: 0x06004006 RID: 16390 RVA: 0x001E7868 File Offset: 0x001E5A68
		private void SetDirty(int index)
		{
			if (this.sampleMode == SplineComputer.SampleMode.Uniform)
			{
				this.SetPointsDirty();
				return;
			}
			this.pointsDirty[index] = true;
			if (index == 0 && this.isClosed)
			{
				this.pointsDirty[this.pointsDirty.Length - 1] = true;
			}
		}

		// Token: 0x06004007 RID: 16391 RVA: 0x001E78A0 File Offset: 0x001E5AA0
		private void CalculateSamples()
		{
			this.queueResample = false;
			if (this.pointCount == 0)
			{
				if (this._rawSamples.Length != 0)
				{
					this._rawSamples = new SplineSample[0];
					this.sampleCollection.samples = new SplineSample[0];
				}
				return;
			}
			if (this.pointCount == 1)
			{
				if (this._rawSamples.Length != 1)
				{
					this._rawSamples = new SplineSample[1];
					this._rawSamples[0] = new SplineSample();
					this.sampleCollection.samples = new SplineSample[1];
					this.sampleCollection.samples[0] = new SplineSample();
				}
				this.Evaluate(0.0, this._rawSamples[0]);
				return;
			}
			if (this._sampleMode == SplineComputer.SampleMode.Uniform)
			{
				this.spline.EvaluateUniform(ref this._rawSamples, ref this.originalSamplePercents, 0.0, 1.0);
			}
			else
			{
				if (this.originalSamplePercents.Length != 0)
				{
					this.originalSamplePercents = new double[0];
				}
				if (this._rawSamples.Length != this.spline.iterations)
				{
					this._rawSamples = new SplineSample[this.spline.iterations];
					for (int i = 0; i < this._rawSamples.Length; i++)
					{
						this._rawSamples[i] = new SplineSample();
					}
				}
				bool flag = true;
				if (this.type == Spline.Type.Bezier || this.type == Spline.Type.Linear)
				{
					flag = false;
				}
				for (int j = 0; j < this._rawSamples.Length; j++)
				{
					double num = (double)j / (double)(this._rawSamples.Length - 1);
					if (flag ? this.IsDirtyHermite(num) : this.IsDirtyBezier(num))
					{
						this.spline.Evaluate(this._rawSamples[j], num);
					}
				}
			}
			if (this.isClosed)
			{
				this._rawSamples[this._rawSamples.Length - 1].CopyFrom(this._rawSamples[0]);
				this._rawSamples[this._rawSamples.Length - 1].percent = 1.0;
			}
		}

		// Token: 0x06004008 RID: 16392 RVA: 0x001E7A88 File Offset: 0x001E5C88
		private void TransformSamples(bool forceTransformAll = false)
		{
			if (this._transformedSamples.Length != this._rawSamples.Length)
			{
				this._transformedSamples = new SplineSample[this._rawSamples.Length];
				for (int i = 0; i < this._transformedSamples.Length; i++)
				{
					this._transformedSamples[i] = new SplineSample(this._rawSamples[i]);
				}
			}
			bool flag = true;
			if (this.type == Spline.Type.Bezier || this.type == Spline.Type.Linear)
			{
				flag = false;
			}
			if (this.space == SplineComputer.Space.Local)
			{
				for (int j = 0; j < this._rawSamples.Length; j++)
				{
					if (!((!forceTransformAll && flag) ? (!this.IsDirtyHermite(this._rawSamples[j].percent)) : (!this.IsDirtyBezier(this._rawSamples[j].percent))))
					{
						this._transformedSamples[j].CopyFrom(this._rawSamples[j]);
						this.TransformResult(this._transformedSamples[j]);
					}
				}
			}
			else
			{
				this._transformedSamples = this._rawSamples;
			}
			if (this._sampleMode == SplineComputer.SampleMode.Optimized)
			{
				this.OptimizeSamples();
			}
			else
			{
				this.sampleCollection.samples = this._transformedSamples;
				if (this.sampleFlter.Length != 0)
				{
					this.sampleFlter = new bool[0];
				}
				this._sampleCount = this.sampleCollection.Count;
			}
			if (this._sampleMode == SplineComputer.SampleMode.Optimized)
			{
				if (this.sampleCollection.optimizedIndices.Length != this._rawSamples.Length)
				{
					this.sampleCollection.optimizedIndices = new int[this._rawSamples.Length];
				}
				this.sampleCollection.optimizedIndices[0] = 0;
				this.sampleCollection.optimizedIndices[this.sampleCollection.optimizedIndices.Length - 1] = this.sampleCollection.Count - 1;
				for (int k = 1; k < this._rawSamples.Length - 1; k++)
				{
					this.sampleCollection.optimizedIndices[k] = 0;
					double num = (double)k / (double)(this._rawSamples.Length - 1);
					int num2 = 0;
					while (num2 < this.sampleCollection.Count && this.sampleCollection.samples[num2].percent <= num)
					{
						this.sampleCollection.optimizedIndices[k] = num2;
						num2++;
					}
				}
				if (this.sampleCollection.optimizedIndices.Length > 1)
				{
					this.sampleCollection.optimizedIndices[this.sampleCollection.optimizedIndices.Length - 1] = this.sampleCollection.Count - 1;
				}
			}
			else if (this.sampleCollection.Count > 0)
			{
				this.sampleCollection.optimizedIndices = new int[0];
			}
			this.sampleCollection.sampleMode = this._sampleMode;
			this.queueRebuild = true;
			this.hasSamples = (this._sampleCount > 0);
			this.UnsetPointsDirty();
		}

		// Token: 0x06004009 RID: 16393 RVA: 0x001E7D30 File Offset: 0x001E5F30
		private void OptimizeSamples()
		{
			if (this._transformedSamples.Length <= 1)
			{
				return;
			}
			if (this.sampleFlter.Length != this._rawSamples.Length)
			{
				this.sampleFlter = new bool[this._rawSamples.Length];
			}
			this._sampleCount = 2;
			Vector3 forward = this._transformedSamples[0].forward;
			this.sampleFlter[0] = true;
			this.sampleFlter[this.sampleFlter.Length - 1] = true;
			for (int i = 1; i < this._transformedSamples.Length - 1; i++)
			{
				if (Vector3.Angle(forward, this._transformedSamples[i].forward) >= this._optimizeAngleThreshold)
				{
					this.sampleFlter[i] = true;
					this._sampleCount++;
					forward = this._transformedSamples[i].forward;
				}
				else
				{
					this.sampleFlter[i] = false;
				}
			}
			if (this.sampleCollection.Count != this._sampleCount || this.sampleCollection.samples == this._transformedSamples)
			{
				this.sampleCollection.samples = new SplineSample[this._sampleCount];
				for (int j = 0; j < this.sampleCollection.Count; j++)
				{
					this.sampleCollection.samples[j] = new SplineSample();
				}
			}
			int num = 0;
			for (int k = 0; k < this._transformedSamples.Length; k++)
			{
				if (this.sampleFlter[k])
				{
					this.sampleCollection.samples[num].CopyFrom(this._transformedSamples[k]);
					num++;
				}
			}
		}

		// Token: 0x0600400A RID: 16394 RVA: 0x001E7EA8 File Offset: 0x001E60A8
		private bool IsDirtyBezier(double samplePercent)
		{
			float num = (float)samplePercent * (float)(this.pointCount - 1);
			int num2 = Mathf.FloorToInt(num);
			if (this.pointsDirty[num2])
			{
				return true;
			}
			int num3 = num2 + 1;
			if (num3 > this.pointCount - 1)
			{
				if (this.isClosed)
				{
					num3 = 0;
				}
				else
				{
					num3 = this.pointCount - 1;
				}
			}
			if (this.pointsDirty[num3])
			{
				return true;
			}
			int num4 = num2 - 1;
			if (num4 < 0)
			{
				if (this.isClosed)
				{
					num4 = this.pointCount - 1;
				}
				else
				{
					num4 = 0;
				}
			}
			return this.pointsDirty[num4] && Mathf.Approximately(num, (float)num2);
		}

		// Token: 0x0600400B RID: 16395 RVA: 0x001E7F3C File Offset: 0x001E613C
		private bool IsDirtyHermite(double samplePercent)
		{
			float num = (float)samplePercent * (float)(this.pointCount - 1);
			int num2 = Mathf.FloorToInt(num);
			if (this.pointsDirty[num2])
			{
				return true;
			}
			int num3 = num2 + 1;
			if (num3 > this.pointCount - 1)
			{
				if (this.isClosed)
				{
					num3 = 0;
				}
				else
				{
					num3 = this.pointCount - 1;
				}
			}
			int num4 = num3 + 1;
			if (num4 > this.pointCount - 1)
			{
				if (this.isClosed)
				{
					num4 = 1;
				}
				else
				{
					num4 = this.pointCount - 1;
				}
			}
			if (this.pointsDirty[num3] || this.pointsDirty[num4])
			{
				return true;
			}
			int num5 = num2 - 1;
			if (num5 < 0)
			{
				if (this.isClosed)
				{
					num5 = this.pointCount - 2;
				}
				else
				{
					num5 = 0;
				}
			}
			int num6 = num5 - 1;
			if (num6 < 0)
			{
				if (this.isClosed)
				{
					num6 = this.pointCount - 2;
				}
				else
				{
					num6 = 0;
				}
			}
			return this.pointsDirty[num5] || (this.pointsDirty[num6] && Mathf.Approximately(num, (float)num2));
		}

		// Token: 0x0600400C RID: 16396 RVA: 0x001E802F File Offset: 0x001E622F
		public void Break()
		{
			this.Break(0);
		}

		// Token: 0x0600400D RID: 16397 RVA: 0x001E8038 File Offset: 0x001E6238
		public void Break(int at)
		{
			if (this.spline.isClosed)
			{
				this.spline.Break(at);
				if (at != 0)
				{
					this.SetPointsDirty();
				}
				else
				{
					this.SetDirty(0);
					this.SetDirty(this.pointCount - 1);
				}
				this.Rebuild(false);
			}
		}

		// Token: 0x0600400E RID: 16398 RVA: 0x001E8085 File Offset: 0x001E6285
		public void Close()
		{
			if (!this.spline.isClosed)
			{
				this.spline.Close();
				this.SetDirty(0);
				this.SetDirty(this.pointCount - 1);
				this.Rebuild(false);
			}
		}

		// Token: 0x0600400F RID: 16399 RVA: 0x001E80BB File Offset: 0x001E62BB
		public void CatToBezierTangents()
		{
			this.spline.CatToBezierTangents();
			this.SetPoints(this.spline.points, SplineComputer.Space.Local);
		}

		// Token: 0x06004010 RID: 16400 RVA: 0x001E80DC File Offset: 0x001E62DC
		public bool Raycast(out RaycastHit hit, out double hitPercent, LayerMask layerMask, double resolution = 1.0, double from = 0.0, double to = 1.0, QueryTriggerInteraction hitTriggers = QueryTriggerInteraction.UseGlobal)
		{
			resolution = DMath.Clamp01(resolution);
			Spline.FormatFromTo(ref from, ref to, false);
			double num = from;
			Vector3 vector = this.EvaluatePosition(num);
			hitPercent = 0.0;
			double a;
			Vector3 vector2;
			for (;;)
			{
				a = num;
				num = DMath.Move(num, to, this.moveStep / resolution);
				vector2 = this.EvaluatePosition(num);
				if (Physics.Linecast(vector, vector2, out hit, layerMask, hitTriggers))
				{
					break;
				}
				vector = vector2;
				if (num == to)
				{
					return false;
				}
			}
			double t = (double)((hit.point - vector).sqrMagnitude / (vector2 - vector).sqrMagnitude);
			hitPercent = DMath.Lerp(a, num, t);
			return true;
		}

		// Token: 0x06004011 RID: 16401 RVA: 0x001E8180 File Offset: 0x001E6380
		public bool RaycastAll(out RaycastHit[] hits, out double[] hitPercents, LayerMask layerMask, double resolution = 1.0, double from = 0.0, double to = 1.0, QueryTriggerInteraction hitTriggers = QueryTriggerInteraction.UseGlobal)
		{
			resolution = DMath.Clamp01(resolution);
			Spline.FormatFromTo(ref from, ref to, false);
			double num = from;
			Vector3 vector = this.EvaluatePosition(num);
			List<RaycastHit> list = new List<RaycastHit>();
			List<double> list2 = new List<double>();
			bool result = false;
			do
			{
				double a = num;
				num = DMath.Move(num, to, this.moveStep / resolution);
				Vector3 vector2 = this.EvaluatePosition(num);
				RaycastHit[] array = Physics.RaycastAll(vector, vector2 - vector, Vector3.Distance(vector, vector2), layerMask, hitTriggers);
				for (int i = 0; i < array.Length; i++)
				{
					result = true;
					double t = (double)((array[i].point - vector).sqrMagnitude / (vector2 - vector).sqrMagnitude);
					list2.Add(DMath.Lerp(a, num, t));
					list.Add(array[i]);
				}
				vector = vector2;
			}
			while (num != to);
			hits = list.ToArray();
			hitPercents = list2.ToArray();
			return result;
		}

		// Token: 0x06004012 RID: 16402 RVA: 0x001E827C File Offset: 0x001E647C
		public void CheckTriggers(double start, double end, SplineUser user = null)
		{
			for (int i = 0; i < this.triggerGroups.Length; i++)
			{
				this.triggerGroups[i].Check(start, end, null);
			}
		}

		// Token: 0x06004013 RID: 16403 RVA: 0x001E82AC File Offset: 0x001E64AC
		public void CheckTriggers(int group, double start, double end)
		{
			if (group < 0 || group >= this.triggerGroups.Length)
			{
				Debug.LogError("Trigger group " + group + " does not exist");
				return;
			}
			this.triggerGroups[group].Check(start, end, null);
		}

		// Token: 0x06004014 RID: 16404 RVA: 0x001E82E8 File Offset: 0x001E64E8
		public void ResetTriggers()
		{
			for (int i = 0; i < this.triggerGroups.Length; i++)
			{
				this.triggerGroups[i].Reset();
			}
		}

		// Token: 0x06004015 RID: 16405 RVA: 0x001E8318 File Offset: 0x001E6518
		public void ResetTriggers(int group)
		{
			if (group < 0 || group >= this.triggerGroups.Length)
			{
				Debug.LogError("Trigger group " + group + " does not exist");
				return;
			}
			for (int i = 0; i < this.triggerGroups[group].triggers.Length; i++)
			{
				this.triggerGroups[group].triggers[i].Reset();
			}
		}

		// Token: 0x06004016 RID: 16406 RVA: 0x001E8380 File Offset: 0x001E6580
		public List<Node.Connection> GetJunctions(int pointIndex)
		{
			for (int i = 0; i < this.nodes.Length; i++)
			{
				if (this.nodes[i].pointIndex == pointIndex)
				{
					return this.nodes[i].GetConnections(this);
				}
			}
			return new List<Node.Connection>();
		}

		// Token: 0x06004017 RID: 16407 RVA: 0x001E83C4 File Offset: 0x001E65C4
		public Dictionary<int, List<Node.Connection>> GetJunctions(double start = 0.0, double end = 1.0)
		{
			int num;
			double num2;
			this.sampleCollection.GetSamplingValues(start, out num, out num2);
			Dictionary<int, List<Node.Connection>> dictionary = new Dictionary<int, List<Node.Connection>>();
			float num3 = (float)(this.pointCount - 1) * (float)start;
			float num4 = (float)(this.pointCount - 1) * (float)end;
			for (int i = 0; i < this.nodes.Length; i++)
			{
				bool flag = false;
				if (end > start && (float)this.nodes[i].pointIndex > num3 && (float)this.nodes[i].pointIndex < num4)
				{
					flag = true;
				}
				else if ((float)this.nodes[i].pointIndex < num3 && (float)this.nodes[i].pointIndex > num4)
				{
					flag = true;
				}
				if (!flag && Mathf.Abs(num3 - (float)this.nodes[i].pointIndex) <= 0.0001f)
				{
					flag = true;
				}
				if (!flag && Mathf.Abs(num4 - (float)this.nodes[i].pointIndex) <= 0.0001f)
				{
					flag = true;
				}
				if (flag)
				{
					dictionary.Add(this.nodes[i].pointIndex, this.nodes[i].GetConnections(this));
				}
			}
			return dictionary;
		}

		// Token: 0x06004018 RID: 16408 RVA: 0x001E84EC File Offset: 0x001E66EC
		public void ConnectNode(Node node, int pointIndex)
		{
			if (node == null)
			{
				Debug.LogError("Missing Node");
				return;
			}
			if (pointIndex < 0 || pointIndex >= this.spline.points.Length)
			{
				Debug.Log("Invalid point index " + pointIndex);
				return;
			}
			for (int i = 0; i < this.nodes.Length; i++)
			{
				if (!(this.nodes[i].node == null) && (this.nodes[i].pointIndex == pointIndex || this.nodes[i].node == node))
				{
					Node.Connection[] connections = this.nodes[i].node.GetConnections();
					for (int j = 0; j < connections.Length; j++)
					{
						if (connections[j].spline == this)
						{
							Debug.LogError(string.Concat(new object[]
							{
								"Node ",
								node.name,
								" is already connected to spline ",
								base.name,
								" at point ",
								this.nodes[i].pointIndex
							}));
							return;
						}
					}
					this.AddNodeLink(node, pointIndex);
					return;
				}
			}
			node.AddConnection(this, pointIndex);
			this.AddNodeLink(node, pointIndex);
		}

		// Token: 0x06004019 RID: 16409 RVA: 0x001E862C File Offset: 0x001E682C
		public void DisconnectNode(int pointIndex)
		{
			for (int i = 0; i < this.nodes.Length; i++)
			{
				if (this.nodes[i].pointIndex == pointIndex)
				{
					this.nodes[i].node.RemoveConnection(this, pointIndex);
					ArrayUtility.RemoveAt<SplineComputer.NodeLink>(ref this.nodes, i);
					return;
				}
			}
		}

		// Token: 0x0600401A RID: 16410 RVA: 0x001E8680 File Offset: 0x001E6880
		private void AddNodeLink(Node node, int pointIndex)
		{
			ArrayUtility.Add<SplineComputer.NodeLink>(ref this.nodes, new SplineComputer.NodeLink
			{
				node = node,
				pointIndex = pointIndex
			});
			this.UpdateConnectedNodes();
		}

		// Token: 0x0600401B RID: 16411 RVA: 0x001E86B4 File Offset: 0x001E68B4
		public Dictionary<int, Node> GetNodes(double start = 0.0, double end = 1.0)
		{
			int num;
			double num2;
			this.sampleCollection.GetSamplingValues(start, out num, out num2);
			Dictionary<int, Node> dictionary = new Dictionary<int, Node>();
			float num3 = (float)(this.pointCount - 1) * (float)start;
			float num4 = (float)(this.pointCount - 1) * (float)end;
			for (int i = 0; i < this.nodes.Length; i++)
			{
				bool flag = false;
				if (end > start && (float)this.nodes[i].pointIndex > num3 && (float)this.nodes[i].pointIndex < num4)
				{
					flag = true;
				}
				else if ((float)this.nodes[i].pointIndex < num3 && (float)this.nodes[i].pointIndex > num4)
				{
					flag = true;
				}
				if (!flag && Mathf.Abs(num3 - (float)this.nodes[i].pointIndex) <= 0.0001f)
				{
					flag = true;
				}
				if (!flag && Mathf.Abs(num4 - (float)this.nodes[i].pointIndex) <= 0.0001f)
				{
					flag = true;
				}
				if (flag)
				{
					dictionary.Add(this.nodes[i].pointIndex, this.nodes[i].node);
				}
			}
			return dictionary;
		}

		// Token: 0x0600401C RID: 16412 RVA: 0x001E87D8 File Offset: 0x001E69D8
		public Node GetNode(int pointIndex)
		{
			if (pointIndex < 0 || pointIndex >= this.pointCount)
			{
				return null;
			}
			for (int i = 0; i < this.nodes.Length; i++)
			{
				if (this.nodes[i].pointIndex == pointIndex)
				{
					return this.nodes[i].node;
				}
			}
			return null;
		}

		// Token: 0x0600401D RID: 16413 RVA: 0x001E8828 File Offset: 0x001E6A28
		public void TransferNode(int pointIndex, int newPointIndex)
		{
			if (newPointIndex < 0 || newPointIndex >= this.pointCount)
			{
				Debug.LogError("Invalid new point index " + newPointIndex);
				return;
			}
			if (this.GetNode(newPointIndex) != null)
			{
				Debug.LogError("Cannot move node to point " + newPointIndex + ". Point already connected to a node");
				return;
			}
			Node node = this.GetNode(pointIndex);
			if (node == null)
			{
				Debug.LogError("No node connected to point " + pointIndex);
				return;
			}
			this.DisconnectNode(pointIndex);
			this.ConnectNode(node, newPointIndex);
		}

		// Token: 0x0600401E RID: 16414 RVA: 0x001E88B8 File Offset: 0x001E6AB8
		public void ShiftNodes(int startIndex, int endIndex, int shift)
		{
			if (startIndex < endIndex)
			{
				for (int i = endIndex; i >= startIndex; i--)
				{
					if (this.GetNode(i) != null)
					{
						this.TransferNode(i, i + shift);
					}
				}
				return;
			}
			for (int j = startIndex; j >= endIndex; j--)
			{
				if (this.GetNode(j) != null)
				{
					this.TransferNode(j, j + shift);
				}
			}
		}

		// Token: 0x0600401F RID: 16415 RVA: 0x001E8914 File Offset: 0x001E6B14
		public void GetConnectedComputers(List<SplineComputer> computers, List<int> connectionIndices, List<int> connectedIndices, double percent, Spline.Direction direction, bool includeEqual)
		{
			if (computers == null)
			{
				computers = new List<SplineComputer>();
			}
			if (connectionIndices == null)
			{
				connectionIndices = new List<int>();
			}
			if (connectedIndices == null)
			{
				connectionIndices = new List<int>();
			}
			computers.Clear();
			connectionIndices.Clear();
			connectedIndices.Clear();
			int num = Mathf.FloorToInt((float)(this.pointCount - 1) * (float)percent);
			for (int i = 0; i < this.nodes.Length; i++)
			{
				bool flag = false;
				if (includeEqual)
				{
					if (direction == Spline.Direction.Forward)
					{
						flag = (this.nodes[i].pointIndex >= num);
					}
					else
					{
						flag = (this.nodes[i].pointIndex <= num);
					}
				}
				if (flag)
				{
					Node.Connection[] connections = this.nodes[i].node.GetConnections();
					for (int j = 0; j < connections.Length; j++)
					{
						if (connections[j].spline != this)
						{
							computers.Add(connections[j].spline);
							connectionIndices.Add(this.nodes[i].pointIndex);
							connectedIndices.Add(connections[j].pointIndex);
						}
					}
				}
			}
		}

		// Token: 0x06004020 RID: 16416 RVA: 0x001E8A1C File Offset: 0x001E6C1C
		public List<SplineComputer> GetConnectedComputers()
		{
			List<SplineComputer> list = new List<SplineComputer>();
			list.Add(this);
			if (this.nodes.Length == 0)
			{
				return list;
			}
			this.GetConnectedComputers(ref list);
			return list;
		}

		// Token: 0x06004021 RID: 16417 RVA: 0x001E8A4A File Offset: 0x001E6C4A
		public void GetSamplingValues(double percent, out int index, out double lerp)
		{
			this.sampleCollection.GetSamplingValues(percent, out index, out lerp);
		}

		// Token: 0x06004022 RID: 16418 RVA: 0x001E8A5C File Offset: 0x001E6C5C
		private void GetConnectedComputers(ref List<SplineComputer> computers)
		{
			SplineComputer splineComputer = computers[computers.Count - 1];
			if (splineComputer == null)
			{
				return;
			}
			for (int i = 0; i < splineComputer.nodes.Length; i++)
			{
				if (!(splineComputer.nodes[i].node == null))
				{
					Node.Connection[] connections = splineComputer.nodes[i].node.GetConnections();
					for (int j = 0; j < connections.Length; j++)
					{
						bool flag = false;
						if (!(connections[j].spline == this))
						{
							for (int k = 0; k < computers.Count; k++)
							{
								if (computers[k] == connections[j].spline)
								{
									flag = true;
									break;
								}
							}
							if (!flag)
							{
								computers.Add(connections[j].spline);
								this.GetConnectedComputers(ref computers);
							}
						}
					}
				}
			}
		}

		// Token: 0x06004023 RID: 16419 RVA: 0x001E8B38 File Offset: 0x001E6D38
		private void RemoveNodeLinkAt(int index)
		{
			SplineComputer.NodeLink[] array = new SplineComputer.NodeLink[this.nodes.Length - 1];
			for (int i = 0; i < this.nodes.Length; i++)
			{
				if (i != index)
				{
					if (i < index)
					{
						array[i] = this.nodes[i];
					}
					else
					{
						array[i - 1] = this.nodes[i];
					}
				}
			}
			this.nodes = array;
		}

		// Token: 0x06004024 RID: 16420 RVA: 0x001E8B94 File Offset: 0x001E6D94
		private void SetNodeForPoint(int index, SplinePoint worldPoint)
		{
			for (int i = 0; i < this.nodes.Length; i++)
			{
				if (this.nodes[i].pointIndex == index)
				{
					this.nodes[i].node.UpdatePoint(this, this.nodes[i].pointIndex, worldPoint, true);
					return;
				}
			}
		}

		// Token: 0x06004025 RID: 16421 RVA: 0x001E8BE8 File Offset: 0x001E6DE8
		private void UpdateConnectedNodes(SplinePoint[] worldPoints)
		{
			for (int i = 0; i < this.nodes.Length; i++)
			{
				if (this.nodes[i].node == null)
				{
					this.RemoveNodeLinkAt(i);
					i--;
					this.Rebuild(false);
				}
				else
				{
					bool flag = false;
					Node.Connection[] connections = this.nodes[i].node.GetConnections();
					for (int j = 0; j < connections.Length; j++)
					{
						if (connections[j].spline == this)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						this.RemoveNodeLinkAt(i);
						i--;
						this.Rebuild(false);
					}
					else
					{
						this.nodes[i].node.UpdatePoint(this, this.nodes[i].pointIndex, worldPoints[this.nodes[i].pointIndex], true);
						this.nodes[i].node.UpdateConnectedComputers(this);
					}
				}
			}
		}

		// Token: 0x06004026 RID: 16422 RVA: 0x001E8CD0 File Offset: 0x001E6ED0
		private void UpdateConnectedNodes()
		{
			for (int i = 0; i < this.nodes.Length; i++)
			{
				if (this.nodes[i].node == null)
				{
					this.RemoveNodeLinkAt(i);
					this.Rebuild(false);
					i--;
				}
				else
				{
					bool flag = false;
					Node.Connection[] connections = this.nodes[i].node.GetConnections();
					for (int j = 0; j < connections.Length; j++)
					{
						if (connections[j].spline == this && connections[j].pointIndex == this.nodes[i].pointIndex)
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						this.nodes[i].node.UpdatePoint(this, this.nodes[i].pointIndex, this.GetPoint(this.nodes[i].pointIndex, SplineComputer.Space.World), true);
						this.nodes[i].node.UpdateConnectedComputers(this);
					}
					else
					{
						this.RemoveNodeLinkAt(i);
						this.Rebuild(false);
						i--;
					}
				}
			}
		}

		// Token: 0x06004027 RID: 16423 RVA: 0x001E8DCF File Offset: 0x001E6FCF
		public Vector3 TransformPoint(Vector3 point)
		{
			return this.transformMatrix.MultiplyPoint3x4(point);
		}

		// Token: 0x06004028 RID: 16424 RVA: 0x001E8DDD File Offset: 0x001E6FDD
		public Vector3 InverseTransformPoint(Vector3 point)
		{
			return this.inverseTransformMatrix.MultiplyPoint3x4(point);
		}

		// Token: 0x06004029 RID: 16425 RVA: 0x001E8DEB File Offset: 0x001E6FEB
		public Vector3 TransformDirection(Vector3 direction)
		{
			return this.transformMatrix.MultiplyVector(direction);
		}

		// Token: 0x0600402A RID: 16426 RVA: 0x001E8DF9 File Offset: 0x001E6FF9
		public Vector3 InverseTransformDirection(Vector3 direction)
		{
			return this.inverseTransformMatrix.MultiplyVector(direction);
		}

		// Token: 0x04002CEA RID: 11498
		[HideInInspector]
		public bool multithreaded;

		// Token: 0x04002CEB RID: 11499
		[HideInInspector]
		public bool rebuildOnAwake;

		// Token: 0x04002CEC RID: 11500
		[HideInInspector]
		public SplineComputer.UpdateMode updateMode;

		// Token: 0x04002CED RID: 11501
		[HideInInspector]
		public TriggerGroup[] triggerGroups = new TriggerGroup[0];

		// Token: 0x04002CEE RID: 11502
		[HideInInspector]
		[SerializeField]
		private Spline spline = new Spline(Spline.Type.CatmullRom);

		// Token: 0x04002CEF RID: 11503
		[HideInInspector]
		[SerializeField]
		private SplineSample[] _rawSamples = new SplineSample[0];

		// Token: 0x04002CF0 RID: 11504
		[HideInInspector]
		[SerializeField]
		private SplineSample[] _transformedSamples = new SplineSample[0];

		// Token: 0x04002CF1 RID: 11505
		[HideInInspector]
		[SerializeField]
		private SampleCollection sampleCollection = new SampleCollection();

		// Token: 0x04002CF2 RID: 11506
		[HideInInspector]
		[SerializeField]
		private double[] originalSamplePercents = new double[0];

		// Token: 0x04002CF3 RID: 11507
		private bool[] sampleFlter = new bool[0];

		// Token: 0x04002CF4 RID: 11508
		[HideInInspector]
		[SerializeField]
		private int _sampleCount;

		// Token: 0x04002CF5 RID: 11509
		[HideInInspector]
		[SerializeField]
		private bool _is2D;

		// Token: 0x04002CF6 RID: 11510
		[HideInInspector]
		[SerializeField]
		private bool hasSamples;

		// Token: 0x04002CF7 RID: 11511
		[HideInInspector]
		[SerializeField]
		private bool[] pointsDirty = new bool[0];

		// Token: 0x04002CF8 RID: 11512
		[HideInInspector]
		[SerializeField]
		[Range(0.001f, 45f)]
		private float _optimizeAngleThreshold = 0.5f;

		// Token: 0x04002CF9 RID: 11513
		[HideInInspector]
		[SerializeField]
		private SplineComputer.Space _space = SplineComputer.Space.Local;

		// Token: 0x04002CFA RID: 11514
		[HideInInspector]
		[SerializeField]
		private SplineComputer.SampleMode _sampleMode;

		// Token: 0x04002CFB RID: 11515
		[HideInInspector]
		[SerializeField]
		private SplineUser[] subscribers = new SplineUser[0];

		// Token: 0x04002CFC RID: 11516
		[HideInInspector]
		[SerializeField]
		[FormerlySerializedAs("_nodeLinks")]
		private SplineComputer.NodeLink[] nodes = new SplineComputer.NodeLink[0];

		// Token: 0x04002CFD RID: 11517
		private bool rebuildPending;

		// Token: 0x04002CFE RID: 11518
		private bool _trsCheck;

		// Token: 0x04002CFF RID: 11519
		private Transform _trs;

		// Token: 0x04002D00 RID: 11520
		private Matrix4x4 transformMatrix;

		// Token: 0x04002D01 RID: 11521
		private Matrix4x4 inverseTransformMatrix;

		// Token: 0x04002D02 RID: 11522
		private bool queueResample;

		// Token: 0x04002D03 RID: 11523
		private bool queueRebuild;

		// Token: 0x04002D04 RID: 11524
		private Vector3 lastPosition = Vector3.zero;

		// Token: 0x04002D05 RID: 11525
		private Vector3 lastScale = Vector3.zero;

		// Token: 0x04002D06 RID: 11526
		private bool uniformScale = true;

		// Token: 0x04002D07 RID: 11527
		private Quaternion lastRotation = Quaternion.identity;

		// Token: 0x02000999 RID: 2457
		public enum Space
		{
			// Token: 0x040044A9 RID: 17577
			World,
			// Token: 0x040044AA RID: 17578
			Local
		}

		// Token: 0x0200099A RID: 2458
		public enum EvaluateMode
		{
			// Token: 0x040044AC RID: 17580
			Cached,
			// Token: 0x040044AD RID: 17581
			Calculate
		}

		// Token: 0x0200099B RID: 2459
		public enum SampleMode
		{
			// Token: 0x040044AF RID: 17583
			Default,
			// Token: 0x040044B0 RID: 17584
			Uniform,
			// Token: 0x040044B1 RID: 17585
			Optimized
		}

		// Token: 0x0200099C RID: 2460
		public enum UpdateMode
		{
			// Token: 0x040044B3 RID: 17587
			Update,
			// Token: 0x040044B4 RID: 17588
			FixedUpdate,
			// Token: 0x040044B5 RID: 17589
			LateUpdate,
			// Token: 0x040044B6 RID: 17590
			AllUpdate,
			// Token: 0x040044B7 RID: 17591
			None
		}

		// Token: 0x0200099D RID: 2461
		[Serializable]
		internal class NodeLink
		{
			// Token: 0x06005454 RID: 21588 RVA: 0x00246540 File Offset: 0x00244740
			internal List<Node.Connection> GetConnections(SplineComputer exclude)
			{
				Node.Connection[] connections = this.node.GetConnections();
				List<Node.Connection> list = new List<Node.Connection>();
				for (int i = 0; i < connections.Length; i++)
				{
					if (!(connections[i].spline == exclude))
					{
						list.Add(connections[i]);
					}
				}
				return list;
			}

			// Token: 0x040044B8 RID: 17592
			[SerializeField]
			internal Node node;

			// Token: 0x040044B9 RID: 17593
			[SerializeField]
			internal int pointIndex;
		}
	}
}
