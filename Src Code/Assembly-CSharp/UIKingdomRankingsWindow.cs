using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x02000226 RID: 550
public class UIKingdomRankingsWindow : MonoBehaviour, IListener
{
	// Token: 0x170001B0 RID: 432
	// (get) Token: 0x06002166 RID: 8550 RVA: 0x0012F09D File Offset: 0x0012D29D
	// (set) Token: 0x06002167 RID: 8551 RVA: 0x0012F0A5 File Offset: 0x0012D2A5
	public KingdomRanking Data { get; private set; }

	// Token: 0x06002168 RID: 8552 RVA: 0x0012F0B0 File Offset: 0x0012D2B0
	private void Init()
	{
		if (!this.m_initalzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_ButtonClose != null)
		{
			this.m_ButtonClose.onClick = new BSGButton.OnClick(this.HandleOnClose);
		}
		if (this.m_RowPrototype != null)
		{
			this.m_RowPrototype.gameObject.SetActive(false);
		}
		Game game = GameLogic.Get(false);
		if (game != null)
		{
			game.AddListener(this);
		}
		this.m_initalzied = true;
	}

	// Token: 0x06002169 RID: 8553 RVA: 0x0012F12A File Offset: 0x0012D32A
	public void SetObject(KingdomRanking ranking)
	{
		this.Init();
		this.Data = ranking;
		this.Populate();
		this.UpdateRankings();
	}

	// Token: 0x0600216A RID: 8554 RVA: 0x0012F148 File Offset: 0x0012D348
	private void Populate()
	{
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		Vars vars = new Vars(this.Data);
		vars.Set<Logic.Kingdom>("kingdom", kingdom);
		vars.Set<int>("fame", this.Data.GetFame(kingdom));
		vars.Set<int>("rank", this.Data.GetRank(kingdom));
		vars.Set<int>("ranks", this.Data.rows.Count);
		UIText.SetText(this.m_CaptionLabel, this.Data.def.field, "name", vars, null);
		UIText.SetText(this.m_Description, this.Data.def.field, "description", vars, null);
		UIText.SetTextKey(this.m_TableRowHeader, "id_place", "UIKingdomRankingsWindow.row_header.number", null, null);
		UIText.SetTextKey(this.m_TableRowHeader, "id_kingdom_name", "UIKingdomRankingsWindow.row_header.kingdom", null, null);
		UIText.SetTextKey(this.m_TableRowHeader, "id_score", "UIKingdomRankingsWindow.row_header.score", null, null);
		UIText.SetTextKey(this.m_TableRowHeader, "id_prestige", "UIKingdomRankingsWindow.row_header.fame", null, null);
		GameObject gameObject = this.m_Content.transform.GetChild(0).gameObject;
		int i;
		for (i = this.m_Content.transform.childCount; i < this.Data.rows.Count; i++)
		{
			global::Common.Spawn(gameObject, false, false).transform.SetParent(this.m_Content.transform, false);
		}
		while (i > this.Data.rows.Count)
		{
			i--;
			UnityEngine.Object.DestroyImmediate(this.m_Content.transform.GetChild(i).gameObject);
		}
	}

	// Token: 0x0600216B RID: 8555 RVA: 0x0012F2F4 File Offset: 0x0012D4F4
	private void UpdateRankings()
	{
		if (this.Data == null)
		{
			return;
		}
		if (this.m_Content == null)
		{
			return;
		}
		if (this.m_RowPrototype == null)
		{
			return;
		}
		while (this.m_Rows.Count < this.Data.rows.Count)
		{
			GameObject item = global::Common.Spawn(this.m_RowPrototype, this.m_Content.transform, false, "");
			this.m_Rows.Add(item);
		}
		for (int i = 0; i < this.m_Rows.Count; i++)
		{
			if (this.Data.rows.Count > i)
			{
				KingdomRanking.Row row = this.Data.rows[i];
				GameObject gameObject = this.m_Rows[i];
				UIText.SetText(gameObject, "id_place", row.rank.ToString());
				UIText.SetText(gameObject, "id_kingdom_name", global::Defs.Localize(row.kingdom.GetNameKey(null, ""), null, null, true, true));
				UIText.SetText(gameObject, "id_score", Mathf.Round(row.score).ToString());
				UIText.SetText(gameObject, "id_prestige", row.fame.ToString());
				gameObject.SetActive(true);
			}
			else
			{
				this.m_Rows[i].SetActive(false);
			}
		}
	}

	// Token: 0x0600216C RID: 8556 RVA: 0x0012F449 File Offset: 0x0012D649
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "ranking_updated")
		{
			this.UpdateRankings();
		}
	}

	// Token: 0x0600216D RID: 8557 RVA: 0x000C4358 File Offset: 0x000C2558
	private void HandleOnClose(BSGButton bnt)
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	// Token: 0x0600216E RID: 8558 RVA: 0x0012F45E File Offset: 0x0012D65E
	private void OnDestroy()
	{
		Game game = GameLogic.Get(false);
		if (game != null)
		{
			game.DelListener(this);
		}
		if (UIKingdomRankingsWindow.instance == this)
		{
			UIKingdomRankingsWindow.instance = null;
		}
	}

	// Token: 0x0600216F RID: 8559 RVA: 0x0012F488 File Offset: 0x0012D688
	public static UIKingdomRankingsWindow Create(KingdomRanking ranking, RectTransform parent = null)
	{
		if (UIKingdomRankingsWindow.instance != null)
		{
			UnityEngine.Object.DestroyImmediate(UIKingdomRankingsWindow.instance.gameObject);
			UIKingdomRankingsWindow.instance = null;
		}
		if (ranking == null)
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
		GameObject prefab = UICommon.GetPrefab("KingdomRankingsWindow", null);
		if (prefab == null)
		{
			return null;
		}
		UIKingdomRankingsWindow.instance = global::Common.Spawn(prefab, parent, false, "").GetOrAddComponent<UIKingdomRankingsWindow>();
		UIKingdomRankingsWindow.instance.SetObject(ranking);
		return UIKingdomRankingsWindow.instance;
	}

	// Token: 0x0400165C RID: 5724
	[UIFieldTarget("id_CaptionLabel")]
	private TextMeshProUGUI m_CaptionLabel;

	// Token: 0x0400165D RID: 5725
	[UIFieldTarget("id_Description")]
	private TextMeshProUGUI m_Description;

	// Token: 0x0400165E RID: 5726
	[UIFieldTarget("id_Content")]
	private GameObject m_Content;

	// Token: 0x0400165F RID: 5727
	[UIFieldTarget("id_TableRowHeader")]
	private GameObject m_TableRowHeader;

	// Token: 0x04001660 RID: 5728
	[UIFieldTarget("id_Close")]
	private BSGButton m_ButtonClose;

	// Token: 0x04001661 RID: 5729
	[UIFieldTarget("id_RowPrototype")]
	private GameObject m_RowPrototype;

	// Token: 0x04001663 RID: 5731
	private List<GameObject> m_Rows = new List<GameObject>();

	// Token: 0x04001664 RID: 5732
	private bool m_initalzied = true;

	// Token: 0x04001665 RID: 5733
	public static UIKingdomRankingsWindow instance;
}
