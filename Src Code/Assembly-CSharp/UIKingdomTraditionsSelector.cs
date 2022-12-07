using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000214 RID: 532
public class UIKingdomTraditionsSelector : MonoBehaviour, IListener
{
	// Token: 0x1700019E RID: 414
	// (get) Token: 0x06002037 RID: 8247 RVA: 0x001280AE File Offset: 0x001262AE
	// (set) Token: 0x06002038 RID: 8248 RVA: 0x001280B6 File Offset: 0x001262B6
	public Logic.Kingdom Data { get; private set; }

	// Token: 0x1700019F RID: 415
	// (get) Token: 0x06002039 RID: 8249 RVA: 0x001280BF File Offset: 0x001262BF
	// (set) Token: 0x0600203A RID: 8250 RVA: 0x001280C7 File Offset: 0x001262C7
	public int TraditionSlotIndex { get; private set; } = -1;

	// Token: 0x0600203B RID: 8251 RVA: 0x001280D0 File Offset: 0x001262D0
	private void Init()
	{
		if (this.m_Initiazled)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_Back != null)
		{
			this.m_Back.onClick = new BSGButton.OnClick(this.HandleOnClose);
		}
		if (this.m_TraditionsPickerHost != null)
		{
			this.m_TraditionsPicker = this.m_TraditionsPickerHost.AddComponent<UIKingdomTraditionsSelector.TraditionsPicker>();
			UIKingdomTraditionsSelector.TraditionsPicker traditionsPicker = this.m_TraditionsPicker;
			traditionsPicker.OnSelect = (Action<Tradition.Def>)Delegate.Combine(traditionsPicker.OnSelect, new Action<Tradition.Def>(this.HandleOnPickerData));
		}
		if (this.m_BackLabel != null)
		{
			UIText.SetTextKey(this.m_BackLabel, "KingdomTraditionsWindow.back", null, null);
		}
		this.m_Initiazled = true;
	}

	// Token: 0x0600203C RID: 8252 RVA: 0x00128180 File Offset: 0x00126380
	public void SetData(Logic.Kingdom k, int slotIndex)
	{
		this.Init();
		Logic.Kingdom data = this.Data;
		if (data != null)
		{
			data.DelListener(this);
		}
		this.Data = k;
		this.TraditionSlotIndex = slotIndex;
		Logic.Kingdom data2 = this.Data;
		if (data2 != null)
		{
			data2.AddListener(this);
		}
		this.m_SelectedTraditionDef = this.GetTraditonAtSlot(k, slotIndex);
		this.m_TraditionsPicker.SetData(this.Data, this.TraditionSlotIndex);
		this.Refresh();
	}

	// Token: 0x0600203D RID: 8253 RVA: 0x001281F0 File Offset: 0x001263F0
	private Tradition.Def GetTraditonAtSlot(Logic.Kingdom k, int index)
	{
		if (k == null)
		{
			return null;
		}
		if (k.traditions == null)
		{
			return null;
		}
		if (k.traditions.Count <= index)
		{
			return null;
		}
		Tradition tradition = k.traditions[index];
		if (tradition == null)
		{
			return null;
		}
		return tradition.def;
	}

	// Token: 0x0600203E RID: 8254 RVA: 0x000023FD File Offset: 0x000005FD
	private void Refresh()
	{
	}

	// Token: 0x0600203F RID: 8255 RVA: 0x00128228 File Offset: 0x00126428
	private void Select(Tradition.Def adoptee = null)
	{
		this.m_SelectedTraditionDef = adoptee;
		this.TryAdoptTradition();
	}

	// Token: 0x06002040 RID: 8256 RVA: 0x00128237 File Offset: 0x00126437
	private void HandleOnPickerData(Tradition.Def def)
	{
		if (this.Data == null)
		{
			return;
		}
		this.FindTraditionIndex(this.Data, def);
		this.Select(def);
	}

	// Token: 0x06002041 RID: 8257 RVA: 0x00128258 File Offset: 0x00126458
	public int FindTraditionIndex(Logic.Kingdom k, Tradition.Def def)
	{
		if (def == null)
		{
			return -1;
		}
		if (k == null)
		{
			return -1;
		}
		if (k.traditions == null)
		{
			return -1;
		}
		for (int i = 0; i < k.traditions.Count; i++)
		{
			Tradition tradition = k.traditions[i];
			if (tradition != null && tradition.def == def)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06002042 RID: 8258 RVA: 0x001282AC File Offset: 0x001264AC
	public void OnMessage(object obj, string message, object param)
	{
		if (this == null || base.gameObject == null)
		{
			Logic.Kingdom data = this.Data;
			if (data == null)
			{
				return;
			}
			data.DelListener(this);
			return;
		}
		else
		{
			if (message == "destroying" || message == "finishing")
			{
				this.SetData(null, -1);
				return;
			}
			if (!(message == "traditions_changed"))
			{
				return;
			}
			this.Refresh();
			return;
		}
	}

	// Token: 0x06002043 RID: 8259 RVA: 0x0012831C File Offset: 0x0012651C
	private void HandleAbandonTradition(BSGButton button)
	{
		if (this.Data == null)
		{
			return;
		}
		if (this.m_SelectedTraditionDef == null)
		{
			return;
		}
		if (this.Data.traditions == null || this.Data.traditions.Count == 0)
		{
			return;
		}
		Tradition tradition = this.Data.traditions.Find((Tradition x) => ((x != null) ? x.def : null) == this.m_SelectedTraditionDef);
		if (tradition == null)
		{
			return;
		}
		Action action = this.Data.actions.Find("AbandonTraditionAction");
		if (action != null)
		{
			if (action.args != null)
			{
				action.args = null;
			}
			action.AddArg(ref action.args, tradition.def.id, 0);
			ActionVisuals.ExecuteAction(action);
		}
	}

	// Token: 0x06002044 RID: 8260 RVA: 0x001283C8 File Offset: 0x001265C8
	private void HandleImproveTradition(BSGButton button)
	{
		if (this.Data == null)
		{
			return;
		}
		if (this.m_SelectedTraditionDef == null)
		{
			return;
		}
		if (this.Data.traditions == null || this.Data.traditions.Count == 0)
		{
			return;
		}
		Tradition tradition = this.Data.traditions.Find((Tradition x) => x.def == this.m_SelectedTraditionDef);
		if (tradition == null)
		{
			return;
		}
		Action action = this.Data.actions.Find("IncreaseRankTraditionAction");
		if (action != null)
		{
			if (action.args != null)
			{
				action.args = null;
			}
			action.AddArg(ref action.args, tradition.def.id, 0);
			ActionVisuals.ExecuteAction(action);
		}
	}

	// Token: 0x06002045 RID: 8261 RVA: 0x00128474 File Offset: 0x00126674
	private void TryAdoptTradition()
	{
		if (this.Data == null)
		{
			return;
		}
		if (this.m_SelectedTraditionDef == null)
		{
			return;
		}
		Tradition tradition = null;
		if (this.Data.traditions != null && this.Data.traditions.Count > 0)
		{
			tradition = this.Data.traditions.Find((Tradition x) => ((x != null) ? x.def : null) == this.m_SelectedTraditionDef);
		}
		if (tradition == null)
		{
			Action action = this.Data.actions.Find("AddAnyTraditionAction");
			if (action != null)
			{
				int idx = this.TraditionSlotIndex;
				if (this.Data.GetTradition(idx) != null)
				{
					idx = this.Data.GetFreeTraditionIndex(Tradition.Type.All);
				}
				if (action.args != null)
				{
					action.args = null;
				}
				action.AddArg(ref action.args, this.m_SelectedTraditionDef.id, 0);
				action.AddArg(ref action.args, this.TraditionSlotIndex, 1);
				if (action.Execute(null))
				{
					this.Close();
				}
			}
		}
	}

	// Token: 0x06002046 RID: 8262 RVA: 0x00128566 File Offset: 0x00126766
	protected static string GetAdoptionState(Logic.Kingdom k, Tradition.Def def)
	{
		if (k == null || def == null)
		{
			return "unknown";
		}
		if (k.HasTradition(def))
		{
			return "adopted";
		}
		if (!k.CheckTraditionRequirements(def))
		{
			return "reqierment_not_met";
		}
		return "adoptable";
	}

	// Token: 0x06002047 RID: 8263 RVA: 0x00128597 File Offset: 0x00126797
	private void HandleOnClose(BSGButton button)
	{
		this.Close();
	}

	// Token: 0x06002048 RID: 8264 RVA: 0x001285A0 File Offset: 0x001267A0
	private void Close()
	{
		Logic.Kingdom data = this.Data;
		object obj;
		if (data == null)
		{
			obj = null;
		}
		else
		{
			Actions actions = data.actions;
			obj = ((actions != null) ? actions.Find("AddAnyTraditionAction") : null);
		}
		object obj2 = obj;
		if (obj2 != null)
		{
			List<Value> args = obj2.args;
			if (args != null)
			{
				args.Clear();
			}
		}
		Logic.Kingdom data2 = this.Data;
		if (data2 != null)
		{
			data2.DelListener(this);
		}
		this.Data = null;
		this.TraditionSlotIndex = -1;
		Action onClose = this.OnClose;
		if (onClose == null)
		{
			return;
		}
		onClose();
	}

	// Token: 0x04001564 RID: 5476
	[UIFieldTarget("id_TraditionsPicker")]
	private GameObject m_TraditionsPickerHost;

	// Token: 0x04001565 RID: 5477
	[UIFieldTarget("id_Back")]
	private BSGButton m_Back;

	// Token: 0x04001566 RID: 5478
	[UIFieldTarget("id_BackLabel")]
	private TextMeshProUGUI m_BackLabel;

	// Token: 0x04001567 RID: 5479
	public Action OnClose;

	// Token: 0x0400156A RID: 5482
	private UIKingdomTraditionsSelector.TraditionsPicker m_TraditionsPicker;

	// Token: 0x0400156B RID: 5483
	private Tradition.Def m_SelectedTraditionDef;

	// Token: 0x0400156C RID: 5484
	private bool m_Initiazled;

	// Token: 0x0200074F RID: 1871
	internal class TraditionsPicker : MonoBehaviour
	{
		// Token: 0x170005A9 RID: 1449
		// (get) Token: 0x06004A97 RID: 19095 RVA: 0x0022165B File Offset: 0x0021F85B
		// (set) Token: 0x06004A98 RID: 19096 RVA: 0x00221663 File Offset: 0x0021F863
		public Logic.Kingdom Data { get; private set; }

		// Token: 0x170005AA RID: 1450
		// (get) Token: 0x06004A99 RID: 19097 RVA: 0x0022166C File Offset: 0x0021F86C
		// (set) Token: 0x06004A9A RID: 19098 RVA: 0x00221674 File Offset: 0x0021F874
		public int PreferedSlotIndex { get; private set; }

		// Token: 0x06004A9B RID: 19099 RVA: 0x0022167D File Offset: 0x0021F87D
		private void Init()
		{
			if (this.m_Initalized)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			this.m_Initalized = true;
		}

		// Token: 0x06004A9C RID: 19100 RVA: 0x00221696 File Offset: 0x0021F896
		private void OnEnable()
		{
			this.Init();
			RectTransform traditionsSelectorContainer = this.m_TraditionsSelectorContainer;
			TooltipPlacement.AddBlocker((traditionsSelectorContainer != null) ? traditionsSelectorContainer.gameObject : null, null);
		}

		// Token: 0x06004A9D RID: 19101 RVA: 0x002216B6 File Offset: 0x0021F8B6
		private void OnDisable()
		{
			RectTransform traditionsSelectorContainer = this.m_TraditionsSelectorContainer;
			TooltipPlacement.DelBlocker((traditionsSelectorContainer != null) ? traditionsSelectorContainer.gameObject : null);
		}

		// Token: 0x06004A9E RID: 19102 RVA: 0x002216CF File Offset: 0x0021F8CF
		public void SetData(Logic.Kingdom k, int prefred_slot_index)
		{
			this.Init();
			this.Data = k;
			this.PreferedSlotIndex = prefred_slot_index;
			this.UpdateSlots();
		}

		// Token: 0x06004A9F RID: 19103 RVA: 0x002216EC File Offset: 0x0021F8EC
		public void UpdateSlots()
		{
			this.m_Slots.Clear();
			if (this.Data == null)
			{
				return;
			}
			if (this.Data == null)
			{
				return;
			}
			if (this.m_TraditionsSelectorContainer == null)
			{
				return;
			}
			UICommon.DeleteChildren(this.m_TraditionsSelectorContainer);
			List<Tradition.Def> defs = this.Data.game.defs.GetDefs<Tradition.Def>();
			if (defs == null)
			{
				return;
			}
			GameObject prefab = UIDynastyTradition.GetPrefab("extended");
			for (int i = 0; i < defs.Count; i++)
			{
				Tradition.Def def = defs[i];
				Vars vars = new Vars();
				vars.Set<Logic.Kingdom>("owner", this.Data);
				vars.Set<int>("slot_index", this.PreferedSlotIndex);
				UIDynastyTradition uidynastyTradition = UIDynastyTradition.Create(def, prefab, this.m_TraditionsSelectorContainer, vars);
				if (!(uidynastyTradition == null))
				{
					uidynastyTradition.OnSelect += this.HandleSlotSelected;
					uidynastyTradition.ShowName(false);
					uidynastyTradition.SetHighlightAdopted(true);
					this.m_Slots.Add(uidynastyTradition);
					string adoptionState = UIKingdomTraditionsSelector.TraditionsPicker.GetAdoptionState(this.Data, def);
					uidynastyTradition.SetVisualState(adoptionState);
				}
			}
		}

		// Token: 0x06004AA0 RID: 19104 RVA: 0x00128566 File Offset: 0x00126766
		public static string GetAdoptionState(Logic.Kingdom k, Tradition.Def def)
		{
			if (k == null || def == null)
			{
				return "unknown";
			}
			if (k.HasTradition(def))
			{
				return "adopted";
			}
			if (!k.CheckTraditionRequirements(def))
			{
				return "reqierment_not_met";
			}
			return "adoptable";
		}

		// Token: 0x06004AA1 RID: 19105 RVA: 0x00221801 File Offset: 0x0021FA01
		private void Update()
		{
			if (UnityEngine.Time.unscaledTime > this.lastChangeCheck + this.checkInterval)
			{
				this.CheckSlotState();
				this.lastChangeCheck = UnityEngine.Time.unscaledTime;
			}
		}

		// Token: 0x06004AA2 RID: 19106 RVA: 0x00221828 File Offset: 0x0021FA28
		private void CheckSlotState()
		{
			for (int i = 0; i < this.m_Slots.Count; i++)
			{
				Tradition.Def def = this.m_Slots[i].def;
				string adoptionState = UIKingdomTraditionsSelector.TraditionsPicker.GetAdoptionState(this.Data, def);
				if (adoptionState != this.m_Slots[i].GetVisualState())
				{
					this.m_Slots[i].SetVisualState(adoptionState);
				}
			}
		}

		// Token: 0x06004AA3 RID: 19107 RVA: 0x00221895 File Offset: 0x0021FA95
		private void HandleSlotSelected(UIDynastyTradition icon, PointerEventData e)
		{
			Action<Tradition.Def> onSelect = this.OnSelect;
			if (onSelect == null)
			{
				return;
			}
			onSelect(icon.def);
		}

		// Token: 0x0400398A RID: 14730
		[UIFieldTarget("id_TraditionsContainer")]
		private RectTransform m_TraditionsSelectorContainer;

		// Token: 0x0400398C RID: 14732
		public Action<Tradition.Def> OnSelect;

		// Token: 0x0400398E RID: 14734
		private List<UIDynastyTradition> m_Slots = new List<UIDynastyTradition>();

		// Token: 0x0400398F RID: 14735
		private bool m_Initalized;

		// Token: 0x04003990 RID: 14736
		private float lastChangeCheck;

		// Token: 0x04003991 RID: 14737
		private float checkInterval = 0.5f;
	}
}
