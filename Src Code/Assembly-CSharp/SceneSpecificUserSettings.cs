using System;
using UnityEngine;

// Token: 0x02000177 RID: 375
public class SceneSpecificUserSettings : MonoBehaviour
{
	// Token: 0x06001320 RID: 4896 RVA: 0x000C7398 File Offset: 0x000C5598
	private void OnEnable()
	{
		UserSettings.goOcean = GameObject.Find("Ocean");
		UserSettings.goPPVs = GameObject.Find("PPV");
		UserSettings.SetAntialiasing(UserSettings.AntialiasingModeStr);
		UserSettings.SetReflectionsToggle(UserSettings.GetSetting("reflections"), UserSettings.Reflecitons);
		UserSettings.SetSSRTQuality(UserSettings.SSRTStr);
		UserSettings.SetSSAOToggle(UserSettings.SSAO);
		UserSettings.SetFogToggle(UserSettings.Fog);
		UserSettings.SetDepthOfFieldToggle(UserSettings.DepthOfField);
		UserSettings.SetBloomToggle(UserSettings.Bloom);
		UserSettings.UpdateCameraDepthMode();
	}
}
