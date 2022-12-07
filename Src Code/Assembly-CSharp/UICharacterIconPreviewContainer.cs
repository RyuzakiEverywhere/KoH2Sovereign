using System;
using Logic;
using UnityEngine;

// Token: 0x020001E7 RID: 487
public class UICharacterIconPreviewContainer : MonoBehaviour
{
	// Token: 0x17000187 RID: 391
	// (get) Token: 0x06001D5A RID: 7514 RVA: 0x00114A9C File Offset: 0x00112C9C
	public static UICharacterIconPreviewContainer Instance
	{
		get
		{
			if (UICharacterIconPreviewContainer.instance == null)
			{
				GameObject gameObject = new GameObject();
				UICharacterIconPreviewContainer.instance = gameObject.AddComponent<UICharacterIconPreviewContainer>();
				BaseUI baseUI = BaseUI.Get();
				if (baseUI != null)
				{
					gameObject.transform.SetParent(baseUI.tCanvas, false);
				}
				else
				{
					gameObject.transform.SetParent(GameObject.Find("Canvas").transform, false);
				}
				UICharacterIconPreviewContainer.instance.name = "UIP_PortraitPreviewContainer";
				UICharacterIconPreviewContainer.instance.LoadPrefab();
			}
			return UICharacterIconPreviewContainer.instance;
		}
	}

	// Token: 0x06001D5B RID: 7515 RVA: 0x00114B24 File Offset: 0x00112D24
	private void Awake()
	{
		UICharacterIconPreviewContainer.instance = this;
	}

	// Token: 0x06001D5C RID: 7516 RVA: 0x00114B2C File Offset: 0x00112D2C
	private GameObject LoadPrefab()
	{
		global::Defs.Get(true);
		this.WindowPrefab = global::Defs.GetObj<GameObject>("PortraitPreviewWindow", "window_prefab", null);
		return this.WindowPrefab;
	}

	// Token: 0x06001D5D RID: 7517 RVA: 0x00114B54 File Offset: 0x00112D54
	private GameObject SpawnPrefab()
	{
		GameObject gameObject = null;
		if (this.WindowPrefab != null)
		{
			gameObject = global::Common.Spawn(this.WindowPrefab, false, false);
			gameObject.transform.SetParent(base.transform, false);
		}
		return gameObject;
	}

	// Token: 0x06001D5E RID: 7518 RVA: 0x00114B92 File Offset: 0x00112D92
	public void PreviewCharacter(Logic.Character character)
	{
		this.PreviewCharacter(character.portraitID, PresetRecipes.CharacterToAges(character), character.portrait_variantID);
	}

	// Token: 0x06001D5F RID: 7519 RVA: 0x00114BAC File Offset: 0x00112DAC
	public void PreviewTexture(Texture2D texture)
	{
		UICharacterIconPreview[] componentsInChildren = base.GetComponentsInChildren<UICharacterIconPreview>();
		GameObject gameObject = null;
		if (componentsInChildren.Length == 0)
		{
			gameObject = this.SpawnPrefab();
		}
		if (gameObject == null)
		{
			return;
		}
		UICharacterIconPreview component = gameObject.GetComponent<UICharacterIconPreview>();
		if (component == null)
		{
			return;
		}
		component.characterId = 0;
		component.PreviewTexture(texture);
	}

	// Token: 0x06001D60 RID: 7520 RVA: 0x00114BF4 File Offset: 0x00112DF4
	public void PreviewCharacter(int characterId, int age, int variantID = 0)
	{
		UICharacterIconPreview[] componentsInChildren = base.GetComponentsInChildren<UICharacterIconPreview>();
		bool flag = false;
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].characterId == characterId)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			return;
		}
		GameObject gameObject = this.SpawnPrefab();
		if (gameObject == null)
		{
			return;
		}
		UICharacterIconPreview component = gameObject.GetComponent<UICharacterIconPreview>();
		if (component == null)
		{
			return;
		}
		component.characterId = characterId;
		component.age = age;
		component.variantId = variantID;
		component.PreviewCharacter();
	}

	// Token: 0x04001337 RID: 4919
	private static UICharacterIconPreviewContainer instance;

	// Token: 0x04001338 RID: 4920
	private GameObject WindowPrefab;
}
