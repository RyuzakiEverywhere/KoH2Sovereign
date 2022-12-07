using System;
using UnityEngine;

// Token: 0x02000027 RID: 39
public class FlockScare : MonoBehaviour
{
	// Token: 0x060000AD RID: 173 RVA: 0x00006730 File Offset: 0x00004930
	private void CheckProximityToLandingSpots()
	{
		this.IterateLandingSpots();
		if (this.currentController._activeLandingSpots > 0 && this.CheckDistanceToLandingSpot(this.landingSpotControllers[this.lsc]))
		{
			this.landingSpotControllers[this.lsc].ScareAll();
		}
		base.Invoke("CheckProximityToLandingSpots", this.scareInterval);
	}

	// Token: 0x060000AE RID: 174 RVA: 0x0000678C File Offset: 0x0000498C
	private void IterateLandingSpots()
	{
		this.ls += this.checkEveryNthLandingSpot;
		this.currentController = this.landingSpotControllers[this.lsc];
		int childCount = this.currentController.transform.childCount;
		if (this.ls > childCount - 1)
		{
			this.ls -= childCount;
			if (this.lsc < this.landingSpotControllers.Length - 1)
			{
				this.lsc++;
				return;
			}
			this.lsc = 0;
		}
	}

	// Token: 0x060000AF RID: 175 RVA: 0x00006814 File Offset: 0x00004A14
	private bool CheckDistanceToLandingSpot(LandingSpotController lc)
	{
		Transform child = lc.transform.GetChild(this.ls);
		return child.GetComponent<LandingSpot>().landingChild != null && (child.position - base.transform.position).sqrMagnitude < this.distanceToScare * this.distanceToScare;
	}

	// Token: 0x060000B0 RID: 176 RVA: 0x00006878 File Offset: 0x00004A78
	private void Invoker()
	{
		for (int i = 0; i < this.InvokeAmounts; i++)
		{
			float num = this.scareInterval / (float)this.InvokeAmounts * (float)i;
			base.Invoke("CheckProximityToLandingSpots", this.scareInterval + num);
		}
	}

	// Token: 0x060000B1 RID: 177 RVA: 0x000068BB File Offset: 0x00004ABB
	private void OnEnable()
	{
		base.CancelInvoke("CheckProximityToLandingSpots");
		if (this.landingSpotControllers.Length != 0)
		{
			this.Invoker();
		}
	}

	// Token: 0x060000B2 RID: 178 RVA: 0x000068D7 File Offset: 0x00004AD7
	private void OnDisable()
	{
		base.CancelInvoke("CheckProximityToLandingSpots");
	}

	// Token: 0x0400011A RID: 282
	public LandingSpotController[] landingSpotControllers;

	// Token: 0x0400011B RID: 283
	public float scareInterval = 0.1f;

	// Token: 0x0400011C RID: 284
	public float distanceToScare = 2f;

	// Token: 0x0400011D RID: 285
	public int checkEveryNthLandingSpot = 1;

	// Token: 0x0400011E RID: 286
	public int InvokeAmounts = 1;

	// Token: 0x0400011F RID: 287
	private int lsc;

	// Token: 0x04000120 RID: 288
	private int ls;

	// Token: 0x04000121 RID: 289
	private LandingSpotController currentController;
}
