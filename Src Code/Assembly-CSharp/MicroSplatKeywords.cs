using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000059 RID: 89
public class MicroSplatKeywords : ScriptableObject
{
	// Token: 0x0600020D RID: 525 RVA: 0x0001FC20 File Offset: 0x0001DE20
	public bool IsKeywordEnabled(string k)
	{
		return this.keywords.Contains(k);
	}

	// Token: 0x0600020E RID: 526 RVA: 0x0001FC2E File Offset: 0x0001DE2E
	public void EnableKeyword(string k)
	{
		if (!this.IsKeywordEnabled(k))
		{
			this.keywords.Add(k);
		}
	}

	// Token: 0x0600020F RID: 527 RVA: 0x0001FC45 File Offset: 0x0001DE45
	public void DisableKeyword(string k)
	{
		if (this.IsKeywordEnabled(k))
		{
			this.keywords.Remove(k);
		}
	}

	// Token: 0x0400031B RID: 795
	public List<string> keywords = new List<string>();
}
