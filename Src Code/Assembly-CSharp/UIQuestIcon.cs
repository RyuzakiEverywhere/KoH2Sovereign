using System;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000277 RID: 631
public class UIQuestIcon : ObjectIcon, IListener
{
	// Token: 0x170001CA RID: 458
	// (get) Token: 0x060026A9 RID: 9897 RVA: 0x00152851 File Offset: 0x00150A51
	// (set) Token: 0x060026AA RID: 9898 RVA: 0x00152859 File Offset: 0x00150A59
	public Quest Data { get; private set; }

	// Token: 0x170001CB RID: 459
	// (get) Token: 0x060026AB RID: 9899 RVA: 0x00152862 File Offset: 0x00150A62
	// (set) Token: 0x060026AC RID: 9900 RVA: 0x0015286A File Offset: 0x00150A6A
	public Vars Vars { get; private set; }

	// Token: 0x14000032 RID: 50
	// (add) Token: 0x060026AD RID: 9901 RVA: 0x00152874 File Offset: 0x00150A74
	// (remove) Token: 0x060026AE RID: 9902 RVA: 0x001528AC File Offset: 0x00150AAC
	public event Action<UIQuestIcon> OnSelect;

	// Token: 0x14000033 RID: 51
	// (add) Token: 0x060026AF RID: 9903 RVA: 0x001528E4 File Offset: 0x00150AE4
	// (remove) Token: 0x060026B0 RID: 9904 RVA: 0x0015291C File Offset: 0x00150B1C
	public event Action<UIQuestIcon> OnFocus;

	// Token: 0x060026B1 RID: 9905 RVA: 0x00112285 File Offset: 0x00110485
	public override void Awake()
	{
		base.Awake();
		if (this.logicObject == null)
		{
			this.SetObject(null, null);
		}
	}

	// Token: 0x060026B2 RID: 9906 RVA: 0x00152951 File Offset: 0x00150B51
	private void OnDestroy()
	{
		this.OnSelect = null;
		this.OnFocus = null;
		if (this.Data != null)
		{
			this.Data.DelListener(this);
		}
	}

	// Token: 0x060026B3 RID: 9907 RVA: 0x00152978 File Offset: 0x00150B78
	public override void SetObject(object obj, Vars vars = null)
	{
		base.SetObject(obj, vars);
		if (this.Data != null)
		{
			this.Data.DelListener(this);
		}
		if (obj != null)
		{
			Tooltip.Get(base.gameObject, true).SetObj(obj, null, null);
			Quest data;
			if ((data = (obj as Quest)) != null)
			{
				this.Data = data;
				this.Data.AddListener(this);
			}
		}
		else
		{
			this.Data = null;
			Tooltip tooltip = Tooltip.Get(base.gameObject, false);
			if (tooltip != null)
			{
				UnityEngine.Object.Destroy(tooltip);
			}
		}
		UICommon.FindComponents(this, false);
		this.Refresh();
	}

	// Token: 0x060026B4 RID: 9908 RVA: 0x00152A08 File Offset: 0x00150C08
	private void Refresh()
	{
		if (this.Data != null)
		{
			if (this.Data is UnionQuest)
			{
				this.id_Crest.SetObject(this.Data.game.GetKingdom(this.Data.def.field.GetString("outcome.unlock_union", null, "", true, true, true, '.')), null);
			}
			else
			{
				this.id_Crest.SetObject(this.Data.owner as Logic.Kingdom, null);
			}
			Tooltip.Get(base.gameObject, true).SetObj(this.Data, null, null);
		}
	}

	// Token: 0x060026B5 RID: 9909 RVA: 0x00152AA8 File Offset: 0x00150CA8
	public override void OnClick(PointerEventData e)
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

	// Token: 0x060026B6 RID: 9910 RVA: 0x00152B04 File Offset: 0x00150D04
	public static UIQuestIcon Create(Quest quest, GameObject prototype, RectTransform parent, Vars vars)
	{
		if (quest == null)
		{
			Debug.LogWarning("Fail to create quest icon! Reson: no character data e provided.");
			return null;
		}
		if (prototype == null)
		{
			Debug.LogWarning("Fail to create quest Info widnow! Reson: no prototype provided.");
			return null;
		}
		if (parent == null)
		{
			Debug.LogWarning("Fail to create quest Info widnow! Reson: no parent provided.");
			return null;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prototype, Vector3.zero, Quaternion.identity, parent);
		UIQuestIcon uiquestIcon = gameObject.GetComponent<UIQuestIcon>();
		if (uiquestIcon == null)
		{
			uiquestIcon = gameObject.AddComponent<UIQuestIcon>();
		}
		uiquestIcon.SetObject(quest, vars);
		return uiquestIcon;
	}

	// Token: 0x060026B7 RID: 9911 RVA: 0x00152B7C File Offset: 0x00150D7C
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "quest_changed")
		{
			this.Refresh();
		}
	}

	// Token: 0x04001A2B RID: 6699
	[UIFieldTarget("id_Crest")]
	private UIKingdomIcon id_Crest;

	// Token: 0x04001A2C RID: 6700
	private bool m_Selected;
}
