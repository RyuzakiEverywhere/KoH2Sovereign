using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200002B RID: 43
public class LandingSpotController : MonoBehaviour
{
	// Token: 0x060000C1 RID: 193 RVA: 0x000074B8 File Offset: 0x000056B8
	public void Start()
	{
		if (this._thisT == null)
		{
			this._thisT = base.transform;
		}
		if (this._flock == null)
		{
			this._flock = (FlockController)Object.FindObjectOfType(typeof(FlockController));
			Debug.Log(this + " has no assigned FlockController, a random FlockController has been assigned");
		}
		if (this._landOnStart)
		{
			base.StartCoroutine(this.InstantLandOnStart(0.1f));
		}
	}

	// Token: 0x060000C2 RID: 194 RVA: 0x00007531 File Offset: 0x00005731
	public void ScareAll()
	{
		this.ScareAll(0f, 1f);
	}

	// Token: 0x060000C3 RID: 195 RVA: 0x00007544 File Offset: 0x00005744
	public void ScareAll(float minDelay, float maxDelay)
	{
		for (int i = 0; i < this._thisT.childCount; i++)
		{
			if (this._thisT.GetChild(i).GetComponent<LandingSpot>() != null)
			{
				this._thisT.GetChild(i).GetComponent<LandingSpot>().Invoke("ReleaseFlockChild", Random.Range(minDelay, maxDelay));
			}
		}
	}

	// Token: 0x060000C4 RID: 196 RVA: 0x000075A4 File Offset: 0x000057A4
	public void LandAll()
	{
		for (int i = 0; i < this._thisT.childCount; i++)
		{
			if (this._thisT.GetChild(i).GetComponent<LandingSpot>() != null)
			{
				LandingSpot component = this._thisT.GetChild(i).GetComponent<LandingSpot>();
				base.StartCoroutine(component.GetFlockChild(0f, 2f));
			}
		}
	}

	// Token: 0x060000C5 RID: 197 RVA: 0x00007609 File Offset: 0x00005809
	public IEnumerator InstantLandOnStart(float delay)
	{
		yield return new WaitForSeconds(delay);
		for (int i = 0; i < this._thisT.childCount; i++)
		{
			if (this._thisT.GetChild(i).GetComponent<LandingSpot>() != null)
			{
				this._thisT.GetChild(i).GetComponent<LandingSpot>().InstantLand();
			}
		}
		yield break;
	}

	// Token: 0x060000C6 RID: 198 RVA: 0x0000761F File Offset: 0x0000581F
	public IEnumerator InstantLand(float delay)
	{
		yield return new WaitForSeconds(delay);
		for (int i = 0; i < this._thisT.childCount; i++)
		{
			if (this._thisT.GetChild(i).GetComponent<LandingSpot>() != null)
			{
				this._thisT.GetChild(i).GetComponent<LandingSpot>().InstantLand();
			}
		}
		yield break;
	}

	// Token: 0x0400012E RID: 302
	public bool _randomRotate = true;

	// Token: 0x0400012F RID: 303
	public Vector2 _autoCatchDelay = new Vector2(10f, 20f);

	// Token: 0x04000130 RID: 304
	public Vector2 _autoDismountDelay = new Vector2(10f, 20f);

	// Token: 0x04000131 RID: 305
	public float _maxBirdDistance = 20f;

	// Token: 0x04000132 RID: 306
	public float _minBirdDistance = 5f;

	// Token: 0x04000133 RID: 307
	public bool _takeClosest;

	// Token: 0x04000134 RID: 308
	public FlockController _flock;

	// Token: 0x04000135 RID: 309
	public bool _landOnStart;

	// Token: 0x04000136 RID: 310
	public bool _soarLand = true;

	// Token: 0x04000137 RID: 311
	public bool _onlyBirdsAbove;

	// Token: 0x04000138 RID: 312
	public float _landingSpeedModifier = 0.5f;

	// Token: 0x04000139 RID: 313
	public float _landingTurnSpeedModifier = 5f;

	// Token: 0x0400013A RID: 314
	public Transform _featherPS;

	// Token: 0x0400013B RID: 315
	public Transform _thisT;

	// Token: 0x0400013C RID: 316
	public int _activeLandingSpots;

	// Token: 0x0400013D RID: 317
	public float _snapLandDistance = 0.1f;

	// Token: 0x0400013E RID: 318
	public float _landedRotateSpeed = 0.01f;
}
