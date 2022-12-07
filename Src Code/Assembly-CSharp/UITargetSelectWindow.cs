using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Token: 0x020002E1 RID: 737
public class UITargetSelectWindow : UIWindow
{
	// Token: 0x06002EA3 RID: 11939 RVA: 0x00180D06 File Offset: 0x0017EF06
	public override string GetDefId()
	{
		return UITargetSelectWindow.wnd_def_id;
	}

	// Token: 0x1700023F RID: 575
	// (get) Token: 0x06002EA4 RID: 11940 RVA: 0x00180D0D File Offset: 0x0017EF0D
	public static UITargetSelectWindow Instance
	{
		get
		{
			if (UITargetSelectWindow.sm_Instance == null || !UITargetSelectWindow.ValidScene())
			{
				UITargetSelectWindow.LoadPrefab();
			}
			return UITargetSelectWindow.sm_Instance;
		}
	}

	// Token: 0x06002EA5 RID: 11941 RVA: 0x00180D30 File Offset: 0x0017EF30
	protected override void Awake()
	{
		base.Awake();
		this.Show(false);
		UICommon.FindComponents(this, false);
		if (this.Button_ConfirmSelect != null)
		{
			this.Button_ConfirmSelect.onClick = new BSGButton.OnClick(this.HandleConfirm);
			this.Button_ConfirmSelect.Enable(false, false);
		}
		if (this.Button_Cancel != null)
		{
			this.Button_Cancel.onClick = new BSGButton.OnClick(this.HandleCancel);
		}
	}

	// Token: 0x06002EA6 RID: 11942 RVA: 0x00180DA8 File Offset: 0x0017EFA8
	public override void Show(bool show)
	{
		if (UITargetSelectWindow.in_show)
		{
			return;
		}
		UITargetSelectWindow.in_show = true;
		base.Show(show);
		UITargetSelectWindow.in_show = false;
		if (!show)
		{
			this.ValidateMethod = null;
		}
		if (show)
		{
			this.SetupConfirmButton();
		}
	}

	// Token: 0x06002EA7 RID: 11943 RVA: 0x00180DD8 File Offset: 0x0017EFD8
	public override void Show()
	{
		try
		{
			base.gameObject.SetActive(true);
			base.gameObject.transform.SetAsLastSibling();
			base.Show();
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
	}

	// Token: 0x06002EA8 RID: 11944 RVA: 0x00180E20 File Offset: 0x0017F020
	public override void Hide(bool silent = false)
	{
		try
		{
			this.Clean();
			base.gameObject.SetActive(false);
			base.Hide(silent);
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
		this.ClearValidateCallbacks();
	}

	// Token: 0x06002EA9 RID: 11945 RVA: 0x00180E68 File Offset: 0x0017F068
	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (UITargetSelectWindow.sm_Instance == this)
		{
			UITargetSelectWindow.sm_Instance = null;
		}
		this.ValidateMethod = null;
	}

	// Token: 0x06002EAA RID: 11946 RVA: 0x00180E8C File Offset: 0x0017F08C
	private static bool ValidScene()
	{
		if (UITargetSelectWindow.sm_Instance == null)
		{
			return true;
		}
		Scene scene = UITargetSelectWindow.sm_Instance.gameObject.scene;
		Scene activeScene = SceneManager.GetActiveScene();
		return scene == activeScene;
	}

	// Token: 0x06002EAB RID: 11947 RVA: 0x00180EC3 File Offset: 0x0017F0C3
	public override bool OnBackInputAction()
	{
		this.Show(false);
		return true;
	}

	// Token: 0x06002EAC RID: 11948 RVA: 0x00180ED0 File Offset: 0x0017F0D0
	private void Clean()
	{
		if (this.items != null && this.items.Count > 0)
		{
			for (int i = 0; i < this.items.Count; i++)
			{
				this.items[i].Release();
			}
		}
		this.CallOnCancel();
		this.OnSelectObject = null;
		this.OnCancel = null;
		this.m_CurrentSelectedItem = null;
		if (this.Button_ConfirmSelect != null)
		{
			this.Button_ConfirmSelect.Enable(false, false);
		}
		this.items.Clear();
		UICommon.DeleteChildren(this.Container);
	}

	// Token: 0x06002EAD RID: 11949 RVA: 0x00180F66 File Offset: 0x0017F166
	public void ClearValidateCallbacks()
	{
		this.OnCancel = null;
		this.ValidateMethod = null;
		this.ValidateOkButttonMethod = null;
		this.SetupOkButttonMethod = null;
	}

	// Token: 0x06002EAE RID: 11950 RVA: 0x00180F84 File Offset: 0x0017F184
	public static bool ShowDialog(List<TargetPickerData> eligableTargets, Value suggestedValue, Action<Value> Select, Action Cancel, Vars additionalData = null, Func<bool> validate = null, Func<Value, bool> validateOkButtton = null, Action<BSGButton> setupOkButtton = null, string def_id = "")
	{
		if (UITargetSelectWindow.sm_Instance == null || !UITargetSelectWindow.ValidScene())
		{
			UITargetSelectWindow.LoadPrefab();
		}
		if (UITargetSelectWindow.sm_Instance != null)
		{
			if (UITargetSelectWindow.sm_Instance.gameObject.activeSelf)
			{
				UITargetSelectWindow.sm_Instance.HandleCancel(null);
			}
			if (UITargetSelectWindow.sm_Instance.SetData(eligableTargets, suggestedValue, Select, Cancel, additionalData, def_id, ""))
			{
				UITargetSelectWindow.sm_Instance.ValidateMethod = validate;
				UITargetSelectWindow.sm_Instance.ValidateOkButttonMethod = validateOkButtton;
				UITargetSelectWindow.sm_Instance.SetupOkButttonMethod = setupOkButtton;
				UITargetSelectWindow.sm_Instance.Show(true);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002EAF RID: 11951 RVA: 0x00181020 File Offset: 0x0017F220
	public static bool ShowDialog(List<TargetPickerData> eligableTargets, Logic.Object suggestedObject, Action<Value> Select, Action Cancel, Vars additionalData = null, Func<bool> validate = null, Func<Value, bool> validateOkButtton = null, Action<BSGButton> setupOkButtton = null, string def_id = "", string table_def_id = "")
	{
		if (UITargetSelectWindow.sm_Instance == null || !UITargetSelectWindow.ValidScene())
		{
			UITargetSelectWindow.LoadPrefab();
		}
		if (suggestedObject == null)
		{
			suggestedObject = BaseUI.SelLO();
		}
		if (UITargetSelectWindow.sm_Instance != null)
		{
			if (UITargetSelectWindow.sm_Instance.gameObject.activeSelf)
			{
				UITargetSelectWindow.sm_Instance.HandleCancel(null);
			}
			if (UITargetSelectWindow.sm_Instance.SetData(eligableTargets, suggestedObject, Select, Cancel, additionalData, def_id, table_def_id))
			{
				UITargetSelectWindow.sm_Instance.ValidateMethod = validate;
				UITargetSelectWindow.sm_Instance.ValidateOkButttonMethod = validateOkButtton;
				UITargetSelectWindow.sm_Instance.SetupOkButttonMethod = setupOkButtton;
				UITargetSelectWindow.sm_Instance.Show(true);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002EB0 RID: 11952 RVA: 0x001810C8 File Offset: 0x0017F2C8
	private static void LoadPrefab()
	{
		BaseUI baseUI = BaseUI.Get();
		if (baseUI != null)
		{
			GameObject gameObject = global::Common.FindChildByName(baseUI.tCanvas.gameObject, "id_MessageContainer", true, true);
			if (gameObject == null)
			{
				gameObject = baseUI.tCanvas.gameObject;
			}
			GameObject gameObject2 = UICommon.LoadPrefab(UITargetSelectWindow.wnd_def_id, gameObject.transform);
			if (UITargetSelectWindow.sm_Instance != null)
			{
				global::Common.DestroyObj(UITargetSelectWindow.sm_Instance.gameObject);
			}
			UITargetSelectWindow.sm_Instance = gameObject2.GetComponent<UITargetSelectWindow>();
		}
	}

	// Token: 0x06002EB1 RID: 11953 RVA: 0x00181148 File Offset: 0x0017F348
	public bool Match(UITargetItem item, Logic.Object obj)
	{
		if (item == null || obj == null)
		{
			return false;
		}
		if (item.Data.Target.obj_val == obj)
		{
			return true;
		}
		object obj_val = item.Data.Target.obj_val;
		if (obj_val != null)
		{
			Logic.Kingdom kingdom;
			if ((kingdom = (obj_val as Logic.Kingdom)) == null)
			{
				Logic.Realm realm;
				if ((realm = (obj_val as Logic.Realm)) == null)
				{
					Logic.Character character;
					if ((character = (obj_val as Logic.Character)) != null)
					{
						Logic.Character character2 = character;
						Logic.Army army = obj as Logic.Army;
						if (((army != null) ? army.leader : null) == character2)
						{
							return true;
						}
					}
				}
				else
				{
					Logic.Realm realm2 = realm;
					Logic.Settlement settlement = obj as Logic.Settlement;
					if (((settlement != null) ? settlement.GetRealm() : null) == realm2)
					{
						return true;
					}
					Logic.Army army2 = obj as Logic.Army;
					if (((army2 != null) ? army2.realm_in : null) == realm2)
					{
						return true;
					}
				}
			}
			else
			{
				Logic.Kingdom kingdom2 = kingdom;
				if (obj.GetKingdom() == kingdom2)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06002EB2 RID: 11954 RVA: 0x0002C538 File Offset: 0x0002A738
	public bool SelectValue(Value val)
	{
		return false;
	}

	// Token: 0x06002EB3 RID: 11955 RVA: 0x0018120C File Offset: 0x0017F40C
	public bool SelectLO(Logic.Object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (this.Match(this.m_CurrentSelectedItem, obj))
		{
			this.SelectItem(this.m_CurrentSelectedItem);
			return true;
		}
		for (int i = 0; i < this.items.Count; i++)
		{
			UITargetItem item = this.items[i];
			if (this.Match(item, obj))
			{
				this.SelectItem(item);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002EB4 RID: 11956 RVA: 0x00181274 File Offset: 0x0017F474
	private void BringSelectionIntoView()
	{
		if (this.m_ScrollRect == null)
		{
			return;
		}
		if (this.m_CurrentSelectedItem == null)
		{
			return;
		}
		RectTransform viewport = this.m_ScrollRect.viewport;
		Transform transform = this.m_CurrentSelectedItem.transform as RectTransform;
		Vector3 localPosition = this.m_ScrollRect.content.localPosition;
		float height = viewport.rect.height;
		float y = transform.localPosition.y;
		float num = viewport.localPosition.y + y + localPosition.y;
		if (num >= -height / 2f && num <= height / 2f)
		{
			return;
		}
		localPosition.y = 0f - viewport.localPosition.y - y;
		this.m_ScrollRect.content.localPosition = localPosition;
	}

	// Token: 0x06002EB5 RID: 11957 RVA: 0x00181340 File Offset: 0x0017F540
	public bool SetData(List<TargetPickerData> eligableTargets, Value suggestedValue, Action<Value> Select, Action Cancel, Vars vars = null, string def_id = "TargetSelect", string table_def_id = "")
	{
		this.m_Vars = vars;
		if (eligableTargets == null)
		{
			return false;
		}
		if (this.ItemProrotype == null)
		{
			Debug.Log("UITargetSelectWindow fail init! Reason NO item Prototype provided");
			return false;
		}
		this.PopulateTexts(def_id);
		this.temItemList.Clear();
		this.reusable.Clear();
		this.temItemList.AddRange(this.items);
		for (int i = 0; i < eligableTargets.Count; i++)
		{
			TargetPickerData targetPickerData = eligableTargets[i];
			if (this.items == null || i >= this.items.Count)
			{
				break;
			}
			UITargetItem uitargetItem = this.items[i];
			for (int j = 0; j < this.items.Count; j++)
			{
				if (uitargetItem.MatchData(targetPickerData))
				{
					this.reusable.Add(targetPickerData, uitargetItem);
					this.temItemList.Remove(uitargetItem);
					break;
				}
			}
		}
		this.items.Clear();
		for (int k = 0; k < eligableTargets.Count; k++)
		{
			TargetPickerData key = eligableTargets[k];
			if (this.reusable.ContainsKey(key))
			{
				UITargetItem uitargetItem2 = this.reusable[key];
				uitargetItem2.OnSelect = new Action<UITargetItem>(this.SelectItem);
				uitargetItem2.OnConfirm = new Action<UITargetItem>(this.SelectAndConfirmItem);
				this.items.Add(uitargetItem2);
			}
			else
			{
				UITargetItem uitargetItem3 = UITargetItem.Create(eligableTargets[k], this.ItemProrotype, this.Container, eligableTargets[k].Vars);
				if (uitargetItem3 != null)
				{
					UITargetItem uitargetItem4 = uitargetItem3;
					uitargetItem4.OnSelect = (Action<UITargetItem>)Delegate.Combine(uitargetItem4.OnSelect, new Action<UITargetItem>(this.SelectItem));
					UITargetItem uitargetItem5 = uitargetItem3;
					uitargetItem5.OnConfirm = (Action<UITargetItem>)Delegate.Combine(uitargetItem5.OnConfirm, new Action<UITargetItem>(this.SelectAndConfirmItem));
					this.items.Add(uitargetItem3);
				}
			}
		}
		for (int l = 0; l < this.temItemList.Count; l++)
		{
			global::Common.DestroyObj(this.temItemList[l].gameObject);
		}
		if (suggestedValue.is_object)
		{
			this.SelectValue(suggestedValue);
		}
		else
		{
			this.SelectLO(suggestedValue.obj_val as Logic.Object);
		}
		this.OnSelectObject = Select;
		this.OnCancel = Cancel;
		this.Show(true);
		if (this.m_Table != null)
		{
			this.PopulateTable(table_def_id, eligableTargets);
		}
		return true;
	}

	// Token: 0x06002EB6 RID: 11958 RVA: 0x001815AC File Offset: 0x0017F7AC
	private void PopulateTexts(string id)
	{
		UITargetSelectWindow.Def def = UITargetSelectWindow.Def.Load(id);
		if (def != null)
		{
			if (def.caption != null)
			{
				UIText.SetText(this.m_Title, def.caption, this.m_Vars, null);
			}
			else
			{
				TextMeshProUGUI title = this.m_Title;
				if (title != null)
				{
					title.gameObject.SetActive(false);
				}
			}
			if (def.text1 != null)
			{
				UIText.SetText(this.m_Text1, def.text1, this.m_Vars, null);
			}
			else
			{
				TextMeshProUGUI text = this.m_Text1;
				if (text != null)
				{
					text.gameObject.SetActive(false);
				}
			}
			if (def.text2 != null)
			{
				UIText.SetText(this.m_Text2, def.text2, this.m_Vars, null);
			}
			else
			{
				TextMeshProUGUI text2 = this.m_Text2;
				if (text2 != null)
				{
					text2.gameObject.SetActive(false);
				}
			}
			if (def.ok != null)
			{
				UIText.SetText(this.m_OkLabel, def.ok, this.m_Vars, null);
			}
			if (def.cancel != null)
			{
				UIText.SetText(this.m_CancelLabel, def.cancel, this.m_Vars, null);
			}
			return;
		}
		TextMeshProUGUI title2 = this.m_Title;
		if (title2 != null)
		{
			title2.gameObject.SetActive(false);
		}
		TextMeshProUGUI text3 = this.m_Text1;
		if (text3 != null)
		{
			text3.gameObject.SetActive(false);
		}
		TextMeshProUGUI text4 = this.m_Text2;
		if (text4 == null)
		{
			return;
		}
		text4.gameObject.SetActive(false);
	}

	// Token: 0x06002EB7 RID: 11959 RVA: 0x001816F4 File Offset: 0x0017F8F4
	private void SetupConfirmButton()
	{
		if (this.Button_ConfirmSelect == null)
		{
			return;
		}
		if (this.SetupOkButttonMethod != null)
		{
			this.SetupOkButttonMethod(this.Button_ConfirmSelect);
			return;
		}
		Tooltip.Get(this.Button_ConfirmSelect.gameObject, true).SetObj(null, null, null);
	}

	// Token: 0x06002EB8 RID: 11960 RVA: 0x00181744 File Offset: 0x0017F944
	private void UpdateConfirmButtonStatus()
	{
		if (this.Button_ConfirmSelect == null)
		{
			return;
		}
		if (this.m_CurrentSelectedItem == null)
		{
			this.Button_ConfirmSelect.Enable(false, false);
			return;
		}
		if (this.ValidateOkButttonMethod == null)
		{
			if (this.Button_ConfirmSelect != null)
			{
				this.Button_ConfirmSelect.Enable(this.m_CurrentSelectedItem != null, false);
				return;
			}
		}
		else
		{
			this.Button_ConfirmSelect.Enable(this.ValidateOkButttonMethod(this.GetSelecedItem()), false);
		}
	}

	// Token: 0x06002EB9 RID: 11961 RVA: 0x001817C8 File Offset: 0x0017F9C8
	private void SelectItem(UITargetItem item)
	{
		if (this.m_CurrentSelectedItem == item)
		{
			return;
		}
		if (this.m_CurrentSelectedItem != null)
		{
			this.m_CurrentSelectedItem.Select(false);
		}
		this.m_CurrentSelectedItem = item;
		UITargetItem currentSelectedItem = this.m_CurrentSelectedItem;
		if (currentSelectedItem != null)
		{
			currentSelectedItem.Select(true);
		}
		this.invalidateFocusItem = true;
	}

	// Token: 0x06002EBA RID: 11962 RVA: 0x0018181E File Offset: 0x0017FA1E
	private void SelectAndConfirmItem(UITargetItem item)
	{
		if (this.m_CurrentSelectedItem != null)
		{
			this.m_CurrentSelectedItem.Select(false);
		}
		this.m_CurrentSelectedItem = item;
		this.m_CurrentSelectedItem.Select(true);
		this.HandleConfirm(null);
	}

	// Token: 0x06002EBB RID: 11963 RVA: 0x00181854 File Offset: 0x0017FA54
	private void CallOnCancel()
	{
		Action onCancel = this.OnCancel;
		if (onCancel == null)
		{
			return;
		}
		this.OnCancel = null;
		onCancel();
	}

	// Token: 0x06002EBC RID: 11964 RVA: 0x00181879 File Offset: 0x0017FA79
	public void HandleCancel(BSGButton btn)
	{
		this.CallOnCancel();
		this.Show(false);
	}

	// Token: 0x06002EBD RID: 11965 RVA: 0x00181888 File Offset: 0x0017FA88
	private void HandleConfirm(BSGButton btn)
	{
		this.OnCancel = null;
		if (this.OnSelectObject != null)
		{
			Action<Value> onSelectObject = this.OnSelectObject;
			Value obj = (this.m_CurrentSelectedItem != null) ? this.m_CurrentSelectedItem.Data.Target : Value.Null;
			this.Show(false);
			onSelectObject(obj);
			return;
		}
		this.Show(false);
	}

	// Token: 0x06002EBE RID: 11966 RVA: 0x001818E5 File Offset: 0x0017FAE5
	public Value GetSelecedItem()
	{
		if (!(this.m_CurrentSelectedItem != null))
		{
			return Value.Null;
		}
		return this.m_CurrentSelectedItem.Data.Target;
	}

	// Token: 0x06002EBF RID: 11967 RVA: 0x0018190C File Offset: 0x0017FB0C
	private void PollSelectionChanged()
	{
		Logic.Object @object = BaseUI.SelLO();
		if (@object == UITargetSelectWindow.last_sel)
		{
			return;
		}
		UITargetSelectWindow.last_sel = @object;
		if (@object == null)
		{
			return;
		}
		this.SelectLO(@object);
	}

	// Token: 0x06002EC0 RID: 11968 RVA: 0x0018193C File Offset: 0x0017FB3C
	protected override void Update()
	{
		base.Update();
		if (this.ValidateMethod != null && !this.ValidateMethod())
		{
			this.Show(false);
		}
		this.PollSelectionChanged();
		if (this.invalidateFocusItem)
		{
			this.BringSelectionIntoView();
			this.invalidateFocusItem = false;
		}
		for (int i = this.items.Count - 1; i >= 0; i--)
		{
			UITargetItem uitargetItem = this.items[i];
			if (!uitargetItem.IsValid())
			{
				this.items.RemoveAt(i);
				global::Common.DestroyObj(uitargetItem.gameObject);
				if (this.m_CurrentSelectedItem = uitargetItem)
				{
					this.SelectItem(null);
				}
			}
		}
		this.UpdateConfirmButtonStatus();
	}

	// Token: 0x06002EC1 RID: 11969 RVA: 0x001819E8 File Offset: 0x0017FBE8
	public List<UITargetItem> GetItems()
	{
		return this.items;
	}

	// Token: 0x06002EC2 RID: 11970 RVA: 0x001819F0 File Offset: 0x0017FBF0
	private void PopulateTable(string table_def, List<TargetPickerData> targets)
	{
		List<KeyValuePair<Value, Vars>> list = new List<KeyValuePair<Value, Vars>>();
		if (targets != null && targets.Count > 0)
		{
			for (int i = 0; i < targets.Count; i++)
			{
				list.Add(new KeyValuePair<Value, Vars>(targets[i].Target, targets[i].Vars));
			}
		}
		TableDataProvider dp = TableDataProvider.CreateFromDef(table_def, list);
		this.m_Table.Build(dp);
		UIGenericTable table = this.m_Table;
		table.OnRowSelected = (Action<UIGenericTable.Row, int>)Delegate.Combine(table.OnRowSelected, new Action<UIGenericTable.Row, int>(this.HandleOnTableRowSelect));
	}

	// Token: 0x06002EC3 RID: 11971 RVA: 0x000023FD File Offset: 0x000005FD
	private void HandleOnTableRowSelect(UIGenericTable.Row r, int index)
	{
	}

	// Token: 0x06002EC4 RID: 11972 RVA: 0x00181A80 File Offset: 0x0017FC80
	public static UITargetSelectWindow.Data GenerateTestData()
	{
		UITargetSelectWindow.Data data = new UITargetSelectWindow.Data();
		data.SelectCallback = delegate(Logic.Object x)
		{
			Debug.Log("Selected " + x.ToString());
		};
		data.CancelCallback = delegate()
		{
			Debug.Log("Cancel");
		};
		data.Vars = new Vars();
		data.Suggested = null;
		data.CaptionKey = "TargetSelect.selectGovernProvinceCaption";
		data.BodyKey = "TargetSelect.selectGovernProvinceBody";
		new List<Logic.Object>();
		new List<Logic.Object>();
		new List<int>();
		TableDataProvider dataProvider = new TableDataProvider();
		data.DataProvider = dataProvider;
		return data;
	}

	// Token: 0x04001F8D RID: 8077
	private static string wnd_def_id = "TargetSelectWindow";

	// Token: 0x04001F8E RID: 8078
	private static UITargetSelectWindow sm_Instance;

	// Token: 0x04001F8F RID: 8079
	public GameObject ItemProrotype;

	// Token: 0x04001F90 RID: 8080
	public RectTransform Container;

	// Token: 0x04001F91 RID: 8081
	[UIFieldTarget("id_Title")]
	private TextMeshProUGUI m_Title;

	// Token: 0x04001F92 RID: 8082
	[UIFieldTarget("id_text1")]
	private TextMeshProUGUI m_Text1;

	// Token: 0x04001F93 RID: 8083
	[UIFieldTarget("id_text2")]
	private TextMeshProUGUI m_Text2;

	// Token: 0x04001F94 RID: 8084
	[UIFieldTarget("id_OkLabel")]
	private TextMeshProUGUI m_OkLabel;

	// Token: 0x04001F95 RID: 8085
	[UIFieldTarget("id_CancelLabel")]
	private TextMeshProUGUI m_CancelLabel;

	// Token: 0x04001F96 RID: 8086
	[UIFieldTarget("id_ScrollRect")]
	private ScrollRect m_ScrollRect;

	// Token: 0x04001F97 RID: 8087
	[UIFieldTarget("id_Table")]
	private UIGenericTable m_Table;

	// Token: 0x04001F98 RID: 8088
	[UIFieldTarget("id_Confirm")]
	private BSGButton Button_ConfirmSelect;

	// Token: 0x04001F99 RID: 8089
	[UIFieldTarget("id_Cancel")]
	private BSGButton Button_Cancel;

	// Token: 0x04001F9A RID: 8090
	private bool invalidateFocusItem;

	// Token: 0x04001F9B RID: 8091
	private List<UITargetItem> items = new List<UITargetItem>();

	// Token: 0x04001F9C RID: 8092
	private UITargetItem m_CurrentSelectedItem;

	// Token: 0x04001F9D RID: 8093
	private Action<Value> OnSelectObject;

	// Token: 0x04001F9E RID: 8094
	private Action OnCancel;

	// Token: 0x04001F9F RID: 8095
	private Vars m_Vars;

	// Token: 0x04001FA0 RID: 8096
	private Func<bool> ValidateMethod;

	// Token: 0x04001FA1 RID: 8097
	private Func<Value, bool> ValidateOkButttonMethod;

	// Token: 0x04001FA2 RID: 8098
	private Action<BSGButton> SetupOkButttonMethod;

	// Token: 0x04001FA3 RID: 8099
	private static bool in_show = false;

	// Token: 0x04001FA4 RID: 8100
	private Dictionary<TargetPickerData, UITargetItem> reusable = new Dictionary<TargetPickerData, UITargetItem>();

	// Token: 0x04001FA5 RID: 8101
	private List<UITargetItem> temItemList = new List<UITargetItem>();

	// Token: 0x04001FA6 RID: 8102
	public static Logic.Object last_sel = null;

	// Token: 0x02000854 RID: 2132
	private class Def
	{
		// Token: 0x060050BA RID: 20666 RVA: 0x0023AA44 File Offset: 0x00238C44
		public static UITargetSelectWindow.Def Load(string id)
		{
			Game game = GameLogic.Get(true);
			if (game == null)
			{
				return null;
			}
			if (string.IsNullOrEmpty(id))
			{
				id = "TargetSelect";
			}
			DT.Field field = game.dt.Find(id, null);
			if (field == null)
			{
				id = "TargetSelect";
				field = game.dt.Find(id, null);
			}
			if (field == null)
			{
				return null;
			}
			return new UITargetSelectWindow.Def
			{
				caption = field.FindChild("caption", null, true, true, true, '.'),
				text1 = field.FindChild("text1", null, true, true, true, '.'),
				text2 = field.FindChild("text2", null, true, true, true, '.'),
				ok = field.FindChild("ok", null, true, true, true, '.'),
				cancel = field.FindChild("cancel", null, true, true, true, '.'),
				back = field.FindChild("back", null, true, true, true, '.')
			};
		}

		// Token: 0x04003EBD RID: 16061
		public DT.Field caption;

		// Token: 0x04003EBE RID: 16062
		public DT.Field text1;

		// Token: 0x04003EBF RID: 16063
		public DT.Field text2;

		// Token: 0x04003EC0 RID: 16064
		public DT.Field ok;

		// Token: 0x04003EC1 RID: 16065
		public DT.Field cancel;

		// Token: 0x04003EC2 RID: 16066
		public DT.Field back;
	}

	// Token: 0x02000855 RID: 2133
	public class Data
	{
		// Token: 0x04003EC3 RID: 16067
		public TableDataProvider DataProvider;

		// Token: 0x04003EC4 RID: 16068
		public Logic.Object Suggested;

		// Token: 0x04003EC5 RID: 16069
		public Action<Logic.Object> SelectCallback;

		// Token: 0x04003EC6 RID: 16070
		public Action CancelCallback;

		// Token: 0x04003EC7 RID: 16071
		public Vars Vars;

		// Token: 0x04003EC8 RID: 16072
		public string CaptionKey;

		// Token: 0x04003EC9 RID: 16073
		public string BodyKey;

		// Token: 0x04003ECA RID: 16074
		public string BodyKey2;
	}

	// Token: 0x02000856 RID: 2134
	public struct Settings
	{
		// Token: 0x060050BD RID: 20669 RVA: 0x0023AB28 File Offset: 0x00238D28
		public static UITargetSelectWindow.Settings DEFAULT()
		{
			return new UITargetSelectWindow.Settings
			{
				width = 300f,
				height = 300f
			};
		}

		// Token: 0x04003ECB RID: 16075
		public float width;

		// Token: 0x04003ECC RID: 16076
		public float height;
	}
}
