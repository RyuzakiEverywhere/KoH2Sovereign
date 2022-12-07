using System;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x0200030A RID: 778
public class UIStackIcon : Hotspot, IListener, IPoolable
{
	// Token: 0x1400003E RID: 62
	// (add) Token: 0x0600307A RID: 12410 RVA: 0x00188EAC File Offset: 0x001870AC
	// (remove) Token: 0x0600307B RID: 12411 RVA: 0x00188EE4 File Offset: 0x001870E4
	public event Action<UIStackIcon, PointerEventData> OnSelect;

	// Token: 0x0600307C RID: 12412 RVA: 0x00188F19 File Offset: 0x00187119
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.tooltip = Tooltip.Get(base.gameObject, true);
		this.m_Initialzied = true;
	}

	// Token: 0x0600307D RID: 12413 RVA: 0x00188F44 File Offset: 0x00187144
	public void SetData(Def def, Vars vars = null)
	{
		this.Init();
		this.def = def;
		this.vars = vars;
		if (vars != null)
		{
			this.tooltip.SetDef(vars.Get<string>("tooltip", null), vars);
			this.m_StackCountCallback = vars.Get<Func<int>>("stack_value", null);
		}
	}

	// Token: 0x0600307E RID: 12414 RVA: 0x00188F94 File Offset: 0x00187194
	private void LateUpdate()
	{
		if (this.m_Count != null && this.m_StackCountCallback != null)
		{
			UIText.SetText(this.m_Count, this.m_StackCountCallback().ToString());
		}
	}

	// Token: 0x0600307F RID: 12415 RVA: 0x00188FD5 File Offset: 0x001871D5
	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x06003080 RID: 12416 RVA: 0x00188FE4 File Offset: 0x001871E4
	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x06003081 RID: 12417 RVA: 0x00188FF3 File Offset: 0x001871F3
	public override void OnClick(PointerEventData e)
	{
		base.OnClick(e);
		if (this.OnSelect != null)
		{
			this.OnSelect(this, e);
		}
	}

	// Token: 0x06003082 RID: 12418 RVA: 0x000DB26E File Offset: 0x000D946E
	public void UpdateHighlight()
	{
		bool isPlaying = Application.isPlaying;
	}

	// Token: 0x06003083 RID: 12419 RVA: 0x00189011 File Offset: 0x00187211
	public void OnMessage(object obj, string message, object param)
	{
		if (!(message == "destroying") && !(message == "finishing"))
		{
			return;
		}
		Logic.Object @object = obj as Logic.Object;
		if (@object == null)
		{
			return;
		}
		@object.DelListener(this);
	}

	// Token: 0x06003084 RID: 12420 RVA: 0x0018903F File Offset: 0x0018723F
	private void OnDestroy()
	{
		this.OnSelect = null;
		this.def = null;
		this.vars = null;
	}

	// Token: 0x06003085 RID: 12421 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolSpawned()
	{
	}

	// Token: 0x06003086 RID: 12422 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolActivated()
	{
	}

	// Token: 0x06003087 RID: 12423 RVA: 0x00189056 File Offset: 0x00187256
	public void OnPoolDeactivated()
	{
		this.OnDestroy();
	}

	// Token: 0x06003088 RID: 12424 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolDestroyed()
	{
	}

	// Token: 0x04002088 RID: 8328
	[UIFieldTarget("id_Count")]
	private TextMeshProUGUI m_Count;

	// Token: 0x04002089 RID: 8329
	public Def def;

	// Token: 0x0400208A RID: 8330
	public Vars vars;

	// Token: 0x0400208C RID: 8332
	private bool m_Initialzied;

	// Token: 0x0400208D RID: 8333
	private Func<int> m_StackCountCallback;

	// Token: 0x0400208E RID: 8334
	private Tooltip tooltip;
}
