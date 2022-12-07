using System;
using UnityEngine;

// Token: 0x02000307 RID: 775
public class UILocalizeText : MonoBehaviour
{
	// Token: 0x06003072 RID: 12402 RVA: 0x00188DE5 File Offset: 0x00186FE5
	private void OnEnable()
	{
		Debug.LogError(Common.ObjPath(base.gameObject) + ": UILocalizeText behavior is obsolete, please remove it");
	}

	// Token: 0x04002085 RID: 8325
	public string key;
}
