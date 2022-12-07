using System;

// Token: 0x020001FF RID: 511
public class Sampler
{
	// Token: 0x06001F38 RID: 7992 RVA: 0x00121166 File Offset: 0x0011F366
	public Sampler(int size)
	{
		this.samples = new float[size];
	}

	// Token: 0x06001F39 RID: 7993 RVA: 0x0012117C File Offset: 0x0011F37C
	private void RecalcMinMax()
	{
		this.min = float.MaxValue;
		this.max = float.MinValue;
		for (int i = 0; i < this.samples.Length; i++)
		{
			float num = this.samples[i];
			if (num < this.min)
			{
				this.min = num;
			}
			if (num > this.max)
			{
				this.max = num;
			}
		}
	}

	// Token: 0x06001F3A RID: 7994 RVA: 0x001211DC File Offset: 0x0011F3DC
	public void Add(float sample)
	{
		float num = this.samples[0];
		this.sum += sample - num;
		for (int i = 1; i < this.samples.Length; i++)
		{
			this.samples[i - 1] = this.samples[i];
		}
		this.samples[this.samples.Length - 1] = sample;
		this.avg = this.sum / (float)this.samples.Length;
		if (num == this.min || num == this.max)
		{
			this.RecalcMinMax();
			return;
		}
		if (sample < this.min)
		{
			this.min = sample;
		}
		if (sample > this.max)
		{
			this.max = sample;
		}
	}

	// Token: 0x06001F3B RID: 7995 RVA: 0x00121288 File Offset: 0x0011F488
	public void Add(int sample)
	{
		this.Add((float)sample);
	}

	// Token: 0x06001F3C RID: 7996 RVA: 0x00121292 File Offset: 0x0011F492
	public static string ToStr(float val)
	{
		return val.ToString("F1");
	}

	// Token: 0x06001F3D RID: 7997 RVA: 0x001212A0 File Offset: 0x0011F4A0
	public override string ToString()
	{
		return string.Concat(new string[]
		{
			Sampler.ToStr(this.avg),
			" (",
			Sampler.ToStr(this.min),
			" - ",
			Sampler.ToStr(this.max),
			")"
		});
	}

	// Token: 0x040014B0 RID: 5296
	public float[] samples;

	// Token: 0x040014B1 RID: 5297
	public float min;

	// Token: 0x040014B2 RID: 5298
	public float avg;

	// Token: 0x040014B3 RID: 5299
	public float max;

	// Token: 0x040014B4 RID: 5300
	private float sum;
}
