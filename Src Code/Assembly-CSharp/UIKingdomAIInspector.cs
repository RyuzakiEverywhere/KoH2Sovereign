using System;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x0200020A RID: 522
public class UIKingdomAIInspector : MonoBehaviour
{
	// Token: 0x06001FC7 RID: 8135 RVA: 0x001256CA File Offset: 0x001238CA
	private void Start()
	{
		this.SetData(GameLogic.Get(false));
	}

	// Token: 0x06001FC8 RID: 8136 RVA: 0x001256D8 File Offset: 0x001238D8
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_Close != null)
		{
			this.m_Close.onClick = new BSGButton.OnClick(this.HandleClose);
		}
		this.m_Initialzied = true;
	}

	// Token: 0x06001FC9 RID: 8137 RVA: 0x00125716 File Offset: 0x00123916
	public void SetData(Game g)
	{
		this.Init();
		this.game = g;
		if (this.game == null)
		{
			return;
		}
		if (this.m_KigdomRowPrototype != null)
		{
			this.m_KigdomRowPrototype.gameObject.SetActive(false);
		}
		this.Refresh();
	}

	// Token: 0x06001FCA RID: 8138 RVA: 0x00125754 File Offset: 0x00123954
	private void Refresh()
	{
		if (this.game == null)
		{
			return;
		}
		for (int i = 0; i < this.game.kingdoms.Count; i++)
		{
			Logic.Kingdom kingdom = this.game.kingdoms[i];
			if (kingdom != null && kingdom.IsValid() && !kingdom.IsDefeated())
			{
				UIKingdomAIInspector.KingodmRow.Create(kingdom, this.m_KigdomRowPrototype, this.m_KindgomsContianer);
			}
		}
	}

	// Token: 0x06001FCB RID: 8139 RVA: 0x001257BD File Offset: 0x001239BD
	public static GameObject GetPrefab()
	{
		return UICommon.GetPrefab("KingdomAIInspector", null);
	}

	// Token: 0x06001FCC RID: 8140 RVA: 0x001257CA File Offset: 0x001239CA
	public static RectTransform GetDefaultContianer()
	{
		BaseUI baseUI = BaseUI.Get();
		return ((baseUI != null) ? baseUI.tCanvas : null) as RectTransform;
	}

	// Token: 0x06001FCD RID: 8141 RVA: 0x001257E4 File Offset: 0x001239E4
	public static void Show(bool show)
	{
		if (show && UIKingdomAIInspector.current != null)
		{
			return;
		}
		if (!show && UIKingdomAIInspector.current != null)
		{
			UnityEngine.Object.Destroy(UIKingdomAIInspector.current.gameObject);
			return;
		}
		if (show && UIKingdomAIInspector.current == null)
		{
			UIKingdomAIInspector.current = UIKingdomAIInspector.Create(UIKingdomAIInspector.GetPrefab(), UIKingdomAIInspector.GetDefaultContianer());
		}
	}

	// Token: 0x06001FCE RID: 8142 RVA: 0x00125846 File Offset: 0x00123A46
	private void HandleClose(BSGButton btn)
	{
		UIKingdomAIInspector.Show(false);
	}

	// Token: 0x06001FCF RID: 8143 RVA: 0x0012584E File Offset: 0x00123A4E
	public static UIKingdomAIInspector Create(GameObject prefab, RectTransform parent)
	{
		if (prefab == null)
		{
			return null;
		}
		if (parent == null)
		{
			return null;
		}
		return UnityEngine.Object.Instantiate<GameObject>(prefab, parent).GetOrAddComponent<UIKingdomAIInspector>();
	}

	// Token: 0x04001512 RID: 5394
	private Game game;

	// Token: 0x04001513 RID: 5395
	[UIFieldTarget("id_Close")]
	private BSGButton m_Close;

	// Token: 0x04001514 RID: 5396
	[UIFieldTarget("id_KigdomRowPrototype")]
	private GameObject m_KigdomRowPrototype;

	// Token: 0x04001515 RID: 5397
	[UIFieldTarget("id_KindgomsContianer")]
	private RectTransform m_KindgomsContianer;

	// Token: 0x04001516 RID: 5398
	private bool m_Initialzied;

	// Token: 0x04001517 RID: 5399
	private static UIKingdomAIInspector current;

	// Token: 0x02000747 RID: 1863
	internal class KingodmRow : MonoBehaviour
	{
		// Token: 0x170005A3 RID: 1443
		// (get) Token: 0x06004A5F RID: 19039 RVA: 0x002207B0 File Offset: 0x0021E9B0
		// (set) Token: 0x06004A60 RID: 19040 RVA: 0x002207B8 File Offset: 0x0021E9B8
		public Logic.Kingdom Data { get; private set; }

		// Token: 0x06004A61 RID: 19041 RVA: 0x002207C1 File Offset: 0x0021E9C1
		private void Init()
		{
			if (this.m_Initialzied)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			this.m_Initialzied = true;
		}

		// Token: 0x06004A62 RID: 19042 RVA: 0x002207DA File Offset: 0x0021E9DA
		public void SetData(Logic.Kingdom k)
		{
			this.Init();
			this.Data = k;
			this.Build();
		}

		// Token: 0x06004A63 RID: 19043 RVA: 0x002207EF File Offset: 0x0021E9EF
		private void Update()
		{
			if (this.Data != null)
			{
				this.Refresh();
			}
		}

		// Token: 0x06004A64 RID: 19044 RVA: 0x00220800 File Offset: 0x0021EA00
		private void Build()
		{
			if (this.m_KingdomIcon != null)
			{
				this.m_KingdomIcon.SetObject(this.Data, null);
			}
			if (this.m_KingdomName != null)
			{
				UIText.SetTextKey(this.m_KingdomName, (this.Data == null) ? "" : "Kingdom.name", new Vars(this.Data), null);
			}
			this.Refresh();
		}

		// Token: 0x06004A65 RID: 19045 RVA: 0x00220874 File Offset: 0x0021EA74
		private void Refresh()
		{
			if (this.Data == null)
			{
				return;
			}
			if (this.m_Expense_Last != null)
			{
				this.m_Expense_Last.text = this.Data.ai.last_expense.type.ToString();
			}
			if (this.m_Expense_Currrent != null)
			{
				this.m_Expense_Currrent.text = this.Data.ai.next_build_expense.ToString();
			}
		}

		// Token: 0x06004A66 RID: 19046 RVA: 0x002208F1 File Offset: 0x0021EAF1
		public static UIKingdomAIInspector.KingodmRow Create(Logic.Kingdom k, GameObject prefab, RectTransform parent)
		{
			if (k == null)
			{
				return null;
			}
			if (prefab == null)
			{
				return null;
			}
			if (parent == null)
			{
				return null;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab, parent);
			gameObject.gameObject.SetActive(true);
			UIKingdomAIInspector.KingodmRow orAddComponent = gameObject.GetOrAddComponent<UIKingdomAIInspector.KingodmRow>();
			orAddComponent.SetData(k);
			return orAddComponent;
		}

		// Token: 0x04003964 RID: 14692
		private bool m_Initialzied;

		// Token: 0x04003965 RID: 14693
		[UIFieldTarget("id_KingdomIcon")]
		private UIKingdomIcon m_KingdomIcon;

		// Token: 0x04003966 RID: 14694
		[UIFieldTarget("id_KingdomName")]
		private TextMeshProUGUI m_KingdomName;

		// Token: 0x04003967 RID: 14695
		[UIFieldTarget("id_Expense_Last")]
		private TextMeshProUGUI m_Expense_Last;

		// Token: 0x04003968 RID: 14696
		[UIFieldTarget("id_Expense_Currrent")]
		private TextMeshProUGUI m_Expense_Currrent;
	}
}
