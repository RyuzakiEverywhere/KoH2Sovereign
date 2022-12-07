using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000294 RID: 660
public class UIRoyalDungeon : UIWindow, IListener
{
	// Token: 0x060028AA RID: 10410 RVA: 0x0015BA55 File Offset: 0x00159C55
	public override string GetDefId()
	{
		return UIRoyalDungeon.def_id;
	}

	// Token: 0x170001F4 RID: 500
	// (get) Token: 0x060028AB RID: 10411 RVA: 0x0015BA5C File Offset: 0x00159C5C
	// (set) Token: 0x060028AC RID: 10412 RVA: 0x0015BA64 File Offset: 0x00159C64
	public Logic.Kingdom Data { get; private set; }

	// Token: 0x060028AD RID: 10413 RVA: 0x0015BA70 File Offset: 0x00159C70
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_PrisonerPrototype != null)
		{
			LayoutElement component = this.m_PrisonerPrototype.GetComponent<LayoutElement>();
			this.m_RowHeight = ((component != null) ? component.preferredHeight : this.m_RowHeight);
			this.m_PrisonerPrototype.gameObject.SetActive(false);
		}
		if (this.m_PriosnerContainer != null)
		{
			VerticalLayoutGroup component2 = this.m_PriosnerContainer.GetComponent<VerticalLayoutGroup>();
			this.m_RowSpacing = ((component2 != null) ? component2.spacing : this.m_RowSpacing);
		}
		if (this.m_Button_Close != null)
		{
			this.m_Button_Close.onClick = new BSGButton.OnClick(this.HandleOnCloseButtonCLick);
		}
		this.m_Vars = new Vars();
		this.m_Initialized = true;
	}

	// Token: 0x060028AE RID: 10414 RVA: 0x0015BB35 File Offset: 0x00159D35
	private void LateUpdate()
	{
		if (this.m_RefreshOnPrisonChanged)
		{
			this.OnPrisonChanged();
			this.m_RefreshOnPrisonChanged = false;
		}
	}

	// Token: 0x060028AF RID: 10415 RVA: 0x0015BB4C File Offset: 0x00159D4C
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
		this.m_Vars.obj = this.Data;
		if (this.m_Caption != null)
		{
			UIText.SetTextKey(this.m_Caption, "RoyalDungeon.caption", this.m_Vars, null);
		}
		this.RefreshPrisonerList();
		this.RefreshKingdomActions();
		this.RereshCapacityText();
	}

	// Token: 0x060028B0 RID: 10416 RVA: 0x0015BBD8 File Offset: 0x00159DD8
	private void RefreshKingdomActions()
	{
		if (this.m_KingdomActions == null && this.m_GlobalActions != null)
		{
			this.m_KingdomActions = this.m_GlobalActions.gameObject.AddComponent<UIRoyalDungeon.KingdomActions>();
		}
		if (this.m_KingdomActions == null)
		{
			return;
		}
		if (this.Data != this.m_KingdomActions.Kingdom)
		{
			this.m_KingdomActions.SetData(this.Data, null);
		}
	}

	// Token: 0x060028B1 RID: 10417 RVA: 0x0015BC4C File Offset: 0x00159E4C
	private void RefreshPrisonerList()
	{
		if (this.Data == null)
		{
			return;
		}
		if (this.m_PriosnerContainer == null)
		{
			return;
		}
		if (this.m_PrisonerPrototype == null)
		{
			return;
		}
		int count = this.Data.prisoners.Count;
		if (count == 0)
		{
			this.Close(false);
			return;
		}
		while (this.m_Prisoners.Count < count)
		{
			UIPrisoner uiprisoner = UIPrisoner.Create(null, this.m_PrisonerPrototype, this.m_PriosnerContainer);
			if (uiprisoner == null)
			{
				break;
			}
			this.m_Prisoners.Add(uiprisoner);
		}
		int num = 0;
		for (int i = 0; i < this.m_Prisoners.Count; i++)
		{
			Logic.Character character = (this.Data.prisoners.Count > i) ? this.Data.prisoners[i] : null;
			this.m_Prisoners[i].SetData(character, null);
			this.m_Prisoners[i].gameObject.SetActive(character != null);
			if (character != null)
			{
				num++;
			}
		}
		if (this.m_BodyScrollRect != null)
		{
			num = Mathf.Min(num, this.m_MaxVisibleRows);
			RectTransform viewport = this.m_BodyScrollRect.viewport;
			LayoutElement layoutElement = (viewport != null) ? viewport.GetComponent<LayoutElement>() : null;
			if (layoutElement != null)
			{
				layoutElement.preferredHeight = (float)num * this.m_RowHeight + this.m_RowSpacing * (float)num - 1f;
			}
		}
	}

	// Token: 0x060028B2 RID: 10418 RVA: 0x0015BDA8 File Offset: 0x00159FA8
	private void RereshCapacityText()
	{
		if (this.m_Body == null)
		{
			return;
		}
		if (this.Data == null || this.Data.prisoners == null || this.Data.prisoners.Count == 0)
		{
			UIText.SetTextKey(this.m_Body, "RoyalDungeon.population.None", this.m_Vars, null);
			return;
		}
		float stat = this.Data.GetStat(Stats.ks_prison_capacity, false);
		float num = (float)this.Data.prisoners.Count / stat;
		if ((double)num <= 0.5)
		{
			UIText.SetTextKey(this.m_Body, "RoyalDungeon.population.Low", this.m_Vars, null);
			return;
		}
		if ((double)num > 0.5 && num <= 1f)
		{
			UIText.SetTextKey(this.m_Body, "RoyalDungeon.population.Medium", this.m_Vars, null);
			return;
		}
		UIText.SetTextKey(this.m_Body, "RoyalDungeon.population.High", this.m_Vars, null);
	}

	// Token: 0x060028B3 RID: 10419 RVA: 0x0015BE92 File Offset: 0x0015A092
	public void OnPrisonChanged()
	{
		this.RefreshPrisonerList();
		this.RereshCapacityText();
	}

	// Token: 0x060028B4 RID: 10420 RVA: 0x0015BEA0 File Offset: 0x0015A0A0
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "prison_changed" || message == "refresh_dungeon_ui_on_offer")
		{
			using (Game.Profile("UIRoyalDungeon OnMessage", false, 0f, null))
			{
				this.m_RefreshOnPrisonChanged = true;
			}
		}
		if (message == "destroying" || message == "finishing")
		{
			(obj as Logic.Object).DelListener(this);
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
	}

	// Token: 0x060028B5 RID: 10421 RVA: 0x0011FFF8 File Offset: 0x0011E1F8
	private void HandleOnCloseButtonCLick(BSGButton button)
	{
		this.Close(false);
	}

	// Token: 0x060028B6 RID: 10422 RVA: 0x0015BF34 File Offset: 0x0015A134
	protected override void OnDestroy()
	{
		if (UIRoyalDungeon.current == this)
		{
			UIRoyalDungeon.current = null;
		}
		if (this.Data != null)
		{
			this.Data.DelListener(this);
		}
		base.OnDestroy();
	}

	// Token: 0x060028B7 RID: 10423 RVA: 0x0015BF63 File Offset: 0x0015A163
	public static GameObject GetPrefab()
	{
		return UICommon.GetPrefab(UIRoyalDungeon.def_id, null);
	}

	// Token: 0x060028B8 RID: 10424 RVA: 0x0015BF70 File Offset: 0x0015A170
	public static void ToggleOpen(Logic.Kingdom prisoner)
	{
		if (prisoner == null)
		{
			if (UIRoyalDungeon.current != null)
			{
				UIRoyalDungeon.current.Close(false);
				UIRoyalDungeon.current = null;
			}
			return;
		}
		if (UIRoyalDungeon.current != null)
		{
			UIRoyalDungeon uiroyalDungeon = UIRoyalDungeon.current;
			if (((uiroyalDungeon != null) ? uiroyalDungeon.Data : null) == prisoner)
			{
				UIRoyalDungeon.current.Close(false);
				UIRoyalDungeon.current = null;
				return;
			}
			UIRoyalDungeon.current.SetData(prisoner);
			return;
		}
		else
		{
			WorldUI worldUI = WorldUI.Get();
			if (worldUI == null)
			{
				return;
			}
			GameObject prefab = UIRoyalDungeon.GetPrefab();
			if (prefab == null)
			{
				return;
			}
			GameObject gameObject = global::Common.FindChildByName(worldUI.gameObject, "id_MessageContainer", true, true);
			if (gameObject != null)
			{
				UICommon.DeleteChildren(gameObject.transform, typeof(UIRoyalDungeon));
				UIRoyalDungeon.current = UIRoyalDungeon.Create(prisoner, prefab, gameObject.transform as RectTransform);
				DT.Field soundsDef = BaseUI.soundsDef;
				BaseUI.PlaySoundEvent((soundsDef != null) ? soundsDef.GetString("open_royal_dungeon_window", null, "", true, true, true, '.') : null, null);
			}
			return;
		}
	}

	// Token: 0x060028B9 RID: 10425 RVA: 0x0015C06E File Offset: 0x0015A26E
	public static bool IsActive()
	{
		return UIRoyalDungeon.current != null;
	}

	// Token: 0x060028BA RID: 10426 RVA: 0x0015C07B File Offset: 0x0015A27B
	public static UIRoyalDungeon Create(Logic.Kingdom kingdom, GameObject prototype, RectTransform parent)
	{
		if (prototype == null)
		{
			return null;
		}
		if (kingdom == null)
		{
			return null;
		}
		if (parent == null)
		{
			return null;
		}
		UIRoyalDungeon orAddComponent = UnityEngine.Object.Instantiate<GameObject>(prototype, parent).GetOrAddComponent<UIRoyalDungeon>();
		orAddComponent.SetData(kingdom);
		orAddComponent.Open();
		return orAddComponent;
	}

	// Token: 0x04001B7C RID: 7036
	private static string def_id = "RoyalDungeonWindow";

	// Token: 0x04001B7D RID: 7037
	private const string PREFRED_CONTAINER = "id_MessageContainer";

	// Token: 0x04001B7E RID: 7038
	[UIFieldTarget("id_PrisonerPrototype")]
	private GameObject m_PrisonerPrototype;

	// Token: 0x04001B7F RID: 7039
	[UIFieldTarget("id_PrisonersContainer")]
	private RectTransform m_PriosnerContainer;

	// Token: 0x04001B80 RID: 7040
	[UIFieldTarget("id_GlobalActions")]
	private RectTransform m_GlobalActions;

	// Token: 0x04001B81 RID: 7041
	[UIFieldTarget("id_Body")]
	private TextMeshProUGUI m_Body;

	// Token: 0x04001B82 RID: 7042
	[UIFieldTarget("id_Caption")]
	private TextMeshProUGUI m_Caption;

	// Token: 0x04001B83 RID: 7043
	[UIFieldTarget("id_BodyScrollRect")]
	private ScrollRect m_BodyScrollRect;

	// Token: 0x04001B84 RID: 7044
	[UIFieldTarget("id_Button_Close")]
	private BSGButton m_Button_Close;

	// Token: 0x04001B86 RID: 7046
	private List<UIPrisoner> m_Prisoners = new List<UIPrisoner>();

	// Token: 0x04001B87 RID: 7047
	private UIRoyalDungeon.KingdomActions m_KingdomActions;

	// Token: 0x04001B88 RID: 7048
	private Vars m_Vars;

	// Token: 0x04001B89 RID: 7049
	private int m_MaxVisibleRows = 4;

	// Token: 0x04001B8A RID: 7050
	private float m_RowHeight = 80f;

	// Token: 0x04001B8B RID: 7051
	private float m_RowSpacing = 4f;

	// Token: 0x04001B8C RID: 7052
	private bool m_RefreshOnPrisonChanged;

	// Token: 0x04001B8D RID: 7053
	private static UIRoyalDungeon current;

	// Token: 0x020007E7 RID: 2023
	protected internal class KingdomActions : MonoBehaviour, IListener
	{
		// Token: 0x17000617 RID: 1559
		// (get) Token: 0x06004EDE RID: 20190 RVA: 0x002338FF File Offset: 0x00231AFF
		// (set) Token: 0x06004EDF RID: 20191 RVA: 0x00233907 File Offset: 0x00231B07
		public Logic.Kingdom Kingdom { get; private set; }

		// Token: 0x17000618 RID: 1560
		// (get) Token: 0x06004EE0 RID: 20192 RVA: 0x00233910 File Offset: 0x00231B10
		// (set) Token: 0x06004EE1 RID: 20193 RVA: 0x00233918 File Offset: 0x00231B18
		public Vars Vars { get; private set; }

		// Token: 0x06004EE2 RID: 20194 RVA: 0x00233921 File Offset: 0x00231B21
		public void SetData(Logic.Kingdom kingdom, Vars vars)
		{
			UICommon.FindComponents(this, false);
			this.Kingdom = kingdom;
			this.Vars = (vars ?? new Vars(kingdom));
			Logic.Kingdom kingdom2 = this.Kingdom;
			if (kingdom2 != null)
			{
				kingdom2.AddListener(this);
			}
			this.Refresh();
		}

		// Token: 0x06004EE3 RID: 20195 RVA: 0x00233960 File Offset: 0x00231B60
		private void Update()
		{
			if (this.Kingdom == null)
			{
				return;
			}
			if (this.m_ActiveActions != null && this.m_ActiveActions.Count > 0)
			{
				for (int i = 0; i < this.m_ActiveActions.Count; i++)
				{
					UIActionIcon uiactionIcon = this.m_ActiveActions[i];
					Action action;
					if (uiactionIcon == null)
					{
						action = null;
					}
					else
					{
						ActionVisuals data = uiactionIcon.Data;
						action = ((data != null) ? data.logic : null);
					}
					Action action2 = action;
					if (action2 != null && action2.target == null)
					{
						Action.State state = action2.state;
					}
				}
			}
		}

		// Token: 0x06004EE4 RID: 20196 RVA: 0x002339D9 File Offset: 0x00231BD9
		private void Refresh()
		{
			if (this.Kingdom == null)
			{
				return;
			}
			this.PopulateActions();
		}

		// Token: 0x06004EE5 RID: 20197 RVA: 0x002339EC File Offset: 0x00231BEC
		private void PopulateActions()
		{
			if (this.m_ActionsContainer == null)
			{
				return;
			}
			UICommon.DeleteChildren(this.m_ActionsContainer);
			this.m_ActiveActions.Clear();
			Logic.Kingdom kingdom = BaseUI.LogicKingdom();
			if (kingdom == null)
			{
				return;
			}
			if (kingdom.actions == null)
			{
				return;
			}
			for (int i = 0; i < kingdom.actions.Count; i++)
			{
				Action action = kingdom.actions[i];
				if (false | action is GlobalKillPrisonerAction | action is GlobalReleasePrisonerAction)
				{
					string a = action.Validate(false);
					if (a == "ok" || a == "_cooldown")
					{
						GameObject icon = ObjectIcon.GetIcon(action, this.Vars, this.m_ActionsContainer);
						if (icon != null)
						{
							UIActionIcon component = icon.GetComponent<UIActionIcon>();
							if (component != null)
							{
								this.m_ActiveActions.Add(component);
							}
						}
					}
				}
			}
		}

		// Token: 0x06004EE6 RID: 20198 RVA: 0x00233AD6 File Offset: 0x00231CD6
		private void OnDestroy()
		{
			Logic.Kingdom kingdom = this.Kingdom;
			if (kingdom == null)
			{
				return;
			}
			kingdom.DelListener(this);
		}

		// Token: 0x06004EE7 RID: 20199 RVA: 0x00233AE9 File Offset: 0x00231CE9
		public void OnMessage(object obj, string message, object param)
		{
			if (message == "destroying" || message == "finishing")
			{
				(obj as Logic.Object).DelListener(this);
				if (this != null)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
				return;
			}
		}

		// Token: 0x04003CE2 RID: 15586
		[UIFieldTarget("id_Actions")]
		private RectTransform m_ActionsContainer;

		// Token: 0x04003CE5 RID: 15589
		private List<UIActionIcon> m_ActiveActions = new List<UIActionIcon>(10);
	}
}
