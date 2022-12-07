using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x020002C5 RID: 709
public class TableDataProvider
{
	// Token: 0x06002C7B RID: 11387 RVA: 0x00173C64 File Offset: 0x00171E64
	public TableDataProvider.RowData GetRowData(int row_index, bool create = true)
	{
		if (this.rows != null && row_index >= 0 && row_index < this.rows.Count)
		{
			return this.rows[row_index];
		}
		if (!create)
		{
			return null;
		}
		return this.CreateEmptyData();
	}

	// Token: 0x06002C7C RID: 11388 RVA: 0x00173C98 File Offset: 0x00171E98
	private TableDataProvider.RowData CreateEmptyData()
	{
		TableDataProvider.RowData rowData = new TableDataProvider.RowData();
		rowData.def = this.rowDef;
		rowData.cells = new List<TableDataProvider.CellData>();
		for (int i = 0; i < this.columns.Count; i++)
		{
			TableDataProvider.CellData cellData = new TableDataProvider.CellData();
			cellData.settings = this.columns[i];
			cellData.target = default(Value);
			rowData.cells.Add(cellData);
		}
		return rowData;
	}

	// Token: 0x06002C7D RID: 11389 RVA: 0x00173D09 File Offset: 0x00171F09
	public int GetRowCount()
	{
		return this.rows.Count;
	}

	// Token: 0x06002C7E RID: 11390 RVA: 0x00173D18 File Offset: 0x00171F18
	public static TableDataProvider CreateFromDef(string def_key, List<KeyValuePair<Value, Vars>> data)
	{
		global::Defs defs = global::Defs.Get(false);
		if (defs == null)
		{
			return null;
		}
		DT.Def def = defs.dt.FindDef(def_key);
		if (def == null)
		{
			return null;
		}
		return TableDataProvider.CreateFromDef(def.field, data);
	}

	// Token: 0x06002C7F RID: 11391 RVA: 0x00173D58 File Offset: 0x00171F58
	public static TableDataProvider CreateFromDef(DT.Field def, List<KeyValuePair<Value, Vars>> data)
	{
		if (global::Defs.Get(false) == null)
		{
			return null;
		}
		if (def == null)
		{
			return null;
		}
		TableDataProvider tableDataProvider = new TableDataProvider();
		tableDataProvider.tableDef = new TableDataProvider.TableDef();
		tableDataProvider.tableDef.Load(def);
		DT.Field field = def.FindChild("column_list", null, true, true, true, '.');
		if (field != null)
		{
			List<DT.Field> list = field.Children();
			int num = 0;
			for (int i = 0; i < list.Count; i++)
			{
				DT.Field field2 = list[i];
				if (field2 != null)
				{
					TableDataProvider.ColumnData columnData = new TableDataProvider.ColumnData();
					columnData.Parse(field2, num);
					tableDataProvider.columns.Add(columnData);
					num++;
				}
			}
		}
		tableDataProvider.rowDef = new TableDataProvider.RowData.Def();
		tableDataProvider.rowDef.Load(def.FindChild("row_settings", null, true, true, true, '.'));
		if (data != null)
		{
			tableDataProvider.rows = new List<TableDataProvider.RowData>();
			for (int j = 0; j < data.Count; j++)
			{
				KeyValuePair<Value, Vars> keyValuePair = data[j];
				TableDataProvider.RowData rowData = new TableDataProvider.RowData();
				rowData.def = tableDataProvider.rowDef;
				rowData.target = keyValuePair.Key;
				rowData.orignalIndex = j;
				rowData.PopulateCells(tableDataProvider.columns, keyValuePair.Key, keyValuePair.Value);
				tableDataProvider.rows.Add(rowData);
			}
		}
		return tableDataProvider;
	}

	// Token: 0x06002C80 RID: 11392 RVA: 0x00173EA4 File Offset: 0x001720A4
	public float GetHeaderHeight()
	{
		float num = 0f;
		if (this.columns != null && this.columns.Count > 0)
		{
			for (int i = 0; i < this.columns.Count; i++)
			{
				TableDataProvider.ColumnData columnData = this.columns[i];
				if (columnData != null && columnData.def.header_height > num)
				{
					num = columnData.def.header_height;
				}
			}
			return num;
		}
		return 30f;
	}

	// Token: 0x04001E55 RID: 7765
	public TableDataProvider.TableDef tableDef;

	// Token: 0x04001E56 RID: 7766
	public TableDataProvider.RowData.Def rowDef;

	// Token: 0x04001E57 RID: 7767
	public List<TableDataProvider.ColumnData> columns = new List<TableDataProvider.ColumnData>();

	// Token: 0x04001E58 RID: 7768
	public List<TableDataProvider.RowData> rows = new List<TableDataProvider.RowData>();

	// Token: 0x02000819 RID: 2073
	public class TableDef
	{
		// Token: 0x06004F8D RID: 20365 RVA: 0x00235DD8 File Offset: 0x00233FD8
		public bool Load(DT.Field field)
		{
			this.name = field.key;
			this.window_height = field.GetFloat("window_height", null, this.window_height, true, true, true, '.');
			this.max_rows = field.GetInt("max_rows", null, this.max_rows, true, true, true, '.');
			this.default_sorting_column = field.GetString("default_sorting_column", null, this.default_sorting_column, true, true, true, '.');
			this.autosave_settings = field.GetBool("autosave_settings", null, this.autosave_settings, true, true, true, '.');
			this.show_header = field.GetBool("show_header", null, this.show_header, true, true, true, '.');
			this.row_height = field.GetFloat("row_height", null, this.row_height, true, true, true, '.');
			this.header_height = field.GetFloat("header_height", null, this.header_height, true, true, true, '.');
			return true;
		}

		// Token: 0x06004F8E RID: 20366 RVA: 0x00235EC0 File Offset: 0x002340C0
		public override string ToString()
		{
			return string.Format("Table: {0}, window height {1}, max rows: {2}, default sorting column {3}, show header {4}, autosave settings {5}", new object[]
			{
				this.name,
				this.window_height,
				this.max_rows,
				this.default_sorting_column,
				this.show_header,
				this.autosave_settings
			});
		}

		// Token: 0x04003DBE RID: 15806
		public float window_height = 750f;

		// Token: 0x04003DBF RID: 15807
		public int max_rows = 10;

		// Token: 0x04003DC0 RID: 15808
		public bool autosave_settings;

		// Token: 0x04003DC1 RID: 15809
		public string default_sorting_column;

		// Token: 0x04003DC2 RID: 15810
		public bool show_header = true;

		// Token: 0x04003DC3 RID: 15811
		public string name;

		// Token: 0x04003DC4 RID: 15812
		public float row_height = 36f;

		// Token: 0x04003DC5 RID: 15813
		public float header_height = 43f;
	}

	// Token: 0x0200081A RID: 2074
	public class RowData
	{
		// Token: 0x06004F90 RID: 20368 RVA: 0x00235F60 File Offset: 0x00234160
		public void PopulateCells(List<TableDataProvider.ColumnData> columns, Value value, Vars vars)
		{
			if (columns == null || columns.Count == 0)
			{
				return;
			}
			this.cells = new List<TableDataProvider.CellData>(columns.Count);
			for (int i = 0; i < columns.Count; i++)
			{
				TableDataProvider.CellData item = new TableDataProvider.CellData
				{
					settings = columns[i],
					target = value,
					vars = vars
				};
				this.cells.Add(item);
			}
		}

		// Token: 0x06004F91 RID: 20369 RVA: 0x00235FC8 File Offset: 0x002341C8
		public int CompareTo(TableDataProvider.RowData row, TableDataProvider.ColumnData c, bool ascending)
		{
			Value sortValue = c.GetSortValue(this.target, this);
			Value sortValue2 = c.GetSortValue(row.target, row);
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

		// Token: 0x04003DC6 RID: 15814
		public TableDataProvider.RowData.Def def;

		// Token: 0x04003DC7 RID: 15815
		public Value target;

		// Token: 0x04003DC8 RID: 15816
		public List<TableDataProvider.CellData> cells;

		// Token: 0x04003DC9 RID: 15817
		public int orignalIndex;

		// Token: 0x02000A35 RID: 2613
		public class Def
		{
			// Token: 0x060055C2 RID: 21954 RVA: 0x0024A210 File Offset: 0x00248410
			public bool Load(DT.Field row_field)
			{
				if (row_field == null)
				{
					return false;
				}
				this.field = row_field;
				this.prefab = global::Defs.GetObj<GameObject>(row_field, "prefab", null);
				this.empty_prefab = global::Defs.GetObj<GameObject>(row_field, "empty_prefab", null);
				this.background = global::Defs.GetObj<Sprite>(row_field, "background", null);
				this.odd_normal = new TableDataProvider.RowData.Def.Formatting(row_field.FindChild("odd.Normal", null, true, true, true, '.'));
				this.odd_hover = new TableDataProvider.RowData.Def.Formatting(row_field.FindChild("odd.Hover", null, true, true, true, '.'));
				this.odd_selected = new TableDataProvider.RowData.Def.Formatting(row_field.FindChild("odd.Selected", null, true, true, true, '.'));
				this.odd_disbaled = new TableDataProvider.RowData.Def.Formatting(row_field.FindChild("odd.Disbaled", null, true, true, true, '.'));
				this.even_normal = new TableDataProvider.RowData.Def.Formatting(row_field.FindChild("even.Normal", null, true, true, true, '.'));
				this.even_hover = new TableDataProvider.RowData.Def.Formatting(row_field.FindChild("even.Hover", null, true, true, true, '.'));
				this.even_selected = new TableDataProvider.RowData.Def.Formatting(row_field.FindChild("even.Selected", null, true, true, true, '.'));
				this.even_disbaled = new TableDataProvider.RowData.Def.Formatting(row_field.FindChild("even.Disbaled", null, true, true, true, '.'));
				this.horizontal_spacing = this.field.GetFloat("horizontal_spacing", null, this.horizontal_spacing, true, true, true, '.');
				this.vertical_spacing = this.field.GetFloat("vertical_spacing", null, this.vertical_spacing, true, true, true, '.');
				this.odd_line_sprite = global::Defs.GetObj<Sprite>(this.field, "odd_line_sprite", null);
				this.even_line_sprite = global::Defs.GetObj<Sprite>(this.field, "even_line_sprite", null);
				this.odd_line_color = global::Defs.GetColor(this.field, "odd_line_color", null);
				this.even_line_color = global::Defs.GetColor(this.field, "even_line_color ", null);
				this.LoadPadding(this.field.FindChild("padding", null, true, true, true, '.'), ref this.padding);
				return true;
			}

			// Token: 0x060055C3 RID: 21955 RVA: 0x0024A404 File Offset: 0x00248604
			private void LoadPadding(DT.Field field, ref RectOffset p)
			{
				if (field == null)
				{
					return;
				}
				p.top = field.GetInt("top", null, p.top, true, true, true, '.');
				p.left = field.GetInt("left", null, p.left, true, true, true, '.');
				p.right = field.GetInt("right", null, p.right, true, true, true, '.');
				p.bottom = field.GetInt("bottom", null, p.bottom, true, true, true, '.');
			}

			// Token: 0x040046B9 RID: 18105
			public GameObject prefab;

			// Token: 0x040046BA RID: 18106
			public GameObject empty_prefab;

			// Token: 0x040046BB RID: 18107
			public Sprite background;

			// Token: 0x040046BC RID: 18108
			public TableDataProvider.RowData.Def.Formatting odd_normal;

			// Token: 0x040046BD RID: 18109
			public TableDataProvider.RowData.Def.Formatting odd_hover;

			// Token: 0x040046BE RID: 18110
			public TableDataProvider.RowData.Def.Formatting odd_selected;

			// Token: 0x040046BF RID: 18111
			public TableDataProvider.RowData.Def.Formatting odd_disbaled;

			// Token: 0x040046C0 RID: 18112
			public TableDataProvider.RowData.Def.Formatting even_normal;

			// Token: 0x040046C1 RID: 18113
			public TableDataProvider.RowData.Def.Formatting even_hover;

			// Token: 0x040046C2 RID: 18114
			public TableDataProvider.RowData.Def.Formatting even_selected;

			// Token: 0x040046C3 RID: 18115
			public TableDataProvider.RowData.Def.Formatting even_disbaled;

			// Token: 0x040046C4 RID: 18116
			public float horizontal_spacing = 4f;

			// Token: 0x040046C5 RID: 18117
			public float vertical_spacing = 4f;

			// Token: 0x040046C6 RID: 18118
			public Sprite odd_line_sprite;

			// Token: 0x040046C7 RID: 18119
			public Sprite even_line_sprite;

			// Token: 0x040046C8 RID: 18120
			public Color odd_line_color;

			// Token: 0x040046C9 RID: 18121
			public Color even_line_color;

			// Token: 0x040046CA RID: 18122
			public RectOffset padding = new RectOffset(0, 0, 0, 0);

			// Token: 0x040046CB RID: 18123
			public DT.Field field;

			// Token: 0x02000A50 RID: 2640
			public class Formatting
			{
				// Token: 0x0600563D RID: 22077 RVA: 0x0024E13B File Offset: 0x0024C33B
				public Formatting(DT.Field cell_formatting)
				{
					this.Load(cell_formatting);
				}

				// Token: 0x0600563E RID: 22078 RVA: 0x0024E156 File Offset: 0x0024C356
				public bool Load(DT.Field cell_formatting)
				{
					if (cell_formatting == null)
					{
						return false;
					}
					this.tint = global::Defs.GetColor(cell_formatting, "tint", this.tint, null);
					return true;
				}

				// Token: 0x04004756 RID: 18262
				public Color tint = Color.white;
			}
		}
	}

	// Token: 0x0200081B RID: 2075
	public class ColumnData
	{
		// Token: 0x06004F93 RID: 20371 RVA: 0x002360A2 File Offset: 0x002342A2
		public void Parse(DT.Field columnField, int index)
		{
			this.def = new TableDataProvider.ColumnData.Def();
			this.def.Load(columnField);
			this.index = index;
		}

		// Token: 0x06004F94 RID: 20372 RVA: 0x002360C3 File Offset: 0x002342C3
		public GameObject GetIconPrefab()
		{
			return global::Defs.GetObj<GameObject>(this.def.field, "cell_icon_value.prefab", null);
		}

		// Token: 0x06004F95 RID: 20373 RVA: 0x002360DB File Offset: 0x002342DB
		public GameObject GetValuePrefab()
		{
			return global::Defs.GetObj<GameObject>(this.def.field, "cell_value.prefab", null);
		}

		// Token: 0x06004F96 RID: 20374 RVA: 0x002360F4 File Offset: 0x002342F4
		public Value GetSortValue(Value target, TableDataProvider.RowData row = null)
		{
			if (!target.is_valid)
			{
				return Value.Null;
			}
			Vars vars = new Vars(target);
			DT.Field field;
			if (this.def.useNonChachedFields)
			{
				field = this.def.field.FindChild("sort_value", new Vars(target), true, true, true, '.');
			}
			else
			{
				TableDataProvider.ColumnData.Def def = this.def;
				field = ((def != null) ? def.sort_value : null);
			}
			if (field == null)
			{
				return Value.Null;
			}
			Value value = field.Value(vars, true, true);
			if (value.type != Value.Type.String)
			{
				return value;
			}
			string a = field.String(null, "");
			if (a == "castle_name")
			{
				Logic.Realm realm = target.obj_val as Logic.Realm;
				global::Settlement settlement = ((realm != null) ? realm.castle.visuals : null) as global::Settlement;
				return (settlement != null) ? settlement.GetLocalziedCastle() : null;
			}
			if (a == "cell_value" && row != null)
			{
				return this.GetCellValue(row);
			}
			return global::Defs.Localize(field, new Vars(target), null, true, true);
		}

		// Token: 0x06004F97 RID: 20375 RVA: 0x002361F4 File Offset: 0x002343F4
		private Value GetCellValue(TableDataProvider.RowData row)
		{
			if (row == null)
			{
				return "";
			}
			if (row.cells == null || row.cells.Count <= this.index)
			{
				return "";
			}
			return row.cells[this.index].GetLocalziedText();
		}

		// Token: 0x06004F98 RID: 20376 RVA: 0x00236250 File Offset: 0x00234450
		public float GetIconWidth()
		{
			return this.def.field.GetFloat("width", null, 0f, true, true, true, '.');
		}

		// Token: 0x06004F99 RID: 20377 RVA: 0x00236272 File Offset: 0x00234472
		public float GetIconHeight()
		{
			return this.def.field.GetFloat("height", null, 0f, true, true, true, '.');
		}

		// Token: 0x04003DCA RID: 15818
		public TableDataProvider.ColumnData.Def def;

		// Token: 0x04003DCB RID: 15819
		public int index;

		// Token: 0x02000A36 RID: 2614
		public class Def
		{
			// Token: 0x060055C5 RID: 21957 RVA: 0x0024A4C0 File Offset: 0x002486C0
			public bool Load(DT.Field column_field)
			{
				this.field = column_field;
				this.name = column_field.key;
				this.width = column_field.GetFloat("width", null, this.width, true, true, true, '.');
				this.header_height = column_field.GetFloat("header_height", null, this.header_height, true, true, true, '.');
				this.row_height = column_field.GetFloat("row_height", null, this.row_height, true, true, true, '.');
				this.hide_if_empty = column_field.GetBool("hide_if_empty", null, this.hide_if_empty, true, true, true, '.');
				this.sort_method = column_field.GetString("sort_method", null, this.sort_method, true, true, true, '.');
				this.cell_prefab = global::Defs.GetObj<GameObject>(column_field, "cell_prefab", null);
				this.header_prefab = global::Defs.GetObj<GameObject>(column_field, "header_prefab", null);
				this.icon = global::Defs.GetObj<Sprite>(column_field, "icon", null);
				DT.Field field = column_field.FindChild("icon", null, true, true, true, '.');
				this.icon_show_condition = ((field != null) ? field.FindChild("show_condition", null, true, true, true, '.') : null);
				this.icon_width = ((field != null) ? field.GetFloat("width", null, this.icon_width, true, true, true, '.') : 0f);
				this.icon_height = ((field != null) ? field.GetFloat("height", null, this.icon_width, true, true, true, '.') : 0f);
				this.header_icon = global::Defs.GetObj<Sprite>(column_field, "header_icon", null);
				this.header_background = global::Defs.GetObj<Sprite>(column_field, "header_background", null);
				this.header_background_over = global::Defs.GetObj<Sprite>(column_field, "header_background_over", null);
				this.header_background_select = global::Defs.GetObj<Sprite>(column_field, "header_background_select", null);
				this.cell_background = global::Defs.GetObj<Sprite>(column_field, "cell_background", null);
				this.cell_normal = new TableDataProvider.ColumnData.Def.Formating(column_field.FindChild("cell_formatting.Normal", null, true, true, true, '.'));
				this.cell_hover = new TableDataProvider.ColumnData.Def.Formating(column_field.FindChild("cell_formatting.Hover", null, true, true, true, '.'));
				this.cell_selected = new TableDataProvider.ColumnData.Def.Formating(column_field.FindChild("cell_formatting.Selected", null, true, true, true, '.'));
				this.cell_disbaled = new TableDataProvider.ColumnData.Def.Formating(column_field.FindChild("cell_formatting.Disbaled", null, true, true, true, '.'));
				this.header_normal = new TableDataProvider.ColumnData.Def.Formating(column_field.FindChild("header_formatting.Normal", null, true, true, true, '.'));
				this.header_hover = new TableDataProvider.ColumnData.Def.Formating(column_field.FindChild("header_formatting.Hover", null, true, true, true, '.'));
				this.header_selected = new TableDataProvider.ColumnData.Def.Formating(column_field.FindChild("header_formatting.Selected", null, true, true, true, '.'));
				this.header_disbaled = new TableDataProvider.ColumnData.Def.Formating(column_field.FindChild("header_formatting.Disbaled", null, true, true, true, '.'));
				this.cell_icon_tooltip = column_field.GetString("icon.tooltip", null, "", true, true, true, '.');
				this.cell_value = this.field.FindChild("cell_value", null, true, true, true, '.');
				this.cell_icon_value = this.field.FindChild("cell_icon_value", null, true, true, true, '.');
				this.sort_value = this.field.FindChild("sort_value", null, true, true, true, '.');
				this.update_interval = column_field.GetFloat("update_interval", null, this.update_interval, true, true, true, '.');
				this.useNonChachedFields = this.field.HasCases(true, true);
				return true;
			}

			// Token: 0x060055C6 RID: 21958 RVA: 0x0024A800 File Offset: 0x00248A00
			public override string ToString()
			{
				return string.Format("Table: {0}, width {1}, sort_method: {2}, icon: {3}, cell_prefab: {4}", new object[]
				{
					this.name,
					this.width,
					this.sort_method,
					this.icon,
					this.cell_prefab
				});
			}

			// Token: 0x040046CC RID: 18124
			public GameObject cell_prefab;

			// Token: 0x040046CD RID: 18125
			public GameObject header_prefab;

			// Token: 0x040046CE RID: 18126
			public bool hide_if_empty;

			// Token: 0x040046CF RID: 18127
			public float width = 160f;

			// Token: 0x040046D0 RID: 18128
			public Sprite icon;

			// Token: 0x040046D1 RID: 18129
			public Sprite header_icon;

			// Token: 0x040046D2 RID: 18130
			public DT.Field icon_show_condition;

			// Token: 0x040046D3 RID: 18131
			public float icon_width;

			// Token: 0x040046D4 RID: 18132
			public float icon_height;

			// Token: 0x040046D5 RID: 18133
			public Sprite header_background;

			// Token: 0x040046D6 RID: 18134
			public Sprite header_background_over;

			// Token: 0x040046D7 RID: 18135
			public Sprite header_background_select;

			// Token: 0x040046D8 RID: 18136
			public Sprite cell_background;

			// Token: 0x040046D9 RID: 18137
			public string sort_method = "Ascending";

			// Token: 0x040046DA RID: 18138
			public string name;

			// Token: 0x040046DB RID: 18139
			public DT.Field field;

			// Token: 0x040046DC RID: 18140
			public float update_interval = 1f;

			// Token: 0x040046DD RID: 18141
			public float header_height = 60f;

			// Token: 0x040046DE RID: 18142
			public float row_height = 30f;

			// Token: 0x040046DF RID: 18143
			public TableDataProvider.ColumnData.Def.Formating cell_normal;

			// Token: 0x040046E0 RID: 18144
			public TableDataProvider.ColumnData.Def.Formating cell_hover;

			// Token: 0x040046E1 RID: 18145
			public TableDataProvider.ColumnData.Def.Formating cell_selected;

			// Token: 0x040046E2 RID: 18146
			public TableDataProvider.ColumnData.Def.Formating cell_disbaled;

			// Token: 0x040046E3 RID: 18147
			public TableDataProvider.ColumnData.Def.Formating header_normal;

			// Token: 0x040046E4 RID: 18148
			public TableDataProvider.ColumnData.Def.Formating header_hover;

			// Token: 0x040046E5 RID: 18149
			public TableDataProvider.ColumnData.Def.Formating header_selected;

			// Token: 0x040046E6 RID: 18150
			public TableDataProvider.ColumnData.Def.Formating header_disbaled;

			// Token: 0x040046E7 RID: 18151
			public DT.Field cell_value;

			// Token: 0x040046E8 RID: 18152
			public DT.Field cell_icon_value;

			// Token: 0x040046E9 RID: 18153
			public DT.Field sort_value;

			// Token: 0x040046EA RID: 18154
			public string cell_icon_tooltip;

			// Token: 0x040046EB RID: 18155
			public bool useNonChachedFields = true;

			// Token: 0x02000A51 RID: 2641
			public class Formating
			{
				// Token: 0x0600563F RID: 22079 RVA: 0x0024E178 File Offset: 0x0024C378
				public Formating(DT.Field cell_formatting)
				{
					this.Load(cell_formatting);
				}

				// Token: 0x06005640 RID: 22080 RVA: 0x0024E1F4 File Offset: 0x0024C3F4
				public bool Load(DT.Field cell_formatting)
				{
					if (cell_formatting == null)
					{
						return false;
					}
					Enum.TryParse<TextAnchor>(cell_formatting.GetString("alignment", null, "", true, true, true, '.'), out this.cell_alignment);
					Enum.TryParse<TextAlignmentOptions>(cell_formatting.GetString("text_alignment", null, "", true, true, true, '.'), out this.text_alignment);
					this.tint = global::Defs.GetColor(cell_formatting, "tint", this.tint, null);
					this.text_tint = global::Defs.GetColor(cell_formatting, "text_tint", this.tint, null);
					this.text_size = cell_formatting.GetFloat("text_size", null, this.text_size, true, true, true, '.');
					this.text_size_min = cell_formatting.GetFloat("text_min_size", null, this.text_size_min, true, true, true, '.');
					this.auto_sizing = cell_formatting.GetBool("auto_sizing", null, this.auto_sizing, true, true, true, '.');
					this.LoadPadding(cell_formatting.FindChild("padding", null, true, true, true, '.'), ref this.padding);
					return true;
				}

				// Token: 0x06005641 RID: 22081 RVA: 0x0024E2F0 File Offset: 0x0024C4F0
				private void LoadPadding(DT.Field cell_formatting, ref RectOffset p)
				{
					if (cell_formatting == null)
					{
						return;
					}
					p.top = cell_formatting.GetInt("top", null, p.top, true, true, true, '.');
					p.left = cell_formatting.GetInt("left", null, p.left, true, true, true, '.');
					p.right = cell_formatting.GetInt("right", null, p.right, true, true, true, '.');
					p.bottom = cell_formatting.GetInt("bottom", null, p.bottom, true, true, true, '.');
				}

				// Token: 0x04004757 RID: 18263
				public TextAnchor cell_alignment = TextAnchor.MiddleCenter;

				// Token: 0x04004758 RID: 18264
				public TextAlignmentOptions text_alignment = TextAlignmentOptions.MidlineRight;

				// Token: 0x04004759 RID: 18265
				public Color tint = Color.white;

				// Token: 0x0400475A RID: 18266
				public Color text_tint = Color.black;

				// Token: 0x0400475B RID: 18267
				public float text_size = 16f;

				// Token: 0x0400475C RID: 18268
				public float text_size_min = 10f;

				// Token: 0x0400475D RID: 18269
				public bool auto_sizing = true;

				// Token: 0x0400475E RID: 18270
				public float max_text_size = 50f;

				// Token: 0x0400475F RID: 18271
				public RectOffset padding = new RectOffset(0, 0, 0, 0);
			}
		}
	}

	// Token: 0x0200081C RID: 2076
	public class CellData
	{
		// Token: 0x06004F9B RID: 20379 RVA: 0x00236294 File Offset: 0x00234494
		public string GetLocalziedText()
		{
			if (!this.target.is_valid)
			{
				return null;
			}
			DT.Field field;
			if (this.settings.def.useNonChachedFields)
			{
				field = this.settings.def.field.FindChild("cell_value", new Vars(this.target), true, true, true, '.');
			}
			else
			{
				TableDataProvider.ColumnData columnData = this.settings;
				DT.Field field2;
				if (columnData == null)
				{
					field2 = null;
				}
				else
				{
					TableDataProvider.ColumnData.Def def = columnData.def;
					field2 = ((def != null) ? def.cell_value : null);
				}
				field = field2;
			}
			if (field == null)
			{
				return null;
			}
			return global::Defs.Localize(field, new Vars(this.target), null, true, true);
		}

		// Token: 0x06004F9C RID: 20380 RVA: 0x00236328 File Offset: 0x00234528
		public Value GetIconValue()
		{
			if (!this.target.is_valid)
			{
				return Value.Null;
			}
			Vars vars = new Vars(this.target);
			DT.Field field = this.settings.def.field.FindChild("cell_icon_value", vars, true, true, true, '.');
			if (field == null)
			{
				return Value.Null;
			}
			DT.Field field2 = field.FindChild("show_condition", vars, true, true, true, '.');
			if (field2 != null && !field2.Bool(vars, false))
			{
				return Value.Null;
			}
			return field.Value(vars, true, true);
		}

		// Token: 0x06004F9D RID: 20381 RVA: 0x002363AC File Offset: 0x002345AC
		public Sprite GetStaticIcon()
		{
			Vars vars = new Vars(this.target);
			DT.Field field = this.settings.def.field.FindChild("icon", vars, true, true, true, '.');
			if (field == null)
			{
				return null;
			}
			DT.Field field2 = field.FindChild("show_condition", vars, true, true, true, '.');
			if (field2 != null && !field2.Bool(vars, false))
			{
				return null;
			}
			return global::Defs.GetObj<Sprite>(field, null);
		}

		// Token: 0x06004F9E RID: 20382 RVA: 0x00236414 File Offset: 0x00234614
		public string GetIconVariant()
		{
			if (!this.target.is_valid)
			{
				return string.Empty;
			}
			DT.Field field = this.settings.def.field.FindChild("cell_icon_value", this.vars, true, true, true, '.');
			if (field == null)
			{
				return null;
			}
			DT.Field field2 = field.FindChild("variant", null, true, true, true, '.');
			if (field2 == null)
			{
				return string.Empty;
			}
			return field2.String(null, "");
		}

		// Token: 0x06004F9F RID: 20383 RVA: 0x00236488 File Offset: 0x00234688
		public Vector2 GetIconSize()
		{
			if (!this.target.is_valid)
			{
				return new Vector2(0f, 0f);
			}
			Vars vars = new Vars(this.target);
			DT.Field field = this.settings.def.field.FindChild("cell_icon_value", vars, true, true, true, '.');
			if (field == null)
			{
				return new Vector2(0f, 0f);
			}
			return new Vector2(field.GetFloat("width", null, 0f, true, true, true, '.'), field.GetFloat("height", null, 0f, true, true, true, '.'));
		}

		// Token: 0x06004FA0 RID: 20384 RVA: 0x00236523 File Offset: 0x00234723
		public override string ToString()
		{
			return string.Format("CellData{0} {1}", this.settings.def, this.target);
		}

		// Token: 0x04003DCC RID: 15820
		public TableDataProvider.ColumnData settings;

		// Token: 0x04003DCD RID: 15821
		public Value target;

		// Token: 0x04003DCE RID: 15822
		public Vars vars;
	}
}
