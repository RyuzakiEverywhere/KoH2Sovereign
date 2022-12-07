using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020002DB RID: 731
public class TooltipInstance : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IVars
{
	// Token: 0x06002E44 RID: 11844 RVA: 0x0017E85C File Offset: 0x0017CA5C
	private void OnEnable()
	{
		if (this.ht == null)
		{
			this.ht = global::Common.GetComponent<UIHyperText>(base.transform, "id_HyperText");
		}
		if (this.id_Pin == null)
		{
			this.id_Pin = global::Common.FindChildByName(base.gameObject, "TooltipPin", true, true);
		}
		if (this.id_Pin == null)
		{
			return;
		}
		if (this.pinned)
		{
			this.Pin();
			return;
		}
		this.id_Pin.gameObject.SetActive(true);
		if (this.id_RightClickIcon == null)
		{
			this.id_RightClickIcon = global::Common.FindChildByName(this.id_Pin, "id_RightClickIcon", true, true);
		}
		if (this.id_RightClickIcon != null)
		{
			this.id_RightClickIcon.SetActive(false);
		}
		if (this.id_PinBackground == null)
		{
			this.id_PinBackground = global::Common.FindChildByName(this.id_Pin, "id_PinBackground", true, true);
		}
		if (this.id_PinBackground != null)
		{
			this.id_PinBackground.SetActive(false);
		}
		if (this.id_Border == null)
		{
			this.id_Border = global::Common.FindChildByName(this.id_Pin, "id_Border", true, true);
		}
		if (this.id_Border != null)
		{
			this.id_Border.SetActive(true);
		}
		if (this.id_Close == null)
		{
			this.id_Close = global::Common.FindChildComponent<BSGButton>(this.id_Pin, "id_Close");
		}
		if (this.id_Close != null)
		{
			this.id_Close.onClick = new BSGButton.OnClick(this.Close);
			this.id_Close.gameObject.SetActive(false);
		}
	}

	// Token: 0x06002E45 RID: 11845 RVA: 0x0017E9FC File Offset: 0x0017CBFC
	private void OnDisable()
	{
		if (this.pinned)
		{
			this.Unpin();
			this.pinned = true;
		}
	}

	// Token: 0x06002E46 RID: 11846 RVA: 0x0017EA14 File Offset: 0x0017CC14
	public void Pin()
	{
		this.pinned = true;
		TooltipPlacement.AddBlocker(base.gameObject, null);
		if (this.id_Pin == null)
		{
			return;
		}
		this.id_Pin.transform.SetAsLastSibling();
		this.id_Pin.SetActive(true);
		if (this.id_RightClickIcon != null)
		{
			this.id_RightClickIcon.SetActive(false);
		}
		if (this.id_Border != null)
		{
			this.id_Border.SetActive(true);
		}
		if (this.id_Close != null)
		{
			this.id_Close.gameObject.SetActive(true);
		}
		if (this.id_PinBackground != null)
		{
			this.id_PinBackground.gameObject.SetActive(true);
		}
	}

	// Token: 0x06002E47 RID: 11847 RVA: 0x0017EAD4 File Offset: 0x0017CCD4
	public void Unpin()
	{
		this.pinned = false;
		TooltipPlacement.DelBlocker(base.gameObject);
		if (this.id_Pin == null)
		{
			return;
		}
		this.id_Pin.SetActive(true);
		if (this.id_RightClickIcon != null)
		{
			this.id_RightClickIcon.SetActive(false);
		}
		if (this.id_Border != null)
		{
			this.id_Border.SetActive(true);
		}
		if (this.id_Close != null)
		{
			this.id_Close.gameObject.SetActive(false);
		}
		if (this.id_PinBackground != null)
		{
			this.id_PinBackground.gameObject.SetActive(false);
		}
	}

	// Token: 0x06002E48 RID: 11848 RVA: 0x0017EB80 File Offset: 0x0017CD80
	public void Close(BSGButton btn)
	{
		BaseUI baseUI = BaseUI.Get();
		if (baseUI == null)
		{
			return;
		}
		baseUI.DestroyTooltipInstance(base.gameObject);
	}

	// Token: 0x06002E49 RID: 11849 RVA: 0x0017EB98 File Offset: 0x0017CD98
	private bool IsPinable(Tooltip tt)
	{
		if (tt.pinable == 0)
		{
			return false;
		}
		if (tt.pinable > 0)
		{
			return true;
		}
		UIText[] componentsInChildren = base.GetComponentsInChildren<UIText>();
		if (componentsInChildren == null || componentsInChildren.Length == 0)
		{
			return false;
		}
		foreach (UIText uitext in componentsInChildren)
		{
			if (uitext.has_numbers)
			{
				return true;
			}
			if (uitext.has_links)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002E4A RID: 11850 RVA: 0x0017EBF4 File Offset: 0x0017CDF4
	public bool Init(Tooltip src, IVars vars)
	{
		this.src = src;
		this.vars = vars;
		DT.Field field;
		if (src == null)
		{
			field = null;
		}
		else
		{
			DT.Def def = src.def;
			if (def == null)
			{
				field = null;
			}
			else
			{
				DT.Field field2 = def.field;
				field = ((field2 != null) ? field2.FindChild("visible_condition", null, true, true, true, '.') : null);
			}
		}
		this.validate = field;
		if (src == null || src.instance != base.gameObject)
		{
			return false;
		}
		this.update_timers = true;
		return true;
	}

	// Token: 0x06002E4B RID: 11851 RVA: 0x0017EC6C File Offset: 0x0017CE6C
	private void UpdateTimers()
	{
		this.update_timers = false;
		if (this.src == null)
		{
			return;
		}
		if (this.IsPinable(this.src))
		{
			this.pinable_after = UnityEngine.Time.unscaledTime + this.src.pinable_delay;
		}
		if (this.src.is_tutorial_tooltip && this.src.tutorial_hotspot_def != null && !this.src.tutorial_hotspot_def.seen)
		{
			this.seen_after = UnityEngine.Time.unscaledTime + this.src.tutorial_hotspot_def.seen_delay;
		}
	}

	// Token: 0x06002E4C RID: 11852 RVA: 0x0017ECFC File Offset: 0x0017CEFC
	public void Done()
	{
		if (this.pinned)
		{
			this.Unpin();
		}
		if (this.src != null)
		{
			this.src.instance = null;
		}
		this.src = null;
		this.pinable = false;
		this.pinable_after = float.PositiveInfinity;
		this.seen_after = float.PositiveInfinity;
		this.update_timers = false;
	}

	// Token: 0x06002E4D RID: 11853 RVA: 0x0017ED5C File Offset: 0x0017CF5C
	private void Update()
	{
		BaseUI baseUI = BaseUI.Get();
		if (baseUI == null)
		{
			return;
		}
		if (!this.pinned)
		{
			Tooltip tooltip = baseUI.tooltip;
			if (((tooltip != null) ? tooltip.instance : null) != base.gameObject)
			{
				this.Close(null);
				return;
			}
		}
		if (this.src == null && !this.Init(baseUI.tooltip, this.vars))
		{
			return;
		}
		if (this.update_timers)
		{
			this.UpdateTimers();
		}
		if (this.validate != null && !this.validate.Bool(this.vars, false))
		{
			this.Close(null);
			return;
		}
		if (this.src != null && this.ht == null)
		{
			this.src.CallHandler(baseUI, Tooltip.Event.Update);
			if (this.src.RecalcTexts())
			{
				baseUI.FillTooltip(this.src);
			}
		}
		if (this.src != null && this.src.is_tutorial_tooltip && this.src.tutorial_hotspot_def != null && !this.src.tutorial_hotspot_def.seen && UnityEngine.Time.unscaledTime >= this.seen_after)
		{
			this.src.tutorial_hotspot_def.Seen();
		}
		if (!this.pinable && UnityEngine.Time.unscaledTime >= this.pinable_after)
		{
			this.pinable = true;
			if (this.id_Pin != null)
			{
				this.id_Pin.SetActive(true);
			}
			if (this.id_RightClickIcon != null)
			{
				this.id_RightClickIcon.SetActive(true);
			}
			if (this.id_PinBackground != null)
			{
				this.id_PinBackground.SetActive(true);
			}
		}
	}

	// Token: 0x06002E4E RID: 11854 RVA: 0x0017EEFB File Offset: 0x0017D0FB
	public Value GetVar(string key, IVars _vars = null, bool as_value = true)
	{
		if (this.vars == null)
		{
			return Value.Unknown;
		}
		return this.vars.GetVar(key, _vars, as_value);
	}

	// Token: 0x06002E4F RID: 11855 RVA: 0x0017EF1C File Offset: 0x0017D11C
	public void OnPointerDown(PointerEventData eventData)
	{
		if (eventData.button != PointerEventData.InputButton.Right || BaseUI.IgnoreRightClick())
		{
			return;
		}
		BaseUI baseUI = BaseUI.Get();
		Tooltip tooltip = (baseUI != null) ? baseUI.tooltip : null;
		if (tooltip != null && tooltip.instance != null && global::Common.IsParent(base.gameObject, tooltip.gameObject))
		{
			return;
		}
		this.Close(null);
	}

	// Token: 0x06002E50 RID: 11856 RVA: 0x0017EF80 File Offset: 0x0017D180
	public static void RemovePinnedTooltips()
	{
		BaseUI baseUI = BaseUI.Get();
		GameObject gameObject;
		if (baseUI == null)
		{
			gameObject = null;
		}
		else
		{
			Canvas canvas = baseUI.canvas;
			gameObject = ((canvas != null) ? canvas.gameObject : null);
		}
		GameObject gameObject2 = gameObject;
		if (gameObject2 == null)
		{
			return;
		}
		List<TooltipInstance> list = new List<TooltipInstance>();
		global::Common.FindChildrenWithComponent<TooltipInstance>(gameObject2, list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].Close(null);
		}
	}

	// Token: 0x04001F4D RID: 8013
	public IVars vars;

	// Token: 0x04001F4E RID: 8014
	public DT.Field validate;

	// Token: 0x04001F4F RID: 8015
	public bool pinable;

	// Token: 0x04001F50 RID: 8016
	public bool pinned;

	// Token: 0x04001F51 RID: 8017
	public Tooltip src;

	// Token: 0x04001F52 RID: 8018
	private bool update_timers;

	// Token: 0x04001F53 RID: 8019
	private float pinable_after = float.PositiveInfinity;

	// Token: 0x04001F54 RID: 8020
	private float seen_after = float.PositiveInfinity;

	// Token: 0x04001F55 RID: 8021
	private GameObject id_Pin;

	// Token: 0x04001F56 RID: 8022
	private GameObject id_RightClickIcon;

	// Token: 0x04001F57 RID: 8023
	private GameObject id_Border;

	// Token: 0x04001F58 RID: 8024
	private GameObject id_PinBackground;

	// Token: 0x04001F59 RID: 8025
	private BSGButton id_Close;

	// Token: 0x04001F5A RID: 8026
	private UIHyperText ht;
}
