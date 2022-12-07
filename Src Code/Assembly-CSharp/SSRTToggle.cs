using System;
using UnityEngine;

// Token: 0x02000065 RID: 101
public class SSRTToggle : MonoBehaviour
{
	// Token: 0x0600025C RID: 604 RVA: 0x00022A08 File Offset: 0x00020C08
	private void Start()
	{
		this.ssrt = base.GetComponent<SSRT>();
	}

	// Token: 0x0600025D RID: 605 RVA: 0x00022A18 File Offset: 0x00020C18
	private void Update()
	{
		if (this.ssrt)
		{
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				this.ssrt.enabled = true;
				this.ssrt.debugMode = SSRT.DebugMode.Combined;
				this.ssrt.lightOnly = false;
				this.ssrt.directLightingAO = false;
			}
			if (Input.GetKeyDown(KeyCode.Alpha3))
			{
				this.ssrt.enabled = true;
				this.ssrt.debugMode = SSRT.DebugMode.GI;
				this.ssrt.lightOnly = false;
			}
			if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				this.ssrt.enabled = false;
			}
			if (Input.GetKeyDown(KeyCode.Alpha4))
			{
				this.ssrt.enabled = true;
				this.ssrt.debugMode = SSRT.DebugMode.GI;
				this.ssrt.lightOnly = true;
			}
			if (Input.GetKeyDown(KeyCode.Alpha5))
			{
				this.ssrt.enabled = true;
				this.ssrt.debugMode = SSRT.DebugMode.Combined;
				this.ssrt.lightOnly = false;
				this.ssrt.directLightingAO = true;
			}
			if (Input.GetKeyDown(KeyCode.F))
			{
				this.ssrt.resolutionDownscale = SSRT.ResolutionDownscale.Full;
			}
			if (Input.GetKeyDown(KeyCode.H))
			{
				this.ssrt.resolutionDownscale = SSRT.ResolutionDownscale.Half;
			}
		}
	}

	// Token: 0x040003A8 RID: 936
	private SSRT ssrt;
}
