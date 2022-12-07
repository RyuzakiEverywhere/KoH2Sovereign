using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000279 RID: 633
public class UIQuestsInfoWindow : MonoBehaviour, IListener
{
	// Token: 0x170001CC RID: 460
	// (get) Token: 0x060026BC RID: 9916 RVA: 0x00152B91 File Offset: 0x00150D91
	// (set) Token: 0x060026BD RID: 9917 RVA: 0x00152B99 File Offset: 0x00150D99
	public Logic.Kingdom Data { get; private set; }

	// Token: 0x060026BE RID: 9918 RVA: 0x00152BA2 File Offset: 0x00150DA2
	private void OnDestroy()
	{
		if (this.Data != null)
		{
			this.Data.DelListener(this);
		}
		this.m_SelectedQuest = null;
		this.m_questList.Clear();
	}

	// Token: 0x060026BF RID: 9919 RVA: 0x00152BCC File Offset: 0x00150DCC
	public void SetData(Logic.Kingdom k)
	{
		if (this.Data != null)
		{
			this.Data.DelListener(this);
		}
		if (k == null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_Close != null)
		{
			this.m_Close.onClick = new BSGButton.OnClick(this.HandleClose);
		}
		if (this.m_CompleteQuest != null)
		{
			this.m_CompleteQuest.onClick = new BSGButton.OnClick(this.HandleCompleteQuest);
		}
		this.Data = k;
		this.Data.AddListener(this);
		this.Refresh();
	}

	// Token: 0x060026C0 RID: 9920 RVA: 0x00152C66 File Offset: 0x00150E66
	private void Refresh()
	{
		if (this.m_KingdomIcon != null)
		{
			this.m_KingdomIcon.SetObject(this.Data, null);
		}
		this.PopulateQuestList();
	}

	// Token: 0x060026C1 RID: 9921 RVA: 0x00152C90 File Offset: 0x00150E90
	private void PopulateQuestList()
	{
		if (this.m_questList != null && this.m_questList.Count > 0)
		{
			for (int i = 0; i < this.m_questList.Count; i++)
			{
				UnityEngine.Object.Destroy(this.m_questList[i].gameObject);
			}
			this.m_questList.Clear();
		}
		if (this.Data == null)
		{
			return;
		}
		if (this.Data.quests == null)
		{
			return;
		}
		if (this.Data.quests.Count == 0)
		{
			return;
		}
		List<Quest> quests = this.Data.quests.GetQuests();
		if (quests != null && quests.Count > 0)
		{
			for (int j = 0; j < quests.Count; j++)
			{
				this.AddQuest(quests[j]);
			}
			this.SelectQuest(quests[0]);
		}
	}

	// Token: 0x060026C2 RID: 9922 RVA: 0x00152D5C File Offset: 0x00150F5C
	private void AddQuest(Quest quest)
	{
		if (quest == null)
		{
			return;
		}
		if (this.QuestRowElementPrototype == null)
		{
			return;
		}
		UIQuestsInfoWindow.QuestDataRow questDataRow = UnityEngine.Object.Instantiate<GameObject>(this.QuestRowElementPrototype, this.m_QuestsContainer).AddComponent<UIQuestsInfoWindow.QuestDataRow>();
		questDataRow.SetData(quest, null);
		questDataRow.OnSelect += this.QuestSelectHandler;
		this.m_questList.Add(questDataRow);
	}

	// Token: 0x060026C3 RID: 9923 RVA: 0x00152DB9 File Offset: 0x00150FB9
	private void QuestSelectHandler(UIQuestsInfoWindow.QuestDataRow obj)
	{
		this.SelectQuest(obj.Data);
	}

	// Token: 0x060026C4 RID: 9924 RVA: 0x00152DC8 File Offset: 0x00150FC8
	private void SelectQuest(Quest quest)
	{
		this.m_SelectedQuest = quest;
		for (int i = 0; i < this.m_questList.Count; i++)
		{
			UIQuestsInfoWindow.QuestDataRow questDataRow = this.m_questList[i];
			if (questDataRow.Data == this.m_SelectedQuest)
			{
				questDataRow.Select(true);
				this.InspectQuest(this.m_SelectedQuest);
			}
			else
			{
				questDataRow.Select(false);
			}
		}
	}

	// Token: 0x060026C5 RID: 9925 RVA: 0x00152E2C File Offset: 0x0015102C
	private void InspectQuest(Quest quest)
	{
		if (quest != null)
		{
			if (this.m_QuestTitle != null)
			{
				UIText.SetText(this.m_QuestTitle, global::Defs.Localize(quest.def.field, "name", null, null, true, true));
			}
			if (this.m_QuestDescription != null)
			{
				UIText.SetText(this.m_QuestDescription, global::Defs.Localize(quest.def.field, "long_description", null, null, true, true));
			}
		}
	}

	// Token: 0x060026C6 RID: 9926 RVA: 0x00152EA0 File Offset: 0x001510A0
	private void Update()
	{
		if (this.Data != BaseUI.LogicKingdom())
		{
			this.SetData(BaseUI.LogicKingdom());
		}
	}

	// Token: 0x060026C7 RID: 9927 RVA: 0x00152EBA File Offset: 0x001510BA
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "quest_changed")
		{
			this.Refresh();
		}
	}

	// Token: 0x060026C8 RID: 9928 RVA: 0x00152ED0 File Offset: 0x001510D0
	public static bool ToggleOpen()
	{
		GameObject prefab = UICommon.GetPrefab("KingdomQuestWindow", null);
		if (prefab == null)
		{
			return false;
		}
		BaseUI baseUI = BaseUI.Get();
		if (baseUI != null)
		{
			GameObject gameObject = global::Common.FindChildByName(baseUI.tCanvas.gameObject, "id_MessageContainer", true, true);
			if (gameObject != null && !UICommon.DeleteChildren(gameObject.transform, typeof(UIQuestsInfoWindow)))
			{
				UIQuestsInfoWindow component = UnityEngine.Object.Instantiate<GameObject>(prefab, gameObject.transform).GetComponent<UIQuestsInfoWindow>();
				if (component != null)
				{
					component.SetData(BaseUI.LogicKingdom());
					DT.Field soundsDef = BaseUI.soundsDef;
					BaseUI.PlaySoundEvent((soundsDef != null) ? soundsDef.GetString("open_kingdom_missions_window", null, "", true, true, true, '.') : null, null);
				}
			}
		}
		return false;
	}

	// Token: 0x060026C9 RID: 9929 RVA: 0x000C4358 File Offset: 0x000C2558
	private void HandleClose(BSGButton b)
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	// Token: 0x060026CA RID: 9930 RVA: 0x00152F8C File Offset: 0x0015118C
	private void HandleCompleteQuest(BSGButton b)
	{
		if (this.m_SelectedQuest != null)
		{
			this.m_SelectedQuest.Complete();
		}
	}

	// Token: 0x04001A31 RID: 6705
	private const string PREFRED_CONTAINER = "id_MessageContainer";

	// Token: 0x04001A32 RID: 6706
	[SerializeField]
	private GameObject QuestRowElementPrototype;

	// Token: 0x04001A33 RID: 6707
	[SerializeField]
	private GameObject QuestConditionPrototype;

	// Token: 0x04001A34 RID: 6708
	[UIFieldTarget("id_Close_Button")]
	private BSGButton m_Close;

	// Token: 0x04001A35 RID: 6709
	[UIFieldTarget("id_KingdomIcon")]
	private UIKingdomIcon m_KingdomIcon;

	// Token: 0x04001A36 RID: 6710
	[UIFieldTarget("id_QuestTitle")]
	private TextMeshProUGUI m_QuestTitle;

	// Token: 0x04001A37 RID: 6711
	[UIFieldTarget("id_QuestDescription")]
	private TextMeshProUGUI m_QuestDescription;

	// Token: 0x04001A38 RID: 6712
	[UIFieldTarget("id_QuestsContainer")]
	private RectTransform m_QuestsContainer;

	// Token: 0x04001A39 RID: 6713
	[UIFieldTarget("id_CompleteQuest")]
	private BSGButton m_CompleteQuest;

	// Token: 0x04001A3A RID: 6714
	private List<UIQuestsInfoWindow.QuestDataRow> m_questList = new List<UIQuestsInfoWindow.QuestDataRow>();

	// Token: 0x04001A3B RID: 6715
	private Quest m_SelectedQuest;

	// Token: 0x020007DC RID: 2012
	protected internal class QuestDataRow : Hotspot, IListener
	{
		// Token: 0x17000609 RID: 1545
		// (get) Token: 0x06004E6E RID: 20078 RVA: 0x002323F5 File Offset: 0x002305F5
		// (set) Token: 0x06004E6F RID: 20079 RVA: 0x002323FD File Offset: 0x002305FD
		public Quest Data { get; private set; }

		// Token: 0x1700060A RID: 1546
		// (get) Token: 0x06004E70 RID: 20080 RVA: 0x00232406 File Offset: 0x00230606
		// (set) Token: 0x06004E71 RID: 20081 RVA: 0x0023240E File Offset: 0x0023060E
		public Vars Vars { get; private set; }

		// Token: 0x1400004E RID: 78
		// (add) Token: 0x06004E72 RID: 20082 RVA: 0x00232418 File Offset: 0x00230618
		// (remove) Token: 0x06004E73 RID: 20083 RVA: 0x00232450 File Offset: 0x00230650
		public event Action<UIQuestsInfoWindow.QuestDataRow> OnSelect;

		// Token: 0x1400004F RID: 79
		// (add) Token: 0x06004E74 RID: 20084 RVA: 0x00232488 File Offset: 0x00230688
		// (remove) Token: 0x06004E75 RID: 20085 RVA: 0x002324C0 File Offset: 0x002306C0
		public event Action<UIQuestsInfoWindow.QuestDataRow> OnFocus;

		// Token: 0x06004E76 RID: 20086 RVA: 0x002324F8 File Offset: 0x002306F8
		public void SetData(Quest book, Vars vars = null)
		{
			if (this.Data != null)
			{
				this.Data.DelListener(this);
			}
			UICommon.FindComponents(this, false);
			this.Data = book;
			this.Vars = (vars ?? new Vars(this.Data));
			if (this.Data != null)
			{
				this.Data.AddListener(this);
			}
			this.Refresh();
			this.UpdateHighlight();
		}

		// Token: 0x06004E77 RID: 20087 RVA: 0x00232562 File Offset: 0x00230762
		private void OnDestroy()
		{
			this.OnSelect = null;
			this.OnFocus = null;
			if (this.Data != null)
			{
				this.Data.DelListener(this);
			}
		}

		// Token: 0x06004E78 RID: 20088 RVA: 0x00232586 File Offset: 0x00230786
		public void Select(bool selected)
		{
			this.m_Selected = selected;
			this.UpdateHighlight();
		}

		// Token: 0x06004E79 RID: 20089 RVA: 0x00232598 File Offset: 0x00230798
		public void Refresh()
		{
			if (this.Data == null)
			{
				return;
			}
			if (this.Data.def == null)
			{
				return;
			}
			if (this.m_Name != null)
			{
				UIText.SetText(this.m_Name, global::Defs.Localize(this.Data.def.field, "name", null, null, true, true));
			}
			if (this.m_Description != null)
			{
				UIText.SetText(this.m_Description, global::Defs.Localize(this.Data.def.field, "long_description", null, null, true, true));
			}
		}

		// Token: 0x06004E7A RID: 20090 RVA: 0x0023262C File Offset: 0x0023082C
		public override void OnClick(PointerEventData e)
		{
			if (e.clickCount == 1 && this.OnSelect != null)
			{
				this.OnSelect(this);
			}
			if (e.clickCount > 1)
			{
				if (this.OnSelect != null)
				{
					this.OnSelect(this);
				}
				if (this.OnFocus != null)
				{
					this.OnFocus(this);
				}
			}
		}

		// Token: 0x06004E7B RID: 20091 RVA: 0x00232687 File Offset: 0x00230887
		public void UpdateHighlight()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			if (this.m_SelectionBorder != null)
			{
				this.m_SelectionBorder.enabled = this.m_Selected;
			}
		}

		// Token: 0x06004E7C RID: 20092 RVA: 0x002326B0 File Offset: 0x002308B0
		public void OnMessage(object obj, string message, object param)
		{
			if (message == "quest_changed")
			{
				this.Refresh();
				return;
			}
		}

		// Token: 0x04003CB0 RID: 15536
		[UIFieldTarget("id_Name")]
		private TextMeshProUGUI m_Name;

		// Token: 0x04003CB1 RID: 15537
		[UIFieldTarget("id_Description")]
		private TextMeshProUGUI m_Description;

		// Token: 0x04003CB2 RID: 15538
		[UIFieldTarget("id_SelectionBorder")]
		private Image m_SelectionBorder;

		// Token: 0x04003CB7 RID: 15543
		private bool m_Selected;
	}
}
