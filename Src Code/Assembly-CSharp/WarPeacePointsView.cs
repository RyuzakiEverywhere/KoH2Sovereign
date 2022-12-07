using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x020000FF RID: 255
public class WarPeacePointsView : PoliticalView
{
	// Token: 0x1700008D RID: 141
	// (get) Token: 0x06000BE7 RID: 3047 RVA: 0x00085C79 File Offset: 0x00083E79
	private bool alt_down
	{
		get
		{
			return UICommon.GetKey(KeyCode.RightAlt, false);
		}
	}

	// Token: 0x06000BE8 RID: 3048 RVA: 0x00085C86 File Offset: 0x00083E86
	public override void LoadDef(DT.Field field)
	{
		base.LoadDef(field);
		this.colorTexture = global::Defs.GetObj<Texture2D>(field, "color_texture", null);
	}

	// Token: 0x06000BE9 RID: 3049 RVA: 0x00085CA4 File Offset: 0x00083EA4
	protected override void OnDeactivate()
	{
		base.OnDeactivate();
		WorldUI worldUI = WorldUI.Get();
		if (worldUI != null)
		{
			worldUI.OnKeyPressed -= base.Apply;
			worldUI.OnKeyPressed -= this.RefreshTooltip;
			worldUI.OnKeyReleased -= base.Apply;
			worldUI.OnKeyReleased -= this.RefreshTooltip;
		}
		this.wppRatios.Clear();
	}

	// Token: 0x06000BEA RID: 3050 RVA: 0x00085D1C File Offset: 0x00083F1C
	public void RefreshTooltip()
	{
		WorldUI worldUI = WorldUI.Get();
		if (worldUI != null && this.tt != null)
		{
			worldUI.RefreshTooltip(this.tt, false);
		}
	}

	// Token: 0x06000BEB RID: 3051 RVA: 0x00085D54 File Offset: 0x00083F54
	protected override void OnActivate()
	{
		base.OnActivate();
		WorldUI worldUI = WorldUI.Get();
		if (worldUI != null)
		{
			worldUI.OnKeyPressed += this.RefreshTooltip;
			worldUI.OnKeyReleased += this.RefreshTooltip;
			worldUI.OnKeyPressed += base.Apply;
			worldUI.OnKeyReleased += base.Apply;
		}
		this.lastUpdateTime = UnityEngine.Time.unscaledTime;
		this.firstPass = true;
		this.lastTTUpdateTime = UnityEngine.Time.unscaledTime;
		this.wppRatios.Clear();
	}

	// Token: 0x06000BEC RID: 3052 RVA: 0x00085DE8 File Offset: 0x00083FE8
	public override void OnApply(bool secondary)
	{
		if (UnityEngine.Time.unscaledTime - this.lastUpdateTime > 30f)
		{
			this.wppRatios.Clear();
			this.lastUpdateTime = UnityEngine.Time.unscaledTime;
		}
		base.OnApply(secondary);
		if (this.colorTexture == null)
		{
			Debug.Log("War peace points texture is not set");
			return;
		}
		int height = this.colorTexture.height;
		List<global::Realm> realms = this.wm.Realms;
		global::Realm realm = global::Realm.Get(this.wm.highlighted_realm);
		global::Realm realm2 = global::Realm.Get(this.wm.prev_highlighted_realm);
		int num = (realm != null) ? realm.kingdom.id : 0;
		int num2 = (realm2 != null) ? realm2.kingdom.id : 0;
		if (!this.firstPass && this.alt == this.alt_down && num == num2 && this.prevKingdom == this.wm.SrcKingdom.id)
		{
			return;
		}
		this.firstPass = false;
		this.alt = this.alt_down;
		this.prevKingdom = this.wm.SrcKingdom.id;
		for (int i = 1; i <= realms.Count; i++)
		{
			global::Realm realm3 = realms[i - 1];
			Color newColor = Color.gray;
			global::Kingdom kingdom = realm3.GetKingdom();
			if (kingdom == null)
			{
				this.SetRealmColor(i, newColor);
			}
			else
			{
				if (this.kSrc != null && kingdom == this.kSrc)
				{
					newColor = Color.clear;
				}
				else if (realm3.logic != null)
				{
					float num3 = this.alt_down ? this.GetRatio(this.kSrc, kingdom) : this.GetRatio(kingdom, this.kSrc);
					newColor = this.colorTexture.GetPixel(0, (int)(num3 / 2f * (float)height));
				}
				this.SetRealmColor(i, newColor);
			}
		}
	}

	// Token: 0x06000BED RID: 3053 RVA: 0x00085FB8 File Offset: 0x000841B8
	private float GetRatio(global::Kingdom kSrc, global::Kingdom k)
	{
		float num = 0f;
		if (((k != null) ? k.logic : null) != null)
		{
			WarPeacePointsView.WppRatio key = new WarPeacePointsView.WppRatio
			{
				kID = kSrc.logic.id,
				otherKID = k.logic.id
			};
			if (!this.wppRatios.TryGetValue(key, out num))
			{
				ProsAndCons prosAndCons = ProsAndCons.Get(WarPeacePointsView.pc_name, kSrc.logic, k.logic);
				prosAndCons.Calc(false);
				num = prosAndCons.ratio;
				this.wppRatios[key] = num;
			}
		}
		return num;
	}

	// Token: 0x06000BEE RID: 3054 RVA: 0x0008604C File Offset: 0x0008424C
	public override bool HandleTooltip(BaseUI ui, Tooltip tooltip, Tooltip.Event evt)
	{
		if (evt == Tooltip.Event.Fill || evt == Tooltip.Event.Update)
		{
			WorldMap worldMap = WorldMap.Get();
			WorldUI.Get();
			if (worldMap != null && worldMap.highlighted_realm != 0)
			{
				global::Realm realm = global::Realm.Get(worldMap.highlighted_realm);
				int num = (realm != null) ? realm.kingdom.id : 0;
				global::Realm realm2 = global::Realm.Get(worldMap.prev_highlighted_realm);
				int num2 = (realm2 != null) ? realm2.kingdom.id : 0;
				bool flag = this.alt != this.alt_down;
				if (!flag && num == num2 && this.prevKingdom == worldMap.SrcKingdom.id && UnityEngine.Time.unscaledTime - this.lastTTUpdateTime <= 5f)
				{
					return true;
				}
				this.lastTTUpdateTime = UnityEngine.Time.unscaledTime;
				if (num != 0)
				{
					if (num2 == num && !flag)
					{
						return false;
					}
					global::Kingdom kingdom = global::Kingdom.Get(worldMap.SrcKingdom);
					ProsAndCons testProCon;
					if (this.alt_down)
					{
						testProCon = ProsAndCons.GetTestProCon(WarPeacePointsView.pc_name, kingdom.logic, global::Kingdom.Get(num).logic);
					}
					else
					{
						testProCon = ProsAndCons.GetTestProCon(WarPeacePointsView.pc_name, global::Kingdom.Get(num).logic, kingdom.logic);
					}
					testProCon.Calc(true);
					tooltip.text = null;
					tooltip.TextKey = testProCon.Dump();
					if (tooltip.TextKey != "")
					{
						tooltip.TextKey = global::Defs.ReplaceVars("#{align:left}" + tooltip.TextKey.Substring(1), null, true, '\0');
					}
					if (tooltip.instance != null && !flag)
					{
						tooltip.instance.transform.position = Input.mousePosition;
					}
				}
				else
				{
					tooltip.text = null;
					tooltip.TextKey = null;
				}
			}
			return false;
		}
		return base.HandleTooltip(ui, tooltip, evt);
	}

	// Token: 0x0400094A RID: 2378
	public static string pc_name = "PC_War";

	// Token: 0x0400094B RID: 2379
	public Texture2D colorTexture;

	// Token: 0x0400094C RID: 2380
	private float lastUpdateTime;

	// Token: 0x0400094D RID: 2381
	private float lastTTUpdateTime;

	// Token: 0x0400094E RID: 2382
	private bool alt;

	// Token: 0x0400094F RID: 2383
	private int prevKingdom;

	// Token: 0x04000950 RID: 2384
	private Dictionary<WarPeacePointsView.WppRatio, float> wppRatios = new Dictionary<WarPeacePointsView.WppRatio, float>();

	// Token: 0x04000951 RID: 2385
	private bool firstPass;

	// Token: 0x02000606 RID: 1542
	internal struct WppRatio
	{
		// Token: 0x04003385 RID: 13189
		public int kID;

		// Token: 0x04003386 RID: 13190
		public int otherKID;
	}
}
