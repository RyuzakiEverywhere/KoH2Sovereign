using System;
using UnityEngine;

namespace Dreamteck.Splines
{
	// Token: 0x020004CA RID: 1226
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	[AddComponentMenu("Dreamteck/Splines/Users/Waveform Generator")]
	public class WaveformGenerator : MeshGenerator
	{
		// Token: 0x170004A9 RID: 1193
		// (get) Token: 0x0600411B RID: 16667 RVA: 0x001EF02D File Offset: 0x001ED22D
		// (set) Token: 0x0600411C RID: 16668 RVA: 0x001EF035 File Offset: 0x001ED235
		public WaveformGenerator.Axis axis
		{
			get
			{
				return this._axis;
			}
			set
			{
				if (value != this._axis)
				{
					this._axis = value;
					this.Rebuild();
				}
			}
		}

		// Token: 0x170004AA RID: 1194
		// (get) Token: 0x0600411D RID: 16669 RVA: 0x001EF04D File Offset: 0x001ED24D
		// (set) Token: 0x0600411E RID: 16670 RVA: 0x001EF055 File Offset: 0x001ED255
		public bool symmetry
		{
			get
			{
				return this._symmetry;
			}
			set
			{
				if (value != this._symmetry)
				{
					this._symmetry = value;
					this.Rebuild();
				}
			}
		}

		// Token: 0x170004AB RID: 1195
		// (get) Token: 0x0600411F RID: 16671 RVA: 0x001EF06D File Offset: 0x001ED26D
		// (set) Token: 0x06004120 RID: 16672 RVA: 0x001EF075 File Offset: 0x001ED275
		public WaveformGenerator.UVWrapMode uvWrapMode
		{
			get
			{
				return this._uvWrapMode;
			}
			set
			{
				if (value != this._uvWrapMode)
				{
					this._uvWrapMode = value;
					this.Rebuild();
				}
			}
		}

		// Token: 0x170004AC RID: 1196
		// (get) Token: 0x06004121 RID: 16673 RVA: 0x001EF08D File Offset: 0x001ED28D
		// (set) Token: 0x06004122 RID: 16674 RVA: 0x001EF095 File Offset: 0x001ED295
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

		// Token: 0x06004123 RID: 16675 RVA: 0x001EF0B4 File Offset: 0x001ED2B4
		protected override void Awake()
		{
			base.Awake();
			this.mesh.name = "waveform";
		}

		// Token: 0x06004124 RID: 16676 RVA: 0x001EF0CC File Offset: 0x001ED2CC
		protected override void BuildMesh()
		{
			base.BuildMesh();
			this.Generate();
		}

		// Token: 0x06004125 RID: 16677 RVA: 0x001EF0DA File Offset: 0x001ED2DA
		protected override void Build()
		{
			base.Build();
		}

		// Token: 0x06004126 RID: 16678 RVA: 0x001EF0E2 File Offset: 0x001ED2E2
		protected override void LateRun()
		{
			base.LateRun();
		}

		// Token: 0x06004127 RID: 16679 RVA: 0x001EF0EC File Offset: 0x001ED2EC
		private void Generate()
		{
			int vertexCount = base.sampleCount * (this._slices + 1);
			this.AllocateMesh(vertexCount, this._slices * (base.sampleCount - 1) * 6);
			int num = 0;
			float num2 = 0f;
			float num3 = 0f;
			Vector3 position = base.spline.position;
			Vector3 vector = base.spline.TransformDirection(Vector3.right);
			WaveformGenerator.Axis axis = this._axis;
			if (axis != WaveformGenerator.Axis.Y)
			{
				if (axis == WaveformGenerator.Axis.Z)
				{
					vector = base.spline.TransformDirection(Vector3.forward);
				}
			}
			else
			{
				vector = base.spline.TransformDirection(Vector3.up);
			}
			for (int i = 0; i < base.sampleCount; i++)
			{
				Vector3 position2 = base.GetSampleRaw(i).position;
				Vector3 vector2 = base.spline.InverseTransformPoint(position2);
				Vector3 vector3 = vector2;
				Vector3 forward = base.GetSampleRaw(i).forward;
				Vector3 up = base.GetSampleRaw(i).up;
				float num4 = 1f;
				if ((this._uvWrapMode == WaveformGenerator.UVWrapMode.UniformX || this._uvWrapMode == WaveformGenerator.UVWrapMode.Uniform) && i > 0)
				{
					num3 += Vector3.Distance(base.GetSampleRaw(i).position, base.GetSampleRaw(i - 1).position);
				}
				switch (this._axis)
				{
				case WaveformGenerator.Axis.X:
					vector3.x = (this._symmetry ? (-vector2.x) : 0f);
					num4 = base.uvScale.y * Mathf.Abs(vector2.x);
					num2 += vector2.x;
					break;
				case WaveformGenerator.Axis.Y:
					vector3.y = (this._symmetry ? (-vector2.y) : 0f);
					num4 = base.uvScale.y * Mathf.Abs(vector2.y);
					num2 += vector2.y;
					break;
				case WaveformGenerator.Axis.Z:
					vector3.z = (this._symmetry ? (-vector2.z) : 0f);
					num4 = base.uvScale.y * Mathf.Abs(vector2.z);
					num2 += vector2.z;
					break;
				}
				vector3 = base.spline.TransformPoint(vector3);
				Vector3 normalized = Vector3.Cross(vector, forward).normalized;
				Vector3 a = Vector3.Cross(up, forward);
				for (int j = 0; j < this._slices + 1; j++)
				{
					float num5 = (float)j / (float)this._slices;
					this.tsMesh.vertices[num] = Vector3.Lerp(vector3, position2, num5) + vector * base.offset.y + a * base.offset.x;
					this.tsMesh.normals[num] = normalized;
					switch (this._uvWrapMode)
					{
					case WaveformGenerator.UVWrapMode.Clamp:
						this.tsMesh.uv[num] = new Vector2((float)base.GetSampleRaw(i).percent * base.uvScale.x + base.uvOffset.x, num5 * base.uvScale.y + base.uvOffset.y);
						break;
					case WaveformGenerator.UVWrapMode.UniformX:
						this.tsMesh.uv[num] = new Vector2(num3 * base.uvScale.x + base.uvOffset.x, num5 * base.uvScale.y + base.uvOffset.y);
						break;
					case WaveformGenerator.UVWrapMode.UniformY:
						this.tsMesh.uv[num] = new Vector2((float)base.GetSampleRaw(i).percent * base.uvScale.x + base.uvOffset.x, num4 * num5 * base.uvScale.y + base.uvOffset.y);
						break;
					case WaveformGenerator.UVWrapMode.Uniform:
						this.tsMesh.uv[num] = new Vector2(num3 * base.uvScale.x + base.uvOffset.x, num4 * num5 * base.uvScale.y + base.uvOffset.y);
						break;
					}
					this.tsMesh.colors[num] = base.GetSampleRaw(i).color * base.color;
					num++;
				}
			}
			if (base.sampleCount > 0)
			{
				num2 /= (float)base.sampleCount;
			}
			MeshUtility.GeneratePlaneTriangles(ref this.tsMesh.triangles, this._slices, base.sampleCount, num2 < 0f, 0, 0, false);
		}

		// Token: 0x04002D7C RID: 11644
		[SerializeField]
		[HideInInspector]
		private WaveformGenerator.Axis _axis = WaveformGenerator.Axis.Y;

		// Token: 0x04002D7D RID: 11645
		[SerializeField]
		[HideInInspector]
		private bool _symmetry;

		// Token: 0x04002D7E RID: 11646
		[SerializeField]
		[HideInInspector]
		private WaveformGenerator.UVWrapMode _uvWrapMode;

		// Token: 0x04002D7F RID: 11647
		[SerializeField]
		[HideInInspector]
		private int _slices = 1;

		// Token: 0x020009AB RID: 2475
		public enum Axis
		{
			// Token: 0x0400450C RID: 17676
			X,
			// Token: 0x0400450D RID: 17677
			Y,
			// Token: 0x0400450E RID: 17678
			Z
		}

		// Token: 0x020009AC RID: 2476
		public enum Space
		{
			// Token: 0x04004510 RID: 17680
			World,
			// Token: 0x04004511 RID: 17681
			Local
		}

		// Token: 0x020009AD RID: 2477
		public enum UVWrapMode
		{
			// Token: 0x04004513 RID: 17683
			Clamp,
			// Token: 0x04004514 RID: 17684
			UniformX,
			// Token: 0x04004515 RID: 17685
			UniformY,
			// Token: 0x04004516 RID: 17686
			Uniform
		}
	}
}
