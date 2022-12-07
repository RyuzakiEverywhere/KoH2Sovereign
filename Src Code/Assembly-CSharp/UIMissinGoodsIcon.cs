using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020002B7 RID: 695
public class UIMissinGoodsIcon : Hotspot, IVars, IListener
{
	// Token: 0x17000227 RID: 551
	// (get) Token: 0x06002B91 RID: 11153 RVA: 0x0016F8CE File Offset: 0x0016DACE
	// (set) Token: 0x06002B92 RID: 11154 RVA: 0x0016F8D6 File Offset: 0x0016DAD6
	public Logic.Kingdom Data { get; private set; }

	// Token: 0x06002B93 RID: 11155 RVA: 0x0016F8DF File Offset: 0x0016DADF
	private void Init()
	{
		if (this.m_Initialzed)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		Tooltip.Get(base.gameObject, true).SetDef("MissingGoodsTooltip", new Vars(this));
		this.m_Initialzed = true;
	}

	// Token: 0x06002B94 RID: 11156 RVA: 0x0016F914 File Offset: 0x0016DB14
	public bool HasMissingGoods()
	{
		return this.Data != null && this.Data.HasMissingGoods();
	}

	// Token: 0x06002B95 RID: 11157 RVA: 0x0016F92C File Offset: 0x0016DB2C
	public void SetKingdom(Logic.Kingdom kingdom, Vars vars = null)
	{
		if (kingdom == this.Data)
		{
			return;
		}
		this.Init();
		Logic.Kingdom data = this.Data;
		if (data != null)
		{
			data.DelListener(this);
		}
		this.Data = kingdom;
		Logic.Kingdom data2 = this.Data;
		if (data2 != null)
		{
			data2.AddListener(this);
		}
		this.Refresh();
	}

	// Token: 0x06002B96 RID: 11158 RVA: 0x0016F97C File Offset: 0x0016DB7C
	private void Refresh()
	{
		if (this.Data == null)
		{
			return;
		}
		List<string> missingGoods = this.Data.GetMissingGoods();
		int num = (missingGoods != null) ? missingGoods.Count : 0;
		if (this.m_Count != null)
		{
			this.m_Count.gameObject.SetActive(num > 0);
		}
		if (this.m_MissingGoodsValue != null && num > 0)
		{
			UIText.SetText(this.m_MissingGoodsValue, num.ToString());
		}
	}

	// Token: 0x06002B97 RID: 11159 RVA: 0x0016F9F1 File Offset: 0x0016DBF1
	private void Update()
	{
		if (this.m_Invalidate)
		{
			this.Refresh();
			this.m_Invalidate = false;
		}
	}

	// Token: 0x06002B98 RID: 11160 RVA: 0x000DB26E File Offset: 0x000D946E
	public void UpdateHighlight()
	{
		bool isPlaying = Application.isPlaying;
	}

	// Token: 0x06002B99 RID: 11161 RVA: 0x0016FA08 File Offset: 0x0016DC08
	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x06002B9A RID: 11162 RVA: 0x0016FA17 File Offset: 0x0016DC17
	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x06002B9B RID: 11163 RVA: 0x0016FA26 File Offset: 0x0016DC26
	public override void OnClick(PointerEventData e)
	{
		if (this.Data != null)
		{
			UIKingdomAdvantagesWindow.ToggleOpen(this.Data);
		}
		base.OnClick(e);
	}

	// Token: 0x06002B9C RID: 11164 RVA: 0x0016FA44 File Offset: 0x0016DC44
	public static UIMissinGoodsIcon Create(Logic.Kingdom kingdom, Vars vars, RectTransform parent)
	{
		string text = "window_prefab";
		if (vars != null)
		{
			string text2 = vars.Get<string>("variant", null);
			if (!string.IsNullOrEmpty(text2))
			{
				text = text + "." + text2;
			}
		}
		GameObject obj = global::Defs.GetObj<GameObject>("MissingGoodsIcon", text, null);
		if (obj == null)
		{
			return null;
		}
		UIMissinGoodsIcon orAddComponent = UnityEngine.Object.Instantiate<GameObject>(obj, Vector3.zero, Quaternion.identity, parent).GetOrAddComponent<UIMissinGoodsIcon>();
		if (orAddComponent != null)
		{
			orAddComponent.SetKingdom(kingdom, vars);
		}
		return orAddComponent;
	}

	// Token: 0x06002B9D RID: 11165 RVA: 0x0016FAC0 File Offset: 0x0016DCC0
	private string GetMissingGoodsLocalziedList()
	{
		if (this.Data == null)
		{
			return null;
		}
		List<string> missingGoods = this.Data.GetMissingGoods();
		if (missingGoods == null || missingGoods.Count == 0)
		{
			return null;
		}
		string text = "@";
		for (int i = 0; i < missingGoods.Count; i++)
		{
			string str = missingGoods[i];
			if (i > 0)
			{
				if (i == missingGoods.Count - 1)
				{
					text += "{list_final_separator}";
				}
				else
				{
					text += "{list_separator}";
				}
			}
			text = text + "{" + str + "}";
		}
		return text;
	}

	// Token: 0x06002B9E RID: 11166 RVA: 0x0016FB4B File Offset: 0x0016DD4B
	public override Value GetVar(string key, IVars vars = null, bool as_value = true)
	{
		if (key == "obj")
		{
			return this.Data;
		}
		if (!(key == "missing_goods_list"))
		{
			return base.GetVar(key, vars, as_value);
		}
		return this.GetMissingGoodsLocalziedList();
	}

	// Token: 0x06002B9F RID: 11167 RVA: 0x0016FB8A File Offset: 0x0016DD8A
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "buildings_missing_resources_change")
		{
			this.m_Invalidate = true;
		}
	}

	// Token: 0x06002BA0 RID: 11168 RVA: 0x0016FBA0 File Offset: 0x0016DDA0
	private void OnDestroy()
	{
		Logic.Kingdom data = this.Data;
		if (data == null)
		{
			return;
		}
		data.DelListener(this);
	}

	// Token: 0x04001DB0 RID: 7600
	[UIFieldTarget("id_Icon")]
	private Image m_Icon;

	// Token: 0x04001DB1 RID: 7601
	[UIFieldTarget("id_Count")]
	private GameObject m_Count;

	// Token: 0x04001DB2 RID: 7602
	[UIFieldTarget("id_MissingGoodsValue")]
	private TextMeshProUGUI m_MissingGoodsValue;

	// Token: 0x04001DB4 RID: 7604
	private bool m_Invalidate;

	// Token: 0x04001DB5 RID: 7605
	private bool m_Initialzed;
}
