using System;
using UnityEngine;
using UnityEngine.UI;

namespace MalbersAnimations
{
	// Token: 0x02000410 RID: 1040
	public class UIFollowTransform : MonoBehaviour
	{
		// Token: 0x1700035E RID: 862
		// (get) Token: 0x06003870 RID: 14448 RVA: 0x001BBF39 File Offset: 0x001BA139
		private Graphic Graph
		{
			get
			{
				if (this.graphic == null)
				{
					this.graphic = base.GetComponent<Graphic>();
				}
				return this.graphic;
			}
		}

		// Token: 0x06003871 RID: 14449 RVA: 0x001BBF5B File Offset: 0x001BA15B
		private void OnEnable()
		{
			this.Aling();
		}

		// Token: 0x06003872 RID: 14450 RVA: 0x001BBF63 File Offset: 0x001BA163
		public void SetTransform(Transform newTarget)
		{
			this.WorldTransform = newTarget;
		}

		// Token: 0x06003873 RID: 14451 RVA: 0x001BBF5B File Offset: 0x001BA15B
		private void Start()
		{
			this.Aling();
		}

		// Token: 0x06003874 RID: 14452 RVA: 0x001BBF6C File Offset: 0x001BA16C
		private void Awake()
		{
			this.main = Camera.main;
			this.graphic = base.GetComponent<Graphic>();
		}

		// Token: 0x06003875 RID: 14453 RVA: 0x001BBF5B File Offset: 0x001BA15B
		private void Update()
		{
			this.Aling();
		}

		// Token: 0x06003876 RID: 14454 RVA: 0x001BBF85 File Offset: 0x001BA185
		public void Aling()
		{
			if (!this.main || !this.WorldTransform)
			{
				return;
			}
			base.transform.position = this.main.WorldToScreenPoint(this.WorldTransform.position);
		}

		// Token: 0x06003877 RID: 14455 RVA: 0x001BBFC3 File Offset: 0x001BA1C3
		public virtual void Fade_In_Out(bool value)
		{
			this.Graph.CrossFadeColor(value ? this.FadeIn : this.FadeOut, this.time, false, true);
		}

		// Token: 0x06003878 RID: 14456 RVA: 0x001BBFE9 File Offset: 0x001BA1E9
		public virtual void Fade_In(float time)
		{
			this.graphic.CrossFadeColor(this.FadeIn, time, false, true);
		}

		// Token: 0x06003879 RID: 14457 RVA: 0x001BBFFF File Offset: 0x001BA1FF
		public virtual void Fade_Out(float time)
		{
			this.graphic.CrossFadeColor(this.FadeOut, time, false, true);
		}

		// Token: 0x040028D6 RID: 10454
		private Camera main;

		// Token: 0x040028D7 RID: 10455
		public Transform WorldTransform;

		// Token: 0x040028D8 RID: 10456
		public Color FadeOut;

		// Token: 0x040028D9 RID: 10457
		public Color FadeIn;

		// Token: 0x040028DA RID: 10458
		public float time = 0.3f;

		// Token: 0x040028DB RID: 10459
		private Graphic graphic;
	}
}
