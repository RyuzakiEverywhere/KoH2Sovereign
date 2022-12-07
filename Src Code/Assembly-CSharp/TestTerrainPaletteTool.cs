using System;
using UnityEngine;

// Token: 0x02000334 RID: 820
[ExecuteInEditMode]
public class TestTerrainPaletteTool : MonoBehaviour
{
	// Token: 0x0600324E RID: 12878 RVA: 0x00198270 File Offset: 0x00196470
	private void Update()
	{
		if (this.generate)
		{
			this.generate = false;
			base.GetComponent<TestTerrainMerger>().GenerateAll();
		}
		if (this.resetOutput)
		{
			this.resetOutput = false;
			base.GetComponent<TestTerrainMerger>().ResetOutput();
		}
		if (this.takeAndSaveSnapshot)
		{
			this.takeAndSaveSnapshot = false;
			base.GetComponent<TestTerrainMerger>().TakeAndSaveSnapshotToFileAndPNGs(this.snapshotPath, this.tdPalette, this.snapshotBounds);
		}
		if (this.pasteNaturalFormSnapshot)
		{
			this.pasteNaturalFormSnapshot = false;
			base.GetComponent<TestTerrainMerger>().PasteNaturalFormSnapshotFromFile(this.snapshotPath, this.tdDestination, this.pasteBounds);
		}
	}

	// Token: 0x0600324F RID: 12879 RVA: 0x00198309 File Offset: 0x00196509
	public void ResetDestination()
	{
		base.GetComponent<TestTerrainMerger>().ResetOutput();
	}

	// Token: 0x040021D0 RID: 8656
	public bool resetOutput;

	// Token: 0x040021D1 RID: 8657
	public bool generate;

	// Token: 0x040021D2 RID: 8658
	[Header("Save/Load snapshot")]
	public bool takeAndSaveSnapshot;

	// Token: 0x040021D3 RID: 8659
	public bool pasteNaturalFormSnapshot;

	// Token: 0x040021D4 RID: 8660
	public TerrainData tdPalette;

	// Token: 0x040021D5 RID: 8661
	public Bounds snapshotBounds;

	// Token: 0x040021D6 RID: 8662
	public string snapshotPath;

	// Token: 0x040021D7 RID: 8663
	public string snapshotPathRelative;

	// Token: 0x040021D8 RID: 8664
	public TerrainData tdDestination;

	// Token: 0x040021D9 RID: 8665
	public Bounds pasteBounds;

	// Token: 0x040021DA RID: 8666
	private ShapeSnapshot shot;
}
