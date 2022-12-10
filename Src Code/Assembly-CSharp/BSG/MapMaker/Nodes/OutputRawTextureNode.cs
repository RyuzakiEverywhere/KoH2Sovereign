using System;
using System.Diagnostics;
using System.IO;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x0200039C RID: 924
	[Node.CreateNodeMenuAttribute("BSG Nodes/Texture/OutputRawTexture")]
	[Serializable]
	public class OutputRawTextureNode : Node, IOutputNode
	{
		// Token: 0x06003522 RID: 13602 RVA: 0x000448AF File Offset: 0x00042AAF
		public override object GetValue(NodePort port)
		{
			return null;
		}

		// Token: 0x06003523 RID: 13603 RVA: 0x001AA18C File Offset: 0x001A838C
		public unsafe override void Build()
		{
			if (string.IsNullOrEmpty(this.SavePath))
			{
				Debug.LogWarning("No path given to OutputTextureNode - ignored");
				return;
			}
			int num = this.size;
			if (this.size == 0)
			{
				num = base.mm.tgt_terrain.terrainData.heightmapResolution - 1;
			}
			Stopwatch stopwatch = Stopwatch.StartNew();
			this.raw = base.GetInputValue<GetFloat2DCB>("raw", default(GetFloat2DCB));
			if (this.raw.data == null)
			{
				return;
			}
			long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
			stopwatch.Restart();
			ushort[] array = new ushort[num * num];
			fixed (ushort* ptr = &array[0])
			{
				ushort* praw = ptr;
				OutputRawTextureNode.BuildJob jobData = default(OutputRawTextureNode.BuildJob);
				jobData.resolution = num;
				jobData.tile_size = new float2((float)num, (float)num) / (float)num;
				jobData.praw = praw;
				jobData.cb = this.raw;
				if (this.SingleThreaded)
				{
					jobData.Run(num);
				}
				else
				{
					jobData.Schedule(num, 16, default(JobHandle)).Complete();
				}
			}
			long elapsedMilliseconds2 = stopwatch.ElapsedMilliseconds;
			stopwatch.Restart();
			using (FileStream fileStream = File.OpenWrite(this.SavePath))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
				{
					for (int i = 0; i < num; i++)
					{
						for (int j = 0; j < num; j++)
						{
							binaryWriter.Write(array[i * num + j]);
						}
					}
				}
			}
			long elapsedMilliseconds3 = stopwatch.ElapsedMilliseconds;
			Debug.Log(string.Format("Texture initted for {0}ms, calculated for {1}ms, applied for {2}ms", elapsedMilliseconds, elapsedMilliseconds2, elapsedMilliseconds3));
		}

		// Token: 0x06003524 RID: 13604 RVA: 0x001AA354 File Offset: 0x001A8554
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

		// Token: 0x04002452 RID: 9298
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetFloat2DCB raw;

		// Token: 0x04002453 RID: 9299
		public int size;

		// Token: 0x04002454 RID: 9300
		public string SavePath;

		// Token: 0x04002455 RID: 9301
		public bool SingleThreaded;

		// Token: 0x020008DE RID: 2270
		[BurstCompile(CompileSynchronously = true)]
		private struct BuildJob : IJobParallelFor
		{
			// Token: 0x06005219 RID: 21017 RVA: 0x0023FB28 File Offset: 0x0023DD28
			public unsafe void Execute(int y)
			{
				float wy = (float)y * this.tile_size.y;
				for (int i = 0; i < this.resolution; i++)
				{
					float wx = (float)i * this.tile_size.x;
					ushort num = (ushort)(this.cb.GetValue(wx, wy) * 65535f);
					this.praw[y * this.resolution + i] = num;
				}
			}

			// Token: 0x0400414E RID: 16718
			public int resolution;

			// Token: 0x0400414F RID: 16719
			public float2 tile_size;

			// Token: 0x04004150 RID: 16720
			[NativeDisableUnsafePtrRestriction]
			public unsafe ushort* praw;

			// Token: 0x04004151 RID: 16721
			public GetFloat2DCB cb;
		}
	}
}
