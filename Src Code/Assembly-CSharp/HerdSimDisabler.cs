using System;
using UnityEngine;

// Token: 0x02000031 RID: 49
public class HerdSimDisabler : MonoBehaviour
{
	// Token: 0x0600010B RID: 267 RVA: 0x0000AB14 File Offset: 0x00008D14
	public void Start()
	{
		if (this._distanceFromMainCam)
		{
			this._distanceFrom = Camera.main.transform;
		}
		base.InvokeRepeating("CheckDisable", this._checkDisableEverSeconds + Random.value * this._checkDisableEverSeconds, this._checkDisableEverSeconds);
		base.InvokeRepeating("CheckEnable", this._checkEnableEverSeconds + Random.value * this._checkEnableEverSeconds, this._checkEnableEverSeconds);
		base.Invoke("DisableOnStart", 0.01f);
	}

	// Token: 0x0600010C RID: 268 RVA: 0x0000AB91 File Offset: 0x00008D91
	public void DisableOnStart()
	{
		if (this._disableOnStart)
		{
			base.transform.GetComponent<HerdSimCore>().Disable(this._disableModel, this._disableCollider);
		}
	}

	// Token: 0x0600010D RID: 269 RVA: 0x0000ABB8 File Offset: 0x00008DB8
	public void CheckDisable()
	{
		if (this._distanceFrom != null && base.transform.GetComponent<HerdSimCore>()._enabled && (base.transform.position - this._distanceFrom.position).sqrMagnitude > (float)this._distanceDisable)
		{
			base.transform.GetComponent<HerdSimCore>().Disable(this._disableModel, this._disableCollider);
		}
	}

	// Token: 0x0600010E RID: 270 RVA: 0x0000AC30 File Offset: 0x00008E30
	public void CheckEnable()
	{
		if (this._distanceFrom != null && !base.transform.GetComponent<HerdSimCore>()._enabled && (base.transform.position - this._distanceFrom.position).sqrMagnitude < (float)this._distanceDisable)
		{
			base.transform.GetComponent<HerdSimCore>().Enable();
		}
	}

	// Token: 0x040001DB RID: 475
	public int _distanceDisable = 1000;

	// Token: 0x040001DC RID: 476
	public Transform _distanceFrom;

	// Token: 0x040001DD RID: 477
	public bool _distanceFromMainCam;

	// Token: 0x040001DE RID: 478
	public float _checkDisableEverSeconds = 10f;

	// Token: 0x040001DF RID: 479
	public float _checkEnableEverSeconds = 1f;

	// Token: 0x040001E0 RID: 480
	public bool _disableModel;

	// Token: 0x040001E1 RID: 481
	public bool _disableCollider;

	// Token: 0x040001E2 RID: 482
	public bool _disableOnStart;
}
