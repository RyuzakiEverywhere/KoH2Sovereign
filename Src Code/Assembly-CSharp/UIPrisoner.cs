using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x02000293 RID: 659
public class UIPrisoner : MonoBehaviour, IListener
{
	// Token: 0x170001F2 RID: 498
	// (get) Token: 0x0600289B RID: 10395 RVA: 0x0015B6E6 File Offset: 0x001598E6
	// (set) Token: 0x0600289C RID: 10396 RVA: 0x0015B6EE File Offset: 0x001598EE
	public Logic.Character Prisoner { get; private set; }

	// Token: 0x170001F3 RID: 499
	// (get) Token: 0x0600289D RID: 10397 RVA: 0x0015B6F7 File Offset: 0x001598F7
	// (set) Token: 0x0600289E RID: 10398 RVA: 0x0015B6FF File Offset: 0x001598FF
	public Vars Vars { get; private set; }

	// Token: 0x0600289F RID: 10399 RVA: 0x0015B708 File Offset: 0x00159908
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_Initialized = true;
	}

	// Token: 0x060028A0 RID: 10400 RVA: 0x0015B724 File Offset: 0x00159924
	public void SetData(Logic.Character character, Vars vars)
	{
		this.Init();
		Logic.Character prisoner = this.Prisoner;
		if (prisoner != null)
		{
			prisoner.DelListener(this);
		}
		this.Prisoner = character;
		this.Vars = (vars ?? new Vars(this.Prisoner));
		Logic.Character prisoner2 = this.Prisoner;
		if (prisoner2 != null)
		{
			prisoner2.AddListener(this);
		}
		this.BuildStatic();
		this.UpdateFlavorText();
		this.UpdateActions();
	}

	// Token: 0x060028A1 RID: 10401 RVA: 0x0015B78F File Offset: 0x0015998F
	private void OnSelectCharacterIcon(UICharacterIcon icon)
	{
		BaseUI.PlayVoiceEvent("character_voice:greet_enemy_prisoner", icon.Data);
	}

	// Token: 0x060028A2 RID: 10402 RVA: 0x0015B7A4 File Offset: 0x001599A4
	private void BuildStatic()
	{
		UICharacterIcon componentInChildren = base.GetComponentInChildren<UICharacterIcon>();
		if (componentInChildren != null)
		{
			componentInChildren.SetObject(this.Prisoner, null);
			componentInChildren.ShowCrest(true);
			if (this.Prisoner != null)
			{
				componentInChildren.OnSelect += this.OnSelectCharacterIcon;
			}
			else
			{
				componentInChildren.OnSelect -= this.OnSelectCharacterIcon;
			}
		}
		if (this.TMP_Name != null)
		{
			UIText.SetTextKey(this.TMP_Name, "Character.title_name", this.Vars, null);
		}
		if (this.TMP_Origin != null)
		{
			UIText.SetTextKey(this.TMP_Origin, "Kingdom.name", new Vars(this.Prisoner.GetKingdom()), null);
		}
	}

	// Token: 0x060028A3 RID: 10403 RVA: 0x0015B85C File Offset: 0x00159A5C
	private void Update()
	{
		if (this.m_Invalidate)
		{
			this.UpdateActions();
			this.UpdateFlavorText();
			this.m_Invalidate = false;
		}
	}

	// Token: 0x060028A4 RID: 10404 RVA: 0x0015B87C File Offset: 0x00159A7C
	private void UpdateActions()
	{
		if (this.m_KingdomActions == null)
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			return;
		}
		this.m_KingdomActions.GetOrAddComponent<UIPrisoner.KingdomActions>().SetData(kingdom, this.Prisoner, this.Vars);
	}

	// Token: 0x060028A5 RID: 10405 RVA: 0x0015B8C0 File Offset: 0x00159AC0
	private void UpdateFlavorText()
	{
		if (this.TMP_FlavorText != null)
		{
			if (this.Prisoner != null && this.Prisoner.IsOwnStance(this.Prisoner.prison_kingdom))
			{
				UIText.SetTextKey(this.TMP_FlavorText, "RoyalDungeon.Prisoner.descriptionOwnKingdom", this.Prisoner, null);
				return;
			}
			if (this.Prisoner != null && !this.Prisoner.GetKingdom().IsRegular())
			{
				if (this.Prisoner.GetSpecialCourtKingdom() == this.Prisoner.prison_kingdom)
				{
					UIText.SetTextKey(this.TMP_FlavorText, "RoyalDungeon.Prisoner.descriptionNoKingdomOwnCourt", this.Prisoner, null);
					return;
				}
				UIText.SetTextKey(this.TMP_FlavorText, "RoyalDungeon.Prisoner.descriptionNoKingdom", this.Prisoner, null);
				return;
			}
			else
			{
				UIText.SetTextKey(this.TMP_FlavorText, "RoyalDungeon.Prisoner.descriptionOtherKingdom", this.Prisoner, null);
			}
		}
	}

	// Token: 0x060028A6 RID: 10406 RVA: 0x0015B98F File Offset: 0x00159B8F
	private void OnDestroy()
	{
		Logic.Character prisoner = this.Prisoner;
		if (prisoner == null)
		{
			return;
		}
		prisoner.DelListener(this);
	}

	// Token: 0x060028A7 RID: 10407 RVA: 0x0015B9A4 File Offset: 0x00159BA4
	public static UIPrisoner Create(Logic.Character character, GameObject prototype, RectTransform parent)
	{
		if (prototype == null)
		{
			Debug.LogWarning("Fail to create character Info widnow! Reson: no prototype provided.");
			return null;
		}
		if (parent == null)
		{
			Debug.LogWarning("Fail to create character Info widnow! Reson: no parent provided.");
			return null;
		}
		UIPrisoner orAddComponent = UnityEngine.Object.Instantiate<GameObject>(prototype, parent).GetOrAddComponent<UIPrisoner>();
		orAddComponent.gameObject.SetActive(true);
		orAddComponent.SetData(character, null);
		return orAddComponent;
	}

	// Token: 0x060028A8 RID: 10408 RVA: 0x0015B9FC File Offset: 0x00159BFC
	public void OnMessage(object obj, string message, object param)
	{
		if (obj is Logic.Character && (message == "destroying" || message == "finishing"))
		{
			this.SetData(null, null);
			return;
		}
		if (message == "force_refresh_actions" || message == "kingdom_changed")
		{
			this.m_Invalidate = true;
		}
	}

	// Token: 0x04001B74 RID: 7028
	[UIFieldTarget("id_KingActions")]
	private GameObject m_KingdomActions;

	// Token: 0x04001B75 RID: 7029
	[UIFieldTarget("id_Name")]
	private TextMeshProUGUI TMP_Name;

	// Token: 0x04001B76 RID: 7030
	[UIFieldTarget("id_Origin")]
	private TextMeshProUGUI TMP_Origin;

	// Token: 0x04001B77 RID: 7031
	[UIFieldTarget("id_FlavorText")]
	private TextMeshProUGUI TMP_FlavorText;

	// Token: 0x04001B7A RID: 7034
	private bool m_Initialized;

	// Token: 0x04001B7B RID: 7035
	private bool m_Invalidate;

	// Token: 0x020007E6 RID: 2022
	protected internal class KingdomActions : MonoBehaviour, IListener
	{
		// Token: 0x17000614 RID: 1556
		// (get) Token: 0x06004ED1 RID: 20177 RVA: 0x002336D3 File Offset: 0x002318D3
		// (set) Token: 0x06004ED2 RID: 20178 RVA: 0x002336DB File Offset: 0x002318DB
		public Logic.Kingdom Kingdom { get; private set; }

		// Token: 0x17000615 RID: 1557
		// (get) Token: 0x06004ED3 RID: 20179 RVA: 0x002336E4 File Offset: 0x002318E4
		// (set) Token: 0x06004ED4 RID: 20180 RVA: 0x002336EC File Offset: 0x002318EC
		public Logic.Character Prisoner { get; private set; }

		// Token: 0x17000616 RID: 1558
		// (get) Token: 0x06004ED5 RID: 20181 RVA: 0x002336F5 File Offset: 0x002318F5
		// (set) Token: 0x06004ED6 RID: 20182 RVA: 0x002336FD File Offset: 0x002318FD
		public Vars Vars { get; private set; }

		// Token: 0x06004ED7 RID: 20183 RVA: 0x00233708 File Offset: 0x00231908
		public void SetData(Logic.Kingdom kingdom, Logic.Character prisoner, Vars vars)
		{
			UICommon.FindComponents(this, false);
			Logic.Kingdom kingdom2 = this.Kingdom;
			if (kingdom2 != null)
			{
				kingdom2.DelListener(this);
			}
			this.Kingdom = kingdom;
			this.Prisoner = prisoner;
			this.Vars = (vars ?? new Vars(this.Prisoner));
			Logic.Kingdom kingdom3 = this.Kingdom;
			if (kingdom3 != null)
			{
				kingdom3.AddListener(this);
			}
			this.Refresh();
		}

		// Token: 0x06004ED8 RID: 20184 RVA: 0x0023376F File Offset: 0x0023196F
		private void Update()
		{
			Logic.Character prisoner = this.Prisoner;
		}

		// Token: 0x06004ED9 RID: 20185 RVA: 0x00233778 File Offset: 0x00231978
		public void Refresh()
		{
			if (this.Kingdom == null)
			{
				return;
			}
			if (this.Prisoner == null)
			{
				return;
			}
			this.PopulateActions();
		}

		// Token: 0x06004EDA RID: 20186 RVA: 0x00233794 File Offset: 0x00231994
		private void PopulateActions()
		{
			if (this.m_ActionsContainer == null)
			{
				return;
			}
			UICommon.DeleteChildren(this.m_ActionsContainer);
			this.m_ActiveActions.Clear();
			Logic.Kingdom kingdom = BaseUI.LogicKingdom();
			if (kingdom == null)
			{
				return;
			}
			if (kingdom.actions == null)
			{
				return;
			}
			for (int i = 0; i < kingdom.actions.Count; i++)
			{
				Action action = kingdom.actions[i];
				if (action is PrisonAction && action.ValidateTarget(this.Prisoner))
				{
					action.target = this.Prisoner;
					string a = action.Validate(false);
					action.target = null;
					if (a == "ok" || a == "_cooldown")
					{
						GameObject icon = ObjectIcon.GetIcon(action, this.Vars, this.m_ActionsContainer);
						if (icon != null)
						{
							UIActionIcon component = icon.GetComponent<UIActionIcon>();
							if (component != null)
							{
								component.SetOverrideTarget(this.Prisoner);
								this.m_ActiveActions.Add(component);
							}
						}
					}
				}
			}
		}

		// Token: 0x06004EDB RID: 20187 RVA: 0x002338A3 File Offset: 0x00231AA3
		private void OnDestroy()
		{
			Logic.Kingdom kingdom = this.Kingdom;
			if (kingdom == null)
			{
				return;
			}
			kingdom.DelListener(this);
		}

		// Token: 0x06004EDC RID: 20188 RVA: 0x002338B6 File Offset: 0x00231AB6
		public void OnMessage(object obj, string message, object param)
		{
			if (message == "destroying" || message == "finishing")
			{
				(obj as Logic.Object).DelListener(this);
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
		}

		// Token: 0x04003CDD RID: 15581
		[UIFieldTarget("id_Actions")]
		private RectTransform m_ActionsContainer;

		// Token: 0x04003CE1 RID: 15585
		private List<UIActionIcon> m_ActiveActions = new List<UIActionIcon>(10);
	}
}
