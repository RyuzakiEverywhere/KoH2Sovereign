using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000271 RID: 625
public class BSG_TMP_DropDown : TMP_Dropdown
{
	// Token: 0x06002641 RID: 9793 RVA: 0x00150427 File Offset: 0x0014E627
	protected override void Awake()
	{
		this._dropdown = base.GetComponent<Dropdown>();
		base.Awake();
	}

	// Token: 0x06002642 RID: 9794 RVA: 0x0015043C File Offset: 0x0014E63C
	public override void OnPointerClick(PointerEventData eventData)
	{
		base.OnPointerClick(eventData);
		if (this.indexesToHide.Count == 0)
		{
			return;
		}
		Canvas componentInChildren = base.GetComponentInChildren<Canvas>();
		if (!componentInChildren)
		{
			return;
		}
		Toggle[] componentsInChildren = componentInChildren.GetComponentsInChildren<Toggle>(true);
		for (int i = 1; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].gameObject.SetActive(!this.indexesToHide.Contains(i - 1));
		}
	}

	// Token: 0x06002643 RID: 9795 RVA: 0x001504A4 File Offset: 0x0014E6A4
	public void HideOption(int index, bool hide)
	{
		if (hide)
		{
			if (!this.indexesToHide.Contains(index))
			{
				this.indexesToHide.Add(index);
			}
		}
		else if (this.indexesToHide.Contains(index))
		{
			this.indexesToHide.Remove(index);
		}
		Canvas componentInChildren = base.GetComponentInChildren<Canvas>();
		if (!componentInChildren)
		{
			return;
		}
		componentInChildren.GetComponentsInChildren<Toggle>(true)[index].gameObject.SetActive(!hide);
	}

	// Token: 0x06002644 RID: 9796 RVA: 0x00150514 File Offset: 0x0014E714
	public void EnableOption(string label, bool enable)
	{
		int num = this._dropdown.options.FindIndex((Dropdown.OptionData o) => string.Equals(o.text, label));
		this.HideOption(num + 1, enable);
	}

	// Token: 0x040019DA RID: 6618
	[Tooltip("Indexes that should be ignored. Indexes are 0 based.")]
	public List<int> indexesToHide = new List<int>();

	// Token: 0x040019DB RID: 6619
	private Dropdown _dropdown;
}
