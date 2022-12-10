using System;
using UnityEngine;

namespace Dreamteck.Splines
{
	// Token: 0x020004B5 RID: 1205
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	[AddComponentMenu("Dreamteck/Splines/Users/Path Generator")]
	public class PathGenerator : MeshGenerator
	{
		// Token: 0x17000460 RID: 1120
		// (get) Token: 0x06003F6B RID: 16235 RVA: 0x001E4FFF File Offset: 0x001E31FF
		// (set) Token: 0x06003F6C RID: 16236 RVA: 0x001E5007 File Offset: 0x001E3207
		public int slices
		{
			get
			{
				return this._slices;
			}
			set
			{
				if (value != this._slices)
				{
					if (value < 1)
					{
						value = 1;
					}
					this._slices = value;
					this.Rebuild();
				}
			}
		}

		// Token: 0x17000461 RID: 1121
		// (get) Token: 0x06003F6D RID: 16237 RVA: 0x001E5026 File Offset: 0x001E3226
		// (set) Token: 0x06003F6E RID: 16238 RVA: 0x001E5030 File Offset: 0x001E3230
		public bool useShapeCurve
		{
			get
			{
				return this._useShapeCurve;
			}
			set
			{
				if (value != this._useShapeCurve)
				{
					this._useShapeCurve = value;
					if (this._useShapeCurve)
					{
						this._shape = new AnimationCurve();
						this._shape.AddKey(new Keyframe(0f, 0f));
						this._shape.AddKey(new Keyframe(1f, 0f));
					}
					else
					{
						this._shape = null;
					}
					this.Rebuild();
				}
			}
		}

		// Token: 0x17000462 RID: 1122
		// (get) Token: 0x06003F6F RID: 16239 RVA: 0x001E50A5 File Offset: 0x001E32A5
		// (set) Token: 0x06003F70 RID: 16240 RVA: 0x001E50AD File Offset: 0x001E32AD
		public float shapeExposure
		{
			get
			{
				return this._shapeExposure;
			}
			set
			{
				if (base.spline != null && value != this._shapeExposure)
				{
					this._shapeExposure = value;
					this.Rebuild();
				}
			}
		}

		// Token: 0x17000463 RID: 1123
		// (get) Token: 0x06003F71 RID: 16241 RVA: 0x001E50D3 File Offset: 0x001E32D3
		// (set) Token: 0x06003F72 RID: 16242 RVA: 0x001E50DC File Offset: 0x001E32DC
		public AnimationCurve shape
		{
			get
			{
				return this._shape;
			}
			set
			{
				if (this._lastShape == null)
				{
					this._lastShape = new AnimationCurve();
				}
				bool flag = false;
				if (value.keys.Length != this._lastShape.keys.Length)
				{
					flag = true;
				}
				else
				{
					for (int i = 0; i < value.keys.Length; i++)
					{
						if (value.keys[i].inTangent != this._lastShape.keys[i].inTangent || value.keys[i].outTangent != this._lastShape.keys[i].outTangent || value.keys[i].time != this._lastShape.keys[i].time || value.keys[i].value != value.keys[i].value)
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					this.Rebuild();
				}
				this._lastShape.keys = new Keyframe[value.keys.Length];
				value.keys.CopyTo(this._lastShape.keys, 0);
				this._lastShape.preWrapMode = value.preWrapMode;
				this._lastShape.postWrapMode = value.postWrapMode;
				this._shape = value;
			}
		}

		// Token: 0x06003F73 RID: 16243 RVA: 0x001E5239 File Offset: 0x001E3439
		protected override void Awake()
		{
			base.Awake();
			this.mesh.name = "path";
		}

		// Token: 0x06003F74 RID: 16244 RVA: 0x001E5251 File Offset: 0x001E3451
		protected override void Reset()
		{
			base.Reset();
		}

		// Token: 0x06003F75 RID: 16245 RVA: 0x001E5259 File Offset: 0x001E3459
		protected override void BuildMesh()
		{
			if (base.sampleCount == 0)
			{
				return;
			}
			base.BuildMesh();
			this.GenerateVertices();
			MeshUtility.GeneratePlaneTriangles(ref this.tsMesh.triangles, this._slices, base.sampleCount, false, 0, 0, false);
		}

		// Token: 0x06003F76 RID: 16246 RVA: 0x001E5294 File Offset: 0x001E3494
		private void GenerateVertices()
		{
			int vertexCount = (this._slices + 1) * base.sampleCount;
			this.AllocateMesh(vertexCount, this._slices * (base.sampleCount - 1) * 6);
			int num = 0;
			base.ResetUVDistance();
			bool flag = base.offset != Vector3.zero;
			for (int i = 0; i < base.sampleCount; i++)
			{
				base.GetSample(i, this.evalResult);
				Vector3 a = Vector3.zero;
				try
				{
					a = this.evalResult.position;
				}
				catch (Exception ex)
				{
					Debug.Log(ex.Message + " for i = " + i);
					break;
				}
				Vector3 right = this.evalResult.right;
				if (flag)
				{
					a += base.offset.x * right + base.offset.y * this.evalResult.up + base.offset.z * this.evalResult.forward;
				}
				float d = base.size * this.evalResult.size;
				Vector3 b = Vector3.zero;
				Quaternion rotation = Quaternion.AngleAxis(base.rotation, this.evalResult.forward);
				if (base.uvMode == MeshGenerator.UVMode.UniformClamp || base.uvMode == MeshGenerator.UVMode.UniformClip)
				{
					base.AddUVDistance(i);
				}
				Color color = this.evalResult.color * base.color;
				for (int j = 0; j < this._slices + 1; j++)
				{
					float num2 = (float)j / (float)this._slices;
					float d2 = 0f;
					if (this._useShapeCurve)
					{
						d2 = this._shape.Evaluate(num2);
					}
					this.tsMesh.vertices[num] = a + rotation * right * d * 0.5f - rotation * right * d * num2 + rotation * this.evalResult.up * d2 * this._shapeExposure;
					base.CalculateUVs(this.evalResult.percent, 1f - num2);
					this.tsMesh.uv[num] = Vector2.one * 0.5f + Quaternion.AngleAxis(base.uvRotation, Vector3.forward) * (Vector2.one * 0.5f - MeshGenerator.uvs);
					if (this._slices > 1)
					{
						if (j < this._slices)
						{
							float num3 = (float)(j + 1) / (float)this._slices;
							d2 = 0f;
							if (this._useShapeCurve)
							{
								d2 = this._shape.Evaluate(num3);
							}
							Vector3 a2 = a + rotation * right * d * 0.5f - rotation * right * d * num3 + rotation * this.evalResult.up * d2 * this._shapeExposure;
							Vector3 vector = -Vector3.Cross(this.evalResult.forward, a2 - this.tsMesh.vertices[num]).normalized;
							if (j > 0)
							{
								Vector3 b2 = -Vector3.Cross(this.evalResult.forward, this.tsMesh.vertices[num] - b).normalized;
								this.tsMesh.normals[num] = Vector3.Slerp(vector, b2, 0.5f);
							}
							else
							{
								this.tsMesh.normals[num] = vector;
							}
						}
						else
						{
							this.tsMesh.normals[num] = -Vector3.Cross(this.evalResult.forward, this.tsMesh.vertices[num] - b).normalized;
						}
					}
					else
					{
						this.tsMesh.normals[num] = this.evalResult.up;
						if (base.rotation != 0f)
						{
							this.tsMesh.normals[num] = rotation * this.tsMesh.normals[num];
						}
					}
					this.tsMesh.colors[num] = color;
					b = this.tsMesh.vertices[num];
					num++;
				}
			}
		}

		// Token: 0x04002CD7 RID: 11479
		[SerializeField]
		[HideInInspector]
		private int _slices = 1;

		// Token: 0x04002CD8 RID: 11480
		[SerializeField]
		[HideInInspector]
		private bool _useShapeCurve;

		// Token: 0x04002CD9 RID: 11481
		[SerializeField]
		[HideInInspector]
		private AnimationCurve _shape;

		// Token: 0x04002CDA RID: 11482
		[SerializeField]
		[HideInInspector]
		private AnimationCurve _lastShape;

		// Token: 0x04002CDB RID: 11483
		[SerializeField]
		[HideInInspector]
		private float _shapeExposure = 1f;
	}
}
