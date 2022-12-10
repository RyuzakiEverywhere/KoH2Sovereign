using System;
using System.Collections;
using UnityEngine;

namespace CrazyMinnow.SALSA
{
	// Token: 0x0200048C RID: 1164
	public class CM_RuntimeSetupExample : MonoBehaviour
	{
		// Token: 0x06003DD6 RID: 15830 RVA: 0x001D9303 File Offset: 0x001D7503
		private void Start()
		{
			base.StartCoroutine(this.RemoveComponents());
		}

		// Token: 0x06003DD7 RID: 15831 RVA: 0x001D9314 File Offset: 0x001D7514
		private void Update()
		{
			if (this.loadSalsaAndRandomEyes && !this.salsa3D && !this.re3D)
			{
				base.gameObject.AddComponent<Salsa3D>();
				this.salsa3D = base.GetComponent<Salsa3D>();
				this.salsa3D.skinnedMeshRenderer = base.GetComponent<SkinnedMeshRenderer>();
				this.salsa3D.saySmallIndex = 0;
				this.salsa3D.sayMediumIndex = 1;
				this.salsa3D.sayLargeIndex = 2;
				this.salsa3D.SetAudioClip(this.audioClip);
				this.salsa3D.saySmallTrigger = 0.002f;
				this.salsa3D.sayMediumTrigger = 0.004f;
				this.salsa3D.sayLargeTrigger = 0.006f;
				this.salsa3D.audioUpdateDelay = 0.08f;
				this.salsa3D.blendSpeed = 10f;
				this.salsa3D.rangeOfMotion = 100f;
				this.salsa3D.broadcast = true;
				this.salsa3D.broadcastReceivers = new GameObject[1];
				this.salsa3D.broadcastReceivers[0] = this.broadcastReciever;
				this.salsa3D.expandBroadcast = true;
				this.salsa3D.Play();
				base.gameObject.AddComponent<RandomEyes3D>();
				this.re3D = base.GetComponent<RandomEyes3D>();
				this.re3D.skinnedMeshRenderer = base.GetComponent<SkinnedMeshRenderer>();
				this.re3D.lookUpIndex = 3;
				this.re3D.lookDownIndex = 4;
				this.re3D.lookLeftIndex = 5;
				this.re3D.lookRightIndex = 6;
				this.re3D.blinkIndex = 7;
				this.re3D.rangeOfMotion = 100f;
				this.re3D.blendSpeed = 10f;
				this.re3D.blinkDuration = 0.05f;
				this.re3D.blinkSpeed = 20f;
				this.re3D.SetOpenMax(0f);
				this.re3D.SetCloseMax(100f);
				this.re3D.SetRandomEyes(true);
				this.re3D.SetBlink(true);
				this.re3D.AutoLinkCustomShapes(true, this.salsa3D);
				this.re3D.expandCustomShapes = true;
				this.re3D.broadcastCS = true;
				this.re3D.broadcastCSReceivers = new GameObject[1];
				this.re3D.broadcastCSReceivers[0] = GameObject.Find("Broadcasts");
				this.re3D.expandBroadcastCS = true;
			}
		}

		// Token: 0x06003DD8 RID: 15832 RVA: 0x001D958C File Offset: 0x001D778C
		private IEnumerator RemoveComponents()
		{
			for (;;)
			{
				if (!this.loadSalsaAndRandomEyes && this.salsa3D && this.re3D)
				{
					Object.DestroyImmediate(this.salsa3D);
					Object.DestroyImmediate(base.GetComponent<AudioSource>());
					Object.DestroyImmediate(this.re3D);
				}
				yield return new WaitForSeconds(0.1f);
			}
			yield break;
		}

		// Token: 0x04002BC2 RID: 11202
		public AudioClip audioClip;

		// Token: 0x04002BC3 RID: 11203
		public GameObject broadcastReciever;

		// Token: 0x04002BC4 RID: 11204
		public bool loadSalsaAndRandomEyes;

		// Token: 0x04002BC5 RID: 11205
		private Salsa3D salsa3D;

		// Token: 0x04002BC6 RID: 11206
		private RandomEyes3D re3D;
	}
}
