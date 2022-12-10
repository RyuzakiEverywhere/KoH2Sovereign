using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x020003B5 RID: 949
	[Node.CreateNodeMenuAttribute("BSG Nodes/Texture/Texture Memory")]
	[Serializable]
	public class TextureMemoryNode : Node, IOutputNode, IDebugNode
	{
		// Token: 0x060035A2 RID: 13730 RVA: 0x000448AF File Offset: 0x00042AAF
		public override object GetValue(NodePort port)
		{
			return null;
		}

		// Token: 0x060035A3 RID: 13731 RVA: 0x001AE4B8 File Offset: 0x001AC6B8
		public unsafe override void Build()
		{
			if (this.debugNode && !MapMakerGraph.Debug_Mode)
			{
				return;
			}
			Terrain tgt_terrain = base.mm.tgt_terrain;
			TerrainData terrainData = (tgt_terrain != null) ? tgt_terrain.terrainData : null;
			if (terrainData == null)
			{
				return;
			}
			int num = this.size;
			if (this.size == 0)
			{
				num = terrainData.heightmapResolution - 1;
			}
			this.tex = base.GetInputValue<Texture2D>("tex", null);
			if (this.tex == null)
			{
				this.tex = new Texture2D(num, num);
				this.tex.filterMode = FilterMode.Bilinear;
				if (this.tex == null)
				{
					return;
				}
				Stopwatch stopwatch = Stopwatch.StartNew();
				this.rgb = base.GetInputValue<GetFloat2DCB>("rgb", default(GetFloat2DCB));
				this.r = base.GetInputValue<GetFloat2DCB>("r", default(GetFloat2DCB));
				this.g = base.GetInputValue<GetFloat2DCB>("g", default(GetFloat2DCB));
				this.b = base.GetInputValue<GetFloat2DCB>("b", default(GetFloat2DCB));
				this.a = base.GetInputValue<GetFloat2DCB>("a", default(GetFloat2DCB));
				if (this.rgb.data == null && this.r.data == null && this.g.data == null && this.b.data == null)
				{
					return;
				}
				long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
				stopwatch.Restart();
				Color[] array = new Color[num * num];
				fixed (Color* ptr = &array[0])
				{
					Color* prgba = ptr;
					TextureMemoryNode.BuildJob buildJob = default(TextureMemoryNode.BuildJob);
					buildJob.resolution = num;
					buildJob.tile_size = new float2(terrainData.size.x, terrainData.size.z) / (float)num;
					buildJob.prgba = prgba;
					buildJob.rgbcb = this.rgb;
					buildJob.rcb = this.r;
					buildJob.gcb = this.g;
					buildJob.bcb = this.b;
					buildJob.acb = this.a;
					if (this.r.data == null)
					{
						buildJob.ignoreR = true;
					}
					if (this.g.data == null)
					{
						buildJob.ignoreG = true;
					}
					if (this.b.data == null)
					{
						buildJob.ignoreB = true;
					}
					if (this.a.data == null)
					{
						buildJob.ignoreA = true;
					}
					buildJob.ignoreRGB = (this.rgb.data == null || !buildJob.ignoreR || !buildJob.ignoreG || !buildJob.ignoreB || !buildJob.ignoreA);
					if (this.SingleThreaded)
					{
						buildJob.Run(num);
					}
					else
					{
						buildJob.Schedule(num, 16, default(JobHandle)).Complete();
					}
				}
				long elapsedMilliseconds2 = stopwatch.ElapsedMilliseconds;
				stopwatch.Restart();
				this.tex.SetPixels(array);
				this.tex.Apply();
				long elapsedMilliseconds3 = stopwatch.ElapsedMilliseconds;
				Debug.Log(string.Format("Texture initted for {0}ms, calculated for {1}ms, applied for {2}ms", elapsedMilliseconds, elapsedMilliseconds2, elapsedMilliseconds3));
			}
			if (base.mm.generated_textures == null)
			{
				base.mm.generated_textures = new Dictionary<string, Texture2D>();
			}
			base.mm.generated_textures[this.key] = this.tex;
		}

		// Token: 0x060035A4 RID: 13732 RVA: 0x001AE824 File Offset: 0x001ACA24
		public int2 GetResolution(NodePort port)
		{
			int2 result = this.size;
			if (this.size == 0)
			{
				Terrain tgt_terrain = base.mm.tgt_terrain;
				TerrainData terrainData = (tgt_terrain != null) ? tgt_terrain.terrainData : null;
				if (terrainData != null)
				{
					result = terrainData.heightmapResolution - 1;
				}
			}
			return result;
		}

		// Token: 0x060035A5 RID: 13733 RVA: 0x001AE875 File Offset: 0x001ACA75
		public bool IsDebugMode()
		{
			return this.debugNode;
		}

		// Token: 0x0400251B RID: 9499
		private const float DEFAULT_RGB = 0f;

		// Token: 0x0400251C RID: 9500
		public const float DEFAULT_A = 1f;

		// Token: 0x0400251D RID: 9501
		public string key;

		// Token: 0x0400251E RID: 9502
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public Texture2D tex;

		// Token: 0x0400251F RID: 9503
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetFloat2DCB rgb;

		// Token: 0x04002520 RID: 9504
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetFloat2DCB r;

		// Token: 0x04002521 RID: 9505
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetFloat2DCB g;

		// Token: 0x04002522 RID: 9506
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetFloat2DCB b;

		// Token: 0x04002523 RID: 9507
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetFloat2DCB a;

		// Token: 0x04002524 RID: 9508
		public int size;

		// Token: 0x04002525 RID: 9509
		public bool SingleThreaded;

		// Token: 0x04002526 RID: 9510
		[Space]
		public bool debugNode = true;

		// Token: 0x020008FA RID: 2298
		[BurstCompile(CompileSynchronously = true)]
		private struct BuildJob : IJobParallelFor
		{
			// Token: 0x0600522C RID: 21036 RVA: 0x002405AC File Offset: 0x0023E7AC
			public unsafe void Execute(int y)
			{
				float wy = (float)y * this.tile_size.y;
				for (int i = 0; i < this.resolution; i++)
				{
					float wx = (float)i * this.tile_size.x;
					if (this.ignoreRGB)
					{
						float r = 0f;
						if (!this.ignoreR)
						{
							r = this.rcb.GetValue(wx, wy);
						}
						float g = 0f;
						if (!this.ignoreG)
						{
							g = this.gcb.GetValue(wx, wy);
						}
						float b = 0f;
						if (!this.ignoreB)
						{
							b = this.bcb.GetValue(wx, wy);
						}
						float a = 1f;
						if (!this.ignoreA)
						{
							a = this.acb.GetValue(wx, wy);
						}
						this.prgba[y * this.resolution + i] = new Color(r, g, b, a);
					}
					else
					{
						float value = this.rgbcb.GetValue(wx, wy);
						float a2 = 1f;
						this.prgba[y * this.resolution + i] = new Color(value, value, value, a2);
					}
				}
			}

			// Token: 0x040041D0 RID: 16848
			public int resolution;

			// Token: 0x040041D1 RID: 16849
			public float2 tile_size;

			// Token: 0x040041D2 RID: 16850
			[NativeDisableUnsafePtrRestriction]
			public unsafe Color* prgba;

			// Token: 0x040041D3 RID: 16851
			public GetFloat2DCB rgbcb;

			// Token: 0x040041D4 RID: 16852
			public GetFloat2DCB rcb;

			// Token: 0x040041D5 RID: 16853
			public GetFloat2DCB gcb;

			// Token: 0x040041D6 RID: 16854
			public GetFloat2DCB bcb;

			// Token: 0x040041D7 RID: 16855
			public GetFloat2DCB acb;

			// Token: 0x040041D8 RID: 16856
			public bool ignoreRGB;

			// Token: 0x040041D9 RID: 16857
			public bool ignoreR;

			// Token: 0x040041DA RID: 16858
			public bool ignoreG;

			// Token: 0x040041DB RID: 16859
			public bool ignoreB;

			// Token: 0x040041DC RID: 16860
			public bool ignoreA;
		}
	}
}
