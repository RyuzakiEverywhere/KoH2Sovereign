using System;
using UnityEngine;

// Token: 0x02000328 RID: 808
[ExecuteInEditMode]
public class ImportSettlements : MonoBehaviour
{
	// Token: 0x04002187 RID: 8583
	public string MapName = "Europe";

	// Token: 0x04002188 RID: 8584
	public GameObject CastlePrefab;

	// Token: 0x04002189 RID: 8585
	public GameObject VillagePrefab;

	// Token: 0x0400218A RID: 8586
	public GameObject FarmPrefab;

	// Token: 0x0400218B RID: 8587
	public Vector2 OldTerrainSize = new Vector2(3000f, 2400f);

	// Token: 0x0400218C RID: 8588
	public Vector2 NewTerrainSize = new Vector2(2400f, 1920f);
}
