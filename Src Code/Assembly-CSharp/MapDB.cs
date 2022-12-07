using System;

// Token: 0x02000337 RID: 823
public class MapDB
{
	// Token: 0x040021DC RID: 8668
	public MapDB.Node[] db;

	// Token: 0x02000889 RID: 2185
	[Flags]
	public enum Features
	{
		// Token: 0x04003FE9 RID: 16361
		None = 0,
		// Token: 0x04003FEA RID: 16362
		LowForest = 2,
		// Token: 0x04003FEB RID: 16363
		HighForest = 4,
		// Token: 0x04003FEC RID: 16364
		MixedForest = 0,
		// Token: 0x04003FED RID: 16365
		HasStructures = 8
	}

	// Token: 0x0200088A RID: 2186
	public struct Node
	{
		// Token: 0x17000669 RID: 1641
		// (get) Token: 0x06005179 RID: 20857 RVA: 0x0023DAD2 File Offset: 0x0023BCD2
		public bool LowForest
		{
			get
			{
				return (this.info & MapDB.Features.LowForest) > MapDB.Features.None;
			}
		}

		// Token: 0x1700066A RID: 1642
		// (get) Token: 0x0600517A RID: 20858 RVA: 0x0023DADF File Offset: 0x0023BCDF
		public bool HighForest
		{
			get
			{
				return (this.info & MapDB.Features.HighForest) > MapDB.Features.None;
			}
		}

		// Token: 0x1700066B RID: 1643
		// (get) Token: 0x0600517B RID: 20859 RVA: 0x0023DAEC File Offset: 0x0023BCEC
		public bool MixedForest
		{
			get
			{
				return this.LowForest && this.HighForest;
			}
		}

		// Token: 0x04003FEE RID: 16366
		public MapDB.Features info;
	}
}
