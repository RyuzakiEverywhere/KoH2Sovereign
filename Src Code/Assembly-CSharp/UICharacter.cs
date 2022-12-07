using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x020001E4 RID: 484
public class UICharacter : ObjectWindow
{
	// Token: 0x17000183 RID: 387
	// (get) Token: 0x06001CE9 RID: 7401 RVA: 0x0011183F File Offset: 0x0010FA3F
	// (set) Token: 0x06001CEA RID: 7402 RVA: 0x00111847 File Offset: 0x0010FA47
	public Logic.Character Data { get; private set; }

	// Token: 0x17000184 RID: 388
	// (get) Token: 0x06001CEB RID: 7403 RVA: 0x00111850 File Offset: 0x0010FA50
	// (set) Token: 0x06001CEC RID: 7404 RVA: 0x00111858 File Offset: 0x0010FA58
	public Vars Vars { get; private set; }

	// Token: 0x1400001B RID: 27
	// (add) Token: 0x06001CED RID: 7405 RVA: 0x00111864 File Offset: 0x0010FA64
	// (remove) Token: 0x06001CEE RID: 7406 RVA: 0x0011189C File Offset: 0x0010FA9C
	public event Action<UICharacter> OnInitialzied;

	// Token: 0x06001CEF RID: 7407 RVA: 0x001118D4 File Offset: 0x0010FAD4
	private void Start()
	{
		if (this.Data == null)
		{
			base.gameObject.SetActive(false);
		}
		if (this.m_Close != null)
		{
			this.m_Close.onClick = new BSGButton.OnClick(this.HandleClose);
		}
		if (this.m_GovernStatus != null)
		{
			this.m_GovernStatus.onClick = new BSGButton.OnClick(this.HandleGovernStatus);
		}
	}

	// Token: 0x06001CF0 RID: 7408 RVA: 0x0011193F File Offset: 0x0010FB3F
	public override void SetObject(Logic.Object obj, Vars vars = null)
	{
		base.SetObject(obj, vars);
		this.SetData(obj as Logic.Character, vars);
	}

	// Token: 0x06001CF1 RID: 7409 RVA: 0x00111958 File Offset: 0x0010FB58
	private void SetData(Logic.Character character, Vars vars)
	{
		UICommon.FindComponents(this, false);
		this.Data = character;
		this.Vars = (vars ?? new Vars(this.Data));
		this.m_Statuses = base.GetComponentInChildren<UIStatusList>();
		this.Refresh();
		this.SelectDefault();
	}

	// Token: 0x06001CF2 RID: 7410 RVA: 0x001119A6 File Offset: 0x0010FBA6
	public void SelectStatus(Logic.Status s)
	{
		if (this.m_Statuses != null)
		{
			this.m_Statuses.SelectStatus(s);
		}
	}

	// Token: 0x06001CF3 RID: 7411 RVA: 0x001119C2 File Offset: 0x0010FBC2
	public void SelectAddSkill()
	{
		this.SelectDefault();
	}

	// Token: 0x06001CF4 RID: 7412 RVA: 0x001119CA File Offset: 0x0010FBCA
	public void SelectDefault()
	{
		if (this.m_Statuses != null)
		{
			this.m_Statuses.SelectDefaultStatus();
		}
	}

	// Token: 0x06001CF5 RID: 7413 RVA: 0x001119E5 File Offset: 0x0010FBE5
	private void HandleClose(BSGButton b)
	{
		this.Close();
	}

	// Token: 0x06001CF6 RID: 7414 RVA: 0x000C4358 File Offset: 0x000C2558
	public void Close()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	// Token: 0x06001CF7 RID: 7415 RVA: 0x001119F0 File Offset: 0x0010FBF0
	public override void Refresh()
	{
		if (this.Data == null)
		{
			return;
		}
		base.gameObject.SetActive(true);
		UICharacterIcon componentInChildren = base.GetComponentInChildren<UICharacterIcon>();
		if (componentInChildren != null)
		{
			componentInChildren.SetObject(this.Data, null);
			componentInChildren.OnSelect += this.HandleCharaterOnSelect;
			componentInChildren.ShowCrest(this.Data.GetKingdom() != BaseUI.LogicKingdom());
		}
		UIText.SetTextKey(this.TMP_Name, "Character.title_name", this.Vars, null);
		this.UpdateAge();
		this.UpdateTitle();
		this.UpdateGovernStatus();
		this.UpdateStatusesVisability();
		this.UpdateUnskilledCharacterDescription();
		if (this.m_Statuses != null)
		{
			this.m_Statuses.SetBlackList(new List<string>
			{
				"GoverningStatus",
				"NotGoverningStatus",
				"BanishedGovernorStatus"
			});
			this.m_Statuses.ShowMainStatus(false);
			this.m_Statuses.OnStatusSelected += this.HandleOnStatusSelected;
			this.m_Statuses.SetData(this.Data);
		}
		if (this.OnInitialzied != null)
		{
			this.OnInitialzied(this);
		}
	}

	// Token: 0x06001CF8 RID: 7416 RVA: 0x00111B18 File Offset: 0x0010FD18
	private void HandleOnSkillSlotSelected(UISkill obj)
	{
		if (obj != null)
		{
			UIStatusList statuses = this.m_Statuses;
			if (statuses == null)
			{
				return;
			}
			statuses.SupressAutoSelect(true);
			return;
		}
		else
		{
			UIStatusList statuses2 = this.m_Statuses;
			if (statuses2 == null)
			{
				return;
			}
			statuses2.SelectDefaultStatus();
			return;
		}
	}

	// Token: 0x06001CF9 RID: 7417 RVA: 0x00111B45 File Offset: 0x0010FD45
	private void HandleOnStatusSelected(Logic.Status obj)
	{
		UIStatusList statuses = this.m_Statuses;
		if (statuses == null)
		{
			return;
		}
		statuses.SupressAutoSelect(false);
	}

	// Token: 0x06001CFA RID: 7418 RVA: 0x001119C2 File Offset: 0x0010FBC2
	private void HandleCharaterOnSelect(UICharacterIcon obj)
	{
		this.SelectDefault();
	}

	// Token: 0x06001CFB RID: 7419 RVA: 0x00111B58 File Offset: 0x0010FD58
	private void UpdateTitle()
	{
		if (this.TMP_Titile != null)
		{
			if (!string.IsNullOrEmpty(this.Data.class_title))
			{
				UIText.SetTextKey(this.TMP_Titile, this.Data.class_title, null, null);
				string text = this.Data.class_def.field.Path(false, false, '.');
				Tooltip.Get(this.TMP_Titile.gameObject, true).SetText(text + ".description", text, this.Vars);
				this.TMP_Titile.gameObject.SetActive(true);
				return;
			}
			this.TMP_Titile.gameObject.SetActive(false);
		}
	}

	// Token: 0x06001CFC RID: 7420 RVA: 0x00111C08 File Offset: 0x0010FE08
	private void UpdateAge()
	{
		if (this.Data == null)
		{
			return;
		}
		bool flag = this.MustShowAge(this.Data);
		if (flag)
		{
			string key = "Character.age." + this.Data.age.ToString();
			UIText.SetTextKey(this.TMP_Age, key, null, null);
		}
		if (this.m_TitleAgeSeparator != null)
		{
			this.m_TitleAgeSeparator.gameObject.SetActive(flag && !string.IsNullOrEmpty(this.Data.class_title));
		}
		if (this.TMP_Age != null)
		{
			this.TMP_Age.gameObject.SetActive(flag);
		}
	}

	// Token: 0x06001CFD RID: 7421 RVA: 0x00111CB4 File Offset: 0x0010FEB4
	private bool MustShowAge(Logic.Character c)
	{
		if (c == null)
		{
			return false;
		}
		if (c.IsAging() || c.can_die)
		{
			return true;
		}
		Logic.Kingdom kingdom = c.GetKingdom();
		return kingdom != null && kingdom.royalFamily != null && kingdom.royalFamily.Relatives != null && kingdom.royalFamily.Relatives.Contains(c);
	}

	// Token: 0x06001CFE RID: 7422 RVA: 0x00111D10 File Offset: 0x0010FF10
	private void UpdateStatusesVisability()
	{
		bool active = this.Data.statuses != null && this.Data.statuses.Count > 1;
		if (this.m_SecoundaryStatusesBackground != null)
		{
			this.m_SecoundaryStatusesBackground.gameObject.SetActive(active);
		}
	}

	// Token: 0x06001CFF RID: 7423 RVA: 0x00111D60 File Offset: 0x0010FF60
	private void UpdateGovernStatus()
	{
		if (this.CanGovern(this.Data))
		{
			if (this.m_GovernStatus != null)
			{
				this.m_GovernStatus.gameObject.SetActive(true);
			}
			if (this.Data.IsGovernor())
			{
				UIText.SetTextKey(this.m_GovernLabel, "Charatcer.status.governor", new Vars(this.Data), null);
			}
			else if (this.Data.IsBanishedGovernor())
			{
				UIText.SetTextKey(this.m_GovernLabel, "Charatcer.status.banished_governor", new Vars(this.Data), null);
			}
			else
			{
				UIText.SetTextKey(this.m_GovernLabel, "Charatcer.status.not_governor", new Vars(this.Data), null);
			}
			if (this.m_GovernLabel != null)
			{
				Tooltip.Get(this.m_GovernLabel.transform.parent.gameObject, true).SetDef("GovernningTooltip", new Vars(this.Data));
				return;
			}
		}
		else if (this.m_GovernStatus != null)
		{
			this.m_GovernStatus.gameObject.SetActive(false);
		}
	}

	// Token: 0x06001D00 RID: 7424 RVA: 0x00111E81 File Offset: 0x00110081
	private bool CanGovern(Logic.Character character)
	{
		return this.Data.FindStatus<NotGoverningStatus>() != null || this.Data.FindStatus<GoverningStatus>() != null || this.Data.FindStatus<BanishedGovernorStatus>() != null;
	}

	// Token: 0x06001D01 RID: 7425 RVA: 0x00111EB4 File Offset: 0x001100B4
	private void HandleGovernStatus(BSGButton b)
	{
		if (this.m_Statuses != null)
		{
			if (this.Data.IsGovernor())
			{
				this.m_Statuses.SelectStatus(this.Data.FindStatus<GoverningStatus>());
			}
			if (this.Data.IsBanishedGovernor())
			{
				this.m_Statuses.SelectStatus(this.Data.FindStatus<BanishedGovernorStatus>());
				return;
			}
			this.m_Statuses.SelectStatus(this.Data.FindStatus<NotGoverningStatus>());
		}
	}

	// Token: 0x06001D02 RID: 7426 RVA: 0x00111F2C File Offset: 0x0011012C
	private int GetReservedSkillPoints()
	{
		if (this.Data == null)
		{
			return 0;
		}
		int num = 0;
		if (this.Data.FindStatus<ReadingBookStatus>() != null)
		{
			num++;
		}
		return num;
	}

	// Token: 0x06001D03 RID: 7427 RVA: 0x00111F58 File Offset: 0x00110158
	private void UpdateUnskilledCharacterDescription()
	{
		if (this.m_NeutralDescription == null)
		{
			return;
		}
		Logic.Kingdom kingdom = this.Data.GetKingdom();
		if (kingdom == null)
		{
			this.m_NeutralDescription.gameObject.SetActive(false);
			return;
		}
		if (this.Data.CanHaveSkills())
		{
			this.m_NeutralDescription.gameObject.SetActive(false);
			return;
		}
		if (this.Data.IsQueen())
		{
			this.m_NeutralDescription.gameObject.SetActive(true);
			UIText.SetTextKey(this.m_NeutralDescription, "Character.generic_description_queen", new Vars(this.Data), null);
			return;
		}
		if (this.Data.IsPrincess())
		{
			this.m_NeutralDescription.gameObject.SetActive(true);
			UIText.SetTextKey(this.m_NeutralDescription, "Character.generic_description_princess", new Vars(this.Data), null);
			return;
		}
		if (kingdom.royalFamily.Relatives.Contains(this.Data))
		{
			this.m_NeutralDescription.gameObject.SetActive(true);
			UIText.SetTextKey(this.m_NeutralDescription, "Character.generic_description_relative", new Vars(this.Data), null);
			return;
		}
	}

	// Token: 0x06001D04 RID: 7428 RVA: 0x00112080 File Offset: 0x00110280
	protected override void HandleLogicMessage(object obj, string message, object param)
	{
		base.HandleLogicMessage(obj, message, param);
		if (message == "statuses_changed")
		{
			this.UpdateGovernStatus();
			this.UpdateStatusesVisability();
			return;
		}
		if (message == "character_age_change")
		{
			this.UpdateAge();
			return;
		}
		if (!(message == "character_class_change") && !(message == "title_changed"))
		{
			return;
		}
		this.UpdateTitle();
	}

	// Token: 0x06001D05 RID: 7429 RVA: 0x001120E8 File Offset: 0x001102E8
	public static UICharacter Create(Logic.Character character, GameObject prototype, RectTransform parent, Action<UICharacter> OnInitilized = null)
	{
		if (character == null)
		{
			Debug.LogWarning("Fail to create character Info widnow! Reson: no character data e provided.");
			return null;
		}
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
		UICharacter component = UnityEngine.Object.Instantiate<GameObject>(prototype, parent).GetComponent<UICharacter>();
		if (component == null)
		{
			Debug.LogWarning("Fail to create character Info widnow! Reson: Incompatible prototype.");
			return null;
		}
		if (OnInitilized != null)
		{
			component.OnInitialzied += OnInitilized;
		}
		component.gameObject.SetActive(true);
		component.SetObject(character, null);
		component.gameObject.transform.localPosition = Vector3.zero;
		return component;
	}

	// Token: 0x040012CE RID: 4814
	[UIFieldTarget("id_Name")]
	private TextMeshProUGUI TMP_Name;

	// Token: 0x040012CF RID: 4815
	[UIFieldTarget("id_Title")]
	private TextMeshProUGUI TMP_Titile;

	// Token: 0x040012D0 RID: 4816
	[UIFieldTarget("id_Age")]
	private TextMeshProUGUI TMP_Age;

	// Token: 0x040012D1 RID: 4817
	[UIFieldTarget("id_TitleAgeSeparator")]
	private GameObject m_TitleAgeSeparator;

	// Token: 0x040012D2 RID: 4818
	[UIFieldTarget("id_GovernStatus")]
	private BSGButton m_GovernStatus;

	// Token: 0x040012D3 RID: 4819
	[UIFieldTarget("id_GovernLabel")]
	private TextMeshProUGUI m_GovernLabel;

	// Token: 0x040012D4 RID: 4820
	[UIFieldTarget("id_Close")]
	private BSGButton m_Close;

	// Token: 0x040012D5 RID: 4821
	[UIFieldTarget("id_Group_SkillSelection")]
	private GameObject m_Group_SkillSelection;

	// Token: 0x040012D6 RID: 4822
	[UIFieldTarget("id_NeutralDescription")]
	private TextMeshProUGUI m_NeutralDescription;

	// Token: 0x040012D7 RID: 4823
	[UIFieldTarget("id_SecoundaryStatusesBackground")]
	private GameObject m_SecoundaryStatusesBackground;

	// Token: 0x040012D8 RID: 4824
	private UIStatusList m_Statuses;
}
