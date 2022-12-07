using System;
using UnityEngine;

// Token: 0x02000017 RID: 23
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class CameraDepthMode : MonoBehaviour
{
	// Token: 0x0600003C RID: 60 RVA: 0x00003421 File Offset: 0x00001621
	private void Start()
	{
		base.GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
	}

	// Token: 0x0600003D RID: 61 RVA: 0x00003421 File Offset: 0x00001621
	private void OnEnable()
	{
		base.GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
	}

	// Token: 0x0600003E RID: 62 RVA: 0x0000342F File Offset: 0x0000162F
	private void OnDisable()
	{
		base.GetComponent<Camera>().depthTextureMode = DepthTextureMode.None;
	}
}
