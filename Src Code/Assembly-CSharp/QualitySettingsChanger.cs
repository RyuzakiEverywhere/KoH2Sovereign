using System;
using UnityEngine;

// Token: 0x020000E2 RID: 226
public class QualitySettingsChanger : MonoBehaviour
{
	// Token: 0x06000B53 RID: 2899 RVA: 0x000809C7 File Offset: 0x0007EBC7
	private void Start()
	{
		this.Apply();
	}

	// Token: 0x06000B54 RID: 2900 RVA: 0x000809D0 File Offset: 0x0007EBD0
	public void Apply()
	{
		if (string.IsNullOrEmpty(this.level))
		{
			return;
		}
		int num = Array.IndexOf<string>(QualitySettings.names, this.level);
		if (num < 0)
		{
			return;
		}
		QualitySettings.SetQualityLevel(num, true);
	}

	// Token: 0x040008C0 RID: 2240
	public string level = "";
}
