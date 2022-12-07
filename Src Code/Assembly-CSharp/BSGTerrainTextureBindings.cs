using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000AB RID: 171
[CreateAssetMenu(fileName = "BSGTerrainTextureBindings", menuName = "BSG/BSGTerrainTextureBindings")]
public class BSGTerrainTextureBindings : ScriptableObject
{
	// Token: 0x1700004C RID: 76
	// (get) Token: 0x06000603 RID: 1539 RVA: 0x000417D2 File Offset: 0x0003F9D2
	public int resolution
	{
		get
		{
			return this.texture_bindings[0].texture.width;
		}
	}

	// Token: 0x06000604 RID: 1540 RVA: 0x000417EC File Offset: 0x0003F9EC
	public Texture2D GetTextureBinding(Texture2D source_texture)
	{
		string name = source_texture.name;
		for (int i = 0; i < this.texture_bindings.Count; i++)
		{
			if (this.texture_bindings[i].source_texture_name.Equals(name))
			{
				return this.texture_bindings[i].texture;
			}
		}
		return null;
	}

	// Token: 0x06000605 RID: 1541 RVA: 0x00041842 File Offset: 0x0003FA42
	public bool TryGetTextureBinding(Texture2D source_texture, out Texture2D output_texture)
	{
		output_texture = this.GetTextureBinding(source_texture);
		return output_texture != null;
	}

	// Token: 0x04000598 RID: 1432
	public List<BSGTerrainTextureBindings.TextureBinding> texture_bindings = new List<BSGTerrainTextureBindings.TextureBinding>();

	// Token: 0x04000599 RID: 1433
	public Texture2D fallback_texture;

	// Token: 0x02000575 RID: 1397
	[Serializable]
	public class TextureBinding
	{
		// Token: 0x0400305E RID: 12382
		public string source_texture_name;

		// Token: 0x0400305F RID: 12383
		public Texture2D texture;
	}
}
