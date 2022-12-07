using System;
using UnityEngine;

// Token: 0x02000309 RID: 777
public class UISpriteSet : MonoBehaviour
{
	// Token: 0x06003078 RID: 12408 RVA: 0x00188E64 File Offset: 0x00187064
	public Sprite GetSprite(string spriteName)
	{
		int num = Array.FindIndex<UISpriteSet.UISpriteSetPair>(this.Data, (UISpriteSet.UISpriteSetPair obj) => obj.SpriteName == spriteName);
		if (num != -1)
		{
			return this.Data[num].Sprite;
		}
		return null;
	}

	// Token: 0x04002087 RID: 8327
	public UISpriteSet.UISpriteSetPair[] Data;

	// Token: 0x02000871 RID: 2161
	[Serializable]
	public class UISpriteSetPair
	{
		// Token: 0x04003F39 RID: 16185
		public string SpriteName;

		// Token: 0x04003F3A RID: 16186
		public Sprite Sprite;
	}
}
