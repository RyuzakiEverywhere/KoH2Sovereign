using System;
using System.Collections.Generic;
using System.Text;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000218 RID: 536
public class UIOpinionsWindow : UIWindow, IListener
{
	// Token: 0x0600207A RID: 8314 RVA: 0x00129591 File Offset: 0x00127791
	public override string GetDefId()
	{
		return UIOpinionsWindow.def_id;
	}

	// Token: 0x170001A2 RID: 418
	// (get) Token: 0x0600207B RID: 8315 RVA: 0x00129598 File Offset: 0x00127798
	// (set) Token: 0x0600207C RID: 8316 RVA: 0x001295A0 File Offset: 0x001277A0
	public Logic.Kingdom Data { get; private set; }

	// Token: 0x0600207D RID: 8317 RVA: 0x001295AC File Offset: 0x001277AC
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
				this.Button_Close[i].SetAudioSet("DefaultAudioSetPaper");
			}
		}
		if (this.m_OpinionInfoPrototype != null)
		{
			this.m_OpinionInfoPrototype.SetActive(false);
		}
		this.m_Initialized = true;
	}

	// Token: 0x0600207E RID: 8318 RVA: 0x00129630 File Offset: 0x00127830
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
		this.BuildInfoPanels();
		this.PopulateStatics();
		this.UpdateInfoPanels();
		this.Open();
	}

	// Token: 0x0600207F RID: 8319 RVA: 0x00129688 File Offset: 0x00127888
	private void BuildInfoPanels()
	{
		if (this.m_Panels != null && this.m_Panels.Count > 0)
		{
			return;
		}
		if (this.m_OpinionInfoPrototype == null)
		{
			return;
		}
		if (this.m_OpinionsContainer == null)
		{
			return;
		}
		List<Opinion.Def> defs = GameLogic.Get(true).defs.GetDefs<Opinion.Def>();
		for (int i = 0; i < defs.Count; i++)
		{
			UIOpinionsWindow.OpinionPanel orAddComponent = UnityEngine.Object.Instantiate<GameObject>(this.m_OpinionInfoPrototype, this.m_OpinionsContainer).GetOrAddComponent<UIOpinionsWindow.OpinionPanel>();
			this.m_Panels.Add(orAddComponent);
		}
	}

	// Token: 0x06002080 RID: 8320 RVA: 0x00129710 File Offset: 0x00127910
	private void UpdateInfoPanels()
	{
		Logic.Kingdom data = this.Data;
		List<Opinion> list;
		if (data == null)
		{
			list = null;
		}
		else
		{
			Opinions opinions = data.opinions;
			list = ((opinions != null) ? opinions.opinions : null);
		}
		List<Opinion> list2 = list;
		for (int i = 0; i < this.m_Panels.Count; i++)
		{
			Opinion opinion = (list2 != null && list2.Count > i) ? list2[i] : null;
			this.m_Panels[i].SetData(opinion);
			this.m_Panels[i].gameObject.SetActive(opinion != null);
		}
	}

	// Token: 0x06002081 RID: 8321 RVA: 0x000023FD File Offset: 0x000005FD
	public void SelectOpinion(Opinion o)
	{
	}

	// Token: 0x06002082 RID: 8322 RVA: 0x00129798 File Offset: 0x00127998
	private void PopulateStatics()
	{
		if (this.Data == null)
		{
			return;
		}
		if (this.m_Caption != null)
		{
			UIText.SetTextKey(this.m_Caption, "Opinions.caption", new Vars(this.Data), null);
		}
		if (this.m_Crest != null)
		{
			this.m_Crest.SetObject(this.Data.GetKingdom(), null);
		}
	}

	// Token: 0x06002083 RID: 8323 RVA: 0x00129802 File Offset: 0x00127A02
	public void OnMessage(object obj, string message, object param)
	{
		if (!(this == null) && !(base.gameObject == null))
		{
			return;
		}
		Logic.Kingdom data = this.Data;
		if (data == null)
		{
			return;
		}
		data.DelListener(this);
	}

	// Token: 0x06002084 RID: 8324 RVA: 0x0011796E File Offset: 0x00115B6E
	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	// Token: 0x06002085 RID: 8325 RVA: 0x0011FFF8 File Offset: 0x0011E1F8
	private void HandleOnClose(BSGButton button)
	{
		this.Close(false);
	}

	// Token: 0x06002086 RID: 8326 RVA: 0x0012982D File Offset: 0x00127A2D
	public static GameObject GetPrefab()
	{
		return UICommon.GetPrefab(UIOpinionsWindow.def_id, null);
	}

	// Token: 0x06002087 RID: 8327 RVA: 0x0012983C File Offset: 0x00127A3C
	public static void ToggleOpen(Logic.Kingdom k, Opinion o = null)
	{
		if (k == null)
		{
			if (UIOpinionsWindow.current != null)
			{
				UIOpinionsWindow.current.Close(false);
				UIOpinionsWindow.current = null;
			}
			return;
		}
		if (UIOpinionsWindow.current != null)
		{
			UIOpinionsWindow uiopinionsWindow = UIOpinionsWindow.current;
			Logic.Kingdom kingdom;
			if (uiopinionsWindow == null)
			{
				kingdom = null;
			}
			else
			{
				Logic.Kingdom data = uiopinionsWindow.Data;
				kingdom = ((data != null) ? data.GetKingdom() : null);
			}
			if (kingdom == k)
			{
				UIOpinionsWindow.current.Close(false);
				UIOpinionsWindow.current = null;
				return;
			}
			UIOpinionsWindow.ToggleOpen(null, null);
			UIOpinionsWindow.current.SetData(k);
			if (o != null)
			{
				UIOpinionsWindow.current.SelectOpinion(o);
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
			GameObject prefab = UIOpinionsWindow.GetPrefab();
			if (prefab == null)
			{
				return;
			}
			GameObject gameObject = global::Common.FindChildByName(worldUI.gameObject, "id_MessageContainer", true, true);
			if (gameObject != null)
			{
				UICommon.DeleteChildren(gameObject.transform, typeof(UIOpinionsWindow));
				UIOpinionsWindow.current = UIOpinionsWindow.Create(k, prefab, gameObject.transform as RectTransform);
				if (o != null)
				{
					UIOpinionsWindow.current.SelectOpinion(o);
				}
			}
			return;
		}
	}

	// Token: 0x06002088 RID: 8328 RVA: 0x00129942 File Offset: 0x00127B42
	public static UIOpinionsWindow Create(Logic.Kingdom k, GameObject prototype, RectTransform parent)
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
		UIOpinionsWindow orAddComponent = UnityEngine.Object.Instantiate<GameObject>(prototype, parent).GetOrAddComponent<UIOpinionsWindow>();
		orAddComponent.SetData(k);
		orAddComponent.Open();
		return orAddComponent;
	}

	// Token: 0x06002089 RID: 8329 RVA: 0x00129978 File Offset: 0x00127B78
	public static string GetModsText(Opinion opinion)
	{
		if (((opinion != null) ? opinion.mods : null) == null)
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < opinion.mods.Count; i++)
		{
			string modText = UIOpinionsWindow.GetModText(opinion.mods[i]);
			if (!string.IsNullOrEmpty(modText))
			{
				stringBuilder.AppendLine(modText);
			}
		}
		if (stringBuilder.Length == 0)
		{
			return null;
		}
		return "#" + stringBuilder.ToString();
	}

	// Token: 0x0600208A RID: 8330 RVA: 0x001299ED File Offset: 0x00127BED
	static UIOpinionsWindow()
	{
		Opinion.Def.get_mods_text = new Opinion.Def.GetModsTextFunc(UIOpinionsWindow.GetModsText);
	}

	// Token: 0x0600208B RID: 8331 RVA: 0x00129A0C File Offset: 0x00127C0C
	public static string GetModText(Opinion.StatModifier mod)
	{
		bool flag;
		mod.value = mod.def.CalcValue(mod.opinion, out flag);
		if (!flag)
		{
			return null;
		}
		DT.Field field = mod.def.field.FindChild("tooltip", null, true, true, true, '.');
		if (field != null)
		{
			return global::Defs.Localize(field, mod, null, false, true);
		}
		return global::Defs.LocalizeStatModifier("KingdomStats", mod.def.stat_name, mod.value, mod.def.type, mod, false, false);
	}

	// Token: 0x040015A5 RID: 5541
	private static string def_id = "OpinionsWindow";

	// Token: 0x040015A6 RID: 5542
	[UIFieldTarget("id_Button_Close", true)]
	private BSGButton[] Button_Close;

	// Token: 0x040015A7 RID: 5543
	[UIFieldTarget("id_Crest")]
	private UIKingdomIcon m_Crest;

	// Token: 0x040015A8 RID: 5544
	[UIFieldTarget("id_Caption")]
	private TextMeshProUGUI m_Caption;

	// Token: 0x040015A9 RID: 5545
	[UIFieldTarget("id_OpinionsContainer")]
	private RectTransform m_OpinionsContainer;

	// Token: 0x040015AA RID: 5546
	[UIFieldTarget("id_OpinionInfoPrototype")]
	private GameObject m_OpinionInfoPrototype;

	// Token: 0x040015AC RID: 5548
	private List<UIOpinionsWindow.OpinionPanel> m_Panels = new List<UIOpinionsWindow.OpinionPanel>();

	// Token: 0x040015AD RID: 5549
	private static UIOpinionsWindow current;

	// Token: 0x02000757 RID: 1879
	internal class OpinionPanel : MonoBehaviour
	{
		// Token: 0x170005B2 RID: 1458
		// (get) Token: 0x06004AF5 RID: 19189 RVA: 0x00223013 File Offset: 0x00221213
		// (set) Token: 0x06004AF6 RID: 19190 RVA: 0x0022301B File Offset: 0x0022121B
		public Opinion Data { get; private set; }

		// Token: 0x06004AF7 RID: 19191 RVA: 0x00223024 File Offset: 0x00221224
		public void SetData(Opinion opinion)
		{
			this.Init();
			this.Data = opinion;
			this.Refresh();
		}

		// Token: 0x06004AF8 RID: 19192 RVA: 0x00223039 File Offset: 0x00221239
		private void Init()
		{
			if (this.m_Initialzied)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			this.m_Initialzied = true;
		}

		// Token: 0x06004AF9 RID: 19193 RVA: 0x00223054 File Offset: 0x00221254
		private void Refresh()
		{
			if (this.m_Illustration != null)
			{
				this.m_Illustration.sprite = global::Defs.GetObj<Sprite>(this.Data.def.field, "illustration", this.Data.kingdom);
			}
			if (this.m_Caption != null)
			{
				UIText.SetText(this.m_Caption, global::Defs.Localize(this.Data.def.field, "name", this.Data.kingdom, null, true, true));
			}
		}

		// Token: 0x06004AFA RID: 19194 RVA: 0x002230E0 File Offset: 0x002212E0
		private void UpdateDynamics()
		{
			if (this.m_Effects != null)
			{
				UIText.SetText(this.m_Effects, global::Defs.Localize(UIOpinionsWindow.GetModsText(this.Data), null, null, true, true));
			}
			if (this.m_OpinionValue != null)
			{
				this.m_OpinionValue.text = Mathf.RoundToInt(this.Data.value).ToString();
			}
		}

		// Token: 0x06004AFB RID: 19195 RVA: 0x0022314B File Offset: 0x0022134B
		private void Update()
		{
			this.UpdateDynamics();
		}

		// Token: 0x040039CC RID: 14796
		[UIFieldTarget("id_Illustration")]
		private Image m_Illustration;

		// Token: 0x040039CD RID: 14797
		[UIFieldTarget("id_Caption")]
		private TextMeshProUGUI m_Caption;

		// Token: 0x040039CE RID: 14798
		[UIFieldTarget("id_Effects")]
		private TextMeshProUGUI m_Effects;

		// Token: 0x040039CF RID: 14799
		[UIFieldTarget("id_OpinionValue")]
		private TextMeshProUGUI m_OpinionValue;

		// Token: 0x040039D1 RID: 14801
		private bool m_Initialzied;
	}
}
