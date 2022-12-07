using System;
using UnityEngine;

// Token: 0x02000021 RID: 33
public class UnSoftDistanceDisabler : MonoBehaviour
{
	// Token: 0x06000079 RID: 121 RVA: 0x00004A40 File Offset: 0x00002C40
	public void Start()
	{
		if (this._distanceFromMainCam)
		{
			this._distanceFrom = Camera.main.transform;
		}
		base.InvokeRepeating("CheckDisable", this._disableCheckInterval + Random.value * this._disableCheckInterval, this._disableCheckInterval);
		base.InvokeRepeating("CheckEnable", this._enableCheckInterval + Random.value * this._enableCheckInterval, this._enableCheckInterval);
		base.Invoke("DisableOnStart", 0.01f);
	}

	// Token: 0x0600007A RID: 122 RVA: 0x00004ABD File Offset: 0x00002CBD
	public void DisableOnStart()
	{
		if (this._disableOnStart)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600007B RID: 123 RVA: 0x00004AD4 File Offset: 0x00002CD4
	public void CheckDisable()
	{
		if (base.gameObject.activeInHierarchy && (base.transform.position - this._distanceFrom.position).sqrMagnitude > (float)(this._distanceDisable * this._distanceDisable))
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600007C RID: 124 RVA: 0x00004B30 File Offset: 0x00002D30
	public void CheckEnable()
	{
		if (!base.gameObject.activeInHierarchy && (base.transform.position - this._distanceFrom.position).sqrMagnitude < (float)(this._distanceDisable * this._distanceDisable))
		{
			base.gameObject.SetActive(true);
		}
	}

	// Token: 0x040000A6 RID: 166
	public int _distanceDisable = 1000;

	// Token: 0x040000A7 RID: 167
	public Transform _distanceFrom;

	// Token: 0x040000A8 RID: 168
	public bool _distanceFromMainCam;

	// Token: 0x040000A9 RID: 169
	[Tooltip("The amount of time in seconds between checks")]
	public float _disableCheckInterval = 10f;

	// Token: 0x040000AA RID: 170
	[Tooltip("The amount of time in seconds between checks")]
	public float _enableCheckInterval = 1f;

	// Token: 0x040000AB RID: 171
	public bool _disableOnStart;
}
