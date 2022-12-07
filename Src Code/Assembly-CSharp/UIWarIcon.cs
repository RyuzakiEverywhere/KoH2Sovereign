using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200030C RID: 780
public class UIWarIcon : ObjectIcon, IListener, IPoolable, Tooltip.IHandler
{
	// Token: 0x17000260 RID: 608
	// (get) Token: 0x060030AA RID: 12458 RVA: 0x00189855 File Offset: 0x00187A55
	// (set) Token: 0x060030AB RID: 12459 RVA: 0x0018985D File Offset: 0x00187A5D
	public War War { get; private set; }

	// Token: 0x1400003F RID: 63
	// (add) Token: 0x060030AC RID: 12460 RVA: 0x00189868 File Offset: 0x00187A68
	// (remove) Token: 0x060030AD RID: 12461 RVA: 0x001898A0 File Offset: 0x00187AA0
	public event Action<UIWarIcon> OnSelect;

	// Token: 0x14000040 RID: 64
	// (add) Token: 0x060030AE RID: 12462 RVA: 0x001898D8 File Offset: 0x00187AD8
	// (remove) Token: 0x060030AF RID: 12463 RVA: 0x00189910 File Offset: 0x00187B10
	public event Action<UIWarIcon> OnFocus;

	// Token: 0x060030B0 RID: 12464 RVA: 0x00189945 File Offset: 0x00187B45
	public override void Awake()
	{
		base.Awake();
		this.m_WasActivated = true;
		if (this.m_AddListeners)
		{
			War war = this.War;
			if (war == null)
			{
				return;
			}
			war.AddListener(this);
		}
	}

	// Token: 0x060030B1 RID: 12465 RVA: 0x00189970 File Offset: 0x00187B70
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.tooltip = Tooltip.Get(base.gameObject, true);
		this.tooltip.handler = new Tooltip.Handler(this.HandleTooltip);
		this.tooltip.SetDef("WarTooltip", null);
		this.tooltip.SetVars(new Vars(BaseUI.LogicKingdom()));
		this.m_Initialzied = true;
	}

	// Token: 0x060030B2 RID: 12466 RVA: 0x001899EC File Offset: 0x00187BEC
	private void SetIcon(War war)
	{
		if (war == null)
		{
			return;
		}
		if (this.m_Icon != null)
		{
			Logic.Kingdom item = BaseUI.LogicKingdom();
			if (war.attackers.Contains(item) || war.defenders.Contains(item))
			{
				this.m_Icon.sprite = global::Defs.GetObj<Sprite>(war.def.field, "icon", null);
				return;
			}
			this.m_Icon.sprite = global::Defs.GetObj<Sprite>(war.def.field, "icon_third_party", null);
		}
	}

	// Token: 0x060030B3 RID: 12467 RVA: 0x00189A70 File Offset: 0x00187C70
	public override void SetObject(object obj, Vars vars = null)
	{
		this.Init();
		this.observer = null;
		this.SetIcon(obj as War);
		if (this.logicObject == obj)
		{
			return;
		}
		if (this.War != null)
		{
			this.War.DelListener(this);
		}
		base.SetObject(obj, vars);
		if (obj is War)
		{
			this.War = (obj as War);
		}
		this.vars = ((vars != null) ? vars : new Vars(this.War));
		if (this.m_WasActivated)
		{
			this.War.AddListener(this);
		}
		else
		{
			this.m_AddListeners = true;
		}
		this.RefreshCrest();
		this.tooltip.vars.Clear();
		this.tooltip.vars.obj = this.War;
		if (this.enemy != null)
		{
			this.tooltip.vars.Set<Logic.Kingdom>("target", this.enemy);
		}
		if (this.player != null)
		{
			this.tooltip.vars.Set<Logic.Kingdom>("kingdom", this.player);
		}
	}

	// Token: 0x060030B4 RID: 12468 RVA: 0x00189B80 File Offset: 0x00187D80
	public void ShowGlow(bool shown)
	{
		if (this.m_Glow)
		{
			this.m_Glow.gameObject.SetActive(shown);
		}
	}

	// Token: 0x060030B5 RID: 12469 RVA: 0x00189BA0 File Offset: 0x00187DA0
	private void ExtractSides()
	{
		this.player = null;
		this.enemy = null;
		if (this.War == null)
		{
			return;
		}
		this.player = BaseUI.LogicKingdom();
		this.enemy = ((this.observer != null) ? this.War.GetEnemyLeader(this.observer) : this.War.GetEnemyLeader(this.player));
		Tooltip tooltip = this.tooltip;
		if (((tooltip != null) ? tooltip.vars : null) != null)
		{
			if (this.enemy != null)
			{
				this.tooltip.vars.Set<Logic.Kingdom>("target", this.enemy);
			}
			if (this.player != null)
			{
				this.tooltip.vars.Set<Logic.Kingdom>("kingdom", this.player);
			}
		}
	}

	// Token: 0x060030B6 RID: 12470 RVA: 0x00189C5B File Offset: 0x00187E5B
	public void SetObserver(Logic.Kingdom observer)
	{
		this.observer = observer;
		this.RefreshCrest();
	}

	// Token: 0x060030B7 RID: 12471 RVA: 0x00189C6C File Offset: 0x00187E6C
	private void RefreshCrest()
	{
		this.ExtractSides();
		if (this.enemy != null)
		{
			this.m_EnemeyCrest.gameObject.SetActive(true);
			this.m_EnemeyCrest.SetObject(this.enemy, null);
			return;
		}
		if (this.War.IsJihad())
		{
			this.m_EnemeyCrest.gameObject.SetActive(true);
			this.m_EnemeyCrest.SetObject(this.War.attacker.IsCaliphate() ? this.War.defender : this.War.attacker, null);
			return;
		}
		this.m_EnemeyCrest.gameObject.SetActive(false);
	}

	// Token: 0x060030B8 RID: 12472 RVA: 0x000023FD File Offset: 0x000005FD
	public void UpdateHighlight()
	{
	}

	// Token: 0x060030B9 RID: 12473 RVA: 0x00189D14 File Offset: 0x00187F14
	public override void OnClick(PointerEventData e)
	{
		base.OnClick(e);
		if (e.button == PointerEventData.InputButton.Left)
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
		this.UpdateHighlight();
	}

	// Token: 0x060030BA RID: 12474 RVA: 0x00189D84 File Offset: 0x00187F84
	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x060030BB RID: 12475 RVA: 0x00189D93 File Offset: 0x00187F93
	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x060030BC RID: 12476 RVA: 0x00113DCB File Offset: 0x00111FCB
	public override void OnRightClick(PointerEventData e)
	{
		base.OnRightClick(e);
	}

	// Token: 0x060030BD RID: 12477 RVA: 0x00189DA4 File Offset: 0x00187FA4
	public static string GetWarBonusesText(War war, Logic.Kingdom k)
	{
		Dictionary<string, War.Bonus> dictionary = (war != null) ? war.GetBonuses(k) : null;
		if (dictionary == null)
		{
			return null;
		}
		string text = "";
		Vars vars = war.CreateWarBonusConditionVars(k);
		foreach (KeyValuePair<string, War.Bonus> keyValuePair in dictionary)
		{
			War.Bonus value = keyValuePair.Value;
			if (value.condition.Value(vars, true, true))
			{
				Vars vars2 = new Vars();
				vars2.Set<DT.Field>("name", value.field.FindChild("name", null, true, true, true, '.'));
				vars2.Set<float>("value", value.value);
				text += global::Defs.Localize("War.bonus_text", vars2, null, true, true);
			}
		}
		if (!string.IsNullOrEmpty(text))
		{
			return text;
		}
		return null;
	}

	// Token: 0x060030BE RID: 12478 RVA: 0x00189E90 File Offset: 0x00188090
	public bool HandleTooltip(BaseUI ui, Tooltip tooltip, Tooltip.Event evt)
	{
		if (evt == Tooltip.Event.Fill || evt == Tooltip.Event.Update)
		{
			if (BaseUI.LogicKingdom() != this.player)
			{
				this.ExtractSides();
			}
			tooltip.vars.Set<string>("war_score", UIWarsOverviewWindow.GetWarScore(this.War, this.player, this.vars));
			string warBonusesText = UIWarIcon.GetWarBonusesText(this.War, this.player);
			if (warBonusesText != null)
			{
				tooltip.vars.Set<string>("bonuses_text", "#" + warBonusesText);
			}
			else
			{
				tooltip.vars.Set<Value>("bonuses_text", Value.Null);
			}
		}
		return false;
	}

	// Token: 0x060030BF RID: 12479 RVA: 0x00189F2C File Offset: 0x0018812C
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "type_changed")
		{
			this.SetIcon(this.War);
			this.RefreshCrest();
			return;
		}
		if (!(message == "destroying"))
		{
			return;
		}
		Logic.Object @object = obj as Logic.Object;
		if (@object == null)
		{
			return;
		}
		@object.DelListener(this);
	}

	// Token: 0x060030C0 RID: 12480 RVA: 0x00189F78 File Offset: 0x00188178
	private void OnDestroy()
	{
		AspectRatioFitter component = base.GetComponent<AspectRatioFitter>();
		if (component != null)
		{
			UnityEngine.Object.DestroyImmediate(component);
		}
		this.OnSelect = null;
		this.OnFocus = null;
		War war = this.War;
		if (war != null)
		{
			war.DelListener(this);
		}
		this.War = null;
		this.logicObject = null;
	}

	// Token: 0x060030C1 RID: 12481 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolSpawned()
	{
	}

	// Token: 0x060030C2 RID: 12482 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolActivated()
	{
	}

	// Token: 0x060030C3 RID: 12483 RVA: 0x00189FC9 File Offset: 0x001881C9
	public void OnPoolDeactivated()
	{
		this.OnDestroy();
	}

	// Token: 0x060030C4 RID: 12484 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolDestroyed()
	{
	}

	// Token: 0x040020A2 RID: 8354
	[UIFieldTarget("id_EnemyCrest")]
	private UIKingdomIcon m_EnemeyCrest;

	// Token: 0x040020A3 RID: 8355
	[UIFieldTarget("id_Icon")]
	private Image m_Icon;

	// Token: 0x040020A4 RID: 8356
	[UIFieldTarget("id_Glow")]
	private GameObject m_Glow;

	// Token: 0x040020A5 RID: 8357
	[UIFieldTarget("id_Background")]
	private UIKingdomIcon m_Background;

	// Token: 0x040020A7 RID: 8359
	private Logic.Kingdom player;

	// Token: 0x040020A8 RID: 8360
	private Logic.Kingdom enemy;

	// Token: 0x040020A9 RID: 8361
	private Logic.Kingdom observer;

	// Token: 0x040020AC RID: 8364
	private bool m_Initialzied;

	// Token: 0x040020AD RID: 8365
	private bool m_WasActivated;

	// Token: 0x040020AE RID: 8366
	private bool m_AddListeners;

	// Token: 0x040020AF RID: 8367
	private Tooltip tooltip;
}
