using System;
using UnityEngine;

// Token: 0x02000029 RID: 41
public class LandingButtons : MonoBehaviour
{
	// Token: 0x060000B7 RID: 183 RVA: 0x0000698C File Offset: 0x00004B8C
	public void OnGUI()
	{
		GUI.Label(new Rect(20f, 20f, 125f, 18f), "Landing Spots: " + this._landingSpotController.transform.childCount);
		if (GUI.Button(new Rect(20f, 40f, 125f, 18f), "Scare All"))
		{
			this._landingSpotController.ScareAll();
		}
		if (GUI.Button(new Rect(20f, 60f, 125f, 18f), "Land In Reach"))
		{
			this._landingSpotController.LandAll();
		}
		if (GUI.Button(new Rect(20f, 80f, 125f, 18f), "Land Instant"))
		{
			base.StartCoroutine(this._landingSpotController.InstantLand(0.01f));
		}
		if (GUI.Button(new Rect(20f, 100f, 125f, 18f), "Destroy"))
		{
			this._flockController.destroyBirds();
		}
		GUI.Label(new Rect(20f, 120f, 125f, 18f), "Bird Amount: " + this._flockController._childAmount);
		this._flockController._childAmount = (int)GUI.HorizontalSlider(new Rect(20f, 140f, 125f, 18f), (float)this._flockController._childAmount, 0f, 250f);
	}

	// Token: 0x04000124 RID: 292
	public LandingSpotController _landingSpotController;

	// Token: 0x04000125 RID: 293
	public FlockController _flockController;

	// Token: 0x04000126 RID: 294
	public float hSliderValue = 250f;
}
