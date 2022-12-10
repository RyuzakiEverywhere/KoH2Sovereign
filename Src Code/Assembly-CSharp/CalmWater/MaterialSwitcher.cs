using System;
using UnityEngine;

namespace CalmWater
{
	// Token: 0x020004E1 RID: 1249
	public class MaterialSwitcher : MonoBehaviour
	{
		// Token: 0x0600420D RID: 16909 RVA: 0x001F7D2B File Offset: 0x001F5F2B
		private void Start()
		{
			this.m = this.WaterPlane.GetComponent<MirrorReflection>();
		}

		// Token: 0x0600420E RID: 16910 RVA: 0x001F7D3E File Offset: 0x001F5F3E
		public void SetDX11Mat()
		{
			this.WaterPlane.material = this.DX11Mat;
			this.m.setMaterial();
		}

		// Token: 0x0600420F RID: 16911 RVA: 0x001F7D5C File Offset: 0x001F5F5C
		public void SetClassicMat()
		{
			this.WaterPlane.material = this.ClassicMat;
			this.m.setMaterial();
		}

		// Token: 0x04002DFE RID: 11774
		public MeshRenderer WaterPlane;

		// Token: 0x04002DFF RID: 11775
		public Material ClassicMat;

		// Token: 0x04002E00 RID: 11776
		public Material DX11Mat;

		// Token: 0x04002E01 RID: 11777
		private MirrorReflection m;
	}
}
