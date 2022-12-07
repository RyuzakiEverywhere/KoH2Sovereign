using System;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000267 RID: 615
public class UIPactIcon : ObjectIcon, IListener, IPoolable
{
	// Token: 0x170001C4 RID: 452
	// (get) Token: 0x060025EA RID: 9706 RVA: 0x0014EC66 File Offset: 0x0014CE66
	// (set) Token: 0x060025EB RID: 9707 RVA: 0x0014EC6E File Offset: 0x0014CE6E
	public Pact Pact { get; private set; }

	// Token: 0x1400002F RID: 47
	// (add) Token: 0x060025EC RID: 9708 RVA: 0x0014EC78 File Offset: 0x0014CE78
	// (remove) Token: 0x060025ED RID: 9709 RVA: 0x0014ECB0 File Offset: 0x0014CEB0
	public event Action<UIPactIcon> OnSelect;

	// Token: 0x14000030 RID: 48
	// (add) Token: 0x060025EE RID: 9710 RVA: 0x0014ECE8 File Offset: 0x0014CEE8
	// (remove) Token: 0x060025EF RID: 9711 RVA: 0x0014ED20 File Offset: 0x0014CF20
	public event Action<UIPactIcon> OnFocus;

	// Token: 0x060025F0 RID: 9712 RVA: 0x0014ED55 File Offset: 0x0014CF55
	public override void Awake()
	{
		base.Awake();
		this.m_WasActivated = true;
		if (this.m_AddListeners)
		{
			Pact pact = this.Pact;
			if (pact == null)
			{
				return;
			}
			pact.AddListener(this);
		}
	}

	// Token: 0x060025F1 RID: 9713 RVA: 0x0014ED7D File Offset: 0x0014CF7D
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.tooltipVars = new Vars();
		this.m_Initialzied = true;
	}

	// Token: 0x060025F2 RID: 9714 RVA: 0x0014EDA4 File Offset: 0x0014CFA4
	public override void SetObject(object obj, Vars vars = null)
	{
		this.Init();
		if (this.logicObject == obj)
		{
			return;
		}
		if (this.Pact != null)
		{
			this.Pact.DelListener(this);
		}
		base.SetObject(obj, vars);
		if (obj is Pact)
		{
			this.Pact = (obj as Pact);
		}
		this.vars = ((vars != null) ? vars : new Vars(this.Pact));
		if (this.m_WasActivated)
		{
			this.Pact.AddListener(this);
		}
		else
		{
			this.m_AddListeners = true;
		}
		this.ExtractSides();
		if (this.m_Crest != null)
		{
			this.m_Crest.SetObject(this.m_ShowOwnerCrest ? this.Pact.leader : this.enemy, null);
		}
		if (this.m_Icon != null)
		{
			string key = "icon";
			if (this.Pact.members.Contains(BaseUI.LogicKingdom()))
			{
				key = "icon_positive";
			}
			else if (this.Pact.target == BaseUI.LogicKingdom())
			{
				key = "icon_negative";
			}
			Pact pact = this.Pact;
			DT.Field field;
			if (pact == null)
			{
				field = null;
			}
			else
			{
				Pact.Def def = pact.def;
				field = ((def != null) ? def.field : null);
			}
			Sprite obj2 = global::Defs.GetObj<Sprite>(field, key, null);
			if (obj2 == null)
			{
				this.m_Icon.gameObject.SetActive(false);
			}
			else
			{
				this.m_Icon.overrideSprite = obj2;
				this.m_Icon.gameObject.SetActive(true);
			}
		}
		this.tooltipVars.Clear();
		this.tooltipVars.obj = this.Pact;
		if (this.enemy != null)
		{
			this.tooltipVars.Set<Logic.Kingdom>("target", this.enemy);
		}
		if (this.player != null)
		{
			this.tooltipVars.Set<Logic.Kingdom>("kingdom", this.player);
		}
		this.RefreshTooltip();
	}

	// Token: 0x060025F3 RID: 9715 RVA: 0x0014EF75 File Offset: 0x0014D175
	public void ShowOwnerCrest(bool v)
	{
		this.m_ShowOwnerCrest = v;
		this.m_Crest.SetObject(this.m_ShowOwnerCrest ? this.Pact.leader : this.enemy, null);
	}

	// Token: 0x060025F4 RID: 9716 RVA: 0x0014EFA5 File Offset: 0x0014D1A5
	public void EnableTooltip(bool e, bool force = true)
	{
		if (this.m_EnableTooltip == e && !force)
		{
			return;
		}
		this.m_EnableTooltip = e;
		this.RefreshTooltip();
	}

	// Token: 0x060025F5 RID: 9717 RVA: 0x0014EFC4 File Offset: 0x0014D1C4
	private void RefreshTooltip()
	{
		if (this.m_EnableTooltip)
		{
			this.tooltip = Tooltip.Get(base.gameObject, true);
			this.tooltip.SetDef("PactTooltip", this.tooltipVars);
			return;
		}
		if (this.tooltip != null)
		{
			UnityEngine.Object.Destroy(this.tooltip);
		}
	}

	// Token: 0x060025F6 RID: 9718 RVA: 0x0014F01B File Offset: 0x0014D21B
	private void ExtractSides()
	{
		this.player = null;
		this.enemy = null;
		if (this.Pact == null)
		{
			return;
		}
		this.player = BaseUI.LogicKingdom();
		this.enemy = this.Pact.target;
	}

	// Token: 0x060025F7 RID: 9719 RVA: 0x0014F050 File Offset: 0x0014D250
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
	}

	// Token: 0x060025F8 RID: 9720 RVA: 0x0014F0BC File Offset: 0x0014D2BC
	private void OnDestroy()
	{
		AspectRatioFitter component = base.GetComponent<AspectRatioFitter>();
		if (component != null)
		{
			UnityEngine.Object.DestroyImmediate(component);
		}
		this.OnSelect = null;
		this.OnFocus = null;
		Pact pact = this.Pact;
		if (pact != null)
		{
			pact.DelListener(this);
		}
		this.Pact = null;
		this.logicObject = null;
		this.m_ShowOwnerCrest = false;
	}

	// Token: 0x060025F9 RID: 9721 RVA: 0x0014F114 File Offset: 0x0014D314
	private static Logic.Character FindSpy(Logic.Kingdom plr_kingdom)
	{
		return plr_kingdom.court.Find((Logic.Character c) => c != null && c.IsSpy() && !c.IsPrisoner());
	}

	// Token: 0x060025FA RID: 9722 RVA: 0x0014F140 File Offset: 0x0014D340
	public static void OnPactMessage(Logic.Kingdom k, string message, Vars v)
	{
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			return;
		}
		Pact pact = v.obj.obj_val as Pact;
		if (!pact.IsVisibleBy(kingdom, null))
		{
			return;
		}
		string text;
		if (message == "add_pact")
		{
			if (k == pact.leader)
			{
				text = "Created";
			}
			else
			{
				text = "Joined";
			}
		}
		else if (message == "del_pact")
		{
			if (!pact.IsValid())
			{
				return;
			}
			if (k == pact.leader)
			{
				text = "Dissolved";
			}
			else
			{
				text = "Left";
			}
			string text2 = v.Get("reason", true).String(null);
			if (!string.IsNullOrEmpty(text2))
			{
				text += text2;
			}
		}
		else if (message == "reveal_pact")
		{
			text = "Revealed";
		}
		else
		{
			if (!(message == "conceal_pact"))
			{
				return;
			}
			text = "Concealed";
		}
		string arg;
		if (kingdom == pact.leader)
		{
			arg = "Leader";
		}
		else if (kingdom == pact.target)
		{
			arg = "Target";
		}
		else if (kingdom == k)
		{
			arg = "TheKingdom";
		}
		else if (pact.members.Contains(kingdom))
		{
			arg = "Supporter";
		}
		else
		{
			arg = "NotInvolved";
		}
		string str = string.Format("{0}Pact{1}PlayerIs{2}", pact.type, text, arg);
		string text3 = null;
		Logic.Character character = UIPactIcon.FindSpy(kingdom);
		if (character != null)
		{
			text3 = str + "RumorMessage";
			if (global::Defs.GetDefField(text3, null) == null)
			{
				text3 = null;
			}
		}
		if (text3 == null)
		{
			text3 = str + "Message";
			if (global::Defs.GetDefField(text3, null) == null)
			{
				return;
			}
		}
		Vars vars = new Vars();
		vars.Set<Pact>("pact", pact);
		vars.Set<Logic.Kingdom>("kingdom", k);
		vars.Set<Logic.Character>("spy", character);
		MessageIcon.Create(text3, vars, true, null);
	}

	// Token: 0x060025FB RID: 9723 RVA: 0x00132C4A File Offset: 0x00130E4A
	public void OnMessage(object obj, string message, object param)
	{
		if (!(message == "destroying") && !(message == "finishing"))
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

	// Token: 0x060025FC RID: 9724 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolActivated()
	{
	}

	// Token: 0x060025FD RID: 9725 RVA: 0x0014F308 File Offset: 0x0014D508
	public void OnPoolDeactivated()
	{
		this.EnableTooltip(true, true);
		this.OnDestroy();
	}

	// Token: 0x060025FE RID: 9726 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolDestroyed()
	{
	}

	// Token: 0x060025FF RID: 9727 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolSpawned()
	{
	}

	// Token: 0x040019B1 RID: 6577
	[UIFieldTarget("id_Crest")]
	private UIKingdomIcon m_Crest;

	// Token: 0x040019B2 RID: 6578
	[UIFieldTarget("id_Icon")]
	private Image m_Icon;

	// Token: 0x040019B4 RID: 6580
	private Logic.Kingdom player;

	// Token: 0x040019B5 RID: 6581
	private Logic.Kingdom enemy;

	// Token: 0x040019B8 RID: 6584
	private bool m_Initialzied;

	// Token: 0x040019B9 RID: 6585
	private bool m_WasActivated;

	// Token: 0x040019BA RID: 6586
	private bool m_AddListeners;

	// Token: 0x040019BB RID: 6587
	private bool m_ShowOwnerCrest;

	// Token: 0x040019BC RID: 6588
	private bool m_EnableTooltip = true;

	// Token: 0x040019BD RID: 6589
	private Tooltip tooltip;

	// Token: 0x040019BE RID: 6590
	private Vars tooltipVars;
}
