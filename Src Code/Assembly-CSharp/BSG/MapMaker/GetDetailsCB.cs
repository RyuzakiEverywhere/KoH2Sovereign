using System;
using Unity.Mathematics;

namespace BSG.MapMaker
{
	// Token: 0x02000375 RID: 885
	[Serializable]
	public struct GetDetailsCB
	{
		// Token: 0x06003439 RID: 13369 RVA: 0x001A27D8 File Offset: 0x001A09D8
		public unsafe static void GetEmptyDensities(float2 worldSpacePos, float* output_prototype_densities, int output_prototype_densities_length)
		{
			for (int i = 0; i < output_prototype_densities_length; i++)
			{
				output_prototype_densities[i] = 0f;
			}
		}

		// Token: 0x0400232A RID: 9002
		public GetDetailsCB.GetDetailDensitiesForCoord GetDetailDensitiesFunction;

		// Token: 0x020008B5 RID: 2229
		// (Invoke) Token: 0x060051E2 RID: 20962
		public unsafe delegate void GetDetailDensitiesForCoord(float2 worldSpacePos, float* output_prototype_densities, int output_prototype_densities_length);
	}
}
