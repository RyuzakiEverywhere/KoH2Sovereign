using System;

namespace BSG.MapMaker
{
	// Token: 0x02000378 RID: 888
	[Serializable]
	public struct GetTerrainCB
	{
		// Token: 0x0400232F RID: 9007
		public GetFloat2DCB heights;

		// Token: 0x04002330 RID: 9008
		public GetSplatCB splats;

		// Token: 0x04002331 RID: 9009
		public GetTreesCB trees;

		// Token: 0x04002332 RID: 9010
		public ObjectsPackage objects;
	}
}
