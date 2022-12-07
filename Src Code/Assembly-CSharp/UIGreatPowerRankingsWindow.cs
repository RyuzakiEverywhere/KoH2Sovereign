using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x02000221 RID: 545
public class UIGreatPowerRankingsWindow : MonoBehaviour
{
	// Token: 0x170001AA RID: 426
	// (get) Token: 0x060020FE RID: 8446 RVA: 0x0012C370 File Offset: 0x0012A570
	// (set) Token: 0x060020FF RID: 8447 RVA: 0x0012C378 File Offset: 0x0012A578
	public GreatPowers Data { get; private set; }

	// Token: 0x06002100 RID: 8448 RVA: 0x0012C381 File Offset: 0x0012A581
	public void SetObject(GreatPowers ranking)
	{
		UICommon.FindComponents(this, false);
		this.Data = ranking;
		this.Init();
	}

	// Token: 0x06002101 RID: 8449 RVA: 0x0012C398 File Offset: 0x0012A598
	private void Init()
	{
		if (this.m_ButtonClose != null)
		{
			this.m_ButtonClose.onClick = new BSGButton.OnClick(this.HandleOnClose);
		}
		if (this.Data == null || this.m_Content == null || this.m_Content.transform.childCount == 0)
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		Vars vars = new Vars(this.Data);
		vars.Set<Logic.Kingdom>("kingdom", kingdom);
		vars.Set<Value>("rank", kingdom.GetVar("great_power_ranking_position", null, true));
		vars.Set<Value>("ranks", kingdom.GetVar("great_power_rankings", null, true));
		UIText.SetText(this.m_Title, this.Data.fame_ranking.def.field, "name", vars, null);
		UIText.SetText(this.m_Description, this.Data.fame_ranking.def.field, "tooltip", vars, null);
		List<Logic.Kingdom> list = this.Data.TopKingdoms(false);
		GameObject gameObject = this.m_Content.transform.GetChild(0).gameObject;
		int i;
		for (i = this.m_Content.transform.childCount; i < list.Count; i++)
		{
			global::Common.Spawn(gameObject, false, false).transform.SetParent(this.m_Content.transform, false);
		}
		while (i > list.Count)
		{
			i--;
			UnityEngine.Object.DestroyImmediate(this.m_Content.transform.GetChild(i).gameObject);
		}
		for (int j = 0; j < list.Count; j++)
		{
			Logic.Kingdom kingdom2 = list[j];
			GameObject gameObject2 = this.m_Content.transform.GetChild(j).gameObject;
			UIText.SetText(gameObject2, "id_place", (j + 1).ToString());
			UIText.SetText(gameObject2, "id_kingdom_name", global::Defs.Localize(kingdom2.GetNameKey(null, ""), null, null, true, true));
		}
	}

	// Token: 0x06002102 RID: 8450 RVA: 0x000C4358 File Offset: 0x000C2558
	private void HandleOnClose(BSGButton bnt)
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	// Token: 0x06002103 RID: 8451 RVA: 0x0012C596 File Offset: 0x0012A796
	private void OnDestroy()
	{
		if (UIGreatPowerRankingsWindow.instance == this)
		{
			UIGreatPowerRankingsWindow.instance = null;
		}
	}

	// Token: 0x06002104 RID: 8452 RVA: 0x0012C5AC File Offset: 0x0012A7AC
	public static UIGreatPowerRankingsWindow Create(GreatPowers great_powers, RectTransform parent = null)
	{
		if (UIGreatPowerRankingsWindow.instance != null)
		{
			UnityEngine.Object.DestroyImmediate(UIGreatPowerRankingsWindow.instance.gameObject);
			UIGreatPowerRankingsWindow.instance = null;
		}
		if (great_powers == null)
		{
			return null;
		}
		if (parent == null)
		{
			WorldUI worldUI = WorldUI.Get();
			if (worldUI == null)
			{
				return null;
			}
			parent = global::Common.FindChildComponent<RectTransform>(worldUI.gameObject, "id_MessageContainer");
		}
		GameObject prefab = UICommon.GetPrefab("GreatPowerRankingsWindow", null);
		if (prefab == null)
		{
			return null;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab, parent);
		UIGreatPowerRankingsWindow.instance = gameObject.GetComponent<UIGreatPowerRankingsWindow>();
		if (UIGreatPowerRankingsWindow.instance == null)
		{
			UIGreatPowerRankingsWindow.instance = gameObject.AddComponent<UIGreatPowerRankingsWindow>();
		}
		UIGreatPowerRankingsWindow.instance.SetObject(great_powers);
		return UIGreatPowerRankingsWindow.instance;
	}

	// Token: 0x04001601 RID: 5633
	[UIFieldTarget("id_Title")]
	private TextMeshProUGUI m_Title;

	// Token: 0x04001602 RID: 5634
	[UIFieldTarget("id_Description")]
	private TextMeshProUGUI m_Description;

	// Token: 0x04001603 RID: 5635
	[UIFieldTarget("id_Content")]
	private GameObject m_Content;

	// Token: 0x04001604 RID: 5636
	[UIFieldTarget("id_Close")]
	private BSGButton m_ButtonClose;

	// Token: 0x04001606 RID: 5638
	public static UIGreatPowerRankingsWindow instance;
}
