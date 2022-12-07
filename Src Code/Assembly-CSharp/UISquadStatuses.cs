using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x020001DF RID: 479
public class UISquadStatuses : MonoBehaviour
{
	// Token: 0x06001C63 RID: 7267 RVA: 0x0010BEA8 File Offset: 0x0010A0A8
	private void Update()
	{
		this.UpdateBuffs();
	}

	// Token: 0x06001C64 RID: 7268 RVA: 0x0010BEB0 File Offset: 0x0010A0B0
	public void SetSquad(Logic.Squad squad)
	{
		this.Clear();
		this.m_logic = squad;
		this.Initialize();
	}

	// Token: 0x06001C65 RID: 7269 RVA: 0x0010BEC8 File Offset: 0x0010A0C8
	public void Clear()
	{
		this.m_buffs.Clear();
		for (int i = base.transform.childCount - 1; i >= 0; i--)
		{
			global::Common.DestroyObj(base.transform.GetChild(i).gameObject);
		}
	}

	// Token: 0x06001C66 RID: 7270 RVA: 0x0010BF10 File Offset: 0x0010A110
	private void Initialize()
	{
		if (this.m_componentDefinition == null)
		{
			this.m_componentDefinition = global::Defs.GetDefField("UISquadStatuses", null);
		}
		if (this.m_unitStatusPrefab == null)
		{
			this.m_unitStatusPrefab = global::Defs.GetObj<GameObject>(this.m_componentDefinition, "status_prefab", null);
		}
		if (this.m_logic == null)
		{
			return;
		}
		for (int i = 0; i < this.m_logic.buffs.Count; i++)
		{
			this.AddStatus(this.m_logic.buffs[i], UIBattleViewSquad.icon_settings);
		}
	}

	// Token: 0x06001C67 RID: 7271 RVA: 0x0010BF9C File Offset: 0x0010A19C
	private void UpdateBuffs()
	{
		foreach (SquadBuff squadBuff in this.m_buffs.Keys)
		{
			this.m_buffs[squadBuff].gameObject.SetActive(squadBuff.enabled);
		}
	}

	// Token: 0x06001C68 RID: 7272 RVA: 0x0010C00C File Offset: 0x0010A20C
	private void AddStatus(SquadBuff buff, DT.Field icon_settings)
	{
		if (this.m_buffs.ContainsKey(buff) || this.m_unitStatusPrefab == null)
		{
			return;
		}
		DT.Field field = icon_settings.FindChild(buff.field.key, null, true, true, true, '.');
		if (field == null)
		{
			return;
		}
		UISquadStatus component = global::Common.Spawn(this.m_unitStatusPrefab, base.transform, false, "").GetComponent<UISquadStatus>();
		if (component == null)
		{
			return;
		}
		component.SetDef(buff, field);
		this.m_buffs.Add(buff, component);
		component.gameObject.SetActive(false);
	}

	// Token: 0x04001294 RID: 4756
	private Logic.Squad m_logic;

	// Token: 0x04001295 RID: 4757
	private DT.Field m_componentDefinition;

	// Token: 0x04001296 RID: 4758
	private GameObject m_unitStatusPrefab;

	// Token: 0x04001297 RID: 4759
	private Dictionary<SquadBuff, UISquadStatus> m_buffs = new Dictionary<SquadBuff, UISquadStatus>();
}
