using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000321 RID: 801
public class ResampleTest : MonoBehaviour
{
	// Token: 0x06003202 RID: 12802 RVA: 0x00195718 File Offset: 0x00193918
	public void Do()
	{
		this.result = ResampleTest.ResizeTexture(this.original, this.mode, this.scale);
		this.sourceSprite.sprite = Sprite.Create(this.original, new Rect(0f, 0f, (float)this.original.width, (float)this.original.height), new Vector2(0.5f, 0.5f));
		this.resultSprite.sprite = Sprite.Create(this.result, new Rect(0f, 0f, (float)this.result.width, (float)this.result.height), new Vector2(0.5f, 0.5f));
		this.VERY_Small.sprite = this.resultSprite.sprite;
	}

	// Token: 0x06003203 RID: 12803 RVA: 0x001957F0 File Offset: 0x001939F0
	public static Texture2D ResizeTexture(Texture2D pSource, ResampleTest.ImageFilterMode pFilterMode, float pScale)
	{
		Color[] pixels = pSource.GetPixels(0);
		Vector2 vector = new Vector2((float)pSource.width, (float)pSource.height);
		float num = (float)Mathf.RoundToInt((float)pSource.width * pScale);
		float num2 = (float)Mathf.RoundToInt((float)pSource.height * pScale);
		Texture2D texture2D = new Texture2D((int)num, (int)num2, TextureFormat.RGBA32, false);
		int num3 = (int)num * (int)num2;
		Color[] array = new Color[num3];
		Vector2 vector2 = new Vector2(vector.x / num, vector.y / num2);
		Vector2 vector3 = default(Vector2);
		for (int i = 0; i < num3; i++)
		{
			float num4 = (float)i % num;
			float num5 = Mathf.Floor((float)i / num);
			vector3.x = num4 / num * vector.x;
			vector3.y = num5 / num2 * vector.y;
			if (pFilterMode == ResampleTest.ImageFilterMode.Nearest)
			{
				vector3.x = Mathf.Round(vector3.x);
				vector3.y = Mathf.Round(vector3.y);
				int num6 = (int)(vector3.y * vector.x + vector3.x);
				array[i] = pixels[num6];
			}
			else if (pFilterMode == ResampleTest.ImageFilterMode.Biliner)
			{
				float t = vector3.x - Mathf.Floor(vector3.x);
				float t2 = vector3.y - Mathf.Floor(vector3.y);
				int num7 = (int)(Mathf.Floor(vector3.y) * vector.x + Mathf.Floor(vector3.x));
				int num8 = (int)(Mathf.Floor(vector3.y) * vector.x + Mathf.Ceil(vector3.x));
				int num9 = (int)(Mathf.Ceil(vector3.y) * vector.x + Mathf.Floor(vector3.x));
				int num10 = (int)(Mathf.Ceil(vector3.y) * vector.x + Mathf.Ceil(vector3.x));
				array[i] = Color.Lerp(Color.Lerp(pixels[num7], pixels[num8], t), Color.Lerp(pixels[num9], pixels[num10], t), t2);
			}
			else if (pFilterMode == ResampleTest.ImageFilterMode.Average)
			{
				int num11 = (int)Mathf.Max(Mathf.Floor(vector3.x - vector2.x * 0.5f), 0f);
				int num12 = (int)Mathf.Min(Mathf.Ceil(vector3.x + vector2.x * 0.5f), vector.x);
				int num13 = (int)Mathf.Max(Mathf.Floor(vector3.y - vector2.y * 0.5f), 0f);
				int num14 = (int)Mathf.Min(Mathf.Ceil(vector3.y + vector2.y * 0.5f), vector.y);
				Color a = default(Color);
				float num15 = 0f;
				for (int j = num13; j < num14; j++)
				{
					for (int k = num11; k < num12; k++)
					{
						a += pixels[(int)((float)j * vector.x + (float)k)];
						num15 += 1f;
					}
				}
				array[i] = a / num15;
			}
		}
		texture2D.SetPixels(array);
		texture2D.Apply();
		return texture2D;
	}

	// Token: 0x04002167 RID: 8551
	public Texture2D original;

	// Token: 0x04002168 RID: 8552
	public Texture2D result;

	// Token: 0x04002169 RID: 8553
	public Image sourceSprite;

	// Token: 0x0400216A RID: 8554
	public Image resultSprite;

	// Token: 0x0400216B RID: 8555
	public Image VERY_Small;

	// Token: 0x0400216C RID: 8556
	public ResampleTest.ImageFilterMode mode = ResampleTest.ImageFilterMode.Average;

	// Token: 0x0400216D RID: 8557
	public float scale;

	// Token: 0x02000884 RID: 2180
	public enum ImageFilterMode
	{
		// Token: 0x04003FD5 RID: 16341
		Nearest,
		// Token: 0x04003FD6 RID: 16342
		Biliner,
		// Token: 0x04003FD7 RID: 16343
		Average
	}
}
