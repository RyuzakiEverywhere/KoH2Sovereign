using System;
using UnityEngine;

// Token: 0x02000032 RID: 50
public class HerdSimScary : MonoBehaviour
{
	// Token: 0x06000110 RID: 272 RVA: 0x0000ACC2 File Offset: 0x00008EC2
	public void Start()
	{
		this.Init();
	}

	// Token: 0x06000111 RID: 273 RVA: 0x0000ACCC File Offset: 0x00008ECC
	public void Init()
	{
		if (this._scareType.Length != 0)
		{
			base.InvokeRepeating("BeScary", Random.value * this._scaryInterval + 1f, this._scaryInterval);
			base.InvokeRepeating("CheckChase", 2f, 2f);
			return;
		}
		Debug.Log(base.transform.name + " has nothing to scare; Please assigne ScareType");
	}

	// Token: 0x06000112 RID: 274 RVA: 0x0000AD35 File Offset: 0x00008F35
	public void CheckChase()
	{
		this._canChase = !this._canChase;
		if (!this._canChase)
		{
			this._chase = null;
		}
	}

	// Token: 0x06000113 RID: 275 RVA: 0x0000AD58 File Offset: 0x00008F58
	public void BeScary()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, 4f, this._herdLayerMask);
		HerdSimCore herdSimCore = null;
		for (int i = 0; i < array.Length; i++)
		{
			Transform parent = array[i].transform.parent;
			if (parent != null)
			{
				herdSimCore = parent.GetComponent<HerdSimCore>();
			}
			if (herdSimCore != null)
			{
				bool flag = false;
				for (int j = 0; j < this._scareType.Length; j++)
				{
					if (herdSimCore._type == this._scareType[j])
					{
						flag = true;
					}
				}
				if (flag)
				{
					herdSimCore.Scare(base.transform);
					if (this._chase == null && this._canChase)
					{
						this._chase = herdSimCore;
					}
				}
			}
		}
		if (this._chase != null)
		{
			HerdSimCore component = base.GetComponent<HerdSimCore>();
			if (component != null)
			{
				component._waypoint = this._chase.transform.position;
				component._mode = 2;
			}
		}
	}

	// Token: 0x040001E3 RID: 483
	public HerdSimCore _chase;

	// Token: 0x040001E4 RID: 484
	public int[] _scareType;

	// Token: 0x040001E5 RID: 485
	public bool _canChase;

	// Token: 0x040001E6 RID: 486
	public float _scaryInterval = 0.25f;

	// Token: 0x040001E7 RID: 487
	public LayerMask _herdLayerMask = -1;
}
