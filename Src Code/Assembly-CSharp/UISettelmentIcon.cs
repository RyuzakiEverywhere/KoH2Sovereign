using System;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020002BC RID: 700
public class UISettelmentIcon : ObjectIcon, IListener
{
	// Token: 0x1700022D RID: 557
	// (get) Token: 0x06002BEB RID: 11243 RVA: 0x001711A1 File Offset: 0x0016F3A1
	// (set) Token: 0x06002BEC RID: 11244 RVA: 0x001711A9 File Offset: 0x0016F3A9
	public Logic.Settlement Inst { get; private set; }

	// Token: 0x1700022E RID: 558
	// (get) Token: 0x06002BED RID: 11245 RVA: 0x001711B2 File Offset: 0x0016F3B2
	// (set) Token: 0x06002BEE RID: 11246 RVA: 0x001711BA File Offset: 0x0016F3BA
	public Logic.Settlement.Def Def { get; private set; }

	// Token: 0x1400003B RID: 59
	// (add) Token: 0x06002BEF RID: 11247 RVA: 0x001711C4 File Offset: 0x0016F3C4
	// (remove) Token: 0x06002BF0 RID: 11248 RVA: 0x001711FC File Offset: 0x0016F3FC
	public event Action<UISettelmentIcon> OnSelect;

	// Token: 0x06002BF1 RID: 11249 RVA: 0x00171231 File Offset: 0x0016F431
	public override void Awake()
	{
		base.Awake();
		this.SetObject(null, null);
	}

	// Token: 0x06002BF2 RID: 11250 RVA: 0x00171241 File Offset: 0x0016F441
	private void OnDestroy()
	{
		if (this.Inst != null)
		{
			this.Inst.DelListener(this);
		}
		this.OnSelect = null;
	}

	// Token: 0x06002BF3 RID: 11251 RVA: 0x00171260 File Offset: 0x0016F460
	public override void SetObject(object obj, Vars vars = null)
	{
		base.SetObject(obj, vars);
		UICommon.FindComponents(this, false);
		if (this.logicObject != null)
		{
			if (obj is Logic.Settlement)
			{
				this.Inst = (obj as Logic.Settlement);
				this.Def = this.Inst.def;
				this.Inst.AddListener(this);
				Tooltip.Get(base.gameObject, true).SetObj(obj, null, null);
			}
			else if (obj is Logic.Settlement.Def)
			{
				this.Def = (obj as Logic.Settlement.Def);
			}
			else if (obj is DT.Field)
			{
				this.Def = GameLogic.Get(true).defs.Find<Logic.Settlement.Def>((obj as DT.Field).key);
			}
		}
		else
		{
			this.Inst = null;
			this.Def = null;
		}
		if (this.Inst != null)
		{
			Tooltip.Get(base.gameObject, true).SetObj(this.Inst, null, null);
		}
		else if (this.Def != null)
		{
			Tooltip.Get(base.gameObject, true).SetDef("SettlementTooltip", new Vars(this.Def));
		}
		this.Refresh();
	}

	// Token: 0x06002BF4 RID: 11252 RVA: 0x00171374 File Offset: 0x0016F574
	public static Value SetupHTTooltip(UIHyperText.CallbackParams arg)
	{
		UIHyperText ht = arg.ht;
		IVars vars = (ht != null) ? ht.vars : null;
		if (vars == null)
		{
			return Value.Unknown;
		}
		Logic.Settlement.Def def = vars.GetVar("def", null, true).Get<Logic.Settlement.Def>();
		if (def == null)
		{
			def = vars.GetVar("obj", null, true).Get<Logic.Settlement.Def>();
			if (def == null)
			{
				return Value.Unknown;
			}
		}
		Vars vars2 = vars as Vars;
		if (vars2 == null)
		{
			vars2 = new Vars(vars);
			arg.ht.vars = vars2;
		}
		Logic.Kingdom k = (UIText.cur_article != null) ? null : BaseUI.LogicKingdom();
		UIResources.FillAvailability(vars2, "Settlement.availability_texts", def.id, k);
		return Value.Unknown;
	}

	// Token: 0x06002BF5 RID: 11253 RVA: 0x0017141C File Offset: 0x0016F61C
	private void Refresh()
	{
		if (this.Def != null && this.m_Icon != null)
		{
			this.m_Icon.overrideSprite = global::Defs.GetObj<Sprite>(this.Def.field, "icon", null);
		}
		bool active = false;
		bool active2 = this.Inst != null;
		if (this.Inst != null)
		{
			if (this.m_LevelValue != null)
			{
				UIText.SetText(this.m_LevelValue, this.Inst.level.ToString());
			}
			if (this.Inst is Village)
			{
				Village village = this.Inst as Village;
				active = (village.settlementUpgrade != null && village.settlementUpgrade.upgrading);
			}
		}
		if (this.m_UpgradeIcon != null)
		{
			this.m_UpgradeIcon.SetActive(active);
		}
		if (this.m_Level != null)
		{
			this.m_Level.gameObject.SetActive(active2);
		}
	}

	// Token: 0x06002BF6 RID: 11254 RVA: 0x00171509 File Offset: 0x0016F709
	public void Select(bool selected)
	{
		if (this.m_Selected == selected)
		{
			return;
		}
		this.m_Selected = selected;
		this.UpdateHighlight();
	}

	// Token: 0x06002BF7 RID: 11255 RVA: 0x00171522 File Offset: 0x0016F722
	public override void OnClick(PointerEventData e)
	{
		base.OnClick(e);
		if (this.OnSelect != null)
		{
			this.OnSelect(this);
		}
	}

	// Token: 0x06002BF8 RID: 11256 RVA: 0x0017153F File Offset: 0x0016F73F
	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x06002BF9 RID: 11257 RVA: 0x0017154E File Offset: 0x0016F74E
	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x06002BFA RID: 11258 RVA: 0x00171560 File Offset: 0x0016F760
	public void UpdateHighlight()
	{
		if (this.m_Border != null)
		{
			Color color;
			if (this.m_Selected)
			{
				if (this.mouse_in)
				{
					color = this.selected;
				}
				else
				{
					color = this.selected;
				}
			}
			else if (this.mouse_in)
			{
				color = this.over;
			}
			else
			{
				color = this.normal;
			}
			this.m_Border.color = color;
			this.m_Background.color = (this.m_Selected ? this.selected : Color.white);
		}
	}

	// Token: 0x06002BFB RID: 11259 RVA: 0x001715E1 File Offset: 0x0016F7E1
	void IListener.OnMessage(object obj, string message, object param)
	{
		this.Refresh();
	}

	// Token: 0x04001DE3 RID: 7651
	[UIFieldTarget("id_SettlementIcon")]
	private Image m_Icon;

	// Token: 0x04001DE4 RID: 7652
	[UIFieldTarget("id_ProducationIcon")]
	private Image m_ProducationIcon;

	// Token: 0x04001DE5 RID: 7653
	[UIFieldTarget("id_Border")]
	private Image m_Border;

	// Token: 0x04001DE6 RID: 7654
	[UIFieldTarget("id_Background")]
	private Image m_Background;

	// Token: 0x04001DE7 RID: 7655
	[UIFieldTarget("id_Level")]
	private GameObject m_Level;

	// Token: 0x04001DE8 RID: 7656
	[UIFieldTarget("id_Level_Value")]
	private TextMeshProUGUI m_LevelValue;

	// Token: 0x04001DE9 RID: 7657
	[UIFieldTarget("id_Upgrade")]
	private GameObject m_UpgradeIcon;

	// Token: 0x04001DEA RID: 7658
	[SerializeField]
	private Color normal = new Color32(92, 92, 92, byte.MaxValue);

	// Token: 0x04001DEB RID: 7659
	[SerializeField]
	private Color over = new Color32(92, 92, 92, byte.MaxValue);

	// Token: 0x04001DEC RID: 7660
	[SerializeField]
	private Color selected = new Color32(92, 92, 92, byte.MaxValue);

	// Token: 0x04001DF0 RID: 7664
	private bool m_Selected;
}
