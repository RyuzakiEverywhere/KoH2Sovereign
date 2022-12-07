using System;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020002B9 RID: 697
public class UIRealmIcon : ObjectIcon, IListener
{
	// Token: 0x17000228 RID: 552
	// (get) Token: 0x06002BB2 RID: 11186 RVA: 0x00170096 File Offset: 0x0016E296
	// (set) Token: 0x06002BB3 RID: 11187 RVA: 0x0017009E File Offset: 0x0016E29E
	public Logic.Realm Data { get; private set; }

	// Token: 0x17000229 RID: 553
	// (get) Token: 0x06002BB4 RID: 11188 RVA: 0x001700A7 File Offset: 0x0016E2A7
	// (set) Token: 0x06002BB5 RID: 11189 RVA: 0x001700AF File Offset: 0x0016E2AF
	public Logic.Realm CompareTarget { get; private set; }

	// Token: 0x1400003A RID: 58
	// (add) Token: 0x06002BB6 RID: 11190 RVA: 0x001700B8 File Offset: 0x0016E2B8
	// (remove) Token: 0x06002BB7 RID: 11191 RVA: 0x001700F0 File Offset: 0x0016E2F0
	public event Action<UIRealmIcon> OnSelect;

	// Token: 0x1700022A RID: 554
	// (get) Token: 0x06002BB8 RID: 11192 RVA: 0x00170125 File Offset: 0x0016E325
	// (set) Token: 0x06002BB9 RID: 11193 RVA: 0x0017012D File Offset: 0x0016E32D
	public UIRealmIcon.State state { get; private set; }

	// Token: 0x1700022B RID: 555
	// (get) Token: 0x06002BBA RID: 11194 RVA: 0x00170136 File Offset: 0x0016E336
	// (set) Token: 0x06002BBB RID: 11195 RVA: 0x0017013E File Offset: 0x0016E33E
	public DT.Field state_def { get; private set; }

	// Token: 0x1700022C RID: 556
	// (get) Token: 0x06002BBC RID: 11196 RVA: 0x00170147 File Offset: 0x0016E347
	// (set) Token: 0x06002BBD RID: 11197 RVA: 0x0017014F File Offset: 0x0016E34F
	public DT.Field icon_def { get; private set; }

	// Token: 0x06002BBE RID: 11198 RVA: 0x00170158 File Offset: 0x0016E358
	private void Init()
	{
		if (this.m_initialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_initialized = false;
	}

	// Token: 0x06002BBF RID: 11199 RVA: 0x00170171 File Offset: 0x0016E371
	private void OnDestroy()
	{
		if (this.Data != null)
		{
			this.Data.DelListener(this);
			Logic.Realm data = this.Data;
			if (data != null)
			{
				data.castle.DelListener(this);
			}
		}
		this.OnSelect = null;
	}

	// Token: 0x06002BC0 RID: 11200 RVA: 0x001701A8 File Offset: 0x0016E3A8
	public override void SetObject(object obj, Vars vars = null)
	{
		this.Init();
		base.SetObject(obj, vars);
		if (this.Data != null)
		{
			this.Data.DelListener(this);
			Logic.Realm data = this.Data;
			if (data != null)
			{
				data.castle.DelListener(this);
			}
		}
		if (this.logicObject != null)
		{
			Tooltip.Get(base.gameObject, true).SetObj(obj, null, null);
			if (obj is Logic.Realm)
			{
				this.Data = (obj as Logic.Realm);
				this.Data.AddListener(this);
				Logic.Realm data2 = this.Data;
				if (data2 != null)
				{
					data2.castle.AddListener(this);
				}
			}
		}
		else
		{
			this.Data = null;
		}
		this.Refresh();
	}

	// Token: 0x06002BC1 RID: 11201 RVA: 0x00170250 File Offset: 0x0016E450
	private void Update()
	{
		if (this.Data == null)
		{
			base.enabled = false;
			return;
		}
		this.UpdateState();
	}

	// Token: 0x06002BC2 RID: 11202 RVA: 0x00170268 File Offset: 0x0016E468
	public void UpdateState()
	{
		UIRealmIcon.State state = this.DecideState();
		this.SetState(state);
		this.UpdateIcon();
	}

	// Token: 0x06002BC3 RID: 11203 RVA: 0x0017028C File Offset: 0x0016E48C
	public void SetState(UIRealmIcon.State state)
	{
		if (this.state == state && this.state_def != null)
		{
			return;
		}
		this.state = state;
		if (this.icon_def == null)
		{
			this.icon_def = global::Defs.GetDefField("RealmIcon", null);
		}
		if (this.icon_def != null)
		{
			this.state_def = this.icon_def.FindChild(state.ToString(), null, true, true, true, '.');
			if (this.state_def == null)
			{
				Debug.LogWarning(string.Format("{0}: undefined state '{1}'", this, state));
				this.state_def = this.icon_def.FindChild("State", null, true, true, true, '.');
				return;
			}
		}
		else
		{
			this.state_def = null;
		}
	}

	// Token: 0x06002BC4 RID: 11204 RVA: 0x0017033C File Offset: 0x0016E53C
	public UIRealmIcon.State DecideState()
	{
		Logic.Realm data = this.Data;
		if (data == null)
		{
			return UIRealmIcon.State.Normal;
		}
		bool flag;
		if (data == null)
		{
			flag = (null != null);
		}
		else
		{
			Castle castle = data.castle;
			flag = (((castle != null) ? castle.battle : null) != null);
		}
		if (flag)
		{
			return UIRealmIcon.State.Siege;
		}
		if (data.IsDisorder())
		{
			return UIRealmIcon.State.Disorder;
		}
		if (data.IsOccupied())
		{
			return UIRealmIcon.State.Occupation;
		}
		return UIRealmIcon.State.Normal;
	}

	// Token: 0x06002BC5 RID: 11205 RVA: 0x00170386 File Offset: 0x0016E586
	public void ShowCrest(bool shown)
	{
		if (this.m_showCrest == shown)
		{
			return;
		}
		this.m_showCrest = shown;
		this.UpdateCrest();
	}

	// Token: 0x06002BC6 RID: 11206 RVA: 0x0017039F File Offset: 0x0016E59F
	public void ShowKingdomCrest(bool shown)
	{
		if (this.m_showKingdomCrest == shown)
		{
			return;
		}
		this.m_showKingdomCrest = shown;
		this.UpdateCrest();
	}

	// Token: 0x06002BC7 RID: 11207 RVA: 0x001703B8 File Offset: 0x0016E5B8
	public void SetCompareTarget(Logic.Realm realm)
	{
		this.CompareTarget = realm;
		this.Refresh();
	}

	// Token: 0x06002BC8 RID: 11208 RVA: 0x001703C7 File Offset: 0x0016E5C7
	private void Refresh()
	{
		if (this.Data == null)
		{
			return;
		}
		this.UpdateIcon();
		this.UpdateCrest();
		this.UpdateReligion();
		this.UpdateHighlight();
		this.UpdateLevel();
		this.UpdateNavalLink();
		this.UpdateStance();
		this.UpdateState();
	}

	// Token: 0x06002BC9 RID: 11209 RVA: 0x00170404 File Offset: 0x0016E604
	private void UpdateIcon()
	{
		if (this.state_def == null)
		{
			return;
		}
		int num = Mathf.Max(this.Data.castle.GetCitadelLevel(), 1);
		if (this.m_Icon != null)
		{
			Sprite obj = global::Defs.GetObj<Sprite>(num - 1, this.icon_def, "icon", null);
			if (obj != null)
			{
				this.m_Icon.overrideSprite = obj;
			}
			else
			{
				this.m_Icon.overrideSprite = null;
			}
			this.m_Icon.color = global::Defs.GetColor(this.state_def, "icon_color", null);
		}
		if (this.m_Border != null)
		{
			Sprite obj2 = global::Defs.GetObj<Sprite>(this.state_def, "border", null);
			if (obj2 != null)
			{
				this.m_Border.overrideSprite = obj2;
			}
			else
			{
				this.m_Border.overrideSprite = null;
			}
			this.m_Border.color = global::Defs.GetColor(this.state_def, "border_color", null);
		}
	}

	// Token: 0x06002BCA RID: 11210 RVA: 0x001704F4 File Offset: 0x0016E6F4
	private void UpdateCrest()
	{
		if (this.Data == null)
		{
			return;
		}
		if (this.m_Crest == null)
		{
			return;
		}
		if (!this.m_showCrest)
		{
			this.m_Crest.gameObject.SetActive(false);
			return;
		}
		this.m_Crest.gameObject.SetActive(true);
		int id;
		if (this.m_showKingdomCrest)
		{
			id = this.Data.GetKingdom().id;
		}
		else
		{
			Logic.Kingdom kingdom = this.Data.game.GetKingdom(this.Data.name);
			id = ((kingdom != null) ? kingdom.id : this.Data.GetKingdom().id);
		}
		global::Kingdom kingdom2 = global::Kingdom.Get(id);
		if (kingdom2 != null)
		{
			this.m_Crest.SetObject(kingdom2.logic, null);
		}
		this.m_Crest.gameObject.SetActive(!this.m_showKingdomCrest || BaseUI.LogicKingdom().id != kingdom2.id);
	}

	// Token: 0x06002BCB RID: 11211 RVA: 0x001705E4 File Offset: 0x0016E7E4
	private void UpdateReligion()
	{
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		bool flag = this.Data.religion != kingdom.religion;
		if (this.m_Religion != null)
		{
			this.m_Religion.gameObject.SetActive(flag);
		}
		if (flag && this.m_ReligionIcon != null)
		{
			this.m_ReligionIcon.overrideSprite = global::Defs.GetObj<Sprite>(this.Data.religion.def.field, "icon", null);
		}
	}

	// Token: 0x06002BCC RID: 11212 RVA: 0x0017066C File Offset: 0x0016E86C
	private void UpdateLevel()
	{
		if (this.Data == null)
		{
			return;
		}
		if (this.m_LevelValue != null)
		{
			UIText.SetText(this.m_LevelValue, this.CalcLevelValue(this.Data).ToString());
			Logic.Kingdom kingdom = BaseUI.LogicKingdom();
			this.m_LevelValue.color = UICommon.GetRelationColor(kingdom.GetRelationship(this.Data.GetKingdom()));
		}
	}

	// Token: 0x06002BCD RID: 11213 RVA: 0x001706D6 File Offset: 0x0016E8D6
	private int CalcLevelValue(Logic.Realm r)
	{
		if (r == null)
		{
			return 0;
		}
		if (r.castle == null)
		{
			return 0;
		}
		return 1 + r.castle.NumBuildings(false, false);
	}

	// Token: 0x06002BCE RID: 11214 RVA: 0x001706F8 File Offset: 0x0016E8F8
	private void UpdateNavalLink()
	{
		if (this.m_NavalLink == null)
		{
			return;
		}
		if (this.Data == null)
		{
			return;
		}
		if (this.CompareTarget == null)
		{
			this.m_NavalLink.gameObject.SetActive(false);
			return;
		}
		this.m_NavalLink.gameObject.SetActive(this.CompareTarget.HasNeighborThroughSea(this.Data));
	}

	// Token: 0x06002BCF RID: 11215 RVA: 0x00170758 File Offset: 0x0016E958
	private void UpdateStance()
	{
		if (this.m_Stance == null)
		{
			return;
		}
		if (this.Data == null || this.CompareTarget == null || this.Data == this.CompareTarget)
		{
			this.m_Stance.gameObject.SetActive(false);
			return;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		Logic.Kingdom kingdom2 = this.Data.GetKingdom();
		if (kingdom == kingdom2)
		{
			this.m_Stance.gameObject.SetActive(false);
			return;
		}
		this.m_Stance.gameObject.SetActive(true);
		RelationUtils.Stance warStance = kingdom.GetWarStance(kingdom2);
		this.m_Stance.color = global::Defs.GetColor("Kingdom", "Stance." + warStance.ToString() + ".color");
	}

	// Token: 0x06002BD0 RID: 11216 RVA: 0x00170816 File Offset: 0x0016EA16
	public void Select(bool selected)
	{
		if (this.m_Selected == selected)
		{
			return;
		}
		this.m_Selected = selected;
		this.UpdateHighlight();
	}

	// Token: 0x06002BD1 RID: 11217 RVA: 0x0017082F File Offset: 0x0016EA2F
	public override void OnClick(PointerEventData e)
	{
		base.OnClick(e);
		if (this.OnSelect != null)
		{
			this.OnSelect(this);
			return;
		}
		if (this.Data == null)
		{
			return;
		}
		if (e.button == PointerEventData.InputButton.Left)
		{
			this.SelectRealm(e.clickCount > 1);
		}
	}

	// Token: 0x06002BD2 RID: 11218 RVA: 0x00170870 File Offset: 0x0016EA70
	private void SelectRealm(bool focus = false)
	{
		if (this.Data == null)
		{
			return;
		}
		if (this.Data.settlements == null)
		{
			return;
		}
		if (this.Data.settlements.Count == 0)
		{
			return;
		}
		Logic.Settlement settlement = this.Data.settlements[0];
		if (settlement == null)
		{
			return;
		}
		global::Settlement settlement2 = settlement.visuals as global::Settlement;
		if (settlement2 == null)
		{
			return;
		}
		WorldUI worldUI = WorldUI.Get();
		if (worldUI == null)
		{
			return;
		}
		worldUI.SelectObj(settlement2.gameObject, false, true, true, true);
		if (focus)
		{
			worldUI.LookAt(settlement2.gameObject.transform.position, false);
		}
	}

	// Token: 0x06002BD3 RID: 11219 RVA: 0x0017090D File Offset: 0x0016EB0D
	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x06002BD4 RID: 11220 RVA: 0x0017091C File Offset: 0x0016EB1C
	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x06002BD5 RID: 11221 RVA: 0x000DB26E File Offset: 0x000D946E
	public void UpdateHighlight()
	{
		bool isPlaying = Application.isPlaying;
	}

	// Token: 0x06002BD6 RID: 11222 RVA: 0x0017092B File Offset: 0x0016EB2B
	void IListener.OnMessage(object obj, string message, object param)
	{
		if (message == "religion_changed" || message == "kingdom_changed" || message == "structures_changed")
		{
			this.Refresh();
			return;
		}
	}

	// Token: 0x04001DBC RID: 7612
	[UIFieldTarget("id_Icon")]
	private Image m_Icon;

	// Token: 0x04001DBD RID: 7613
	[UIFieldTarget("id_Border")]
	private Image m_Border;

	// Token: 0x04001DBE RID: 7614
	[UIFieldTarget("id_Background")]
	private Image m_Background;

	// Token: 0x04001DBF RID: 7615
	[UIFieldTarget("id_Crest")]
	private UIKingdomIcon m_Crest;

	// Token: 0x04001DC0 RID: 7616
	[UIFieldTarget("id_LevelValue")]
	private TextMeshProUGUI m_LevelValue;

	// Token: 0x04001DC1 RID: 7617
	[UIFieldTarget("id_NavalLink")]
	private GameObject m_NavalLink;

	// Token: 0x04001DC2 RID: 7618
	[UIFieldTarget("id_Religion")]
	private GameObject m_Religion;

	// Token: 0x04001DC3 RID: 7619
	[UIFieldTarget("id_ReligionIcon")]
	private Image m_ReligionIcon;

	// Token: 0x04001DC4 RID: 7620
	[UIFieldTarget("id_Stance")]
	private Image m_Stance;

	// Token: 0x04001DCB RID: 7627
	private bool m_showKingdomCrest = true;

	// Token: 0x04001DCC RID: 7628
	private bool m_showCrest = true;

	// Token: 0x04001DCD RID: 7629
	private bool m_Selected;

	// Token: 0x04001DCE RID: 7630
	private bool m_initialized;

	// Token: 0x02000813 RID: 2067
	public enum State
	{
		// Token: 0x04003DA6 RID: 15782
		Normal,
		// Token: 0x04003DA7 RID: 15783
		Siege,
		// Token: 0x04003DA8 RID: 15784
		Disorder,
		// Token: 0x04003DA9 RID: 15785
		Occupation
	}
}
