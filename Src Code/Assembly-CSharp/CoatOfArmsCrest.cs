using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x02000108 RID: 264
[Serializable]
public class CoatOfArmsCrest
{
	// Token: 0x06000C31 RID: 3121 RVA: 0x0008889C File Offset: 0x00086A9C
	public CoatOfArmsCrest()
	{
		this.division_base = new CoatOfArmsTexture(null);
	}

	// Token: 0x06000C32 RID: 3122 RVA: 0x000888F4 File Offset: 0x00086AF4
	public CoatOfArmsCrest(DT.Field def)
	{
		DT.Field field = def.FindChild("division", null, true, true, true, '.');
		if (field != null)
		{
			this.division_base = new CoatOfArmsTexture(field, null);
		}
		for (int i = 0; i < CoatOfArms.Instance.modes.Count; i++)
		{
			DT.Field field2 = def.FindChild("division_" + CoatOfArms.Instance.modes[i].ToString(), null, true, true, true, '.');
			if (field2 != null)
			{
				if (field2 != null)
				{
					CoatOfArmsTexture value = new CoatOfArmsTexture(field2, CoatOfArms.Instance.modes[i]);
					this.divisionPerMode.Add(CoatOfArms.Instance.modes[i].dir, value);
				}
			}
			else if (CoatOfArmsUtility.FindInChildren(def, CoatOfArms.Instance.modes[i].dir) != null)
			{
				CoatOfArmsTexture value2 = new CoatOfArmsTexture(field, CoatOfArms.Instance.modes[i]);
				this.divisionPerMode.Add(CoatOfArms.Instance.modes[i].dir, value2);
			}
			CoatOfArmsTexture coatOfArmsTexture;
			this.divisionPerMode.TryGetValue(CoatOfArms.Instance.modes[i].dir, out coatOfArmsTexture);
			if (i == 0 && coatOfArmsTexture != null)
			{
				this.division_base = coatOfArmsTexture;
			}
			if (coatOfArmsTexture == null)
			{
				CoatOfArmsTexture coatOfArmsTexture2 = new CoatOfArmsTexture(this.division_base, true);
				coatOfArmsTexture2.mode = CoatOfArms.Instance.modes[i];
				coatOfArmsTexture2.changed = false;
				this.divisionPerMode.Add(CoatOfArms.Instance.modes[i].dir, coatOfArmsTexture2);
			}
		}
		DT.Field field3 = def.FindChild("ordinaries", null, true, true, true, '.');
		if (field3 != null && field3.children != null && field3.children.Count > 0)
		{
			for (int j = 0; j < field3.children.Count; j++)
			{
				this.ordinaries_base.Add(new CoatOfArmsTexture(field3.children[j], null));
			}
		}
		for (int k = 0; k < CoatOfArms.Instance.modes.Count; k++)
		{
			DT.Field field4 = def.FindChild("ordinaries_" + CoatOfArms.Instance.modes[k].dir, null, true, true, true, '.');
			if (field4 != null && field4.children != null && field4.children.Count > 0)
			{
				List<CoatOfArmsTexture> list = new List<CoatOfArmsTexture>();
				for (int l = 0; l < field4.children.Count; l++)
				{
					list.Add(new CoatOfArmsTexture(field4.children[l], CoatOfArms.Instance.modes[k]));
				}
				if (list.Count > 0)
				{
					this.ordinariesPerMode.Add(CoatOfArms.Instance.modes[k].dir, list);
				}
			}
			else if (field3 != null && field3.children != null)
			{
				List<CoatOfArmsTexture> list2 = new List<CoatOfArmsTexture>();
				for (int m = 0; m < field3.children.Count; m++)
				{
					if (CoatOfArmsUtility.FindInChildren(field3.children[m], CoatOfArms.Instance.modes[k].dir) != null)
					{
						CoatOfArmsTexture item = new CoatOfArmsTexture(field3.children[m], CoatOfArms.Instance.modes[k]);
						list2.Add(item);
					}
				}
				if (list2.Count > 0)
				{
					this.ordinariesPerMode.Add(CoatOfArms.Instance.modes[k].dir, this.ordinaries_base);
				}
			}
			List<CoatOfArmsTexture> list3;
			this.ordinariesPerMode.TryGetValue(CoatOfArms.Instance.modes[k].dir, out list3);
			if (k == 0 && list3 != null)
			{
				this.ordinaries_base = list3;
			}
			if (list3 == null)
			{
				List<CoatOfArmsTexture> list4 = new List<CoatOfArmsTexture>(this.ordinaries_base.Count);
				foreach (CoatOfArmsTexture ct in this.ordinaries_base)
				{
					list4.Add(new CoatOfArmsTexture(ct, true)
					{
						mode = CoatOfArms.Instance.modes[k],
						changed = false
					});
				}
				this.ordinariesPerMode.Add(CoatOfArms.Instance.modes[k].dir, list4);
			}
		}
		DT.Field field5 = def.FindChild("charges", null, true, true, true, '.');
		if (field5 != null && field5.children != null && field5.children.Count > 0)
		{
			for (int n = 0; n < field5.children.Count; n++)
			{
				this.charges_base.Add(new CoatOfArmsTexture(field5.children[n], null));
			}
		}
		for (int num = 0; num < CoatOfArms.Instance.modes.Count; num++)
		{
			DT.Field field6 = def.FindChild("charges_" + CoatOfArms.Instance.modes[num].dir, null, true, true, true, '.');
			if (field6 != null && field6.children != null && field6.children.Count > 0)
			{
				List<CoatOfArmsTexture> list5 = new List<CoatOfArmsTexture>();
				for (int num2 = 0; num2 < field6.children.Count; num2++)
				{
					list5.Add(new CoatOfArmsTexture(field6.children[num2], CoatOfArms.Instance.modes[num]));
				}
				if (list5.Count > 0)
				{
					this.chargesPerMode.Add(CoatOfArms.Instance.modes[num].dir, list5);
				}
			}
			else if (field5 != null && field5.children != null)
			{
				List<CoatOfArmsTexture> list6 = new List<CoatOfArmsTexture>();
				for (int num3 = 0; num3 < field5.children.Count; num3++)
				{
					if (CoatOfArmsUtility.FindInChildren(field5.children[num3], CoatOfArms.Instance.modes[num].dir) != null)
					{
						CoatOfArmsTexture item2 = new CoatOfArmsTexture(field5.children[num3], CoatOfArms.Instance.modes[num]);
						list6.Add(item2);
					}
				}
				if (list6.Count > 0)
				{
					this.chargesPerMode.Add(CoatOfArms.Instance.modes[num].dir, list6);
				}
			}
			List<CoatOfArmsTexture> list7;
			this.chargesPerMode.TryGetValue(CoatOfArms.Instance.modes[num].dir, out list7);
			if (num == 0 && list7 != null)
			{
				this.charges_base = list7;
			}
			if (list7 == null)
			{
				List<CoatOfArmsTexture> list8 = new List<CoatOfArmsTexture>(this.charges_base.Count);
				foreach (CoatOfArmsTexture ct2 in this.charges_base)
				{
					CoatOfArmsTexture coatOfArmsTexture3 = new CoatOfArmsTexture(ct2, true);
					coatOfArmsTexture3.mode = CoatOfArms.Instance.modes[num];
					coatOfArmsTexture3.transform = CoatOfArmsUtility.ConvertChargeTransform(coatOfArmsTexture3.transform, CoatOfArms.Instance.modes[0], coatOfArmsTexture3.mode);
					coatOfArmsTexture3.changed = false;
					list8.Add(coatOfArmsTexture3);
				}
				this.chargesPerMode.Add(CoatOfArms.Instance.modes[num].dir, list8);
			}
		}
	}

	// Token: 0x06000C33 RID: 3123 RVA: 0x000890E4 File Offset: 0x000872E4
	public Texture RemapTexture(RenderTexture rt, CrestMode mode)
	{
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = rt;
		Graphics.SetRenderTarget(rt);
		CoatOfArmsTexture coatOfArmsTexture;
		this.divisionPerMode.TryGetValue(mode.dir, out coatOfArmsTexture);
		Texture texture;
		if (coatOfArmsTexture != null)
		{
			texture = coatOfArmsTexture.RemapTexture(mode, false, false);
		}
		else
		{
			if (this.division_base == null || this.division_base.texture == null)
			{
				Graphics.Blit(CoatOfArms.Instance.WHITETEXTURE, rt);
				RenderTexture.active = active;
				return rt;
			}
			texture = this.division_base.RemapTexture(mode, false, false);
		}
		Graphics.Blit(texture, rt);
		if (texture.GetType() == typeof(RenderTexture))
		{
			RenderTexture.ReleaseTemporary(texture as RenderTexture);
		}
		RenderTexture temporary = RenderTexture.GetTemporary(mode.width, mode.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
		Graphics.SetRenderTarget(temporary);
		List<Texture> list = new List<Texture>();
		bool flag = false;
		List<CoatOfArmsTexture> list2 = null;
		this.ordinariesPerMode.TryGetValue(mode.dir, out list2);
		if (list2 == null)
		{
			list2 = this.ordinaries_base;
		}
		if (list2 != null && list2.Count > 0)
		{
			for (int i = 0; i < list2.Count; i++)
			{
				if (!(list2[i].texture == null) && !(list2[i].texture == CoatOfArms.Instance.WHITETEXTURE))
				{
					flag = true;
					Texture texture2 = list2[i].RemapTexture(mode, true, false);
					list.Add(texture2);
					Graphics.Blit(texture2, temporary);
				}
			}
		}
		if (flag)
		{
			CoatOfArmsUtility.MergeTextures(rt, temporary, rt, CoatOfArmsUtility.mergeMat);
		}
		RenderTexture renderTexture = RenderTexture.GetTemporary(mode.width, mode.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
		Graphics.SetRenderTarget(renderTexture);
		GL.Clear(true, true, Color.clear, 0f);
		List<CoatOfArmsTexture> list3 = null;
		this.chargesPerMode.TryGetValue(mode.dir, out list3);
		if (list3 == null)
		{
			list3 = this.charges_base;
		}
		if (list3 != null && list3.Count > 0)
		{
			float num = (float)Mathf.Min(mode.width, mode.height) / (float)Mathf.Max(mode.width, mode.height);
			Texture[] array = new Texture[list3.Count];
			for (int j = 0; j < list3.Count; j++)
			{
				array[j] = list3[j].RemapTexture(mode, true, false);
			}
			renderTexture = TexRender.Begin(renderTexture, mode.width, mode.height);
			for (int k = 0; k < list3.Count; k++)
			{
				CoatOfArmsTexture coatOfArmsTexture2 = list3[k];
				if (!(coatOfArmsTexture2.texture == null) && !(coatOfArmsTexture2.texture == CoatOfArms.Instance.WHITETEXTURE))
				{
					CoATransform transform = coatOfArmsTexture2.GetTransform(null);
					TexRender.Draw(new TexRender.Quad(array[k], Shader.Find("Unlit/Transparent"))
					{
						pos = new Vector2(transform.position.x * (float)mode.width, transform.position.y * (float)mode.height),
						scale = new Vector2(transform.scale.x * num, transform.scale.y * num),
						rot = transform.rotation,
						pivot = new Vector2(transform.pivot.x, transform.pivot.y)
					});
					CoatOfArmsUtility.MergeTextures(rt, renderTexture, rt, CoatOfArmsUtility.mergeMat);
				}
			}
			TexRender.End();
			for (int l = 0; l < array.Length; l++)
			{
				RenderTexture.ReleaseTemporary(array[l] as RenderTexture);
			}
		}
		Graphics.SetRenderTarget(active);
		RenderTexture.ReleaseTemporary(renderTexture);
		RenderTexture.ReleaseTemporary(temporary);
		for (int m = 0; m < list.Count; m++)
		{
			Texture texture3 = list[m];
			if (texture3.GetType() == typeof(RenderTexture))
			{
				RenderTexture.ReleaseTemporary(texture3 as RenderTexture);
			}
		}
		return rt;
	}

	// Token: 0x06000C34 RID: 3124 RVA: 0x000023FD File Offset: 0x000005FD
	public static void DumpRenderTexture(RenderTexture rt, string pngOutPath)
	{
	}

	// Token: 0x04000986 RID: 2438
	public CoatOfArmsTexture division_base;

	// Token: 0x04000987 RID: 2439
	public List<CoatOfArmsTexture> ordinaries_base = new List<CoatOfArmsTexture>();

	// Token: 0x04000988 RID: 2440
	public List<CoatOfArmsTexture> charges_base = new List<CoatOfArmsTexture>();

	// Token: 0x04000989 RID: 2441
	public Dictionary<string, CoatOfArmsTexture> divisionPerMode = new Dictionary<string, CoatOfArmsTexture>();

	// Token: 0x0400098A RID: 2442
	public Dictionary<string, List<CoatOfArmsTexture>> ordinariesPerMode = new Dictionary<string, List<CoatOfArmsTexture>>();

	// Token: 0x0400098B RID: 2443
	public Dictionary<string, List<CoatOfArmsTexture>> chargesPerMode = new Dictionary<string, List<CoatOfArmsTexture>>();
}
