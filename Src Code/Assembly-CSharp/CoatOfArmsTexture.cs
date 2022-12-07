using System;
using System.Collections.Generic;
using System.IO;
using Logic;
using UnityEngine;

// Token: 0x02000104 RID: 260
[Serializable]
public class CoatOfArmsTexture
{
	// Token: 0x17000090 RID: 144
	// (get) Token: 0x06000C16 RID: 3094 RVA: 0x00087C64 File Offset: 0x00085E64
	public Texture2D texture
	{
		get
		{
			if (this.texPath == "")
			{
				return CoatOfArms.Instance.WHITETEXTURE;
			}
			Texture2D texture2D = Assets.Get<Texture2D>(this.texPath);
			if (texture2D == null)
			{
				return CoatOfArms.Instance.WHITETEXTURE;
			}
			return texture2D;
		}
	}

	// Token: 0x06000C17 RID: 3095 RVA: 0x00087CAF File Offset: 0x00085EAF
	public CoATransform GetTransform(CrestMode mode = null)
	{
		return this.transform;
	}

	// Token: 0x06000C18 RID: 3096 RVA: 0x00087CB7 File Offset: 0x00085EB7
	public CoatOfArmsTexture(CrestMode mode = null)
	{
		this.mode = mode;
	}

	// Token: 0x06000C19 RID: 3097 RVA: 0x00087CEC File Offset: 0x00085EEC
	public CoatOfArmsTexture(CoatOfArmsTexture ct, bool copy_mapper = false)
	{
		this.texPath = ct.texPath;
		this.mode = ct.mode;
		if (copy_mapper)
		{
			this.SetMapperViaCopy(ct.mapper);
		}
		else
		{
			this.mapper = ct.mapper;
		}
		this.transform = new CoATransform(ct.transform);
	}

	// Token: 0x06000C1A RID: 3098 RVA: 0x00087D69 File Offset: 0x00085F69
	public CoatOfArmsTexture(Texture2D texture, CrestMode mode = null)
	{
		this.SetTexture(texture);
		this.mode = mode;
	}

	// Token: 0x06000C1B RID: 3099 RVA: 0x00087DA4 File Offset: 0x00085FA4
	public CoatOfArmsTexture(DT.Field def, CrestMode mode = null)
	{
		DT.Field field = def;
		if (mode != null)
		{
			DT.Field field2 = CoatOfArmsUtility.FindInChildren(def, mode.dir);
			if (field2 != null)
			{
				field = field2;
			}
		}
		this.transform = new CoATransform(field);
		this.mode = mode;
		this.texPath = DT.Unquote(def.GetValueStr("t", "", true, true, true, '.'));
		DT.Field field3 = def.FindChild("mapper", null, true, true, true, '.');
		this.SetMapper(field3);
	}

	// Token: 0x06000C1C RID: 3100 RVA: 0x00087E40 File Offset: 0x00086040
	public void SetMapper(DT.Field m)
	{
		this.mapper = new TextureMapperContainer(this.GetAvailableColors());
		if (m == null || m.children == null || m.children.Count == 0)
		{
			return;
		}
		for (int i = 0; i < m.children.Count; i++)
		{
			DT.Field field = m.children[i];
			Color color;
			CoatOfArmsUtility.letterToColor.TryGetValue(field.key, out color);
			if (field.ToString().Contains("europe.3.division_shield.mapper.g"))
			{
				field.FindChild("tint", null, true, true, true, '.');
			}
			if (field.ToString().Contains("europe.22.division_shield.mapper.g"))
			{
				field.FindChild("tint", null, true, true, true, '.');
			}
			DT.Field field2 = field.FindChild("texture", null, false, true, true, '.');
			if (field2 == null)
			{
				DT.Field field3 = field.FindChild("tint", null, true, true, true, '.');
				string text = null;
				if (field3 != null)
				{
					if (CoatOfArms.Instance.paletteColors.ContainsKey(field3.value_str))
					{
						text = field3.value_str;
					}
					else
					{
						if (CoatOfArmsUtility.letterToColor.ContainsKey(field3.value_str))
						{
							text = field3.value_str;
						}
						if (text == null)
						{
							Color c = global::Defs.ColorFromString(field3.value_str, color);
							text = CoatOfArmsUtility.GetNearestPaletteColor(CoatOfArms.Instance.paletteColors, c);
						}
					}
				}
				else if (CoatOfArms.Instance.paletteColors.ContainsKey(field.key))
				{
					text = field.key;
				}
				else
				{
					text = "w";
				}
				this.mapper[color] = new TextureMapper(color, text);
			}
			else
			{
				this.mapper[color] = new TextureMapper(color, new CoatOfArmsTexture(field2, this.mode), "w");
			}
		}
	}

	// Token: 0x06000C1D RID: 3101 RVA: 0x00087FF8 File Offset: 0x000861F8
	public void SetMapperViaCopy(TextureMapperContainer m)
	{
		this.mapper = new TextureMapperContainer(this.GetAvailableColors());
		if (m == null)
		{
			return;
		}
		foreach (KeyValuePair<Color, TextureMapper> keyValuePair in m)
		{
			Color key = keyValuePair.Key;
			if (!string.IsNullOrEmpty(keyValuePair.Value.tintColorStr))
			{
				this.mapper[key] = new TextureMapper(key, keyValuePair.Value.tintColorStr);
			}
			if (keyValuePair.Value.texture != null)
			{
				this.mapper[key] = new TextureMapper(key, new CoatOfArmsTexture(keyValuePair.Value.texture, false), keyValuePair.Value.tintColorStr);
			}
		}
	}

	// Token: 0x06000C1E RID: 3102 RVA: 0x000880D0 File Offset: 0x000862D0
	public List<Color> GetAvailableColors()
	{
		if (this.texture == null)
		{
			return new List<Color>
			{
				Color.red,
				Color.green,
				Color.blue
			};
		}
		if (this.availableColors == null)
		{
			this.availableColors = CoatOfArmsUtility.GetAvailableColors(this.texture);
		}
		return this.availableColors;
	}

	// Token: 0x06000C1F RID: 3103 RVA: 0x00088131 File Offset: 0x00086331
	public void Flip(bool x = true, bool y = true)
	{
		if (x)
		{
			this.GetTransform(null).tileX *= -1f;
		}
		if (y)
		{
			this.GetTransform(null).tileY *= -1f;
		}
	}

	// Token: 0x06000C20 RID: 3104 RVA: 0x0008816C File Offset: 0x0008636C
	public void SetTexture(Texture2D texture)
	{
		if (texture == null)
		{
			return;
		}
		List<Color> colors = CoatOfArmsUtility.GetAvailableColors(texture);
		if (this.mapper != null)
		{
			this.mapper = new TextureMapperContainer(colors, this.mapper);
		}
		else
		{
			this.mapper = new TextureMapperContainer(colors);
		}
		this.changed = true;
	}

	// Token: 0x06000C21 RID: 3105 RVA: 0x000881BC File Offset: 0x000863BC
	public Texture2D GetTexture(CrestMode mode)
	{
		if (this.texPath == "")
		{
			return this.texture;
		}
		string text = Path.GetDirectoryName(this.texPath);
		string fileName = Path.GetFileName(this.texPath);
		if (mode.dir == "")
		{
			text = this.texPath;
		}
		else
		{
			text = string.Concat(new string[]
			{
				text,
				"/",
				mode.dir,
				"/",
				fileName
			});
		}
		text = text.Replace('\\', '/');
		Texture2D texture2D = Assets.Get<Texture2D>(text);
		if (texture2D != null)
		{
			return texture2D;
		}
		return this.texture;
	}

	// Token: 0x06000C22 RID: 3106 RVA: 0x00088268 File Offset: 0x00086468
	public Texture RemapTexture(CrestMode mode, bool keepSize = false, bool remapMode = false)
	{
		bool flag = false;
		Texture2D texture = this.GetTexture(mode);
		if (texture != null && texture != CoatOfArms.Instance.WHITETEXTURE)
		{
			flag = true;
		}
		List<Texture> list = new List<Texture>();
		if (!flag)
		{
			return texture;
		}
		Material material = new Material(Shader.Find("BSG/CoatOfArms/ReplaceColors"));
		material.hideFlags = HideFlags.HideAndDontSave;
		if (this.mapper != null)
		{
			foreach (KeyValuePair<Color, TextureMapper> keyValuePair in this.mapper)
			{
				if (keyValuePair.Value.texture != null)
				{
					string str = CoatOfArmsUtility.colorToLetter[keyValuePair.Value.channelColor].ToUpperInvariant();
					Texture texture2 = keyValuePair.Value.texture.RemapTexture(mode, keepSize, true);
					material.SetTexture("_Tex" + str, texture2);
					list.Add(texture2);
					if (texture2 != CoatOfArms.Instance.WHITETEXTURE)
					{
						material.SetColor("_Tint" + str, Color.white);
					}
					else
					{
						Color white;
						if (!CoatOfArms.Instance.paletteColors.TryGetValue(keyValuePair.Value.tintColorStr, out white) && !CoatOfArmsUtility.letterToColor.TryGetValue(keyValuePair.Value.tintColorStr, out white))
						{
							Debug.LogWarning("Cant find palette color for text \"" + keyValuePair.Value.tintColorStr + "\". Choosing default color - white.");
							white = Color.white;
						}
						material.SetColor("_Tint" + str, white.gamma);
					}
				}
			}
		}
		int width = mode.width;
		int height = mode.height;
		if (keepSize && texture != null)
		{
			width = texture.width;
			height = texture.height;
		}
		RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
		CoATransform coATransform = this.GetTransform(null);
		if (coATransform.tileX != 1f || coATransform.tileY != 1f || coATransform.offsetX != 0f || coATransform.offsetY != 0f || remapMode)
		{
			Vector2 vector = new Vector2(coATransform.tileX, coATransform.tileY);
			if (remapMode)
			{
				float num = (float)Mathf.Min(mode.width, mode.height);
				vector *= new Vector2((float)mode.width / num, (float)mode.height / num);
			}
			RenderTexture temporary2 = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
			Graphics.Blit(texture, temporary2, vector, new Vector2(coATransform.offsetX, coATransform.offsetY));
			Graphics.Blit(temporary2, temporary, material);
			RenderTexture.ReleaseTemporary(temporary2);
		}
		else
		{
			Graphics.Blit(texture, temporary, material);
		}
		for (int i = 0; i < list.Count; i++)
		{
			Texture texture3 = list[i];
			if (texture3.GetType() == typeof(RenderTexture))
			{
				RenderTexture.ReleaseTemporary(texture3 as RenderTexture);
			}
		}
		UnityEngine.Object.Destroy(material);
		return temporary;
	}

	// Token: 0x04000973 RID: 2419
	public string texPath = "";

	// Token: 0x04000974 RID: 2420
	public TextureMapperContainer mapper;

	// Token: 0x04000975 RID: 2421
	public CoATransform transform = new CoATransform();

	// Token: 0x04000976 RID: 2422
	public bool show = true;

	// Token: 0x04000977 RID: 2423
	public CrestMode mode;

	// Token: 0x04000978 RID: 2424
	private List<Color> availableColors;

	// Token: 0x04000979 RID: 2425
	public bool changed = true;
}
