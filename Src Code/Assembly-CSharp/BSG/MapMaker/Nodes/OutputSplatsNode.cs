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
	// Token: 0x0200039D RID: 925
	[Node.CreateNodeMenuAttribute("BSG Nodes/Splats/OutputSplats")]
	[Serializable]
	public class OutputSplatsNode : Node, IOutputNode
	{
		// Token: 0x06003526 RID: 13606 RVA: 0x000448AF File Offset: 0x00042AAF
		public override object GetValue(NodePort port)
		{
			return null;
		}

		// Token: 0x06003527 RID: 13607 RVA: 0x001AA3A8 File Offset: 0x001A85A8
		public unsafe override void Build()
		{
			Terrain tgt_terrain = base.mm.tgt_terrain;
			TerrainData terrainData = (tgt_terrain != null) ? tgt_terrain.terrainData : null;
			if (terrainData == null)
			{
				return;
			}
			Stopwatch stopwatch = Stopwatch.StartNew();
			this.splats = base.GetInputValue<GetSplatCB>("splats", default(GetSplatCB));
			if (this.splats.data == null)
			{
				return;
			}
			long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
			stopwatch.Restart();
			int alphamapResolution = terrainData.alphamapResolution;
			int alphamapLayers = terrainData.alphamapLayers;
			float[,,] array = new float[alphamapResolution, alphamapResolution, alphamapLayers];
			fixed (float* ptr = &array[0, 0, 0])
			{
				float* psplats = ptr;
				OutputSplatsNode.BuildJob jobData = default(OutputSplatsNode.BuildJob);
				jobData.resolution = alphamapResolution;
				jobData.tile_size = new float2(terrainData.size.x, terrainData.size.z) / (float)alphamapResolution;
				jobData.layers = alphamapLayers;
				jobData.psplats = psplats;
				jobData.cb = this.splats;
				if (this.SingleThreaded)
				{
					jobData.Run(alphamapResolution);
				}
				else
				{
					jobData.Schedule(alphamapResolution, 16, default(JobHandle)).Complete();
				}
			}
			long elapsedMilliseconds2 = stopwatch.ElapsedMilliseconds;
			stopwatch.Restart();
			terrainData.SetAlphamaps(0, 0, array);
			long elapsedMilliseconds3 = stopwatch.ElapsedMilliseconds;
			Debug.Log(string.Format("Splats initted for {0}ms, calculated for {1}ms, applied for {2}ms", elapsedMilliseconds, elapsedMilliseconds2, elapsedMilliseconds3));
		}

		// Token: 0x06003528 RID: 13608 RVA: 0x001AA510 File Offset: 0x001A8710
		public int2 GetResolution(NodePort port)
		{
			Terrain tgt_terrain = base.mm.tgt_terrain;
			TerrainData terrainData = (tgt_terrain != null) ? tgt_terrain.terrainData : null;
			if (terrainData != null)
			{
				return terrainData.alphamapResolution;
			}
			return 0;
		}

		// Token: 0x04002456 RID: 9302
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetSplatCB splats;

		// Token: 0x04002457 RID: 9303
		public bool SingleThreaded;

		// Token: 0x020008DF RID: 2271
		[BurstCompile(CompileSynchronously = true)]
		private struct BuildJob : IJobParallelFor
		{
			// Token: 0x0600521A RID: 21018 RVA: 0x0023FB90 File Offset: 0x0023DD90
			public unsafe void Execute(int y)
			{
				float wy = (float)y * this.tile_size.y;
				for (int i = 0; i < this.resolution; i++)
				{
					float wx = (float)i * this.tile_size.x;
					float* ptr = this.psplats + y * this.resolution * this.layers + i * this.layers;
					float num = 0f;
					this.cb.GetTile(wx, wy, ptr, this.layers);
					for (int j = 0; j < this.layers; j++)
					{
						float num2 = ptr[j];
						num += num2;
					}
					if (num != 1f)
					{
						if (num == 0f)
						{
							*ptr = 1f;
						}
						else
						{
							float num3 = 1f / num;
							for (int k = 0; k < this.layers; k++)
							{
								ptr[k] *= num3;
							}
						}
					}
				}
			}

			// Token: 0x04004152 RID: 16722
			public int resolution;

			// Token: 0x04004153 RID: 16723
			public float2 tile_size;

			// Token: 0x04004154 RID: 16724
			public int layers;

			// Token: 0x04004155 RID: 16725
			[NativeDisableUnsafePtrRestriction]
			public unsafe float* psplats;

			// Token: 0x04004156 RID: 16726
			public GetSplatCB cb;
		}
	}
}
