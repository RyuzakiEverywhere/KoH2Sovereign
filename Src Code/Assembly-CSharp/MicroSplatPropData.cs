using System;
using UnityEngine;

// Token: 0x0200005B RID: 91
public class MicroSplatPropData : ScriptableObject
{
	// Token: 0x06000219 RID: 537 RVA: 0x000200F0 File Offset: 0x0001E2F0
	private void RevisionData()
	{
		if (this.values.Length == 256)
		{
			Color[] array = new Color[512];
			for (int i = 0; i < 16; i++)
			{
				for (int j = 0; j < 16; j++)
				{
					array[j * 32 + i] = this.values[j * 16 + i];
				}
			}
			this.values = array;
		}
	}

	// Token: 0x0600021A RID: 538 RVA: 0x00020155 File Offset: 0x0001E355
	public Color GetValue(int x, int y)
	{
		this.RevisionData();
		return this.values[y * 32 + x];
	}

	// Token: 0x0600021B RID: 539 RVA: 0x0002016E File Offset: 0x0001E36E
	public void SetValue(int x, int y, Color c)
	{
		this.RevisionData();
		this.values[y * 32 + x] = c;
	}

	// Token: 0x0600021C RID: 540 RVA: 0x00020188 File Offset: 0x0001E388
	public void SetValue(int x, int y, int channel, float value)
	{
		this.RevisionData();
		int num = y * 32 + x;
		Color color = this.values[num];
		color[channel] = value;
		this.values[num] = color;
	}

	// Token: 0x0600021D RID: 541 RVA: 0x000201C8 File Offset: 0x0001E3C8
	public Texture2D GetTexture()
	{
		this.RevisionData();
		if (this.tex == null)
		{
			if (Application.platform == RuntimePlatform.Switch)
			{
				this.tex = new Texture2D(32, 16, TextureFormat.RGBAHalf, false, true);
			}
			else
			{
				this.tex = new Texture2D(32, 16, TextureFormat.RGBAFloat, false, true);
			}
			this.tex.hideFlags = HideFlags.HideAndDontSave;
			this.tex.wrapMode = TextureWrapMode.Clamp;
			this.tex.filterMode = FilterMode.Point;
		}
		this.tex.SetPixels(this.values);
		this.tex.Apply();
		return this.tex;
	}

	// Token: 0x0600021E RID: 542 RVA: 0x00020264 File Offset: 0x0001E464
	public Texture2D GetGeoCurve()
	{
		if (this.geoTex == null)
		{
			this.geoTex = new Texture2D(256, 1, TextureFormat.RHalf, false, true);
			this.geoTex.hideFlags = HideFlags.HideAndDontSave;
		}
		for (int i = 0; i < 256; i++)
		{
			float num = this.geoCurve.Evaluate((float)i / 255f);
			this.geoTex.SetPixel(i, 0, new Color(num, num, num, num));
		}
		this.geoTex.Apply();
		return this.geoTex;
	}

	// Token: 0x04000335 RID: 821
	private const int sMaxTextures = 32;

	// Token: 0x04000336 RID: 822
	[HideInInspector]
	public Color[] values = new Color[512];

	// Token: 0x04000337 RID: 823
	private Texture2D tex;

	// Token: 0x04000338 RID: 824
	[HideInInspector]
	public AnimationCurve geoCurve = AnimationCurve.Linear(0f, 0f, 0f, 0f);

	// Token: 0x04000339 RID: 825
	private Texture2D geoTex;
}
