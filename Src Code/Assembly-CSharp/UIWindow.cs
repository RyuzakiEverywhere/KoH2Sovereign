using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x0200030B RID: 779
public class UIWindow : MonoBehaviour, IPoolable, IDragHandler, IEventSystemHandler, IPointerDownHandler
{
	// Token: 0x1700025F RID: 607
	// (get) Token: 0x0600308A RID: 12426 RVA: 0x0018905E File Offset: 0x0018725E
	// (set) Token: 0x0600308B RID: 12427 RVA: 0x00189066 File Offset: 0x00187266
	public UIWindow parentWindow
	{
		get
		{
			return this.m_parentWindow;
		}
		set
		{
			if (this.m_parentWindow == value)
			{
				return;
			}
			this.m_parentWindow = value;
			this.m_parentWindow.RemoveWindow(this);
		}
	}

	// Token: 0x0600308C RID: 12428 RVA: 0x0018908A File Offset: 0x0018728A
	public bool IsShown()
	{
		return this.isShown;
	}

	// Token: 0x0600308D RID: 12429 RVA: 0x00189092 File Offset: 0x00187292
	protected virtual void Awake()
	{
		this.rectTransform = (base.transform as RectTransform);
	}

	// Token: 0x0600308E RID: 12430 RVA: 0x001890A5 File Offset: 0x001872A5
	public virtual string GetDefId()
	{
		return UIWindow.def_id;
	}

	// Token: 0x0600308F RID: 12431 RVA: 0x001890AC File Offset: 0x001872AC
	public virtual void Open()
	{
		if (this == null)
		{
			return;
		}
		if (this.window_def == null)
		{
			this.LoadDef();
		}
		this.destroyWhenWindowFromSameExclusiveSetIsShown = true;
		this.Show();
	}

	// Token: 0x06003090 RID: 12432 RVA: 0x001890D3 File Offset: 0x001872D3
	public virtual void Close(bool silent = false)
	{
		if (this.on_close != null)
		{
			this.on_close(this);
		}
		this.on_close = null;
		this.Hide(silent);
		global::Common.DestroyObj(base.gameObject);
	}

	// Token: 0x06003091 RID: 12433 RVA: 0x00189102 File Offset: 0x00187302
	protected virtual void OnDestroy()
	{
		if (this.isShown)
		{
			this.Hide(false);
			if (this.on_close != null)
			{
				this.on_close(this);
			}
		}
	}

	// Token: 0x06003092 RID: 12434 RVA: 0x00189127 File Offset: 0x00187327
	public virtual void Show(bool show)
	{
		if (show)
		{
			this.Show();
			return;
		}
		this.Hide(false);
	}

	// Token: 0x06003093 RID: 12435 RVA: 0x0018913C File Offset: 0x0018733C
	public virtual void LoadDef()
	{
		this.window_def = global::Defs.GetDefField(this.GetDefId(), null);
		if (this.window_def == null)
		{
			return;
		}
		this.openSound = this.window_def.GetString("open_sound", null, this.openSound, true, true, true, '.');
		this.openVoice = this.window_def.GetString("open_voice", null, this.openVoice, true, true, true, '.');
		this.closeSound = this.window_def.GetString("close_sound", null, this.closeSound, true, true, true, '.');
		this.persist = this.window_def.GetString("persist", null, this.persist, true, true, true, '.');
		this.keepOnScreen = this.window_def.GetFloat("keep_on_screen", null, this.keepOnScreen, true, true, true, '.');
		this.draggable = this.window_def.GetBool("draggable", null, this.draggable, true, true, true, '.');
		this.exclusiveWindowSet = this.window_def.GetString("exclusive_window_set", null, this.exclusiveWindowSet, true, true, true, '.');
		this.makeTransformLastSiblingOnSetFocus = this.window_def.GetBool("make_transform_last_sibling_on_set_focus", null, this.makeTransformLastSiblingOnSetFocus, true, true, true, '.');
	}

	// Token: 0x06003094 RID: 12436 RVA: 0x00189274 File Offset: 0x00187474
	public static bool NoExclusiveWindowSet(string set)
	{
		return string.IsNullOrEmpty(set) || set == "None";
	}

	// Token: 0x06003095 RID: 12437 RVA: 0x0018928B File Offset: 0x0018748B
	public bool IsFromExclusiveWindowSet(string set)
	{
		return !UIWindow.NoExclusiveWindowSet(this.exclusiveWindowSet) && this.exclusiveWindowSet == set;
	}

	// Token: 0x06003096 RID: 12438 RVA: 0x001892A8 File Offset: 0x001874A8
	public virtual void Show()
	{
		if (this == null)
		{
			return;
		}
		if (this.isShown)
		{
			return;
		}
		if (this.window_def == null)
		{
			this.LoadDef();
		}
		bool flag = false;
		BaseUI baseUI = BaseUI.Get();
		UIWindow uiwindow = (baseUI != null) ? baseUI.window_dispatcher.GetWindowFromExclusiveSet(this.exclusiveWindowSet) : null;
		if (uiwindow != null && uiwindow != this)
		{
			flag = this.IsSimilar(uiwindow);
			if (this.destroyWhenWindowFromSameExclusiveSetIsShown)
			{
				uiwindow.Close(true);
			}
			else
			{
				uiwindow.Hide(true);
			}
		}
		string param = global::Common.ObjPath(base.gameObject);
		Analytics instance = Analytics.instance;
		if (instance != null)
		{
			instance.OnMessage(base.gameObject, "ui_window_shown", param);
		}
		Tutorial.Rule.OnMessage(base.gameObject, "ui_window_shown", param);
		Vector3 position;
		if (UIWindowDispatcher.dict.TryGetValue(this.persist, out position))
		{
			this.rectTransform.position = position;
		}
		this.EnsureInScreen();
		this.SetOnFocus();
		if (this.openSound != null && !flag)
		{
			DT.Field soundsDef = BaseUI.soundsDef;
			BaseUI.PlaySoundEvent((soundsDef != null) ? soundsDef.GetString(this.openSound, null, "", true, true, true, '.') : null, null);
		}
		if (!string.IsNullOrEmpty(this.openVoice) && !flag)
		{
			Vars vars = new Vars();
			vars.SetVar("def_id", this.GetDefId());
			DT.Field soundsDef2 = BaseUI.soundsDef;
			BaseUI.PlayVoiceEvent((soundsDef2 != null) ? soundsDef2.GetString(this.openVoice, null, this.openVoice, true, true, true, '.') : null, vars);
		}
		this.isShown = true;
	}

	// Token: 0x06003097 RID: 12439 RVA: 0x00189420 File Offset: 0x00187620
	public virtual void Hide(bool silent = false)
	{
		if (!this.isShown)
		{
			return;
		}
		string param = global::Common.ObjPath(base.gameObject);
		Tutorial.Rule.OnMessage(base.gameObject, "ui_window_hidden", param);
		BaseUI baseUI = BaseUI.Get();
		if (baseUI != null)
		{
			baseUI.window_dispatcher.DismissWindow(this);
		}
		if (this == null)
		{
			return;
		}
		if (this.closeSound != null && !silent)
		{
			DT.Field soundsDef = BaseUI.soundsDef;
			BaseUI.PlaySoundEvent((soundsDef != null) ? soundsDef.GetString(this.closeSound, null, "", true, true, true, '.') : null, null);
		}
		this.isShown = false;
	}

	// Token: 0x06003098 RID: 12440 RVA: 0x0002C538 File Offset: 0x0002A738
	public virtual bool IsSimilar(UIWindow w)
	{
		return false;
	}

	// Token: 0x06003099 RID: 12441 RVA: 0x001894AD File Offset: 0x001876AD
	protected virtual void Update()
	{
		this.CheckOutOfBoundsClick();
	}

	// Token: 0x0600309A RID: 12442 RVA: 0x001894B5 File Offset: 0x001876B5
	public void RemoveWindow(UIWindow widnow)
	{
		if (this.children.Contains(widnow))
		{
			this.children.Remove(widnow);
		}
	}

	// Token: 0x0600309B RID: 12443 RVA: 0x001894D2 File Offset: 0x001876D2
	public void AddWindow(UIWindow widnow)
	{
		if (widnow == null)
		{
			return;
		}
		if (widnow.parentWindow == this)
		{
			return;
		}
		widnow.parentWindow = this;
		this.children.Add(widnow);
	}

	// Token: 0x0600309C RID: 12444 RVA: 0x00189500 File Offset: 0x00187700
	private void CheckOutOfBoundsClick()
	{
		if ((Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)) && !RectTransformUtility.RectangleContainsScreenPoint(this.rectTransform, Input.mousePosition, null) && this.OnOutOfBoundsClick != null)
		{
			this.OnOutOfBoundsClick(this);
		}
	}

	// Token: 0x0600309D RID: 12445 RVA: 0x00189540 File Offset: 0x00187740
	public void OnDrag(PointerEventData eventData)
	{
		if (!this.draggable)
		{
			return;
		}
		if (this.lastDragFrame == UnityEngine.Time.frameCount)
		{
			return;
		}
		this.lastDragFrame = UnityEngine.Time.frameCount;
		this.rectTransform.position += new Vector3(eventData.delta.x, eventData.delta.y);
		this.EnsureInScreen();
		if (!string.IsNullOrEmpty(this.persist))
		{
			UIWindowDispatcher.dict[this.persist] = this.rectTransform.position;
		}
	}

	// Token: 0x0600309E RID: 12446 RVA: 0x001895CE File Offset: 0x001877CE
	public void OnPointerDown(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Middle)
		{
			return;
		}
		if (eventData.button == PointerEventData.InputButton.Right && BaseUI.IgnoreRightClick())
		{
			return;
		}
		this.SetOnFocus();
	}

	// Token: 0x0600309F RID: 12447 RVA: 0x001895F1 File Offset: 0x001877F1
	public virtual bool OnBackInputAction()
	{
		this.Close(false);
		return true;
	}

	// Token: 0x060030A0 RID: 12448 RVA: 0x001895FB File Offset: 0x001877FB
	public virtual void OnOutOfBoundsClickAction()
	{
		if (this.OnOutOfBoundsClick != null)
		{
			this.OnOutOfBoundsClick(this);
		}
	}

	// Token: 0x060030A1 RID: 12449 RVA: 0x00189611 File Offset: 0x00187811
	public bool IsOnFocus()
	{
		return BaseUI.Get().window_dispatcher.GetFocusWindow() == this;
	}

	// Token: 0x060030A2 RID: 12450 RVA: 0x00189628 File Offset: 0x00187828
	public void SetOnFocus()
	{
		BaseUI baseUI = BaseUI.Get();
		if (baseUI != null)
		{
			UIWindowDispatcher window_dispatcher = baseUI.window_dispatcher;
			if (window_dispatcher != null)
			{
				window_dispatcher.SetFocusWindow(this);
			}
		}
		if (this.makeTransformLastSiblingOnSetFocus)
		{
			base.gameObject.transform.SetAsLastSibling();
		}
	}

	// Token: 0x060030A3 RID: 12451 RVA: 0x00189660 File Offset: 0x00187860
	private void EnsureInScreen()
	{
		if (this.rectTransform == null)
		{
			this.rectTransform = base.GetComponent<RectTransform>();
		}
		if (this.rectTransform == null)
		{
			return;
		}
		Vector3[] array = new Vector3[4];
		this.rectTransform.GetWorldCorners(array);
		float x = array[0].x;
		float y = array[0].y;
		float num = array[2].x - array[1].x;
		float num2 = array[1].y - array[0].y;
		float num3 = num * (1f - this.keepOnScreen);
		float num4 = num2 * (1f - this.keepOnScreen);
		float num5 = (float)Screen.height;
		float num6 = (float)Screen.width;
		Vector3 position = this.rectTransform.position;
		float num7 = position.x - (x + num / 2f);
		float num8 = position.y - (y + num2 / 2f);
		if (x + num3 < 0f)
		{
			position.x = -num3 + num / 2f + num7;
		}
		else if (x + num - num3 > num6)
		{
			position.x = num6 + num3 - num / 2f + num7;
		}
		if (y + num4 < 0f)
		{
			position.y = -num4 + num2 / 2f + num8;
		}
		else if (y + num2 - num4 > num5)
		{
			position.y = num5 + num4 - num2 / 2f + num8;
		}
		this.rectTransform.position = position;
	}

	// Token: 0x060030A4 RID: 12452 RVA: 0x000023FD File Offset: 0x000005FD
	public virtual void OnPoolSpawned()
	{
	}

	// Token: 0x060030A5 RID: 12453 RVA: 0x000023FD File Offset: 0x000005FD
	public virtual void OnPoolActivated()
	{
	}

	// Token: 0x060030A6 RID: 12454 RVA: 0x001897F3 File Offset: 0x001879F3
	public virtual void OnPoolDeactivated()
	{
		this.OnDestroy();
	}

	// Token: 0x060030A7 RID: 12455 RVA: 0x000023FD File Offset: 0x000005FD
	public virtual void OnPoolDestroyed()
	{
	}

	// Token: 0x0400208F RID: 8335
	private UIWindow m_parentWindow;

	// Token: 0x04002090 RID: 8336
	[NonSerialized]
	public List<UIWindow> children = new List<UIWindow>();

	// Token: 0x04002091 RID: 8337
	public Action<UIWindow> OnOutOfBoundsClick;

	// Token: 0x04002092 RID: 8338
	public string persist = "";

	// Token: 0x04002093 RID: 8339
	public float keepOnScreen = 0.75f;

	// Token: 0x04002094 RID: 8340
	protected string openSound;

	// Token: 0x04002095 RID: 8341
	public string openVoice;

	// Token: 0x04002096 RID: 8342
	protected string closeSound;

	// Token: 0x04002097 RID: 8343
	protected bool draggable = true;

	// Token: 0x04002098 RID: 8344
	public DT.Field window_def;

	// Token: 0x04002099 RID: 8345
	public string exclusiveWindowSet = "None";

	// Token: 0x0400209A RID: 8346
	public bool makeTransformLastSiblingOnSetFocus = true;

	// Token: 0x0400209B RID: 8347
	private RectTransform rectTransform;

	// Token: 0x0400209C RID: 8348
	private bool isShown;

	// Token: 0x0400209D RID: 8349
	protected bool m_Initialized;

	// Token: 0x0400209E RID: 8350
	public UIWindow.OnClose on_close;

	// Token: 0x0400209F RID: 8351
	private bool destroyWhenWindowFromSameExclusiveSetIsShown;

	// Token: 0x040020A0 RID: 8352
	private static string def_id = "UIWindow";

	// Token: 0x040020A1 RID: 8353
	private int lastDragFrame;

	// Token: 0x02000873 RID: 2163
	// (Invoke) Token: 0x06005136 RID: 20790
	public delegate void OnClose(UIWindow wnd);
}
