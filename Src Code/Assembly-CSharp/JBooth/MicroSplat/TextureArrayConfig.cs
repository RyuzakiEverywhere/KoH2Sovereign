using System;
using System.Collections.Generic;
using UnityEngine;

namespace JBooth.MicroSplat
{
	// Token: 0x020003BB RID: 955
	[CreateAssetMenu(menuName = "MicroSplat/Texture Array Config", order = 1)]
	[ExecuteInEditMode]
	public class TextureArrayConfig : ScriptableObject
	{
		// Token: 0x060035C0 RID: 13760 RVA: 0x0002C538 File Offset: 0x0002A738
		public bool IsAdvancedDetail()
		{
			return false;
		}

		// Token: 0x060035C1 RID: 13761 RVA: 0x001AF370 File Offset: 0x001AD570
		private void Awake()
		{
			TextureArrayConfig.sAllConfigs.Add(this);
		}

		// Token: 0x060035C2 RID: 13762 RVA: 0x001AF37D File Offset: 0x001AD57D
		private void OnDestroy()
		{
			TextureArrayConfig.sAllConfigs.Remove(this);
		}

		// Token: 0x060035C3 RID: 13763 RVA: 0x001AF38C File Offset: 0x001AD58C
		public static TextureArrayConfig FindConfig(Texture2DArray diffuse)
		{
			for (int i = 0; i < TextureArrayConfig.sAllConfigs.Count; i++)
			{
				if (TextureArrayConfig.sAllConfigs[i].diffuseArray == diffuse)
				{
					return TextureArrayConfig.sAllConfigs[i];
				}
			}
			return null;
		}

		// Token: 0x04002549 RID: 9545
		[HideInInspector]
		public bool antiTileArray;

		// Token: 0x0400254A RID: 9546
		[HideInInspector]
		public bool emisMetalArray;

		// Token: 0x0400254B RID: 9547
		[HideInInspector]
		public TextureArrayConfig.TextureMode textureMode = TextureArrayConfig.TextureMode.PBR;

		// Token: 0x0400254C RID: 9548
		[HideInInspector]
		public TextureArrayConfig.ClusterMode clusterMode;

		// Token: 0x0400254D RID: 9549
		[HideInInspector]
		public TextureArrayConfig.PackingMode packingMode;

		// Token: 0x0400254E RID: 9550
		[HideInInspector]
		public int hash;

		// Token: 0x0400254F RID: 9551
		private static List<TextureArrayConfig> sAllConfigs = new List<TextureArrayConfig>();

		// Token: 0x04002550 RID: 9552
		[HideInInspector]
		public Texture2DArray diffuseArray;

		// Token: 0x04002551 RID: 9553
		[HideInInspector]
		public Texture2DArray normalSAOArray;

		// Token: 0x04002552 RID: 9554
		[HideInInspector]
		public Texture2DArray smoothAOArray;

		// Token: 0x04002553 RID: 9555
		[HideInInspector]
		public Texture2DArray diffuseArray2;

		// Token: 0x04002554 RID: 9556
		[HideInInspector]
		public Texture2DArray normalSAOArray2;

		// Token: 0x04002555 RID: 9557
		[HideInInspector]
		public Texture2DArray smoothAOArray2;

		// Token: 0x04002556 RID: 9558
		[HideInInspector]
		public Texture2DArray diffuseArray3;

		// Token: 0x04002557 RID: 9559
		[HideInInspector]
		public Texture2DArray normalSAOArray3;

		// Token: 0x04002558 RID: 9560
		[HideInInspector]
		public Texture2DArray smoothAOArray3;

		// Token: 0x04002559 RID: 9561
		[HideInInspector]
		public Texture2DArray emisArray;

		// Token: 0x0400255A RID: 9562
		[HideInInspector]
		public Texture2DArray emisArray2;

		// Token: 0x0400255B RID: 9563
		[HideInInspector]
		public Texture2DArray emisArray3;

		// Token: 0x0400255C RID: 9564
		public TextureArrayConfig.TextureArrayGroup defaultTextureSettings = new TextureArrayConfig.TextureArrayGroup();

		// Token: 0x0400255D RID: 9565
		public List<TextureArrayConfig.PlatformTextureOverride> platformOverrides = new List<TextureArrayConfig.PlatformTextureOverride>();

		// Token: 0x0400255E RID: 9566
		public TextureArrayConfig.SourceTextureSize sourceTextureSize;

		// Token: 0x0400255F RID: 9567
		[HideInInspector]
		public TextureArrayConfig.AllTextureChannel allTextureChannelHeight = TextureArrayConfig.AllTextureChannel.G;

		// Token: 0x04002560 RID: 9568
		[HideInInspector]
		public TextureArrayConfig.AllTextureChannel allTextureChannelSmoothness = TextureArrayConfig.AllTextureChannel.G;

		// Token: 0x04002561 RID: 9569
		[HideInInspector]
		public TextureArrayConfig.AllTextureChannel allTextureChannelAO = TextureArrayConfig.AllTextureChannel.G;

		// Token: 0x04002562 RID: 9570
		[HideInInspector]
		public TextureArrayConfig.AllTextureChannel allTextureChannelAlpha = TextureArrayConfig.AllTextureChannel.A;

		// Token: 0x04002563 RID: 9571
		[HideInInspector]
		public List<TextureArrayConfig.TextureEntry> sourceTextures = new List<TextureArrayConfig.TextureEntry>();

		// Token: 0x04002564 RID: 9572
		[HideInInspector]
		public List<TextureArrayConfig.TextureEntry> sourceTextures2 = new List<TextureArrayConfig.TextureEntry>();

		// Token: 0x04002565 RID: 9573
		[HideInInspector]
		public List<TextureArrayConfig.TextureEntry> sourceTextures3 = new List<TextureArrayConfig.TextureEntry>();

		// Token: 0x020008FF RID: 2303
		public enum AllTextureChannel
		{
			// Token: 0x040041F3 RID: 16883
			R,
			// Token: 0x040041F4 RID: 16884
			G,
			// Token: 0x040041F5 RID: 16885
			B,
			// Token: 0x040041F6 RID: 16886
			A,
			// Token: 0x040041F7 RID: 16887
			Custom
		}

		// Token: 0x02000900 RID: 2304
		public enum TextureChannel
		{
			// Token: 0x040041F9 RID: 16889
			R,
			// Token: 0x040041FA RID: 16890
			G,
			// Token: 0x040041FB RID: 16891
			B,
			// Token: 0x040041FC RID: 16892
			A
		}

		// Token: 0x02000901 RID: 2305
		public enum Compression
		{
			// Token: 0x040041FE RID: 16894
			AutomaticCompressed,
			// Token: 0x040041FF RID: 16895
			ForceDXT,
			// Token: 0x04004200 RID: 16896
			ForcePVR,
			// Token: 0x04004201 RID: 16897
			ForceETC2,
			// Token: 0x04004202 RID: 16898
			ForceASTC,
			// Token: 0x04004203 RID: 16899
			ForceCrunch,
			// Token: 0x04004204 RID: 16900
			Uncompressed
		}

		// Token: 0x02000902 RID: 2306
		public enum TextureSize
		{
			// Token: 0x04004206 RID: 16902
			k4096 = 4096,
			// Token: 0x04004207 RID: 16903
			k2048 = 2048,
			// Token: 0x04004208 RID: 16904
			k1024 = 1024,
			// Token: 0x04004209 RID: 16905
			k512 = 512,
			// Token: 0x0400420A RID: 16906
			k256 = 256,
			// Token: 0x0400420B RID: 16907
			k128 = 128,
			// Token: 0x0400420C RID: 16908
			k64 = 64,
			// Token: 0x0400420D RID: 16909
			k32 = 32
		}

		// Token: 0x02000903 RID: 2307
		[Serializable]
		public class TextureArraySettings
		{
			// Token: 0x06005231 RID: 21041 RVA: 0x00240988 File Offset: 0x0023EB88
			public TextureArraySettings(TextureArrayConfig.TextureSize s, TextureArrayConfig.Compression c, FilterMode f, int a = 1)
			{
				this.textureSize = s;
				this.compression = c;
				this.filterMode = f;
				this.Aniso = a;
			}

			// Token: 0x0400420E RID: 16910
			public TextureArrayConfig.TextureSize textureSize;

			// Token: 0x0400420F RID: 16911
			public TextureArrayConfig.Compression compression;

			// Token: 0x04004210 RID: 16912
			public FilterMode filterMode;

			// Token: 0x04004211 RID: 16913
			[Range(0f, 16f)]
			public int Aniso = 1;
		}

		// Token: 0x02000904 RID: 2308
		public enum PackingMode
		{
			// Token: 0x04004213 RID: 16915
			Fastest,
			// Token: 0x04004214 RID: 16916
			Quality
		}

		// Token: 0x02000905 RID: 2309
		public enum SourceTextureSize
		{
			// Token: 0x04004216 RID: 16918
			Unchanged,
			// Token: 0x04004217 RID: 16919
			k32 = 32,
			// Token: 0x04004218 RID: 16920
			k256 = 256
		}

		// Token: 0x02000906 RID: 2310
		public enum TextureMode
		{
			// Token: 0x0400421A RID: 16922
			Basic,
			// Token: 0x0400421B RID: 16923
			PBR
		}

		// Token: 0x02000907 RID: 2311
		public enum ClusterMode
		{
			// Token: 0x0400421D RID: 16925
			None,
			// Token: 0x0400421E RID: 16926
			TwoVariations,
			// Token: 0x0400421F RID: 16927
			ThreeVariations
		}

		// Token: 0x02000908 RID: 2312
		[Serializable]
		public class TextureArrayGroup
		{
			// Token: 0x04004220 RID: 16928
			public TextureArrayConfig.TextureArraySettings diffuseSettings = new TextureArrayConfig.TextureArraySettings(TextureArrayConfig.TextureSize.k1024, TextureArrayConfig.Compression.AutomaticCompressed, FilterMode.Bilinear, 1);

			// Token: 0x04004221 RID: 16929
			public TextureArrayConfig.TextureArraySettings normalSettings = new TextureArrayConfig.TextureArraySettings(TextureArrayConfig.TextureSize.k1024, TextureArrayConfig.Compression.AutomaticCompressed, FilterMode.Bilinear, 1);

			// Token: 0x04004222 RID: 16930
			public TextureArrayConfig.TextureArraySettings smoothSettings = new TextureArrayConfig.TextureArraySettings(TextureArrayConfig.TextureSize.k1024, TextureArrayConfig.Compression.AutomaticCompressed, FilterMode.Bilinear, 1);

			// Token: 0x04004223 RID: 16931
			public TextureArrayConfig.TextureArraySettings antiTileSettings = new TextureArrayConfig.TextureArraySettings(TextureArrayConfig.TextureSize.k1024, TextureArrayConfig.Compression.AutomaticCompressed, FilterMode.Bilinear, 1);

			// Token: 0x04004224 RID: 16932
			public TextureArrayConfig.TextureArraySettings emissiveSettings = new TextureArrayConfig.TextureArraySettings(TextureArrayConfig.TextureSize.k1024, TextureArrayConfig.Compression.AutomaticCompressed, FilterMode.Bilinear, 1);
		}

		// Token: 0x02000909 RID: 2313
		[Serializable]
		public class PlatformTextureOverride
		{
			// Token: 0x04004225 RID: 16933
			public TextureArrayConfig.TextureArrayGroup settings = new TextureArrayConfig.TextureArrayGroup();
		}

		// Token: 0x0200090A RID: 2314
		[Serializable]
		public class TextureEntry
		{
			// Token: 0x06005234 RID: 21044 RVA: 0x00240A3C File Offset: 0x0023EC3C
			public void Reset()
			{
				this.diffuse = null;
				this.height = null;
				this.normal = null;
				this.smoothness = null;
				this.ao = null;
				this.isRoughness = false;
				this.alpha = null;
				this.detailNoise = null;
				this.distanceNoise = null;
				this.metal = null;
				this.emis = null;
				this.heightChannel = TextureArrayConfig.TextureChannel.G;
				this.smoothnessChannel = TextureArrayConfig.TextureChannel.G;
				this.aoChannel = TextureArrayConfig.TextureChannel.G;
				this.alphaChannel = TextureArrayConfig.TextureChannel.G;
				this.distanceChannel = TextureArrayConfig.TextureChannel.G;
				this.detailChannel = TextureArrayConfig.TextureChannel.G;
			}

			// Token: 0x06005235 RID: 21045 RVA: 0x00240AC0 File Offset: 0x0023ECC0
			public bool HasTextures()
			{
				return this.diffuse != null || this.height != null || this.normal != null || this.smoothness != null || this.ao != null;
			}

			// Token: 0x04004226 RID: 16934
			public Texture2D diffuse;

			// Token: 0x04004227 RID: 16935
			public Texture2D height;

			// Token: 0x04004228 RID: 16936
			public TextureArrayConfig.TextureChannel heightChannel = TextureArrayConfig.TextureChannel.G;

			// Token: 0x04004229 RID: 16937
			public Texture2D normal;

			// Token: 0x0400422A RID: 16938
			public Texture2D smoothness;

			// Token: 0x0400422B RID: 16939
			public TextureArrayConfig.TextureChannel smoothnessChannel = TextureArrayConfig.TextureChannel.G;

			// Token: 0x0400422C RID: 16940
			public bool isRoughness;

			// Token: 0x0400422D RID: 16941
			public Texture2D ao;

			// Token: 0x0400422E RID: 16942
			public TextureArrayConfig.TextureChannel aoChannel = TextureArrayConfig.TextureChannel.G;

			// Token: 0x0400422F RID: 16943
			public Texture2D alpha;

			// Token: 0x04004230 RID: 16944
			public TextureArrayConfig.TextureChannel alphaChannel = TextureArrayConfig.TextureChannel.G;

			// Token: 0x04004231 RID: 16945
			public bool normalizeAlpha;

			// Token: 0x04004232 RID: 16946
			public Texture2D emis;

			// Token: 0x04004233 RID: 16947
			public Texture2D metal;

			// Token: 0x04004234 RID: 16948
			public TextureArrayConfig.TextureChannel metalChannel = TextureArrayConfig.TextureChannel.G;

			// Token: 0x04004235 RID: 16949
			public Texture2D noiseNormal;

			// Token: 0x04004236 RID: 16950
			public Texture2D detailNoise;

			// Token: 0x04004237 RID: 16951
			public TextureArrayConfig.TextureChannel detailChannel = TextureArrayConfig.TextureChannel.G;

			// Token: 0x04004238 RID: 16952
			public Texture2D distanceNoise;

			// Token: 0x04004239 RID: 16953
			public TextureArrayConfig.TextureChannel distanceChannel = TextureArrayConfig.TextureChannel.G;
		}
	}
}
