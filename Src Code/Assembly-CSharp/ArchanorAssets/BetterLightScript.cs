using System;
using UnityEngine;

namespace ArchanorAssets
{
	// Token: 0x020004E3 RID: 1251
	public class BetterLightScript : MonoBehaviour
	{
		// Token: 0x06004213 RID: 16915 RVA: 0x001F7D84 File Offset: 0x001F5F84
		private void Start()
		{
			if (base.gameObject.GetComponent<Light>())
			{
				this.li = base.gameObject.GetComponent<Light>();
				this.initIntensity = this.li.intensity;
				return;
			}
			MonoBehaviour.print("No light object found on " + base.gameObject.name);
		}

		// Token: 0x06004214 RID: 16916 RVA: 0x001F7DE0 File Offset: 0x001F5FE0
		private void Update()
		{
			if (base.gameObject.GetComponent<Light>())
			{
				this.li.intensity -= this.initIntensity * (Time.deltaTime / this.life);
				if (this.killAfterLife && this.li.intensity <= 0f)
				{
					Object.Destroy(base.gameObject);
				}
			}
		}

		// Token: 0x04002E02 RID: 11778
		[Header("Seconds to dim the light")]
		public float life = 0.2f;

		// Token: 0x04002E03 RID: 11779
		public bool killAfterLife = true;

		// Token: 0x04002E04 RID: 11780
		private Light li;

		// Token: 0x04002E05 RID: 11781
		private float initIntensity;
	}
}
