using System;
using UnityEngine;

// Token: 0x02000011 RID: 17
public class LightFlicker : MonoBehaviour
{
	// Token: 0x0600002A RID: 42 RVA: 0x00002F3F File Offset: 0x0000113F
	private void Start()
	{
		this.originalColor = base.GetComponent<Light>().color;
		this.l = base.GetComponent<Light>();
	}

	// Token: 0x0600002B RID: 43 RVA: 0x00002F5E File Offset: 0x0000115E
	private void Update()
	{
		if (this.l == null)
		{
			return;
		}
		this.l.color = this.originalColor * this.EvalWave();
	}

	// Token: 0x0600002C RID: 44 RVA: 0x00002F8C File Offset: 0x0000118C
	private float EvalWave()
	{
		float num = (Time.time + this.phase) * this.frequency;
		num -= Mathf.Floor(num);
		float num2;
		if (this.waveFunction == LightFlicker.WaveFunction.Sin)
		{
			num2 = Mathf.Sin(num * 2f * 3.1415927f);
		}
		else if (this.waveFunction == LightFlicker.WaveFunction.Triange)
		{
			if (num < 0.5f)
			{
				num2 = 4f * num - 1f;
			}
			else
			{
				num2 = -4f * num + 3f;
			}
		}
		else if (this.waveFunction == LightFlicker.WaveFunction.Square)
		{
			if (num < 0.5f)
			{
				num2 = 1f;
			}
			else
			{
				num2 = -1f;
			}
		}
		else if (this.waveFunction == LightFlicker.WaveFunction.Sawtooth)
		{
			num2 = num;
		}
		else if (this.waveFunction == LightFlicker.WaveFunction.InvvertedSawtooth)
		{
			num2 = 1f - num;
		}
		else if (this.waveFunction == LightFlicker.WaveFunction.Noise)
		{
			num2 = 1f - Random.value * 2f;
		}
		else
		{
			num2 = 1f;
		}
		return num2 * this.amplitude + this.baseValue;
	}

	// Token: 0x0400003E RID: 62
	public LightFlicker.WaveFunction waveFunction;

	// Token: 0x0400003F RID: 63
	public float baseValue;

	// Token: 0x04000040 RID: 64
	public float amplitude = 1f;

	// Token: 0x04000041 RID: 65
	public float phase;

	// Token: 0x04000042 RID: 66
	public float frequency = 0.5f;

	// Token: 0x04000043 RID: 67
	private Light l;

	// Token: 0x04000044 RID: 68
	private Color originalColor;

	// Token: 0x020004EA RID: 1258
	public enum WaveFunction
	{
		// Token: 0x04002E27 RID: 11815
		Sin,
		// Token: 0x04002E28 RID: 11816
		Triange,
		// Token: 0x04002E29 RID: 11817
		Square,
		// Token: 0x04002E2A RID: 11818
		Sawtooth,
		// Token: 0x04002E2B RID: 11819
		InvvertedSawtooth,
		// Token: 0x04002E2C RID: 11820
		Noise
	}
}
