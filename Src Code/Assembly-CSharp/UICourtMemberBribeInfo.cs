using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x02000289 RID: 649
public class UICourtMemberBribeInfo : MonoBehaviour, IListener
{
	// Token: 0x170001D6 RID: 470
	// (get) Token: 0x060027AB RID: 10155 RVA: 0x00157EC6 File Offset: 0x001560C6
	// (set) Token: 0x060027AC RID: 10156 RVA: 0x00157ECE File Offset: 0x001560CE
	public HasPuppetStatus PupetStatus { get; private set; }

	// Token: 0x170001D7 RID: 471
	// (get) Token: 0x060027AD RID: 10157 RVA: 0x00157ED7 File Offset: 0x001560D7
	// (set) Token: 0x060027AE RID: 10158 RVA: 0x00157EDF File Offset: 0x001560DF
	public Logic.Character Spy { get; private set; }

	// Token: 0x170001D8 RID: 472
	// (get) Token: 0x060027AF RID: 10159 RVA: 0x00157EE8 File Offset: 0x001560E8
	// (set) Token: 0x060027B0 RID: 10160 RVA: 0x00157EF0 File Offset: 0x001560F0
	public Logic.Character Pupet { get; private set; }

	// Token: 0x170001D9 RID: 473
	// (get) Token: 0x060027B1 RID: 10161 RVA: 0x00157EF9 File Offset: 0x001560F9
	// (set) Token: 0x060027B2 RID: 10162 RVA: 0x00157F01 File Offset: 0x00156101
	public Vars Vars { get; private set; }

	// Token: 0x060027B3 RID: 10163 RVA: 0x00157F0C File Offset: 0x0015610C
	public void SetData(Logic.Status status, Vars vars)
	{
		this.Init();
		Logic.Character pupet = this.Pupet;
		if (pupet != null)
		{
			pupet.DelListener(this);
		}
		this.PupetStatus = (status as HasPuppetStatus);
		this.Pupet = this.PupetStatus.puppet;
		this.Spy = this.PupetStatus.own_character;
		Logic.Character pupet2 = this.Pupet;
		if (pupet2 != null)
		{
			pupet2.AddListener(this);
		}
		this.Vars = vars;
		this.Refresh();
	}

	// Token: 0x060027B4 RID: 10164 RVA: 0x00157F7E File Offset: 0x0015617E
	private void Init()
	{
		if (this.m_Initalized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_BribeActionIconPrototype != null)
		{
			this.m_BribeActionIconPrototype.SetActive(false);
		}
		this.m_Initalized = true;
	}

	// Token: 0x060027B5 RID: 10165 RVA: 0x00157FB4 File Offset: 0x001561B4
	private void Refresh()
	{
		UIText.SetTextKey(this.m_CharacterName, "Character.name", new Vars(this.Pupet), null);
		UIText.SetText(this.m_FlavorText, global::Defs.Localize(this.PupetStatus.def.field, "flavor1", new Vars(this.PupetStatus), null, true, true));
		UIText.SetText(this.m_FlavorActionText, global::Defs.Localize(this.PupetStatus.def.field, "flavor2", new Vars(this.PupetStatus), null, true, true));
		if (this.PupetStatus != null)
		{
			UICourtMemberBribeInfo.tempDefList.Clear();
			if (this.GetStatsActions(this.PupetStatus, UICourtMemberBribeInfo.tempDefList))
			{
				for (int i = 0; i < UICourtMemberBribeInfo.tempDefList.Count; i++)
				{
					Logic.Character spy = this.Spy;
					Action action;
					if (spy == null)
					{
						action = null;
					}
					else
					{
						Actions actions = spy.actions;
						action = ((actions != null) ? actions.Find(UICourtMemberBribeInfo.tempDefList[i]) : null);
					}
					Action action2 = action;
					if (action2 != null && action2.def.opportunity == null)
					{
						ActionVisuals actionVisuals = action2.visuals as ActionVisuals;
						if (actionVisuals != null)
						{
							action2.target = this.PupetStatus.puppet;
							if (this.m_BribeActionIconPrototype != null)
							{
								GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_BribeActionIconPrototype, this.m_BribleActions);
								gameObject.SetActive(true);
								UIActionIcon.Possess(actionVisuals, gameObject, null);
							}
							else
							{
								GameObject icon = ObjectIcon.GetIcon(action2, null, this.m_BribleActions);
								UIActionIcon uiactionIcon = (icon != null) ? icon.GetComponent<UIActionIcon>() : null;
								if (uiactionIcon != null)
								{
									uiactionIcon.ShowIfNotActive = true;
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x060027B6 RID: 10166 RVA: 0x000023FD File Offset: 0x000005FD
	private void Clear()
	{
	}

	// Token: 0x060027B7 RID: 10167 RVA: 0x0015815C File Offset: 0x0015635C
	public void Close()
	{
		this.Clear();
		Logic.Character pupet = this.Pupet;
		if (pupet != null)
		{
			pupet.DelListener(this);
		}
		global::Common.DestroyObj(base.gameObject);
	}

	// Token: 0x060027B8 RID: 10168 RVA: 0x00158181 File Offset: 0x00156381
	private void OnDestroy()
	{
		Logic.Character pupet = this.Pupet;
		if (pupet == null)
		{
			return;
		}
		pupet.DelListener(this);
	}

	// Token: 0x060027B9 RID: 10169 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnMessage(object obj, string message, object param)
	{
	}

	// Token: 0x060027BA RID: 10170 RVA: 0x00158194 File Offset: 0x00156394
	private bool GetStatsActions(Logic.Status data, List<Action.Def> result)
	{
		if (!(data.owner is Logic.Character))
		{
			return false;
		}
		this.AddActions(result, data.def);
		return true;
	}

	// Token: 0x060027BB RID: 10171 RVA: 0x001581B4 File Offset: 0x001563B4
	private void AddActions(List<Action.Def> result, Logic.Status.Def def)
	{
		if (def == null)
		{
			return;
		}
		Logic.Status.Def def2 = def.BasedOn<Logic.Status.Def>();
		this.AddActions(result, def2);
		if (def.actions == null)
		{
			return;
		}
		for (int i = 0; i < def.actions.Count; i++)
		{
			result.Add(def.actions[i]);
		}
	}

	// Token: 0x060027BC RID: 10172 RVA: 0x00158205 File Offset: 0x00156405
	public static GameObject GetPrefab()
	{
		return UICommon.GetPrefab("CourtMemberBribeInfo", null);
	}

	// Token: 0x060027BD RID: 10173 RVA: 0x00158214 File Offset: 0x00156414
	public static UICourtMemberBribeInfo Create(Logic.Status status, GameObject prototype, RectTransform parent, Vars vars)
	{
		if (prototype == null)
		{
			return null;
		}
		if (parent == null)
		{
			return null;
		}
		UICourtMemberBribeInfo orAddComponent = UnityEngine.Object.Instantiate<GameObject>(prototype, Vector3.zero, Quaternion.identity, parent).GetOrAddComponent<UICourtMemberBribeInfo>();
		orAddComponent.SetData(status, vars);
		UICommon.SetAligment(orAddComponent.transform as RectTransform, TextAnchor.MiddleCenter);
		return orAddComponent;
	}

	// Token: 0x04001AFC RID: 6908
	[UIFieldTarget("id_CharacterName")]
	private TextMeshProUGUI m_CharacterName;

	// Token: 0x04001AFD RID: 6909
	[UIFieldTarget("id_FlavorText")]
	private TextMeshProUGUI m_FlavorText;

	// Token: 0x04001AFE RID: 6910
	[UIFieldTarget("id_FlavorActionText")]
	private TextMeshProUGUI m_FlavorActionText;

	// Token: 0x04001AFF RID: 6911
	[UIFieldTarget("id_BribleActions")]
	private RectTransform m_BribleActions;

	// Token: 0x04001B00 RID: 6912
	[UIFieldTarget("id_BribeActionIconPrototype")]
	private GameObject m_BribeActionIconPrototype;

	// Token: 0x04001B05 RID: 6917
	private List<UIOpportunityIcon> m_Icons = new List<UIOpportunityIcon>();

	// Token: 0x04001B06 RID: 6918
	private bool m_Initalized;

	// Token: 0x04001B07 RID: 6919
	private static List<Action.Def> tempDefList = new List<Action.Def>();
}
