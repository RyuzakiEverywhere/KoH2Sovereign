using System;
using Unity.Mathematics;
using XNode;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x02000397 RID: 919
	[Node.CreateNodeMenuAttribute("BSG Nodes/Details/Normalize details")]
	[Serializable]
	public class NormalizeDetailsNode : Node
	{
		// Token: 0x06003506 RID: 13574 RVA: 0x001A9658 File Offset: 0x001A7858
		public override object GetValue(NodePort port)
		{
			if (!this.isDataFilled)
			{
				this.FillData();
			}
			return new GetDetailsCB
			{
				GetDetailDensitiesFunction = new GetDetailsCB.GetDetailDensitiesForCoord(this.NormalizeDensities)
			};
		}

		// Token: 0x06003507 RID: 13575 RVA: 0x001A4C74 File Offset: 0x001A2E74
		public override void Initialize()
		{
			base.Initialize();
		}

		// Token: 0x06003508 RID: 13576 RVA: 0x001A9694 File Offset: 0x001A7894
		public override void CleanUp()
		{
			base.CleanUp();
			this.isDataFilled = false;
		}

		// Token: 0x06003509 RID: 13577 RVA: 0x001A96A4 File Offset: 0x001A78A4
		public unsafe void NormalizeDensities(float2 worldSpacePos, float* output_detail_densities, int output_detail_densities_length)
		{
			float num = 1E-05f;
			this.input.GetDetailDensitiesFunction(worldSpacePos, output_detail_densities, output_detail_densities_length);
			for (int i = 0; i < output_detail_densities_length; i++)
			{
				num += math.clamp(output_detail_densities[i], 0f, 1f);
			}
			if (num > 1f)
			{
				float num2 = 1f / num;
				for (int j = 0; j < output_detail_densities_length; j++)
				{
					output_detail_densities[j] = math.clamp(output_detail_densities[j], 0f, 1f) * num2;
				}
			}
		}

		// Token: 0x0600350A RID: 13578 RVA: 0x001A972C File Offset: 0x001A792C
		public unsafe void NormalizeDensities2(float2 worldSpacePos, float* output_detail_densities, int output_detail_densities_length)
		{
			float num = 0f;
			this.input.GetDetailDensitiesFunction(worldSpacePos, output_detail_densities, output_detail_densities_length);
			int num2 = 0;
			float num3 = 0f;
			for (int i = 0; i < output_detail_densities_length; i++)
			{
				float num4 = math.clamp(output_detail_densities[i], 0f, 1f);
				if (num4 > num3)
				{
					num3 = num4;
					num2 = i;
				}
				num += num4;
			}
			if (num > 1f)
			{
				output_detail_densities[num2] = output_detail_densities[num2] - (num - 1f);
			}
		}

		// Token: 0x0600350B RID: 13579 RVA: 0x001A97AC File Offset: 0x001A79AC
		private void FillData()
		{
			if (base.GetInputPort("input").IsConnected)
			{
				this.input = base.GetInputValue<GetDetailsCB>("input", default(GetDetailsCB));
			}
			else
			{
				this.input = new GetDetailsCB
				{
					GetDetailDensitiesFunction = new GetDetailsCB.GetDetailDensitiesForCoord(GetDetailsCB.GetEmptyDensities)
				};
			}
			this.isDataFilled = true;
		}

		// Token: 0x04002436 RID: 9270
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetDetailsCB input;

		// Token: 0x04002437 RID: 9271
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetDetailsCB res;

		// Token: 0x04002438 RID: 9272
		private bool isDataFilled;
	}
}
