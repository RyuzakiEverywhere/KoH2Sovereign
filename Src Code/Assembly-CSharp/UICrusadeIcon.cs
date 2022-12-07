using System;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000235 RID: 565
public class UICrusadeIcon : ObjectIcon, IListener, IPoolable
{
	// Token: 0x170001B9 RID: 441
	// (get) Token: 0x0600227B RID: 8827 RVA: 0x00138C34 File Offset: 0x00136E34
	// (set) Token: 0x0600227C RID: 8828 RVA: 0x00138C3C File Offset: 0x00136E3C
	public Crusade Crusade { get; private set; }

	// Token: 0x1400002D RID: 45
	// (add) Token: 0x0600227D RID: 8829 RVA: 0x00138C48 File Offset: 0x00136E48
	// (remove) Token: 0x0600227E RID: 8830 RVA: 0x00138C80 File Offset: 0x00136E80
	public event Action<UICrusadeIcon> OnSelect;

	// Token: 0x1400002E RID: 46
	// (add) Token: 0x0600227F RID: 8831 RVA: 0x00138CB8 File Offset: 0x00136EB8
	// (remove) Token: 0x06002280 RID: 8832 RVA: 0x00138CF0 File Offset: 0x00136EF0
	public event Action<UICrusadeIcon> OnFocus;

	// Token: 0x06002281 RID: 8833 RVA: 0x00138D28 File Offset: 0x00136F28
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_CrusaderIcon != null)
		{
			this.m_CrusaderIcon.OnSelect += this.HandleOnCharacterSelect;
			this.m_CrusaderIcon.OnFocus += this.HandleOnCharacterFocus;
			this.m_CrusaderIcon.DisableTooltip(true);
		}
		if (this.m_Banner != null)
		{
			this.m_Banner.onClick = new BSGButton.OnClick(this.HandleOpenWindow);
		}
		this.m_Initialzied = true;
	}

	// Token: 0x06002282 RID: 8834 RVA: 0x00138DB9 File Offset: 0x00136FB9
	private void HandleOpenWindow(BSGButton btn)
	{
		if (BaseUI.Get().dblclk)
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

	// Token: 0x06002283 RID: 8835 RVA: 0x00138DF0 File Offset: 0x00136FF0
	public override void SetObject(object obj, Vars vars = null)
	{
		this.Init();
		if (this.logicObject == obj)
		{
			return;
		}
		if (this.Crusade != null)
		{
			this.Crusade.DelListener(this);
		}
		base.SetObject(obj, vars);
		if (obj is Crusade)
		{
			this.Crusade = (obj as Crusade);
		}
		this.vars = vars;
		if (vars == null)
		{
			vars = new Vars(this.Crusade);
			vars.Set<Logic.Character>("owner", this.Crusade.leader);
		}
		this.Crusade.AddListener(this);
		Tooltip.Get(base.gameObject, true).SetDef("CrusadeTooltip", vars);
		this.Refresh();
	}

	// Token: 0x06002284 RID: 8836 RVA: 0x00138E98 File Offset: 0x00137098
	private void Refresh()
	{
		this.m_CrusaderIcon.SetObject(this.Crusade.leader, null);
	}

	// Token: 0x06002285 RID: 8837 RVA: 0x00138EB4 File Offset: 0x001370B4
	public override void OnClick(PointerEventData e)
	{
		base.OnClick(e);
		if (e.button == PointerEventData.InputButton.Left)
		{
			if (e.clickCount == 1)
			{
				if (this.OnSelect != null)
				{
					this.OnSelect(this);
				}
				else
				{
					this.ExecuteDefaultSelectAction();
				}
			}
			if (e.clickCount > 1 && this.OnSelect != null)
			{
				this.OnSelect(this);
			}
		}
	}

	// Token: 0x06002286 RID: 8838 RVA: 0x000023FD File Offset: 0x000005FD
	private void ExecuteDefaultSelectAction()
	{
	}

	// Token: 0x06002287 RID: 8839 RVA: 0x00138F14 File Offset: 0x00137114
	private void HandleOnCharacterFocus(UICharacterIcon icon)
	{
		WorldUI worldUI = WorldUI.Get();
		if (worldUI != null)
		{
			Logic.Army army = icon.Data.GetArmy();
			if (army != null)
			{
				worldUI.LookAt(army.position, false);
				worldUI.SelectObjFromLogic(army, false, true);
			}
		}
	}

	// Token: 0x06002288 RID: 8840 RVA: 0x00138F5C File Offset: 0x0013715C
	private void HandleOnCharacterSelect(UICharacterIcon icon)
	{
		WorldUI worldUI = WorldUI.Get();
		if (worldUI != null)
		{
			Logic.Army army = icon.Data.GetArmy();
			if (army != null)
			{
				worldUI.SelectObjFromLogic(army, false, true);
			}
		}
	}

	// Token: 0x06002289 RID: 8841 RVA: 0x00138F90 File Offset: 0x00137190
	private void OnDestroy()
	{
		AspectRatioFitter component = base.GetComponent<AspectRatioFitter>();
		if (component != null)
		{
			UnityEngine.Object.DestroyImmediate(component);
		}
		Crusade crusade = this.Crusade;
		if (crusade != null)
		{
			crusade.DelListener(this);
		}
		this.Crusade = null;
		this.logicObject = null;
	}

	// Token: 0x0600228A RID: 8842 RVA: 0x00138FD3 File Offset: 0x001371D3
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "destroying" || message == "finishing")
		{
			(obj as Logic.Object).DelListener(this);
			return;
		}
	}

	// Token: 0x0600228B RID: 8843 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolSpawned()
	{
	}

	// Token: 0x0600228C RID: 8844 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolActivated()
	{
	}

	// Token: 0x0600228D RID: 8845 RVA: 0x00138FFC File Offset: 0x001371FC
	public void OnPoolDeactivated()
	{
		this.OnDestroy();
	}

	// Token: 0x0600228E RID: 8846 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolDestroyed()
	{
	}

	// Token: 0x0400171A RID: 5914
	[UIFieldTarget("id_CrusaderIcon")]
	private UICharacterIcon m_CrusaderIcon;

	// Token: 0x0400171B RID: 5915
	[UIFieldTarget("id_Banner")]
	private BSGButton m_Banner;

	// Token: 0x0400171F RID: 5919
	private bool m_Initialzied;
}
