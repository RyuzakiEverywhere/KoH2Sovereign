using System;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020001D9 RID: 473
public class UICapturePoint : MonoBehaviour, IListener
{
	// Token: 0x1400001A RID: 26
	// (add) Token: 0x06001C2D RID: 7213 RVA: 0x0010AAE4 File Offset: 0x00108CE4
	// (remove) Token: 0x06001C2E RID: 7214 RVA: 0x0010AB1C File Offset: 0x00108D1C
	public event Action onChange;

	// Token: 0x1700017A RID: 378
	// (get) Token: 0x06001C2F RID: 7215 RVA: 0x0010AB51 File Offset: 0x00108D51
	// (set) Token: 0x06001C30 RID: 7216 RVA: 0x0010AB59 File Offset: 0x00108D59
	public Logic.CapturePoint Logic { get; private set; }

	// Token: 0x06001C31 RID: 7217 RVA: 0x0010AB64 File Offset: 0x00108D64
	public void SetData(Logic.CapturePoint data)
	{
		this.Init();
		this.btn.onEvent = new BSGButton.OnEvent(this.OnEvent);
		this.Logic = data;
		this.Logic.AddListener(this);
		this.RefreshControllerKingdom();
		DT.Def def = this.Logic.def.field.def;
		this.m_tooltip = Tooltip.Get(this.btn.gameObject, true);
		this.vars.Set<Logic.CapturePoint>("obj", this.Logic);
		this.m_tooltip.SetText(def.field.key + ".tooltip", def.field.key + ".name", this.vars);
	}

	// Token: 0x06001C32 RID: 7218 RVA: 0x0010AC28 File Offset: 0x00108E28
	private void OnEvent(BSGButton btn, BSGButton.Event e, PointerEventData eventData)
	{
		if (e <= BSGButton.Event.Leave)
		{
			this.UpdateHighlight(e);
			return;
		}
		if (e != BSGButton.Event.Up)
		{
			return;
		}
		if (eventData.clickCount == 2)
		{
			if (this.Logic == null || this.Logic.visuals == null)
			{
				return;
			}
			BaseUI.Get<BattleViewUI>().LookAt((this.Logic.visuals as global::CapturePoint).gameObject.transform.position, false);
		}
	}

	// Token: 0x06001C33 RID: 7219 RVA: 0x0010AC8F File Offset: 0x00108E8F
	private void OnDestroy()
	{
		if (this.Logic == null)
		{
			return;
		}
		this.Logic.DelListener(this);
	}

	// Token: 0x06001C34 RID: 7220 RVA: 0x0010ACA6 File Offset: 0x00108EA6
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		this.m_definition = global::Defs.GetDefField("CapturePointSlot", null);
		UICommon.FindComponents(this, false);
		this.m_Initialzied = true;
		this.vars = new Vars();
	}

	// Token: 0x06001C35 RID: 7221 RVA: 0x0010ACDC File Offset: 0x00108EDC
	private void Update()
	{
		if (this.Logic == null)
		{
			return;
		}
		global::CapturePoint capturePoint = this.Logic.visuals as global::CapturePoint;
		if (capturePoint == null)
		{
			return;
		}
		if (capturePoint.Highlighted != this.was_highlighted)
		{
			this.was_highlighted = capturePoint.Highlighted;
			if (this.was_highlighted)
			{
				this.btn_img.SetState(BSGButton.State.Rollover);
			}
			else
			{
				this.btn_img.SetState(BSGButton.State.Normal);
			}
		}
		if (this.Logic.is_capturing)
		{
			this.m_Filling.fillAmount = this.Logic.capture_progress.Get();
			return;
		}
		this.m_Filling.fillAmount = 1f;
	}

	// Token: 0x06001C36 RID: 7222 RVA: 0x0010AD84 File Offset: 0x00108F84
	private void RefreshControllerKingdom()
	{
		DT.Field field = (global::Battle.PlayerBattleSide() == this.Logic.battle_side) ? this.m_definition.FindChild("Our", null, true, true, true, '.') : this.m_definition.FindChild("Enemy", null, true, true, true, '.');
		this.m_Filling.sprite = global::Defs.GetObj<Sprite>(field, "background", null);
		this.m_Star.sprite = global::Defs.GetObj<Sprite>(field, "icon", null);
		Action action = this.onChange;
		if (action == null)
		{
			return;
		}
		action();
	}

	// Token: 0x06001C37 RID: 7223 RVA: 0x0010AE10 File Offset: 0x00109010
	void IListener.OnMessage(object obj, string message, object param)
	{
		if (message == "battle_side_changed")
		{
			this.RefreshControllerKingdom();
			return;
		}
	}

	// Token: 0x06001C38 RID: 7224 RVA: 0x0010AE28 File Offset: 0x00109028
	public void UpdateHighlight(BSGButton.Event e)
	{
		if (!Application.isPlaying)
		{
			return;
		}
		global::CapturePoint capturePoint = this.Logic.visuals as global::CapturePoint;
		capturePoint.MouseOvered = false;
		if (e == BSGButton.Event.Enter)
		{
			capturePoint.MouseOvered = true;
		}
		capturePoint.UpdateSelection();
	}

	// Token: 0x0400125E RID: 4702
	[UIFieldTarget("id_Filling")]
	private Image m_Filling;

	// Token: 0x0400125F RID: 4703
	[UIFieldTarget("id_Star")]
	private Image m_Star;

	// Token: 0x04001260 RID: 4704
	private Vars vars;

	// Token: 0x04001261 RID: 4705
	[UIFieldTarget("id_Background")]
	private BSGButton btn;

	// Token: 0x04001262 RID: 4706
	[UIFieldTarget("id_Background")]
	private BSGButtonImage btn_img;

	// Token: 0x04001263 RID: 4707
	private bool was_highlighted;

	// Token: 0x04001264 RID: 4708
	private DT.Field m_definition;

	// Token: 0x04001265 RID: 4709
	private Tooltip m_tooltip;

	// Token: 0x04001266 RID: 4710
	private bool m_Initialzied;
}
