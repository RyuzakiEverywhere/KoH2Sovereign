using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x02000275 RID: 629
public class UIProvinceSelectorWindow : UIWindow, IListener
{
	// Token: 0x06002687 RID: 9863 RVA: 0x00152102 File Offset: 0x00150302
	public override string GetDefId()
	{
		return UIProvinceSelectorWindow.def_id;
	}

	// Token: 0x170001C8 RID: 456
	// (get) Token: 0x06002688 RID: 9864 RVA: 0x00152109 File Offset: 0x00150309
	// (set) Token: 0x06002689 RID: 9865 RVA: 0x00152111 File Offset: 0x00150311
	public Logic.Kingdom Kingdom { get; private set; }

	// Token: 0x0600268A RID: 9866 RVA: 0x0015211C File Offset: 0x0015031C
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_Close != null)
		{
			this.m_Close.onClick = new BSGButton.OnClick(this.HandleCloseButton);
		}
		this.windowDef = global::Defs.GetDefField("ProvinceSelectorWindow", null);
		this.m_Initialzied = true;
	}

	// Token: 0x0600268B RID: 9867 RVA: 0x00152178 File Offset: 0x00150378
	public void SetData(Logic.Kingdom kingdom)
	{
		this.Init();
		this.defs_version = global::Defs.Version;
		if (this.Kingdom != null)
		{
			this.Kingdom.DelListener(this);
		}
		this.Kingdom = kingdom;
		if (this.Kingdom != null)
		{
			this.Kingdom.AddListener(this);
		}
		this.PopulateStatics();
		this.PopulateTable(this.GetTableDef());
		this.RestoreState();
	}

	// Token: 0x0600268C RID: 9868 RVA: 0x001521E0 File Offset: 0x001503E0
	private DT.Field GetTableDef()
	{
		if (this.windowDef == null)
		{
			return null;
		}
		string @string = this.windowDef.GetString("table_id", null, "", true, true, true, '.');
		if (string.IsNullOrEmpty(@string))
		{
			return null;
		}
		DT.Field defField = global::Defs.GetDefField(@string, null);
		if (defField == null)
		{
			return null;
		}
		return defField;
	}

	// Token: 0x0600268D RID: 9869 RVA: 0x0015222C File Offset: 0x0015042C
	protected override void Update()
	{
		base.Update();
		if (this.defs_version != global::Defs.Version)
		{
			this.defs_version = global::Defs.Version;
			if (Application.isPlaying)
			{
				this.RefreshTable();
				return;
			}
		}
		if (this.m_InvalidateProvider)
		{
			this.m_InvalidateProvider = false;
			this.RefreshTable();
		}
	}

	// Token: 0x0600268E RID: 9870 RVA: 0x0015227A File Offset: 0x0015047A
	private void RefreshTable()
	{
		this.PopulateTable(this.GetTableDef());
	}

	// Token: 0x0600268F RID: 9871 RVA: 0x00152288 File Offset: 0x00150488
	private void PopulateStatics()
	{
		if (this.m_Caption != null)
		{
			UIText.SetTextKey(this.m_Caption, "ProvinceSelectorWindow.caption", new Vars(this.Kingdom), null);
		}
	}

	// Token: 0x06002690 RID: 9872 RVA: 0x001522BC File Offset: 0x001504BC
	private void PopulateTable(DT.Field tableDef)
	{
		if (this.m_Table == null)
		{
			return;
		}
		List<KeyValuePair<Value, Vars>> list = new List<KeyValuePair<Value, Vars>>();
		if (this.Kingdom.realms != null && this.Kingdom.realms.Count > 0)
		{
			for (int i = 0; i < this.Kingdom.realms.Count; i++)
			{
				list.Add(new KeyValuePair<Value, Vars>(this.Kingdom.realms[i], null));
			}
		}
		TableDataProvider dp = TableDataProvider.CreateFromDef(tableDef, list);
		this.m_Table.Build(dp);
		UIGenericTable table = this.m_Table;
		table.OnRowSelected = (Action<UIGenericTable.Row, int>)Delegate.Combine(table.OnRowSelected, new Action<UIGenericTable.Row, int>(this.HandleOnTableRowSelect));
		UIGenericTable table2 = this.m_Table;
		table2.OnRowFocused = (Action<UIGenericTable.Row, int>)Delegate.Combine(table2.OnRowFocused, new Action<UIGenericTable.Row, int>(this.HandleOnTableRowFocused));
	}

	// Token: 0x06002691 RID: 9873 RVA: 0x001523A0 File Offset: 0x001505A0
	private void HandleOnTableRowSelect(UIGenericTable.Row r, int index)
	{
		Logic.Realm realm = r.GetTragetData().obj_val as Logic.Realm;
		this.SelectRealm(realm, false);
	}

	// Token: 0x06002692 RID: 9874 RVA: 0x001523C8 File Offset: 0x001505C8
	private void HandleOnTableRowFocused(UIGenericTable.Row r, int index)
	{
		Logic.Realm realm = r.GetTragetData().obj_val as Logic.Realm;
		this.SelectRealm(realm, true);
	}

	// Token: 0x06002693 RID: 9875 RVA: 0x001523F0 File Offset: 0x001505F0
	private void SelectRealm(Logic.Realm realm, bool focus = false)
	{
		if (realm == null)
		{
			return;
		}
		Castle castle = realm.castle;
		if (castle == null)
		{
			return;
		}
		global::Settlement settlement = castle.visuals as global::Settlement;
		if (settlement == null)
		{
			return;
		}
		WorldUI worldUI = WorldUI.Get();
		if (worldUI == null)
		{
			return;
		}
		worldUI.SelectObj(settlement.gameObject, false, true, true, true);
		if (focus)
		{
			worldUI.LookAt(settlement.gameObject.transform.position, false);
		}
	}

	// Token: 0x06002694 RID: 9876 RVA: 0x0011FFF8 File Offset: 0x0011E1F8
	private void HandleCloseButton(BSGButton b)
	{
		this.Close(false);
	}

	// Token: 0x06002695 RID: 9877 RVA: 0x0015245C File Offset: 0x0015065C
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "realm_added" || message == "realm_deleted")
		{
			this.m_InvalidateProvider = true;
		}
	}

	// Token: 0x06002696 RID: 9878 RVA: 0x00152480 File Offset: 0x00150680
	public void RestoreState()
	{
		if (UIProvinceSelectorWindow.storage == null)
		{
			return;
		}
		if (this.m_Table == null)
		{
			return;
		}
		this.m_Table.ApplySort(UIProvinceSelectorWindow.storage.col_index, UIProvinceSelectorWindow.storage.sort_order);
		UIProvinceSelectorWindow.storage.Clear();
	}

	// Token: 0x06002697 RID: 9879 RVA: 0x001524D0 File Offset: 0x001506D0
	public void StoreState()
	{
		if (UIProvinceSelectorWindow.storage == null)
		{
			return;
		}
		UIProvinceSelectorWindow.storage.Clear();
		if (this.m_Table == null)
		{
			return;
		}
		UIProvinceSelectorWindow.storage.col_index = this.m_Table.GetColumnIndex(this.m_Table.CurrentSortColumn);
		UIProvinceSelectorWindow.storage.sort_order = this.m_Table.CurrentSortOrder;
	}

	// Token: 0x06002698 RID: 9880 RVA: 0x00152533 File Offset: 0x00150733
	protected override void OnDestroy()
	{
		Logic.Kingdom kingdom = this.Kingdom;
		if (kingdom != null)
		{
			kingdom.DelListener(this);
		}
		base.OnDestroy();
	}

	// Token: 0x06002699 RID: 9881 RVA: 0x0015254D File Offset: 0x0015074D
	public override void Close(bool silent = false)
	{
		this.StoreState();
		if (this.on_close != null)
		{
			this.on_close(this);
		}
		this.Hide(silent);
		if (base.gameObject != null)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600269A RID: 9882 RVA: 0x0015258A File Offset: 0x0015078A
	public static GameObject GetPrefab()
	{
		return UICommon.GetPrefab(UIProvinceSelectorWindow.def_id, null);
	}

	// Token: 0x0600269B RID: 9883 RVA: 0x00152598 File Offset: 0x00150798
	public static void ToggleOpen(Logic.Kingdom k)
	{
		if (k == null)
		{
			if (UIProvinceSelectorWindow.current != null)
			{
				UIProvinceSelectorWindow.current.Close(false);
			}
			return;
		}
		if (UIProvinceSelectorWindow.current != null)
		{
			UIProvinceSelectorWindow uiprovinceSelectorWindow = UIProvinceSelectorWindow.current;
			if (((uiprovinceSelectorWindow != null) ? uiprovinceSelectorWindow.Kingdom : null) == k)
			{
				UIProvinceSelectorWindow.current.Close(false);
				return;
			}
			UIProvinceSelectorWindow.current.SetData(k);
			return;
		}
		else
		{
			WorldUI worldUI = WorldUI.Get();
			if (worldUI == null)
			{
				return;
			}
			GameObject prefab = UIProvinceSelectorWindow.GetPrefab();
			if (prefab == null)
			{
				return;
			}
			GameObject gameObject = global::Common.FindChildByName(worldUI.gameObject, "id_MessageContainer", true, true);
			if (gameObject != null)
			{
				UICommon.DeleteChildren(gameObject.transform, typeof(UIProvinceSelectorWindow));
				UIProvinceSelectorWindow.current = UIProvinceSelectorWindow.Create(k, prefab, gameObject.transform as RectTransform);
			}
			return;
		}
	}

	// Token: 0x0600269C RID: 9884 RVA: 0x00152663 File Offset: 0x00150863
	public static bool IsActive()
	{
		return UIProvinceSelectorWindow.current != null;
	}

	// Token: 0x0600269D RID: 9885 RVA: 0x00152670 File Offset: 0x00150870
	public static UIProvinceSelectorWindow Create(Logic.Kingdom kingdom, GameObject prototype, RectTransform parent)
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
		UIProvinceSelectorWindow orAddComponent = global::Common.Spawn(prototype, parent, false, "").GetOrAddComponent<UIProvinceSelectorWindow>();
		orAddComponent.SetData(kingdom);
		orAddComponent.on_close = (UIWindow.OnClose)Delegate.Combine(orAddComponent.on_close, new UIWindow.OnClose(delegate(UIWindow _)
		{
			UIProvinceSelectorWindow.current = null;
		}));
		orAddComponent.Open();
		DT.Field soundsDef = BaseUI.soundsDef;
		BaseUI.PlaySoundEvent((soundsDef != null) ? soundsDef.GetString("open_provinces_window", null, "", true, true, true, '.') : null, null);
		return orAddComponent;
	}

	// Token: 0x04001A1C RID: 6684
	private static string def_id = "ProvinceSelectorWindow";

	// Token: 0x04001A1D RID: 6685
	[UIFieldTarget("id_Caption")]
	private TextMeshProUGUI m_Caption;

	// Token: 0x04001A1E RID: 6686
	[UIFieldTarget("id_Close")]
	private BSGButton m_Close;

	// Token: 0x04001A1F RID: 6687
	[UIFieldTarget("id_Table")]
	private UIGenericTable m_Table;

	// Token: 0x04001A21 RID: 6689
	private bool m_Initialzied;

	// Token: 0x04001A22 RID: 6690
	private bool m_InvalidateProvider;

	// Token: 0x04001A23 RID: 6691
	private DT.Field windowDef;

	// Token: 0x04001A24 RID: 6692
	private int defs_version;

	// Token: 0x04001A25 RID: 6693
	private static UIProvinceSelectorWindow current;

	// Token: 0x04001A26 RID: 6694
	private static UIProvinceSelectorWindow.StateStorage storage = new UIProvinceSelectorWindow.StateStorage();

	// Token: 0x020007D9 RID: 2009
	private class StateStorage
	{
		// Token: 0x06004E63 RID: 20067 RVA: 0x002322B9 File Offset: 0x002304B9
		public void Clear()
		{
			this.col_index = -1;
			this.sort_order = UIGenericTable.SortOrder.None;
		}

		// Token: 0x04003CA8 RID: 15528
		public int col_index = -1;

		// Token: 0x04003CA9 RID: 15529
		public UIGenericTable.SortOrder sort_order;
	}
}
