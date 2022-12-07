using System;
using System.Collections;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x0200030D RID: 781
public class WatchesWindow : MonoBehaviour
{
	// Token: 0x060030C6 RID: 12486 RVA: 0x00189FD4 File Offset: 0x001881D4
	public static void Show()
	{
		if (WatchesWindow.instance == null)
		{
			BaseUI baseUI = BaseUI.Get();
			if (((baseUI != null) ? baseUI.canvas : null) == null)
			{
				return;
			}
			GameObject tutorial_message_container = baseUI.tutorial_message_container;
			Transform transform;
			if ((transform = ((tutorial_message_container != null) ? tutorial_message_container.transform : null)) == null)
			{
				GameObject message_container = baseUI.message_container;
				transform = (((message_container != null) ? message_container.transform : null) ?? baseUI.canvas.transform);
			}
			Transform parent = transform;
			GameObject gameObject = Assets.Get<GameObject>("Assets/UI/Debug/UIP_Watches.prefab");
			if (gameObject == null)
			{
				return;
			}
			WatchesWindow.instance = UnityEngine.Object.Instantiate<GameObject>(gameObject, parent).GetComponent<WatchesWindow>();
		}
		WatchesWindow.instance.gameObject.SetActive(true);
	}

	// Token: 0x060030C7 RID: 12487 RVA: 0x0018A077 File Offset: 0x00188277
	public static void Hide()
	{
		if (WatchesWindow.instance == null)
		{
			return;
		}
		WatchesWindow.instance.gameObject.SetActive(false);
	}

	// Token: 0x060030C8 RID: 12488 RVA: 0x0018A098 File Offset: 0x00188298
	public static void Clear()
	{
		WatchesWindow watchesWindow = WatchesWindow.instance;
		if (((watchesWindow != null) ? watchesWindow.watches : null) == null)
		{
			return;
		}
		for (int i = WatchesWindow.instance.watches.Count - 1; i > 0; i--)
		{
			WatchesWindow.instance.watches[i].Del();
		}
		WatchesWindow.instance.watches.Clear();
	}

	// Token: 0x060030C9 RID: 12489 RVA: 0x0018A0FC File Offset: 0x001882FC
	public static void AddWatch(Logic.Object obj, Expression expression)
	{
		WatchesWindow.Show();
		if (WatchesWindow.instance == null)
		{
			return;
		}
		WatchesWindow.instance.Init();
		if (WatchesWindow.instance.watches == null || WatchesWindow.instance.watches.Count == 0)
		{
			return;
		}
		GameObject go = UnityEngine.Object.Instantiate<GameObject>(WatchesWindow.instance.watches[0].go, WatchesWindow.instance.transform);
		new WatchesWindow.Watch(WatchesWindow.instance, obj, expression, go);
	}

	// Token: 0x060030CA RID: 12490 RVA: 0x0018A178 File Offset: 0x00188378
	private void Init()
	{
		if (this.watches != null)
		{
			return;
		}
		this.watches = new List<WatchesWindow.Watch>();
		if (base.transform.childCount == 0)
		{
			return;
		}
		GameObject gameObject = base.transform.GetChild(0).gameObject;
		new WatchesWindow.Watch(WatchesWindow.instance, null, null, gameObject);
	}

	// Token: 0x060030CB RID: 12491 RVA: 0x0018A1C7 File Offset: 0x001883C7
	private void OnEnable()
	{
		if (WatchesWindow.instance == null)
		{
			WatchesWindow.instance = this;
		}
	}

	// Token: 0x060030CC RID: 12492 RVA: 0x0018A1DC File Offset: 0x001883DC
	private void OnDestroy()
	{
		if (WatchesWindow.instance == this)
		{
			WatchesWindow.instance = null;
		}
	}

	// Token: 0x060030CD RID: 12493 RVA: 0x0018A1F4 File Offset: 0x001883F4
	private void Update()
	{
		if (this.watches == null)
		{
			return;
		}
		for (int i = 0; i < this.watches.Count; i++)
		{
			this.watches[i].Refresh();
		}
	}

	// Token: 0x040020B0 RID: 8368
	private static WatchesWindow instance;

	// Token: 0x040020B1 RID: 8369
	private List<WatchesWindow.Watch> watches;

	// Token: 0x02000874 RID: 2164
	private class Watch
	{
		// Token: 0x06005139 RID: 20793 RVA: 0x0023C0A0 File Offset: 0x0023A2A0
		public Watch(WatchesWindow wnd, Logic.Object obj, Expression expression, GameObject go)
		{
			this.obj = obj;
			this.expression = expression;
			this.go = go;
			this.id_del = global::Common.FindChildComponent<BSGButton>(go, "id_del");
			this.id_obj = global::Common.FindChildComponent<TextMeshProUGUI>(go, "id_obj");
			this.id_expression = global::Common.FindChildComponent<TextMeshProUGUI>(go, "id_expression");
			this.id_value = global::Common.FindChildComponent<TextMeshProUGUI>(go, "id_value");
			wnd.watches.Add(this);
			if (this.id_del != null)
			{
				if (expression == null)
				{
					this.id_del.onClick = delegate(BSGButton btn)
					{
						WatchesWindow.Hide();
					};
				}
				else
				{
					this.id_del.onClick = delegate(BSGButton btn)
					{
						this.Del();
					};
				}
			}
			this.Refresh();
		}

		// Token: 0x0600513A RID: 20794 RVA: 0x0023C177 File Offset: 0x0023A377
		public void Del()
		{
			WatchesWindow instance = WatchesWindow.instance;
			if (instance != null)
			{
				List<WatchesWindow.Watch> watches = instance.watches;
				if (watches != null)
				{
					watches.Remove(this);
				}
			}
			UnityEngine.Object.Destroy(this.go);
		}

		// Token: 0x0600513B RID: 20795 RVA: 0x0023C1A4 File Offset: 0x0023A3A4
		private void SetText(TextMeshProUGUI component, string txt)
		{
			if (component == null)
			{
				return;
			}
			component.text = txt;
			Tooltip tooltip = Tooltip.Get(component.gameObject, true);
			tooltip.max_width = (float)Screen.width * 0.9f;
			tooltip.SetText("#" + txt, null, null);
			tooltip.pinable = 1;
		}

		// Token: 0x0600513C RID: 20796 RVA: 0x0023C1FC File Offset: 0x0023A3FC
		public void Refresh()
		{
			if (this.expression == null)
			{
				return;
			}
			this.SetText(this.id_obj, Logic.Object.ToString(this.obj));
			this.SetText(this.id_expression, this.expression.ToString());
			if (this.id_value == null)
			{
				return;
			}
			Vars.ReflectionMode old_mode = Vars.PushReflectionMode(Vars.ReflectionMode.Enabled);
			DefsContext context = GameLogic.Get(true).dt.context;
			context.vars = new Vars(this.obj);
			Value value = this.expression.Calc(context, true);
			Value val = this.expression.Calc(context, false);
			context.vars = null;
			Vars.PopReflectionMode(old_mode);
			string str = Logic.Object.Dump(value);
			string str2 = this.Dump(val);
			this.id_value.text = "<noparse>" + str + "</noparse>";
			Tooltip tooltip = Tooltip.Get(this.id_value.gameObject, true);
			tooltip.max_width = (float)Screen.width * 0.9f;
			tooltip.SetText("#" + str2, null, null);
		}

		// Token: 0x0600513D RID: 20797 RVA: 0x0023C30C File Offset: 0x0023A50C
		public string Dump(Value val)
		{
			IList lst;
			if ((lst = (val.obj_val as IList)) != null)
			{
				Vars vars = new Vars(this.obj);
				string text = global::Defs.LocalizeList(lst, vars, "", "\n", "\n", false);
				if (text != null)
				{
					return text;
				}
			}
			if (val.obj_val != null)
			{
				Vars vars2 = new Vars(this.obj);
				string text2 = global::Defs.LocalizedObjName(val.obj_val, vars2, "", false);
				if (text2 != null)
				{
					return text2;
				}
			}
			return Logic.Object.Dump(val);
		}

		// Token: 0x04003F3C RID: 16188
		public Logic.Object obj;

		// Token: 0x04003F3D RID: 16189
		public Expression expression;

		// Token: 0x04003F3E RID: 16190
		public GameObject go;

		// Token: 0x04003F3F RID: 16191
		public BSGButton id_del;

		// Token: 0x04003F40 RID: 16192
		public TextMeshProUGUI id_obj;

		// Token: 0x04003F41 RID: 16193
		public TextMeshProUGUI id_expression;

		// Token: 0x04003F42 RID: 16194
		public TextMeshProUGUI id_value;
	}
}
