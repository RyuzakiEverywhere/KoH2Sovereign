using System;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200028F RID: 655
public class UICourtMemberRankIcon : Hotspot, IListener
{
	// Token: 0x170001E6 RID: 486
	// (get) Token: 0x0600282C RID: 10284 RVA: 0x00159770 File Offset: 0x00157970
	// (set) Token: 0x0600282D RID: 10285 RVA: 0x00159778 File Offset: 0x00157978
	public Logic.Character Character { get; private set; }

	// Token: 0x170001E7 RID: 487
	// (get) Token: 0x0600282E RID: 10286 RVA: 0x00159781 File Offset: 0x00157981
	// (set) Token: 0x0600282F RID: 10287 RVA: 0x00159789 File Offset: 0x00157989
	public Vars Vars { get; private set; }

	// Token: 0x170001E8 RID: 488
	// (get) Token: 0x06002830 RID: 10288 RVA: 0x00159792 File Offset: 0x00157992
	public bool Selected
	{
		get
		{
			return this.m_Selected;
		}
	}

	// Token: 0x06002831 RID: 10289 RVA: 0x0015979A File Offset: 0x0015799A
	public void SetData(Logic.Character character, Vars vars)
	{
		this.Init();
		if (this.Character != character)
		{
			this.CloseSubSelections();
			this.RemoveListeners();
			this.Character = character;
			this.AddListeners();
		}
		this.Vars = vars;
		this.Refresh();
		this.UpdateHighlight();
	}

	// Token: 0x06002832 RID: 10290 RVA: 0x001597D7 File Offset: 0x001579D7
	private void Refresh()
	{
		if (this.Character == null)
		{
			return;
		}
		Tooltip.Get(base.gameObject, true).SetDef("CharacterLevelTooltip", new Vars(this.Character));
		this.UpdateRankLabel();
	}

	// Token: 0x06002833 RID: 10291 RVA: 0x00159810 File Offset: 0x00157A10
	private void UpdateRankLabel()
	{
		if (this.m_ClassLevel != null)
		{
			UIText.SetText(this.m_ClassLevel, global::Character.GetLevelText(this.Character));
		}
		Logic.Character character = this.Character;
		bool active = character != null && character.GetClassLevel() == 0;
		if (this.m_NewSkill != null)
		{
			this.m_NewSkill.gameObject.SetActive(active);
		}
	}

	// Token: 0x06002834 RID: 10292 RVA: 0x00159876 File Offset: 0x00157A76
	private void Init()
	{
		if (this.m_Initalized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_Initalized = true;
	}

	// Token: 0x06002835 RID: 10293 RVA: 0x0015988F File Offset: 0x00157A8F
	public void Enabled(bool enabled)
	{
		if (this.m_Enabled == enabled)
		{
			return;
		}
		this.m_Enabled = enabled;
		this.UpdateHighlight();
	}

	// Token: 0x06002836 RID: 10294 RVA: 0x001598A8 File Offset: 0x00157AA8
	public void Select(bool selected)
	{
		if (this.m_Selected == selected)
		{
			return;
		}
		this.m_Selected = selected;
		this.UpdateHighlight();
	}

	// Token: 0x06002837 RID: 10295 RVA: 0x001598C4 File Offset: 0x00157AC4
	public void ToggleSkillWindow(bool open)
	{
		if (this.Character != null)
		{
			if (this.skillsPopup != null)
			{
				if (open)
				{
					this.skillsPopup.SetData(this.Character, null);
					return;
				}
				this.CloseSubSelections();
				return;
			}
			else if (open)
			{
				this.skillsPopup = UICourtMemberSkills.Create(this.Character, UICourtMemberSkills.GetPrefab(), this.GetPopupContainer(), null);
				UICommon.SetAligment(this.skillsPopup.transform as RectTransform, TextAnchor.UpperCenter);
			}
		}
	}

	// Token: 0x06002838 RID: 10296 RVA: 0x0015993C File Offset: 0x00157B3C
	private RectTransform GetPopupContainer()
	{
		if (this.m_PopupContianer != null)
		{
			return this.m_PopupContianer;
		}
		WorldUI worldUI = WorldUI.Get();
		if (worldUI == null)
		{
			return null;
		}
		GameObject gameObject = global::Common.FindChildByName(worldUI.gameObject, "id_MessageContainer", true, true);
		return ((gameObject != null) ? gameObject.transform : null) as RectTransform;
	}

	// Token: 0x06002839 RID: 10297 RVA: 0x00159992 File Offset: 0x00157B92
	public void SetPopupContainer(RectTransform m_SubSelectionContainer)
	{
		this.m_PopupContianer = m_SubSelectionContainer;
	}

	// Token: 0x0600283A RID: 10298 RVA: 0x0015999B File Offset: 0x00157B9B
	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x0600283B RID: 10299 RVA: 0x001599AA File Offset: 0x00157BAA
	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x0600283C RID: 10300 RVA: 0x001599B9 File Offset: 0x00157BB9
	public override void OnClick(PointerEventData e)
	{
		if (!this.m_Enabled)
		{
			return;
		}
		base.OnClick(e);
		if (this.OnSelected != null)
		{
			this.OnSelected(this, e);
		}
	}

	// Token: 0x0600283D RID: 10301 RVA: 0x001599E0 File Offset: 0x00157BE0
	public void UpdateHighlight()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.m_GroupSelected != null)
		{
			this.m_GroupSelected.gameObject.SetActive(this.m_Selected);
		}
	}

	// Token: 0x0600283E RID: 10302 RVA: 0x00159A0E File Offset: 0x00157C0E
	public void Close()
	{
		this.RemoveListeners();
		this.CloseSubSelections();
		this.m_PopupContianer = null;
		global::Common.DestroyObj(base.gameObject);
	}

	// Token: 0x0600283F RID: 10303 RVA: 0x00159A2E File Offset: 0x00157C2E
	public void CloseSubSelections()
	{
		if (this.skillsPopup != null)
		{
			this.skillsPopup.Close();
			this.skillsPopup = null;
		}
	}

	// Token: 0x06002840 RID: 10304 RVA: 0x00159A50 File Offset: 0x00157C50
	private void AddListeners()
	{
		Logic.Character character = this.Character;
		if (character != null)
		{
			character.AddListener(this);
		}
		Logic.Character character2 = this.Character;
		if (character2 == null)
		{
			return;
		}
		Logic.Kingdom kingdom = character2.GetKingdom();
		if (kingdom == null)
		{
			return;
		}
		Logic.RoyalFamily royalFamily = kingdom.royalFamily;
		if (royalFamily == null)
		{
			return;
		}
		royalFamily.AddListener(this);
	}

	// Token: 0x06002841 RID: 10305 RVA: 0x00159A89 File Offset: 0x00157C89
	private void RemoveListeners()
	{
		Logic.Character character = this.Character;
		if (character != null)
		{
			character.DelListener(this);
		}
		Logic.Character character2 = this.Character;
		if (character2 == null)
		{
			return;
		}
		Logic.Kingdom kingdom = character2.GetKingdom();
		if (kingdom == null)
		{
			return;
		}
		Logic.RoyalFamily royalFamily = kingdom.royalFamily;
		if (royalFamily == null)
		{
			return;
		}
		royalFamily.DelListener(this);
	}

	// Token: 0x06002842 RID: 10306 RVA: 0x00159AC4 File Offset: 0x00157CC4
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "rank_changed" || message == "kingdom_changed" || message == "character_class_change" || message == "royal_new_sovereign" || message == "skills_changed")
		{
			this.UpdateRankLabel();
		}
	}

	// Token: 0x06002843 RID: 10307 RVA: 0x00159B18 File Offset: 0x00157D18
	private void OnDestroy()
	{
		this.RemoveListeners();
	}

	// Token: 0x06002844 RID: 10308 RVA: 0x00159B20 File Offset: 0x00157D20
	public static GameObject GetPrefab()
	{
		return UICommon.GetPrefab("CourtMemberRankActionIcon", null);
	}

	// Token: 0x06002845 RID: 10309 RVA: 0x00158905 File Offset: 0x00156B05
	public static UICourtMemberRankIcon Posses(Logic.Character character, GameObject gameObject, Vars vars)
	{
		if (gameObject == null)
		{
			return null;
		}
		UICourtMemberRankIcon orAddComponent = gameObject.GetOrAddComponent<UICourtMemberRankIcon>();
		orAddComponent.SetData(character, vars);
		return orAddComponent;
	}

	// Token: 0x06002846 RID: 10310 RVA: 0x00159B2D File Offset: 0x00157D2D
	public static UICourtMemberRankIcon Create(Logic.Character character, GameObject prototype, RectTransform parent, Vars vars)
	{
		if (prototype == null)
		{
			return null;
		}
		if (parent == null)
		{
			return null;
		}
		UICourtMemberRankIcon orAddComponent = UnityEngine.Object.Instantiate<GameObject>(prototype, Vector3.zero, Quaternion.identity, parent).GetOrAddComponent<UICourtMemberRankIcon>();
		orAddComponent.SetData(character, vars);
		return orAddComponent;
	}

	// Token: 0x04001B3D RID: 6973
	[UIFieldTarget("id_Icon")]
	private Image m_Icon;

	// Token: 0x04001B3E RID: 6974
	[UIFieldTarget("id_ClassLevel")]
	private TextMeshProUGUI m_ClassLevel;

	// Token: 0x04001B3F RID: 6975
	[UIFieldTarget("id_Background")]
	private Image m_Background;

	// Token: 0x04001B40 RID: 6976
	[UIFieldTarget("id_NewSkill")]
	private Image m_NewSkill;

	// Token: 0x04001B41 RID: 6977
	[UIFieldTarget("id_GroupSelected")]
	private GameObject m_GroupSelected;

	// Token: 0x04001B44 RID: 6980
	private bool m_Selected;

	// Token: 0x04001B45 RID: 6981
	private bool m_Enabled = true;

	// Token: 0x04001B46 RID: 6982
	private bool m_Initalized;

	// Token: 0x04001B47 RID: 6983
	private UICourtMemberSkills skillsPopup;

	// Token: 0x04001B48 RID: 6984
	public Action<UICourtMemberRankIcon, PointerEventData> OnSelected;

	// Token: 0x04001B49 RID: 6985
	private RectTransform m_PopupContianer;
}
