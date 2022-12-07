using System;
using UnityEngine;

// Token: 0x02000142 RID: 322
public class RndAnim : MonoBehaviour
{
	// Token: 0x06001129 RID: 4393 RVA: 0x000B6008 File Offset: 0x000B4208
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		if (this.attachment != null)
		{
			this.particles = this.attachment.GetComponent<ParticleSystem>();
			if (this.particles != null)
			{
				this.particles.Stop();
			}
			this.light = this.attachment.GetComponentInChildren<Light>();
			if (this.light != null)
			{
				this.light.gameObject.SetActive(false);
			}
			this.sound = this.attachment.GetComponentInChildren<AudioSource>();
			if (this.sound != null)
			{
				this.sound.Stop();
				this.sound.gameObject.SetActive(false);
			}
		}
		this.trigger_time = Time.time + Random.Range(this.minInterval, this.maxInterval);
	}

	// Token: 0x0600112A RID: 4394 RVA: 0x000B60E4 File Offset: 0x000B42E4
	private void Update()
	{
		float time = Time.time;
		if (time < this.trigger_time)
		{
			return;
		}
		if (this.integer != "")
		{
			this.animator.SetInteger(this.integer, Random.Range(this.minInt, this.maxInt));
		}
		if (this.trigger != "")
		{
			this.animator.SetTrigger(this.trigger);
		}
		this.trigger_time = time + Random.Range(this.minInterval, this.maxInterval);
	}

	// Token: 0x0600112B RID: 4395 RVA: 0x000B6174 File Offset: 0x000B4374
	public void Attach()
	{
		if (this.attachment != null)
		{
			this.attachment.SetActive(true);
		}
		if (this.sound != null)
		{
			this.sound.gameObject.SetActive(true);
			this.sound.Play();
		}
		if (this.particles != null)
		{
			this.particles.Play();
		}
		if (this.light != null)
		{
			this.light.gameObject.SetActive(true);
		}
	}

	// Token: 0x0600112C RID: 4396 RVA: 0x000B6200 File Offset: 0x000B4400
	public void Detach()
	{
		if (this.light != null)
		{
			this.light.gameObject.SetActive(false);
		}
		if (this.sound != null && this.sound.loop)
		{
			this.sound.Stop();
			this.sound.gameObject.SetActive(false);
		}
		if (this.particles != null)
		{
			this.particles.Stop();
			return;
		}
		if (this.attachment != null)
		{
			this.attachment.SetActive(false);
		}
	}

	// Token: 0x04000B5F RID: 2911
	public float minInterval = 5f;

	// Token: 0x04000B60 RID: 2912
	public float maxInterval = 10f;

	// Token: 0x04000B61 RID: 2913
	public string trigger = "";

	// Token: 0x04000B62 RID: 2914
	public string integer = "";

	// Token: 0x04000B63 RID: 2915
	public int minInt;

	// Token: 0x04000B64 RID: 2916
	public int maxInt = 3;

	// Token: 0x04000B65 RID: 2917
	public GameObject attachment;

	// Token: 0x04000B66 RID: 2918
	private Animator animator;

	// Token: 0x04000B67 RID: 2919
	private ParticleSystem particles;

	// Token: 0x04000B68 RID: 2920
	private Light light;

	// Token: 0x04000B69 RID: 2921
	private AudioSource sound;

	// Token: 0x04000B6A RID: 2922
	private float trigger_time;

	// Token: 0x04000B6B RID: 2923
	private float end_time;
}
