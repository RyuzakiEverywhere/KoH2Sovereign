using System;
using UnityEngine;

// Token: 0x02000151 RID: 337
public class TerrainSnap : MonoBehaviour
{
	// Token: 0x0600116B RID: 4459 RVA: 0x000B753C File Offset: 0x000B573C
	public override string ToString()
	{
		return Common.ToString<TerrainSnap>(this);
	}

	// Token: 0x0600116C RID: 4460 RVA: 0x000023FD File Offset: 0x000005FD
	private void Log(string msg)
	{
	}

	// Token: 0x0600116D RID: 4461 RVA: 0x000B7544 File Offset: 0x000B5744
	private void OnEnable()
	{
		if (TerrainSnap.do_not_spawn)
		{
			Object.DestroyImmediate(this);
			return;
		}
	}

	// Token: 0x0600116E RID: 4462 RVA: 0x000B7554 File Offset: 0x000B5754
	private void Start()
	{
		if (Application.isPlaying)
		{
			if (this.child)
			{
				this.RestoreHeight();
			}
			base.enabled = false;
			return;
		}
	}

	// Token: 0x0600116F RID: 4463 RVA: 0x000B7573 File Offset: 0x000B5773
	public void RestoreHeight()
	{
		Common.SnapToTerrain(base.transform, this.height, null, -1f);
	}

	// Token: 0x04000B8A RID: 2954
	public bool child;

	// Token: 0x04000B8B RID: 2955
	public float height;

	// Token: 0x04000B8C RID: 2956
	public static bool analyzing;

	// Token: 0x04000B8D RID: 2957
	public static bool do_not_spawn;
}
