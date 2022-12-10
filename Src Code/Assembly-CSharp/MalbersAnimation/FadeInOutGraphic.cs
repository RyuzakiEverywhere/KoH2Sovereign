using System;
using UnityEngine;
using UnityEngine.UI;

namespace MalbersAnimations
{
	// Token: 0x02000414 RID: 1044
	public class FadeInOutGraphic : MonoBehaviour
	{
		// Token: 0x06003884 RID: 14468 RVA: 0x001BC0FE File Offset: 0x001BA2FE
		public virtual void Fade_In_Out(GameObject fade)
		{
			this.Fade_In_Out(fade != null);
		}

		// Token: 0x06003885 RID: 14469 RVA: 0x001BC10D File Offset: 0x001BA30D
		public virtual void Fade_In_Out(bool fade)
		{
			this.graphic.CrossFadeColor(fade ? this.FadeIn : this.FadeOut, this.time, false, true);
		}

		// Token: 0x06003886 RID: 14470 RVA: 0x001BC133 File Offset: 0x001BA333
		public virtual void Fade_In(float time)
		{
			this.graphic.CrossFadeColor(this.FadeIn, time, false, true);
		}

		// Token: 0x06003887 RID: 14471 RVA: 0x001BC149 File Offset: 0x001BA349
		public virtual void Fade_Out(float time)
		{
			this.graphic.CrossFadeColor(this.FadeOut, time, false, true);
		}

		// Token: 0x040028E2 RID: 10466
		public Graphic graphic;

		// Token: 0x040028E3 RID: 10467
		public Color FadeIn;

		// Token: 0x040028E4 RID: 10468
		public Color FadeOut;

		// Token: 0x040028E5 RID: 10469
		public float time = 0.25f;
	}
}
