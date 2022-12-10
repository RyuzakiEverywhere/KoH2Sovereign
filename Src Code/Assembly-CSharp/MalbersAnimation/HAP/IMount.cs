using System;
using UnityEngine;

namespace MalbersAnimations.HAP
{
	// Token: 0x0200042A RID: 1066
	public interface IMount
	{
		// Token: 0x17000397 RID: 919
		// (get) Token: 0x06003944 RID: 14660
		Transform MountPoint { get; }

		// Token: 0x17000398 RID: 920
		// (get) Token: 0x06003945 RID: 14661
		Transform FootLeftIK { get; }

		// Token: 0x17000399 RID: 921
		// (get) Token: 0x06003946 RID: 14662
		Transform FootRightIK { get; }

		// Token: 0x1700039A RID: 922
		// (get) Token: 0x06003947 RID: 14663
		Transform KneeLeftIK { get; }

		// Token: 0x1700039B RID: 923
		// (get) Token: 0x06003948 RID: 14664
		Transform KneeRightIK { get; }

		// Token: 0x1700039C RID: 924
		// (get) Token: 0x06003949 RID: 14665
		Animal Animal { get; }

		// Token: 0x1700039D RID: 925
		// (get) Token: 0x0600394A RID: 14666
		// (set) Token: 0x0600394B RID: 14667
		Rider ActiveRider { get; set; }

		// Token: 0x1700039E RID: 926
		// (get) Token: 0x0600394C RID: 14668
		Transform transform { get; }

		// Token: 0x1700039F RID: 927
		// (get) Token: 0x0600394D RID: 14669
		bool StraightSpine { get; }

		// Token: 0x170003A0 RID: 928
		// (get) Token: 0x0600394E RID: 14670
		Quaternion SpineOffset { get; }

		// Token: 0x170003A1 RID: 929
		// (get) Token: 0x0600394F RID: 14671
		// (set) Token: 0x06003950 RID: 14672
		bool Mounted { get; set; }

		// Token: 0x170003A2 RID: 930
		// (get) Token: 0x06003951 RID: 14673
		// (set) Token: 0x06003952 RID: 14674
		bool CanBeMounted { get; set; }

		// Token: 0x170003A3 RID: 931
		// (get) Token: 0x06003953 RID: 14675
		bool CanDismount { get; }

		// Token: 0x170003A4 RID: 932
		// (get) Token: 0x06003954 RID: 14676
		// (set) Token: 0x06003955 RID: 14677
		string MountLayer { get; set; }

		// Token: 0x170003A5 RID: 933
		// (get) Token: 0x06003956 RID: 14678
		// (set) Token: 0x06003957 RID: 14679
		string MountIdle { get; set; }

		// Token: 0x06003958 RID: 14680
		void EnableControls(bool value);
	}
}
