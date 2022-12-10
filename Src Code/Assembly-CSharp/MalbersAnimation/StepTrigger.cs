using System;
using System.Collections;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x0200040C RID: 1036
	public class StepTrigger : MonoBehaviour
	{
		// Token: 0x1700035D RID: 861
		// (get) Token: 0x06003864 RID: 14436 RVA: 0x001BBA10 File Offset: 0x001B9C10
		// (set) Token: 0x06003865 RID: 14437 RVA: 0x001BBA18 File Offset: 0x001B9C18
		public bool HasTrack
		{
			get
			{
				return this.hastrack;
			}
			set
			{
				this.hastrack = value;
			}
		}

		// Token: 0x06003866 RID: 14438 RVA: 0x001BBA24 File Offset: 0x001B9C24
		private void Awake()
		{
			this._StepsManager = base.GetComponentInParent<StepsManager>();
			if (this._StepsManager == null)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			if (!this._StepsManager.Active)
			{
				base.gameObject.SetActive(false);
				return;
			}
			this.StepAudio = base.GetComponent<AudioSource>();
			if (this.StepAudio == null)
			{
				this.StepAudio = base.gameObject.AddComponent<AudioSource>();
			}
			this.StepAudio.spatialBlend = 1f;
			if (this._StepsManager)
			{
				this.StepAudio.volume = this._StepsManager.StepsVolume;
			}
			this.wait = new WaitForSeconds(this.WaitNextStep);
		}

		// Token: 0x06003867 RID: 14439 RVA: 0x001BBAE0 File Offset: 0x001B9CE0
		private void OnTriggerEnter(Collider other)
		{
			if (!this.waitrack && this._StepsManager)
			{
				base.StartCoroutine(this.WaitForStep());
				this._StepsManager.EnterStep(this);
				this.hastrack = true;
			}
		}

		// Token: 0x06003868 RID: 14440 RVA: 0x001BBB17 File Offset: 0x001B9D17
		private void OnTriggerExit(Collider other)
		{
			this.hastrack = false;
		}

		// Token: 0x06003869 RID: 14441 RVA: 0x001BBB20 File Offset: 0x001B9D20
		private IEnumerator WaitForStep()
		{
			this.waitrack = true;
			yield return this.wait;
			this.waitrack = false;
			yield break;
		}

		// Token: 0x040028AC RID: 10412
		private StepsManager _StepsManager;

		// Token: 0x040028AD RID: 10413
		public float WaitNextStep = 0.2f;

		// Token: 0x040028AE RID: 10414
		[HideInInspector]
		public AudioSource StepAudio;

		// Token: 0x040028AF RID: 10415
		private WaitForSeconds wait;

		// Token: 0x040028B0 RID: 10416
		private bool hastrack;

		// Token: 0x040028B1 RID: 10417
		private bool waitrack;
	}
}
