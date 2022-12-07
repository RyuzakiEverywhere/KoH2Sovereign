using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000013 RID: 19
public class Lightning : MonoBehaviour
{
	// Token: 0x0600002F RID: 47 RVA: 0x00003098 File Offset: 0x00001298
	private void Start()
	{
		base.StartCoroutine(this.Light());
	}

	// Token: 0x06000030 RID: 48 RVA: 0x000030A7 File Offset: 0x000012A7
	private IEnumerator Light()
	{
		for (;;)
		{
			yield return new WaitForSeconds(Random.Range(this.offMin, this.offMax));
			this.LightningBolt.SetActive(true);
			this.LightningBolt.transform.Rotate(0f, (float)Random.Range(1, 360), 0f);
			base.StartCoroutine(this.Soundfx());
			yield return new WaitForSeconds(Random.Range(this.onMin, this.onMax));
			this.LightningBolt.SetActive(false);
		}
		yield break;
	}

	// Token: 0x06000031 RID: 49 RVA: 0x000030B6 File Offset: 0x000012B6
	private IEnumerator Soundfx()
	{
		this.ThunderRND = (float)Random.Range(1, 5);
		this.ThunderVol = Random.Range(0.2f, 1f);
		this.ThunderWait = 9f - this.ThunderVol * 3f * 3f - 2f;
		while (this.ThunderRND == 1f)
		{
			yield return new WaitForSeconds(this.ThunderWait);
			this.ThunderAudioA.volume = this.ThunderVol;
			this.ThunderAudioA.Play();
			this.ThunderRND = 0f;
		}
		while (this.ThunderRND == 2f)
		{
			yield return new WaitForSeconds(this.ThunderWait);
			this.ThunderAudioB.volume = this.ThunderVol;
			this.ThunderAudioB.Play();
			this.ThunderRND = 0f;
		}
		while (this.ThunderRND == 3f)
		{
			yield return new WaitForSeconds(this.ThunderWait);
			this.ThunderAudioC.volume = this.ThunderVol;
			this.ThunderAudioC.Play();
			this.ThunderRND = 0f;
		}
		while (this.ThunderRND == 4f)
		{
			yield return new WaitForSeconds(this.ThunderWait);
			this.ThunderAudioD.volume = this.ThunderVol;
			this.ThunderAudioD.Play();
			this.ThunderRND = 0f;
		}
		yield break;
	}

	// Token: 0x0400004C RID: 76
	public float offMin = 10f;

	// Token: 0x0400004D RID: 77
	public float offMax = 60f;

	// Token: 0x0400004E RID: 78
	public AudioSource ThunderAudioA;

	// Token: 0x0400004F RID: 79
	public AudioSource ThunderAudioB;

	// Token: 0x04000050 RID: 80
	public AudioSource ThunderAudioC;

	// Token: 0x04000051 RID: 81
	public AudioSource ThunderAudioD;

	// Token: 0x04000052 RID: 82
	public GameObject LightningBolt;

	// Token: 0x04000053 RID: 83
	private float onMin = 0.5f;

	// Token: 0x04000054 RID: 84
	private float onMax = 1.25f;

	// Token: 0x04000055 RID: 85
	private float ThunderRND = 1f;

	// Token: 0x04000056 RID: 86
	private float ThunderVol;

	// Token: 0x04000057 RID: 87
	private float ThunderWait;
}
