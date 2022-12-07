using System;
using System.Collections.Generic;
using FMODUnity;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000205 RID: 517
public class Hotspot : MonoBehaviour, IVars, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
	// Token: 0x14000029 RID: 41
	// (add) Token: 0x06001F66 RID: 8038 RVA: 0x00122D1C File Offset: 0x00120F1C
	// (remove) Token: 0x06001F67 RID: 8039 RVA: 0x00122D54 File Offset: 0x00120F54
	public event Action<Hotspot, EventTriggerType, PointerEventData> OnPointerEvent;

	// Token: 0x06001F68 RID: 8040 RVA: 0x00122D89 File Offset: 0x00120F89
	public virtual void Awake()
	{
		this.SetAudioSet("ButtonAudioSet");
	}

	// Token: 0x06001F69 RID: 8041 RVA: 0x00122D96 File Offset: 0x00120F96
	public virtual Value GetVar(string key, IVars vars = null, bool as_value = true)
	{
		return Value.Unknown;
	}

	// Token: 0x06001F6A RID: 8042 RVA: 0x00122D9D File Offset: 0x00120F9D
	public virtual void OnRightClick(PointerEventData e)
	{
		Action<Hotspot, EventTriggerType, PointerEventData> onPointerEvent = this.OnPointerEvent;
		if (onPointerEvent == null)
		{
			return;
		}
		onPointerEvent(this, EventTriggerType.PointerClick, e);
	}

	// Token: 0x06001F6B RID: 8043 RVA: 0x0002C53B File Offset: 0x0002A73B
	protected virtual bool ShouldPlayClickSound()
	{
		return true;
	}

	// Token: 0x06001F6C RID: 8044 RVA: 0x00122DB2 File Offset: 0x00120FB2
	public virtual void OnClick(PointerEventData e)
	{
		if (this.ShouldPlayClickSound())
		{
			BaseUI.PlaySoundEvent(this.clickSound, null);
		}
		Action<Hotspot, EventTriggerType, PointerEventData> onPointerEvent = this.OnPointerEvent;
		if (onPointerEvent == null)
		{
			return;
		}
		onPointerEvent(this, EventTriggerType.PointerClick, e);
	}

	// Token: 0x06001F6D RID: 8045 RVA: 0x00122D9D File Offset: 0x00120F9D
	public virtual void OnDoubleClick(PointerEventData e)
	{
		Action<Hotspot, EventTriggerType, PointerEventData> onPointerEvent = this.OnPointerEvent;
		if (onPointerEvent == null)
		{
			return;
		}
		onPointerEvent(this, EventTriggerType.PointerClick, e);
	}

	// Token: 0x06001F6E RID: 8046 RVA: 0x00122DDC File Offset: 0x00120FDC
	public virtual void OnPointerDown(PointerEventData e)
	{
		Hotspot.EndDrag(false);
		if (this.mouse_down)
		{
			return;
		}
		this.mouse_btn = e.button;
		this.mouse_down = true;
		this.mouse_dragged = false;
		Action<Hotspot, EventTriggerType, PointerEventData> onPointerEvent = this.OnPointerEvent;
		if (onPointerEvent != null)
		{
			onPointerEvent(this, EventTriggerType.PointerDown, e);
		}
		if (this.mouse_btn == PointerEventData.InputButton.Right && !BaseUI.IgnoreRightClick())
		{
			this.OnRightClick(e);
		}
	}

	// Token: 0x06001F6F RID: 8047 RVA: 0x000448AF File Offset: 0x00042AAF
	public virtual DT.Field GetCursorFieldKey(DT.Field field)
	{
		return null;
	}

	// Token: 0x06001F70 RID: 8048 RVA: 0x00122E40 File Offset: 0x00121040
	public virtual void OnPointerUp(PointerEventData e)
	{
		if (!this.mouse_down || e.button != this.mouse_btn)
		{
			return;
		}
		this.mouse_down = false;
		if (!this.mouse_dragged && this.mouse_btn == PointerEventData.InputButton.Left)
		{
			if (e.clickCount < 2)
			{
				this.OnClick(e);
			}
			else
			{
				this.OnDoubleClick(e);
			}
		}
		Action<Hotspot, EventTriggerType, PointerEventData> onPointerEvent = this.OnPointerEvent;
		if (onPointerEvent == null)
		{
			return;
		}
		onPointerEvent(this, EventTriggerType.PointerUp, e);
	}

	// Token: 0x06001F71 RID: 8049 RVA: 0x00122EA8 File Offset: 0x001210A8
	public virtual void OnBeginDrag(PointerEventData e)
	{
		if (!this.mouse_down || e.button != this.mouse_btn)
		{
			return;
		}
		this.mouse_dragged = true;
		Action<Hotspot, EventTriggerType, PointerEventData> onPointerEvent = this.OnPointerEvent;
		if (onPointerEvent != null)
		{
			onPointerEvent(this, EventTriggerType.BeginDrag, e);
		}
		if (e.button != PointerEventData.InputButton.Left)
		{
			return;
		}
		Hotspot.StartDrag(base.transform);
	}

	// Token: 0x06001F72 RID: 8050 RVA: 0x00122EFC File Offset: 0x001210FC
	private static void StartDrag(Transform t)
	{
		Vector3 mousePosition = Input.mousePosition;
		while (t != null)
		{
			Hotspot component = t.GetComponent<Hotspot>();
			if (!(component == null) && RectTransformUtility.RectangleContainsScreenPoint(component.GetComponent<RectTransform>(), mousePosition))
			{
				GameObject dragObject = component.GetDragObject();
				if (!(dragObject == null))
				{
					RectTransform component2 = dragObject.GetComponent<RectTransform>();
					if (component2 == null)
					{
						return;
					}
					if (!Hotspot.ValidateStartDrag(component))
					{
						return;
					}
					Transform transform = BaseUI.Get().canvas.transform;
					GameObject obj = global::Defs.GetObj<GameObject>("DragAndDrop", "dragged_object", null);
					GameObject gameObject;
					RectTransform component3;
					RectTransform parent;
					if (obj != null)
					{
						gameObject = global::Common.Spawn(obj, transform, false, "");
						component3 = gameObject.GetComponent<RectTransform>();
						parent = (global::Common.FindChildComponent<RectTransform>(gameObject, "id_Content") ?? component3);
					}
					else
					{
						gameObject = new GameObject("DraggedObject", new Type[]
						{
							typeof(RectTransform),
							typeof(Hotspot)
						});
						component3 = gameObject.GetComponent<RectTransform>();
						component3.pivot = Vector2.one * 0.5f;
						component3.anchorMin = (component3.anchorMax = Vector2.zero);
						component3.SetParent(transform, false);
						parent = component3;
					}
					component3.sizeDelta = component2.rect.size;
					component3.position = Input.mousePosition;
					GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(dragObject, parent);
					RectTransform component4 = gameObject2.GetComponent<RectTransform>();
					Hotspot.FixFuckingUnityNotCloningImagePropertiesProperly(component2, component4);
					UICommon.FillParent(component4);
					component.PostProcessDragObject(gameObject2);
					Hotspot.dragged_obj = gameObject;
					Hotspot.drag_source = component;
					component.UpdateDragHighlight(null, component, Hotspot.dragged_obj);
				}
			}
			t = t.parent;
		}
	}

	// Token: 0x06001F73 RID: 8051 RVA: 0x001230B4 File Offset: 0x001212B4
	private static bool ValidateStartDrag(Hotspot drag_source)
	{
		bool result = false;
		for (int i = 0; i < Hotspot.accept_drop_hotspots.Count; i++)
		{
			Hotspot hotspot = Hotspot.accept_drop_hotspots[i];
			string text = hotspot.ValidateDrop(drag_source, Hotspot.dragged_obj);
			if (text != null)
			{
				result = true;
				hotspot.UpdateDragHighlight(text, drag_source, null);
			}
		}
		return result;
	}

	// Token: 0x06001F74 RID: 8052 RVA: 0x00123100 File Offset: 0x00121300
	private static void FixFuckingUnityNotCloningImagePropertiesProperly(Transform t_src, Transform t_clone)
	{
		Image component = t_src.GetComponent<Image>();
		if (component != null && component.overrideSprite != null)
		{
			Image component2 = t_clone.GetComponent<Image>();
			if (component2 != null)
			{
				component2.overrideSprite = component.overrideSprite;
			}
		}
		for (int i = 0; i < t_src.childCount; i++)
		{
			Transform child = t_src.GetChild(i);
			Transform child2 = t_clone.GetChild(i);
			Hotspot.FixFuckingUnityNotCloningImagePropertiesProperly(child, child2);
		}
	}

	// Token: 0x06001F75 RID: 8053 RVA: 0x0002C538 File Offset: 0x0002A738
	public virtual bool AcceptsDrop()
	{
		return false;
	}

	// Token: 0x06001F76 RID: 8054 RVA: 0x000448AF File Offset: 0x00042AAF
	public virtual GameObject GetDragObject()
	{
		return null;
	}

	// Token: 0x06001F77 RID: 8055 RVA: 0x000023FD File Offset: 0x000005FD
	public virtual void PostProcessDragObject(GameObject obj)
	{
	}

	// Token: 0x06001F78 RID: 8056 RVA: 0x000448AF File Offset: 0x00042AAF
	public virtual string ValidateDrop(Hotspot src_hotspot, GameObject dragged_obj)
	{
		return null;
	}

	// Token: 0x06001F79 RID: 8057 RVA: 0x0002C538 File Offset: 0x0002A738
	public virtual bool AcceptDrop(string operation, Hotspot src_hotspot, GameObject dragged_obj)
	{
		return false;
	}

	// Token: 0x06001F7A RID: 8058 RVA: 0x00123170 File Offset: 0x00121370
	public virtual void UpdateDragHighlight(string operation, Hotspot src_hotspot, GameObject dragged_obj)
	{
		Image image = global::Common.FindChildComponent<Image>(base.gameObject, "id_DragHighlight");
		if (image == null)
		{
			return;
		}
		string text;
		if (src_hotspot == this)
		{
			text = "source";
		}
		else if (operation != null)
		{
			text = "accept";
		}
		else
		{
			text = null;
		}
		if (text == null)
		{
			image.gameObject.SetActive(false);
			return;
		}
		Color color = global::Defs.GetColor("DragAndDrop", "highlight_colors." + text, Color.clear);
		if (color.a == 0f)
		{
			image.gameObject.SetActive(false);
			return;
		}
		image.color = color;
		image.gameObject.SetActive(true);
	}

	// Token: 0x06001F7B RID: 8059 RVA: 0x00123210 File Offset: 0x00121410
	public virtual void OnDrag(PointerEventData e)
	{
		Action<Hotspot, EventTriggerType, PointerEventData> onPointerEvent = this.OnPointerEvent;
		if (onPointerEvent != null)
		{
			onPointerEvent(this, EventTriggerType.Drag, e);
		}
		if (Hotspot.dragged_obj != null)
		{
			Hotspot.dragged_obj.transform.position = Input.mousePosition;
		}
		Hotspot.UpdateDropTarget();
		Hotspot.UpdateDraggedObj();
	}

	// Token: 0x06001F7C RID: 8060 RVA: 0x0012325C File Offset: 0x0012145C
	public void OnEndDrag(PointerEventData eventData)
	{
		Hotspot.EndDrag(true);
	}

	// Token: 0x06001F7D RID: 8061 RVA: 0x00123268 File Offset: 0x00121468
	public static bool EndDrag(bool apply)
	{
		if (Hotspot.dragged_obj == null)
		{
			return false;
		}
		if (apply)
		{
			Hotspot.UpdateDropTarget();
			if (Hotspot.drop_target != null && Hotspot.drag_and_drop_operation != null)
			{
				Hotspot.drop_target.AcceptDrop(Hotspot.drag_and_drop_operation, Hotspot.drag_source, Hotspot.dragged_obj);
			}
		}
		for (int i = 0; i < Hotspot.accept_drop_hotspots.Count; i++)
		{
			Hotspot.accept_drop_hotspots[i].UpdateDragHighlight(null, null, null);
		}
		Hotspot hotspot = Hotspot.drag_source;
		if (hotspot != null)
		{
			hotspot.UpdateDragHighlight(null, null, null);
		}
		global::Common.DestroyObj(Hotspot.dragged_obj);
		Hotspot.dragged_obj = null;
		Hotspot.drag_source = null;
		Hotspot.drop_target = null;
		Hotspot.drag_and_drop_operation = null;
		return true;
	}

	// Token: 0x06001F7E RID: 8062 RVA: 0x00123318 File Offset: 0x00121518
	public static void EndDrag(Hotspot src)
	{
		if (Hotspot.drag_source == src)
		{
			Hotspot.EndDrag(false);
		}
	}

	// Token: 0x06001F7F RID: 8063 RVA: 0x000023FD File Offset: 0x000005FD
	public virtual void OnDragEnter(Hotspot src_hotspot, GameObject dragged_obj)
	{
	}

	// Token: 0x06001F80 RID: 8064 RVA: 0x000023FD File Offset: 0x000005FD
	public virtual void OnDragLeave(Hotspot src_hotspot, GameObject dragged_obj)
	{
	}

	// Token: 0x06001F81 RID: 8065 RVA: 0x00123330 File Offset: 0x00121530
	private static void UpdateDropTarget()
	{
		Hotspot dropTarget = Hotspot.GetDropTarget(out Hotspot.drag_and_drop_operation);
		if (dropTarget == Hotspot.drop_target)
		{
			return;
		}
		if (Hotspot.drop_target != null)
		{
			Hotspot.drop_target.OnDragLeave(Hotspot.drag_source, Hotspot.dragged_obj);
		}
		Hotspot.drop_target = dropTarget;
		if (Hotspot.drop_target != null)
		{
			Hotspot.drop_target.OnDragEnter(Hotspot.drag_source, Hotspot.dragged_obj);
		}
	}

	// Token: 0x06001F82 RID: 8066 RVA: 0x001233A0 File Offset: 0x001215A0
	private static Hotspot GetDropTarget(out string operation)
	{
		operation = null;
		if (Hotspot.dragged_obj == null)
		{
			return null;
		}
		Vector3 mousePosition = Input.mousePosition;
		for (int i = 0; i < Hotspot.accept_drop_hotspots.Count; i++)
		{
			Hotspot hotspot = Hotspot.accept_drop_hotspots[i];
			if (RectTransformUtility.RectangleContainsScreenPoint(hotspot.GetComponent<RectTransform>(), mousePosition))
			{
				operation = hotspot.ValidateDrop(Hotspot.drag_source, Hotspot.dragged_obj);
				if (operation != null)
				{
					return hotspot;
				}
			}
		}
		return null;
	}

	// Token: 0x06001F83 RID: 8067 RVA: 0x00123414 File Offset: 0x00121614
	private static void UpdateDraggedObj()
	{
		GameObject gameObject = global::Common.FindChildByName(Hotspot.dragged_obj, "id_Operation", true, true);
		if (gameObject == null)
		{
			return;
		}
		gameObject.SetActive(Hotspot.drag_and_drop_operation != null);
		if (Hotspot.drag_and_drop_operation == null)
		{
			return;
		}
		TextMeshProUGUI textMeshProUGUI = global::Common.FindChildComponent<TextMeshProUGUI>(gameObject, "id_OperationText");
		if (textMeshProUGUI == null)
		{
			return;
		}
		string text = global::Defs.Localize("DragAndDrop.operations." + Hotspot.drag_and_drop_operation, Hotspot.drag_source, null, false, true);
		UIText.SetText(textMeshProUGUI, text);
	}

	// Token: 0x06001F84 RID: 8068 RVA: 0x0012348D File Offset: 0x0012168D
	public virtual void OnPointerEnter(PointerEventData e)
	{
		this.mouse_in = true;
		Hotspot.SetPicked(this);
		Action<Hotspot, EventTriggerType, PointerEventData> onPointerEvent = this.OnPointerEvent;
		if (onPointerEvent != null)
		{
			onPointerEvent(this, EventTriggerType.PointerEnter, e);
		}
		BaseUI.PlaySoundEvent(this.rolloverSound, null);
	}

	// Token: 0x06001F85 RID: 8069 RVA: 0x001234BC File Offset: 0x001216BC
	public virtual void OnPointerExit(PointerEventData e)
	{
		this.mouse_in = false;
		Hotspot.SetPicked(null);
		Action<Hotspot, EventTriggerType, PointerEventData> onPointerEvent = this.OnPointerEvent;
		if (onPointerEvent == null)
		{
			return;
		}
		onPointerEvent(this, EventTriggerType.PointerExit, e);
	}

	// Token: 0x06001F86 RID: 8070 RVA: 0x001234DE File Offset: 0x001216DE
	private static void SetPicked(Hotspot hs)
	{
		Hotspot.picked = hs;
	}

	// Token: 0x06001F87 RID: 8071 RVA: 0x001234E8 File Offset: 0x001216E8
	public virtual void SetAudioSet(string def_id)
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

	// Token: 0x06001F88 RID: 8072 RVA: 0x0012354E File Offset: 0x0012174E
	protected virtual void OnEnable()
	{
		if (this.AcceptsDrop())
		{
			Hotspot.accept_drop_hotspots.Add(this);
		}
	}

	// Token: 0x06001F89 RID: 8073 RVA: 0x00123563 File Offset: 0x00121763
	protected virtual void OnDisable()
	{
		this.mouse_down = false;
		this.mouse_in = false;
		this.mouse_dragged = false;
		Hotspot.EndDrag(this);
		Hotspot.accept_drop_hotspots.Remove(this);
	}

	// Token: 0x040014D8 RID: 5336
	[HideInInspector]
	public PointerEventData.InputButton mouse_btn;

	// Token: 0x040014D9 RID: 5337
	[HideInInspector]
	public bool mouse_in;

	// Token: 0x040014DA RID: 5338
	[HideInInspector]
	public bool mouse_down;

	// Token: 0x040014DB RID: 5339
	[HideInInspector]
	public bool mouse_dragged;

	// Token: 0x040014DC RID: 5340
	public static GameObject dragged_obj = null;

	// Token: 0x040014DD RID: 5341
	public static Hotspot drag_source = null;

	// Token: 0x040014DE RID: 5342
	public static Hotspot drop_target = null;

	// Token: 0x040014DF RID: 5343
	public static string drag_and_drop_operation = null;

	// Token: 0x040014E0 RID: 5344
	[EventRef(compact = true)]
	public string rolloverSound;

	// Token: 0x040014E1 RID: 5345
	[EventRef(compact = true)]
	public string clickSound;

	// Token: 0x040014E2 RID: 5346
	public static Hotspot picked = null;

	// Token: 0x040014E4 RID: 5348
	private static List<Hotspot> accept_drop_hotspots = new List<Hotspot>();
}
