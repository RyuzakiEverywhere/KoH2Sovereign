using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Logic;
using Logic.ExtensionMethods;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000220 RID: 544
public class UIWarsOverviewWindow : UIWindow, IListener
{
	// Token: 0x060020E3 RID: 8419 RVA: 0x0012B9CD File Offset: 0x00129BCD
	public override string GetDefId()
	{
		return UIWarsOverviewWindow.def_id;
	}

	// Token: 0x170001A9 RID: 425
	// (get) Token: 0x060020E4 RID: 8420 RVA: 0x0012B9D4 File Offset: 0x00129BD4
	// (set) Token: 0x060020E5 RID: 8421 RVA: 0x0012B9DC File Offset: 0x00129BDC
	public Logic.Kingdom Data { get; private set; }

	// Token: 0x060020E6 RID: 8422 RVA: 0x0012B9E8 File Offset: 0x00129BE8
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.Button_Close != null)
		{
			for (int i = 0; i < this.Button_Close.Length; i++)
			{
				this.Button_Close[i].onClick = new BSGButton.OnClick(this.HandleOnClose);
			}
		}
		GameObject gameObject = global::Common.FindChildByName(base.gameObject, "id_Overview", true, true);
		this.m_Overview = ((gameObject != null) ? gameObject.AddComponent<UIWarsOverviewWindow.UIOverview>() : null);
		GameObject gameObject2 = global::Common.FindChildByName(base.gameObject, "id_WarInfo", true, true);
		this.m_WarInfo = ((gameObject2 != null) ? gameObject2.AddComponent<UIWarsOverviewWindow.UIWarInfo>() : null);
		if (this.m_WarRowPrototype != null)
		{
			this.m_WarRowPrototype.SetActive(false);
		}
		if (this.m_WarRowEmptyPrototype != null)
		{
			this.m_WarRowEmptyPrototype.SetActive(false);
		}
		this.m_Initialized = true;
	}

	// Token: 0x060020E7 RID: 8423 RVA: 0x0012BABC File Offset: 0x00129CBC
	public void SetData(Logic.Kingdom k)
	{
		this.Init();
		Logic.Kingdom data = this.Data;
		if (data != null)
		{
			data.DelListener(this);
		}
		this.Data = k;
		Logic.Kingdom data2 = this.Data;
		if (data2 != null)
		{
			data2.AddListener(this);
		}
		if (this.m_Overview != null)
		{
			this.m_Overview.SetData(this.Data);
		}
		this.PopulateStatics();
		this.Refresh();
		if (this.m_WarExhaustion != null)
		{
			Tooltip.Get(this.m_WarExhaustion, true).SetDef("WarExhaustionTooltip", null);
		}
		this.SelectWar(null);
	}

	// Token: 0x060020E8 RID: 8424 RVA: 0x0012BB51 File Offset: 0x00129D51
	protected override void Update()
	{
		base.Update();
		this.UpdateWarExhaustion();
		if (this.m_Invalidate)
		{
			this.Refresh();
			this.m_Invalidate = false;
		}
	}

	// Token: 0x060020E9 RID: 8425 RVA: 0x0011796E File Offset: 0x00115B6E
	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	// Token: 0x060020EA RID: 8426 RVA: 0x0012BB74 File Offset: 0x00129D74
	private void PopulateStatics()
	{
		if (this.Data == null)
		{
			return;
		}
		if (this.m_Caption != null)
		{
			UIText.SetTextKey(this.m_Caption, "WarsOverviewWindow.caption", new Vars(this.Data), null);
		}
		if (this.m_CurrentWarsLabels != null)
		{
			UIText.SetTextKey(this.m_CurrentWarsLabels, "WarsOverviewWindow.wars", new Vars(this.Data), null);
		}
		if (this.m_Crest != null)
		{
			this.m_Crest.SetObject(this.Data, null);
		}
		if (this.m_NoWarsLabel != null)
		{
			UIText.SetTextKey(this.m_NoWarsLabel, "WarOverview.War.noWars", new Vars(this.Data), null);
		}
	}

	// Token: 0x060020EB RID: 8427 RVA: 0x0012BC38 File Offset: 0x00129E38
	private void Refresh()
	{
		this.UpdateWarExhaustion();
		if (this.m_NoWarsLabel != null)
		{
			bool flag = this.Data == null || this.Data.wars == null || this.Data.wars.Count == 0;
			this.m_NoWarsLabel.gameObject.SetActive(flag && this.m_ShowNoWarsLabel);
		}
		this.PopulateWars();
		if (this.m_SelectedWar != null && !this.IsValidWar(this.m_SelectedWar))
		{
			this.SelectWar(null);
		}
	}

	// Token: 0x060020EC RID: 8428 RVA: 0x0012BCC4 File Offset: 0x00129EC4
	private void PopulateWars()
	{
		if (this.m_WarRowPrototype == null)
		{
			return;
		}
		if (this.m_WarsContainer == null)
		{
			return;
		}
		for (int i = 0; i < this.m_Wars.Count; i++)
		{
			global::Common.DestroyObj(this.m_Wars[i].gameObject);
		}
		this.m_Wars.Clear();
		for (int j = 0; j < this.m_EmptyWars.Count; j++)
		{
			global::Common.DestroyObj(this.m_EmptyWars[j].gameObject);
		}
		this.m_EmptyWars.Clear();
		Logic.Kingdom data = this.Data;
		int? num;
		if (data == null)
		{
			num = null;
		}
		else
		{
			List<War> wars = data.wars;
			num = ((wars != null) ? new int?(wars.Count) : null);
		}
		int num2 = num ?? 0;
		for (int k = 0; k < num2; k++)
		{
			War war = this.Data.wars[k];
			if (!war.IsConcluded() && war.IsValid())
			{
				Logic.Kingdom attacker = war.attacker;
				if (attacker != null && !attacker.IsDefeated())
				{
					Logic.Kingdom defender = war.defender;
					if (defender != null && !defender.IsDefeated())
					{
						UIWarsOverviewWindow.UIWarRow uiwarRow = UnityEngine.Object.Instantiate<GameObject>(this.m_WarRowPrototype, this.m_WarsContainer.transform).AddComponent<UIWarsOverviewWindow.UIWarRow>();
						uiwarRow.gameObject.SetActive(true);
						uiwarRow.SetData(this.Data, war);
						UIWarsOverviewWindow.UIWarRow uiwarRow2 = uiwarRow;
						uiwarRow2.OnSelect = (Action<UIWarsOverviewWindow.UIWarRow>)Delegate.Combine(uiwarRow2.OnSelect, new Action<UIWarsOverviewWindow.UIWarRow>(this.HandleOnRowSelect));
						this.m_Wars.Add(uiwarRow);
					}
				}
			}
		}
		int num3 = 4;
		if (num2 < num3)
		{
			int num4 = num3 - num2;
			for (int l = 0; l < num4; l++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_WarRowEmptyPrototype, this.m_WarsContainer.transform);
				gameObject.SetActive(true);
				this.m_EmptyWars.Add(gameObject);
			}
		}
	}

	// Token: 0x060020ED RID: 8429 RVA: 0x0012BEC6 File Offset: 0x0012A0C6
	private void HandleOnRowSelect(UIWarsOverviewWindow.UIWarRow r)
	{
		this.SelectWar(r.War);
	}

	// Token: 0x060020EE RID: 8430 RVA: 0x000023FD File Offset: 0x000005FD
	public void SelectWar(War sel)
	{
	}

	// Token: 0x060020EF RID: 8431 RVA: 0x0012BED4 File Offset: 0x0012A0D4
	public void SelectPact(Pact sel)
	{
		if (this.m_Overview != null)
		{
			this.m_Overview.Select(sel);
		}
	}

	// Token: 0x060020F0 RID: 8432 RVA: 0x0012BEF0 File Offset: 0x0012A0F0
	private bool IsValidWar(War war)
	{
		return war != null && war.IsValid() && war.attacker != null && war.attacker.IsValid() && !war.attacker.IsDefeated() && war.defender != null && war.defender.IsValid() && !war.defender.IsDefeated();
	}

	// Token: 0x060020F1 RID: 8433 RVA: 0x0012BF58 File Offset: 0x0012A158
	private void UpdateWarExhaustion()
	{
		int ex = (int)this.Data.GetStat(Stats.ks_war_exhaustion, false);
		this.warExhaustionVars.Clear();
		this.warExhaustionVars.Set<string>("level", "#" + this.GetExhaustionString(ex));
		this.warExhaustionVars.Set<string>("color", "#<color=" + this.GetExhaustionColor(ex) + ">");
		UIText.SetText(this.m_WarExhaustionLabel, global::Defs.Localize("WarOverview.warExhaustion", this.warExhaustionVars, null, true, true));
		if (this.m_WarExhaustionValue != null)
		{
			this.m_WarExhaustionValue.text = ex.ToString();
		}
	}

	// Token: 0x060020F2 RID: 8434 RVA: 0x0012C008 File Offset: 0x0012A208
	private string GetExhaustionColor(int ex)
	{
		if (ex <= 50)
		{
			return "#81912B";
		}
		if (ex > 500)
		{
			return "#93270E";
		}
		if (ex > 200)
		{
			return "#A68728";
		}
		return "#81912B";
	}

	// Token: 0x060020F3 RID: 8435 RVA: 0x0012C038 File Offset: 0x0012A238
	private void ExtractExhaustionColor()
	{
		DT.Field defField = global::Defs.GetDefField("War", null);
		if (defField == null)
		{
			return;
		}
		DT.Field field = defField.FindChild("warscore_exsoution_tresholds", null, true, true, true, '.');
		defField.FindChild("warscore_exsoution_tresholds_color", null, true, true, true, '.');
	}

	// Token: 0x060020F4 RID: 8436 RVA: 0x0003B36E File Offset: 0x0003956E
	private string GetExhaustionString(int ex)
	{
		return "";
	}

	// Token: 0x060020F5 RID: 8437 RVA: 0x0012C07C File Offset: 0x0012A27C
	public void OnMessage(object obj, string message, object param)
	{
		if (!(this == null) && !(base.gameObject == null))
		{
			if (message == "war_started" || message == "war_ended" || message == "left_war" || message == "joined_war")
			{
				this.m_Invalidate = true;
			}
			return;
		}
		Logic.Kingdom data = this.Data;
		if (data == null)
		{
			return;
		}
		data.DelListener(this);
	}

	// Token: 0x060020F6 RID: 8438 RVA: 0x0011FFF8 File Offset: 0x0011E1F8
	private void HandleOnClose(BSGButton button)
	{
		this.Close(false);
	}

	// Token: 0x060020F7 RID: 8439 RVA: 0x0012C0ED File Offset: 0x0012A2ED
	public static GameObject GetPrefab()
	{
		return UICommon.GetPrefab(UIWarsOverviewWindow.def_id, null);
	}

	// Token: 0x060020F8 RID: 8440 RVA: 0x0012C0FC File Offset: 0x0012A2FC
	public static void ToggleOpen(Logic.Kingdom k, War w = null, Pact pact = null)
	{
		if (k == null)
		{
			if (UIWarsOverviewWindow.current != null)
			{
				UIWarsOverviewWindow.current.Close(false);
			}
			return;
		}
		if (UIWarsOverviewWindow.current != null)
		{
			UIWarsOverviewWindow uiwarsOverviewWindow = UIWarsOverviewWindow.current;
			Logic.Kingdom kingdom;
			if (uiwarsOverviewWindow == null)
			{
				kingdom = null;
			}
			else
			{
				Logic.Kingdom data = uiwarsOverviewWindow.Data;
				kingdom = ((data != null) ? data.GetKingdom() : null);
			}
			if (kingdom == k)
			{
				UIWarsOverviewWindow.current.Close(false);
				return;
			}
			UIWarsOverviewWindow.ToggleOpen(null, null, null);
			if (w != null)
			{
				UIWarsOverviewWindow.current.SelectWar(w);
			}
			if (pact != null)
			{
				UIWarsOverviewWindow.current.SelectPact(pact);
			}
			return;
		}
		else
		{
			WorldUI worldUI = WorldUI.Get();
			if (worldUI == null)
			{
				return;
			}
			GameObject prefab = UIWarsOverviewWindow.GetPrefab();
			if (prefab == null)
			{
				return;
			}
			GameObject gameObject = global::Common.FindChildByName(worldUI.gameObject, "id_MessageContainer", true, true);
			if (gameObject != null)
			{
				UICommon.DeleteChildren(gameObject.transform, typeof(UIWarsOverviewWindow));
				UIWarsOverviewWindow.current = UIWarsOverviewWindow.Create(k, prefab, gameObject.transform as RectTransform);
				if (w != null)
				{
					UIWarsOverviewWindow.current.SelectWar(w);
				}
				if (pact != null)
				{
					UIWarsOverviewWindow.current.SelectPact(pact);
				}
			}
			return;
		}
	}

	// Token: 0x060020F9 RID: 8441 RVA: 0x0012C208 File Offset: 0x0012A408
	public static bool IsActive()
	{
		return UIWarsOverviewWindow.current != null;
	}

	// Token: 0x060020FA RID: 8442 RVA: 0x0012C218 File Offset: 0x0012A418
	public static UIWarsOverviewWindow Create(Logic.Kingdom k, GameObject prototype, RectTransform parent)
	{
		if (prototype == null)
		{
			return null;
		}
		if (k == null)
		{
			return null;
		}
		if (parent == null)
		{
			return null;
		}
		UIWarsOverviewWindow orAddComponent = global::Common.Spawn(prototype, parent, false, "").GetOrAddComponent<UIWarsOverviewWindow>();
		orAddComponent.on_close = (UIWindow.OnClose)Delegate.Combine(orAddComponent.on_close, new UIWindow.OnClose(delegate(UIWindow _)
		{
			UIWarsOverviewWindow.current = null;
		}));
		orAddComponent.SetData(k);
		orAddComponent.Open();
		return orAddComponent;
	}

	// Token: 0x060020FB RID: 8443 RVA: 0x0012C294 File Offset: 0x0012A494
	public static string GetWarScore(War w, Logic.Kingdom side, Vars vars)
	{
		if (w == null)
		{
			return "";
		}
		float sideScore = w.GetSideScore(side);
		float sideScore2 = w.GetSideScore((w.attacker == side) ? w.defender : w.attacker);
		float val = w.GetSideScore(side) - 1000f;
		float num = sideScore - sideScore2;
		string text_key;
		if (Mathf.Abs(num) < UIWarsOverviewWindow.baseLine)
		{
			text_key = "WarOverview.War.balanced";
		}
		else if (num > 0f)
		{
			text_key = "WarOverview.War.winning";
		}
		else
		{
			text_key = "WarOverview.War.loosing";
		}
		vars.Set<float>("score", val);
		return "#" + global::Defs.Localize(text_key, vars, null, true, true);
	}

	// Token: 0x040015E8 RID: 5608
	private static string def_id = "WarsOverviewWindow";

	// Token: 0x040015E9 RID: 5609
	[UIFieldTarget("id_Button_Close", true)]
	private BSGButton[] Button_Close;

	// Token: 0x040015EA RID: 5610
	[UIFieldTarget("id_Crest")]
	private UIKingdomIcon m_Crest;

	// Token: 0x040015EB RID: 5611
	[UIFieldTarget("id_Caption")]
	private TextMeshProUGUI m_Caption;

	// Token: 0x040015EC RID: 5612
	[UIFieldTarget("id_CurrentWarsLabels")]
	private TextMeshProUGUI m_CurrentWarsLabels;

	// Token: 0x040015ED RID: 5613
	[UIFieldTarget("id_WarExhaustion")]
	private GameObject m_WarExhaustion;

	// Token: 0x040015EE RID: 5614
	[UIFieldTarget("id_WarExhaustionValue")]
	private TextMeshProUGUI m_WarExhaustionValue;

	// Token: 0x040015EF RID: 5615
	[UIFieldTarget("id_WarExhaustionLabel")]
	private TextMeshProUGUI m_WarExhaustionLabel;

	// Token: 0x040015F0 RID: 5616
	[UIFieldTarget("id_NoWarsLabel")]
	private TextMeshProUGUI m_NoWarsLabel;

	// Token: 0x040015F1 RID: 5617
	[UIFieldTarget("id_WarsContainer")]
	private GameObject m_WarsContainer;

	// Token: 0x040015F2 RID: 5618
	[UIFieldTarget("id_WarRowPrototype")]
	private GameObject m_WarRowPrototype;

	// Token: 0x040015F3 RID: 5619
	[UIFieldTarget("id_WarRowEmptyPrototype")]
	private GameObject m_WarRowEmptyPrototype;

	// Token: 0x040015F4 RID: 5620
	private List<UIWarsOverviewWindow.UIWarRow> m_Wars = new List<UIWarsOverviewWindow.UIWarRow>();

	// Token: 0x040015F5 RID: 5621
	private List<GameObject> m_EmptyWars = new List<GameObject>();

	// Token: 0x040015F6 RID: 5622
	private UIWarsOverviewWindow.UIOverview m_Overview;

	// Token: 0x040015F7 RID: 5623
	private UIWarsOverviewWindow.UIWarInfo m_WarInfo;

	// Token: 0x040015F9 RID: 5625
	private War m_SelectedWar;

	// Token: 0x040015FA RID: 5626
	private bool m_ShowNoWarsLabel;

	// Token: 0x040015FB RID: 5627
	private bool m_Invalidate;

	// Token: 0x040015FC RID: 5628
	private Vars warExhaustionVars = new Vars();

	// Token: 0x040015FD RID: 5629
	private int[] m_ExsoutionTresholds;

	// Token: 0x040015FE RID: 5630
	private int[] m_ExsoutionColors;

	// Token: 0x040015FF RID: 5631
	private static UIWarsOverviewWindow current;

	// Token: 0x04001600 RID: 5632
	private static float baseLine = 200f;

	// Token: 0x02000769 RID: 1897
	protected class UIWarScoreBar : MonoBehaviour
	{
		// Token: 0x170005D2 RID: 1490
		// (get) Token: 0x06004B93 RID: 19347 RVA: 0x00225A89 File Offset: 0x00223C89
		// (set) Token: 0x06004B94 RID: 19348 RVA: 0x00225A91 File Offset: 0x00223C91
		public War War { get; private set; }

		// Token: 0x170005D3 RID: 1491
		// (get) Token: 0x06004B95 RID: 19349 RVA: 0x00225A9A File Offset: 0x00223C9A
		// (set) Token: 0x06004B96 RID: 19350 RVA: 0x00225AA2 File Offset: 0x00223CA2
		public Logic.Kingdom Owner { get; private set; }

		// Token: 0x06004B97 RID: 19351 RVA: 0x00225AAB File Offset: 0x00223CAB
		private void Init()
		{
			if (this.m_Initiazled)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			this.m_Initiazled = true;
		}

		// Token: 0x06004B98 RID: 19352 RVA: 0x00225AC4 File Offset: 0x00223CC4
		public void SetData(Logic.Kingdom owner, War war)
		{
			this.Init();
			this.Owner = owner;
			this.War = war;
			this.vars.obj = this.War;
			Tooltip.Get(base.gameObject, true).SetText("War.score_tooltip", null, this.vars);
			this.Refresh();
		}

		// Token: 0x06004B99 RID: 19353 RVA: 0x00225B1E File Offset: 0x00223D1E
		private void Update()
		{
			this.Refresh();
		}

		// Token: 0x06004B9A RID: 19354 RVA: 0x00225B28 File Offset: 0x00223D28
		private void Refresh()
		{
			if (this.War == null)
			{
				return;
			}
			if (this.Owner == null)
			{
				return;
			}
			int side = this.War.GetSide(this.Owner);
			int side2 = this.War.EnemySide(side);
			int score_ratio_norm = this.War.def.score_ratio_norm;
			int num = (int)this.War.GetSideScore(side);
			int num2 = (int)this.War.GetSideScore(side2);
			float num3 = (float)(2 * score_ratio_norm + num + num2);
			if (num3 == 0f)
			{
				this.m_WarScore.fillAmount = 0.5f;
			}
			else
			{
				this.m_WarScore.fillAmount = (float)(score_ratio_norm + num) / num3;
			}
			UIText.SetText(this.m_WarScoreLabel1, num.ToString());
			UIText.SetText(this.m_WarScoreLabel2, num2.ToString());
		}

		// Token: 0x04003A35 RID: 14901
		[UIFieldTarget("id_WarScoreLabel1")]
		private TextMeshProUGUI m_WarScoreLabel1;

		// Token: 0x04003A36 RID: 14902
		[UIFieldTarget("id_WarScoreLabel2")]
		private TextMeshProUGUI m_WarScoreLabel2;

		// Token: 0x04003A37 RID: 14903
		[UIFieldTarget("id_WinBar")]
		private Image m_WarScore;

		// Token: 0x04003A3A RID: 14906
		private bool m_Initiazled;

		// Token: 0x04003A3B RID: 14907
		private Vars vars = new Vars();
	}

	// Token: 0x0200076A RID: 1898
	protected class UIStrengthBar : MonoBehaviour
	{
		// Token: 0x170005D4 RID: 1492
		// (get) Token: 0x06004B9C RID: 19356 RVA: 0x00225C04 File Offset: 0x00223E04
		// (set) Token: 0x06004B9D RID: 19357 RVA: 0x00225C0C File Offset: 0x00223E0C
		public War War { get; private set; }

		// Token: 0x170005D5 RID: 1493
		// (get) Token: 0x06004B9E RID: 19358 RVA: 0x00225C15 File Offset: 0x00223E15
		// (set) Token: 0x06004B9F RID: 19359 RVA: 0x00225C1D File Offset: 0x00223E1D
		public Logic.Kingdom Owner { get; private set; }

		// Token: 0x06004BA0 RID: 19360 RVA: 0x00225C26 File Offset: 0x00223E26
		private void Init()
		{
			if (this.m_Initiazled)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			this.m_Initiazled = true;
		}

		// Token: 0x06004BA1 RID: 19361 RVA: 0x00225C40 File Offset: 0x00223E40
		public void SetData(Logic.Kingdom owner, War war)
		{
			this.Init();
			this.Owner = owner;
			this.War = war;
			this.vars.obj = this.War;
			Tooltip.Get(base.gameObject, true).SetText("War.strength_tooltip", null, this.vars);
			this.Refresh();
		}

		// Token: 0x06004BA2 RID: 19362 RVA: 0x00225C9A File Offset: 0x00223E9A
		private void Update()
		{
			this.Refresh();
		}

		// Token: 0x06004BA3 RID: 19363 RVA: 0x00225CA4 File Offset: 0x00223EA4
		private void Refresh()
		{
			if (this.War == null)
			{
				return;
			}
			if (this.Owner == null)
			{
				return;
			}
			int side = this.War.GetSide(this.Owner);
			int side2 = this.War.EnemySide(side);
			int score_ratio_norm = this.War.def.score_ratio_norm;
			int num = (int)this.War.CalcArmyStrength(side);
			int num2 = (int)this.War.CalcArmyStrength(side2);
			float num3 = (float)(2 * score_ratio_norm + num + num2);
			if (num3 == 0f)
			{
				this.m_WarScore.fillAmount = 0.5f;
				return;
			}
			this.m_WarScore.fillAmount = (float)(score_ratio_norm + num) / num3;
		}

		// Token: 0x04003A3C RID: 14908
		[UIFieldTarget("id_WinBar")]
		private Image m_WarScore;

		// Token: 0x04003A3F RID: 14911
		private bool m_Initiazled;

		// Token: 0x04003A40 RID: 14912
		private Vars vars = new Vars();
	}

	// Token: 0x0200076B RID: 1899
	public class UIWarRow : MonoBehaviour, IListener
	{
		// Token: 0x170005D6 RID: 1494
		// (get) Token: 0x06004BA5 RID: 19365 RVA: 0x00225D5B File Offset: 0x00223F5B
		// (set) Token: 0x06004BA6 RID: 19366 RVA: 0x00225D63 File Offset: 0x00223F63
		public War War { get; private set; }

		// Token: 0x170005D7 RID: 1495
		// (get) Token: 0x06004BA7 RID: 19367 RVA: 0x00225D6C File Offset: 0x00223F6C
		// (set) Token: 0x06004BA8 RID: 19368 RVA: 0x00225D74 File Offset: 0x00223F74
		public Logic.Kingdom Owner { get; private set; }

		// Token: 0x06004BA9 RID: 19369 RVA: 0x00225D80 File Offset: 0x00223F80
		private void Init()
		{
			if (this.m_Initiazled)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			BSGButton bsgbutton = null;
			if (this.m_MoveContainer != null)
			{
				bsgbutton = this.m_MoveContainer.GetComponent<BSGButton>();
			}
			if (bsgbutton == null)
			{
				bsgbutton = this.GetOrAddComponent<BSGButton>();
			}
			if (this.m_WarScore != null)
			{
				this.m_WarScoreProgress = this.m_WarScore.AddComponent<UIWarsOverviewWindow.UIWarScoreBar>();
			}
			if (this.m_StrengthBar != null)
			{
				this.m_StrengthProgress = this.m_StrengthBar.AddComponent<UIWarsOverviewWindow.UIStrengthBar>();
			}
			bsgbutton.onClick = new BSGButton.OnClick(this.OnClick);
			this.m_Initiazled = true;
		}

		// Token: 0x06004BAA RID: 19370 RVA: 0x00225E20 File Offset: 0x00224020
		public void SetData(Logic.Kingdom owner, War war)
		{
			this.Init();
			War war2 = this.War;
			if (war2 != null)
			{
				war2.DelListener(this);
			}
			this.Owner = owner;
			this.War = war;
			War war3 = this.War;
			if (war3 != null)
			{
				war3.AddListener(this);
			}
			UIWarsOverviewWindow.UIWarScoreBar warScoreProgress = this.m_WarScoreProgress;
			if (warScoreProgress != null)
			{
				warScoreProgress.SetData(owner, war);
			}
			UIWarsOverviewWindow.UIStrengthBar strengthProgress = this.m_StrengthProgress;
			if (strengthProgress != null)
			{
				strengthProgress.SetData(owner, war);
			}
			this.UpdateStatics();
			this.UpdateDynamics();
			this.Select(false);
		}

		// Token: 0x06004BAB RID: 19371 RVA: 0x00225EA0 File Offset: 0x002240A0
		public void Select(bool selected)
		{
			this.m_Selected = selected;
			if (this.m_MoveContainer != null)
			{
				this.m_MoveContainer.anchoredPosition = (this.m_Selected ? new Vector2(15f, 0f) : new Vector2(0f, 0f));
			}
			if (this.m_SelectedArrow != null)
			{
				this.m_SelectedArrow.gameObject.SetActive(this.m_Selected);
			}
			this.UpdateHighlight();
		}

		// Token: 0x06004BAC RID: 19372 RVA: 0x00225F1F File Offset: 0x0022411F
		private int Side1()
		{
			if (this.War == null)
			{
				return -1;
			}
			return this.War.GetSide(this.Owner);
		}

		// Token: 0x06004BAD RID: 19373 RVA: 0x00225F3C File Offset: 0x0022413C
		private Logic.Kingdom Side1Leader()
		{
			if (this.War == null)
			{
				return null;
			}
			int side = this.Side1();
			return this.War.GetLeader(side);
		}

		// Token: 0x06004BAE RID: 19374 RVA: 0x00225F66 File Offset: 0x00224166
		private int Side2()
		{
			if (this.War == null)
			{
				return -1;
			}
			return this.War.EnemySide(this.Owner);
		}

		// Token: 0x06004BAF RID: 19375 RVA: 0x00225F84 File Offset: 0x00224184
		private Logic.Kingdom Side2Leader()
		{
			if (this.War == null)
			{
				return null;
			}
			int side = this.Side2();
			return this.War.GetLeader(side);
		}

		// Token: 0x06004BB0 RID: 19376 RVA: 0x00225FB0 File Offset: 0x002241B0
		private void UpdateStatics()
		{
			Logic.Kingdom obj = this.Side1Leader();
			if (this.m_KingdomCrest1 != null)
			{
				this.m_KingdomCrest1.SetObject(obj, null);
			}
			if (this.m_KingdomName1 != null)
			{
				UIText.SetText(this.m_KingdomName1, global::Defs.LocalizedObjName(obj, null, "", true));
			}
			Logic.Kingdom obj2 = this.Side2Leader();
			if (this.m_KingdomCrest2 != null)
			{
				this.m_KingdomCrest2.SetObject(obj2, null);
			}
			if (this.m_KingdomName2 != null)
			{
				UIText.SetText(this.m_KingdomName2, global::Defs.LocalizedObjName(obj2, null, "", true));
			}
			if (this.m_WarIcon != null)
			{
				this.m_WarIcon.overrideSprite = global::Defs.GetObj<Sprite>(this.War.def.field, "icon", null);
			}
		}

		// Token: 0x06004BB1 RID: 19377 RVA: 0x00226081 File Offset: 0x00224281
		private void UpdateDynamics()
		{
			if (this.War == null)
			{
				return;
			}
			this.UpdateSupporters(this.m_Supporters1, this.Side1());
			this.UpdateSupporters(this.m_Supporters2, this.Side2());
		}

		// Token: 0x06004BB2 RID: 19378 RVA: 0x002260B0 File Offset: 0x002242B0
		private void UpdateSupporters(StackableIconsContainer parent, int side)
		{
			if (parent == null)
			{
				return;
			}
			List<Logic.Kingdom> kingdoms = this.War.GetKingdoms(side);
			int childCount = parent.transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Transform child = parent.transform.GetChild(i);
				Logic.Kingdom kingdom = (kingdoms == null || i + 1 >= kingdoms.Count) ? null : kingdoms[i + 1];
				if (kingdom == null)
				{
					child.gameObject.SetActive(false);
				}
				else
				{
					UIKingdomIcon uikingdomIcon = global::Common.FindChildComponent<UIKingdomIcon>(child.gameObject, "id_SupporterCrest");
					if (uikingdomIcon == null)
					{
						child.gameObject.SetActive(false);
					}
					else
					{
						uikingdomIcon.SetObject(kingdom, null);
						child.gameObject.SetActive(true);
					}
				}
			}
			parent.Refresh();
		}

		// Token: 0x06004BB3 RID: 19379 RVA: 0x000023FD File Offset: 0x000005FD
		private void UpdateHighlight()
		{
		}

		// Token: 0x06004BB4 RID: 19380 RVA: 0x00226170 File Offset: 0x00224370
		private void OnClick(BSGButton b)
		{
			if (this.OnSelect != null)
			{
				this.OnSelect(this);
			}
		}

		// Token: 0x06004BB5 RID: 19381 RVA: 0x00226188 File Offset: 0x00224388
		public void OnMessage(object obj, string message, object param)
		{
			if (this == null || base.gameObject == null)
			{
				War war = this.War;
				if (war == null)
				{
					return;
				}
				war.DelListener(this);
				return;
			}
			else
			{
				if (!(message == "supporter_left") && !(message == "supporter_joined"))
				{
					message == "war_concluded";
					return;
				}
				this.UpdateDynamics();
				return;
			}
		}

		// Token: 0x04003A41 RID: 14913
		[UIFieldTarget("id_KingdomCrest1")]
		private UIKingdomIcon m_KingdomCrest1;

		// Token: 0x04003A42 RID: 14914
		[UIFieldTarget("id_KingdomName1")]
		private TextMeshProUGUI m_KingdomName1;

		// Token: 0x04003A43 RID: 14915
		[UIFieldTarget("id_KingdomCrest2")]
		private UIKingdomIcon m_KingdomCrest2;

		// Token: 0x04003A44 RID: 14916
		[UIFieldTarget("id_KingdomName2")]
		private TextMeshProUGUI m_KingdomName2;

		// Token: 0x04003A45 RID: 14917
		[UIFieldTarget("id_SelectedArrow")]
		private GameObject m_SelectedArrow;

		// Token: 0x04003A46 RID: 14918
		[UIFieldTarget("id_MoveContainer")]
		private RectTransform m_MoveContainer;

		// Token: 0x04003A47 RID: 14919
		[UIFieldTarget("id_WarScore")]
		private GameObject m_WarScore;

		// Token: 0x04003A48 RID: 14920
		[UIFieldTarget("id_StrengthBar")]
		private GameObject m_StrengthBar;

		// Token: 0x04003A49 RID: 14921
		[UIFieldTarget("id_Supporters1")]
		private StackableIconsContainer m_Supporters1;

		// Token: 0x04003A4A RID: 14922
		[UIFieldTarget("id_Supporters2")]
		private StackableIconsContainer m_Supporters2;

		// Token: 0x04003A4B RID: 14923
		[UIFieldTarget("id_WarIcon")]
		private Image m_WarIcon;

		// Token: 0x04003A4C RID: 14924
		private UIWarsOverviewWindow.UIWarScoreBar m_WarScoreProgress;

		// Token: 0x04003A4D RID: 14925
		private UIWarsOverviewWindow.UIStrengthBar m_StrengthProgress;

		// Token: 0x04003A50 RID: 14928
		public Action<UIWarsOverviewWindow.UIWarRow> OnSelect;

		// Token: 0x04003A51 RID: 14929
		private bool m_Selected;

		// Token: 0x04003A52 RID: 14930
		private bool m_Initiazled;
	}

	// Token: 0x0200076C RID: 1900
	protected class UIWarInfo : MonoBehaviour, IListener
	{
		// Token: 0x170005D8 RID: 1496
		// (get) Token: 0x06004BB7 RID: 19383 RVA: 0x002261EB File Offset: 0x002243EB
		// (set) Token: 0x06004BB8 RID: 19384 RVA: 0x002261F3 File Offset: 0x002243F3
		public Logic.Kingdom Owner { get; private set; }

		// Token: 0x170005D9 RID: 1497
		// (get) Token: 0x06004BB9 RID: 19385 RVA: 0x002261FC File Offset: 0x002243FC
		// (set) Token: 0x06004BBA RID: 19386 RVA: 0x00226204 File Offset: 0x00224404
		public Logic.Kingdom OwnLeader { get; private set; }

		// Token: 0x170005DA RID: 1498
		// (get) Token: 0x06004BBB RID: 19387 RVA: 0x0022620D File Offset: 0x0022440D
		// (set) Token: 0x06004BBC RID: 19388 RVA: 0x00226215 File Offset: 0x00224415
		public Logic.Kingdom Enemy { get; private set; }

		// Token: 0x170005DB RID: 1499
		// (get) Token: 0x06004BBD RID: 19389 RVA: 0x0022621E File Offset: 0x0022441E
		// (set) Token: 0x06004BBE RID: 19390 RVA: 0x00226226 File Offset: 0x00224426
		public War War { get; private set; }

		// Token: 0x06004BBF RID: 19391 RVA: 0x00226230 File Offset: 0x00224430
		private void Init()
		{
			if (this.m_Initiazled)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			if (this.m_CapturedProvinceRowProrotype != null)
			{
				this.m_CapturedProvinceRowProrotype.SetActive(false);
			}
			if (this.m_WarScore != null)
			{
				this.m_ScoreBar = this.m_WarScore.AddComponent<UIWarsOverviewWindow.UIWarScoreBar>();
			}
			this.m_Initiazled = true;
		}

		// Token: 0x06004BC0 RID: 19392 RVA: 0x00226290 File Offset: 0x00224490
		public void SetData(Logic.Kingdom owner, War war)
		{
			this.Init();
			War war2 = this.War;
			if (war2 != null)
			{
				war2.DelListener(this);
			}
			this.Owner = owner;
			this.War = war;
			War war3 = this.War;
			if (war3 != null)
			{
				war3.AddListener(this);
			}
			if (this.m_Audiance != null)
			{
				this.m_Audiance.onClick = new BSGButton.OnClick(this.OpenAudiance);
			}
			this.OwnLeader = ((war != null) ? war.GetLeader(this.Owner) : null);
			this.Enemy = ((war != null) ? war.GetEnemyLeader(owner) : null);
			this.m_ScoreBar.SetData(this.Owner, this.War);
			this.PopulateStatics();
			this.Refresh();
		}

		// Token: 0x06004BC1 RID: 19393 RVA: 0x00226348 File Offset: 0x00224548
		private void Refresh()
		{
			this.PopulateOwn();
			this.PopulateEnemey();
			this.PopulateCapturedRealms();
		}

		// Token: 0x06004BC2 RID: 19394 RVA: 0x0022635C File Offset: 0x0022455C
		private void PopulateStatics()
		{
			Vars vars = new Vars(this.Owner);
			vars.Set<Logic.Kingdom>("target", this.Enemy);
			UIText.SetText(this.m_WarLabel, global::Defs.Localize("WarOverview.War.against", vars, null, true, true));
			if (this.m_OwnerKingdom != null)
			{
				this.m_OwnerKingdom.SetObject(this.OwnLeader, null);
			}
			if (this.m_EnemyKingdom != null)
			{
				this.m_EnemyKingdom.SetObject(this.Enemy, null);
			}
		}

		// Token: 0x06004BC3 RID: 19395 RVA: 0x002263E4 File Offset: 0x002245E4
		private void PopulateOwn()
		{
			if (this.Owner == null)
			{
				return;
			}
			if (this.m_OwnBattleWonValue != null)
			{
				this.m_OwnBattleWonValue.text = this.GetBattleWon(this.Owner, this.War).ToString();
			}
			this.tmpList.Clear();
			if (this.m_OwnFriendsCrests != null)
			{
				UICommon.DeleteChildren(this.m_OwnFriendsCrests);
				this.GetFriends(this.Owner, this.tmpList);
				for (int i = 0; i < this.tmpList.Count; i++)
				{
					Logic.Kingdom kingdom = this.tmpList[i];
					if (kingdom != null && !kingdom.IsDefeated() && kingdom != this.Owner)
					{
						Vars vars = new Vars(kingdom);
						vars.Set<string>("variant", "compact");
						ObjectIcon.GetIcon(kingdom, vars, this.m_OwnFriendsCrests);
					}
				}
			}
			this.tmpList.Clear();
			if (this.m_OwnEnemiesCrests != null)
			{
				UICommon.DeleteChildren(this.m_OwnEnemiesCrests);
				this.GetEnemies(this.Owner, this.tmpList);
				for (int j = 0; j < this.tmpList.Count; j++)
				{
					Logic.Kingdom kingdom2 = this.tmpList[j];
					if (kingdom2 != null && !kingdom2.IsDefeated() && kingdom2 != this.Owner)
					{
						Vars vars2 = new Vars(kingdom2);
						vars2.Set<string>("variant", "compact");
						ObjectIcon.GetIcon(kingdom2, vars2, this.m_OwnEnemiesCrests);
					}
				}
			}
			if (this.m_OwnCapturesKnightsIcons != null)
			{
				UICommon.DeleteChildren(this.m_OwnCapturesKnightsIcons);
				if (this.Owner.prisoners != null)
				{
					for (int k = 0; k < this.Owner.prisoners.Count; k++)
					{
						Logic.Character character = this.Owner.prisoners[k];
						if (character != null && character.kingdom_id == this.Enemy.id)
						{
							ObjectIcon.GetIcon(character, null, this.m_OwnCapturesKnightsIcons);
						}
					}
				}
			}
		}

		// Token: 0x06004BC4 RID: 19396 RVA: 0x00226600 File Offset: 0x00224800
		private void PopulateEnemey()
		{
			if (this.Enemy == null)
			{
				return;
			}
			if (this.m_TargetBattleWonValue != null)
			{
				this.m_TargetBattleWonValue.text = this.GetBattleWon(this.Enemy, this.War).ToString();
			}
			this.tmpList.Clear();
			if (this.m_TargetFriendsCrests != null)
			{
				UICommon.DeleteChildren(this.m_TargetFriendsCrests);
				this.GetFriends(this.Enemy, this.tmpList);
				for (int i = 0; i < this.tmpList.Count; i++)
				{
					Logic.Kingdom kingdom = this.tmpList[i];
					if (kingdom != null && !kingdom.IsDefeated() && kingdom != this.Owner)
					{
						Vars vars = new Vars(kingdom);
						vars.Set<string>("variant", "compact");
						ObjectIcon.GetIcon(kingdom, vars, this.m_TargetFriendsCrests);
					}
				}
			}
			this.tmpList.Clear();
			if (this.m_TargetEnemiesCrests != null)
			{
				UICommon.DeleteChildren(this.m_TargetEnemiesCrests);
				this.GetEnemies(this.Enemy, this.tmpList);
				for (int j = 0; j < this.tmpList.Count; j++)
				{
					Logic.Kingdom kingdom2 = this.tmpList[j];
					if (kingdom2 != null && !kingdom2.IsDefeated() && kingdom2 != this.Owner)
					{
						Vars vars2 = new Vars(kingdom2);
						vars2.Set<string>("variant", "compact");
						ObjectIcon.GetIcon(kingdom2, vars2, this.m_TargetEnemiesCrests);
					}
				}
			}
			if (this.m_TargetCapturesKnightsIcons != null)
			{
				UICommon.DeleteChildren(this.m_TargetCapturesKnightsIcons);
				if (this.Enemy.prisoners != null)
				{
					for (int k = 0; k < this.Enemy.prisoners.Count; k++)
					{
						Logic.Character character = this.Enemy.prisoners[k];
						if (character != null && character.kingdom_id == this.Owner.id)
						{
							ObjectIcon.GetIcon(character, null, this.m_TargetCapturesKnightsIcons);
						}
					}
				}
			}
		}

		// Token: 0x06004BC5 RID: 19397 RVA: 0x0022681C File Offset: 0x00224A1C
		private void PopulateCapturedRealms()
		{
			if (this.m_CapturedProvinceRowProrotype == null)
			{
				return;
			}
			if (this.m_ProviceGain == null)
			{
				return;
			}
			if (this.m_ProviceLoss == null)
			{
				return;
			}
			List<Logic.Realm> capturedRealms = this.GetCapturedRealms(this.Owner, this.War);
			List<Logic.Realm> capturedRealms2 = this.GetCapturedRealms(this.Enemy, this.War);
			UICommon.DeleteActiveChildren(this.m_ProviceGain);
			if (capturedRealms != null && capturedRealms.Count > 0)
			{
				for (int i = 0; i < capturedRealms.Count; i++)
				{
					this.<PopulateCapturedRealms>g__AddCapuredrealmRow|42_0(capturedRealms[i], this.m_ProviceGain, capturedRealms[i].GetKingdom() != this.Owner);
				}
			}
			UICommon.DeleteActiveChildren(this.m_ProviceLoss);
			if (capturedRealms2 != null && capturedRealms2.Count > 0)
			{
				for (int j = 0; j < capturedRealms2.Count; j++)
				{
					this.<PopulateCapturedRealms>g__AddCapuredrealmRow|42_0(capturedRealms[j], this.m_ProviceLoss, capturedRealms2[j].GetKingdom() != this.Enemy);
				}
			}
		}

		// Token: 0x06004BC6 RID: 19398 RVA: 0x00226920 File Offset: 0x00224B20
		private int GetBattleWon(Logic.Kingdom k, War w)
		{
			List<HistoryEntry> history = w.history;
			if (history == null)
			{
				return 0;
			}
			int num = 0;
			for (int i = 0; i < history.Count; i++)
			{
				HistoryEntry historyEntry = history[i];
				if (historyEntry != null)
				{
					Vars vars = historyEntry.vars;
					if (!(historyEntry.field.key != "BattleWon") && vars.Get<Logic.Kingdom>("kingdom", null) == k)
					{
						num++;
					}
				}
			}
			return num;
		}

		// Token: 0x06004BC7 RID: 19399 RVA: 0x0022698C File Offset: 0x00224B8C
		private List<Logic.Realm> GetCapturedRealms(Logic.Kingdom k, War w)
		{
			if (k == null || w == null)
			{
				return null;
			}
			List<HistoryEntry> list = (w != null) ? w.history : null;
			if (list == null)
			{
				return null;
			}
			Dictionary<Logic.Realm, Logic.Kingdom> dictionary = new Dictionary<Logic.Realm, Logic.Kingdom>();
			for (int i = 0; i < list.Count; i++)
			{
				HistoryEntry historyEntry = list[i];
				if (historyEntry != null && !(historyEntry.field.key != "CapturedRealm") && historyEntry != null)
				{
					Vars vars = historyEntry.vars;
					Logic.Kingdom kingdom = vars.Get<Logic.Kingdom>("kingdom", null);
					if (kingdom == k)
					{
						vars.Get<Logic.Kingdom>("kingdom2", null);
						Logic.Realm realm = vars.Get<Logic.Realm>("realm", null);
						if (realm != null)
						{
							if (!dictionary.ContainsKey(realm))
							{
								dictionary.Add(realm, kingdom);
							}
							dictionary[realm] = kingdom;
						}
					}
				}
			}
			return new List<Logic.Realm>(dictionary.Keys);
		}

		// Token: 0x06004BC8 RID: 19400 RVA: 0x00226A5C File Offset: 0x00224C5C
		private string GetActivites(War w)
		{
			string text = "Activities:";
			List<HistoryEntry> list = (w != null) ? w.history : null;
			if (list == null)
			{
				return text + " NONE";
			}
			for (int i = 0; i < list.Count; i++)
			{
				HistoryEntry historyEntry = list[i];
				if (historyEntry != null)
				{
					Vars vars = historyEntry.vars;
					string key = historyEntry.field.key;
					Logic.Kingdom kingdom = vars.Get<Logic.Kingdom>("kingdom", null);
					Logic.Kingdom kingdom2 = vars.Get<Logic.Kingdom>("kingdom2", null);
					Logic.Kingdom kingdom3 = vars.Get<Logic.Kingdom>("oposition", null);
					Logic.Realm realm = vars.Get<Logic.Realm>("realm", null);
					text += string.Format("\nActivity: {0}, from {1} against {2}, with opposition {3} at  realm: {4}", new object[]
					{
						key,
						kingdom,
						kingdom2,
						kingdom3,
						realm
					});
				}
			}
			return text;
		}

		// Token: 0x06004BC9 RID: 19401 RVA: 0x00226B32 File Offset: 0x00224D32
		private void GetFriends(Logic.Kingdom k, List<Logic.Kingdom> kingodmList)
		{
			if (k == null)
			{
				return;
			}
			kingodmList.AddRange(k.allies);
			kingodmList.AddRange(k.vassalStates);
			if (k.sovereignState != null)
			{
				kingodmList.Add(k.sovereignState);
			}
		}

		// Token: 0x06004BCA RID: 19402 RVA: 0x00226B64 File Offset: 0x00224D64
		private void GetEnemies(Logic.Kingdom k, List<Logic.Kingdom> kingodmList)
		{
			if (k == null)
			{
				return;
			}
			if (k.wars == null || k.wars.Count == 0)
			{
				return;
			}
			for (int i = 0; i < k.wars.Count; i++)
			{
				War war = k.wars[i];
				kingodmList.Add((war.GetAttacker() == k) ? war.GetDefender() : war.GetAttacker());
			}
		}

		// Token: 0x06004BCB RID: 19403 RVA: 0x00226BCB File Offset: 0x00224DCB
		private void OpenAudiance(BSGButton b)
		{
			if (this.Enemy == null)
			{
				return;
			}
			AudienceWindow.Create(global::Kingdom.Get(this.Enemy.id), "Main", null);
		}

		// Token: 0x06004BCC RID: 19404 RVA: 0x000023FD File Offset: 0x000005FD
		private void Clear()
		{
		}

		// Token: 0x06004BCD RID: 19405 RVA: 0x00226BF2 File Offset: 0x00224DF2
		public void OnMessage(object obj, string message, object param)
		{
			if (this == null || base.gameObject == null)
			{
				this.Clear();
				return;
			}
			message == "war_concluded";
		}

		// Token: 0x06004BCF RID: 19407 RVA: 0x00226C34 File Offset: 0x00224E34
		[CompilerGenerated]
		private void <PopulateCapturedRealms>g__AddCapuredrealmRow|42_0(Logic.Realm r, RectTransform container, bool showCrest)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_CapturedProvinceRowProrotype, container);
			gameObject.gameObject.SetActive(true);
			TextMeshProUGUI textMeshProUGUI = global::Common.FindChildComponent<TextMeshProUGUI>(gameObject, "id_CapturedRealmName");
			UIKingdomIcon uikingdomIcon = global::Common.FindChildComponent<UIKingdomIcon>(gameObject, "id_CurrentOwnerIcon");
			if (textMeshProUGUI != null)
			{
				UIText.SetText(textMeshProUGUI, global::Defs.LocalizedObjName(r, null, "", true));
			}
			if (uikingdomIcon != null)
			{
				if (showCrest)
				{
					uikingdomIcon.SetObject(r.GetKingdom(), null);
				}
				uikingdomIcon.gameObject.SetActive(showCrest);
			}
		}

		// Token: 0x04003A53 RID: 14931
		[UIFieldTarget("id_OwnerKingdom")]
		private UIKingdomIcon m_OwnerKingdom;

		// Token: 0x04003A54 RID: 14932
		[UIFieldTarget("id_EnemyKingdom")]
		private UIKingdomIcon m_EnemyKingdom;

		// Token: 0x04003A55 RID: 14933
		[UIFieldTarget("id_OwnBattleWonValue")]
		private TextMeshProUGUI m_OwnBattleWonValue;

		// Token: 0x04003A56 RID: 14934
		[UIFieldTarget("id_TargetBattleWonValue")]
		private TextMeshProUGUI m_TargetBattleWonValue;

		// Token: 0x04003A57 RID: 14935
		[UIFieldTarget("id_WarLabel")]
		private TextMeshProUGUI m_WarLabel;

		// Token: 0x04003A58 RID: 14936
		[UIFieldTarget("id_OwnFriendsCrests")]
		private RectTransform m_OwnFriendsCrests;

		// Token: 0x04003A59 RID: 14937
		[UIFieldTarget("id_OwnEnemiesCrests")]
		private RectTransform m_OwnEnemiesCrests;

		// Token: 0x04003A5A RID: 14938
		[UIFieldTarget("id_OwnCapturesKnightsIcons")]
		private RectTransform m_OwnCapturesKnightsIcons;

		// Token: 0x04003A5B RID: 14939
		[UIFieldTarget("id_TargetFriendsCrests")]
		private RectTransform m_TargetFriendsCrests;

		// Token: 0x04003A5C RID: 14940
		[UIFieldTarget("id_TargetEnemiesCrests")]
		private RectTransform m_TargetEnemiesCrests;

		// Token: 0x04003A5D RID: 14941
		[UIFieldTarget("id_TargetCapturesKnightsIcons")]
		private RectTransform m_TargetCapturesKnightsIcons;

		// Token: 0x04003A5E RID: 14942
		[UIFieldTarget("id_Audiance")]
		private BSGButton m_Audiance;

		// Token: 0x04003A5F RID: 14943
		[UIFieldTarget("id_ProviceGain")]
		private RectTransform m_ProviceGain;

		// Token: 0x04003A60 RID: 14944
		[UIFieldTarget("id_ProviceLoss")]
		private RectTransform m_ProviceLoss;

		// Token: 0x04003A61 RID: 14945
		[UIFieldTarget("id_CapturedProvinceRowPrototype")]
		private GameObject m_CapturedProvinceRowProrotype;

		// Token: 0x04003A62 RID: 14946
		[UIFieldTarget("id_WarScoreProgress")]
		private GameObject m_WarScore;

		// Token: 0x04003A63 RID: 14947
		[UIFieldTarget("id_ActivitiesDebugInfo")]
		private TextMeshProUGUI m_ActivitiesDebugInfo;

		// Token: 0x04003A68 RID: 14952
		private UIWarsOverviewWindow.UIWarScoreBar m_ScoreBar;

		// Token: 0x04003A69 RID: 14953
		private bool m_Initiazled;

		// Token: 0x04003A6A RID: 14954
		private List<Logic.Kingdom> tmpList = new List<Logic.Kingdom>();
	}

	// Token: 0x0200076D RID: 1901
	public class UIOverview : MonoBehaviour, IListener
	{
		// Token: 0x170005DC RID: 1500
		// (get) Token: 0x06004BD0 RID: 19408 RVA: 0x00226CB1 File Offset: 0x00224EB1
		// (set) Token: 0x06004BD1 RID: 19409 RVA: 0x00226CB9 File Offset: 0x00224EB9
		public Logic.Kingdom Data { get; private set; }

		// Token: 0x06004BD2 RID: 19410 RVA: 0x00226CC4 File Offset: 0x00224EC4
		private void Init()
		{
			if (this.m_Initiazled)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			if (this.m_Audiance != null)
			{
				this.m_Audiance.onClick = new BSGButton.OnClick(this.OpenAudiance);
			}
			GameObject gameObject = global::Common.FindChildByName(base.gameObject, "id_SelectedPact", true, true);
			this.m_PackInfo = ((gameObject != null) ? gameObject.AddComponent<UIWarsOverviewWindow.UIPackInfo>() : null);
			this.m_Initiazled = true;
		}

		// Token: 0x06004BD3 RID: 19411 RVA: 0x00226D31 File Offset: 0x00224F31
		public void SetData(Logic.Kingdom kingdom)
		{
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
			this.ClearSelection(true);
		}

		// Token: 0x06004BD4 RID: 19412 RVA: 0x00226D71 File Offset: 0x00224F71
		private void Refresh()
		{
			if (this.m_EmptyPackSelectionLabel != null)
			{
				this.m_EmptyPackSelectionLabel.SetVars(new Vars(this.Data));
			}
			this.PopulateFriendsAndEnemeies();
			this.PopulatePacts();
			this.PopulateSelection();
		}

		// Token: 0x06004BD5 RID: 19413 RVA: 0x00226DAE File Offset: 0x00224FAE
		public void ClearSelection(bool refresh = true)
		{
			Logic.Kingdom selectedKingdom = this.m_SelectedKingdom;
			if (selectedKingdom != null)
			{
				selectedKingdom.DelListener(this);
			}
			this.m_SelectedKingdom = null;
			Pact selectedPact = this.m_SelectedPact;
			if (selectedPact != null)
			{
				selectedPact.DelListener(this);
			}
			this.m_SelectedPact = null;
			if (refresh)
			{
				this.PopulateSelection();
			}
		}

		// Token: 0x06004BD6 RID: 19414 RVA: 0x00226DEB File Offset: 0x00224FEB
		public void Select(Logic.Kingdom k)
		{
			this.ClearSelection(false);
			this.m_SelectedKingdom = k;
			Logic.Kingdom selectedKingdom = this.m_SelectedKingdom;
			if (selectedKingdom != null)
			{
				selectedKingdom.AddListener(this);
			}
			this.PopulateSelection();
		}

		// Token: 0x06004BD7 RID: 19415 RVA: 0x00226E13 File Offset: 0x00225013
		public void Select(Pact pact)
		{
			this.ClearSelection(false);
			this.m_SelectedPact = pact;
			Pact selectedPact = this.m_SelectedPact;
			if (selectedPact != null)
			{
				selectedPact.AddListener(this);
			}
			this.PopulateSelection();
		}

		// Token: 0x06004BD8 RID: 19416 RVA: 0x00226E3C File Offset: 0x0022503C
		private void PopulateFriendsAndEnemeies()
		{
			if (this.m_FriendsCrests != null)
			{
				List<Logic.Kingdom> friends = UIWarsOverviewWindow.UIOverview.GetFriends(this.Data);
				UICommon.DeleteChildren(this.m_FriendsCrests.transform);
				if (friends != null && friends.Count > 0)
				{
					for (int i = 0; i < friends.Count; i++)
					{
						Logic.Kingdom kingdom = friends[i];
						if (kingdom != null && !kingdom.IsDefeated() && kingdom != this.Data)
						{
							UIWarsOverviewWindow.UIOverview.<PopulateFriendsAndEnemeies>g__AddKingdomIcon|29_0(kingdom, this.m_FriendsCrests.transform as RectTransform);
						}
					}
				}
			}
			if (this.m_EnemiesCrests != null)
			{
				List<Logic.Kingdom> threads = UIWarsOverviewWindow.UIOverview.GetThreads(this.Data);
				UICommon.DeleteChildren(this.m_EnemiesCrests.transform);
				if (threads != null && threads.Count > 0)
				{
					for (int j = 0; j < threads.Count; j++)
					{
						Logic.Kingdom kingdom2 = threads[j];
						if (kingdom2 != null && !kingdom2.IsDefeated() && kingdom2 != this.Data)
						{
							UIWarsOverviewWindow.UIOverview.<PopulateFriendsAndEnemeies>g__AddKingdomIcon|29_0(kingdom2, this.m_EnemiesCrests.transform as RectTransform);
						}
					}
				}
			}
			this.m_FriendsCrests.Refresh();
			this.m_EnemiesCrests.Refresh();
		}

		// Token: 0x06004BD9 RID: 19417 RVA: 0x00226F5C File Offset: 0x0022515C
		private void PopulatePacts()
		{
			if (this.m_OurPacts != null)
			{
				List<Pact> pacts = this.Data.pacts;
				UICommon.DeleteChildren(this.m_OurPacts);
				if (pacts != null && pacts.Count > 0)
				{
					for (int i = 0; i < pacts.Count; i++)
					{
						Pact pact = pacts[i];
						if (pact != null && pact.IsValid())
						{
							this.<PopulatePacts>g__AddPactIcon|30_0(pact, this.m_OurPacts, false);
						}
					}
				}
			}
			if (this.m_PactsAgainstUs != null)
			{
				List<Pact> pacts_against = this.Data.pacts_against;
				UICommon.DeleteChildren(this.m_PactsAgainstUs);
				if (pacts_against != null && pacts_against.Count > 0)
				{
					for (int j = 0; j < pacts_against.Count; j++)
					{
						Pact pact2 = pacts_against[j];
						if (pact2 != null && pact2.IsValid() && pact2.IsVisibleBy(this.Data, null))
						{
							this.<PopulatePacts>g__AddPactIcon|30_0(pact2, this.m_PactsAgainstUs, true);
						}
					}
				}
			}
		}

		// Token: 0x06004BDA RID: 19418 RVA: 0x00227048 File Offset: 0x00225248
		private void PopulateSelection()
		{
			this.PopulateKingdomSelection();
			this.m_PackInfo.SetData(this.Data, this.m_SelectedPact);
			this.m_EmptySelectionContainer.SetActive(this.m_SelectedKingdom == null && this.m_SelectedPact == null);
		}

		// Token: 0x06004BDB RID: 19419 RVA: 0x00227088 File Offset: 0x00225288
		private void PopulateKingdomSelection()
		{
			if (this.m_SelectedKingdom == null)
			{
				this.m_SelectedKingdomContainer.SetActive(false);
				return;
			}
			this.m_SelectedKingdomContainer.SetActive(true);
			UIText.SetTextKey(this.m_KingdomName, "Kingdom.name", new Vars(this.m_SelectedKingdom), null);
			if (this.m_KingdomCrest != null)
			{
				this.m_KingdomCrest.SetObject(this.m_SelectedKingdom, null);
			}
			if (this.m_KingdomRelations != null)
			{
				this.m_KingdomRelations.SetData(this.Data, this.m_SelectedKingdom);
			}
			if (this.m_ReligionLabel)
			{
				string relgionNameKey = global::Religions.GetRelgionNameKey(this.m_SelectedKingdom);
				if (!string.IsNullOrEmpty(relgionNameKey))
				{
					UIText.SetTextKey(this.m_ReligionLabel, relgionNameKey, this.m_SelectedKingdom, null);
				}
				else
				{
					UIText.SetText(this.m_ReligionLabel, global::Defs.Localize(this.m_SelectedKingdom.religion.def.field, "name", this.m_SelectedKingdom.religion, null, true, true));
				}
			}
			if (this.m_ReligionIcon != null)
			{
				this.m_ReligionIcon.SetData(this.m_SelectedKingdom);
			}
			if (this.m_KingdomOpinions != null)
			{
				List<ProsAndCons.Factor> list;
				if (UIWarsOverviewWindow.UIOverview.IsFriend(this.m_SelectedKingdom, this.Data))
				{
					list = this.GetPros(this.Data, this.m_SelectedKingdom);
				}
				else
				{
					list = this.GetCons(this.Data, this.m_SelectedKingdom);
				}
				if (list != null)
				{
					string text = "";
					for (int i = 0; i < list.Count; i++)
					{
						text = text + global::Defs.Localize(list[i].def.field, "reason_to_them", null, null, true, true) + "\n";
					}
					this.m_KingdomOpinions.text = text;
				}
			}
		}

		// Token: 0x06004BDC RID: 19420 RVA: 0x00227246 File Offset: 0x00225446
		private static bool IsThreat(Logic.Kingdom target_kingdom, Logic.Kingdom source_kingdom)
		{
			return source_kingdom.IsThreat(target_kingdom);
		}

		// Token: 0x06004BDD RID: 19421 RVA: 0x00227250 File Offset: 0x00225450
		private static bool IsFriend(Logic.Kingdom target_kingdom, Logic.Kingdom source_kingdom)
		{
			if (source_kingdom.IsDefeated())
			{
				return false;
			}
			if (target_kingdom.IsDefeated())
			{
				return false;
			}
			if (source_kingdom.GetRoyalMarriage(target_kingdom))
			{
				return false;
			}
			RelationUtils.Stance stance = source_kingdom.GetStance(target_kingdom);
			return !stance.IsWar() && !stance.IsAlliance() && target_kingdom.sovereignState != source_kingdom && source_kingdom.sovereignState != target_kingdom && (float)source_kingdom.DistanceToKingdom(target_kingdom) <= source_kingdom.friends_max_distance && source_kingdom.GetRelationship(target_kingdom) >= source_kingdom.friends_min_relationship;
		}

		// Token: 0x06004BDE RID: 19422 RVA: 0x002272D0 File Offset: 0x002254D0
		public static List<Logic.Kingdom> GetThreads(Logic.Kingdom sourceKingdom)
		{
			if (sourceKingdom == null)
			{
				return null;
			}
			List<Logic.Kingdom> kingdoms = sourceKingdom.game.kingdoms;
			List<Logic.Kingdom> list = new List<Logic.Kingdom>();
			for (int i = 0; i < kingdoms.Count; i++)
			{
				Logic.Kingdom kingdom = kingdoms[i];
				if (UIWarsOverviewWindow.UIOverview.IsThreat(kingdom, sourceKingdom))
				{
					list.Add(kingdom);
				}
			}
			list.Sort((Logic.Kingdom x, Logic.Kingdom y) => x.GetRelationship(sourceKingdom).CompareTo(y.GetRelationship(sourceKingdom)));
			if (list.Count > UIWarsOverviewWindow.UIOverview.warscore_tf_display)
			{
				list.RemoveRange(UIWarsOverviewWindow.UIOverview.warscore_tf_display - 1, list.Count - UIWarsOverviewWindow.UIOverview.warscore_tf_display);
			}
			return list;
		}

		// Token: 0x06004BDF RID: 19423 RVA: 0x00227378 File Offset: 0x00225578
		public static List<Logic.Kingdom> GetFriends(Logic.Kingdom kingdom)
		{
			if (kingdom == null)
			{
				return null;
			}
			List<Logic.Kingdom> kingdoms = kingdom.game.kingdoms;
			List<Logic.Kingdom> list = new List<Logic.Kingdom>();
			for (int i = 0; i < kingdoms.Count; i++)
			{
				Logic.Kingdom kingdom2 = kingdoms[i];
				if (UIWarsOverviewWindow.UIOverview.IsFriend(kingdom2, kingdom))
				{
					list.Add(kingdom2);
				}
			}
			list.Sort((Logic.Kingdom x, Logic.Kingdom y) => x.GetRelationship(kingdom).CompareTo(y.GetRelationship(kingdom)));
			if (list.Count > UIWarsOverviewWindow.UIOverview.warscore_tf_display)
			{
				list.RemoveRange(UIWarsOverviewWindow.UIOverview.warscore_tf_display - 1, list.Count - UIWarsOverviewWindow.UIOverview.warscore_tf_display);
			}
			return list;
		}

		// Token: 0x06004BE0 RID: 19424 RVA: 0x00227420 File Offset: 0x00225620
		private List<ProsAndCons.Factor> GetPros(Logic.Kingdom self, Logic.Kingdom target)
		{
			ProsAndCons prosAndCons = ProsAndCons.Get("PC_Propose_DemandAttackKingdom", target, this.Data);
			if (prosAndCons == null)
			{
				return null;
			}
			prosAndCons.Calc(false);
			List<ProsAndCons.Factor> list = new List<ProsAndCons.Factor>(prosAndCons.pros);
			list.Sort((ProsAndCons.Factor x, ProsAndCons.Factor y) => y.value.CompareTo(x.value));
			ProsAndCons.Factor factor = null;
			for (int i = list.Count - 1; i >= 0; i--)
			{
				if (list[i].def.field.key == "pc_we_have_good_relation")
				{
					factor = list[i];
					list.RemoveAt(i);
				}
				else if (list[i].value == 0)
				{
					list.RemoveAt(i);
				}
			}
			if (list.Count > UIWarsOverviewWindow.UIOverview.warscore_display_pcs)
			{
				list.RemoveRange(UIWarsOverviewWindow.UIOverview.warscore_display_pcs - 1, list.Count - UIWarsOverviewWindow.UIOverview.warscore_display_pcs);
			}
			if (list.Count == 0 && factor != null)
			{
				list.Add(factor);
			}
			return list;
		}

		// Token: 0x06004BE1 RID: 19425 RVA: 0x00227514 File Offset: 0x00225714
		private List<ProsAndCons.Factor> GetCons(Logic.Kingdom self, Logic.Kingdom target)
		{
			ProsAndCons prosAndCons = ProsAndCons.Get("PC_War", target, this.Data);
			if (prosAndCons == null)
			{
				return null;
			}
			prosAndCons.Calc(false);
			List<ProsAndCons.Factor> list = new List<ProsAndCons.Factor>(prosAndCons.pros);
			ProsAndCons.Factor factor = null;
			for (int i = list.Count - 1; i >= 0; i--)
			{
				if (list[i].def.field.key == "pc_we_have_bad_relation")
				{
					factor = list[i];
					list.RemoveAt(i);
				}
				else if (list[i].value == 0)
				{
					list.RemoveAt(i);
				}
			}
			list.Sort((ProsAndCons.Factor x, ProsAndCons.Factor y) => y.value.CompareTo(x.value));
			if (list.Count > UIWarsOverviewWindow.UIOverview.warscore_display_pcs)
			{
				list.RemoveRange(UIWarsOverviewWindow.UIOverview.warscore_display_pcs - 1, list.Count - UIWarsOverviewWindow.UIOverview.warscore_display_pcs);
			}
			if (list.Count == 0 && factor != null)
			{
				list.Add(factor);
			}
			return list;
		}

		// Token: 0x06004BE2 RID: 19426 RVA: 0x00227607 File Offset: 0x00225807
		private void OpenAudiance(BSGButton b)
		{
			if (this.m_SelectedKingdom == null)
			{
				return;
			}
			AudienceWindow.Create(global::Kingdom.Get(this.m_SelectedKingdom.id), "Main", null);
		}

		// Token: 0x06004BE3 RID: 19427 RVA: 0x0022762E File Offset: 0x0022582E
		private void Clear()
		{
			this.ClearSelection(false);
			Logic.Kingdom data = this.Data;
			if (data != null)
			{
				data.DelListener(this);
			}
			this.Data = null;
		}

		// Token: 0x06004BE4 RID: 19428 RVA: 0x00227650 File Offset: 0x00225850
		private void OnDestroy()
		{
			this.Clear();
		}

		// Token: 0x06004BE5 RID: 19429 RVA: 0x00227658 File Offset: 0x00225858
		public void OnMessage(object obj, string message, object param)
		{
			if (this == null || base.gameObject == null)
			{
				this.Clear();
				return;
			}
			if (obj == this.Data)
			{
				uint num = <PrivateImplementationDetails>.ComputeStringHash(message);
				if (num <= 2261655967U)
				{
					if (num <= 98381933U)
					{
						if (num != 66019599U)
						{
							if (num != 98381933U)
							{
								return;
							}
							if (!(message == "left_war"))
							{
								return;
							}
						}
						else if (!(message == "del_pact_against"))
						{
							return;
						}
					}
					else if (num != 274589411U)
					{
						if (num != 1383466693U)
						{
							if (num != 2261655967U)
							{
								return;
							}
							if (!(message == "war_started"))
							{
								return;
							}
						}
						else if (!(message == "war_concluded"))
						{
							return;
						}
					}
					else if (!(message == "del_pact"))
					{
						return;
					}
				}
				else if (num <= 2652411498U)
				{
					if (num != 2314870149U)
					{
						if (num != 2500072957U)
						{
							if (num != 2652411498U)
							{
								return;
							}
							if (!(message == "war_ended"))
							{
								return;
							}
						}
						else if (!(message == "add_pact_against"))
						{
							return;
						}
					}
					else if (!(message == "reveal_pact"))
					{
						return;
					}
				}
				else if (num != 3079482253U)
				{
					if (num != 3771806931U)
					{
						if (num != 4192422753U)
						{
							return;
						}
						if (!(message == "add_pact"))
						{
							return;
						}
					}
					else if (!(message == "conceal_pact"))
					{
						return;
					}
				}
				else if (!(message == "joined_war"))
				{
					return;
				}
				this.Refresh();
				return;
			}
			if (obj == this.m_SelectedKingdom)
			{
				return;
			}
			if (obj == this.m_SelectedPact)
			{
				if (message == "destroying" || message == "finishing" || message == "supporter_left" || message == "supporter_joined")
				{
					this.PopulateSelection();
				}
				return;
			}
		}

		// Token: 0x06004BE8 RID: 19432 RVA: 0x00227820 File Offset: 0x00225A20
		[CompilerGenerated]
		internal static void <PopulateFriendsAndEnemeies>g__AddKingdomIcon|29_0(Logic.Kingdom k, RectTransform container)
		{
			Vars vars = new Vars(k);
			GameObject icon = ObjectIcon.GetIcon(k, vars, container);
			if (icon == null)
			{
				return;
			}
			icon.GetComponent<UIKingdomIcon>();
		}

		// Token: 0x06004BE9 RID: 19433 RVA: 0x00227854 File Offset: 0x00225A54
		[CompilerGenerated]
		private void <PopulatePacts>g__AddPactIcon|30_0(Pact pact, RectTransform container, bool show_pact_owner)
		{
			Vars vars = new Vars(pact);
			vars.Set<string>("variant", "compact");
			GameObject icon = ObjectIcon.GetIcon(pact, vars, container);
			UIPactIcon uipactIcon = (icon != null) ? icon.GetComponent<UIPactIcon>() : null;
			uipactIcon.ShowOwnerCrest(show_pact_owner);
			uipactIcon.EnableTooltip(false, true);
			if (uipactIcon != null)
			{
				uipactIcon.OnSelect += delegate(UIPactIcon i)
				{
					this.Select(pact);
				};
			}
		}

		// Token: 0x04003A6B RID: 14955
		[UIFieldTarget("id_FriendsCrests")]
		private StackableIconsContainer m_FriendsCrests;

		// Token: 0x04003A6C RID: 14956
		[UIFieldTarget("id_EnemiesCrests")]
		private StackableIconsContainer m_EnemiesCrests;

		// Token: 0x04003A6D RID: 14957
		[UIFieldTarget("id_OurPacts")]
		private RectTransform m_OurPacts;

		// Token: 0x04003A6E RID: 14958
		[UIFieldTarget("id_PactsAgainstUs")]
		private RectTransform m_PactsAgainstUs;

		// Token: 0x04003A6F RID: 14959
		[UIFieldTarget("id_EmptySelection")]
		private GameObject m_EmptySelectionContainer;

		// Token: 0x04003A70 RID: 14960
		[UIFieldTarget("id_SelectedKingdom")]
		private GameObject m_SelectedKingdomContainer;

		// Token: 0x04003A71 RID: 14961
		[UIFieldTarget("id_SelctedKingdomCrest")]
		private UIKingdomIcon m_KingdomCrest;

		// Token: 0x04003A72 RID: 14962
		[UIFieldTarget("id_KingdomName")]
		private TextMeshProUGUI m_KingdomName;

		// Token: 0x04003A73 RID: 14963
		[UIFieldTarget("id_ReligionIcon")]
		private UIReligion m_ReligionIcon;

		// Token: 0x04003A74 RID: 14964
		[UIFieldTarget("id_ReligionLabel")]
		private TextMeshProUGUI m_ReligionLabel;

		// Token: 0x04003A75 RID: 14965
		[UIFieldTarget("id_KingdomRelations")]
		private UIKingdomRelations m_KingdomRelations;

		// Token: 0x04003A76 RID: 14966
		[UIFieldTarget("id_KingdomOpinions")]
		private TextMeshProUGUI m_KingdomOpinions;

		// Token: 0x04003A77 RID: 14967
		[UIFieldTarget("id_SelectedKingdomAudiance")]
		private BSGButton m_Audiance;

		// Token: 0x04003A78 RID: 14968
		[UIFieldTarget("id_SelectedPact")]
		private GameObject m_SelectedPactContainer;

		// Token: 0x04003A79 RID: 14969
		[UIFieldTarget("id_EmptyPackSelectionLabel")]
		private UIText m_EmptyPackSelectionLabel;

		// Token: 0x04003A7A RID: 14970
		private Logic.Kingdom m_SelectedKingdom;

		// Token: 0x04003A7B RID: 14971
		private Pact m_SelectedPact;

		// Token: 0x04003A7D RID: 14973
		private UIWarsOverviewWindow.UIPackInfo m_PackInfo;

		// Token: 0x04003A7E RID: 14974
		private bool m_Initiazled;

		// Token: 0x04003A7F RID: 14975
		private static int warscore_tf_display = 5;

		// Token: 0x04003A80 RID: 14976
		private static int warscore_display_pcs = 4;
	}

	// Token: 0x0200076E RID: 1902
	public class UIPackInfo : MonoBehaviour, IListener
	{
		// Token: 0x170005DD RID: 1501
		// (get) Token: 0x06004BEA RID: 19434 RVA: 0x002278DF File Offset: 0x00225ADF
		// (set) Token: 0x06004BEB RID: 19435 RVA: 0x002278E7 File Offset: 0x00225AE7
		public Logic.Kingdom Kingdom { get; private set; }

		// Token: 0x170005DE RID: 1502
		// (get) Token: 0x06004BEC RID: 19436 RVA: 0x002278F0 File Offset: 0x00225AF0
		// (set) Token: 0x06004BED RID: 19437 RVA: 0x002278F8 File Offset: 0x00225AF8
		public Pact Pact { get; private set; }

		// Token: 0x06004BEE RID: 19438 RVA: 0x00227901 File Offset: 0x00225B01
		private void Init()
		{
			if (this.m_Initiazled)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			this.m_Initiazled = true;
		}

		// Token: 0x06004BEF RID: 19439 RVA: 0x0022791A File Offset: 0x00225B1A
		public void SetData(Logic.Kingdom kingdom, Pact pact)
		{
			this.Init();
			this.Pact = pact;
			this.Kingdom = kingdom;
			this.PopulatePactSelection();
		}

		// Token: 0x06004BF0 RID: 19440 RVA: 0x00227938 File Offset: 0x00225B38
		private void PopulatePactSelection()
		{
			if (this.Pact != null && !this.Kingdom.pacts.Contains(this.Pact) && !this.Kingdom.pacts_against.Contains(this.Pact))
			{
				this.Pact = null;
			}
			if (this.Pact == null)
			{
				base.gameObject.SetActive(false);
				return;
			}
			base.gameObject.SetActive(true);
			UIKingdomIcon leaderCrest = this.m_LeaderCrest;
			if (leaderCrest != null)
			{
				leaderCrest.SetObject(this.Pact.leader, null);
			}
			UIKingdomIcon targetCrest = this.m_TargetCrest;
			if (targetCrest != null)
			{
				targetCrest.SetObject(this.Pact.target, null);
			}
			UIText.SetTextKey(this.m_PactName, "Pact.name", this.Pact, null);
			UIText.SetTextKey(this.m_OwnerLabel, "Pact.owner_label", this.Pact, null);
			Logic.Kingdom kingdom;
			Logic.Kingdom kingdom2;
			if (this.Pact.type == Pact.Type.Defensive)
			{
				kingdom = this.Pact.target;
				kingdom2 = this.Pact.leader;
			}
			else if (this.Pact.type == Pact.Type.Offensive)
			{
				kingdom = this.Pact.leader;
				kingdom2 = this.Pact.target;
			}
			else
			{
				kingdom2 = (kingdom = null);
			}
			Pact pact;
			Pact pact2;
			bool flag = War.PredictStartMembers(kingdom, kingdom2, out pact, out pact2, UIWarsOverviewWindow.UIPackInfo.tmp_attackers, UIWarsOverviewWindow.UIPackInfo.tmp_defenders);
			List<Logic.Kingdom> list;
			if (this.Pact.type == Pact.Type.Defensive)
			{
				list = UIWarsOverviewWindow.UIPackInfo.tmp_defenders;
			}
			else if (this.Pact.type == Pact.Type.Offensive)
			{
				list = UIWarsOverviewWindow.UIPackInfo.tmp_attackers;
			}
			else
			{
				list = null;
			}
			if (this.m_PactMembers != null)
			{
				int childCount = this.m_PactMembers.childCount;
				int count = this.Pact.members.Count;
				for (int i = 0; i < childCount; i++)
				{
					GameObject gameObject = this.m_PactMembers.GetChild(i).gameObject;
					Logic.Kingdom kingdom3 = (i >= count) ? null : this.Pact.members[i];
					string status_text = null;
					string status_tooltip = null;
					bool can_join = true;
					if (kingdom3 != null && (list == null || !list.Contains(kingdom3) || (!flag && (kingdom3 == kingdom2 || kingdom3 == kingdom))))
					{
						status_text = global::Defs.Localize("Pact.cannot_join_label", null, null, true, true);
						status_tooltip = this.GetCannotJoinReason(kingdom3, this.Pact, UIWarsOverviewWindow.UIPackInfo.tmp_attackers, UIWarsOverviewWindow.UIPackInfo.tmp_defenders);
						can_join = false;
					}
					bool pact_leader = this.Pact.leader == kingdom3;
					this.FillPactMember(this.Pact, gameObject, kingdom3, status_text, status_tooltip, can_join, pact_leader);
				}
			}
		}

		// Token: 0x06004BF1 RID: 19441 RVA: 0x00227BA0 File Offset: 0x00225DA0
		private void FillPactMember(Pact pact, GameObject go, Logic.Kingdom k, string status_text, string status_tooltip, bool can_join, bool pact_leader)
		{
			if (k == null)
			{
				go.SetActive(false);
				return;
			}
			go.SetActive(true);
			UIKingdomIcon uikingdomIcon = global::Common.FindChildComponent<UIKingdomIcon>(go, "id_MemberCrest");
			if (uikingdomIcon != null)
			{
				uikingdomIcon.SetObject(k, null);
			}
			TextMeshProUGUI textMeshProUGUI = global::Common.FindChildComponent<TextMeshProUGUI>(go, "id_MemberName");
			if (textMeshProUGUI != null)
			{
				Vars vars = new Vars(k);
				vars.Set<bool>("can_join_war", can_join);
				UIText.ForceNextLinks(UIText.LinkSettings.Mode.NotColorized);
				UIText.SetTextKey(textMeshProUGUI, "WarOverview.Pacts.kingdom_name", vars, null);
			}
			TextMeshProUGUI textMeshProUGUI2 = global::Common.FindChildComponent<TextMeshProUGUI>(go, "id_MemberStatus");
			if (textMeshProUGUI2 != null)
			{
				if (status_text == null)
				{
					textMeshProUGUI2.gameObject.SetActive(false);
				}
				else
				{
					UIText.SetText(textMeshProUGUI2, status_text);
					textMeshProUGUI2.gameObject.SetActive(true);
					Tooltip tooltip = Tooltip.Get(textMeshProUGUI2.gameObject, true);
					if (status_tooltip == null)
					{
						tooltip.Clear(true);
					}
					else
					{
						tooltip.SetText(status_tooltip, null, null);
					}
				}
			}
			GameObject gameObject = global::Common.FindChildByName(go, "id_NotWarEligable", true, true);
			if (gameObject != null)
			{
				gameObject.gameObject.SetActive(!can_join);
			}
			BSGButton bsgbutton = global::Common.FindChildComponent<BSGButton>(go, "id_RemovePackMember");
			if (bsgbutton != null)
			{
				bsgbutton.gameObject.SetActive(false);
			}
			UIKingdomRelations uikingdomRelations = global::Common.FindChildComponent<UIKingdomRelations>(go, "id_KingdomRelations");
			if (uikingdomRelations != null)
			{
				uikingdomRelations.gameObject.SetActive(k != this.Kingdom);
				if (k != this.Kingdom)
				{
					uikingdomRelations.SetData(k, this.Kingdom);
				}
			}
			GameObject gameObject2 = global::Common.FindChildByName(go, "id_Self", true, true);
			if (gameObject2 != null)
			{
				gameObject2.gameObject.SetActive(k == this.Kingdom);
				if (k == this.Kingdom)
				{
					TextMeshProUGUI textMeshProUGUI3 = global::Common.FindChildComponent<TextMeshProUGUI>(gameObject2, "id_LabelSelf");
					if (textMeshProUGUI3 != null)
					{
						UIText.SetTextKey(textMeshProUGUI3, pact_leader ? "WarOverview.Pacts.pact_leader" : "WarOverview.Pacts.pact_supporter", null, null);
					}
					if (pact_leader)
					{
						GameObject host = global::Common.FindChildByName(gameObject2, "id_LeavePact", true, true);
						Action action = null;
						if (pact.type == Pact.Type.Offensive)
						{
							action = pact.owner.FindAction("DissolveOffensivePactAction");
						}
						if (pact.type == Pact.Type.Defensive)
						{
							action = pact.owner.FindAction("DissolveDefensivePactAction");
						}
						UIActionIcon.Possess(action.visuals as ActionVisuals, host, null);
					}
					else
					{
						BSGButton bsgbutton2 = global::Common.FindChildComponent<BSGButton>(gameObject2, "id_LeavePact");
						if (bsgbutton2 != null)
						{
							bsgbutton2.onClick = new BSGButton.OnClick(this.LeavePact);
							Tooltip.Get(bsgbutton2.gameObject, true).SetDef("LeavePactTooltip", new Vars(pact));
						}
					}
					TextMeshProUGUI textMeshProUGUI4 = global::Common.FindChildComponent<TextMeshProUGUI>(gameObject2, "id_LeaveLabel");
					if (textMeshProUGUI4)
					{
						UIText.SetTextKey(textMeshProUGUI4, pact_leader ? "Pact.dissolve_label" : "Pact.leave_label", null, null);
					}
				}
			}
		}

		// Token: 0x06004BF2 RID: 19442 RVA: 0x00227E5E File Offset: 0x0022605E
		public bool OnLeaveConfirmationMessageButton(MessageWnd wnd, string btn_id)
		{
			if (btn_id == "ok")
			{
				Pact pact = this.Pact;
				if (pact != null)
				{
					pact.Leave(this.Kingdom, null, true);
				}
			}
			wnd.CloseAndDismiss(true);
			return true;
		}

		// Token: 0x06004BF3 RID: 19443 RVA: 0x00227E90 File Offset: 0x00226090
		private void LeavePact(BSGButton b)
		{
			Pact pact = this.Pact;
			if (pact == null || !pact.members.Contains(this.Kingdom))
			{
				return;
			}
			MessageWnd.Create("LeavePactConfirmationMessage", new Vars(pact), null, new MessageWnd.OnButton(this.OnLeaveConfirmationMessageButton)).on_update = delegate(MessageWnd w)
			{
				if (!pact.members.Contains(this.Kingdom))
				{
					w.Close(false);
				}
			};
		}

		// Token: 0x06004BF4 RID: 19444 RVA: 0x00227F10 File Offset: 0x00226110
		private string GetCannotJoinReasonKey(Logic.Kingdom k, Pact pact, List<Logic.Kingdom> attackers, List<Logic.Kingdom> defenders, out Logic.Kingdom k2)
		{
			k2 = null;
			if (k.IsAlly(pact.target))
			{
				return "ally_of_target";
			}
			if (k.IsEnemy(pact.target))
			{
				return "enemy_of_target";
			}
			if (pact.type != Pact.Type.Offensive)
			{
				return null;
			}
			if (defenders.Contains(k))
			{
				return "has_defensive_pact";
			}
			for (int i = 0; i < defenders.Count; i++)
			{
				Logic.Kingdom kingdom = defenders[i];
				if (k.IsAlly(kingdom))
				{
					k2 = kingdom;
					return "ally_of_defender";
				}
			}
			for (int j = 0; j < defenders.Count; j++)
			{
				Logic.Kingdom kingdom2 = defenders[j];
				if (k.IsEnemy(kingdom2))
				{
					k2 = kingdom2;
					return "enemy_of_defender";
				}
			}
			return null;
		}

		// Token: 0x06004BF5 RID: 19445 RVA: 0x00227FC4 File Offset: 0x002261C4
		private string GetCannotJoinReason(Logic.Kingdom k, Pact pact, List<Logic.Kingdom> attackers, List<Logic.Kingdom> defenders)
		{
			Logic.Kingdom kingdom;
			string cannotJoinReasonKey = this.GetCannotJoinReasonKey(k, pact, attackers, defenders, out kingdom);
			if (cannotJoinReasonKey == null)
			{
				return null;
			}
			Vars vars = new Vars(pact);
			vars.Set<Logic.Kingdom>("kingdom", k);
			if (kingdom != null)
			{
				vars.Set<Logic.Kingdom>("kingdom2", kingdom);
			}
			string text = global::Defs.Localize("Pact.cannot_join_reasons." + cannotJoinReasonKey, vars, null, false, true);
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			return "#" + text;
		}

		// Token: 0x06004BF6 RID: 19446 RVA: 0x000023FD File Offset: 0x000005FD
		public void OnMessage(object obj, string message, object param)
		{
		}

		// Token: 0x04003A81 RID: 14977
		[UIFieldTarget("id_LeaderCrest")]
		private UIKingdomIcon m_LeaderCrest;

		// Token: 0x04003A82 RID: 14978
		[UIFieldTarget("id_TargetCrest")]
		private UIKingdomIcon m_TargetCrest;

		// Token: 0x04003A83 RID: 14979
		[UIFieldTarget("id_PactName")]
		private TextMeshProUGUI m_PactName;

		// Token: 0x04003A84 RID: 14980
		[UIFieldTarget("id_OwnerLabel")]
		private TextMeshProUGUI m_OwnerLabel;

		// Token: 0x04003A85 RID: 14981
		[UIFieldTarget("id_PactMembers")]
		private RectTransform m_PactMembers;

		// Token: 0x04003A88 RID: 14984
		private bool m_Initiazled;

		// Token: 0x04003A89 RID: 14985
		private static List<Logic.Kingdom> tmp_attackers = new List<Logic.Kingdom>(8);

		// Token: 0x04003A8A RID: 14986
		private static List<Logic.Kingdom> tmp_defenders = new List<Logic.Kingdom>(8);
	}
}
