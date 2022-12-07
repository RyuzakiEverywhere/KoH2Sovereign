using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000116 RID: 278
[RequireComponent(typeof(SSRTQualityManager))]
public class SSRTQualityManager : MonoBehaviour
{
	// Token: 0x170000A4 RID: 164
	// (get) Token: 0x06000CA6 RID: 3238 RVA: 0x0008D7AC File Offset: 0x0008B9AC
	public static SSRTQualityManager ActiveInstance
	{
		get
		{
			return SSRTQualityManager.Instances.LastOrDefault<SSRTQualityManager>();
		}
	}

	// Token: 0x170000A5 RID: 165
	// (get) Token: 0x06000CA7 RID: 3239 RVA: 0x0008D7B8 File Offset: 0x0008B9B8
	private SSRT ssrt
	{
		get
		{
			return base.GetComponent<SSRT>();
		}
	}

	// Token: 0x170000A6 RID: 166
	// (get) Token: 0x06000CA8 RID: 3240 RVA: 0x0008D7C0 File Offset: 0x0008B9C0
	// (set) Token: 0x06000CA9 RID: 3241 RVA: 0x0008D7C8 File Offset: 0x0008B9C8
	public static SSRTQualityManager.QualityOption Quality
	{
		get
		{
			return SSRTQualityManager.quality;
		}
		set
		{
			if (SSRTQualityManager.quality == value)
			{
				return;
			}
			SSRTQualityManager.quality = value;
			foreach (SSRTQualityManager ssrtqualityManager in SSRTQualityManager.Instances)
			{
				if (ssrtqualityManager != null)
				{
					ssrtqualityManager.UpdateQuality();
				}
			}
		}
	}

	// Token: 0x06000CAA RID: 3242 RVA: 0x0008D82C File Offset: 0x0008BA2C
	private void OnEnable()
	{
		SSRTQualityManager.Instances.Add(this);
		this.UpdateQuality();
	}

	// Token: 0x06000CAB RID: 3243 RVA: 0x0008D83F File Offset: 0x0008BA3F
	private void OnDisable()
	{
		SSRTQualityManager.Instances.Remove(this);
	}

	// Token: 0x06000CAC RID: 3244 RVA: 0x0008D850 File Offset: 0x0008BA50
	private void UpdateQuality()
	{
		switch (SSRTQualityManager.Quality)
		{
		case SSRTQualityManager.QualityOption.None:
			this.DisableSSRT();
			return;
		case SSRTQualityManager.QualityOption.Low:
			this.SetLowQuality();
			return;
		case SSRTQualityManager.QualityOption.Medium:
			this.SetMediumQuality();
			return;
		case SSRTQualityManager.QualityOption.High:
			this.SetHighQuality();
			return;
		default:
			return;
		}
	}

	// Token: 0x06000CAD RID: 3245 RVA: 0x0008D895 File Offset: 0x0008BA95
	private void DisableSSRT()
	{
		this.ssrt.enabled = false;
	}

	// Token: 0x06000CAE RID: 3246 RVA: 0x0008D8A4 File Offset: 0x0008BAA4
	private void SetHighQuality()
	{
		this.ssrt.enabled = true;
		this.ssrt.resolutionDownscale = SSRT.ResolutionDownscale.Full;
		this.ssrt.rotationCount = 4;
		this.ssrt.stepCount = 8;
		this.ssrt.expStart = 0.62f;
		this.ssrt.temporalResponse = 0.3f;
	}

	// Token: 0x06000CAF RID: 3247 RVA: 0x0008D904 File Offset: 0x0008BB04
	private void SetMediumQuality()
	{
		this.ssrt.enabled = true;
		this.ssrt.resolutionDownscale = SSRT.ResolutionDownscale.Half;
		this.ssrt.rotationCount = 3;
		this.ssrt.stepCount = 8;
		this.ssrt.expStart = 0.62f;
		this.ssrt.temporalResponse = 0.2f;
	}

	// Token: 0x06000CB0 RID: 3248 RVA: 0x0008D964 File Offset: 0x0008BB64
	private void SetLowQuality()
	{
		this.ssrt.enabled = true;
		this.ssrt.resolutionDownscale = SSRT.ResolutionDownscale.Half;
		this.ssrt.rotationCount = 2;
		this.ssrt.stepCount = 6;
		this.ssrt.expStart = 0.4f;
		this.ssrt.temporalResponse = 0.1f;
	}

	// Token: 0x040009E1 RID: 2529
	private static List<SSRTQualityManager> Instances = new List<SSRTQualityManager>();

	// Token: 0x040009E2 RID: 2530
	private static SSRTQualityManager.QualityOption quality = SSRTQualityManager.QualityOption.High;

	// Token: 0x02000617 RID: 1559
	public enum QualityOption
	{
		// Token: 0x040033CC RID: 13260
		None,
		// Token: 0x040033CD RID: 13261
		Low,
		// Token: 0x040033CE RID: 13262
		Medium,
		// Token: 0x040033CF RID: 13263
		High
	}
}
