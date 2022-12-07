using System;
using FMODUnity;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020001AA RID: 426
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasRenderer))]
public class BSGButton : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
	// Token: 0x0600189C RID: 6300 RVA: 0x000F20FA File Offset: 0x000F02FA
	public bool IsEnabled()
	{
		return !this.disabled;
	}

	// Token: 0x0600189D RID: 6301 RVA: 0x000F2105 File Offset: 0x000F0305
	public void AllowSelection(bool selectable)
	{
		this.isSelectable = selectable;
	}

	// Token: 0x0600189E RID: 6302 RVA: 0x000F210E File Offset: 0x000F030E
	public void SetSelected(bool selected, bool force_update = false)
	{
		if (this.isSelectable)
		{
			this.isSelected = selected;
			this.UpdateState(force_update);
		}
	}

	// Token: 0x0600189F RID: 6303 RVA: 0x000F2126 File Offset: 0x000F0326
	public void Enable(bool enable, bool force = false)
	{
		if (!this.disabled == enable && !force)
		{
			return;
		}
		this.disabled = !enable;
		this.UpdateState(true);
	}

	// Token: 0x060018A0 RID: 6304 RVA: 0x000F2149 File Offset: 0x000F0349
	public BSGButton.State GetState()
	{
		return this.state;
	}

	// Token: 0x060018A1 RID: 6305 RVA: 0x000F2151 File Offset: 0x000F0351
	public bool MouseIn()
	{
		return this.mouse_in;
	}

	// Token: 0x060018A2 RID: 6306 RVA: 0x000F2159 File Offset: 0x000F0359
	public bool MouseDown()
	{
		return this.mouse_down;
	}

	// Token: 0x060018A3 RID: 6307 RVA: 0x000F2161 File Offset: 0x000F0361
	public bool IsSelectable()
	{
		return this.isSelectable;
	}

	// Token: 0x060018A4 RID: 6308 RVA: 0x000F2169 File Offset: 0x000F0369
	public bool IsSelected()
	{
		return this.IsSelectable() && this.isSelected;
	}

	// Token: 0x060018A5 RID: 6309 RVA: 0x000448AF File Offset: 0x00042AAF
	public virtual DT.Field GetCursorFieldKey(DT.Field field)
	{
		return null;
	}

	// Token: 0x060018A6 RID: 6310 RVA: 0x000F217B File Offset: 0x000F037B
	public void OnPointerDown(PointerEventData eventData)
	{
		if (this.onEvent != null)
		{
			this.onEvent(this, BSGButton.Event.Down, eventData);
		}
		if (eventData.button != PointerEventData.InputButton.Left)
		{
			return;
		}
		this.mouse_down = true;
		this.invalidateState = true;
	}

	// Token: 0x060018A7 RID: 6311 RVA: 0x000F21AC File Offset: 0x000F03AC
	public void OnPointerUp(PointerEventData eventData)
	{
		if (this.onEvent != null)
		{
			this.onEvent(this, BSGButton.Event.Up, eventData);
		}
		if (eventData.button != PointerEventData.InputButton.Left)
		{
			return;
		}
		this.mouse_down = false;
		if (this.isSelectable)
		{
			this.isSelected = !this.isSelected;
		}
		this.invalidateState = true;
		if (this.mouse_in && !this.drag_enter)
		{
			this.Clicked();
		}
		if (eventData.clickCount > 1)
		{
			this.DoubleClicked();
		}
		this.drag_enter = false;
	}

	// Token: 0x060018A8 RID: 6312 RVA: 0x000F222C File Offset: 0x000F042C
	public void OnPointerEnter(PointerEventData eventData)
	{
		BSGButton.SetPicked(this);
		if (this.onEvent != null)
		{
			this.onEvent(this, BSGButton.Event.Enter, eventData);
		}
		this.drag_enter = (!this.mouse_down && Input.GetMouseButton(0));
		this.mouse_in = true;
		this.invalidateState = true;
	}

	// Token: 0x060018A9 RID: 6313 RVA: 0x000F227A File Offset: 0x000F047A
	public void OnPointerExit(PointerEventData eventData)
	{
		BSGButton.SetPicked(null);
		if (this.onEvent != null)
		{
			this.onEvent(this, BSGButton.Event.Leave, eventData);
		}
		this.mouse_in = false;
		this.invalidateState = true;
	}

	// Token: 0x060018AA RID: 6314 RVA: 0x000F22A6 File Offset: 0x000F04A6
	private static void SetPicked(BSGButton btn)
	{
		BSGButton.picked = btn;
	}

	// Token: 0x060018AB RID: 6315 RVA: 0x000F22B0 File Offset: 0x000F04B0
	private void OffsetChild(Transform t, float x, float y)
	{
		Vector3 position = t.position;
		position.x += x;
		position.y -= y;
		t.position = position;
	}

	// Token: 0x060018AC RID: 6316 RVA: 0x000F22E3 File Offset: 0x000F04E3
	private void PlayRolloverSound()
	{
		if (string.IsNullOrEmpty(this.rolloverSound))
		{
			return;
		}
		FMODWrapper.PlayOneShot(this.rolloverSound, base.transform.position);
	}

	// Token: 0x060018AD RID: 6317 RVA: 0x000F2309 File Offset: 0x000F0509
	private void PlayClickSound()
	{
		if (string.IsNullOrEmpty(this.clickSound))
		{
			return;
		}
		BaseUI.PlaySoundEvent(this.clickSound, null);
	}

	// Token: 0x060018AE RID: 6318 RVA: 0x000F2325 File Offset: 0x000F0525
	private void DoubleClicked()
	{
		this.PlayClickSound();
		BSGButton.OnClick onClick = this.onDoubleClick;
		if (onClick == null)
		{
			return;
		}
		onClick(this);
	}

	// Token: 0x060018AF RID: 6319 RVA: 0x000F233E File Offset: 0x000F053E
	public void Clicked()
	{
		this.PlayClickSound();
		if (this.onClick != null)
		{
			this.onClick(this);
			return;
		}
		Debug.Log(base.name + ": unhandled click", base.gameObject);
	}

	// Token: 0x060018B0 RID: 6320 RVA: 0x000F2378 File Offset: 0x000F0578
	private BSGButton.State CalcState()
	{
		if (this.disabled)
		{
			return BSGButton.State.Disabled;
		}
		if (!this.mouse_in)
		{
			if (this.mouse_down)
			{
				return BSGButton.State.Rollover;
			}
			if (!this.isSelected)
			{
				return BSGButton.State.Normal;
			}
			return BSGButton.State.Selected;
		}
		else
		{
			if (this.drag_enter)
			{
				return BSGButton.State.Normal;
			}
			if (this.isSelectable && this.isSelected)
			{
				return BSGButton.State.Selected;
			}
			if (!this.mouse_down)
			{
				return BSGButton.State.Rollover;
			}
			return BSGButton.State.Pressed;
		}
	}

	// Token: 0x060018B1 RID: 6321 RVA: 0x000F23D4 File Offset: 0x000F05D4
	public void UpdateState(bool force_update = false)
	{
		BSGButton.State state = this.CalcState();
		if (state == this.state && !force_update)
		{
			return;
		}
		base.enabled = (state != BSGButton.State.Disabled);
		if (state == BSGButton.State.Rollover)
		{
			this.PlayRolloverSound();
		}
		if (this.normalState != null)
		{
			this.normalState.gameObject.SetActive(state == BSGButton.State.Normal);
		}
		if (this.rolloverState != null)
		{
			this.rolloverState.gameObject.SetActive(state == BSGButton.State.Rollover);
		}
		if (this.pressedState != null)
		{
			this.pressedState.gameObject.SetActive(state == BSGButton.State.Pressed);
		}
		if (this.disabledState != null)
		{
			this.disabledState.gameObject.SetActive(state == BSGButton.State.Disabled);
		}
		if (this.selectedState != null)
		{
			this.selectedState.gameObject.SetActive(state == BSGButton.State.Selected);
		}
		BSGButtonImage component = base.GetComponent<BSGButtonImage>();
		if (component != null)
		{
			component.SetState(state);
		}
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			component = child.GetComponent<BSGButtonImage>();
			if (component != null)
			{
				component.SetState(state);
			}
			else if (state != this.state)
			{
				if (state == BSGButton.State.Pressed)
				{
					this.OffsetChild(child, this.childOfsX, this.childOfsY);
				}
				else if (this.state == BSGButton.State.Pressed)
				{
					this.OffsetChild(child, -this.childOfsX, -this.childOfsY);
				}
			}
		}
		this.state = state;
	}

	// Token: 0x060018B2 RID: 6322 RVA: 0x000F2550 File Offset: 0x000F0750
	private void Awake()
	{
		Tooltip.Get(base.gameObject, true);
		if (base.transform.childCount > 0)
		{
			for (int i = 0; i < base.transform.childCount; i++)
			{
				Transform child = base.transform.GetChild(i);
				if (child.name == "Normal")
				{
					this.normalState = child;
				}
				else if (child.name == "Rollover")
				{
					this.rolloverState = child;
				}
				else if (child.name == "Pressed")
				{
					this.pressedState = child;
				}
				else if (child.name == "Disabled")
				{
					this.disabledState = child;
				}
				else if (child.name == "Selected")
				{
					this.selectedState = child;
				}
			}
		}
		this.UpdateState(true);
		this.SetAudioSet("ButtonAudioSet");
	}

	// Token: 0x060018B3 RID: 6323 RVA: 0x000F263B File Offset: 0x000F083B
	private void OnEnable()
	{
		this.invalidateOverState = true;
	}

	// Token: 0x060018B4 RID: 6324 RVA: 0x000F2644 File Offset: 0x000F0844
	private void OnDisable()
	{
		this.mouse_down = false;
		this.mouse_in = false;
		this.drag_enter = false;
		this.isSelected = false;
	}

	// Token: 0x060018B5 RID: 6325 RVA: 0x000F2662 File Offset: 0x000F0862
	private void OnDestroy()
	{
		this.onEvent = null;
		this.onClick = null;
	}

	// Token: 0x060018B6 RID: 6326 RVA: 0x000F2674 File Offset: 0x000F0874
	private void LateUpdate()
	{
		if (this.invalidateOverState)
		{
			this.mouse_in = RectTransformUtility.RectangleContainsScreenPoint(base.transform as RectTransform, Input.mousePosition);
			this.invalidateOverState = false;
		}
		if (this.invalidateState)
		{
			this.UpdateState(false);
			return;
		}
		if (this.mouse_in && this.drag_enter && Input.GetMouseButtonUp(0))
		{
			this.drag_enter = false;
			this.UpdateState(false);
		}
	}

	// Token: 0x060018B7 RID: 6327 RVA: 0x000F26E8 File Offset: 0x000F08E8
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

	// Token: 0x04000FDC RID: 4060
	[EventRef(compact = true)]
	public string rolloverSound;

	// Token: 0x04000FDD RID: 4061
	[EventRef(compact = true)]
	public string clickSound;

	// Token: 0x04000FDE RID: 4062
	public float childOfsX;

	// Token: 0x04000FDF RID: 4063
	public float childOfsY;

	// Token: 0x04000FE0 RID: 4064
	public static BSGButton picked;

	// Token: 0x04000FE1 RID: 4065
	public BSGButton.OnClick onClick;

	// Token: 0x04000FE2 RID: 4066
	public BSGButton.OnClick onDoubleClick;

	// Token: 0x04000FE3 RID: 4067
	public BSGButton.OnEvent onEvent;

	// Token: 0x04000FE4 RID: 4068
	private BSGButton.State state;

	// Token: 0x04000FE5 RID: 4069
	private bool disabled;

	// Token: 0x04000FE6 RID: 4070
	private bool PreSelected;

	// Token: 0x04000FE7 RID: 4071
	private bool isSelectable;

	// Token: 0x04000FE8 RID: 4072
	private bool mouse_down;

	// Token: 0x04000FE9 RID: 4073
	private bool mouse_in;

	// Token: 0x04000FEA RID: 4074
	private bool drag_enter;

	// Token: 0x04000FEB RID: 4075
	private bool isSelected;

	// Token: 0x04000FEC RID: 4076
	private bool invalidateState;

	// Token: 0x04000FED RID: 4077
	private bool invalidateOverState;

	// Token: 0x04000FEE RID: 4078
	private Transform normalState;

	// Token: 0x04000FEF RID: 4079
	private Transform rolloverState;

	// Token: 0x04000FF0 RID: 4080
	private Transform pressedState;

	// Token: 0x04000FF1 RID: 4081
	private Transform disabledState;

	// Token: 0x04000FF2 RID: 4082
	private Transform selectedState;

	// Token: 0x020006FD RID: 1789
	public enum State
	{
		// Token: 0x040037C2 RID: 14274
		Normal,
		// Token: 0x040037C3 RID: 14275
		Rollover,
		// Token: 0x040037C4 RID: 14276
		Pressed,
		// Token: 0x040037C5 RID: 14277
		Disabled,
		// Token: 0x040037C6 RID: 14278
		Selected
	}

	// Token: 0x020006FE RID: 1790
	public enum Event
	{
		// Token: 0x040037C8 RID: 14280
		Enter,
		// Token: 0x040037C9 RID: 14281
		Leave,
		// Token: 0x040037CA RID: 14282
		Down,
		// Token: 0x040037CB RID: 14283
		Up
	}

	// Token: 0x020006FF RID: 1791
	// (Invoke) Token: 0x06004932 RID: 18738
	public delegate void OnClick(BSGButton btn);

	// Token: 0x02000700 RID: 1792
	// (Invoke) Token: 0x06004936 RID: 18742
	public delegate void OnEvent(BSGButton btn, BSGButton.Event e, PointerEventData eventData);
}
