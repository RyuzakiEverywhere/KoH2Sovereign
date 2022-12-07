using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000292 RID: 658
public class UIRoyalCourt : MonoBehaviour, IListener
{
	// Token: 0x170001EF RID: 495
	// (get) Token: 0x0600287B RID: 10363 RVA: 0x0015A8F2 File Offset: 0x00158AF2
	// (set) Token: 0x0600287C RID: 10364 RVA: 0x0015A8FA File Offset: 0x00158AFA
	public Logic.Kingdom Data { get; private set; }

	// Token: 0x170001F0 RID: 496
	// (get) Token: 0x0600287D RID: 10365 RVA: 0x0015A903 File Offset: 0x00158B03
	// (set) Token: 0x0600287E RID: 10366 RVA: 0x0015A90B File Offset: 0x00158B0B
	public UIHireWidnow hireWidnow { get; private set; }

	// Token: 0x170001F1 RID: 497
	// (get) Token: 0x0600287F RID: 10367 RVA: 0x0015A914 File Offset: 0x00158B14
	public bool HasSelecion
	{
		get
		{
			return this.m_SelectedSlotIndex >= 0;
		}
	}

	// Token: 0x06002880 RID: 10368 RVA: 0x0015A922 File Offset: 0x00158B22
	private IEnumerator Start()
	{
		yield return null;
		bool flag = true;
		while (flag)
		{
			WorldUI ui = WorldUI.Get();
			if (ui == null)
			{
				yield return null;
			}
			if (ui.kingdom == 0)
			{
				yield return null;
			}
			flag = false;
			ui = null;
		}
		this.Init();
		this.BuildHireWidnow();
		this.BuildContextActionWindow();
		this.ClearCurrentSelection();
		yield break;
	}

	// Token: 0x06002881 RID: 10369 RVA: 0x0015A934 File Offset: 0x00158B34
	private void Update()
	{
		if (this.Data == null)
		{
			Logic.Kingdom kingdom = BaseUI.LogicKingdom();
			if (kingdom != null && kingdom.IsValid() && kingdom.court != null)
			{
				this.SetData(kingdom);
			}
		}
		if (this.Data != null && this.m_CurrentContextActions == null)
		{
			this.BuildContextActionWindow();
		}
	}

	// Token: 0x06002882 RID: 10370 RVA: 0x0015A985 File Offset: 0x00158B85
	private void LateUpdate()
	{
		if (this.m_Invalidate)
		{
			this.Refresh();
			this.m_Invalidate = false;
		}
	}

	// Token: 0x06002883 RID: 10371 RVA: 0x0015A99C File Offset: 0x00158B9C
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		GameObject gameObject = global::Common.FindChildByName(base.gameObject, "Background", false, true);
		if (gameObject != null)
		{
			TooltipPlacement.AddBlocker(gameObject, null);
		}
		this.BuildSlots();
		this.m_Initialzied = true;
	}

	// Token: 0x06002884 RID: 10372 RVA: 0x0015A9EC File Offset: 0x00158BEC
	public void SetData(Logic.Kingdom kingdom)
	{
		this.Init();
		if (this.Data != null)
		{
			Logic.Kingdom data = this.Data;
			if (data != null)
			{
				data.royalFamily.DelListener(this);
			}
			this.Data.DelListener(this);
		}
		this.Data = kingdom;
		if (this.Data != null)
		{
			Logic.Kingdom data2 = this.Data;
			if (data2 != null)
			{
				data2.royalFamily.AddListener(this);
			}
			this.Data.AddListener(this);
		}
		this.SetSovereign((kingdom != null) ? kingdom.GetKing() : null);
		this.Refresh();
	}

	// Token: 0x06002885 RID: 10373 RVA: 0x0015AA74 File Offset: 0x00158C74
	public void SelectCourtMember(Logic.Character newCharacter, bool force = false)
	{
		int slotIndex = this.GetSlotIndex(newCharacter);
		if (!force && slotIndex == this.m_SelectedSlotIndex)
		{
			if (slotIndex != -1)
			{
				UIRoyalCourt.UICourtSlot uicourtSlot = this.m_CourtSlots[slotIndex];
				UICharacterIcon uicharacterIcon = (uicourtSlot != null) ? uicourtSlot.CharacterIcon : null;
				if (this.m_CurrentContextActions != null)
				{
					if (!this.m_CurrentContextActions.gameObject.activeInHierarchy)
					{
						UICourtMemberActions currentContextActions = this.m_CurrentContextActions;
						if (currentContextActions != null)
						{
							currentContextActions.Show();
						}
					}
					this.m_CurrentContextActions.UpdateViusals(newCharacter, uicharacterIcon.transform as RectTransform, false);
					return;
				}
			}
			else if (this.m_CurrentContextActions != null)
			{
				this.m_CurrentContextActions.UpdateViusals(null, null, false);
			}
			return;
		}
		this.m_SelectedSlotIndex = slotIndex;
		if (newCharacter == null && this.m_CurrentContextActions != null)
		{
			UICourtMemberActions currentContextActions2 = this.m_CurrentContextActions;
			if (currentContextActions2 != null)
			{
				currentContextActions2.Hide(false);
			}
			UICourtMemberActions currentContextActions3 = this.m_CurrentContextActions;
			if (currentContextActions3 != null)
			{
				currentContextActions3.UpdateViusals(null, null, false);
			}
		}
		this.UpdateSelectonHighlight(newCharacter);
		if (newCharacter != null && newCharacter.IsKing())
		{
			Logic.Kingdom kingdom = BaseUI.LogicKingdom();
			if (newCharacter == ((kingdom != null) ? kingdom.GetKing() : null))
			{
				UICharacterIcon characterIcon = this.m_CourtSlots[0].CharacterIcon;
				characterIcon.Select(true);
				if (this.m_CurrentContextActions != null)
				{
					this.m_CurrentContextActions.UpdateViusals(newCharacter, characterIcon.transform as RectTransform, false);
				}
			}
		}
	}

	// Token: 0x06002886 RID: 10374 RVA: 0x0015ABC4 File Offset: 0x00158DC4
	private void UpdateSelectonHighlight(Logic.Character newCharacter)
	{
		if (this.m_CourtSlots != null && this.m_CourtSlots.Count > 0)
		{
			for (int i = 0; i < this.m_CourtSlots.Count; i++)
			{
				UIRoyalCourt.UICourtSlot uicourtSlot = this.m_CourtSlots[i];
				UICharacterIcon uicharacterIcon = (uicourtSlot != null) ? uicourtSlot.CharacterIcon : null;
				if (uicharacterIcon.Data == newCharacter)
				{
					uicharacterIcon.Select(true);
					if (this.m_CurrentContextActions != null)
					{
						this.m_CurrentContextActions.UpdateViusals(newCharacter, uicharacterIcon.transform as RectTransform, true);
					}
				}
				else
				{
					uicharacterIcon.Select(false);
				}
			}
		}
	}

	// Token: 0x06002887 RID: 10375 RVA: 0x0015AC58 File Offset: 0x00158E58
	public void Refresh()
	{
		if (this.Data == null)
		{
			return;
		}
		if (this.m_CurrentContextActions != null)
		{
			this.m_CurrentContextActions.Hide(false);
		}
		this.Data.InitCourt();
		int num = this.m_CourtSlots.Count;
		if (this.Data.court.Count != this.m_CourtSlots.Count)
		{
			Debug.LogWarning(string.Format("Unextected number of royal court slots: {0} expected: {1}", this.Data.court.Count, this.m_CourtSlots.Count));
			num = Mathf.Min(this.Data.court.Count, this.m_CourtSlots.Count);
		}
		for (int i = 0; i < num; i++)
		{
			Logic.Character courtOrSpecialCourtMember = this.Data.GetCourtOrSpecialCourtMember(i);
			UIRoyalCourt.UICourtSlot uicourtSlot = this.m_CourtSlots[i];
			if (!(uicourtSlot == null))
			{
				uicourtSlot.SetCharacter(courtOrSpecialCourtMember, this.Data.id);
			}
		}
		if (this.m_swapTargetIndex != -1)
		{
			Logic.Character character = null;
			if (this.Data.court.Count > this.m_swapTargetIndex)
			{
				character = this.Data.GetCourtOrSpecialCourtMember(this.m_swapTargetIndex);
			}
			this.m_swapTargetIndex = -1;
			if (character != null)
			{
				this.SelectCourtMember(character, true);
			}
		}
		int j = 0;
		while (j < num)
		{
			Logic.Character courtOrSpecialCourtMember2 = this.Data.GetCourtOrSpecialCourtMember(j);
			if (courtOrSpecialCourtMember2 != null && !(this.m_CourtSlots[j] == null) && courtOrSpecialCourtMember2.select_army_on_spawn)
			{
				this.SelectCourtMember(courtOrSpecialCourtMember2, true);
				courtOrSpecialCourtMember2.select_army_on_spawn = false;
				Logic.Army army = courtOrSpecialCourtMember2.GetArmy();
				if (army != null)
				{
					WorldUI.Get().SelectObjFromLogic(army, false, true);
					break;
				}
				break;
			}
			else
			{
				j++;
			}
		}
		this.RefreshFirstFreeTutorialHighlightSlot();
	}

	// Token: 0x06002888 RID: 10376 RVA: 0x0015AE14 File Offset: 0x00159014
	private void RefreshFirstFreeTutorialHighlightSlot()
	{
		bool flag = false;
		for (int i = 1; i < this.m_CourtSlots.Count; i++)
		{
			UIRoyalCourt.UICourtSlot uicourtSlot = this.m_CourtSlots[i];
			bool flag2 = !flag && uicourtSlot.CharacterIcon.Data == null;
			uicourtSlot.EnableTutorialHighlight(flag2);
			if (flag2)
			{
				flag = true;
			}
		}
	}

	// Token: 0x06002889 RID: 10377 RVA: 0x0015AE68 File Offset: 0x00159068
	private void BuildSlots()
	{
		if (this.m_CourtContainer == null)
		{
			return;
		}
		int num = 9;
		DT.Def def = GameLogic.Get(true).dt.FindDef("Kingdom");
		if (def != null)
		{
			num = def.field.GetInt("royal_court_slots", null, 9, true, true, true, '.');
		}
		UIRoyalCourt.UICourtSlot uicourtSlot = null;
		int num2 = 0;
		int childCount = this.m_CourtContainer.childCount;
		while (num2 < childCount && num2 < num)
		{
			UIRoyalCourt.UICourtSlot orAddComponent = this.m_CourtContainer.GetChild(num2).GetOrAddComponent<UIRoyalCourt.UICourtSlot>();
			orAddComponent.CharacterIconPrototype = this.CharacterIconPrototype;
			if (orAddComponent.name == "id_KingSlot")
			{
				uicourtSlot = orAddComponent;
			}
			else
			{
				this.m_CourtSlots.Add(orAddComponent);
			}
			num2++;
		}
		if (uicourtSlot != null)
		{
			this.m_CourtSlots.Insert(0, uicourtSlot);
		}
		for (int i = 0; i < num; i++)
		{
			UIRoyalCourt.UICourtSlot uicourtSlot2 = this.m_CourtSlots[i];
			uicourtSlot2.OnCourtSlotPointerEvent += this.HandleSlotPointerEvent;
			uicourtSlot2.courtIndex = i;
		}
	}

	// Token: 0x0600288A RID: 10378 RVA: 0x0015AF6C File Offset: 0x0015916C
	private void BuildHireWidnow()
	{
		if (this.hireWidnow == null)
		{
			GameObject gameObject = global::Common.FindChildByName(base.transform.root.gameObject, "id_HireWindowContainer", true, true);
			if (gameObject != null)
			{
				this.hireWidnow = UIHireWidnow.Create(UIHireWidnow.GetPrefab(), gameObject.transform as RectTransform);
				UIHireWidnow hireWidnow = this.hireWidnow;
				if (hireWidnow != null)
				{
					hireWidnow.SetCourt(this);
				}
				UIHireWidnow hireWidnow2 = this.hireWidnow;
				if (hireWidnow2 == null)
				{
					return;
				}
				hireWidnow2.Show(false);
			}
		}
	}

	// Token: 0x0600288B RID: 10379 RVA: 0x0015AFEC File Offset: 0x001591EC
	private void BuildContextActionWindow()
	{
		if (this.m_CurrentContextActions != null)
		{
			return;
		}
		GameObject gameObject = global::Common.Spawn(UICourtMemberActions.GetPrefab(), base.transform.parent, false, "");
		this.m_CurrentContextActions = gameObject.GetOrAddComponent<UICourtMemberActions>();
		this.m_CurrentContextActions.gameObject.SetActive(false);
	}

	// Token: 0x0600288C RID: 10380 RVA: 0x0015B041 File Offset: 0x00159241
	public void FocusSlot(int idx)
	{
		if (idx < 0 || this.m_CourtSlots == null || this.m_CourtSlots.Count == 0)
		{
			return;
		}
		if (idx >= this.m_CourtSlots.Count)
		{
			return;
		}
		this.FocusSlot(this.m_CourtSlots[idx]);
	}

	// Token: 0x0600288D RID: 10381 RVA: 0x0015B080 File Offset: 0x00159280
	private void FocusSlot(UIRoyalCourt.UICourtSlot slot)
	{
		WorldUI worldUI = WorldUI.Get();
		if (worldUI == null)
		{
			return;
		}
		Logic.Character character;
		if (slot == null)
		{
			character = null;
		}
		else
		{
			UICharacterIcon characterIcon = slot.CharacterIcon;
			character = ((characterIcon != null) ? characterIcon.Data : null);
		}
		Logic.Character character2 = character;
		if (!(this.m_CurrentContextActions != null) || this.m_CurrentContextActions.Data != character2)
		{
			this.ClearCurrentSelection();
		}
		worldUI.SelectCourtMember(character2);
		worldUI.LookAt(character2, false);
		if (this.hireWidnow)
		{
			this.hireWidnow.Show(false);
		}
	}

	// Token: 0x0600288E RID: 10382 RVA: 0x0015B104 File Offset: 0x00159304
	private void HandleSlotPointerEvent(UIRoyalCourt.UICourtSlot slot, EventTriggerType evenType, PointerEventData e)
	{
		if (evenType != EventTriggerType.PointerClick)
		{
			return;
		}
		if (UICommon.GetKey(KeyCode.LeftControl, false) || UICommon.GetKey(KeyCode.RightControl, false))
		{
			int courtIndex = slot.courtIndex;
			if (courtIndex == 0 || this.m_SelectedSlotIndex == 0)
			{
				return;
			}
			if (courtIndex != -1 && this.m_SelectedSlotIndex != -1 && courtIndex != this.m_SelectedSlotIndex)
			{
				this.m_swapTargetIndex = courtIndex;
				this.Data.SwapCourtSlots(courtIndex, this.m_SelectedSlotIndex, true);
				return;
			}
		}
		else if (e.button == PointerEventData.InputButton.Left)
		{
			WorldUI worldUI = WorldUI.Get();
			if (worldUI == null)
			{
				return;
			}
			worldUI.SelectionSystem(slot.courtIndex, e.clickCount >= 2);
		}
	}

	// Token: 0x0600288F RID: 10383 RVA: 0x0015B1A5 File Offset: 0x001593A5
	public void SelectSlot(int idx)
	{
		if (idx < 0 || this.m_CourtSlots == null || this.m_CourtSlots.Count == 0)
		{
			return;
		}
		if (idx >= this.m_CourtSlots.Count)
		{
			return;
		}
		this.SelectSlot(this.m_CourtSlots[idx]);
	}

	// Token: 0x06002890 RID: 10384 RVA: 0x0015B1E4 File Offset: 0x001593E4
	public void SelectSlot(UIRoyalCourt.UICourtSlot slot)
	{
		if (slot.CharacterIcon.Data != null)
		{
			WorldUI worldUI = WorldUI.Get();
			if (worldUI == null)
			{
				return;
			}
			if (!(this.m_CurrentContextActions != null) || this.m_CurrentContextActions.Data != slot.CharacterIcon.Data)
			{
				this.ClearCurrentSelection();
			}
			worldUI.SelectCourtMember(slot.CharacterIcon.Data);
			if (this.hireWidnow)
			{
				this.hireWidnow.Show(false);
				return;
			}
		}
		else
		{
			BaseUI.PlaySoundEvent(this.clickSound, null);
			this.ClearCurrentSelection();
			this.UpdateSelectonHighlight(null);
			if (this.hireWidnow)
			{
				this.SelectCourtMember(null, false);
				this.hireWidnow.SetTargetSlotIndex(slot.courtIndex);
				this.hireWidnow.Show(true);
				UICommon.DockTo(this.hireWidnow.transform as RectTransform, slot.transform as RectTransform, TextAnchor.LowerCenter, new Vector2(0.5f, 0f));
			}
			for (int i = 0; i < this.m_CourtSlots.Count; i++)
			{
				UIRoyalCourt.UICourtSlot uicourtSlot = this.m_CourtSlots[i];
				uicourtSlot.Highlight(uicourtSlot.courtIndex == slot.courtIndex);
			}
			this.m_SelectedSlotIndex = slot.courtIndex;
		}
	}

	// Token: 0x06002891 RID: 10385 RVA: 0x0015B328 File Offset: 0x00159528
	public void ClearHireSelection()
	{
		for (int i = 0; i < this.m_CourtSlots.Count; i++)
		{
			this.m_CourtSlots[i].Highlight(false);
		}
	}

	// Token: 0x06002892 RID: 10386 RVA: 0x0015B35D File Offset: 0x0015955D
	public void ClearCurrentSelection()
	{
		this.m_SelectedSlotIndex = -1;
		if (this.m_CurrentContextActions != null)
		{
			this.m_CurrentContextActions.Hide(false);
		}
	}

	// Token: 0x06002893 RID: 10387 RVA: 0x0015B380 File Offset: 0x00159580
	private int GetSlotIndex(Logic.Character c)
	{
		if (c == null)
		{
			return -1;
		}
		for (int i = 0; i < this.m_CourtSlots.Count; i++)
		{
			UIRoyalCourt.UICourtSlot uicourtSlot = this.m_CourtSlots[i];
			Logic.Character character;
			if (uicourtSlot == null)
			{
				character = null;
			}
			else
			{
				UICharacterIcon characterIcon = uicourtSlot.CharacterIcon;
				character = ((characterIcon != null) ? characterIcon.Data : null);
			}
			if (character == c)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06002894 RID: 10388 RVA: 0x0015B3D4 File Offset: 0x001595D4
	public void OnMessage(object obj, string message, object param)
	{
		if (!(this == null) && !(base.gameObject == null))
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(message);
			if (num <= 1211309691U)
			{
				if (num != 190343346U)
				{
					if (num != 191321419U)
					{
						if (num != 1211309691U)
						{
							return;
						}
						if (!(message == "destroying"))
						{
							return;
						}
					}
					else
					{
						if (!(message == "court_changed"))
						{
							return;
						}
						this.m_Invalidate = true;
						if (this.hireWidnow != null)
						{
							this.hireWidnow.Show(false);
						}
						return;
					}
				}
				else
				{
					if (!(message == "royal_new_sovereign"))
					{
						return;
					}
					Logic.Kingdom data = this.Data;
					this.SetSovereign((data != null) ? data.GetKing() : null);
					return;
				}
			}
			else if (num <= 1649643086U)
			{
				if (num != 1398825642U)
				{
					if (num != 1649643086U)
					{
						return;
					}
					if (!(message == "finishing"))
					{
						return;
					}
				}
				else
				{
					if (!(message == "del_court"))
					{
						return;
					}
					this.m_Invalidate = true;
					if (this.hireWidnow != null)
					{
						this.hireWidnow.Show(false);
					}
					return;
				}
			}
			else if (num != 1691588372U)
			{
				if (num != 3021484928U)
				{
					return;
				}
				if (!(message == "mission_kingdom_changed"))
				{
					return;
				}
				this.m_Invalidate = true;
				if (this.hireWidnow != null)
				{
					this.hireWidnow.Show(false);
				}
				return;
			}
			else
			{
				if (!(message == "add_court"))
				{
					return;
				}
				this.m_Invalidate = true;
				if (this.hireWidnow != null)
				{
					this.hireWidnow.Show(false);
				}
				return;
			}
			this.SetData(null);
			return;
		}
		Logic.Kingdom data2 = this.Data;
		if (data2 == null)
		{
			return;
		}
		data2.DelListener(this);
	}

	// Token: 0x06002895 RID: 10389 RVA: 0x0015B575 File Offset: 0x00159775
	public RectTransform GetIconRect(int index)
	{
		if (this.m_CourtContainer.childCount > index)
		{
			return this.m_CourtContainer.GetChild(index) as RectTransform;
		}
		return null;
	}

	// Token: 0x06002896 RID: 10390 RVA: 0x0015B598 File Offset: 0x00159798
	public void SetAudioSet(string def_id)
	{
		global::Defs.Get(false);
		DT.Field defField = global::Defs.GetDefField(def_id, null);
		if (defField == null)
		{
			this.rolloverSound = null;
			this.clickSound = null;
			return;
		}
		this.rolloverSound = defField.GetString("roll_over", null, "", true, true, true, '.');
		this.clickSound = defField.GetString("click", null, "", true, true, true, '.');
	}

	// Token: 0x06002897 RID: 10391 RVA: 0x0015B5FE File Offset: 0x001597FE
	private void OnDestroy()
	{
		if (this.Data != null)
		{
			this.Data.DelListener(this);
		}
	}

	// Token: 0x06002898 RID: 10392 RVA: 0x0015B614 File Offset: 0x00159814
	private void SetSovereign(Logic.Character s)
	{
		this.Refresh();
		this.UpdateSovereignSpesificArt();
	}

	// Token: 0x06002899 RID: 10393 RVA: 0x0015B624 File Offset: 0x00159824
	private void UpdateSovereignSpesificArt()
	{
		if (this.Data == null)
		{
			return;
		}
		Logic.Character king = this.Data.GetKing();
		if (king == null)
		{
			return;
		}
		DT.Field defField = global::Defs.GetDefField("RoyalCourtWindow", null);
		if (defField == null)
		{
			return;
		}
		if (this.m_ClassHeader != null)
		{
			this.m_ClassHeader.overrideSprite = global::Defs.GetObj<Sprite>(defField, "class_header." + king.class_name, null);
		}
		if (this.m_ClassFooter != null)
		{
			this.m_ClassFooter.overrideSprite = global::Defs.GetObj<Sprite>(defField, "class_footer." + king.class_name, null);
		}
	}

	// Token: 0x04001B64 RID: 7012
	[SerializeField]
	private GameObject CharacterIconPrototype;

	// Token: 0x04001B65 RID: 7013
	[UIFieldTarget("id_CourtContainer")]
	private RectTransform m_CourtContainer;

	// Token: 0x04001B66 RID: 7014
	[UIFieldTarget("id_ClassHeader")]
	private Image m_ClassHeader;

	// Token: 0x04001B67 RID: 7015
	[UIFieldTarget("id_ClassFooter")]
	private Image m_ClassFooter;

	// Token: 0x04001B69 RID: 7017
	private List<UIRoyalCourt.UICourtSlot> m_CourtSlots = new List<UIRoyalCourt.UICourtSlot>();

	// Token: 0x04001B6B RID: 7019
	public float KingScaleMod = 1.18f;

	// Token: 0x04001B6C RID: 7020
	[EventRef(compact = true)]
	public string rolloverSound;

	// Token: 0x04001B6D RID: 7021
	[EventRef(compact = true)]
	public string clickSound;

	// Token: 0x04001B6E RID: 7022
	private UICourtMemberActions m_CurrentContextActions;

	// Token: 0x04001B6F RID: 7023
	private bool m_Initialzied;

	// Token: 0x04001B70 RID: 7024
	private int m_SelectedSlotIndex = -1;

	// Token: 0x04001B71 RID: 7025
	private const int defaultSlotCount = 9;

	// Token: 0x04001B72 RID: 7026
	private int m_swapTargetIndex = -1;

	// Token: 0x04001B73 RID: 7027
	private bool m_Invalidate;

	// Token: 0x020007E4 RID: 2020
	public class UICourtSlot : Hotspot
	{
		// Token: 0x17000611 RID: 1553
		// (get) Token: 0x06004EAD RID: 20141 RVA: 0x00232F44 File Offset: 0x00231144
		// (set) Token: 0x06004EAE RID: 20142 RVA: 0x00232F4C File Offset: 0x0023114C
		public UICharacterIcon CharacterIcon { get; private set; }

		// Token: 0x14000050 RID: 80
		// (add) Token: 0x06004EAF RID: 20143 RVA: 0x00232F58 File Offset: 0x00231158
		// (remove) Token: 0x06004EB0 RID: 20144 RVA: 0x00232F90 File Offset: 0x00231190
		public event Action<UIRoyalCourt.UICourtSlot> OnSelect;

		// Token: 0x14000051 RID: 81
		// (add) Token: 0x06004EB1 RID: 20145 RVA: 0x00232FC8 File Offset: 0x002311C8
		// (remove) Token: 0x06004EB2 RID: 20146 RVA: 0x00233000 File Offset: 0x00231200
		public event Action<UIRoyalCourt.UICourtSlot> OnFocus;

		// Token: 0x14000052 RID: 82
		// (add) Token: 0x06004EB3 RID: 20147 RVA: 0x00233038 File Offset: 0x00231238
		// (remove) Token: 0x06004EB4 RID: 20148 RVA: 0x00233070 File Offset: 0x00231270
		public event Action<UIRoyalCourt.UICourtSlot, EventTriggerType, PointerEventData> OnCourtSlotPointerEvent;

		// Token: 0x06004EB5 RID: 20149 RVA: 0x002330A5 File Offset: 0x002312A5
		private void Start()
		{
			this.Init();
		}

		// Token: 0x06004EB6 RID: 20150 RVA: 0x002330AD File Offset: 0x002312AD
		private void OnDestroy()
		{
			this.OnSelect = null;
			this.OnCourtSlotPointerEvent = null;
		}

		// Token: 0x06004EB7 RID: 20151 RVA: 0x002330BD File Offset: 0x002312BD
		public Transform GetIconContainer()
		{
			if (!this.inited)
			{
				this.Init();
			}
			return this.m_CharacterGroup.transform;
		}

		// Token: 0x06004EB8 RID: 20152 RVA: 0x002330D8 File Offset: 0x002312D8
		private void Init()
		{
			if (this.inited)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			if (this.m_HireGroup != null)
			{
				this.m_ButtonEmpty = this.m_HireGroup.GetComponent<BSGButton>();
				this.m_ButtonEmpty.AllowSelection(true);
				this.m_ButtonEmpty.onClick = new BSGButton.OnClick(this.HandleEmptySlotOnClick);
				Hotspot orAddComponent = this.m_HireGroup.GetOrAddComponent<Hotspot>();
				orAddComponent.SetAudioSet("");
				orAddComponent.OnPointerEvent += this.HandleEmptySlotPointerEvent;
				Tooltip.Get(this.m_HireGroup, true).SetDef("VacantSlotTooltip", null);
			}
			if (this.m_TutorialFirstFreeSlot != null)
			{
				this.m_TutorialFirstFreeSlot.gameObject.SetActive(false);
			}
			this.inited = true;
		}

		// Token: 0x06004EB9 RID: 20153 RVA: 0x0023319C File Offset: 0x0023139C
		public void SetCharacter(Logic.Character courtMember, int courtKingdomId)
		{
			this.Init();
			if (this.CharacterIcon == null)
			{
				this.CreateChracterIcon();
				UICommon.FillParent(this.CharacterIcon.transform as RectTransform);
			}
			this.CharacterIcon.ShowMissonKingdomCrest(true);
			this.CharacterIcon.ShowPrisonKingdomCrest(true);
			this.CharacterIcon.SetObject(courtMember, null);
			int num = (courtMember != null) ? courtMember.kingdom_id : 0;
			this.CharacterIcon.ShowCrest(num != 0 && num != courtKingdomId);
			this.CharacterIcon.ShowMaintainStatusIcon(true);
			this.Refresh();
		}

		// Token: 0x06004EBA RID: 20154 RVA: 0x00233234 File Offset: 0x00231434
		public void EnableTutorialHighlight(bool enable)
		{
			if (this.m_TutorialFirstFreeSlot != null)
			{
				this.m_TutorialFirstFreeSlot.SetActive(enable);
			}
		}

		// Token: 0x06004EBB RID: 20155 RVA: 0x00233250 File Offset: 0x00231450
		private void Refresh()
		{
			if (this.CharacterIcon == null || this.CharacterIcon.Data == null)
			{
				this.BuildAsEmpty();
				return;
			}
			this.BuildAsCharacter();
		}

		// Token: 0x06004EBC RID: 20156 RVA: 0x0023327A File Offset: 0x0023147A
		private void BuildAsEmpty()
		{
			if (this.m_CharacterGroup != null)
			{
				this.m_CharacterGroup.SetActive(false);
			}
			if (this.m_HireGroup != null)
			{
				this.m_HireGroup.SetActive(true);
			}
		}

		// Token: 0x06004EBD RID: 20157 RVA: 0x002332B0 File Offset: 0x002314B0
		private void BuildAsCharacter()
		{
			if (this.m_HireGroup != null)
			{
				this.m_HireGroup.SetActive(false);
			}
			if (this.m_CharacterGroup != null)
			{
				this.m_CharacterGroup.gameObject.SetActive(true);
				this.CharacterIcon.transform.SetParent(this.m_CharacterGroup.transform, false);
				UICommon.FillParent(this.CharacterIcon.transform as RectTransform);
			}
		}

		// Token: 0x06004EBE RID: 20158 RVA: 0x00233327 File Offset: 0x00231527
		private void HandleEmptySlotPointerEvent(Hotspot hotspot, EventTriggerType eventType, PointerEventData e)
		{
			Action<UIRoyalCourt.UICourtSlot, EventTriggerType, PointerEventData> onCourtSlotPointerEvent = this.OnCourtSlotPointerEvent;
			if (onCourtSlotPointerEvent == null)
			{
				return;
			}
			onCourtSlotPointerEvent(this, eventType, e);
		}

		// Token: 0x06004EBF RID: 20159 RVA: 0x0023333C File Offset: 0x0023153C
		private void HandleEmptySlotOnClick(BSGButton b)
		{
			if (this.OnSelect != null)
			{
				this.OnSelect(this);
			}
		}

		// Token: 0x06004EC0 RID: 20160 RVA: 0x00233327 File Offset: 0x00231527
		private void HandleCharacterIconOnPointerEvent(Hotspot icon, EventTriggerType eventType, PointerEventData e)
		{
			Action<UIRoyalCourt.UICourtSlot, EventTriggerType, PointerEventData> onCourtSlotPointerEvent = this.OnCourtSlotPointerEvent;
			if (onCourtSlotPointerEvent == null)
			{
				return;
			}
			onCourtSlotPointerEvent(this, eventType, e);
		}

		// Token: 0x06004EC1 RID: 20161 RVA: 0x00233352 File Offset: 0x00231552
		private void HandleCharacterIconOnFocus(UICharacterIcon icon)
		{
			Action<UIRoyalCourt.UICourtSlot> onFocus = this.OnFocus;
			if (onFocus == null)
			{
				return;
			}
			onFocus(this);
		}

		// Token: 0x06004EC2 RID: 20162 RVA: 0x0023333C File Offset: 0x0023153C
		private void HandleCharacterIconOnSelect(UICharacterIcon icon)
		{
			if (this.OnSelect != null)
			{
				this.OnSelect(this);
			}
		}

		// Token: 0x06004EC3 RID: 20163 RVA: 0x00233365 File Offset: 0x00231565
		public void Highlight(bool highlight)
		{
			UICharacterIcon characterIcon = this.CharacterIcon;
			if (((characterIcon != null) ? characterIcon.Data : null) == null && this.m_ButtonEmpty != null)
			{
				this.m_ButtonEmpty.SetSelected(highlight, false);
			}
		}

		// Token: 0x06004EC4 RID: 20164 RVA: 0x00233398 File Offset: 0x00231598
		private void CreateChracterIcon()
		{
			UICharacterIcon component = UnityEngine.Object.Instantiate<GameObject>(this.CharacterIconPrototype, Vector3.zero, Quaternion.identity, this.GetIconContainer()).GetComponent<UICharacterIcon>();
			if (component != null)
			{
				component.ShowCrest(false);
				component.OnSelect += this.HandleCharacterIconOnSelect;
				component.OnFocus += this.HandleCharacterIconOnFocus;
				component.OnPointerEvent += this.HandleCharacterIconOnPointerEvent;
				this.CharacterIcon = component;
			}
		}

		// Token: 0x06004EC5 RID: 20165 RVA: 0x0002C53B File Offset: 0x0002A73B
		public override bool AcceptsDrop()
		{
			return true;
		}

		// Token: 0x06004EC6 RID: 20166 RVA: 0x00233414 File Offset: 0x00231614
		public override GameObject GetDragObject()
		{
			UICharacterIcon characterIcon = this.CharacterIcon;
			if (((characterIcon != null) ? characterIcon.Data : null) == null)
			{
				return null;
			}
			UICharacterIcon characterIcon2 = this.CharacterIcon;
			Logic.Character character = (characterIcon2 != null) ? characterIcon2.Data : null;
			Logic.Kingdom kingdom = BaseUI.LogicKingdom();
			if (character == ((kingdom != null) ? kingdom.GetKing() : null))
			{
				return null;
			}
			return global::Common.FindChildByName(base.gameObject, "Group_Populated", true, true);
		}

		// Token: 0x06004EC7 RID: 20167 RVA: 0x00233470 File Offset: 0x00231670
		public override void PostProcessDragObject(GameObject obj)
		{
			GameObject gameObject = global::Common.FindChildByName(obj, "id_ActionIcon", true, true);
			if (gameObject != null)
			{
				gameObject.SetActive(false);
			}
			GameObject gameObject2 = global::Common.FindChildByName(obj, "id_ActionProgress", true, true);
			if (gameObject2 == null)
			{
				return;
			}
			gameObject2.SetActive(false);
		}

		// Token: 0x06004EC8 RID: 20168 RVA: 0x002334A4 File Offset: 0x002316A4
		public override string ValidateDrop(Hotspot src_hotspot, GameObject dragged_obj)
		{
			UIRoyalCourt.UICourtSlot uicourtSlot;
			if ((uicourtSlot = (src_hotspot as UIRoyalCourt.UICourtSlot)) == null)
			{
				return null;
			}
			if (uicourtSlot == this)
			{
				return null;
			}
			UICharacterIcon characterIcon = this.CharacterIcon;
			if (((characterIcon != null) ? characterIcon.Data : null) != null)
			{
				UICharacterIcon characterIcon2 = this.CharacterIcon;
				Logic.Character character = (characterIcon2 != null) ? characterIcon2.Data : null;
				Logic.Kingdom kingdom = BaseUI.LogicKingdom();
				if (character == ((kingdom != null) ? kingdom.GetKing() : null))
				{
					return null;
				}
			}
			UICharacterIcon characterIcon3 = uicourtSlot.CharacterIcon;
			if (((characterIcon3 != null) ? characterIcon3.Data : null) == null)
			{
				return null;
			}
			UICharacterIcon characterIcon4 = this.CharacterIcon;
			if (((characterIcon4 != null) ? characterIcon4.Data : null) == null)
			{
				return "move_court_member";
			}
			return "swap_court_members";
		}

		// Token: 0x06004EC9 RID: 20169 RVA: 0x00233538 File Offset: 0x00231738
		public override bool AcceptDrop(string operation, Hotspot src_hotspot, GameObject dragged_obj)
		{
			UIRoyalCourt.UICourtSlot uicourtSlot;
			if ((uicourtSlot = (src_hotspot as UIRoyalCourt.UICourtSlot)) == null)
			{
				return false;
			}
			if (uicourtSlot == this)
			{
				return false;
			}
			Logic.Kingdom kingdom = BaseUI.LogicKingdom();
			UICharacterIcon characterIcon = this.CharacterIcon;
			if (((characterIcon != null) ? characterIcon.Data : null) != null)
			{
				UICharacterIcon characterIcon2 = this.CharacterIcon;
				if (((characterIcon2 != null) ? characterIcon2.Data : null) == ((kingdom != null) ? kingdom.GetKing() : null))
				{
					return false;
				}
			}
			UICharacterIcon characterIcon3 = uicourtSlot.CharacterIcon;
			Logic.Character character = (characterIcon3 != null) ? characterIcon3.Data : null;
			if (character == null)
			{
				return false;
			}
			Logic.Kingdom kingdom2;
			if (character.IsInSpecialCourt() && character.IsInSpecialCourt(kingdom))
			{
				kingdom2 = kingdom;
			}
			else
			{
				kingdom2 = character.GetKingdom();
			}
			if (kingdom2 == null)
			{
				return false;
			}
			kingdom2.SwapCourtSlots(uicourtSlot.courtIndex, this.courtIndex, true);
			return true;
		}

		// Token: 0x04003CCE RID: 15566
		[UIFieldTarget("id_HireGroup")]
		private GameObject m_HireGroup;

		// Token: 0x04003CCF RID: 15567
		[UIFieldTarget("id_CharacterGroup")]
		private GameObject m_CharacterGroup;

		// Token: 0x04003CD0 RID: 15568
		[UIFieldTarget("tut_FirstFreeSlot")]
		private GameObject m_TutorialFirstFreeSlot;

		// Token: 0x04003CD1 RID: 15569
		public GameObject CharacterIconPrototype;

		// Token: 0x04003CD2 RID: 15570
		private BSGButton m_ButtonEmpty;

		// Token: 0x04003CD4 RID: 15572
		public int courtIndex;

		// Token: 0x04003CD8 RID: 15576
		private bool inited;
	}
}
