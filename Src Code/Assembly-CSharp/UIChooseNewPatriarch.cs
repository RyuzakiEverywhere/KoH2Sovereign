using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x02000281 RID: 641
public class UIChooseNewPatriarch : MonoBehaviour
{
	// Token: 0x0600272B RID: 10027 RVA: 0x00155010 File Offset: 0x00153210
	private void Start()
	{
		MessageWnd component = base.GetComponent<MessageWnd>();
		if (component != null)
		{
			this.SetData(component.def_field, component.vars, component);
		}
	}

	// Token: 0x0600272C RID: 10028 RVA: 0x00155040 File Offset: 0x00153240
	private void Update()
	{
		Logic.Kingdom kingdom = this.kingdom;
		if (((kingdom != null) ? kingdom.patriarch_candidates : null) == null)
		{
			this.HandleClose();
			return;
		}
		bool flag = false;
		for (int i = 0; i < this.slots.Count; i++)
		{
			UIChooseNewPatriarch.CandidateSlot candidateSlot = this.slots[i];
			if (!this.IsValid(this.kingdom, candidateSlot.Data))
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			this.PopulateCandidates(this.ExctractCandidates());
		}
	}

	// Token: 0x0600272D RID: 10029 RVA: 0x001550B4 File Offset: 0x001532B4
	private bool IsValid(Logic.Kingdom k, Logic.Character c)
	{
		if (k == null)
		{
			return false;
		}
		if (c == null)
		{
			return false;
		}
		if (c.IsDead())
		{
			return false;
		}
		if (k.patriarch_candidates != null)
		{
			for (int i = 0; i < k.patriarch_candidates.Count; i++)
			{
				Orthodox.PatriarchCandidate patriarchCandidate = k.patriarch_candidates[i];
				if (patriarchCandidate.cleric == c)
				{
					return patriarchCandidate.generated || k.IsCourtMember(c);
				}
			}
		}
		return false;
	}

	// Token: 0x0600272E RID: 10030 RVA: 0x0015511C File Offset: 0x0015331C
	public void SetData(DT.Field def_field, Vars vars, MessageWnd message)
	{
		UICommon.FindComponents(this, false);
		this.kingdom = vars.Get<Logic.Kingdom>("kingdom", null);
		if (this.m_kingdom != null && this.kingdom != null)
		{
			this.m_kingdom.SetObject(this.kingdom, null);
		}
		if (this.Button_Close != null)
		{
			this.Button_Close.onClick = new BSGButton.OnClick(this.HanldeOnCloseClick);
		}
		if (this.m_ButtonClose != null)
		{
			this.m_ButtonClose.onClick = new BSGButton.OnClick(this.HandleCloseAction);
		}
		UIText.SetText(this.Value_Caption, def_field, "caption", vars, null);
		UIText.SetText(this.Value_Body, def_field, "body", vars, null);
		UIText.SetText(this.Label_Close, def_field, "button_close", vars, null);
		if (this.m_ButtonChoose != null)
		{
			this.m_ButtonChoose.onClick = new BSGButton.OnClick(this.HandleChooseCandidate);
		}
		this.PopulateCandidates(this.ExctractCandidates());
	}

	// Token: 0x0600272F RID: 10031 RVA: 0x00155220 File Offset: 0x00153420
	private List<Logic.Character> ExctractCandidates()
	{
		Logic.Kingdom kingdom = this.kingdom;
		if (kingdom == null || kingdom.patriarch_candidates == null)
		{
			return null;
		}
		List<Logic.Character> list = new List<Logic.Character>(kingdom.patriarch_candidates.Count);
		for (int i = 0; i < kingdom.patriarch_candidates.Count; i++)
		{
			Orthodox.PatriarchCandidate patriarchCandidate = kingdom.patriarch_candidates[i];
			if (patriarchCandidate.generated || kingdom.IsCourtMember(patriarchCandidate.cleric))
			{
				list.Add(patriarchCandidate.cleric);
			}
		}
		return list;
	}

	// Token: 0x06002730 RID: 10032 RVA: 0x00155298 File Offset: 0x00153498
	private void PopulateCandidates(List<Logic.Character> characters)
	{
		UICommon.DeleteChildren(this.m_ContainerCandidates);
		this.slots.Clear();
		if (characters == null)
		{
			return;
		}
		if (this.m_ContainerCandidates == null)
		{
			return;
		}
		for (int i = 0; i < characters.Count; i++)
		{
			Logic.Character character = characters[i];
			if (character != null)
			{
				UIChooseNewPatriarch.CandidateSlot candidateSlot = UIChooseNewPatriarch.CandidateSlot.Create(this.CandidateSlotProrotype, character, this.kingdom, this.m_ContainerCandidates);
				if (!(candidateSlot == null))
				{
					this.slots.Add(candidateSlot);
					UIChooseNewPatriarch.CandidateSlot candidateSlot2 = candidateSlot;
					candidateSlot2.OnSelect = (Action<UIChooseNewPatriarch.CandidateSlot>)Delegate.Combine(candidateSlot2.OnSelect, new Action<UIChooseNewPatriarch.CandidateSlot>(this.SelectSlot));
				}
			}
		}
		this.SelectSlot(this.slots[0]);
	}

	// Token: 0x06002731 RID: 10033 RVA: 0x0015534C File Offset: 0x0015354C
	private void SelectSlot(UIChooseNewPatriarch.CandidateSlot s)
	{
		this.selectedCandidate = s.Data;
		this.PopulateStats(s.Data);
		this.PopulateChooseButton(s.Data);
		for (int i = 0; i < this.slots.Count; i++)
		{
			UIChooseNewPatriarch.CandidateSlot candidateSlot = this.slots[i];
			candidateSlot.Select(candidateSlot == s);
		}
	}

	// Token: 0x06002732 RID: 10034 RVA: 0x001553AC File Offset: 0x001535AC
	private void PopulateChooseButton(Logic.Character c)
	{
		if (this.m_ButtonChoose == null)
		{
			return;
		}
		TextMeshProUGUI componentInChildren = this.m_ButtonChoose.GetComponentInChildren<TextMeshProUGUI>();
		if (componentInChildren == null)
		{
			return;
		}
		UIText.SetTextKey(componentInChildren, "Orthodox.patriecth_select_label", new Vars(c), null);
	}

	// Token: 0x06002733 RID: 10035 RVA: 0x001553F8 File Offset: 0x001535F8
	private void PopulateStats(Logic.Character c)
	{
		if (this.Value_Stats == null)
		{
			return;
		}
		if (this.kingdom == null)
		{
			return;
		}
		if (this.kingdom.patriarch_candidates != null)
		{
			for (int i = 0; i < this.kingdom.patriarch_candidates.Count; i++)
			{
				Orthodox.PatriarchCandidate patriarchCandidate = this.kingdom.patriarch_candidates[i];
				if (patriarchCandidate.cleric == c)
				{
					UIText.SetText(this.Value_Stats, global::Religions.GetPatriarchBonusesText(this.kingdom, patriarchCandidate, "\n", true));
				}
			}
		}
	}

	// Token: 0x06002734 RID: 10036 RVA: 0x0015547D File Offset: 0x0015367D
	private void HandleChooseCandidate(BSGButton btn)
	{
		if (this.kingdom != null)
		{
			this.kingdom.game.religions.orthodox.PatriarchChosen(this.kingdom, this.selectedCandidate, true);
		}
		this.HandleClose();
	}

	// Token: 0x06002735 RID: 10037 RVA: 0x001554B4 File Offset: 0x001536B4
	private void HanldeOnCloseClick(BSGButton btn)
	{
		this.HandleClose();
		this.HandleFate();
	}

	// Token: 0x06002736 RID: 10038 RVA: 0x001554C4 File Offset: 0x001536C4
	private void HandleClose()
	{
		MessageWnd component = base.GetComponent<MessageWnd>();
		if (component != null)
		{
			component.CloseAndDismiss(true);
		}
	}

	// Token: 0x06002737 RID: 10039 RVA: 0x001554E8 File Offset: 0x001536E8
	private void HandleCloseAction(BSGButton e)
	{
		this.HandleFate();
		this.HandleClose();
	}

	// Token: 0x06002738 RID: 10040 RVA: 0x001554F6 File Offset: 0x001536F6
	private void HandleFate()
	{
		if (this.kingdom != null)
		{
			this.kingdom.game.religions.orthodox.PatriarchChosen(this.kingdom, null, true);
		}
	}

	// Token: 0x04001A91 RID: 6801
	[SerializeField]
	private GameObject CandidateSlotProrotype;

	// Token: 0x04001A92 RID: 6802
	[UIFieldTarget("id_KingdomShield")]
	private UIKingdomIcon m_kingdom;

	// Token: 0x04001A93 RID: 6803
	[UIFieldTarget("id_Caption")]
	private TextMeshProUGUI Value_Caption;

	// Token: 0x04001A94 RID: 6804
	[UIFieldTarget("id_Body")]
	private TextMeshProUGUI Value_Body;

	// Token: 0x04001A95 RID: 6805
	[UIFieldTarget("id_Stats")]
	private TextMeshProUGUI Value_Stats;

	// Token: 0x04001A96 RID: 6806
	[UIFieldTarget("id_Button_Close")]
	private BSGButton Button_Close;

	// Token: 0x04001A97 RID: 6807
	[UIFieldTarget("id_ContainerCandidates")]
	private RectTransform m_ContainerCandidates;

	// Token: 0x04001A98 RID: 6808
	[UIFieldTarget("id_Button_Choose")]
	private BSGButton m_ButtonChoose;

	// Token: 0x04001A99 RID: 6809
	[UIFieldTarget("id_Button_Close")]
	private BSGButton m_ButtonClose;

	// Token: 0x04001A9A RID: 6810
	[UIFieldTarget("Label_Close")]
	private TextMeshProUGUI Label_Close;

	// Token: 0x04001A9B RID: 6811
	private Logic.Kingdom kingdom;

	// Token: 0x04001A9C RID: 6812
	private Logic.Character selectedCandidate;

	// Token: 0x04001A9D RID: 6813
	private List<UIChooseNewPatriarch.CandidateSlot> slots = new List<UIChooseNewPatriarch.CandidateSlot>();

	// Token: 0x020007E2 RID: 2018
	internal class CandidateSlot : MonoBehaviour, IListener
	{
		// Token: 0x1700060B RID: 1547
		// (get) Token: 0x06004E8E RID: 20110 RVA: 0x0023276B File Offset: 0x0023096B
		// (set) Token: 0x06004E8F RID: 20111 RVA: 0x00232773 File Offset: 0x00230973
		public Action<UIChooseNewPatriarch.CandidateSlot> OnSelect { get; set; }

		// Token: 0x1700060C RID: 1548
		// (get) Token: 0x06004E90 RID: 20112 RVA: 0x0023277C File Offset: 0x0023097C
		// (set) Token: 0x06004E91 RID: 20113 RVA: 0x00232784 File Offset: 0x00230984
		public Logic.Character Data { get; private set; }

		// Token: 0x1700060D RID: 1549
		// (get) Token: 0x06004E92 RID: 20114 RVA: 0x0023278D File Offset: 0x0023098D
		// (set) Token: 0x06004E93 RID: 20115 RVA: 0x00232795 File Offset: 0x00230995
		public Logic.Kingdom Kingdom { get; private set; }

		// Token: 0x06004E94 RID: 20116 RVA: 0x0023279E File Offset: 0x0023099E
		public void SetData(Logic.Character c, Logic.Kingdom k)
		{
			UICommon.FindComponents(this, false);
			this.Data = c;
			this.Kingdom = k;
			this.Populate();
			if (this.Data != null)
			{
				this.Data.AddListener(this);
			}
		}

		// Token: 0x06004E95 RID: 20117 RVA: 0x002327CF File Offset: 0x002309CF
		private void OnDestroy()
		{
			if (this.Data != null)
			{
				this.Data.DelListener(this);
			}
		}

		// Token: 0x06004E96 RID: 20118 RVA: 0x002327E8 File Offset: 0x002309E8
		private void Populate()
		{
			if (this.Data == null)
			{
				return;
			}
			if (this.m_Icon)
			{
				Vars vars = new Vars();
				vars.Set<string>("variant", "compact");
				UICharacterIcon component = ObjectIcon.GetIcon(this.Data, vars, this.m_Icon.transform as RectTransform).GetComponent<UICharacterIcon>();
				if (component != null)
				{
					component.SetObject(this.Data, null);
					component.ShowCrest(false);
					component.OnSelect += this.HandleOnIconSelect;
				}
				if (this.m_Icon != null)
				{
					Vector2 sizeDelta = this.Kingdom.IsCourtMember(this.Data) ? new Vector2(64f, 64f) : new Vector2(50f, 50f);
					(this.m_Icon.transform as RectTransform).sizeDelta = sizeDelta;
				}
			}
			if (this.m_Name != null)
			{
				UIText.SetTextKey(this.m_Name, "Character.name_only", new Vars(this.Data), null);
			}
			if (this.m_Age != null)
			{
				string key = "Character.age." + this.Data.age.ToString();
				UIText.SetTextKey(this.m_Age, key, null, null);
			}
		}

		// Token: 0x06004E97 RID: 20119 RVA: 0x00232940 File Offset: 0x00230B40
		private void Update()
		{
			if (this.Kingdom == null || this.Data == null || this.Kingdom.patriarch_candidates == null)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			bool flag = false;
			for (int i = 0; i < this.Kingdom.patriarch_candidates.Count; i++)
			{
				Orthodox.PatriarchCandidate patriarchCandidate = this.Kingdom.patriarch_candidates[i];
				if (patriarchCandidate.cleric == this.Data)
				{
					flag = (patriarchCandidate.generated || this.Kingdom.IsCourtMember(this.Data));
					break;
				}
			}
			if (!flag)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
		}

		// Token: 0x06004E98 RID: 20120 RVA: 0x002329E1 File Offset: 0x00230BE1
		private void HandleOnIconSelect(UICharacterIcon obj)
		{
			if (this.OnSelect != null)
			{
				this.OnSelect(this);
			}
		}

		// Token: 0x06004E99 RID: 20121 RVA: 0x002329F7 File Offset: 0x00230BF7
		public void Select(bool select)
		{
			this.m_Selected = select;
			if (this.m_Selection != null)
			{
				this.m_Selection.gameObject.SetActive(this.m_Selected);
			}
		}

		// Token: 0x06004E9A RID: 20122 RVA: 0x00232A24 File Offset: 0x00230C24
		public static UIChooseNewPatriarch.CandidateSlot Create(GameObject prototype, Logic.Character c, Logic.Kingdom k, RectTransform parent)
		{
			if (prototype == null)
			{
				return null;
			}
			if (parent == null)
			{
				return null;
			}
			if (c == null)
			{
				return null;
			}
			if (k == null)
			{
				return null;
			}
			UIChooseNewPatriarch.CandidateSlot candidateSlot = UnityEngine.Object.Instantiate<GameObject>(prototype, parent).AddComponent<UIChooseNewPatriarch.CandidateSlot>();
			candidateSlot.SetData(c, k);
			return candidateSlot;
		}

		// Token: 0x06004E9B RID: 20123 RVA: 0x00232A5A File Offset: 0x00230C5A
		public void OnMessage(object obj, string message, object param)
		{
			if (message == "patriarch_candidates_changed")
			{
				this.Populate();
				return;
			}
			if (!(message == "destroying") && !(message == "finishing"))
			{
				return;
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}

		// Token: 0x04003CC0 RID: 15552
		private GameObject CandidateSlotProrotype;

		// Token: 0x04003CC1 RID: 15553
		[UIFieldTarget("id_Icon")]
		private RectTransform m_Icon;

		// Token: 0x04003CC2 RID: 15554
		[UIFieldTarget("id_Name")]
		private TextMeshProUGUI m_Name;

		// Token: 0x04003CC3 RID: 15555
		[UIFieldTarget("id_Age")]
		private TextMeshProUGUI m_Age;

		// Token: 0x04003CC4 RID: 15556
		[UIFieldTarget("id_Border")]
		private GameObject m_Selection;

		// Token: 0x04003CC8 RID: 15560
		private bool m_Selected;
	}
}
