using System;
using UnityEngine;

// Token: 0x0200004C RID: 76
public class CTAA_EXCLUDE : MonoBehaviour
{
	// Token: 0x060001DE RID: 478 RVA: 0x0001E324 File Offset: 0x0001C524
	private void Start()
	{
		if (base.GetComponent<Renderer>() != null)
		{
			this.mats = base.GetComponent<Renderer>().materials;
			if (this.mats.Length != 0)
			{
				foreach (Material material in this.mats)
				{
					material.SetFloat("rtmask", 1f);
					material.SetInt("_useAlpha", this.useAlpha ? 1 : 0);
				}
			}
		}
		Material material2 = null;
		if (base.GetComponent<CanvasRenderer>() != null)
		{
			material2 = base.GetComponent<CanvasRenderer>().GetMaterial();
		}
		else if (base.GetComponent<Renderer>() != null)
		{
			material2 = base.GetComponent<Renderer>().material;
		}
		if (material2 != null)
		{
			material2.SetFloat("rtmask", 1f);
			material2.SetInt("_useAlpha", this.useAlpha ? 1 : 0);
		}
		if (this.m_IncludeChildren)
		{
			foreach (Transform transform in base.gameObject.GetComponentsInChildren<Transform>())
			{
				if (transform.gameObject.GetComponent<Renderer>() != null)
				{
					this.mats = transform.gameObject.GetComponent<Renderer>().materials;
					if (this.mats.Length != 0)
					{
						foreach (Material material3 in this.mats)
						{
							material3.SetFloat("rtmask", 1f);
							material3.SetInt("_useAlpha", this.useAlpha ? 1 : 0);
						}
					}
					Material material4;
					if (transform.gameObject.GetComponent<CanvasRenderer>() != null)
					{
						material4 = transform.gameObject.GetComponent<CanvasRenderer>().GetMaterial();
					}
					else
					{
						material4 = transform.gameObject.GetComponent<Renderer>().material;
					}
					if (material4 != null)
					{
						material4.SetFloat("rtmask", 1f);
						material4.SetInt("_useAlpha", this.useAlpha ? 1 : 0);
					}
				}
			}
		}
	}

	// Token: 0x060001DF RID: 479 RVA: 0x0001E518 File Offset: 0x0001C718
	private void Update()
	{
		if (this.UI)
		{
			Material material = null;
			if (base.GetComponent<CanvasRenderer>() != null)
			{
				material = base.GetComponent<CanvasRenderer>().GetMaterial();
			}
			if (material != null)
			{
				material.SetFloat("rtmask", 1f);
				material.SetInt("_useAlpha", this.useAlpha ? 1 : 0);
			}
		}
	}

	// Token: 0x040002DC RID: 732
	public bool useAlpha;

	// Token: 0x040002DD RID: 733
	private Material[] mats;

	// Token: 0x040002DE RID: 734
	public bool m_IncludeChildren;

	// Token: 0x040002DF RID: 735
	public bool UI;
}
