using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000045 RID: 69
public class CameraPresets : ScriptableObject
{
	// Token: 0x06000193 RID: 403 RVA: 0x0000FC5C File Offset: 0x0000DE5C
	[ContextMenu("Add camera preset from camera")]
	public void AddCameraPresetFromCamera()
	{
		GameCamera gameCamera = CameraController.GameCamera;
		CameraPresets.CameraPreset item = new CameraPresets.CameraPreset(gameCamera.transform.position, gameCamera.transform.rotation);
		this.presets.Add(item);
	}

	// Token: 0x06000194 RID: 404 RVA: 0x0000FC97 File Offset: 0x0000DE97
	public void ApplyCameraPreset(int i)
	{
		CameraController.GameCamera.SetScheme("FreeLook");
		CameraController.Set(this.presets[i].position, this.presets[i].rotation);
	}

	// Token: 0x040002B1 RID: 689
	public List<CameraPresets.CameraPreset> presets = new List<CameraPresets.CameraPreset>();

	// Token: 0x02000501 RID: 1281
	[Serializable]
	public class CameraPreset
	{
		// Token: 0x0600426A RID: 17002 RVA: 0x001F8C9E File Offset: 0x001F6E9E
		public CameraPreset(Vector3 point, Quaternion rotation)
		{
			this.position = point;
			this.rotation = rotation;
		}

		// Token: 0x04002E7F RID: 11903
		public Vector3 position;

		// Token: 0x04002E80 RID: 11904
		public Quaternion rotation;
	}
}
