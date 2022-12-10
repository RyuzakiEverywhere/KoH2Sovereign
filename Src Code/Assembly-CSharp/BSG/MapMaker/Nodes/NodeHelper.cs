using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using XNode;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x0200038B RID: 907
	public static class NodeHelper
	{
		// Token: 0x060034AE RID: 13486 RVA: 0x001A5518 File Offset: 0x001A3718
		public static int GetConnectionCount(NodePort nodePort)
		{
			int num = 0;
			foreach (NodePort nodePort2 in nodePort.GetConnections())
			{
				IDebugNode debugNode = nodePort2.node as IDebugNode;
				if (debugNode == null || !debugNode.IsDebugMode() || MapMakerGraph.Debug_Mode)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x060034AF RID: 13487 RVA: 0x001A5588 File Offset: 0x001A3788
		public static int2 GetOutputResolution(NodePort nodePort)
		{
			List<NodePort> graphOutputPorts = NodeHelper.GetGraphOutputPorts(nodePort);
			int2 @int = 0;
			foreach (NodePort nodePort2 in graphOutputPorts)
			{
				IOutputNode outputNode = nodePort2.node as IOutputNode;
				if (outputNode != null)
				{
					int2 resolution = outputNode.GetResolution(nodePort2);
					if (resolution.x > @int.x || resolution.y > @int.y)
					{
						@int = resolution;
					}
				}
			}
			return @int;
		}

		// Token: 0x060034B0 RID: 13488 RVA: 0x001A5618 File Offset: 0x001A3818
		public static List<NodePort> GetGraphOutputPorts(NodePort nodePort)
		{
			List<NodePort> list = new List<NodePort>();
			IEnumerable<NodePort> outputs = nodePort.node.Outputs;
			int num = 0;
			if (outputs != null)
			{
				foreach (NodePort nodePort2 in outputs)
				{
					num++;
					if (nodePort2.ConnectionCount > 0)
					{
						foreach (NodePort nodePort3 in nodePort2.GetConnections())
						{
							foreach (NodePort item in NodeHelper.GetGraphOutputPorts(nodePort3))
							{
								if (!list.Contains(item))
								{
									list.Add(item);
								}
							}
						}
					}
				}
			}
			if (num == 0)
			{
				IDebugNode debugNode = nodePort.node as IDebugNode;
				if (debugNode != null && debugNode.IsDebugMode() && !MapMakerGraph.Debug_Mode)
				{
					return list;
				}
				list.Add(nodePort);
			}
			return list;
		}

		// Token: 0x060034B1 RID: 13489 RVA: 0x001A5740 File Offset: 0x001A3940
		public static List<Node> GetNodesFromInputPort(NodePort inputPort)
		{
			List<Node> list = new List<Node>();
			IEnumerable<NodePort> inputs = inputPort.node.Inputs;
			int num = 0;
			if (inputs != null)
			{
				foreach (NodePort nodePort in inputs)
				{
					if (nodePort.ConnectionCount > 0)
					{
						num++;
						foreach (NodePort inputPort2 in nodePort.GetConnections())
						{
							foreach (Node item in NodeHelper.GetNodesFromInputPort(inputPort2))
							{
								if (!list.Contains(item))
								{
									list.Add(item);
								}
							}
						}
					}
				}
			}
			IDebugNode debugNode = inputPort.node as IDebugNode;
			if (debugNode != null && debugNode.IsDebugMode() && !MapMakerGraph.Debug_Mode)
			{
				return list;
			}
			list.Add(inputPort.node);
			return list;
		}

		// Token: 0x060034B2 RID: 13490 RVA: 0x001A586C File Offset: 0x001A3A6C
		public unsafe static float[,] SampleInput(GetFloat2DCB input, int arraySize, float2 tileSize)
		{
			float[,] array = new float[arraySize, arraySize];
			fixed (float* ptr = &array[0, 0])
			{
				float* poutputs = ptr;
				new SampleFloatsJob
				{
					resolution = arraySize,
					tile_size = tileSize,
					poutputs = poutputs,
					cb = input
				}.Schedule(arraySize, 16, default(JobHandle)).Complete();
			}
			return array;
		}

		// Token: 0x060034B3 RID: 13491 RVA: 0x001A58D4 File Offset: 0x001A3AD4
		public unsafe static float[,,] SampleSplatInput(GetSplatCB input, int arraySize, float2 tileSize, int layers)
		{
			float[,,] array = new float[arraySize, arraySize, layers];
			fixed (float* ptr = &array[0, 0, 0])
			{
				float* poutputs = ptr;
				new NodeHelper.SampleSplatsJob
				{
					resolution = arraySize,
					tile_size = tileSize,
					layers = layers,
					poutputs = poutputs,
					cb = input
				}.Schedule(arraySize, 16, default(JobHandle)).Complete();
			}
			return array;
		}

		// Token: 0x020008C4 RID: 2244
		[BurstCompile(CompileSynchronously = true)]
		private struct SampleSplatsJob : IJobParallelFor
		{
			// Token: 0x060051FE RID: 20990 RVA: 0x0023EEB4 File Offset: 0x0023D0B4
			public unsafe void Execute(int y)
			{
				float wy = (float)y * this.tile_size.y;
				for (int i = 0; i < this.resolution; i++)
				{
					float wx = (float)i * this.tile_size.x;
					float* result = this.poutputs + y * this.resolution * this.layers + i * this.layers;
					this.cb.GetTile(wx, wy, result, this.layers);
				}
			}

			// Token: 0x040040D7 RID: 16599
			public int resolution;

			// Token: 0x040040D8 RID: 16600
			public float2 tile_size;

			// Token: 0x040040D9 RID: 16601
			public int layers;

			// Token: 0x040040DA RID: 16602
			[NativeDisableUnsafePtrRestriction]
			public unsafe float* poutputs;

			// Token: 0x040040DB RID: 16603
			public GetSplatCB cb;
		}
	}
}
