using System;
using XNode;

namespace BSG.MapMaker
{
	// Token: 0x0200037A RID: 890
	[Node.CreateNodeMenuAttribute("")]
	[Serializable]
	public class Node : Node
	{
		// Token: 0x170002C8 RID: 712
		// (get) Token: 0x06003440 RID: 13376 RVA: 0x001A29BE File Offset: 0x001A0BBE
		public MapMakerGraph mm
		{
			get
			{
				return (MapMakerGraph)this.graph;
			}
		}

		// Token: 0x06003441 RID: 13377 RVA: 0x000023FD File Offset: 0x000005FD
		public virtual void Initialize()
		{
		}

		// Token: 0x06003442 RID: 13378 RVA: 0x000023FD File Offset: 0x000005FD
		public virtual void PreCache()
		{
		}

		// Token: 0x06003443 RID: 13379 RVA: 0x000023FD File Offset: 0x000005FD
		public virtual void Build()
		{
		}

		// Token: 0x06003444 RID: 13380 RVA: 0x000023FD File Offset: 0x000005FD
		public virtual void CleanUp()
		{
		}
	}
}
