using System;
using UnityEngine;

namespace TFHC_ForceShield_Shader_Sample
{
	// Token: 0x020004E5 RID: 1253
	public class ForceShieldDestroyBall : MonoBehaviour
	{
		// Token: 0x0600421B RID: 16923 RVA: 0x001F7EA9 File Offset: 0x001F60A9
		private void Start()
		{
			Object.Destroy(base.gameObject, this.lifetime);
		}

		// Token: 0x04002E07 RID: 11783
		public float lifetime = 5f;
	}
}
