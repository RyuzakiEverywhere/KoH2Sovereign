using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x020001F2 RID: 498
public class UINewborn : MonoBehaviour
{
	// Token: 0x1700018D RID: 397
	// (get) Token: 0x06001E06 RID: 7686 RVA: 0x00118A23 File Offset: 0x00116C23
	// (set) Token: 0x06001E07 RID: 7687 RVA: 0x00118A2B File Offset: 0x00116C2B
	public Logic.Character Data { get; private set; }

	// Token: 0x06001E08 RID: 7688 RVA: 0x00118A34 File Offset: 0x00116C34
	private void Start()
	{
		this.ExtractData();
		this.m_Started = true;
	}

	// Token: 0x06001E09 RID: 7689 RVA: 0x00118A43 File Offset: 0x00116C43
	private void OnEnable()
	{
		if (this.m_Started)
		{
			this.m_DataExtracted = false;
		}
	}

	// Token: 0x06001E0A RID: 7690 RVA: 0x00118A54 File Offset: 0x00116C54
	private void LateUpdate()
	{
		if (!this.m_DataExtracted)
		{
			this.ExtractData();
		}
	}

	// Token: 0x06001E0B RID: 7691 RVA: 0x00118A64 File Offset: 0x00116C64
	private void ExtractData()
	{
		this.m_DataExtracted = true;
		MessageWnd component = base.GetComponent<MessageWnd>();
		this.field = component.def_field;
		if (component)
		{
			Logic.Character character = component.vars.Get<Logic.Character>("obj", null);
			if (character != null)
			{
				this.SetData(character, component.vars);
			}
		}
	}

	// Token: 0x06001E0C RID: 7692 RVA: 0x00118AB8 File Offset: 0x00116CB8
	private void Update()
	{
		if (this.Data != null)
		{
			if (this.Data.IsAlive() & (this.Data.IsPrince() || this.Data.IsPrincess()))
			{
				this.UpdateFeastButton();
				return;
			}
			this.SetData(null, null);
		}
	}

	// Token: 0x06001E0D RID: 7693 RVA: 0x00118B08 File Offset: 0x00116D08
	public void SetData(Logic.Character characterData, Vars vars)
	{
		UICommon.FindComponents(this, false);
		this.Data = characterData;
		this.Vars = vars;
		if (this.Data == null || this.Vars == null)
		{
			this.Close();
			return;
		}
		this.BuildActions();
		if (this.Icon_Child != null)
		{
			this.Icon_Child.SetObject(this.Data, null);
		}
		UICharacter component = base.GetComponent<UICharacter>();
		if (component != null)
		{
			component.SetObject(this.Data, vars);
		}
		if (this.m_Caption != null)
		{
			UIText.SetText(this.m_Caption, this.field, "caption", vars, null);
		}
		if (this.m_Description != null)
		{
			UIText.SetText(this.m_Description, this.field, "body", vars, null);
		}
		if (this.m_CharacterAge != null)
		{
			UIText.SetTextKey(this.m_CharacterAge, "CharacterTooltip.character_age", this.Data, null);
		}
		if (this.KingomdShield != null)
		{
			this.KingomdShield.SetObject(this.Data.GetKingdom(), null);
		}
		UIText.SetText(this.m_ClassSelectFlavor, this.field, "class_select", vars, null);
		if (this.m_ButtonEducate != null)
		{
			this.m_ButtonEducate.onClick = new BSGButton.OnClick(this.HandleEducate);
			UIText.SetText(this.m_ButtonEducate.gameObject, "id_Text", this.field, "button_educate", vars, null);
		}
		if (this.m_ButtonEducateAndFeast != null)
		{
			this.m_ButtonEducateAndFeast.onClick = new BSGButton.OnClick(this.HandleEducateAndFeats);
			UIText.SetText(this.m_ButtonEducateAndFeast.gameObject, "id_Text", this.field, "button_educate_and_feast", vars, null);
			Tooltip.Get(this.m_ButtonEducateAndFeast.gameObject, true).SetObj(this.holdAFeastAction, null, null);
		}
		if (this.m_Feast != null)
		{
			this.m_Feast.onClick = new BSGButton.OnClick(this.HandleFeast);
			UIText.SetText(this.m_Feast.gameObject, "id_Text", this.field, "button_feast", vars, null);
			Tooltip.Get(this.m_Feast.gameObject, true).SetObj(this.holdAFeastAction, null, null);
		}
		if (this.m_ButtonClose != null)
		{
			this.m_ButtonClose.onClick = new BSGButton.OnClick(this.HandleClose);
			UIText.SetText(this.m_ButtonClose.gameObject, "id_Text", this.field, "button_close", vars, null);
		}
		if (this.m_OpenRoyalFamily != null)
		{
			this.m_OpenRoyalFamily.onClick = new BSGButton.OnClick(this.HanldeOpenFamily);
			UIText.SetText(this.m_OpenRoyalFamily.gameObject, "id_Text", this.field, "button_family", vars, null);
		}
		if (this.m_KingAbilities != null)
		{
			this.m_KingAbilities.SetData(this.Data, null);
		}
		this.PopulateClassSelector();
		this.SelectClass(null);
		this.UpdateEducateButton();
		this.UpdateFeastButton();
	}

	// Token: 0x06001E0E RID: 7694 RVA: 0x00118E0A File Offset: 0x0011700A
	private void BuildActions()
	{
		this.holdAFeastAction = (this.Data.GetKingdom().actions.Find("HoldAFeastAction") as HoldAFeastAction);
	}

	// Token: 0x06001E0F RID: 7695 RVA: 0x00118E34 File Offset: 0x00117034
	private void PopulateClassSelector()
	{
		if (this.Data == null)
		{
			return;
		}
		if (this.ClassSelectButtonPrototype == null)
		{
			return;
		}
		if (this.m_ClassSelectContainer == null)
		{
			return;
		}
		UICommon.DeleteChildren(this.m_ClassSelectContainer);
		List<CharacterClass.Def> characterClassDefList = CharacterFactory.GetCharacterClassDefList(this.Data.game);
		for (int i = 0; i < characterClassDefList.Count; i++)
		{
			CharacterClass.Def charatcerClass = characterClassDefList[i];
			Action action = delegate()
			{
				this.SelectClass(charatcerClass);
			};
			Vars vars = new Vars(charatcerClass);
			vars.Set<string>("tooltip", "CharacterClassTooltip");
			vars.Set<Sprite>("sprite", global::Defs.GetObj<Sprite>(charatcerClass.field, "icon_class_select", null));
			GameObject prototype = (this.Data.prefered_class_def != null && this.Data.prefered_class_def == charatcerClass) ? this.ClassSelectButtonPrototypeExtended : this.ClassSelectButtonPrototype;
			this.classIconsList.Add(UIGenericActionIcon.Create(action, prototype, this.m_ClassSelectContainer, vars));
		}
	}

	// Token: 0x06001E10 RID: 7696 RVA: 0x00118F54 File Offset: 0x00117154
	private void SelectClass(CharacterClass.Def cls)
	{
		if (this.selectedClass == cls)
		{
			return;
		}
		this.selectedClass = cls;
		for (int i = 0; i < this.classIconsList.Count; i++)
		{
			UIGenericActionIcon uigenericActionIcon = this.classIconsList[i];
			uigenericActionIcon.Select(uigenericActionIcon.Vars.obj.obj_val as CharacterClass.Def == cls);
		}
		this.UpdateEducateButton();
		this.UpdateFeastButton();
	}

	// Token: 0x06001E11 RID: 7697 RVA: 0x00118FC0 File Offset: 0x001171C0
	private void UpdateEducateButton()
	{
		if (this.m_ButtonEducate != null)
		{
			bool flag = this.selectedClass != null;
			this.m_ButtonEducate.Enable(flag, false);
			TextMeshProUGUI textMeshProUGUI = global::Common.FindChildComponent<TextMeshProUGUI>(this.m_ButtonEducate.gameObject, "id_Text");
			if (textMeshProUGUI != null)
			{
				textMeshProUGUI.color = (flag ? Color.black : Color.grey);
			}
		}
	}

	// Token: 0x06001E12 RID: 7698 RVA: 0x00119028 File Offset: 0x00117228
	private void UpdateFeastButton()
	{
		bool flag;
		if (this.Data != null && (this.Data.IsPrincess() || this.selectedClass != null))
		{
			HoldAFeastAction holdAFeastAction = this.holdAFeastAction;
			flag = (((holdAFeastAction != null) ? holdAFeastAction.Validate(false) : null) == "ok");
		}
		else
		{
			flag = false;
		}
		bool flag2 = flag;
		if (this.m_ButtonEducateAndFeast != null)
		{
			this.m_ButtonEducateAndFeast.Enable(flag2, false);
			if (this.m_EducateAndFeastLabel == null)
			{
				this.m_EducateAndFeastLabel = global::Common.FindChildComponent<TextMeshProUGUI>(this.m_ButtonEducateAndFeast.gameObject, "id_Text");
			}
			if (this.m_EducateAndFeastLabel != null)
			{
				this.m_EducateAndFeastLabel.color = (flag2 ? Color.black : Color.grey);
			}
		}
		if (this.m_Feast != null)
		{
			this.m_Feast.Enable(flag2, false);
			if (this.m_FeastLabel == null)
			{
				this.m_FeastLabel = global::Common.FindChildComponent<TextMeshProUGUI>(this.m_Feast.gameObject, "id_Text");
			}
			if (this.m_FeastLabel != null)
			{
				this.m_FeastLabel.color = (flag2 ? Color.black : Color.grey);
			}
		}
	}

	// Token: 0x06001E13 RID: 7699 RVA: 0x0011914B File Offset: 0x0011734B
	private void HandleEducate(BSGButton b)
	{
		if (this.selectedClass == null)
		{
			return;
		}
		this.Data.SetClass(this.selectedClass, true);
		BaseUI.PlaySoundEvent(Logic.Character.GetSoundEffect(this.selectedClass, "select_prince_class_sound_effect"), null);
		this.Close();
	}

	// Token: 0x06001E14 RID: 7700 RVA: 0x00119184 File Offset: 0x00117384
	private void HandleEducateAndFeats(BSGButton b)
	{
		if (this.selectedClass == null)
		{
			return;
		}
		this.Data.SetClass(this.selectedClass, true);
		BaseUI.PlaySoundEvent(Logic.Character.GetSoundEffect(this.selectedClass, "select_prince_class_sound_effect"), null);
		this.Close();
		if (this.Data == null)
		{
			return;
		}
		HoldAFeastAction holdAFeastAction = this.holdAFeastAction;
		if (holdAFeastAction != null)
		{
			holdAFeastAction.Execute(this.Data);
		}
		this.Close();
	}

	// Token: 0x06001E15 RID: 7701 RVA: 0x001191EF File Offset: 0x001173EF
	private void HandleClose(BSGButton b)
	{
		this.Close();
	}

	// Token: 0x06001E16 RID: 7702 RVA: 0x001191F7 File Offset: 0x001173F7
	private void HanldeOpenFamily(BSGButton b)
	{
		UIRoyalFamily.ToggleOpen(this.Data.GetKingdom());
	}

	// Token: 0x06001E17 RID: 7703 RVA: 0x0011920C File Offset: 0x0011740C
	private void HandleFeast(BSGButton b)
	{
		if (this.Data == null)
		{
			return;
		}
		if (this.Data.IsPrince() && this.selectedClass != null)
		{
			this.Data.SetClass(this.selectedClass, true);
			BaseUI.PlaySoundEvent(Logic.Character.GetSoundEffect(this.selectedClass, "select_prince_class_sound_effect"), null);
		}
		HoldAFeastAction holdAFeastAction = this.holdAFeastAction;
		if (holdAFeastAction != null)
		{
			holdAFeastAction.Execute(this.Data);
		}
		this.Close();
	}

	// Token: 0x06001E18 RID: 7704 RVA: 0x00119280 File Offset: 0x00117480
	private void Close()
	{
		MessageWnd component = base.GetComponent<MessageWnd>();
		if (component != null)
		{
			component.CloseAndDismiss(true);
		}
	}

	// Token: 0x06001E19 RID: 7705 RVA: 0x001192A4 File Offset: 0x001174A4
	private void OnDestroy()
	{
		if (this.classIcons != null)
		{
			for (int i = 0; i < this.classIcons.Count; i++)
			{
				UnityEngine.Object.Destroy(this.classIcons[i].gameObject);
			}
			this.classIcons.Clear();
		}
	}

	// Token: 0x040013A9 RID: 5033
	[UIFieldTarget("id_IconNewborn")]
	private UICharacterIcon Icon_Child;

	// Token: 0x040013AA RID: 5034
	[UIFieldTarget("id_Crest")]
	private UIKingdomIcon KingomdShield;

	// Token: 0x040013AB RID: 5035
	[UIFieldTarget("id_ClassContainer")]
	private RectTransform m_ClassSelectContainer;

	// Token: 0x040013AC RID: 5036
	[UIFieldTarget("id_EventTitile")]
	private TextMeshProUGUI m_Caption;

	// Token: 0x040013AD RID: 5037
	[UIFieldTarget("id_EventFlavorText")]
	private TextMeshProUGUI m_Description;

	// Token: 0x040013AE RID: 5038
	[UIFieldTarget("id_CharacterAge")]
	private TextMeshProUGUI m_CharacterAge;

	// Token: 0x040013AF RID: 5039
	[UIFieldTarget("id_ClassSelectFlavor")]
	private TextMeshProUGUI m_ClassSelectFlavor;

	// Token: 0x040013B0 RID: 5040
	[UIFieldTarget("id_Educate")]
	private BSGButton m_ButtonEducate;

	// Token: 0x040013B1 RID: 5041
	[UIFieldTarget("id_EducateAndFeast")]
	private BSGButton m_ButtonEducateAndFeast;

	// Token: 0x040013B2 RID: 5042
	[UIFieldTarget("id_Feast")]
	private BSGButton m_Feast;

	// Token: 0x040013B3 RID: 5043
	[UIFieldTarget("id_MessageWindowClose")]
	private BSGButton m_ButtonClose;

	// Token: 0x040013B4 RID: 5044
	[UIFieldTarget("id_OpenRoyalFamily")]
	private BSGButton m_OpenRoyalFamily;

	// Token: 0x040013B5 RID: 5045
	[UIFieldTarget("id_KingAbilities")]
	private UIKingAbilities m_KingAbilities;

	// Token: 0x040013B6 RID: 5046
	[SerializeField]
	private GameObject ClassSelectButtonPrototype;

	// Token: 0x040013B7 RID: 5047
	[SerializeField]
	private GameObject ClassSelectButtonPrototypeExtended;

	// Token: 0x040013B9 RID: 5049
	public Vars Vars;

	// Token: 0x040013BA RID: 5050
	public DT.Field field;

	// Token: 0x040013BB RID: 5051
	private CharacterClass.Def selectedClass;

	// Token: 0x040013BC RID: 5052
	private List<UIGenericActionIcon> classIcons = new List<UIGenericActionIcon>(10);

	// Token: 0x040013BD RID: 5053
	private List<UIGenericActionIcon> classIconsList = new List<UIGenericActionIcon>();

	// Token: 0x040013BE RID: 5054
	private HoldAFeastAction holdAFeastAction;

	// Token: 0x040013BF RID: 5055
	private bool m_Started;

	// Token: 0x040013C0 RID: 5056
	private bool m_DataExtracted;

	// Token: 0x040013C1 RID: 5057
	private TextMeshProUGUI m_FeastLabel;

	// Token: 0x040013C2 RID: 5058
	private TextMeshProUGUI m_EducateAndFeastLabel;
}
