using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001A3 RID: 419
public class UISquadStanceTint
{
	// Token: 0x060017B1 RID: 6065 RVA: 0x000E8550 File Offset: 0x000E6750
	public void UpdateImageRecolor(Logic.Squad squad, GameObject go, bool clamped, bool force_refresh = false)
	{
		if (this.images == null)
		{
			this.supporter_tint = global::Defs.GetColor("Battle", "supporter_color_tint");
			this.clamped_alpha = global::Defs.GetFloat("Battle", "nameplate_clamped_alpha", null, 0f);
		}
		if (this.images == null || force_refresh)
		{
			Image[] componentsInChildren = go.GetComponentsInChildren<Image>(true);
			if (this.images == null)
			{
				this.images = new List<UISquadStanceTint.ImageRecolorInfo>();
			}
			foreach (Image image in componentsInChildren)
			{
				bool flag = false;
				for (int j = 0; j < this.images.Count; j++)
				{
					if (this.images[j].image == image)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					string name = image.material.shader.name;
					bool flag2 = true;
					for (int k = 0; k < UISquadStanceTint.names_to_ignore.Length; k++)
					{
						if (UISquadStanceTint.names_to_ignore[k] == image.name)
						{
							flag2 = false;
							break;
						}
					}
					if (flag2)
					{
						for (int l = 0; l < UISquadStanceTint.shaders_to_ignore.Length; l++)
						{
							string b = UISquadStanceTint.shaders_to_ignore[l];
							if (name == b)
							{
								flag2 = false;
								break;
							}
						}
						if (flag2)
						{
							UISquadStanceTint.ImageRecolorInfo imageRecolorInfo = new UISquadStanceTint.ImageRecolorInfo();
							imageRecolorInfo.image = image;
							imageRecolorInfo.original_color = imageRecolorInfo.image.color;
							this.images.Add(imageRecolorInfo);
						}
					}
				}
			}
		}
		if (squad == null)
		{
			return;
		}
		Color white = Color.white;
		Logic.Kingdom obj = BaseUI.LogicKingdom();
		if (squad.IsOwnStance(obj))
		{
			bool flag3;
			if (squad == null)
			{
				flag3 = (null != null);
			}
			else
			{
				BattleSimulation.Squad simulation = squad.simulation;
				if (simulation == null)
				{
					flag3 = (null != null);
				}
				else
				{
					Logic.Unit unit = simulation.unit;
					if (unit == null)
					{
						flag3 = (null != null);
					}
					else
					{
						Logic.Army army = unit.army;
						flag3 = (((army != null) ? army.mercenary : null) != null);
					}
				}
			}
			if (!flag3)
			{
				goto IL_1D3;
			}
		}
		if (!squad.IsEnemy(obj) && this.is_pinnable)
		{
			white = this.supporter_tint;
		}
		IL_1D3:
		if (clamped)
		{
			white.a *= this.clamped_alpha;
		}
		if (white == this.last_col)
		{
			return;
		}
		for (int m = this.images.Count - 1; m >= 0; m--)
		{
			UISquadStanceTint.ImageRecolorInfo imageRecolorInfo2 = this.images[m];
			if (imageRecolorInfo2.image == null)
			{
				this.images.RemoveAt(m);
			}
			else
			{
				imageRecolorInfo2.image.color = imageRecolorInfo2.original_color * white;
			}
		}
	}

	// Token: 0x04000F3F RID: 3903
	public static string[] shaders_to_ignore = new string[]
	{
		"BSG/UI_ColorEffects_Alpha_5b"
	};

	// Token: 0x04000F40 RID: 3904
	public static string[] names_to_ignore = new string[]
	{
		"id_Glow",
		"id_Glow_Clamped"
	};

	// Token: 0x04000F41 RID: 3905
	public List<UISquadStanceTint.ImageRecolorInfo> images;

	// Token: 0x04000F42 RID: 3906
	private Color supporter_tint;

	// Token: 0x04000F43 RID: 3907
	private float clamped_alpha;

	// Token: 0x04000F44 RID: 3908
	private Color last_col;

	// Token: 0x04000F45 RID: 3909
	public bool is_pinnable;

	// Token: 0x020006D1 RID: 1745
	public class ImageRecolorInfo
	{
		// Token: 0x0400372E RID: 14126
		public Image image;

		// Token: 0x0400372F RID: 14127
		public Color original_color;
	}
}
