using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200022B RID: 555
public class UIRandomSpritePicker : MonoBehaviour
{
	// Token: 0x060021AF RID: 8623 RVA: 0x0013165C File Offset: 0x0012F85C
	private void OnEnable()
	{
		if (this.options == null || this.options.Length == 0)
		{
			return;
		}
		base.gameObject.GetComponent<Image>().sprite = this.options[Random.Range(0, this.options.Length)];
	}

	// Token: 0x040016A1 RID: 5793
	public Sprite[] options;
}
