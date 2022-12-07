using System;
using System.Runtime.CompilerServices;
using Logic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000274 RID: 628
public class UIProvinceIllustration : MonoBehaviour, IListener
{
	// Token: 0x170001C7 RID: 455
	// (get) Token: 0x06002673 RID: 9843 RVA: 0x001518A6 File Offset: 0x0014FAA6
	// (set) Token: 0x06002674 RID: 9844 RVA: 0x001518AE File Offset: 0x0014FAAE
	public MapObject Data { get; protected set; }

	// Token: 0x06002675 RID: 9845 RVA: 0x001518B7 File Offset: 0x0014FAB7
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_Initialzied = true;
	}

	// Token: 0x06002676 RID: 9846 RVA: 0x001518D0 File Offset: 0x0014FAD0
	public void SetObject(MapObject r)
	{
		this.Init();
		MapObject data = this.Data;
		if (data != null)
		{
			data.DelListener(this);
		}
		this.Data = r;
		MapObject data2 = this.Data;
		if (data2 != null)
		{
			data2.AddListener(this);
		}
		base.enabled = (this.m_SelfUpdate = this.IsMobile(r));
		this.Populate();
		this.Flip(false);
	}

	// Token: 0x06002677 RID: 9847 RVA: 0x00151934 File Offset: 0x0014FB34
	public void Flip(bool flip)
	{
		if (this.m_Fliped == flip)
		{
			return;
		}
		this.m_Fliped = flip;
		if (this.m_Land != null)
		{
			this.m_Land.transform.localScale = new Vector3((float)(flip ? -1 : 1), 1f, 1f);
		}
		if (this.m_Water != null)
		{
			this.m_Water.transform.localScale = new Vector3((float)(flip ? -1 : 1), 1f, 1f);
		}
	}

	// Token: 0x06002678 RID: 9848 RVA: 0x001519BC File Offset: 0x0014FBBC
	public void InvalidateIllsutration()
	{
		this.Populate();
	}

	// Token: 0x06002679 RID: 9849 RVA: 0x001519C4 File Offset: 0x0014FBC4
	private void Populate()
	{
		if (this.Data == null)
		{
			return;
		}
		string cz = this.Data.game.GetClimateZone(this.Data.position).ToString();
		Logic.Realm realm = this.Data.game.GetRealm(this.Data.position);
		object obj;
		if (realm == null)
		{
			obj = null;
		}
		else
		{
			Castle castle = realm.castle;
			obj = ((castle != null) ? castle.visuals : null);
		}
		global::Settlement settlement = obj as global::Settlement;
		string arch = (settlement != null) ? settlement.houses_architecture : "";
		bool flag = this.IsNaval(this.Data);
		bool flag2 = this.IsSiege(this.Data);
		bool flag3 = this.IsInBattle(this.Data);
		bool flag4 = this.IsBreakSiege(this.Data);
		bool flag5 = this.IsAssault(this.Data);
		bool flag6 = this.IsMerc(this.Data);
		bool flag7 = this.IsMobile(this.Data);
		bool flag8 = this.IsKeepSiege(this.Data);
		this.m_Land.gameObject.SetActive(!flag);
		this.m_Water.gameObject.SetActive(flag);
		if (!flag)
		{
			bool available = ((realm != null) ? realm.def : null) != null && realm.def.GetBool("has_mountains", null, false, true, true, true, '.');
			bool available2 = this.Data.game.GetTerrainType(this.Data.position) == TerrainType.Hills;
			bool flag9 = ((realm != null) ? realm.def : null) != null && realm.def.GetBool("has_rivers", null, false, true, true, true, '.');
			bool flag10 = ((realm != null) ? realm.castle : null) != null && realm.castle.coastal;
			bool flag11 = realm != null && realm.IsNearSea();
			bool available3 = ((realm != null) ? realm.castle : null) != null && realm.castle.forest;
			UIProvinceIllustration.<Populate>g__UpdateFeatureSprite|35_0(true, this.m_Sky, cz, "sky", arch);
			UIProvinceIllustration.<Populate>g__UpdateFeatureSprite|35_0(available, this.m_Mountain, cz, "mountains", arch);
			UIProvinceIllustration.<Populate>g__UpdateFeatureSprite|35_0(available2, this.m_Hills, cz, "hills", arch);
			UIProvinceIllustration.<Populate>g__UpdateFeatureSprite|35_0(true, this.m_TerrainBase, cz, "base", arch);
			UIProvinceIllustration.<Populate>g__UpdateFeatureSprite|35_0(available3, this.m_Forest, cz, "forest", arch);
			if (flag7)
			{
				UIProvinceIllustration.<Populate>g__UpdateFeatureSprite|35_0(flag11 && !flag3, this.m_Ocean, cz, "ocean", arch);
			}
			else
			{
				UIProvinceIllustration.<Populate>g__UpdateFeatureSprite|35_0(flag10 && !flag3 && !flag2 && !flag4 && !flag5, this.m_Ocean, cz, "ocean", arch);
			}
			UIProvinceIllustration.<Populate>g__UpdateFeatureSprite|35_0(flag9 && !flag3, this.m_River, cz, "river", arch);
			UIProvinceIllustration.<Populate>g__UpdateFeatureSprite|35_0(flag6 && !flag3, this.m_MercenaryCamp, cz, "mercenary_tents", arch);
			UIProvinceIllustration.<Populate>g__UpdateDecorationSprite|35_1(flag2, this.m_Tents, "tents", arch);
			UIProvinceIllustration.<Populate>g__UpdateDecorationSprite|35_1((flag2 || flag5) && !flag8, this.m_CastleWall, "castle_wall", arch);
			UIProvinceIllustration.<Populate>g__UpdateDecorationSprite|35_1((flag2 || flag5) && !flag8, this.m_CastleBuildings, "castle_building", arch);
			UIProvinceIllustration.<Populate>g__UpdateDecorationSprite|35_1(flag8 && flag2, this.m_Keep, "keep", arch);
			UIProvinceIllustration.<Populate>g__UpdateDecorationSprite|35_1(flag4, this.m_BreakSiegeCastle, "castle_break_siege", arch);
			UIProvinceIllustration.<Populate>g__UpdateDecorationSprite|35_1(flag4, this.m_BreakSiegeTents, "castle_break_siege_tents", arch);
			UIProvinceIllustration.<Populate>g__UpdateDecorationSprite|35_1(flag5, this.m_AssaultCastle, "castle_assault", arch);
			return;
		}
		bool available4 = !WorldMap.IsFarFromLand(this.Data.position, 0.8f);
		UIProvinceIllustration.<Populate>g__UpdateFeatureSprite|35_0(true, this.m_Sea_Sky, cz, "sea_sky", string.Empty);
		UIProvinceIllustration.<Populate>g__UpdateFeatureSprite|35_0(true, this.m_Sea, cz, "sea", string.Empty);
		UIProvinceIllustration.<Populate>g__UpdateFeatureSprite|35_0(available4, this.m_SeaCoastLeft, cz, "sea_coast_left", string.Empty);
		UIProvinceIllustration.<Populate>g__UpdateFeatureSprite|35_0(available4, this.m_SeaCoastRight, cz, "sea_coast_right", string.Empty);
		UIProvinceIllustration.<Populate>g__UpdateFeatureSprite|35_0(!flag3, this.m_Sea_Fleet, cz, "sea_fleet", string.Empty);
		UIProvinceIllustration.<Populate>g__UpdateFeatureSprite|35_0(flag3, this.m_Sea_Battle, cz, "sea_battle", string.Empty);
	}

	// Token: 0x0600267A RID: 9850 RVA: 0x00151DFC File Offset: 0x0014FFFC
	private bool IsInBattle(MapObject mapObject)
	{
		if (mapObject is Logic.Battle)
		{
			return true;
		}
		Logic.Army army = mapObject as Logic.Army;
		if (army != null)
		{
			return army.battle != null;
		}
		Logic.Settlement settlement = mapObject as Logic.Settlement;
		return settlement != null && settlement.battle != null;
	}

	// Token: 0x0600267B RID: 9851 RVA: 0x00151E3C File Offset: 0x0015003C
	private bool IsSiege(MapObject mapObject)
	{
		Logic.Battle battle = mapObject as Logic.Battle;
		return battle != null && battle.type == Logic.Battle.Type.Siege;
	}

	// Token: 0x0600267C RID: 9852 RVA: 0x00151E60 File Offset: 0x00150060
	private bool IsKeepSiege(MapObject mapObject)
	{
		Logic.Battle battle = mapObject as Logic.Battle;
		return battle != null && battle.settlement != null && battle.settlement.type == "Keep";
	}

	// Token: 0x0600267D RID: 9853 RVA: 0x00151E98 File Offset: 0x00150098
	private bool IsBreakSiege(MapObject mapObject)
	{
		Logic.Battle battle = mapObject as Logic.Battle;
		return battle != null && battle.type == Logic.Battle.Type.BreakSiege;
	}

	// Token: 0x0600267E RID: 9854 RVA: 0x00151EBC File Offset: 0x001500BC
	private bool IsAssault(MapObject mapObject)
	{
		Logic.Battle battle = mapObject as Logic.Battle;
		return battle != null && battle.type == Logic.Battle.Type.Assault;
	}

	// Token: 0x0600267F RID: 9855 RVA: 0x00151EE0 File Offset: 0x001500E0
	private bool IsNaval(MapObject mapObject)
	{
		Logic.Battle battle = mapObject as Logic.Battle;
		if (battle != null)
		{
			return battle.type == Logic.Battle.Type.Naval;
		}
		Logic.Army army = mapObject as Logic.Army;
		if (army != null)
		{
			return army.is_in_water && !army.water_crossing.running;
		}
		Logic.Realm realm = this.Data.game.GetRealm(this.Data.position);
		return realm != null && realm.IsSeaRealm();
	}

	// Token: 0x06002680 RID: 9856 RVA: 0x00151F50 File Offset: 0x00150150
	private bool IsMobile(MapObject mo)
	{
		if (mo == null)
		{
			return false;
		}
		if (mo is Logic.Army)
		{
			return true;
		}
		if (mo is Logic.Migrant)
		{
			return true;
		}
		if (mo is Logic.Battle)
		{
			return false;
		}
		Village village = mo as Village;
		return false;
	}

	// Token: 0x06002681 RID: 9857 RVA: 0x00151F80 File Offset: 0x00150180
	private bool IsMerc(MapObject mo)
	{
		if (mo == null)
		{
			return false;
		}
		Logic.Army army = mo as Logic.Army;
		return army != null && army.mercenary != null;
	}

	// Token: 0x06002682 RID: 9858 RVA: 0x00151FA7 File Offset: 0x001501A7
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "moved_in_water" || message == "realm_crossed")
		{
			this.Populate();
			return;
		}
		if (!(message == "moved"))
		{
			return;
		}
		this.Populate();
	}

	// Token: 0x06002683 RID: 9859 RVA: 0x00151FDE File Offset: 0x001501DE
	private void OnDestroy()
	{
		MapObject data = this.Data;
		if (data == null)
		{
			return;
		}
		data.DelListener(this);
	}

	// Token: 0x06002685 RID: 9861 RVA: 0x00151FF4 File Offset: 0x001501F4
	[CompilerGenerated]
	internal static void <Populate>g__UpdateFeatureSprite|35_0(bool available, Image i, string cz, string feature, string arch)
	{
		if (i == null)
		{
			return;
		}
		i.gameObject.SetActive(available);
		if (!available)
		{
			return;
		}
		Sprite obj = global::Defs.GetObj<Sprite>("SettlementIllustrationSettings", "terrain." + feature + "." + cz, null);
		if (obj == null)
		{
			obj = global::Defs.GetObj<Sprite>("SettlementIllustrationSettings", "terrain." + feature, null);
		}
		i.gameObject.SetActive(obj != null);
		if (obj != null)
		{
			i.overrideSprite = obj;
		}
	}

	// Token: 0x06002686 RID: 9862 RVA: 0x0015207C File Offset: 0x0015027C
	[CompilerGenerated]
	internal static void <Populate>g__UpdateDecorationSprite|35_1(bool available, Image i, string decoration, string arch)
	{
		if (i == null)
		{
			return;
		}
		i.gameObject.SetActive(available);
		if (!available)
		{
			return;
		}
		Sprite obj = global::Defs.GetObj<Sprite>("SettlementIllustrationSettings", "decoration." + decoration + "." + arch, null);
		if (obj == null)
		{
			obj = global::Defs.GetObj<Sprite>("SettlementIllustrationSettings", "decoration." + decoration, null);
		}
		i.gameObject.SetActive(obj != null);
		if (obj != null)
		{
			i.overrideSprite = obj;
		}
	}

	// Token: 0x04001A00 RID: 6656
	[UIFieldTarget("id_Land")]
	protected GameObject m_Land;

	// Token: 0x04001A01 RID: 6657
	[UIFieldTarget("id_Sky")]
	protected Image m_Sky;

	// Token: 0x04001A02 RID: 6658
	[UIFieldTarget("id_Mountain")]
	protected Image m_Mountain;

	// Token: 0x04001A03 RID: 6659
	[UIFieldTarget("id_Hills")]
	protected Image m_Hills;

	// Token: 0x04001A04 RID: 6660
	[UIFieldTarget("id_TerrainBase")]
	protected Image m_TerrainBase;

	// Token: 0x04001A05 RID: 6661
	[UIFieldTarget("id_Ocean")]
	protected Image m_Ocean;

	// Token: 0x04001A06 RID: 6662
	[UIFieldTarget("id_River")]
	protected Image m_River;

	// Token: 0x04001A07 RID: 6663
	[UIFieldTarget("id_Forest")]
	protected Image m_Forest;

	// Token: 0x04001A08 RID: 6664
	[UIFieldTarget("id_Settlement")]
	protected Image m_Settlement;

	// Token: 0x04001A09 RID: 6665
	[UIFieldTarget("id_Water")]
	protected GameObject m_Water;

	// Token: 0x04001A0A RID: 6666
	[UIFieldTarget("id_Sea")]
	protected Image m_Sea;

	// Token: 0x04001A0B RID: 6667
	[UIFieldTarget("id_Sea_Sky")]
	protected Image m_Sea_Sky;

	// Token: 0x04001A0C RID: 6668
	[UIFieldTarget("id_Sea_Coast_Left")]
	protected Image m_SeaCoastLeft;

	// Token: 0x04001A0D RID: 6669
	[UIFieldTarget("id_Sea_Coast_Right")]
	protected Image m_SeaCoastRight;

	// Token: 0x04001A0E RID: 6670
	[UIFieldTarget("id_Tents")]
	protected Image m_Tents;

	// Token: 0x04001A0F RID: 6671
	[UIFieldTarget("id_CastleWall")]
	protected Image m_CastleWall;

	// Token: 0x04001A10 RID: 6672
	[UIFieldTarget("id_Keep")]
	protected Image m_Keep;

	// Token: 0x04001A11 RID: 6673
	[UIFieldTarget("id_CastleBuildings")]
	protected Image m_CastleBuildings;

	// Token: 0x04001A12 RID: 6674
	[UIFieldTarget("id_BreakSiegeCastle")]
	protected Image m_BreakSiegeCastle;

	// Token: 0x04001A13 RID: 6675
	[UIFieldTarget("id_BreakSiegeTents")]
	protected Image m_BreakSiegeTents;

	// Token: 0x04001A14 RID: 6676
	[UIFieldTarget("id_AssaultCastle")]
	protected Image m_AssaultCastle;

	// Token: 0x04001A15 RID: 6677
	[UIFieldTarget("id_MercenaryCamp")]
	protected Image m_MercenaryCamp;

	// Token: 0x04001A16 RID: 6678
	[UIFieldTarget("id_Sea_Fleet")]
	protected Image m_Sea_Fleet;

	// Token: 0x04001A17 RID: 6679
	[UIFieldTarget("id_Sea_Battle")]
	protected Image m_Sea_Battle;

	// Token: 0x04001A19 RID: 6681
	private bool m_Initialzied;

	// Token: 0x04001A1A RID: 6682
	private bool m_SelfUpdate;

	// Token: 0x04001A1B RID: 6683
	private bool m_Fliped;
}
