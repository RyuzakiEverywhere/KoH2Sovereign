using System;
using UnityEngine;

namespace TFHC_ForceShield_Shader_Sample
{
	// Token: 0x020004E6 RID: 1254
	public class ForceShieldImpactDetection : MonoBehaviour
	{
		// Token: 0x0600421D RID: 16925 RVA: 0x001F7ECF File Offset: 0x001F60CF
		private void Start()
		{
			this.mat = base.GetComponent<Renderer>().material;
		}

		// Token: 0x0600421E RID: 16926 RVA: 0x001F7EE4 File Offset: 0x001F60E4
		private void Update()
		{
			if (this.hitTime > 0f)
			{
				this.hitTime -= Time.deltaTime * 1000f;
				if (this.hitTime < 0f)
				{
					this.hitTime = 0f;
				}
				this.mat.SetFloat("_HitTime", this.hitTime);
			}
		}

		// Token: 0x0600421F RID: 16927 RVA: 0x001F7F44 File Offset: 0x001F6144
		private void OnCollisionEnter(Collision collision)
		{
			foreach (ContactPoint contactPoint in collision.contacts)
			{
				this.mat.SetVector("_HitPosition", base.transform.InverseTransformPoint(contactPoint.point));
				this.hitTime = 500f;
				this.mat.SetFloat("_HitTime", this.hitTime);
			}
		}

		// Token: 0x04002E08 RID: 11784
		private float hitTime;

		// Token: 0x04002E09 RID: 11785
		private Material mat;
	}
}
