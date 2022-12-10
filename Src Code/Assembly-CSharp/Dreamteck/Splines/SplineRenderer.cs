using System;
using UnityEngine;

namespace Dreamteck.Splines
{
	// Token: 0x020004C5 RID: 1221
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	[AddComponentMenu("Dreamteck/Splines/Users/Spline Renderer")]
	[ExecuteInEditMode]
	public class SplineRenderer : MeshGenerator
	{
		// Token: 0x17000489 RID: 1161
		// (get) Token: 0x06004092 RID: 16530 RVA: 0x001EB6EC File Offset: 0x001E98EC
		// (set) Token: 0x06004093 RID: 16531 RVA: 0x001EB6F4 File Offset: 0x001E98F4
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

		// Token: 0x06004094 RID: 16532 RVA: 0x001EB713 File Offset: 0x001E9913
		protected override void Awake()
		{
			base.Awake();
			this.mesh.name = "spline";
		}

		// Token: 0x06004095 RID: 16533 RVA: 0x001EB72B File Offset: 0x001E992B
		private void Start()
		{
			if (Camera.current != null)
			{
				this.orthographic = Camera.current.orthographic;
			}
		}

		// Token: 0x06004096 RID: 16534 RVA: 0x001EB74A File Offset: 0x001E994A
		protected override void LateRun()
		{
			if (this.updateFrameInterval > 0)
			{
				this.currentFrame++;
				if (this.currentFrame > this.updateFrameInterval)
				{
					this.currentFrame = 0;
				}
			}
		}

		// Token: 0x06004097 RID: 16535 RVA: 0x001EB778 File Offset: 0x001E9978
		protected override void BuildMesh()
		{
			base.BuildMesh();
			this.GenerateVertices(this.vertexDirection, this.orthographic);
			MeshUtility.GeneratePlaneTriangles(ref this.tsMesh.triangles, this._slices, base.sampleCount, false, 0, 0, false);
		}

		// Token: 0x06004098 RID: 16536 RVA: 0x001EB7B4 File Offset: 0x001E99B4
		public void RenderWithCamera(Camera cam)
		{
			if (base.sampleCount == 0)
			{
				return;
			}
			this.orthographic = true;
			if (cam != null)
			{
				if (cam.orthographic)
				{
					this.vertexDirection = -cam.transform.forward;
				}
				else
				{
					this.vertexDirection = cam.transform.position;
				}
				this.orthographic = cam.orthographic;
			}
			this.BuildMesh();
			this.WriteMesh();
		}

		// Token: 0x06004099 RID: 16537 RVA: 0x001EB824 File Offset: 0x001E9A24
		private void OnWillRenderObject()
		{
			if (!this.autoOrient)
			{
				return;
			}
			if (this.updateFrameInterval > 0 && this.currentFrame != 0)
			{
				return;
			}
			if (!Application.isPlaying && !this.init)
			{
				this.Awake();
				this.init = true;
			}
			this.RenderWithCamera(Camera.current);
		}

		// Token: 0x0600409A RID: 16538 RVA: 0x001EB874 File Offset: 0x001E9A74
		public void GenerateVertices(Vector3 vertexDirection, bool orthoGraphic)
		{
			this.AllocateMesh((this._slices + 1) * base.sampleCount, this._slices * (base.sampleCount - 1) * 6);
			int num = 0;
			base.ResetUVDistance();
			bool flag = base.offset != Vector3.zero;
			for (int i = 0; i < base.sampleCount; i++)
			{
				base.GetSample(i, this.evalResult);
				Vector3 vector = this.evalResult.position;
				if (flag)
				{
					vector += base.offset.x * -Vector3.Cross(this.evalResult.forward, this.evalResult.up) + base.offset.y * this.evalResult.up + base.offset.z * this.evalResult.forward;
				}
				Vector3 vector2;
				if (orthoGraphic)
				{
					vector2 = vertexDirection;
				}
				else
				{
					vector2 = (vertexDirection - vector).normalized;
				}
				Vector3 normalized = Vector3.Cross(this.evalResult.forward, vector2).normalized;
				if (base.uvMode == MeshGenerator.UVMode.UniformClamp || base.uvMode == MeshGenerator.UVMode.UniformClip)
				{
					base.AddUVDistance(i);
				}
				Color color = this.evalResult.color * base.color;
				for (int j = 0; j < this._slices + 1; j++)
				{
					float num2 = (float)j / (float)this._slices;
					this.tsMesh.vertices[num] = vector - normalized * this.evalResult.size * 0.5f * base.size + normalized * this.evalResult.size * num2 * base.size;
					base.CalculateUVs(this.evalResult.percent, num2);
					this.tsMesh.uv[num] = Vector2.one * 0.5f + Quaternion.AngleAxis(base.uvRotation, Vector3.forward) * (Vector2.one * 0.5f - MeshGenerator.uvs);
					this.tsMesh.normals[num] = vector2;
					this.tsMesh.colors[num] = color;
					num++;
				}
			}
		}

		// Token: 0x04002D35 RID: 11573
		[HideInInspector]
		public bool autoOrient = true;

		// Token: 0x04002D36 RID: 11574
		[HideInInspector]
		public int updateFrameInterval;

		// Token: 0x04002D37 RID: 11575
		private int currentFrame;

		// Token: 0x04002D38 RID: 11576
		[SerializeField]
		[HideInInspector]
		private int _slices = 1;

		// Token: 0x04002D39 RID: 11577
		[SerializeField]
		[HideInInspector]
		private Vector3 vertexDirection = Vector3.up;

		// Token: 0x04002D3A RID: 11578
		private bool orthographic;

		// Token: 0x04002D3B RID: 11579
		private bool init;
	}
}
