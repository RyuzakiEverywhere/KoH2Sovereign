using System;
using Logic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000284 RID: 644
public class UIRealmReligionIcon : ObjectIcon, IListener
{
	// Token: 0x0600274E RID: 10062 RVA: 0x00155A30 File Offset: 0x00153C30
	private void Init()
	{
		if (this.m_Initiazled)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		BSGButton component = base.GetComponent<BSGButton>();
		if (component != null)
		{
			component.onClick = new BSGButton.OnClick(this.HandleOnClick);
		}
		this.m_Vars = new Vars();
		Tooltip.Get(base.gameObject, true).SetDef("ReligionTensionTooltip", this.m_Vars);
		this.m_Initiazled = true;
	}

	// Token: 0x0600274F RID: 10063 RVA: 0x00155A9D File Offset: 0x00153C9D
	public override void SetObject(object obj, Vars vars)
	{
		this.Init();
		base.SetObject(obj, vars);
		this.SetData(obj as Logic.Realm);
	}

	// Token: 0x06002750 RID: 10064 RVA: 0x00155ABC File Offset: 0x00153CBC
	public void SetData(Logic.Realm r)
	{
		this.Init();
		if (this.Realm != null)
		{
			this.Realm.DelListener(this);
		}
		this.Realm = r;
		if (this.Realm != null)
		{
			this.Realm.AddListener(this);
		}
		this.m_Vars.obj = this.Realm;
		this.Refresh();
	}

	// Token: 0x06002751 RID: 10065 RVA: 0x000023FD File Offset: 0x000005FD
	private void HandleOnClick(BSGButton b)
	{
	}

	// Token: 0x06002752 RID: 10066 RVA: 0x00155B1C File Offset: 0x00153D1C
	private void Refresh()
	{
		if (this.m_Icon == null)
		{
			return;
		}
		if (this.Realm == null)
		{
			return;
		}
		if (this.Realm.religion == null)
		{
			return;
		}
		this.m_Icon.sprite = global::Defs.GetObj<Sprite>(this.Realm.religion.def.field, "icon", null);
	}

	// Token: 0x06002753 RID: 10067 RVA: 0x00155B7C File Offset: 0x00153D7C
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "religion_changed")
		{
			this.Refresh();
		}
		if (message == "destroying" || message == "finishing")
		{
			Logic.Realm realm = this.Realm;
			if (realm != null)
			{
				realm.DelListener(this);
			}
			this.Realm = null;
			if (!this.KeepAlive && this != null)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}

	// Token: 0x06002754 RID: 10068 RVA: 0x00155BEB File Offset: 0x00153DEB
	public static UIRealmReligionIcon Create(Logic.Object obj, GameObject prototype, RectTransform parent, Vars vars)
	{
		if (obj == null)
		{
			return null;
		}
		if (prototype == null)
		{
			return null;
		}
		if (parent == null)
		{
			return null;
		}
		UIRealmReligionIcon orAddComponent = global::Common.Spawn(prototype, parent, false, "").GetOrAddComponent<UIRealmReligionIcon>();
		orAddComponent.SetObject(obj, vars);
		return orAddComponent;
	}

	// Token: 0x04001AAF RID: 6831
	[UIFieldTarget("id_RealmReligonIcon")]
	private Image m_Icon;

	// Token: 0x04001AB0 RID: 6832
	private Logic.Realm Realm;

	// Token: 0x04001AB1 RID: 6833
	private Vars m_Vars;

	// Token: 0x04001AB2 RID: 6834
	public bool KeepAlive;

	// Token: 0x04001AB3 RID: 6835
	private bool m_Initiazled;
}
