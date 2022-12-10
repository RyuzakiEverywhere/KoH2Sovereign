using System;
using System.Diagnostics;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x0200039F RID: 927
	[Node.CreateNodeMenuAttribute("BSG Nodes/Texture/OutputTexture")]
	[Serializable]
	public class OutputTextureNode : Node, IOutputNode, IDebugNode
	{
		// Token: 0x06003535 RID: 13621 RVA: 0x000448AF File Offset: 0x00042AAF
		public override object GetValue(NodePort port)
		{
			return null;
		}

		// Token: 0x06003536 RID: 13622 RVA: 0x001AB0A4 File Offset: 0x001A92A4
		public unsafe override void Build()
		{
			if (this.debugNode && !MapMakerGraph.Debug_Mode)
			{
				return;
			}
			if (string.IsNullOrEmpty(this.SavePath))
			{
				Debug.LogWarning("No path given to OutputTextureNode - ignored");
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
			Texture2D texture2D = new Texture2D(num, num);
			if (texture2D == null)
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
				OutputTextureNode.BuildJob buildJob = default(OutputTextureNode.BuildJob);
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
			texture2D.SetPixels(array);
			texture2D.Apply();
			Common.SaveTexture(texture2D, this.SavePath, true);
			long elapsedMilliseconds3 = stopwatch.ElapsedMilliseconds;
			Debug.Log(string.Format("Texture initted for {0}ms, calculated for {1}ms, applied for {2}ms", elapsedMilliseconds, elapsedMilliseconds2, elapsedMilliseconds3));
		}

		// Token: 0x06003537 RID: 13623 RVA: 0x001AB3BC File Offset: 0x001A95BC
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

		// Token: 0x06003538 RID: 13624 RVA: 0x001AB40D File Offset: 0x001A960D
		public bool IsDebugMode()
		{
			return this.debugNode;
		}

		// Token: 0x04002463 RID: 9315
		private const float DEFAULT_RGB = 0f;

		// Token: 0x04002464 RID: 9316
		public const float DEFAULT_A = 1f;

		// Token: 0x04002465 RID: 9317
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetFloat2DCB rgb;

		// Token: 0x04002466 RID: 9318
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetFloat2DCB r;

		// Token: 0x04002467 RID: 9319
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetFloat2DCB g;

		// Token: 0x04002468 RID: 9320
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetFloat2DCB b;

		// Token: 0x04002469 RID: 9321
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetFloat2DCB a;

		// Token: 0x0400246A RID: 9322
		public int size;

		// Token: 0x0400246B RID: 9323
		public string SavePath;

		// Token: 0x0400246C RID: 9324
		public bool SingleThreaded;

		// Token: 0x0400246D RID: 9325
		[Space]
		public bool debugNode = true;

		// Token: 0x020008E7 RID: 2279
		[BurstCompile(CompileSynchronously = true)]
		private struct BuildJob : IJobParallelFor
		{
			// Token: 0x06005223 RID: 21027 RVA: 0x00240018 File Offset: 0x0023E218
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

			// Token: 0x04004173 RID: 16755
			public int resolution;

			// Token: 0x04004174 RID: 16756
			public float2 tile_size;

			// Token: 0x04004175 RID: 16757
			[NativeDisableUnsafePtrRestriction]
			public unsafe Color* prgba;

			// Token: 0x04004176 RID: 16758
			public GetFloat2DCB rgbcb;

			// Token: 0x04004177 RID: 16759
			public GetFloat2DCB rcb;

			// Token: 0x04004178 RID: 16760
			public GetFloat2DCB gcb;

			// Token: 0x04004179 RID: 16761
			public GetFloat2DCB bcb;

			// Token: 0x0400417A RID: 16762
			public GetFloat2DCB acb;

			// Token: 0x0400417B RID: 16763
			public bool ignoreRGB;

			// Token: 0x0400417C RID: 16764
			public bool ignoreR;

			// Token: 0x0400417D RID: 16765
			public bool ignoreG;

			// Token: 0x0400417E RID: 16766
			public bool ignoreB;

			// Token: 0x0400417F RID: 16767
			public bool ignoreA;
		}
	}
}
