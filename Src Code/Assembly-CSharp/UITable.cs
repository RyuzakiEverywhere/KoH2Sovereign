using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002C7 RID: 711
public class UITable : MonoBehaviour
{
	// Token: 0x06002CA4 RID: 11428 RVA: 0x001749D3 File Offset: 0x00172BD3
	private void Awake()
	{
		UICommon.FindComponents(this, false);
		this.Init();
		this.SetRows();
		this.SetColors();
	}

	// Token: 0x06002CA5 RID: 11429 RVA: 0x001749EE File Offset: 0x00172BEE
	public void AddRow(UITableRow tr)
	{
		this.rows.Add(tr);
		if (this.TableContent != null)
		{
			tr.transform.SetParent(this.TableContent.transform, false);
		}
	}

	// Token: 0x06002CA6 RID: 11430 RVA: 0x00174A21 File Offset: 0x00172C21
	public void SetRows()
	{
		this.rows = new List<UITableRow>(base.GetComponentsInChildren<UITableRow>());
	}

	// Token: 0x06002CA7 RID: 11431 RVA: 0x00174A34 File Offset: 0x00172C34
	public void Clear()
	{
		this.rows = new List<UITableRow>();
	}

	// Token: 0x06002CA8 RID: 11432 RVA: 0x00174A44 File Offset: 0x00172C44
	public void SetColors()
	{
		for (int i = 0; i < this.rows.Count; i++)
		{
			List<UITableCell> cells = this.rows[i].GetCells();
			Color cellBackground = (i % 2 == 0) ? this.evenColor : this.oddColor;
			for (int j = 0; j < cells.Count; j++)
			{
				cells[j].SetCellBackground(cellBackground);
			}
		}
	}

	// Token: 0x06002CA9 RID: 11433 RVA: 0x00174AAB File Offset: 0x00172CAB
	public List<UITableRow> GetRows()
	{
		return this.rows;
	}

	// Token: 0x06002CAA RID: 11434 RVA: 0x00174AB3 File Offset: 0x00172CB3
	public GameObject GetContainer()
	{
		return this.TableContent;
	}

	// Token: 0x06002CAB RID: 11435 RVA: 0x00174ABB File Offset: 0x00172CBB
	public void Refresh(bool refreshRow = true)
	{
		if (this.currentFilter != null)
		{
			this.SortRows(this.currentFilter, true, false);
		}
		this.SetColors();
	}

	// Token: 0x06002CAC RID: 11436 RVA: 0x00174ADC File Offset: 0x00172CDC
	public void SetupHeader()
	{
		if (this.Header != null)
		{
			GameObject gameObject = Common.FindChildByName(this.Header, "id_Rows", true, true);
			if (gameObject != null)
			{
				for (int i = 0; i < this.comparers.Count; i++)
				{
					Transform child = gameObject.transform.GetChild(i);
					UITableCell component = child.GetComponent<UITableCell>();
					Transform transform = child;
					Image image = null;
					if (component != null)
					{
						image = component.GetCellBackground();
						if (image != null)
						{
							transform = image.transform;
						}
					}
					Button button = transform.GetComponent<Button>();
					if (button == null)
					{
						button = child.gameObject.AddComponent<Button>();
						button.targetGraphic = image;
					}
					TableRowComparer comparer = this.comparers[i];
					button.onClick.AddListener(delegate()
					{
						this.SortRows(comparer, true, true);
					});
				}
			}
		}
	}

	// Token: 0x06002CAD RID: 11437 RVA: 0x00174BD6 File Offset: 0x00172DD6
	public void Init()
	{
		this.SetupHeader();
		this.SetColors();
	}

	// Token: 0x06002CAE RID: 11438 RVA: 0x00174BE4 File Offset: 0x00172DE4
	public bool IsSorting()
	{
		return this.sorting;
	}

	// Token: 0x06002CAF RID: 11439 RVA: 0x00174BEC File Offset: 0x00172DEC
	public void SetBorderSize(float borderSize, bool refresh = true)
	{
		this.borderSize = borderSize;
		if (refresh)
		{
			this.RefreshBorders(refresh);
		}
	}

	// Token: 0x06002CB0 RID: 11440 RVA: 0x00174C00 File Offset: 0x00172E00
	public void RefreshBorders(bool refresh = true)
	{
		for (int i = 0; i < this.rows.Count; i++)
		{
			this.rows[i].SetBorderSize(this.borderSize, refresh);
		}
	}

	// Token: 0x06002CB1 RID: 11441 RVA: 0x00174C3C File Offset: 0x00172E3C
	protected void SortRows(TableRowComparer comparer, bool refresh = true, bool changeSortOrder = true)
	{
		this.currentFilter = comparer;
		this.sorting = true;
		if (changeSortOrder)
		{
			comparer.descending = !comparer.descending;
		}
		this.rows.Sort(comparer);
		for (int i = 0; i < this.rows.Count; i++)
		{
			UITableRow uitableRow = this.rows[i];
			if (uitableRow != null)
			{
				uitableRow.transform.SetSiblingIndex(i);
			}
		}
		this.sorting = false;
		this.SetColors();
	}

	// Token: 0x06002CB2 RID: 11442 RVA: 0x00174CBC File Offset: 0x00172EBC
	public void SetComparisonFunctions(List<Comparison<UITableRow>> comparisonFunctions)
	{
		this.comparisonFunctions = comparisonFunctions;
		for (int i = 0; i < this.comparisonFunctions.Count; i++)
		{
			this.comparers.Add(new TableRowComparer(this.comparisonFunctions[i]));
		}
	}

	// Token: 0x06002CB3 RID: 11443 RVA: 0x00174D02 File Offset: 0x00172F02
	public void SetComparisonFunction(int column, Comparison<UITableRow> comparison)
	{
		if (this.Header == null)
		{
			return;
		}
		Common.FindChildByName(this.Header, "id_TableRowHeader", true, true) == null;
	}

	// Token: 0x04001E6C RID: 7788
	protected List<UITableRow> rows = new List<UITableRow>();

	// Token: 0x04001E6D RID: 7789
	protected List<Comparison<UITableRow>> comparisonFunctions = new List<Comparison<UITableRow>>();

	// Token: 0x04001E6E RID: 7790
	protected List<TableRowComparer> comparers = new List<TableRowComparer>();

	// Token: 0x04001E6F RID: 7791
	[UIFieldTarget("Content")]
	protected GameObject TableContent;

	// Token: 0x04001E70 RID: 7792
	[UIFieldTarget("Viewport")]
	protected GameObject TableViewport;

	// Token: 0x04001E71 RID: 7793
	[UIFieldTarget("id_GridHeader")]
	protected GameObject Header;

	// Token: 0x04001E72 RID: 7794
	public TableRowComparer currentFilter;

	// Token: 0x04001E73 RID: 7795
	protected bool sorting;

	// Token: 0x04001E74 RID: 7796
	public Color evenColor = new Color(0.73f, 0.73f, 0.73f);

	// Token: 0x04001E75 RID: 7797
	public Color oddColor = new Color(0.5f, 0.5f, 0.5f);

	// Token: 0x04001E76 RID: 7798
	public float borderSize = 1f;
}
