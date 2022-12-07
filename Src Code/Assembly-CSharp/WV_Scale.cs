using System;
using Logic;

// Token: 0x0200015B RID: 347
public class WV_Scale
{
	// Token: 0x060011AC RID: 4524 RVA: 0x000BA294 File Offset: 0x000B8494
	public WV_Scale()
	{
		DT dt = global::Defs.Get(false).dt;
		this.def = new WV_Scale.Def();
		this.def.Load(dt.Find("wv_scale", null));
	}

	// Token: 0x060011AD RID: 4525 RVA: 0x000BA2D5 File Offset: 0x000B84D5
	public static WV_Scale Get()
	{
		if (WV_Scale.wv_scale == null)
		{
			WV_Scale.wv_scale = new WV_Scale();
		}
		return WV_Scale.wv_scale;
	}

	// Token: 0x060011AE RID: 4526 RVA: 0x000BA2F0 File Offset: 0x000B84F0
	public static float GetSize(WV_Scale.Object_Type key)
	{
		WV_Scale wv_Scale = WV_Scale.Get();
		switch (key)
		{
		case WV_Scale.Object_Type.Unit:
			return wv_Scale.def.unit_scale;
		case WV_Scale.Object_Type.Marshal:
			return wv_Scale.def.marshal_scale;
		case WV_Scale.Object_Type.TownNameplate:
			return wv_Scale.def.town_nameplate;
		case WV_Scale.Object_Type.SettlementResources:
			return wv_Scale.def.settlement_resources;
		default:
			return 1f;
		}
	}

	// Token: 0x04000BED RID: 3053
	private static WV_Scale wv_scale;

	// Token: 0x04000BEE RID: 3054
	public WV_Scale.Def def;

	// Token: 0x0200067F RID: 1663
	public class Def
	{
		// Token: 0x060047DE RID: 18398 RVA: 0x00215B78 File Offset: 0x00213D78
		public void Load(DT.Field field)
		{
			if (field == null)
			{
				return;
			}
			this.unit_scale = field.GetFloat("unit_scale", null, 0f, true, true, true, '.');
			this.marshal_scale = field.GetFloat("marshal_scale", null, 0f, true, true, true, '.');
			this.unit_speed_bias = field.GetFloat("unit_speed_bias", null, 0f, true, true, true, '.');
			this.settlement_resources = field.GetFloat("settlement_resources", null, 0f, true, true, true, '.');
			this.town_nameplate = field.GetFloat("town_nameplate", null, 0f, true, true, true, '.');
		}

		// Token: 0x040035B7 RID: 13751
		public float unit_scale = 1f;

		// Token: 0x040035B8 RID: 13752
		public float marshal_scale = 1f;

		// Token: 0x040035B9 RID: 13753
		public float unit_speed_bias = 1f;

		// Token: 0x040035BA RID: 13754
		public float settlement_resources = 1f;

		// Token: 0x040035BB RID: 13755
		public float town_nameplate = 1f;
	}

	// Token: 0x02000680 RID: 1664
	public enum Object_Type
	{
		// Token: 0x040035BD RID: 13757
		Unit,
		// Token: 0x040035BE RID: 13758
		Marshal,
		// Token: 0x040035BF RID: 13759
		TownNameplate,
		// Token: 0x040035C0 RID: 13760
		SettlementResources
	}
}
