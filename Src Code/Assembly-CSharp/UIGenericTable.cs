using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020002C6 RID: 710
public class UIGenericTable : MonoBehaviour, LoopScrollPrefabSource, LoopScrollDataSource
{
	// Token: 0x17000232 RID: 562
	// (get) Token: 0x06002C82 RID: 11394 RVA: 0x00173F32 File Offset: 0x00172132
	// (set) Token: 0x06002C83 RID: 11395 RVA: 0x00173F3A File Offset: 0x0017213A
	public UIGenericTable.ColumnHeader CurrentSortColumn { get; private set; }

	// Token: 0x17000233 RID: 563
	// (get) Token: 0x06002C84 RID: 11396 RVA: 0x00173F43 File Offset: 0x00172143
	// (set) Token: 0x06002C85 RID: 11397 RVA: 0x00173F4B File Offset: 0x0017214B
	public UIGenericTable.SortOrder CurrentSortOrder { get; private set; }

	// Token: 0x06002C86 RID: 11398 RVA: 0x00173F54 File Offset: 0x00172154
	private void Start()
	{
		if (this.autoPopulateTestDataOnStart && this.dataProvider == null)
		{
			TableDataProvider dp = TableDataProvider.CreateFromDef(this.TestDefKey, new List<KeyValuePair<Value, Vars>>());
			this.Build(dp);
		}
	}

	// Token: 0x06002C87 RID: 11399 RVA: 0x00173F8C File Offset: 0x0017218C
	protected void Init()
	{
		if (this.m_Initilized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_BodyScrollRect != null && this.m_HeaderScrollRect != null)
		{
			LoopScrollRect bodyScrollRect = this.m_BodyScrollRect;
			if (bodyScrollRect != null)
			{
				Scrollbar horizontalScrollbar = bodyScrollRect.horizontalScrollbar;
				if (horizontalScrollbar != null)
				{
					horizontalScrollbar.onValueChanged.AddListener(new UnityAction<float>(this.HandleHorizontalScrollValueChnaged));
				}
			}
		}
		this.m_Initilized = true;
	}

	// Token: 0x06002C88 RID: 11400 RVA: 0x00173FF9 File Offset: 0x001721F9
	private void HandleHorizontalScrollValueChnaged(float v)
	{
		if (this.m_HeaderScrollRect != null)
		{
			this.m_HeaderScrollRect.horizontalNormalizedPosition = v;
		}
	}

	// Token: 0x06002C89 RID: 11401 RVA: 0x00174015 File Offset: 0x00172215
	public void Build(TableDataProvider dp)
	{
		this.Init();
		this.dataProvider = dp;
		this.RebuildAll();
	}

	// Token: 0x06002C8A RID: 11402 RVA: 0x0017402A File Offset: 0x0017222A
	private void RebuildAll()
	{
		this.UpdateTableGlobals();
		this.PopulateHeaders();
		this.PopulateRows();
	}

	// Token: 0x06002C8B RID: 11403 RVA: 0x00174040 File Offset: 0x00172240
	public void Clear()
	{
		this.dataProvider = null;
		this.m_VisibleRows.Clear();
		this.Init();
		if (this.m_HeaderContianer != null)
		{
			for (int i = this.m_HeaderContianer.transform.childCount - 1; i >= 0; i--)
			{
				global::Common.DestroyObj(this.m_HeaderContianer.transform.GetChild(i).gameObject);
			}
		}
		if (this.m_DataContainer != null)
		{
			for (int j = this.m_DataContainer.transform.childCount - 1; j >= 0; j--)
			{
				global::Common.DestroyObj(this.m_DataContainer.transform.GetChild(j).gameObject);
			}
		}
	}

	// Token: 0x06002C8C RID: 11404 RVA: 0x001740F4 File Offset: 0x001722F4
	private void UpdateTableGlobals()
	{
		if (this.dataProvider == null)
		{
			return;
		}
		if (this.m_HeaderScrollRect != null)
		{
			this.m_HeaderScrollRect.GetOrAddComponent<LayoutElement>().minHeight = this.dataProvider.GetHeaderHeight();
		}
		if (this.m_DataContainer != null)
		{
			VerticalLayoutGroup component = this.m_DataContainer.GetComponent<VerticalLayoutGroup>();
			if (component != null)
			{
				component.spacing = this.dataProvider.rowDef.vertical_spacing;
			}
		}
		if (this.m_HeaderContianer != null)
		{
			HorizontalLayoutGroup component2 = this.m_HeaderContianer.GetComponent<HorizontalLayoutGroup>();
			if (component2 != null)
			{
				component2.padding = this.dataProvider.rowDef.padding;
				component2.spacing = this.dataProvider.rowDef.horizontal_spacing;
			}
		}
	}

	// Token: 0x06002C8D RID: 11405 RVA: 0x001741BC File Offset: 0x001723BC
	private void PopulateHeaders()
	{
		if (this.dataProvider == null)
		{
			return;
		}
		if (this.m_HeaderContianer == null)
		{
			return;
		}
		if (this.dataProvider.columns == null || this.dataProvider.columns.Count == 0)
		{
			return;
		}
		int num = Mathf.Max(this.dataProvider.columns.Count, this.m_Columns.Count);
		int i = 0;
		while (i < num)
		{
			TableDataProvider.ColumnData columnData = (i < this.dataProvider.columns.Count) ? this.dataProvider.columns[i] : null;
			if (this.m_Columns.Count > i)
			{
				this.m_Columns[i].SetData(columnData);
				goto IL_11E;
			}
			GameObject gameObject = ((columnData != null) ? columnData.def.header_prefab : null) ?? UIGenericTable.ColumnHeader.GetPrefab();
			if (!(gameObject == null))
			{
				UIGenericTable.ColumnHeader columnHeader = UIGenericTable.ColumnHeader.Create(columnData, gameObject, this.m_HeaderContianer.transform as RectTransform);
				columnHeader.OnSelect = new Action<UIGenericTable.Cell>(this.HandleOnSort);
				columnHeader.gameObject.hideFlags |= HideFlags.DontSaveInEditor;
				this.m_Columns.Add(columnHeader);
				goto IL_11E;
			}
			IL_148:
			i++;
			continue;
			IL_11E:
			this.m_Columns[i].gameObject.SetActive(this.m_Columns[i].settings != null);
			goto IL_148;
		}
	}

	// Token: 0x06002C8E RID: 11406 RVA: 0x0017431C File Offset: 0x0017251C
	private void PopulateRows()
	{
		if (this.dataProvider == null)
		{
			return;
		}
		if (this.m_DataContainer == null)
		{
			return;
		}
		if (this.dataProvider.columns == null || this.dataProvider.columns.Count == 0)
		{
			return;
		}
		this.m_DataContainer.gameObject.SetActive(false);
		int rowCount = this.dataProvider.GetRowCount();
		this.m_BodyScrollRect.prefabSource = this;
		this.m_BodyScrollRect.dataSource = this;
		this.m_BodyScrollRect.totalCount = rowCount;
		this.m_BodyScrollRect.RefillCells(0, false, 0f);
		this.m_DataContainer.gameObject.SetActive(true);
	}

	// Token: 0x06002C8F RID: 11407 RVA: 0x001743C5 File Offset: 0x001725C5
	public void SelectRow(int idx)
	{
		if (this.m_VisibleRows == null)
		{
			return;
		}
		if (idx < 0 || idx >= this.m_VisibleRows.Count)
		{
			return;
		}
		this.SelectRow(this.m_VisibleRows[idx]);
	}

	// Token: 0x06002C90 RID: 11408 RVA: 0x001743F8 File Offset: 0x001725F8
	private void SelectRow(UIGenericTable.Row row)
	{
		this.m_SelectedRowIndex = row.OrignalIndex;
		for (int i = 0; i < this.m_VisibleRows.Count; i++)
		{
			UIGenericTable.Row row2 = this.m_VisibleRows[i];
			if (!(row2 == null))
			{
				row2.Select(row2.OrignalIndex == row.OrignalIndex);
			}
		}
		if (this.OnRowSelected != null)
		{
			this.OnRowSelected(row, row.OrignalIndex);
		}
	}

	// Token: 0x06002C91 RID: 11409 RVA: 0x0017446B File Offset: 0x0017266B
	private void HandleOnRowSelected(UIGenericTable.Row r)
	{
		this.SelectRow(r);
	}

	// Token: 0x06002C92 RID: 11410 RVA: 0x00174474 File Offset: 0x00172674
	private void HandleOnRowFocused(UIGenericTable.Row r)
	{
		this.OnRowFocused(r, this.GetRowIndex(r));
	}

	// Token: 0x06002C93 RID: 11411 RVA: 0x00174489 File Offset: 0x00172689
	private void HandleOnSort(UIGenericTable.Cell h)
	{
		this.SortByColumn(h);
	}

	// Token: 0x06002C94 RID: 11412 RVA: 0x00174492 File Offset: 0x00172692
	public int GetRowCount()
	{
		TableDataProvider tableDataProvider = this.dataProvider;
		if (((tableDataProvider != null) ? tableDataProvider.rows : null) == null)
		{
			return 0;
		}
		return this.dataProvider.rows.Count;
	}

	// Token: 0x06002C95 RID: 11413 RVA: 0x001744BA File Offset: 0x001726BA
	public int GetRowIndex(UIGenericTable.Row row)
	{
		return row.OrignalIndex;
	}

	// Token: 0x06002C96 RID: 11414 RVA: 0x001744C4 File Offset: 0x001726C4
	public Value GeDataAtIndex(int index)
	{
		if (this.dataProvider == null)
		{
			return Value.Null;
		}
		if (index < 0 || this.dataProvider.rows.Count >= index)
		{
			return Value.Null;
		}
		return this.dataProvider.rows[index].target;
	}

	// Token: 0x06002C97 RID: 11415 RVA: 0x00174512 File Offset: 0x00172712
	public int GetSelectedRowIndex()
	{
		return this.m_SelectedRowIndex;
	}

	// Token: 0x06002C98 RID: 11416 RVA: 0x0017451C File Offset: 0x0017271C
	public Value GetSelectedRowData()
	{
		if (this.dataProvider == null)
		{
			return Value.Null;
		}
		TableDataProvider.RowData rowData = this.dataProvider.GetRowData(this.m_SelectedRowIndex, false);
		if (rowData == null)
		{
			return Value.Null;
		}
		return rowData.target;
	}

	// Token: 0x06002C99 RID: 11417 RVA: 0x0017455C File Offset: 0x0017275C
	public int GetColumnIndex(UIGenericTable.ColumnHeader header)
	{
		if (header == null)
		{
			return -1;
		}
		if (this.m_Columns == null || this.m_Columns.Count == 0)
		{
			return -1;
		}
		for (int i = 0; i < this.m_Columns.Count; i++)
		{
			if (this.m_Columns[i] == header)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06002C9A RID: 11418 RVA: 0x001745B8 File Offset: 0x001727B8
	public void ApplySort(int colIndex, UIGenericTable.SortOrder order)
	{
		if (this.m_Columns == null || this.m_Columns.Count == 0)
		{
			return;
		}
		if (colIndex < 0 || colIndex > this.m_Columns.Count - 1)
		{
			return;
		}
		this.ApplySort(this.m_Columns[colIndex], order);
	}

	// Token: 0x06002C9B RID: 11419 RVA: 0x001745F8 File Offset: 0x001727F8
	public void ApplySort(UIGenericTable.ColumnHeader column, UIGenericTable.SortOrder order)
	{
		if (this.m_BodyScrollRect == null)
		{
			return;
		}
		this.CurrentSortColumn = column;
		this.CurrentSortOrder = order;
		for (int i = 0; i < this.m_Columns.Count; i++)
		{
			UIGenericTable.ColumnHeader columnHeader = this.m_Columns[i];
			bool flag = columnHeader == this.CurrentSortColumn;
			columnHeader.SetSelected(flag);
			if (flag)
			{
				columnHeader.SetSortOrderMode(this.CurrentSortOrder);
			}
			else
			{
				columnHeader.SetSortOrderMode(UIGenericTable.SortOrder.None);
			}
		}
		if (this.CurrentSortColumn != null && this.CurrentSortOrder != UIGenericTable.SortOrder.None)
		{
			this.dataProvider.rows.Sort((TableDataProvider.RowData x, TableDataProvider.RowData y) => x.CompareTo(y, this.CurrentSortColumn.settings, this.CurrentSortOrder == UIGenericTable.SortOrder.Ascending));
		}
		else
		{
			this.dataProvider.rows.Sort((TableDataProvider.RowData x, TableDataProvider.RowData y) => x.orignalIndex.CompareTo(y.orignalIndex));
		}
		float value = this.m_BodyScrollRect.verticalScrollbar.value;
		this.m_BodyScrollRect.RefreshCells();
		this.m_BodyScrollRect.verticalScrollbar.value = value;
	}

	// Token: 0x06002C9C RID: 11420 RVA: 0x00174704 File Offset: 0x00172904
	public void SortByColumn(UIGenericTable.Cell header)
	{
		UIGenericTable.ColumnHeader columnHeader = header as UIGenericTable.ColumnHeader;
		if (columnHeader == this.CurrentSortColumn)
		{
			this.CurrentSortColumn = columnHeader;
			if (this.CurrentSortOrder == UIGenericTable.SortOrder.Ascending)
			{
				this.CurrentSortOrder = UIGenericTable.SortOrder.Descending;
			}
			else if (this.CurrentSortOrder == UIGenericTable.SortOrder.Descending)
			{
				this.CurrentSortOrder = UIGenericTable.SortOrder.None;
				this.CurrentSortColumn = null;
			}
			else
			{
				this.CurrentSortOrder = UIGenericTable.SortOrder.Ascending;
			}
		}
		else
		{
			this.CurrentSortColumn = columnHeader;
			this.CurrentSortOrder = UIGenericTable.SortOrder.Ascending;
		}
		this.ApplySort(this.CurrentSortColumn, this.CurrentSortOrder);
	}

	// Token: 0x06002C9D RID: 11421 RVA: 0x00174784 File Offset: 0x00172984
	private void Update()
	{
		if (this.m_Invalidate)
		{
			this.m_Invalidate = false;
			this.RebuildAll();
			return;
		}
		int num = 0;
		for (int i = 0; i < this.m_VisibleRows.Count; i++)
		{
			UIGenericTable.Row row = this.m_VisibleRows[i];
			if (row.IsVisible() && row.NeedsUpdate())
			{
				num++;
				row.UpdateData();
				if (num > 1)
				{
					break;
				}
			}
		}
	}

	// Token: 0x06002C9E RID: 11422 RVA: 0x001747EC File Offset: 0x001729EC
	public GameObject GetLoopObjectInstance(int index)
	{
		UIGenericTable.Row row;
		if (this.m_RowPool.Count == 0)
		{
			row = UIGenericTable.Row.Create(null, this.dataProvider.rowDef.prefab, this.m_DataContainer.transform as RectTransform, -1);
		}
		else
		{
			row = this.m_RowPool.Pop();
		}
		UIGenericTable.Row row2 = row;
		row2.OnSelect = (Action<UIGenericTable.Row>)Delegate.Combine(row2.OnSelect, new Action<UIGenericTable.Row>(this.HandleOnRowSelected));
		UIGenericTable.Row row3 = row;
		row3.OnFocus = (Action<UIGenericTable.Row>)Delegate.Combine(row3.OnFocus, new Action<UIGenericTable.Row>(this.HandleOnRowFocused));
		row.gameObject.SetActive(true);
		this.m_VisibleRows.Add(row);
		return row.gameObject;
	}

	// Token: 0x06002C9F RID: 11423 RVA: 0x001748A0 File Offset: 0x00172AA0
	public void ReturnLoopObjectInstance(Transform trans)
	{
		trans.gameObject.SetActive(false);
		trans.SetParent(base.transform, false);
		UIGenericTable.Row component = trans.GetComponent<UIGenericTable.Row>();
		if (component == null)
		{
			return;
		}
		this.m_RowPool.Push(component);
		this.m_VisibleRows.Remove(component);
		UIGenericTable.Row row = component;
		row.OnSelect = (Action<UIGenericTable.Row>)Delegate.Remove(row.OnSelect, new Action<UIGenericTable.Row>(this.HandleOnRowSelected));
		UIGenericTable.Row row2 = component;
		row2.OnFocus = (Action<UIGenericTable.Row>)Delegate.Remove(row2.OnFocus, new Action<UIGenericTable.Row>(this.HandleOnRowFocused));
	}

	// Token: 0x06002CA0 RID: 11424 RVA: 0x00174934 File Offset: 0x00172B34
	public void ProvideData(Transform transform, int idx)
	{
		UIGenericTable.Row component = transform.GetComponent<UIGenericTable.Row>();
		if (component == null)
		{
			return;
		}
		TableDataProvider.RowData rowData = this.dataProvider.GetRowData(idx, true);
		component.SetData(rowData, idx);
		component.Select(rowData.orignalIndex == this.m_SelectedRowIndex);
	}

	// Token: 0x06002CA1 RID: 11425 RVA: 0x0017497C File Offset: 0x00172B7C
	public void ClearSort()
	{
		this.ApplySort(0, UIGenericTable.SortOrder.None);
	}

	// Token: 0x04001E59 RID: 7769
	public const int DEFAULT_HORIZONTAL_SPACING = 4;

	// Token: 0x04001E5A RID: 7770
	public const int DEFAULT_VERTICAL_SPACING = 4;

	// Token: 0x04001E5B RID: 7771
	[UIFieldTarget("id_HeaderScrollRect")]
	private ScrollRect m_HeaderScrollRect;

	// Token: 0x04001E5C RID: 7772
	[UIFieldTarget("id_BodyScrollRect")]
	private LoopScrollRect m_BodyScrollRect;

	// Token: 0x04001E5D RID: 7773
	[UIFieldTarget("id_HeaderContianer")]
	private GameObject m_HeaderContianer;

	// Token: 0x04001E5E RID: 7774
	[UIFieldTarget("id_DataContainer")]
	private RectTransform m_DataContainer;

	// Token: 0x04001E5F RID: 7775
	public string TestDefKey;

	// Token: 0x04001E60 RID: 7776
	public bool autoPopulateTestDataOnStart;

	// Token: 0x04001E63 RID: 7779
	private TableDataProvider dataProvider;

	// Token: 0x04001E64 RID: 7780
	private List<UIGenericTable.ColumnHeader> m_Columns = new List<UIGenericTable.ColumnHeader>();

	// Token: 0x04001E65 RID: 7781
	private List<UIGenericTable.Row> m_VisibleRows = new List<UIGenericTable.Row>();

	// Token: 0x04001E66 RID: 7782
	private Stack<UIGenericTable.Row> m_RowPool = new Stack<UIGenericTable.Row>();

	// Token: 0x04001E67 RID: 7783
	private bool m_Initilized;

	// Token: 0x04001E68 RID: 7784
	protected bool m_Invalidate;

	// Token: 0x04001E69 RID: 7785
	private int m_SelectedRowIndex = -1;

	// Token: 0x04001E6A RID: 7786
	public Action<UIGenericTable.Row, int> OnRowSelected;

	// Token: 0x04001E6B RID: 7787
	public Action<UIGenericTable.Row, int> OnRowFocused;

	// Token: 0x0200081D RID: 2077
	public enum SortOrder
	{
		// Token: 0x04003DD0 RID: 15824
		None,
		// Token: 0x04003DD1 RID: 15825
		Ascending,
		// Token: 0x04003DD2 RID: 15826
		Descending
	}

	// Token: 0x0200081E RID: 2078
	public class Row : Hotspot
	{
		// Token: 0x1700062E RID: 1582
		// (get) Token: 0x06004FA2 RID: 20386 RVA: 0x00236545 File Offset: 0x00234745
		// (set) Token: 0x06004FA3 RID: 20387 RVA: 0x0023654D File Offset: 0x0023474D
		public TableDataProvider.RowData Data { get; private set; }

		// Token: 0x1700062F RID: 1583
		// (get) Token: 0x06004FA4 RID: 20388 RVA: 0x00236556 File Offset: 0x00234756
		// (set) Token: 0x06004FA5 RID: 20389 RVA: 0x0023655E File Offset: 0x0023475E
		public int OrignalIndex { get; private set; }

		// Token: 0x06004FA6 RID: 20390 RVA: 0x00236567 File Offset: 0x00234767
		protected virtual void Init()
		{
			if (this.m_Initalized)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			this.hlg = base.GetComponent<HorizontalLayoutGroup>();
			this.m_Initalized = true;
		}

		// Token: 0x06004FA7 RID: 20391 RVA: 0x0023658C File Offset: 0x0023478C
		public void SetData(TableDataProvider.RowData rowData, int index)
		{
			this.Init();
			this.Data = rowData;
			this.OrignalIndex = index;
			this.isOdd = (index % 2 != 0);
			this.Build();
		}

		// Token: 0x06004FA8 RID: 20392 RVA: 0x002365B4 File Offset: 0x002347B4
		public void Select(bool selected)
		{
			if (this.m_Selected == selected)
			{
				return;
			}
			this.m_Selected = selected;
			this.UpdateHighlight();
			if (this.m_Cells != null && this.m_Cells.Count > 0)
			{
				int i = 0;
				int count = this.m_Cells.Count;
				while (i < count)
				{
					this.m_Cells[i].Select(false);
					i++;
				}
			}
		}

		// Token: 0x06004FA9 RID: 20393 RVA: 0x00236618 File Offset: 0x00234818
		private void Build()
		{
			if (this == null)
			{
				return;
			}
			if (this.Data == null)
			{
				return;
			}
			if (this.Data.def == null)
			{
				return;
			}
			if (this.hlg != null)
			{
				this.hlg.spacing = this.Data.def.horizontal_spacing;
				this.hlg.padding = this.Data.def.padding;
			}
			int num = Mathf.Max(this.Data.cells.Count, this.m_Cells.Count);
			for (int i = 0; i < num; i++)
			{
				TableDataProvider.CellData cellData = (i < this.Data.cells.Count) ? this.Data.cells[i] : null;
				if (i < this.m_Cells.Count)
				{
					this.m_Cells[i].SetData((cellData != null) ? cellData.settings : null, cellData);
				}
				else
				{
					GameObject gameObject = ((cellData != null) ? cellData.settings.def.cell_prefab : null) ?? UIGenericTable.Cell.GetPrefab();
					if (!(gameObject == null))
					{
						UIGenericTable.Cell cell = UIGenericTable.Cell.Create((cellData != null) ? cellData.settings : null, cellData, gameObject, base.gameObject.transform as RectTransform);
						UIGenericTable.Cell cell2 = cell;
						cell2.OnSelect = (Action<UIGenericTable.Cell>)Delegate.Combine(cell2.OnSelect, new Action<UIGenericTable.Cell>(this.HanldeOnCellSelected));
						UIGenericTable.Cell cell3 = cell;
						cell3.OnFocus = (Action<UIGenericTable.Cell>)Delegate.Combine(cell3.OnFocus, new Action<UIGenericTable.Cell>(this.HanldeOnCellFocused));
						this.m_Cells.Add(cell);
					}
				}
			}
			this.UpdateHighlight();
		}

		// Token: 0x06004FAA RID: 20394 RVA: 0x002367BD File Offset: 0x002349BD
		public void SetOdd(bool newValue)
		{
			if (this.isOdd == newValue)
			{
				return;
			}
			this.isOdd = newValue;
			this.UpdateHighlight();
		}

		// Token: 0x06004FAB RID: 20395 RVA: 0x002367D6 File Offset: 0x002349D6
		public Value GetTragetData()
		{
			if (this.m_Cells == null || this.m_Cells.Count == 0)
			{
				return Value.Null;
			}
			return this.m_Cells[0].Data.target;
		}

		// Token: 0x06004FAC RID: 20396 RVA: 0x000023FD File Offset: 0x000005FD
		private void Update()
		{
		}

		// Token: 0x06004FAD RID: 20397 RVA: 0x00236809 File Offset: 0x00234A09
		private void HanldeOnCellSelected(UIGenericTable.Cell cell)
		{
			if (this.OnSelect != null)
			{
				this.OnSelect(this);
			}
		}

		// Token: 0x06004FAE RID: 20398 RVA: 0x0023681F File Offset: 0x00234A1F
		private void HanldeOnCellFocused(UIGenericTable.Cell cell)
		{
			if (this.OnFocus != null)
			{
				this.OnFocus(this);
			}
		}

		// Token: 0x06004FAF RID: 20399 RVA: 0x00236835 File Offset: 0x00234A35
		public override void OnClick(PointerEventData e)
		{
			base.OnClick(e);
			if (this.OnSelect != null)
			{
				this.OnSelect(this);
			}
		}

		// Token: 0x06004FB0 RID: 20400 RVA: 0x00236852 File Offset: 0x00234A52
		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
			this.UpdateHighlight();
		}

		// Token: 0x06004FB1 RID: 20401 RVA: 0x00236861 File Offset: 0x00234A61
		public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);
			this.UpdateHighlight();
		}

		// Token: 0x06004FB2 RID: 20402 RVA: 0x00236870 File Offset: 0x00234A70
		private void UpdateHighlight()
		{
			if (this.m_Background != null)
			{
				this.m_Background.color = this.GetCurrentFormating().tint;
			}
			if (this.m_SelectedRow != null)
			{
				this.m_SelectedRow.gameObject.SetActive(this.m_Selected);
			}
		}

		// Token: 0x06004FB3 RID: 20403 RVA: 0x002368C8 File Offset: 0x00234AC8
		protected TableDataProvider.RowData.Def.Formatting GetCurrentFormating()
		{
			if (this.m_Disbaled)
			{
				if (!this.isOdd)
				{
					return this.Data.def.even_disbaled;
				}
				return this.Data.def.odd_disbaled;
			}
			else if (this.m_Selected)
			{
				if (!this.isOdd)
				{
					return this.Data.def.even_selected;
				}
				return this.Data.def.odd_selected;
			}
			else if (this.mouse_in)
			{
				if (!this.isOdd)
				{
					return this.Data.def.even_hover;
				}
				return this.Data.def.odd_hover;
			}
			else
			{
				if (!this.isOdd)
				{
					return this.Data.def.even_normal;
				}
				return this.Data.def.odd_normal;
			}
		}

		// Token: 0x06004FB4 RID: 20404 RVA: 0x00236994 File Offset: 0x00234B94
		public static UIGenericTable.Row Create(TableDataProvider.RowData data, GameObject prototype, RectTransform parent, int index)
		{
			if (parent == null)
			{
				return null;
			}
			GameObject gameObject;
			if (prototype != null)
			{
				gameObject = global::Common.SpawnPooled(prototype, parent, false, "");
			}
			else
			{
				gameObject = UIGenericTable.Row.CreateEmptyRow(parent);
			}
			UIGenericTable.Row orAddComponent = gameObject.GetOrAddComponent<UIGenericTable.Row>();
			orAddComponent.SetData(data, index);
			gameObject.gameObject.hideFlags |= HideFlags.DontSaveInEditor;
			return orAddComponent;
		}

		// Token: 0x06004FB5 RID: 20405 RVA: 0x002369F0 File Offset: 0x00234BF0
		public static GameObject CreateEmpty(GameObject prototype, RectTransform parent)
		{
			GameObject gameObject;
			if (prototype != null)
			{
				gameObject = global::Common.SpawnPooled(prototype, parent, false, "");
			}
			else
			{
				gameObject = UIGenericTable.Row.CreateEmptyRow(parent);
			}
			gameObject.gameObject.hideFlags |= HideFlags.DontSaveInEditor;
			return gameObject;
		}

		// Token: 0x06004FB6 RID: 20406 RVA: 0x000448AF File Offset: 0x00042AAF
		public static GameObject GetPrefab()
		{
			return null;
		}

		// Token: 0x06004FB7 RID: 20407 RVA: 0x00236A34 File Offset: 0x00234C34
		public static GameObject CreateEmptyRow(RectTransform parent = null)
		{
			GameObject gameObject = new GameObject("Row", new Type[]
			{
				typeof(RectTransform),
				typeof(HorizontalLayoutGroup),
				typeof(UIGenericTable.Row),
				typeof(ContentSizeFitter)
			});
			HorizontalLayoutGroup component = gameObject.GetComponent<HorizontalLayoutGroup>();
			component.childControlWidth = true;
			component.childControlHeight = true;
			component.childForceExpandWidth = false;
			component.childForceExpandHeight = false;
			component.spacing = 4f;
			ContentSizeFitter component2 = gameObject.GetComponent<ContentSizeFitter>();
			component2.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
			component2.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
			if (parent != null)
			{
				gameObject.transform.SetParent(parent);
			}
			return gameObject;
		}

		// Token: 0x06004FB8 RID: 20408 RVA: 0x0002C53B File Offset: 0x0002A73B
		public bool IsVisible()
		{
			return true;
		}

		// Token: 0x06004FB9 RID: 20409 RVA: 0x00236ADB File Offset: 0x00234CDB
		public bool NeedsUpdate()
		{
			return this.m_LastUpdateTime + this.m_UpdateInterval < UnityEngine.Time.unscaledTime;
		}

		// Token: 0x06004FBA RID: 20410 RVA: 0x00236AF4 File Offset: 0x00234CF4
		public void UpdateData()
		{
			for (int i = 0; i < this.m_Cells.Count; i++)
			{
				UIGenericTable.Cell cell = this.m_Cells[i];
				if (cell.NeedsUpdate())
				{
					cell.UpdateData();
				}
			}
			this.m_LastUpdateTime = UnityEngine.Time.unscaledTime;
		}

		// Token: 0x06004FBB RID: 20411 RVA: 0x00236B40 File Offset: 0x00234D40
		public int CompareTo(UIGenericTable.Row row, TableDataProvider.ColumnData c, bool ascending)
		{
			Value sortValue = c.GetSortValue(this.Data.target, null);
			Value sortValue2 = c.GetSortValue(row.Data.target, null);
			if (sortValue.is_null || sortValue2.is_null)
			{
				return 0;
			}
			if (sortValue.type == Value.Type.Float)
			{
				if (ascending)
				{
					return sortValue2.float_val.CompareTo(sortValue.float_val);
				}
				return sortValue.float_val.CompareTo(sortValue2.float_val);
			}
			else if (sortValue.type == Value.Type.Int)
			{
				if (ascending)
				{
					return sortValue2.int_val.CompareTo(sortValue.int_val);
				}
				return sortValue.int_val.CompareTo(sortValue2.int_val);
			}
			else
			{
				if (sortValue.type != Value.Type.String)
				{
					return 0;
				}
				if (ascending)
				{
					return sortValue.String(null).CompareTo(sortValue2.String(null));
				}
				return sortValue2.String(null).CompareTo(sortValue.String(null));
			}
		}

		// Token: 0x06004FBC RID: 20412 RVA: 0x00236C24 File Offset: 0x00234E24
		private void ResetCells()
		{
			if (this.m_Cells == null)
			{
				return;
			}
			for (int i = 0; i < this.m_Cells.Count; i++)
			{
				this.m_Cells[i].SetData(null, null);
			}
		}

		// Token: 0x06004FBD RID: 20413 RVA: 0x00236C63 File Offset: 0x00234E63
		protected override void OnDisable()
		{
			base.OnDisable();
			this.ResetCells();
		}

		// Token: 0x06004FBE RID: 20414 RVA: 0x00236C71 File Offset: 0x00234E71
		private void Clear()
		{
			this.OnFocus = null;
			this.OnSelect = null;
			this.m_Cells.Clear();
		}

		// Token: 0x04003DD3 RID: 15827
		[UIFieldTarget("id_Background")]
		private Image m_Background;

		// Token: 0x04003DD4 RID: 15828
		[UIFieldTarget("id_SelectedRow")]
		private GameObject m_SelectedRow;

		// Token: 0x04003DD5 RID: 15829
		private HorizontalLayoutGroup hlg;

		// Token: 0x04003DD8 RID: 15832
		private List<UIGenericTable.Cell> m_Cells = new List<UIGenericTable.Cell>();

		// Token: 0x04003DD9 RID: 15833
		private float m_LastUpdateTime;

		// Token: 0x04003DDA RID: 15834
		private float m_UpdateInterval = 1f;

		// Token: 0x04003DDB RID: 15835
		private bool m_Initalized;

		// Token: 0x04003DDC RID: 15836
		private bool m_Selected;

		// Token: 0x04003DDD RID: 15837
		private bool m_Disbaled;

		// Token: 0x04003DDE RID: 15838
		public Action<UIGenericTable.Row> OnSelect;

		// Token: 0x04003DDF RID: 15839
		public Action<UIGenericTable.Row> OnFocus;

		// Token: 0x04003DE0 RID: 15840
		private bool isOdd = true;
	}

	// Token: 0x0200081F RID: 2079
	public class Cell : Hotspot, IPoolable
	{
		// Token: 0x17000630 RID: 1584
		// (get) Token: 0x06004FC0 RID: 20416 RVA: 0x00236CB1 File Offset: 0x00234EB1
		// (set) Token: 0x06004FC1 RID: 20417 RVA: 0x00236CB9 File Offset: 0x00234EB9
		public TableDataProvider.ColumnData settings { get; protected set; }

		// Token: 0x17000631 RID: 1585
		// (get) Token: 0x06004FC2 RID: 20418 RVA: 0x00236CC2 File Offset: 0x00234EC2
		// (set) Token: 0x06004FC3 RID: 20419 RVA: 0x00236CCA File Offset: 0x00234ECA
		public TableDataProvider.CellData Data { get; protected set; }

		// Token: 0x06004FC4 RID: 20420 RVA: 0x00236CD4 File Offset: 0x00234ED4
		public void SetData(TableDataProvider.ColumnData columnSettings, TableDataProvider.CellData cellData)
		{
			this.m_CurrentIconData = Value.Null;
			this.settings = columnSettings;
			this.Data = cellData;
			this.Init();
			if (this.Data != null)
			{
				this.m_UpdateInterval = this.Data.settings.def.update_interval;
			}
			this.Build();
		}

		// Token: 0x06004FC5 RID: 20421 RVA: 0x00236D2C File Offset: 0x00234F2C
		protected virtual void Init()
		{
			if (this.m_Initalized)
			{
				return;
			}
			UICommon.FindComponents(this, true);
			this.m_CellLayoutElement = this.GetOrAddComponent<LayoutElement>();
			if (this.m_Icon != null)
			{
				this.m_StaticIconLayoutElement = this.m_Icon.GetComponent<LayoutElement>();
				if (this.m_StaticIconLayoutElement != null)
				{
					this.m_OriginalStaticIconSize = new Vector2(this.m_StaticIconLayoutElement.preferredWidth, this.m_StaticIconLayoutElement.preferredHeight);
				}
				else
				{
					Rect rect = this.m_Icon.rectTransform.rect;
					this.m_OriginalStaticIconSize = new Vector2(rect.width, rect.height);
				}
			}
			if (this.m_ObjectIconContainer != null)
			{
				this.m_ObjectIconLayoutElement = this.m_ObjectIconContainer.GetComponent<LayoutElement>();
				if (this.m_StaticIconLayoutElement != null)
				{
					this.m_OriginalObjectIconSize = new Vector2(this.m_ObjectIconLayoutElement.preferredWidth, this.m_ObjectIconLayoutElement.preferredHeight);
				}
				else
				{
					Rect rect2 = this.m_Icon.rectTransform.rect;
					this.m_OriginalObjectIconSize = new Vector2(rect2.width, rect2.height);
				}
			}
			this.m_Initalized = true;
		}

		// Token: 0x06004FC6 RID: 20422 RVA: 0x00236E50 File Offset: 0x00235050
		public void UpdateData()
		{
			if (this.Data == null)
			{
				return;
			}
			this.UpdateValue();
			this.UpdateIcon();
			this.m_LastUpdateTime = UnityEngine.Time.unscaledTime;
		}

		// Token: 0x06004FC7 RID: 20423 RVA: 0x00236E72 File Offset: 0x00235072
		public void Select(bool v)
		{
			if (this.m_Selected == v)
			{
				return;
			}
			this.m_Selected = v;
			this.UpdateHighlight();
		}

		// Token: 0x06004FC8 RID: 20424 RVA: 0x00236E8B File Offset: 0x0023508B
		public void Disbale(bool disabled)
		{
			if (this.m_Disbaled == disabled)
			{
				return;
			}
			this.UpdateHighlight();
		}

		// Token: 0x06004FC9 RID: 20425 RVA: 0x00236EA0 File Offset: 0x002350A0
		public virtual void Build()
		{
			this.UpdateValue();
			this.UpdateIcon();
			this.UpdateIconTooltip();
			this.UpdateStaticIcon();
			if (this.m_CellLayoutElement != null && this.settings != null)
			{
				this.m_CellLayoutElement.preferredWidth = this.settings.def.width;
				this.m_CellLayoutElement.preferredHeight = this.settings.def.row_height;
			}
			this.UpdateHighlight();
		}

		// Token: 0x06004FCA RID: 20426 RVA: 0x00236F18 File Offset: 0x00235118
		protected void UpdateValue()
		{
			if (this.m_Text == null)
			{
				return;
			}
			if (this.Data == null)
			{
				if (this.settings != null)
				{
					UIText.SetText(this.m_Text, global::Defs.Localize(this.settings.def.field, "title", null, null, true, true));
				}
				return;
			}
			string localziedText = this.Data.GetLocalziedText();
			if (!string.IsNullOrEmpty(localziedText))
			{
				this.m_Text.gameObject.SetActive(true);
				UIText.SetText(this.m_Text, localziedText);
				return;
			}
			this.m_Text.gameObject.SetActive(false);
		}

		// Token: 0x06004FCB RID: 20427 RVA: 0x00236FB4 File Offset: 0x002351B4
		protected void UpdateStaticIcon()
		{
			if (this.m_Icon == null)
			{
				return;
			}
			TableDataProvider.CellData data = this.Data;
			Sprite sprite = (data != null) ? data.GetStaticIcon() : null;
			if (!(sprite != null))
			{
				this.m_Icon.gameObject.SetActive(false);
				return;
			}
			this.m_Icon.gameObject.SetActive(true);
			this.m_Icon.overrideSprite = sprite;
			this.Data.GetIconValue();
			float icon_width = this.Data.settings.def.icon_width;
			float icon_height = this.Data.settings.def.icon_height;
			if (!(this.m_StaticIconLayoutElement != null))
			{
				if (icon_height != 0f && icon_width != 0f)
				{
					(this.m_Icon.transform as RectTransform).sizeDelta = new Vector2(icon_width, icon_height);
				}
				else
				{
					(this.m_Icon.transform as RectTransform).sizeDelta = this.m_OriginalStaticIconSize;
				}
				this.m_Icon.transform.localPosition = Vector3.zero;
				return;
			}
			if (icon_height != 0f && icon_width != 0f)
			{
				this.m_StaticIconLayoutElement.preferredWidth = icon_width;
				this.m_StaticIconLayoutElement.preferredHeight = icon_height;
				this.m_StaticIconLayoutElement.minWidth = icon_width;
				this.m_StaticIconLayoutElement.minHeight = icon_height;
				return;
			}
			this.m_StaticIconLayoutElement.preferredWidth = this.m_OriginalStaticIconSize.x;
			this.m_StaticIconLayoutElement.preferredHeight = this.m_OriginalStaticIconSize.y;
			this.m_StaticIconLayoutElement.minWidth = this.m_OriginalStaticIconSize.x;
			this.m_StaticIconLayoutElement.minHeight = this.m_OriginalStaticIconSize.y;
		}

		// Token: 0x06004FCC RID: 20428 RVA: 0x00237160 File Offset: 0x00235360
		protected void UpdateIcon()
		{
			if (this.m_ObjectIconContainer == null)
			{
				return;
			}
			if (this.Data == null)
			{
				this.m_ObjectIconContainer.gameObject.SetActive(false);
				return;
			}
			Value iconValue = this.Data.GetIconValue();
			if (!iconValue.is_valid)
			{
				this.m_ObjectIconContainer.gameObject.SetActive(false);
				return;
			}
			if (this.m_CurrentIconData == iconValue)
			{
				return;
			}
			this.m_CurrentIconData = iconValue;
			string iconVariant = this.Data.GetIconVariant();
			Vars vars = null;
			if (!string.IsNullOrEmpty(iconVariant))
			{
				vars = new Vars();
				vars.Set<string>("variant", iconVariant);
			}
			if (this.currentIcon != null)
			{
				global::Common.DestroyObj(this.currentIcon);
				this.currentIcon = null;
			}
			GameObject iconPrefab = this.Data.settings.GetIconPrefab();
			if (iconPrefab != null)
			{
				this.currentIcon = global::Common.Spawn(iconPrefab, this.m_ObjectIconContainer, false, "");
				ObjectIcon component = this.currentIcon.GetComponent<ObjectIcon>();
				if (component != null)
				{
					component.SetObject(iconValue.obj_val, vars);
				}
				FieldIcon component2 = this.currentIcon.GetComponent<FieldIcon>();
				if (component2 != null && iconValue.obj_val is DT.Field)
				{
					component2.SetObject(iconValue.obj_val as DT.Field, null);
				}
			}
			if (this.currentIcon == null)
			{
				this.currentIcon = ObjectIcon.GetIcon(iconValue, vars, this.m_ObjectIconContainer);
			}
			if (this.currentIcon != null)
			{
				this.currentIcon.gameObject.SetActive(true);
				Vector2 iconSize = this.Data.GetIconSize();
				if (iconSize.x != 0f && iconSize.y != 0f)
				{
					this.m_ObjectIconLayoutElement.preferredWidth = iconSize.x;
					this.m_ObjectIconLayoutElement.preferredHeight = iconSize.y;
					this.m_ObjectIconLayoutElement.minWidth = iconSize.x;
					this.m_ObjectIconLayoutElement.minHeight = iconSize.y;
				}
				else
				{
					this.m_ObjectIconLayoutElement.preferredWidth = this.m_OriginalObjectIconSize.x;
					this.m_ObjectIconLayoutElement.preferredHeight = this.m_OriginalObjectIconSize.y;
					this.m_ObjectIconLayoutElement.minWidth = this.m_OriginalObjectIconSize.x;
					this.m_ObjectIconLayoutElement.minHeight = this.m_OriginalObjectIconSize.y;
				}
				UICommon.FillParent(this.currentIcon.transform as RectTransform);
			}
			this.m_ObjectIconContainer.gameObject.SetActive(this.currentIcon != null);
		}

		// Token: 0x06004FCD RID: 20429 RVA: 0x002373E8 File Offset: 0x002355E8
		private void UpdateIconTooltip()
		{
			TableDataProvider.CellData data = this.Data;
			if (string.IsNullOrEmpty((data != null) ? data.settings.def.cell_icon_tooltip : null))
			{
				return;
			}
			if (this.m_Icon == null)
			{
				return;
			}
			Tooltip.Get(this.m_Icon.gameObject, true).SetDef(this.Data.settings.def.cell_icon_tooltip, new Vars(this.Data.target));
		}

		// Token: 0x06004FCE RID: 20430 RVA: 0x00237464 File Offset: 0x00235664
		protected void UpdateHighlight()
		{
			if (this.settings == null)
			{
				return;
			}
			TableDataProvider.ColumnData.Def.Formating currentFormating = this.GetCurrentFormating();
			if (this.m_Contianer != null)
			{
				this.m_Contianer.childAlignment = currentFormating.cell_alignment;
				this.m_Contianer.padding = currentFormating.padding;
			}
			if (this.m_CellColor != null)
			{
				this.m_CellColor.color = currentFormating.tint;
			}
			if (this.m_Text != null)
			{
				this.m_Text.color = currentFormating.text_tint;
				this.m_Text.fontSize = currentFormating.text_size;
				this.m_Text.fontSizeMin = currentFormating.text_size_min;
				this.m_Text.fontSizeMax = ((currentFormating.text_size <= 0f) ? currentFormating.max_text_size : currentFormating.text_size);
				this.m_Text.enableAutoSizing = (currentFormating.auto_sizing || currentFormating.text_size <= 0f);
				this.m_Text.alignment = currentFormating.text_alignment;
			}
		}

		// Token: 0x06004FCF RID: 20431 RVA: 0x00237571 File Offset: 0x00235771
		protected virtual TableDataProvider.ColumnData.Def.Formating GetCurrentFormating()
		{
			return this.settings.def.cell_normal;
		}

		// Token: 0x06004FD0 RID: 20432 RVA: 0x00237583 File Offset: 0x00235783
		public override void OnClick(PointerEventData e)
		{
			base.OnClick(e);
			if (this.OnSelect != null)
			{
				this.OnSelect(this);
			}
		}

		// Token: 0x06004FD1 RID: 20433 RVA: 0x002375A0 File Offset: 0x002357A0
		public override void OnDoubleClick(PointerEventData e)
		{
			base.OnDoubleClick(e);
			if (this.OnFocus != null)
			{
				this.OnFocus(this);
			}
		}

		// Token: 0x06004FD2 RID: 20434 RVA: 0x002375BD File Offset: 0x002357BD
		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
			this.UpdateHighlight();
		}

		// Token: 0x06004FD3 RID: 20435 RVA: 0x002375CC File Offset: 0x002357CC
		public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);
			this.UpdateHighlight();
		}

		// Token: 0x06004FD4 RID: 20436 RVA: 0x002375DB File Offset: 0x002357DB
		public static GameObject GetPrefab()
		{
			return UICommon.GetPrefab("GenericTableCell", null);
		}

		// Token: 0x06004FD5 RID: 20437 RVA: 0x002375E8 File Offset: 0x002357E8
		public static UIGenericTable.Cell Create(TableDataProvider.ColumnData columnData, TableDataProvider.CellData data, GameObject prototype, RectTransform parent)
		{
			if (parent == null)
			{
				return null;
			}
			if (prototype == null)
			{
				return null;
			}
			UIGenericTable.Cell orAddComponent = global::Common.SpawnPooled(prototype, parent, false, "").GetOrAddComponent<UIGenericTable.Cell>();
			orAddComponent.SetData(columnData, data);
			return orAddComponent;
		}

		// Token: 0x06004FD6 RID: 20438 RVA: 0x0023761A File Offset: 0x0023581A
		public bool NeedsUpdate()
		{
			return this.m_LastUpdateTime + this.m_UpdateInterval < UnityEngine.Time.unscaledTime;
		}

		// Token: 0x06004FD7 RID: 20439 RVA: 0x00237630 File Offset: 0x00235830
		private void Clean()
		{
			this.OnFocus = null;
			this.OnSelect = null;
			this.m_CurrentIconData = Value.Null;
			this.m_LastUpdateTime = 0f;
			this.m_Selected = false;
			this.m_Disbaled = false;
			UICommon.DeleteChildren(this.m_ObjectIconContainer);
		}

		// Token: 0x06004FD8 RID: 20440 RVA: 0x0023766F File Offset: 0x0023586F
		private void OnDestroy()
		{
			this.Clean();
		}

		// Token: 0x06004FD9 RID: 20441 RVA: 0x000023FD File Offset: 0x000005FD
		public void OnPoolSpawned()
		{
		}

		// Token: 0x06004FDA RID: 20442 RVA: 0x000023FD File Offset: 0x000005FD
		public void OnPoolActivated()
		{
		}

		// Token: 0x06004FDB RID: 20443 RVA: 0x0023766F File Offset: 0x0023586F
		public void OnPoolDeactivated()
		{
			this.Clean();
		}

		// Token: 0x06004FDC RID: 20444 RVA: 0x000023FD File Offset: 0x000005FD
		public void OnPoolDestroyed()
		{
		}

		// Token: 0x04003DE3 RID: 15843
		[UIFieldTarget("id_Text")]
		protected TextMeshProUGUI m_Text;

		// Token: 0x04003DE4 RID: 15844
		[UIFieldTarget("id_ObjectIcon")]
		protected RectTransform m_ObjectIconContainer;

		// Token: 0x04003DE5 RID: 15845
		[UIFieldTarget("id_Icon")]
		protected Image m_Icon;

		// Token: 0x04003DE6 RID: 15846
		[UIFieldTarget("id_Container")]
		protected LayoutGroup m_Contianer;

		// Token: 0x04003DE7 RID: 15847
		[UIFieldTarget("id_CellColor")]
		protected Image m_CellColor;

		// Token: 0x04003DE8 RID: 15848
		protected LayoutElement m_CellLayoutElement;

		// Token: 0x04003DE9 RID: 15849
		protected LayoutElement m_StaticIconLayoutElement;

		// Token: 0x04003DEA RID: 15850
		protected LayoutElement m_ObjectIconLayoutElement;

		// Token: 0x04003DEB RID: 15851
		private float m_LastUpdateTime;

		// Token: 0x04003DEC RID: 15852
		private float m_UpdateInterval = 1f;

		// Token: 0x04003DED RID: 15853
		private bool m_Initalized;

		// Token: 0x04003DEE RID: 15854
		private bool m_Selected;

		// Token: 0x04003DEF RID: 15855
		private bool m_Disbaled;

		// Token: 0x04003DF0 RID: 15856
		private Vector2 m_OriginalStaticIconSize;

		// Token: 0x04003DF1 RID: 15857
		private Vector2 m_OriginalObjectIconSize;

		// Token: 0x04003DF2 RID: 15858
		private Value m_CurrentIconData;

		// Token: 0x04003DF3 RID: 15859
		public Action<UIGenericTable.Cell> OnSelect;

		// Token: 0x04003DF4 RID: 15860
		public Action<UIGenericTable.Cell> OnFocus;

		// Token: 0x04003DF5 RID: 15861
		private GameObject currentIcon;
	}

	// Token: 0x02000820 RID: 2080
	public class ColumnHeader : UIGenericTable.Cell
	{
		// Token: 0x06004FDE RID: 20446 RVA: 0x0023768A File Offset: 0x0023588A
		public void SetData(TableDataProvider.ColumnData data)
		{
			base.settings = data;
			this.Init();
			this.Build();
		}

		// Token: 0x06004FDF RID: 20447 RVA: 0x0023769F File Offset: 0x0023589F
		public void SetSelected(bool selected)
		{
			this.m_IsSelected = selected;
			if (this.m_Selected != null)
			{
				this.m_Selected.gameObject.SetActive(this.m_IsSelected);
			}
		}

		// Token: 0x06004FE0 RID: 20448 RVA: 0x002376CC File Offset: 0x002358CC
		public void SetSortOrderMode(UIGenericTable.SortOrder so)
		{
			if (this.m_SortOrderAscending != null)
			{
				this.m_SortOrderAscending.SetActive(so == UIGenericTable.SortOrder.Ascending);
			}
			if (this.m_SortOrderDescending != null)
			{
				this.m_SortOrderDescending.SetActive(so == UIGenericTable.SortOrder.Descending);
			}
		}

		// Token: 0x06004FE1 RID: 20449 RVA: 0x00237708 File Offset: 0x00235908
		protected override void Init()
		{
			base.Init();
			this.m_HotspotImage = base.GetComponent<HotspotImage>();
		}

		// Token: 0x06004FE2 RID: 20450 RVA: 0x0023771C File Offset: 0x0023591C
		public override void Build()
		{
			this.PopulateHoverEffect();
			this.UpdateValue();
			this.UpdateIcon();
			this.UpdateTooltip();
			this.SetSelected(false);
			this.SetSortOrderMode(UIGenericTable.SortOrder.None);
			if (this.m_CellColor != null)
			{
				this.m_CellColor.sprite = base.settings.def.header_background;
			}
			if (this.m_Icon != null)
			{
				if (base.settings.def.header_icon != null)
				{
					this.m_Icon.gameObject.SetActive(true);
					this.m_Icon.overrideSprite = base.settings.def.header_icon;
				}
				else
				{
					this.m_Icon.gameObject.SetActive(false);
				}
			}
			if (this.m_CellLayoutElement != null)
			{
				this.m_CellLayoutElement.preferredWidth = base.settings.def.width;
				this.m_CellLayoutElement.preferredHeight = base.settings.def.header_height;
			}
			base.UpdateHighlight();
		}

		// Token: 0x06004FE3 RID: 20451 RVA: 0x00237828 File Offset: 0x00235A28
		private void PopulateHoverEffect()
		{
			if (this.m_HotspotImage != null)
			{
				this.m_HotspotImage.normalImage = base.settings.def.header_background;
				this.m_HotspotImage.rolloverImage = ((base.settings.def.header_background_over != null) ? base.settings.def.header_background_over : base.settings.def.header_background);
				this.m_HotspotImage.pressedImage = ((base.settings.def.header_background_select != null) ? base.settings.def.header_background_select : base.settings.def.header_background);
				this.m_HotspotImage.SetState(BSGButton.State.Normal);
			}
			if (this.m_Selected != null)
			{
				this.m_Selected.overrideSprite = ((base.settings.def.header_background_select != null) ? base.settings.def.header_background_select : base.settings.def.header_background);
			}
		}

		// Token: 0x06004FE4 RID: 20452 RVA: 0x0023794C File Offset: 0x00235B4C
		protected new void UpdateValue()
		{
			if (this.m_Text != null)
			{
				string text = global::Defs.Localize(base.settings.def.field, "title", null, null, true, true);
				if (!string.IsNullOrEmpty(text))
				{
					this.m_Text.gameObject.SetActive(true);
					UIText.SetText(this.m_Text, text);
					return;
				}
				this.m_Text.gameObject.SetActive(false);
			}
		}

		// Token: 0x06004FE5 RID: 20453 RVA: 0x002379C0 File Offset: 0x00235BC0
		protected new void UpdateIcon()
		{
			if (this.m_ObjectIconContainer == null)
			{
				return;
			}
			if (base.Data == null)
			{
				this.m_ObjectIconContainer.gameObject.SetActive(false);
				return;
			}
			Value iconValue = base.Data.GetIconValue();
			if (!iconValue.is_valid)
			{
				this.m_ObjectIconContainer.gameObject.SetActive(false);
				return;
			}
			this.m_ObjectIconContainer.gameObject.SetActive(true);
			UICommon.DeleteChildren(this.m_ObjectIconContainer);
			GameObject icon = ObjectIcon.GetIcon(iconValue, null, this.m_ObjectIconContainer);
			if (!(icon != null))
			{
				this.m_ObjectIconContainer.gameObject.SetActive(false);
				return;
			}
			float iconWidth = base.Data.settings.GetIconWidth();
			float header_height = base.Data.settings.def.header_height;
			if (header_height != 0f && iconWidth != 0f)
			{
				LayoutElement orAddComponent = icon.GetOrAddComponent<LayoutElement>();
				orAddComponent.preferredWidth = iconWidth;
				orAddComponent.preferredHeight = header_height;
				return;
			}
			UICommon.FillParent(icon.transform as RectTransform);
		}

		// Token: 0x06004FE6 RID: 20454 RVA: 0x00237AC0 File Offset: 0x00235CC0
		private void UpdateTooltip()
		{
			Vars vars = new Vars(base.settings.def.field);
			vars.Set<Logic.Kingdom>("kingdom", BaseUI.LogicKingdom());
			Tooltip.Get(base.gameObject, true).SetDef("TableHeaderTooltip", vars);
		}

		// Token: 0x06004FE7 RID: 20455 RVA: 0x00237B0A File Offset: 0x00235D0A
		protected override TableDataProvider.ColumnData.Def.Formating GetCurrentFormating()
		{
			return base.settings.def.header_normal;
		}

		// Token: 0x06004FE8 RID: 20456 RVA: 0x00237B1C File Offset: 0x00235D1C
		public new static GameObject GetPrefab()
		{
			return UICommon.GetPrefab("GenericTableHeader", null);
		}

		// Token: 0x06004FE9 RID: 20457 RVA: 0x00237B29 File Offset: 0x00235D29
		public static UIGenericTable.ColumnHeader Create(TableDataProvider.ColumnData data, GameObject prototype, RectTransform parent)
		{
			if (data == null)
			{
				return null;
			}
			if (parent == null)
			{
				return null;
			}
			if (prototype == null)
			{
				return null;
			}
			UIGenericTable.ColumnHeader orAddComponent = global::Common.Spawn(prototype, parent, false, "").GetOrAddComponent<UIGenericTable.ColumnHeader>();
			orAddComponent.SetData(data);
			return orAddComponent;
		}

		// Token: 0x06004FEA RID: 20458 RVA: 0x00237B5F File Offset: 0x00235D5F
		public override string ToString()
		{
			return "Header (" + base.settings.def.field.key + ") ";
		}

		// Token: 0x04003DF6 RID: 15862
		[UIFieldTarget("id_Selected")]
		private Image m_Selected;

		// Token: 0x04003DF7 RID: 15863
		[UIFieldTarget("id_SortOrderAscending")]
		private GameObject m_SortOrderAscending;

		// Token: 0x04003DF8 RID: 15864
		[UIFieldTarget("id_SortOrderDescending")]
		private GameObject m_SortOrderDescending;

		// Token: 0x04003DF9 RID: 15865
		protected HotspotImage m_HotspotImage;

		// Token: 0x04003DFA RID: 15866
		private bool m_IsSelected;
	}
}
