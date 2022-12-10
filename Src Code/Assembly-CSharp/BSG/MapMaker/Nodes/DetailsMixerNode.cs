using System;
using Unity.Burst;
using Unity.Mathematics;
using XNode;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x02000384 RID: 900
	[BurstCompile(CompileSynchronously = true)]
	[Node.CreateNodeMenuAttribute("BSG Nodes/Details/DetailsMixer")]
	[Serializable]
	public class DetailsMixerNode : Node
	{
		// Token: 0x06003490 RID: 13456 RVA: 0x001A4C38 File Offset: 0x001A2E38
		public override object GetValue(NodePort port)
		{
			if (!this.isDataFilled)
			{
				this.FillData();
			}
			return new GetDetailsCB
			{
				GetDetailDensitiesFunction = new GetDetailsCB.GetDetailDensitiesForCoord(this.GetDetailDensities)
			};
		}

		// Token: 0x06003491 RID: 13457 RVA: 0x001A4C74 File Offset: 0x001A2E74
		public override void Initialize()
		{
			base.Initialize();
		}

		// Token: 0x06003492 RID: 13458 RVA: 0x001A4C7C File Offset: 0x001A2E7C
		public override void CleanUp()
		{
			base.CleanUp();
			this.GetMask = null;
			this.isDataFilled = false;
		}

		// Token: 0x06003493 RID: 13459 RVA: 0x001A4C94 File Offset: 0x001A2E94
		public unsafe void GetDetailDensities(float2 worldSpacePos, float* output_detail_densities, int output_detail_densities_length)
		{
			float* ptr;
			float* ptr2;
			float s;
			checked
			{
				ptr = stackalloc float[unchecked((UIntPtr)output_detail_densities_length) * 4];
				ptr2 = stackalloc float[unchecked((UIntPtr)output_detail_densities_length) * 4];
				this.in1.GetDetailDensitiesFunction(worldSpacePos, ptr, output_detail_densities_length);
				this.in2.GetDetailDensitiesFunction(worldSpacePos, ptr2, output_detail_densities_length);
				s = this.GetMask(worldSpacePos.x, worldSpacePos.y, this.mask.data);
			}
			for (int i = 0; i < output_detail_densities_length; i++)
			{
				output_detail_densities[i] = math.lerp(ptr[i], ptr2[i], s);
			}
		}

		// Token: 0x06003494 RID: 13460 RVA: 0x001A4D20 File Offset: 0x001A2F20
		private void FillData()
		{
			if (base.GetInputPort("in1").IsConnected)
			{
				this.in1 = base.GetInputValue<GetDetailsCB>("in1", default(GetDetailsCB));
			}
			else
			{
				this.in1 = new GetDetailsCB
				{
					GetDetailDensitiesFunction = new GetDetailsCB.GetDetailDensitiesForCoord(GetDetailsCB.GetEmptyDensities)
				};
			}
			if (base.GetInputPort("in2").IsConnected)
			{
				this.in2 = base.GetInputValue<GetDetailsCB>("in2", default(GetDetailsCB));
			}
			else
			{
				this.in2 = new GetDetailsCB
				{
					GetDetailDensitiesFunction = new GetDetailsCB.GetDetailDensitiesForCoord(GetDetailsCB.GetEmptyDensities)
				};
			}
			this.mask = base.GetInputValue<GetFloat2DCB>("mask", default(GetFloat2DCB));
			this.GetMask = new FunctionPointer<GetFloat2DCB.GetValueFunc>(this.mask.get_value_func).Invoke;
			this.isDataFilled = true;
		}

		// Token: 0x04002383 RID: 9091
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetDetailsCB in1;

		// Token: 0x04002384 RID: 9092
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetDetailsCB in2;

		// Token: 0x04002385 RID: 9093
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetFloat2DCB mask;

		// Token: 0x04002386 RID: 9094
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetDetailsCB res;

		// Token: 0x04002387 RID: 9095
		private bool isDataFilled;

		// Token: 0x04002388 RID: 9096
		private GetFloat2DCB.GetValueFunc GetMask;
	}
}
