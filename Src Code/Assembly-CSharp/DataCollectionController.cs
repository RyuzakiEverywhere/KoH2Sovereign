using System;
using UnityEngine;

// Token: 0x020002CE RID: 718
public class DataCollectionController : MonoBehaviour
{
	// Token: 0x06002CFA RID: 11514 RVA: 0x00175DE1 File Offset: 0x00173FE1
	public void OnAllLoaded()
	{
		if (!UserSettings.DataCollectionAsked && UIDataCollectionPromptWindow.Create(this.windowParent) == null)
		{
			Debug.LogWarning("Failed to create data collection prompt. Data collection will be disabled for this session.");
			UserSettings.SetDataCollection(false);
			UserSettings.SetDataCollectionAsked(true);
		}
	}

	// Token: 0x04001EB2 RID: 7858
	[SerializeField]
	private RectTransform windowParent;
}
