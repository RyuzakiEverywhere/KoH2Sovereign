using System;
using System.Collections;
using UnityEngine;

namespace CrazyMinnow.SALSA.Examples
{
	// Token: 0x02000491 RID: 1169
	public class CM_Ethan_Demo : MonoBehaviour
	{
		// Token: 0x06003DE4 RID: 15844 RVA: 0x001D9B25 File Offset: 0x001D7D25
		private void Start()
		{
			base.StartCoroutine(this.WaitStart(1f));
		}

		// Token: 0x06003DE5 RID: 15845 RVA: 0x001D9B3C File Offset: 0x001D7D3C
		private void Salsa_OnTalkStatusChanged(SalsaStatus status)
		{
			if (this.salsaEvents)
			{
				Debug.Log(string.Concat(new object[]
				{
					"Salsa_OnTalkStatusChanged: instance(",
					status.instance.GetType(),
					"), talkerName(",
					status.talkerName,
					"),",
					status.isTalking ? "started" : "finished",
					" saying ",
					status.clipName
				}));
			}
			if (status.clipName == this.clips[0].name && status.isTalking)
			{
				base.StartCoroutine(this.Look(0f, 2f, this.lookTargets[0]));
				base.StartCoroutine(this.Look(5f, 2f, this.lookTargets[1]));
			}
			if (status.clipName == this.clips[0].name && !status.isTalking)
			{
				this.salsa.SetAudioClip(this.clips[1]);
				this.salsa.Play();
			}
			if (status.clipName == this.clips[1].name && status.isTalking)
			{
				base.StartCoroutine(this.Look(0f, 3f, this.lookTargets[2]));
			}
			if (status.clipName == this.clips[1].name && !status.isTalking)
			{
				this.salsa.SetAudioClip(this.clips[2]);
				this.salsa.Play();
			}
			if (status.clipName == this.clips[2].name && status.isTalking)
			{
				base.StartCoroutine(this.Look(6f, 5f, this.lookTargets[0]));
			}
			if (status.clipName == this.clips[2].name && !status.isTalking)
			{
				base.StartCoroutine(this.Look(0f, 2.5f, this.lookTargets[0]));
				this.randomEyes.SetCustomShapeRandom(false);
				this.randomEyes.SetCustomShapeOverride("brows_inner_up", 2f);
				this.randomEyes.SetCustomShapeOverride("smile", 2f);
			}
		}

		// Token: 0x06003DE6 RID: 15846 RVA: 0x001D9D94 File Offset: 0x001D7F94
		private void RandomEyes_OnLookStatusChanged(RandomEyesLookStatus status)
		{
			if (this.randomEyesLookEvents)
			{
				Debug.Log(string.Concat(new object[]
				{
					"RandomEyes_OnLookStatusChanged: instance(",
					status.instance.GetType(),
					"), name(",
					status.instance.name,
					"), blendSpeed(",
					status.blendSpeed,
					"), rangeOfMotion(",
					status.rangeOfMotion,
					")"
				}));
			}
		}

		// Token: 0x06003DE7 RID: 15847 RVA: 0x001D9E1C File Offset: 0x001D801C
		private void RandomEyes_OnCustomShapeChanged(RandomEyesCustomShapeStatus status)
		{
			if (this.randomEyesShapeEvents)
			{
				Debug.Log(string.Concat(new object[]
				{
					"RandomEyes_OnCustomShapeChanged: instance(",
					status.instance.GetType(),
					"), name(",
					status.instance.name,
					"), shapeIndex(",
					status.shapeIndex,
					"), shapeName(",
					status.shapeName,
					"), overrideOn(",
					status.overrideOn.ToString(),
					"), isOn(",
					status.isOn.ToString(),
					"), blendSpeed(",
					status.blendSpeed,
					"), rangeOfMotion(",
					status.rangeOfMotion,
					")"
				}));
			}
		}

		// Token: 0x06003DE8 RID: 15848 RVA: 0x001D9F00 File Offset: 0x001D8100
		private IEnumerator Look(float preDelay, float duration, GameObject lookTarget)
		{
			yield return new WaitForSeconds(preDelay);
			this.randomEyes.SetLookTarget(lookTarget);
			yield return new WaitForSeconds(duration);
			this.randomEyes.SetLookTarget(null);
			yield break;
		}

		// Token: 0x06003DE9 RID: 15849 RVA: 0x001D9F24 File Offset: 0x001D8124
		private IEnumerator WaitStart(float duration)
		{
			yield return new WaitForSeconds(duration);
			this.salsa.SetAudioClip(this.clips[0]);
			this.salsa.Play();
			yield break;
		}

		// Token: 0x04002BDC RID: 11228
		public Salsa3D salsa;

		// Token: 0x04002BDD RID: 11229
		public RandomEyes3D randomEyes;

		// Token: 0x04002BDE RID: 11230
		public GameObject[] lookTargets;

		// Token: 0x04002BDF RID: 11231
		public AudioClip[] clips;

		// Token: 0x04002BE0 RID: 11232
		public bool salsaEvents;

		// Token: 0x04002BE1 RID: 11233
		public bool randomEyesLookEvents;

		// Token: 0x04002BE2 RID: 11234
		public bool randomEyesShapeEvents;
	}
}
