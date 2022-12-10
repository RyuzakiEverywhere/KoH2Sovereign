using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dreamteck.Splines
{
	// Token: 0x020004B2 RID: 1202
	[AddComponentMenu("Dreamteck/Splines/Users/Object Bender")]
	public class ObjectBender : SplineUser
	{
		// Token: 0x17000447 RID: 1095
		// (get) Token: 0x06003F13 RID: 16147 RVA: 0x001E1F8B File Offset: 0x001E018B
		// (set) Token: 0x06003F14 RID: 16148 RVA: 0x001E1F93 File Offset: 0x001E0193
		public bool bend
		{
			get
			{
				return this._bend;
			}
			set
			{
				if (this._bend != value)
				{
					this._bend = value;
					if (value)
					{
						this.UpdateReferences();
						this.Rebuild();
						return;
					}
					this.Revert();
				}
			}
		}

		// Token: 0x17000448 RID: 1096
		// (get) Token: 0x06003F15 RID: 16149 RVA: 0x001E1FBB File Offset: 0x001E01BB
		// (set) Token: 0x06003F16 RID: 16150 RVA: 0x001E1FC3 File Offset: 0x001E01C3
		public ObjectBender.Axis axis
		{
			get
			{
				return this._axis;
			}
			set
			{
				if (base.spline != null && value != this._axis)
				{
					this._axis = value;
					this.UpdateReferences();
					this.Rebuild();
					return;
				}
				this._axis = value;
			}
		}

		// Token: 0x17000449 RID: 1097
		// (get) Token: 0x06003F17 RID: 16151 RVA: 0x001E1FF7 File Offset: 0x001E01F7
		// (set) Token: 0x06003F18 RID: 16152 RVA: 0x001E1FFF File Offset: 0x001E01FF
		public ObjectBender.NormalMode upMode
		{
			get
			{
				return this._normalMode;
			}
			set
			{
				if (base.spline != null && value != this._normalMode)
				{
					this._normalMode = value;
					this.Rebuild();
					return;
				}
				this._normalMode = value;
			}
		}

		// Token: 0x1700044A RID: 1098
		// (get) Token: 0x06003F19 RID: 16153 RVA: 0x001E202D File Offset: 0x001E022D
		// (set) Token: 0x06003F1A RID: 16154 RVA: 0x001E2035 File Offset: 0x001E0235
		public Vector3 customNormal
		{
			get
			{
				return this._customNormal;
			}
			set
			{
				if (base.spline != null && value != this._customNormal)
				{
					this._customNormal = value;
					this.Rebuild();
					return;
				}
				this._customNormal = value;
			}
		}

		// Token: 0x1700044B RID: 1099
		// (get) Token: 0x06003F1B RID: 16155 RVA: 0x001E2068 File Offset: 0x001E0268
		// (set) Token: 0x06003F1C RID: 16156 RVA: 0x001E2070 File Offset: 0x001E0270
		public ObjectBender.ForwardMode forwardMode
		{
			get
			{
				return this._forwardMode;
			}
			set
			{
				if (base.spline != null && value != this._forwardMode)
				{
					this._forwardMode = value;
					this.Rebuild();
					return;
				}
				this._forwardMode = value;
			}
		}

		// Token: 0x1700044C RID: 1100
		// (get) Token: 0x06003F1D RID: 16157 RVA: 0x001E209E File Offset: 0x001E029E
		// (set) Token: 0x06003F1E RID: 16158 RVA: 0x001E20A6 File Offset: 0x001E02A6
		public Vector3 customForward
		{
			get
			{
				return this._customForward;
			}
			set
			{
				if (base.spline != null && value != this._customForward)
				{
					this._customForward = value;
					this.Rebuild();
					return;
				}
				this._customForward = value;
			}
		}

		// Token: 0x06003F1F RID: 16159 RVA: 0x001E20DC File Offset: 0x001E02DC
		private void GetTransformsRecursively(Transform current, ref List<Transform> transformList)
		{
			transformList.Add(current);
			foreach (object obj in current)
			{
				Transform current2 = (Transform)obj;
				this.GetTransformsRecursively(current2, ref transformList);
			}
		}

		// Token: 0x06003F20 RID: 16160 RVA: 0x001E213C File Offset: 0x001E033C
		private void GetObjects()
		{
			List<Transform> list = new List<Transform>();
			this.GetTransformsRecursively(base.transform, ref list);
			ObjectBender.BendProperty[] array = new ObjectBender.BendProperty[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				this.CreateProperty(ref array[i], list[i]);
			}
			this.bendProperties = array;
		}

		// Token: 0x06003F21 RID: 16161 RVA: 0x001E2195 File Offset: 0x001E0395
		public TS_Bounds GetBounds()
		{
			return new TS_Bounds(this.bounds.min, this.bounds.max, this.bounds.center);
		}

		// Token: 0x06003F22 RID: 16162 RVA: 0x001E21C0 File Offset: 0x001E03C0
		private void CreateProperty(ref ObjectBender.BendProperty property, Transform t)
		{
			property = new ObjectBender.BendProperty(t, t == this.trs);
			for (int i = 0; i < this.bendProperties.Length; i++)
			{
				if (this.bendProperties[i].transform.transform == t)
				{
					property.enabled = this.bendProperties[i].enabled;
					property.applyRotation = this.bendProperties[i].applyRotation;
					property.applyScale = this.bendProperties[i].applyScale;
					property.bendMesh = this.bendProperties[i].bendMesh;
					property.bendCollider = this.bendProperties[i].bendCollider;
					property.generateLightmapUVs = this.bendProperties[i].generateLightmapUVs;
					property.colliderUpdateRate = this.bendProperties[i].colliderUpdateRate;
					break;
				}
			}
			if (t.transform != this.trs)
			{
				property.originalPosition = this.trs.InverseTransformPoint(t.position);
				property.originalRotation = Quaternion.Inverse(this.trs.rotation) * t.rotation;
			}
		}

		// Token: 0x06003F23 RID: 16163 RVA: 0x001E22F4 File Offset: 0x001E04F4
		private void CalculateBounds()
		{
			if (this.bounds == null)
			{
				this.bounds = new TS_Bounds(Vector3.zero, Vector3.zero);
			}
			this.bounds.min = (this.bounds.max = Vector3.zero);
			for (int i = 0; i < this.bendProperties.Length; i++)
			{
				this.CalculatePropertyBounds(ref this.bendProperties[i]);
			}
			for (int j = 0; j < this.bendProperties.Length; j++)
			{
				this.CalculatePercents(this.bendProperties[j]);
			}
		}

		// Token: 0x06003F24 RID: 16164 RVA: 0x001E2384 File Offset: 0x001E0584
		private void CalculatePropertyBounds(ref ObjectBender.BendProperty property)
		{
			if (property.transform.transform == this.trs)
			{
				if (0f < this.bounds.min.x)
				{
					this.bounds.min.x = 0f;
				}
				if (0f < this.bounds.min.y)
				{
					this.bounds.min.y = 0f;
				}
				if (0f < this.bounds.min.z)
				{
					this.bounds.min.z = 0f;
				}
				if (0f > this.bounds.max.x)
				{
					this.bounds.max.x = 0f;
				}
				if (0f > this.bounds.max.y)
				{
					this.bounds.max.y = 0f;
				}
				if (0f > this.bounds.max.z)
				{
					this.bounds.max.z = 0f;
				}
			}
			else
			{
				if (property.originalPosition.x < this.bounds.min.x)
				{
					this.bounds.min.x = property.originalPosition.x;
				}
				if (property.originalPosition.y < this.bounds.min.y)
				{
					this.bounds.min.y = property.originalPosition.y;
				}
				if (property.originalPosition.z < this.bounds.min.z)
				{
					this.bounds.min.z = property.originalPosition.z;
				}
				if (property.originalPosition.x > this.bounds.max.x)
				{
					this.bounds.max.x = property.originalPosition.x;
				}
				if (property.originalPosition.y > this.bounds.max.y)
				{
					this.bounds.max.y = property.originalPosition.y;
				}
				if (property.originalPosition.z > this.bounds.max.z)
				{
					this.bounds.max.z = property.originalPosition.z;
				}
			}
			if (property.editMesh != null)
			{
				for (int i = 0; i < property.editMesh.vertices.Length; i++)
				{
					Vector3 vector = property.transform.TransformPoint(property.editMesh.vertices[i]);
					vector = this.trs.InverseTransformPoint(vector);
					if (vector.x < this.bounds.min.x)
					{
						this.bounds.min.x = vector.x;
					}
					if (vector.y < this.bounds.min.y)
					{
						this.bounds.min.y = vector.y;
					}
					if (vector.z < this.bounds.min.z)
					{
						this.bounds.min.z = vector.z;
					}
					if (vector.x > this.bounds.max.x)
					{
						this.bounds.max.x = vector.x;
					}
					if (vector.y > this.bounds.max.y)
					{
						this.bounds.max.y = vector.y;
					}
					if (vector.z > this.bounds.max.z)
					{
						this.bounds.max.z = vector.z;
					}
				}
			}
			if (property.editColliderMesh != null)
			{
				for (int j = 0; j < property.editColliderMesh.vertices.Length; j++)
				{
					Vector3 vector2 = property.transform.TransformPoint(property.editColliderMesh.vertices[j]);
					vector2 = this.trs.InverseTransformPoint(vector2);
					if (vector2.x < this.bounds.min.x)
					{
						this.bounds.min.x = vector2.x;
					}
					if (vector2.y < this.bounds.min.y)
					{
						this.bounds.min.y = vector2.y;
					}
					if (vector2.z < this.bounds.min.z)
					{
						this.bounds.min.z = vector2.z;
					}
					if (vector2.x > this.bounds.max.x)
					{
						this.bounds.max.x = vector2.x;
					}
					if (vector2.y > this.bounds.max.y)
					{
						this.bounds.max.y = vector2.y;
					}
					if (vector2.z > this.bounds.max.z)
					{
						this.bounds.max.z = vector2.z;
					}
				}
			}
			if (property.originalSpline != null)
			{
				for (int k = 0; k < property.originalSpline.points.Length; k++)
				{
					Vector3 vector3 = this.trs.InverseTransformPoint(property.originalSpline.points[k].position);
					if (vector3.x < this.bounds.min.x)
					{
						this.bounds.min.x = vector3.x;
					}
					if (vector3.y < this.bounds.min.y)
					{
						this.bounds.min.y = vector3.y;
					}
					if (vector3.z < this.bounds.min.z)
					{
						this.bounds.min.z = vector3.z;
					}
					if (vector3.x > this.bounds.max.x)
					{
						this.bounds.max.x = vector3.x;
					}
					if (vector3.y > this.bounds.max.y)
					{
						this.bounds.max.y = vector3.y;
					}
					if (vector3.z > this.bounds.max.z)
					{
						this.bounds.max.z = vector3.z;
					}
				}
			}
			this.bounds.CreateFromMinMax(this.bounds.min, this.bounds.max);
		}

		// Token: 0x06003F25 RID: 16165 RVA: 0x001E2A84 File Offset: 0x001E0C84
		public void CalculatePercents(ObjectBender.BendProperty property)
		{
			if (property.transform.transform != this.trs)
			{
				property.positionPercent = this.GetPercentage(this.trs.InverseTransformPoint(property.transform.position));
			}
			else
			{
				property.positionPercent = this.GetPercentage(Vector3.zero);
			}
			if (property.editMesh != null)
			{
				if (property.vertexPercents.Length != property.editMesh.vertexCount)
				{
					property.vertexPercents = new Vector3[property.editMesh.vertexCount];
				}
				if (property.editColliderMesh != null && property.colliderVertexPercents.Length != property.editMesh.vertexCount)
				{
					property.colliderVertexPercents = new Vector3[property.editColliderMesh.vertexCount];
				}
				for (int i = 0; i < property.editMesh.vertexCount; i++)
				{
					Vector3 vector = property.transform.TransformPoint(property.editMesh.vertices[i]);
					vector = this.trs.InverseTransformPoint(vector);
					property.vertexPercents[i] = this.GetPercentage(vector);
				}
				if (property.editColliderMesh != null)
				{
					for (int j = 0; j < property.editColliderMesh.vertexCount; j++)
					{
						Vector3 vector2 = property.transform.TransformPoint(property.editColliderMesh.vertices[j]);
						vector2 = this.trs.InverseTransformPoint(vector2);
						property.colliderVertexPercents[j] = this.GetPercentage(vector2);
					}
				}
			}
			if (property.splineComputer != null)
			{
				SplinePoint[] points = property.splineComputer.GetPoints(SplineComputer.Space.World);
				property.splinePointPercents = new Vector3[points.Length];
				property.primaryTangentPercents = new Vector3[points.Length];
				property.secondaryTangentPercents = new Vector3[points.Length];
				for (int k = 0; k < points.Length; k++)
				{
					property.splinePointPercents[k] = this.GetPercentage(this.trs.InverseTransformPoint(points[k].position));
					property.primaryTangentPercents[k] = this.GetPercentage(this.trs.InverseTransformPoint(points[k].tangent));
					property.secondaryTangentPercents[k] = this.GetPercentage(this.trs.InverseTransformPoint(points[k].tangent2));
				}
			}
		}

		// Token: 0x06003F26 RID: 16166 RVA: 0x001E2CE0 File Offset: 0x001E0EE0
		private void Revert()
		{
			for (int i = 0; i < this.bendProperties.Length; i++)
			{
				this.bendProperties[i].Revert();
			}
		}

		// Token: 0x06003F27 RID: 16167 RVA: 0x001E2D10 File Offset: 0x001E0F10
		public void UpdateReferences()
		{
			this.trs = base.transform;
			if (this._bend)
			{
				for (int i = 0; i < this.bendProperties.Length; i++)
				{
					this.bendProperties[i].Revert();
				}
			}
			this.GetObjects();
			this.CalculateBounds();
			if (this._bend)
			{
				this.Bend();
				for (int j = 0; j < this.bendProperties.Length; j++)
				{
					this.bendProperties[j].Apply(j > 0 || this.trs != base.spline.transform);
					this.bendProperties[j].Update();
				}
			}
		}

		// Token: 0x06003F28 RID: 16168 RVA: 0x001E2DB8 File Offset: 0x001E0FB8
		private void GetevalResult(Vector3 percentage)
		{
			switch (this.axis)
			{
			case ObjectBender.Axis.X:
				base.Evaluate((double)percentage.x, this.evalResult);
				break;
			case ObjectBender.Axis.Y:
				base.Evaluate((double)percentage.y, this.evalResult);
				break;
			case ObjectBender.Axis.Z:
				base.Evaluate((double)percentage.z, this.evalResult);
				break;
			}
			ObjectBender.NormalMode normalMode = this._normalMode;
			if (normalMode != ObjectBender.NormalMode.Auto)
			{
				if (normalMode == ObjectBender.NormalMode.Custom)
				{
					this.evalResult.up = this._customNormal;
				}
			}
			else
			{
				this.evalResult.up = Vector3.Cross(this.evalResult.forward, this.evalResult.right);
			}
			if (this._forwardMode == ObjectBender.ForwardMode.Custom)
			{
				this.evalResult.forward = this.customForward;
			}
			base.ModifySample(this.evalResult);
			Vector3 right = this.evalResult.right;
			Quaternion rhs = Quaternion.identity;
			switch (this.axis)
			{
			case ObjectBender.Axis.X:
				rhs = Quaternion.Euler(0f, -90f, 0f);
				this.evalResult.position += right * Mathf.Lerp(this.bounds.max.z, this.bounds.min.z, percentage.z) * this.evalResult.size;
				this.evalResult.position += this.evalResult.up * Mathf.Lerp(this.bounds.min.y, this.bounds.max.y, percentage.y) * this.evalResult.size;
				break;
			case ObjectBender.Axis.Y:
				rhs = Quaternion.Euler(90f, 0f, 0f);
				this.evalResult.position += right * Mathf.Lerp(this.bounds.min.x, this.bounds.max.x, percentage.x) * this.evalResult.size;
				this.evalResult.position += this.evalResult.up * Mathf.Lerp(this.bounds.min.z, this.bounds.max.z, percentage.z) * this.evalResult.size;
				break;
			case ObjectBender.Axis.Z:
				this.evalResult.position += right * Mathf.Lerp(this.bounds.min.x, this.bounds.max.x, percentage.x) * this.evalResult.size;
				this.evalResult.position += this.evalResult.up * Mathf.Lerp(this.bounds.min.y, this.bounds.max.y, percentage.y) * this.evalResult.size;
				break;
			}
			this.bendRotation = this.evalResult.rotation * rhs;
			this.normalMatrix = Matrix4x4.TRS(this.evalResult.position, this.bendRotation, Vector3.one * this.evalResult.size).inverse.transpose;
		}

		// Token: 0x06003F29 RID: 16169 RVA: 0x001E3174 File Offset: 0x001E1374
		private Vector3 GetPercentage(Vector3 point)
		{
			point.x = Mathf.InverseLerp(this.bounds.min.x, this.bounds.max.x, point.x);
			point.y = Mathf.InverseLerp(this.bounds.min.y, this.bounds.max.y, point.y);
			point.z = Mathf.InverseLerp(this.bounds.min.z, this.bounds.max.z, point.z);
			return point;
		}

		// Token: 0x06003F2A RID: 16170 RVA: 0x001E3218 File Offset: 0x001E1418
		protected override void Build()
		{
			base.Build();
			if (this._bend)
			{
				this.Bend();
			}
		}

		// Token: 0x06003F2B RID: 16171 RVA: 0x001E3230 File Offset: 0x001E1430
		private void Bend()
		{
			if (base.sampleCount <= 1)
			{
				return;
			}
			if (this.bendProperties.Length == 0)
			{
				return;
			}
			for (int i = 0; i < this.bendProperties.Length; i++)
			{
				this.BendObject(this.bendProperties[i]);
			}
		}

		// Token: 0x06003F2C RID: 16172 RVA: 0x001E3274 File Offset: 0x001E1474
		public void BendObject(ObjectBender.BendProperty p)
		{
			if (!p.enabled)
			{
				return;
			}
			this.GetevalResult(p.positionPercent);
			p.transform.position = this.evalResult.position;
			if (p.applyRotation)
			{
				p.transform.rotation = this.bendRotation * (Quaternion.Inverse(p.parentRotation) * p.originalRotation);
			}
			else
			{
				p.transform.rotation = p.originalRotation;
			}
			if (p.applyScale)
			{
				p.transform.scale = p.originalScale * this.evalResult.size;
			}
			Matrix4x4 inverse = Matrix4x4.TRS(p.transform.position, p.transform.rotation, p.transform.scale).inverse;
			if (p.editMesh != null)
			{
				this.BendMesh(p.vertexPercents, p.normals, p.editMesh, inverse);
				p.editMesh.hasUpdate = true;
			}
			if (p._editColliderMesh != null)
			{
				this.BendMesh(p.colliderVertexPercents, p.colliderNormals, p.editColliderMesh, inverse);
				p.editColliderMesh.hasUpdate = true;
			}
			if (p.originalSpline != null)
			{
				for (int i = 0; i < p.splinePointPercents.Length; i++)
				{
					SplinePoint splinePoint = p.originalSpline.points[i];
					this.GetevalResult(p.splinePointPercents[i]);
					splinePoint.position = this.evalResult.position;
					this.GetevalResult(p.primaryTangentPercents[i]);
					splinePoint.tangent = this.evalResult.position;
					this.GetevalResult(p.secondaryTangentPercents[i]);
					splinePoint.tangent2 = this.evalResult.position;
					switch (this.axis)
					{
					case ObjectBender.Axis.X:
						splinePoint.normal = Quaternion.LookRotation(this.evalResult.forward, this.evalResult.up) * Quaternion.FromToRotation(Vector3.up, this.evalResult.up) * splinePoint.normal;
						break;
					case ObjectBender.Axis.Y:
						splinePoint.normal = Quaternion.LookRotation(this.evalResult.forward, this.evalResult.up) * Quaternion.FromToRotation(Vector3.up, this.evalResult.up) * splinePoint.normal;
						break;
					case ObjectBender.Axis.Z:
						splinePoint.normal = Quaternion.LookRotation(this.evalResult.forward, this.evalResult.up) * splinePoint.normal;
						break;
					}
					p.destinationSpline.points[i] = splinePoint;
				}
			}
		}

		// Token: 0x06003F2D RID: 16173 RVA: 0x001E353C File Offset: 0x001E173C
		private void BendMesh(Vector3[] vertexPercents, Vector3[] originalNormals, TS_Mesh mesh, Matrix4x4 worldToLocalMatrix)
		{
			if (mesh.vertexCount != vertexPercents.Length)
			{
				Debug.LogError("Vertex count mismatch");
				return;
			}
			for (int i = 0; i < mesh.vertexCount; i++)
			{
				Vector3 vector = vertexPercents[i];
				if (this.axis == ObjectBender.Axis.Y)
				{
					vector.z = 1f - vector.z;
				}
				this.GetevalResult(vector);
				mesh.vertices[i] = worldToLocalMatrix.MultiplyPoint3x4(this.evalResult.position);
				mesh.normals[i] = worldToLocalMatrix.MultiplyVector(this.normalMatrix.MultiplyVector(originalNormals[i]));
			}
		}

		// Token: 0x06003F2E RID: 16174 RVA: 0x001E35E0 File Offset: 0x001E17E0
		protected override void PostBuild()
		{
			base.PostBuild();
			if (!this._bend)
			{
				return;
			}
			for (int i = 0; i < this.bendProperties.Length; i++)
			{
				this.bendProperties[i].Apply(i > 0 || this.trs != base.spline.transform);
				this.bendProperties[i].Update();
			}
		}

		// Token: 0x06003F2F RID: 16175 RVA: 0x001E3648 File Offset: 0x001E1848
		protected override void LateRun()
		{
			base.LateRun();
			for (int i = 0; i < this.bendProperties.Length; i++)
			{
				this.bendProperties[i].Update();
			}
		}

		// Token: 0x04002CA5 RID: 11429
		[SerializeField]
		[HideInInspector]
		private bool _bend;

		// Token: 0x04002CA6 RID: 11430
		[HideInInspector]
		public ObjectBender.BendProperty[] bendProperties = new ObjectBender.BendProperty[0];

		// Token: 0x04002CA7 RID: 11431
		[SerializeField]
		[HideInInspector]
		private TS_Bounds bounds;

		// Token: 0x04002CA8 RID: 11432
		[SerializeField]
		[HideInInspector]
		private ObjectBender.Axis _axis = ObjectBender.Axis.Z;

		// Token: 0x04002CA9 RID: 11433
		[SerializeField]
		[HideInInspector]
		private ObjectBender.NormalMode _normalMode = ObjectBender.NormalMode.Auto;

		// Token: 0x04002CAA RID: 11434
		[SerializeField]
		[HideInInspector]
		private ObjectBender.ForwardMode _forwardMode;

		// Token: 0x04002CAB RID: 11435
		[SerializeField]
		[HideInInspector]
		[FormerlySerializedAs("_upVector")]
		private Vector3 _customNormal = Vector3.up;

		// Token: 0x04002CAC RID: 11436
		[SerializeField]
		[HideInInspector]
		private Vector3 _customForward = Vector3.forward;

		// Token: 0x04002CAD RID: 11437
		private Matrix4x4 normalMatrix;

		// Token: 0x04002CAE RID: 11438
		private Quaternion bendRotation = Quaternion.identity;

		// Token: 0x02000985 RID: 2437
		public enum Axis
		{
			// Token: 0x0400443F RID: 17471
			X,
			// Token: 0x04004440 RID: 17472
			Y,
			// Token: 0x04004441 RID: 17473
			Z
		}

		// Token: 0x02000986 RID: 2438
		public enum NormalMode
		{
			// Token: 0x04004443 RID: 17475
			Spline,
			// Token: 0x04004444 RID: 17476
			Auto,
			// Token: 0x04004445 RID: 17477
			Custom
		}

		// Token: 0x02000987 RID: 2439
		public enum ForwardMode
		{
			// Token: 0x04004447 RID: 17479
			Spline,
			// Token: 0x04004448 RID: 17480
			Custom
		}

		// Token: 0x02000988 RID: 2440
		[Serializable]
		public class BendProperty
		{
			// Token: 0x170006EF RID: 1775
			// (get) Token: 0x06005419 RID: 21529 RVA: 0x002454D9 File Offset: 0x002436D9
			public bool isValid
			{
				get
				{
					return this.transform != null && this.transform.transform != null;
				}
			}

			// Token: 0x170006F0 RID: 1776
			// (get) Token: 0x0600541A RID: 21530 RVA: 0x002454F6 File Offset: 0x002436F6
			// (set) Token: 0x0600541B RID: 21531 RVA: 0x00245500 File Offset: 0x00243700
			public bool bendMesh
			{
				get
				{
					return this._bendMesh;
				}
				set
				{
					if (value != this._bendMesh)
					{
						this._bendMesh = value;
						if (value)
						{
							if (this.filter != null && this.filter.sharedMesh != null)
							{
								this.normals = this.originalMesh.normals;
								for (int i = 0; i < this.normals.Length; i++)
								{
									this.normals[i] = this.transform.transform.TransformDirection(this.normals[i]);
								}
								return;
							}
						}
						else
						{
							this.RevertMesh();
						}
					}
				}
			}

			// Token: 0x170006F1 RID: 1777
			// (get) Token: 0x0600541C RID: 21532 RVA: 0x00245594 File Offset: 0x00243794
			// (set) Token: 0x0600541D RID: 21533 RVA: 0x0024559C File Offset: 0x0024379C
			public bool bendCollider
			{
				get
				{
					return this._bendCollider;
				}
				set
				{
					if (value != this._bendCollider)
					{
						this._bendCollider = value;
						if (value)
						{
							if (this.collider != null && this.collider.sharedMesh != null && this.collider.sharedMesh != this.originalMesh)
							{
								this.colliderNormals = this.originalColliderMesh.normals;
								return;
							}
						}
						else
						{
							this.RevertCollider();
						}
					}
				}
			}

			// Token: 0x170006F2 RID: 1778
			// (get) Token: 0x0600541E RID: 21534 RVA: 0x0024560D File Offset: 0x0024380D
			// (set) Token: 0x0600541F RID: 21535 RVA: 0x00245615 File Offset: 0x00243815
			public bool bendSpline
			{
				get
				{
					return this._bendSpline;
				}
				set
				{
					this._bendSpline = value;
				}
			}

			// Token: 0x170006F3 RID: 1779
			// (get) Token: 0x06005420 RID: 21536 RVA: 0x00245620 File Offset: 0x00243820
			public TS_Mesh editMesh
			{
				get
				{
					if (!this.bendMesh || this.originalMesh == null)
					{
						this._editMesh = null;
					}
					else if (this._editMesh == null && this.originalMesh != null)
					{
						this._editMesh = new TS_Mesh(this.originalMesh);
					}
					return this._editMesh;
				}
			}

			// Token: 0x170006F4 RID: 1780
			// (get) Token: 0x06005421 RID: 21537 RVA: 0x0024567C File Offset: 0x0024387C
			public TS_Mesh editColliderMesh
			{
				get
				{
					if (!this.bendCollider || this.originalColliderMesh == null)
					{
						this._editColliderMesh = null;
					}
					else if (this._editColliderMesh == null && this.originalColliderMesh != null && this.originalColliderMesh != this.originalMesh)
					{
						this._editColliderMesh = new TS_Mesh(this.originalColliderMesh);
					}
					return this._editColliderMesh;
				}
			}

			// Token: 0x170006F5 RID: 1781
			// (get) Token: 0x06005422 RID: 21538 RVA: 0x002456E8 File Offset: 0x002438E8
			public Spline originalSpline
			{
				get
				{
					if (!this.bendSpline || this.splineComputer == null)
					{
						this._originalSpline = null;
					}
					else if (this._originalSpline == null && this.splineComputer != null)
					{
						this._originalSpline = new Spline(this.splineComputer.type);
						this._originalSpline.points = this.splineComputer.GetPoints(SplineComputer.Space.World);
					}
					return this._originalSpline;
				}
			}

			// Token: 0x06005423 RID: 21539 RVA: 0x00245760 File Offset: 0x00243960
			public BendProperty(Transform t, bool isParent = false)
			{
				this.parent = isParent;
				this.transform = new TS_Transform(t);
				this.originalPosition = t.localPosition;
				this.originalScale = t.localScale;
				this.originalRotation = t.localRotation;
				this.parentRotation = t.transform.rotation;
				if (t.transform.parent != null)
				{
					this.parentRotation = t.transform.parent.rotation;
				}
				this.filter = t.GetComponent<MeshFilter>();
				this.collider = t.GetComponent<MeshCollider>();
				if (this.filter != null && this.filter.sharedMesh != null)
				{
					this.originalMesh = this.filter.sharedMesh;
					this.normals = this.originalMesh.normals;
					for (int i = 0; i < this.normals.Length; i++)
					{
						this.normals[i] = this.transform.transform.TransformDirection(this.normals[i]).normalized;
					}
				}
				if (this.collider != null && this.collider.sharedMesh != null)
				{
					this.originalColliderMesh = this.collider.sharedMesh;
					this.colliderNormals = this.originalColliderMesh.normals;
					for (int j = 0; j < this.colliderNormals.Length; j++)
					{
						this.colliderNormals[j] = this.transform.transform.TransformDirection(this.colliderNormals[j]);
					}
				}
				if (!this.parent)
				{
					this.splineComputer = t.GetComponent<SplineComputer>();
				}
				if (this.splineComputer != null)
				{
					if (this.splineComputer.isClosed)
					{
						this.originalSpline.Close();
					}
					this.destinationSpline = new Spline(this.originalSpline.type);
					this.destinationSpline.points = new SplinePoint[this.originalSpline.points.Length];
					this.destinationSpline.points = this.splineComputer.GetPoints(SplineComputer.Space.World);
					if (this.splineComputer.isClosed)
					{
						this.destinationSpline.Close();
					}
				}
			}

			// Token: 0x06005424 RID: 21540 RVA: 0x00245A54 File Offset: 0x00243C54
			public void Revert()
			{
				if (!this.isValid)
				{
					return;
				}
				this.RevertTransform();
				this.RevertCollider();
				this.RevertMesh();
				if (this.splineComputer != null)
				{
					this.splineComputer.SetPoints(this._originalSpline.points, SplineComputer.Space.World);
				}
			}

			// Token: 0x06005425 RID: 21541 RVA: 0x00245AA1 File Offset: 0x00243CA1
			private void RevertMesh()
			{
				if (this.filter != null)
				{
					this.filter.sharedMesh = this.originalMesh;
				}
				this.destinationMesh = null;
			}

			// Token: 0x06005426 RID: 21542 RVA: 0x00245ACC File Offset: 0x00243CCC
			private void RevertTransform()
			{
				this.transform.localPosition = this.originalPosition;
				this.transform.localRotation = this.originalRotation;
				this.transform.Update();
				this.transform.scale = this.originalScale;
				this.transform.Update();
			}

			// Token: 0x06005427 RID: 21543 RVA: 0x00245B22 File Offset: 0x00243D22
			private void RevertCollider()
			{
				if (this.collider != null)
				{
					this.collider.sharedMesh = this.originalColliderMesh;
				}
				this.destinationColliderMesh = null;
			}

			// Token: 0x06005428 RID: 21544 RVA: 0x00245B4C File Offset: 0x00243D4C
			public void Apply(bool applyTransform)
			{
				if (!this.enabled)
				{
					return;
				}
				if (!this.isValid)
				{
					return;
				}
				if (applyTransform)
				{
					this.transform.Update();
				}
				if (this.editMesh != null && this.editMesh.hasUpdate)
				{
					this.ApplyMesh();
				}
				if (this.bendCollider && this.collider != null && !this.updateCollider && ((this.editColliderMesh == null && this.editMesh != null) || this.editColliderMesh != null))
				{
					this.updateCollider = true;
					if (Application.isPlaying)
					{
						this.colliderUpdateDue = Time.time + this.colliderUpdateRate;
					}
				}
				if (this.splineComputer != null)
				{
					this.ApplySpline();
				}
			}

			// Token: 0x06005429 RID: 21545 RVA: 0x00245C00 File Offset: 0x00243E00
			public void Update()
			{
				if (Time.time >= this.colliderUpdateDue && this.updateCollider)
				{
					this.updateCollider = false;
					this.ApplyCollider();
				}
			}

			// Token: 0x0600542A RID: 21546 RVA: 0x00245C24 File Offset: 0x00243E24
			private void ApplyMesh()
			{
				if (this.filter == null)
				{
					return;
				}
				MeshUtility.CalculateTangents(this.editMesh);
				if (this.destinationMesh == null)
				{
					this.destinationMesh = new Mesh();
					this.destinationMesh.name = this.originalMesh.name;
				}
				this.editMesh.WriteMesh(ref this.destinationMesh);
				this.destinationMesh.RecalculateBounds();
				this.filter.sharedMesh = this.destinationMesh;
			}

			// Token: 0x0600542B RID: 21547 RVA: 0x00245CA8 File Offset: 0x00243EA8
			private void ApplyCollider()
			{
				if (this.collider == null)
				{
					return;
				}
				if (this.originalColliderMesh == this.originalMesh)
				{
					this.collider.sharedMesh = this.filter.sharedMesh;
					return;
				}
				MeshUtility.CalculateTangents(this.editColliderMesh);
				if (this.destinationColliderMesh == null)
				{
					this.destinationColliderMesh = new Mesh();
					this.destinationColliderMesh.name = this.originalColliderMesh.name;
				}
				this.editColliderMesh.WriteMesh(ref this.destinationColliderMesh);
				this.destinationColliderMesh.RecalculateBounds();
				this.collider.sharedMesh = this.destinationColliderMesh;
			}

			// Token: 0x0600542C RID: 21548 RVA: 0x00245D55 File Offset: 0x00243F55
			private void ApplySpline()
			{
				if (this.destinationSpline == null)
				{
					return;
				}
				this.splineComputer.SetPoints(this.destinationSpline.points, SplineComputer.Space.World);
			}

			// Token: 0x04004449 RID: 17481
			public bool enabled = true;

			// Token: 0x0400444A RID: 17482
			public TS_Transform transform;

			// Token: 0x0400444B RID: 17483
			public bool applyRotation = true;

			// Token: 0x0400444C RID: 17484
			public bool applyScale = true;

			// Token: 0x0400444D RID: 17485
			public bool generateLightmapUVs;

			// Token: 0x0400444E RID: 17486
			[SerializeField]
			[HideInInspector]
			private bool _bendMesh = true;

			// Token: 0x0400444F RID: 17487
			[SerializeField]
			[HideInInspector]
			private bool _bendSpline = true;

			// Token: 0x04004450 RID: 17488
			[SerializeField]
			[HideInInspector]
			private bool _bendCollider = true;

			// Token: 0x04004451 RID: 17489
			private float colliderUpdateDue;

			// Token: 0x04004452 RID: 17490
			public float colliderUpdateRate = 0.2f;

			// Token: 0x04004453 RID: 17491
			private bool updateCollider;

			// Token: 0x04004454 RID: 17492
			public Vector3 originalPosition = Vector3.zero;

			// Token: 0x04004455 RID: 17493
			public Vector3 originalScale = Vector3.one;

			// Token: 0x04004456 RID: 17494
			public Quaternion originalRotation = Quaternion.identity;

			// Token: 0x04004457 RID: 17495
			public Quaternion parentRotation = Quaternion.identity;

			// Token: 0x04004458 RID: 17496
			public Vector3 positionPercent;

			// Token: 0x04004459 RID: 17497
			public Vector3[] vertexPercents = new Vector3[0];

			// Token: 0x0400445A RID: 17498
			public Vector3[] normals = new Vector3[0];

			// Token: 0x0400445B RID: 17499
			public Vector3[] colliderVertexPercents = new Vector3[0];

			// Token: 0x0400445C RID: 17500
			public Vector3[] colliderNormals = new Vector3[0];

			// Token: 0x0400445D RID: 17501
			[SerializeField]
			[HideInInspector]
			private Mesh originalMesh;

			// Token: 0x0400445E RID: 17502
			[SerializeField]
			[HideInInspector]
			private Mesh originalColliderMesh;

			// Token: 0x0400445F RID: 17503
			private Spline _originalSpline;

			// Token: 0x04004460 RID: 17504
			[SerializeField]
			[HideInInspector]
			private Mesh destinationMesh;

			// Token: 0x04004461 RID: 17505
			[SerializeField]
			[HideInInspector]
			private Mesh destinationColliderMesh;

			// Token: 0x04004462 RID: 17506
			public Spline destinationSpline;

			// Token: 0x04004463 RID: 17507
			public TS_Mesh _editMesh;

			// Token: 0x04004464 RID: 17508
			public TS_Mesh _editColliderMesh;

			// Token: 0x04004465 RID: 17509
			public MeshFilter filter;

			// Token: 0x04004466 RID: 17510
			public MeshCollider collider;

			// Token: 0x04004467 RID: 17511
			public SplineComputer splineComputer;

			// Token: 0x04004468 RID: 17512
			public Vector3[] splinePointPercents = new Vector3[0];

			// Token: 0x04004469 RID: 17513
			public Vector3[] primaryTangentPercents = new Vector3[0];

			// Token: 0x0400446A RID: 17514
			public Vector3[] secondaryTangentPercents = new Vector3[0];

			// Token: 0x0400446B RID: 17515
			[SerializeField]
			[HideInInspector]
			private bool parent;
		}
	}
}
