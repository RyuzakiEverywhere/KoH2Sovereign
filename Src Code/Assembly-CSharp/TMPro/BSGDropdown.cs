using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TMPro
{
	// Token: 0x02000341 RID: 833
	[RequireComponent(typeof(RectTransform))]
	public class BSGDropdown : TMP_Dropdown
	{
		// Token: 0x0600329B RID: 12955 RVA: 0x0019AD45 File Offset: 0x00198F45
		public override void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}
			base.OnPointerClick(eventData);
			Action onShow = this.OnShow;
			if (onShow != null)
			{
				onShow();
			}
			this.AddToolipBlocker();
		}

		// Token: 0x0600329C RID: 12956 RVA: 0x0019AD6E File Offset: 0x00198F6E
		public override void OnSubmit(BaseEventData eventData)
		{
			base.OnSubmit(eventData);
			Action onShow = this.OnShow;
			if (onShow != null)
			{
				onShow();
			}
			this.AddToolipBlocker();
		}

		// Token: 0x0600329D RID: 12957 RVA: 0x0019AD90 File Offset: 0x00198F90
		private void AddToolipBlocker()
		{
			GameObject gameObject = Common.FindChildByName(base.gameObject, "Dropdown List", true, true);
			if (gameObject == null)
			{
				return;
			}
			gameObject.AddComponent<TooltipBlocker>();
		}

		// Token: 0x0400222B RID: 8747
		public Action OnShow;
	}
}
