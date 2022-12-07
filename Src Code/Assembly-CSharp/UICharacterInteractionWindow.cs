using System;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x020001E9 RID: 489
public class UICharacterInteractionWindow : MonoBehaviour
{
	// Token: 0x17000188 RID: 392
	// (get) Token: 0x06001D69 RID: 7529 RVA: 0x00114D8A File Offset: 0x00112F8A
	// (set) Token: 0x06001D6A RID: 7530 RVA: 0x00114D92 File Offset: 0x00112F92
	public UserCharacterInteraction Data { get; private set; }

	// Token: 0x06001D6B RID: 7531 RVA: 0x00114D9B File Offset: 0x00112F9B
	public void SetData(UserCharacterInteraction data)
	{
		this.Data = data;
		this.Refresh();
	}

	// Token: 0x06001D6C RID: 7532 RVA: 0x000023FD File Offset: 0x000005FD
	private void Refresh()
	{
	}

	// Token: 0x06001D6D RID: 7533 RVA: 0x00114DAC File Offset: 0x00112FAC
	public static UICharacterInteractionWindow Create(Vars vars, GameObject prototype, RectTransform parent)
	{
		if (prototype == null)
		{
			return null;
		}
		if (parent == null)
		{
			return null;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prototype, Vector3.zero, Quaternion.identity, parent);
		UICharacterInteractionWindow uicharacterInteractionWindow = gameObject.GetComponent<UICharacterInteractionWindow>();
		if (uicharacterInteractionWindow == null)
		{
			uicharacterInteractionWindow = gameObject.AddComponent<UICharacterInteractionWindow>();
		}
		uicharacterInteractionWindow.SetData(new UserCharacterInteraction(vars));
		return uicharacterInteractionWindow;
	}

	// Token: 0x0400133C RID: 4924
	[UIFieldTarget("id_ActionCaption")]
	private TextMeshProUGUI TMP_Caption;

	// Token: 0x0400133D RID: 4925
	[UIFieldTarget("id_ActionBody")]
	private TextMeshProUGUI TMP_Body;
}
