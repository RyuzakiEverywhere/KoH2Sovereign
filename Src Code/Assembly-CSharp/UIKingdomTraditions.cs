using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x02000296 RID: 662
public class UIKingdomTraditions : MonoBehaviour, IListener
{
	// Token: 0x170001F5 RID: 501
	// (get) Token: 0x060028C7 RID: 10439 RVA: 0x0015C496 File Offset: 0x0015A696
	// (set) Token: 0x060028C8 RID: 10440 RVA: 0x0015C49E File Offset: 0x0015A69E
	public Logic.Kingdom Data { get; private set; }

	// Token: 0x060028C9 RID: 10441 RVA: 0x0015C4A8 File Offset: 0x0015A6A8
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_TraditionsSelectorHost != null)
		{
			this.traditionSelector = this.m_TraditionsSelectorHost.AddComponent<UIKingdomTraditionsSelector>();
			this.traditionSelector.OnClose = new Action(this.HandleOnSelectorClose);
		}
		this.m_Initialized = true;
	}

	// Token: 0x060028CA RID: 10442 RVA: 0x0015C502 File Offset: 0x0015A702
	public void SetData(Logic.Kingdom k)
	{
		this.Init();
		Logic.Kingdom data = this.Data;
		if (data != null)
		{
			data.DelListener(this);
		}
		this.Data = k;
		Logic.Kingdom data2 = this.Data;
		if (data2 != null)
		{
			data2.AddListener(this);
		}
		this.PopulateTraditionSlots();
		this.UpdateTraditions();
	}

	// Token: 0x060028CB RID: 10443 RVA: 0x0015C544 File Offset: 0x0015A744
	private void PopulateTraditionSlots()
	{
		if (this.m_TraditionSlotObjects == null)
		{
			return;
		}
		if (this.Data == null)
		{
			return;
		}
		this.m_TraditionSlots.Clear();
		new Dictionary<int, int>();
		for (int i = 0; i < this.Data.tradition_slots_types.Length; i++)
		{
			GameObject gameObject = (this.m_TraditionSlotObjects != null && this.m_TraditionSlotObjects.Length > i) ? this.m_TraditionSlotObjects[i] : null;
			if (gameObject == null)
			{
				break;
			}
			TraditionSlot orAddComponent = gameObject.GetOrAddComponent<TraditionSlot>();
			orAddComponent.OnSelect = new Action<TraditionSlot>(this.OpenTraditionSelector);
			Tradition.Def def;
			if (this.Data.traditions == null || this.Data.traditions.Count <= i)
			{
				def = null;
			}
			else
			{
				Tradition tradition = this.Data.traditions[i];
				def = ((tradition != null) ? tradition.def : null);
			}
			Tradition.Def def2 = def;
			orAddComponent.SetData(this.Data, i, def2, null);
			gameObject.SetActive(true);
			this.m_TraditionSlots.Add(orAddComponent);
		}
	}

	// Token: 0x060028CC RID: 10444 RVA: 0x0015C63C File Offset: 0x0015A83C
	private void UpdateTraditions()
	{
		if (this.Data == null)
		{
			return;
		}
		if (this.m_TraditionSlotObjects == null || this.m_TraditionSlots.Count == 0)
		{
			return;
		}
		if (this.Data.tradition_slots_types == null || this.Data.tradition_slots_types.Length == 0)
		{
			return;
		}
		for (int i = 0; i < this.Data.tradition_slots_types.Length; i++)
		{
			TraditionSlot traditionSlot = (this.m_TraditionSlots.Count > i) ? this.m_TraditionSlots[i] : null;
			if (traditionSlot == null)
			{
				break;
			}
			Tradition.Def def;
			if (this.Data.traditions == null || this.Data.traditions.Count <= i)
			{
				def = null;
			}
			else
			{
				Tradition tradition = this.Data.traditions[i];
				def = ((tradition != null) ? tradition.def : null);
			}
			Tradition.Def def2 = def;
			traditionSlot.SetData(this.Data, i, def2, null);
		}
		this.RefreshFirstFreeTutorialHighlightSlot();
	}

	// Token: 0x060028CD RID: 10445 RVA: 0x0015C71C File Offset: 0x0015A91C
	private void RefreshFirstFreeTutorialHighlightSlot()
	{
		bool flag = false;
		for (int i = 0; i < this.m_TraditionSlots.Count; i++)
		{
			TraditionSlot traditionSlot = this.m_TraditionSlots[i];
			bool flag2 = !flag && traditionSlot.IsFree();
			traditionSlot.EnableTutorialHighlight(flag2);
			if (flag2)
			{
				flag = true;
			}
		}
	}

	// Token: 0x060028CE RID: 10446 RVA: 0x0015C768 File Offset: 0x0015A968
	private void OpenTraditionSelector(TraditionSlot s)
	{
		if (s.IsLocked())
		{
			return;
		}
		this.m_TraditionSlotsContainer.gameObject.SetActive(false);
		UIKingdomTraditionsSelector uikingdomTraditionsSelector = this.traditionSelector;
		if (uikingdomTraditionsSelector != null)
		{
			uikingdomTraditionsSelector.gameObject.SetActive(true);
		}
		UIKingdomTraditionsSelector uikingdomTraditionsSelector2 = this.traditionSelector;
		if (uikingdomTraditionsSelector2 == null)
		{
			return;
		}
		uikingdomTraditionsSelector2.SetData(this.Data, s.Index);
	}

	// Token: 0x060028CF RID: 10447 RVA: 0x0015C7C2 File Offset: 0x0015A9C2
	private void HandleOnSelectorClose()
	{
		this.m_TraditionSlotsContainer.gameObject.SetActive(true);
		UIKingdomTraditionsSelector uikingdomTraditionsSelector = this.traditionSelector;
		if (uikingdomTraditionsSelector != null)
		{
			uikingdomTraditionsSelector.gameObject.SetActive(false);
		}
		UIKingdomTraditionsSelector uikingdomTraditionsSelector2 = this.traditionSelector;
		if (uikingdomTraditionsSelector2 == null)
		{
			return;
		}
		uikingdomTraditionsSelector2.SetData(null, -1);
	}

	// Token: 0x060028D0 RID: 10448 RVA: 0x0015C7FE File Offset: 0x0015A9FE
	public void Clear()
	{
		Logic.Kingdom data = this.Data;
		if (data != null)
		{
			data.DelListener(this);
		}
		this.Data = null;
		this.HandleOnSelectorClose();
	}

	// Token: 0x060028D1 RID: 10449 RVA: 0x0015C81F File Offset: 0x0015AA1F
	private void OnDestroy()
	{
		this.Clear();
	}

	// Token: 0x060028D2 RID: 10450 RVA: 0x0015C827 File Offset: 0x0015AA27
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "traditions_changed")
		{
			this.UpdateTraditions();
		}
	}

	// Token: 0x04001B95 RID: 7061
	[UIFieldTarget("id_TraditionSlotsContainer")]
	private RectTransform m_TraditionSlotsContainer;

	// Token: 0x04001B96 RID: 7062
	[UIFieldTarget("id_TraditionSlotPrototype")]
	private GameObject[] m_TraditionSlotObjects;

	// Token: 0x04001B97 RID: 7063
	[UIFieldTarget("id_TraditionsSelector")]
	private GameObject m_TraditionsSelectorHost;

	// Token: 0x04001B98 RID: 7064
	private List<TraditionSlot> m_TraditionSlots = new List<TraditionSlot>();

	// Token: 0x04001B99 RID: 7065
	private UIKingdomTraditionsSelector traditionSelector;

	// Token: 0x04001B9B RID: 7067
	private bool m_Initialized;
}
