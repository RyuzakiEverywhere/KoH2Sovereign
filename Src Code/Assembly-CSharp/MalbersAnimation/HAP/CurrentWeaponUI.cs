using System;
using UnityEngine;
using UnityEngine.UI;

namespace MalbersAnimations.HAP
{
	// Token: 0x02000423 RID: 1059
	public class CurrentWeaponUI : MonoBehaviour
	{
		// Token: 0x06003922 RID: 14626 RVA: 0x001BD889 File Offset: 0x001BBA89
		public void UIWeaponName(GameObject weaponName)
		{
			this.WeaponName.text = ((this.WeaponName != null) ? weaponName.name.Replace("(Clone)", "") : "None");
		}

		// Token: 0x0400293C RID: 10556
		public Text WeaponName;
	}
}
