using System;
using System.Collections;
using Logic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200021A RID: 538
public class UIReligion : MonoBehaviour, IListener
{
	// Token: 0x06002098 RID: 8344 RVA: 0x00129C90 File Offset: 0x00127E90
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
		this.vars = new Vars();
		Tooltip.Get(base.gameObject, true).SetDef("ReligionTooltip", this.vars);
		this.m_Initiazled = true;
	}

	// Token: 0x06002099 RID: 8345 RVA: 0x00129CFD File Offset: 0x00127EFD
	private IEnumerator Start()
	{
		while (this.Kingdom == null)
		{
			Logic.Kingdom kingdom = BaseUI.LogicKingdom();
			if (kingdom != null)
			{
				this.SetData(kingdom);
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x0600209A RID: 8346 RVA: 0x00129D0C File Offset: 0x00127F0C
	private void OnDestroy()
	{
		if (this.Kingdom != null)
		{
			this.Kingdom.DelListener(this);
		}
	}

	// Token: 0x0600209B RID: 8347 RVA: 0x00129D24 File Offset: 0x00127F24
	public void SetData(Logic.Kingdom k)
	{
		this.Init();
		if (this.Kingdom != null)
		{
			this.Kingdom.DelListener(this);
		}
		this.Kingdom = k;
		if (this.Kingdom != null)
		{
			this.Kingdom.AddListener(this);
		}
		this.vars.obj = this.Kingdom.religion;
		this.vars.Set<Logic.Kingdom>("kingdom", this.Kingdom);
		this.Refresh();
	}

	// Token: 0x0600209C RID: 8348 RVA: 0x00129D9D File Offset: 0x00127F9D
	private void HandleOnClick(BSGButton b)
	{
		if (this.Kingdom == BaseUI.LogicKingdom())
		{
			this.OpenReligion();
		}
	}

	// Token: 0x0600209D RID: 8349 RVA: 0x00129DB4 File Offset: 0x00127FB4
	private void OpenReligion()
	{
		if (this.Kingdom == null)
		{
			return;
		}
		GameObject gameObject = global::Common.FindChildByName(base.gameObject.transform.root.gameObject, "id_MessageContainer", true, true);
		if (gameObject == null)
		{
			return;
		}
		if (!UICommon.DeleteChildren(gameObject.transform, typeof(UIReligionWindow)))
		{
			UIReligionWindow.Create(this.Kingdom, UIReligionWindow.GetPrefab(), gameObject.transform as RectTransform);
		}
	}

	// Token: 0x0600209E RID: 8350 RVA: 0x00129E2C File Offset: 0x0012802C
	private void Refresh()
	{
		if (this.m_Icon == null)
		{
			return;
		}
		if (this.Kingdom == null)
		{
			return;
		}
		if (this.Kingdom.religion == null)
		{
			return;
		}
		this.m_Icon.overrideSprite = global::Defs.GetObj<Sprite>(this.Kingdom.religion.def.field, global::Religions.GetRelgionIconKey(this.Kingdom), null);
	}

	// Token: 0x0600209F RID: 8351 RVA: 0x00129E90 File Offset: 0x00128090
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "religion_changed" || message == "excommunicated" || message == "unexcommunicated" || message == "autocephaly" || message == "subordinated")
		{
			this.Refresh();
		}
		if (message == "destroying" || message == "finishing")
		{
			Logic.Kingdom kingdom = this.Kingdom;
			if (kingdom != null)
			{
				kingdom.DelListener(this);
			}
			this.Kingdom = null;
			if (!this.KeepAlive && this != null)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}

	// Token: 0x060020A0 RID: 8352 RVA: 0x00129F34 File Offset: 0x00128134
	public static UIReligion Create(Logic.Object obj, GameObject prototype, RectTransform parent, Vars vars)
	{
		if (obj == null)
		{
			return null;
		}
		if (!(obj is Logic.Kingdom))
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
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prototype, Vector3.zero, Quaternion.identity, parent);
		UIReligion uireligion = gameObject.GetComponent<UIReligion>();
		if (uireligion == null)
		{
			uireligion = gameObject.AddComponent<UIReligion>();
		}
		uireligion.SetData(obj as Logic.Kingdom);
		return uireligion;
	}

	// Token: 0x040015B8 RID: 5560
	[UIFieldTarget("id_Icon")]
	private Image m_Icon;

	// Token: 0x040015B9 RID: 5561
	private Logic.Kingdom Kingdom;

	// Token: 0x040015BA RID: 5562
	private Vars vars;

	// Token: 0x040015BB RID: 5563
	public bool KeepAlive;

	// Token: 0x040015BC RID: 5564
	private bool m_Initiazled;
}
