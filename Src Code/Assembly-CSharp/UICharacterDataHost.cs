using System;
using Logic;
using UnityEngine;

// Token: 0x02000305 RID: 773
public class UICharacterDataHost : MonoBehaviour, ICharacterDataHost
{
	// Token: 0x1400003D RID: 61
	// (add) Token: 0x0600305E RID: 12382 RVA: 0x00188950 File Offset: 0x00186B50
	// (remove) Token: 0x0600305F RID: 12383 RVA: 0x00188988 File Offset: 0x00186B88
	public event Action<ICharacterDataHost> OnChange;

	// Token: 0x06003060 RID: 12384 RVA: 0x001889BD File Offset: 0x00186BBD
	public Logic.Character GetCharacterData()
	{
		return this.m_Data;
	}

	// Token: 0x06003061 RID: 12385 RVA: 0x001889C5 File Offset: 0x00186BC5
	public void SetCharacterData(Logic.Character data)
	{
		this.m_Data = data;
		if (this.OnChange != null)
		{
			this.OnChange(this);
		}
	}

	// Token: 0x06003062 RID: 12386 RVA: 0x001889E4 File Offset: 0x00186BE4
	public static GameObject Create(Logic.Character characterData, Vars vars, GameObject prototype, RectTransform parent)
	{
		if (characterData == null)
		{
			Debug.LogWarning("Fail to create character Info widnow! Reson: no character data e provided.");
			return null;
		}
		if (prototype == null)
		{
			Debug.LogWarning("Fail to create character Info widnow! Reson: no prototype provided.");
			return null;
		}
		if (parent == null)
		{
			Debug.LogWarning("Fail to create character Info widnow! Reson: no parent provided.");
			return null;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prototype, parent);
		UICharacterDataHost uicharacterDataHost = gameObject.GetComponent<UICharacterDataHost>();
		if (uicharacterDataHost == null)
		{
			uicharacterDataHost = gameObject.AddComponent<UICharacterDataHost>();
		}
		uicharacterDataHost.gameObject.SetActive(true);
		uicharacterDataHost.SetCharacterData(characterData);
		uicharacterDataHost.gameObject.transform.localPosition = Vector3.zero;
		return null;
	}

	// Token: 0x04002075 RID: 8309
	private Logic.Character m_Data;
}
