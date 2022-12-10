using System;
using Logic;
using UnityEngine.SceneManagement;

namespace UnityEngine.UI
{
	// Token: 0x02000349 RID: 841
	public class UIShieldRawImage : RawImage
	{
		// Token: 0x060032CA RID: 13002 RVA: 0x0019BDE2 File Offset: 0x00199FE2
		private void Init()
		{
			if (this.m_Initialzied)
			{
				return;
			}
			this.m_Initialzied = true;
		}

		// Token: 0x060032CB RID: 13003 RVA: 0x0019BDF4 File Offset: 0x00199FF4
		public override void RecalculateMasking()
		{
			base.RecalculateMasking();
			if (Application.isPlaying)
			{
				this.SetCrestId(this.crestId, this.kingdomType, null);
			}
		}

		// Token: 0x060032CC RID: 13004 RVA: 0x0019BE16 File Offset: 0x0019A016
		public Material GetCurrentMaterial(out bool masked)
		{
			if (this.m_MaskMaterial != null)
			{
				masked = true;
				return this.m_MaskMaterial;
			}
			masked = false;
			return this.material;
		}

		// Token: 0x060032CD RID: 13005 RVA: 0x0019BE3C File Offset: 0x0019A03C
		public void SetCrestId(int cId, string kingdomType, string mapName = null)
		{
			if (this == null || base.gameObject == null)
			{
				return;
			}
			this.Init();
			this.crestId = cId;
			this.kingdomType = kingdomType;
			if (!string.IsNullOrEmpty(mapName))
			{
				bool flag;
				this.ApplyMaterial(this.GetCurrentMaterial(out flag), this.crestId, mapName);
				return;
			}
			bool flag2;
			this.ApplyMaterial(this.GetCurrentMaterial(out flag2), this.crestId);
		}

		// Token: 0x060032CE RID: 13006 RVA: 0x0019BEA7 File Offset: 0x0019A0A7
		protected override void OnDestroy()
		{
			base.OnDestroy();
		}

		// Token: 0x060032CF RID: 13007 RVA: 0x0019BEB0 File Offset: 0x0019A0B0
		private void ApplyMaterial(Material m, int crestId, string mapName)
		{
			Texture2D texture2D = Assets.Get<Texture2D>(string.Concat(new string[]
			{
				"Assets/Maps/",
				mapName,
				"/CoatOfArmsAtlas",
				this.shieldMode,
				".png"
			}));
			if (texture2D != null)
			{
				m.SetTexture("_MainTex", texture2D);
			}
			global::Defs defs = global::Defs.Get(false);
			if (defs != null)
			{
				DT.Field field = defs.dt.Find("CoatOfArmsModes." + this.shieldMode, null);
				if (field != null)
				{
					m.SetFloat("tileSizeX", (float)field.GetInt("width", null, 0, true, true, true, '.'));
					m.SetFloat("tileSizeY", (float)field.GetInt("height", null, 0, true, true, true, '.'));
				}
			}
			this.SetAllDirty();
		}

		// Token: 0x060032D0 RID: 13008 RVA: 0x0019BF7A File Offset: 0x0019A17A
		protected override void OnPopulateMesh(VertexHelper toFill)
		{
			this.GenerateSimpleSprite(toFill, false);
		}

		// Token: 0x060032D1 RID: 13009 RVA: 0x0019BF84 File Offset: 0x0019A184
		private void GenerateSimpleSprite(VertexHelper vh, bool lPreserveAspect)
		{
			Vector4 drawingDimensions = this.GetDrawingDimensions(lPreserveAspect);
			Vector2 uv = new Vector2((float)this.crestId, (float)global::Kingdom.GetShieldFrameIndex(this.shieldMode, this.kingdomType, false));
			vh.Clear();
			vh.AddVert(new Vector3(drawingDimensions.x, drawingDimensions.y), this.color, new Vector2(0f, 0f), uv, Vector3.up, Vector4.zero);
			vh.AddVert(new Vector3(drawingDimensions.x, drawingDimensions.w), this.color, new Vector2(0f, 1f), uv, Vector3.up, Vector4.zero);
			vh.AddVert(new Vector3(drawingDimensions.z, drawingDimensions.w), this.color, new Vector2(1f, 1f), uv, Vector3.up, Vector4.zero);
			vh.AddVert(new Vector3(drawingDimensions.z, drawingDimensions.y), this.color, new Vector2(1f, 0f), uv, Vector3.up, Vector4.zero);
			vh.AddTriangle(0, 1, 2);
			vh.AddTriangle(2, 3, 0);
		}

		// Token: 0x060032D2 RID: 13010 RVA: 0x0019C0C4 File Offset: 0x0019A2C4
		private Vector4 GetDrawingDimensions(bool shouldPreserveAspect)
		{
			Vector4 zero = Vector4.zero;
			Vector2 vector = (this.mainTexture == null) ? Vector2.zero : new Vector2(base.uvRect.width, base.uvRect.height);
			Rect pixelAdjustedRect = base.GetPixelAdjustedRect();
			int num = Mathf.RoundToInt(vector.x);
			int num2 = Mathf.RoundToInt(vector.y);
			Vector4 vector2 = new Vector4(zero.x / (float)num, zero.y / (float)num2, ((float)num - zero.z) / (float)num, ((float)num2 - zero.w) / (float)num2);
			if (shouldPreserveAspect && vector.sqrMagnitude > 0f)
			{
				float num3 = vector.x / vector.y;
				float num4 = pixelAdjustedRect.width / pixelAdjustedRect.height;
				if (num3 > num4)
				{
					float height = pixelAdjustedRect.height;
					pixelAdjustedRect.height = pixelAdjustedRect.width * (1f / num3);
					pixelAdjustedRect.y += (height - pixelAdjustedRect.height) * base.rectTransform.pivot.y;
				}
				else
				{
					float width = pixelAdjustedRect.width;
					pixelAdjustedRect.width = pixelAdjustedRect.height * num3;
					pixelAdjustedRect.x += (width - pixelAdjustedRect.width) * base.rectTransform.pivot.x;
				}
			}
			vector2 = new Vector4(pixelAdjustedRect.x + pixelAdjustedRect.width * vector2.x, pixelAdjustedRect.y + pixelAdjustedRect.height * vector2.y, pixelAdjustedRect.x + pixelAdjustedRect.width * vector2.z, pixelAdjustedRect.y + pixelAdjustedRect.height * vector2.w);
			return vector2;
		}

		// Token: 0x060032D3 RID: 13011 RVA: 0x0019C294 File Offset: 0x0019A494
		private void ApplyMaterial(Material m, int crestId)
		{
			if (this == null || base.gameObject == null)
			{
				return;
			}
			if (m == null)
			{
				return;
			}
			Game game = GameLogic.Get(false);
			string mapName;
			if (game != null && !string.IsNullOrEmpty(game.map_name))
			{
				mapName = game.map_name;
			}
			else
			{
				mapName = SceneManager.GetActiveScene().name.ToLowerInvariant();
			}
			this.ApplyMaterial(m, crestId, mapName);
		}

		// Token: 0x04002255 RID: 8789
		public int crestId;

		// Token: 0x04002256 RID: 8790
		[HideInInspector]
		public string kingdomType = "regular";

		// Token: 0x04002257 RID: 8791
		[HideInInspector]
		public string shieldMode = "shield";

		// Token: 0x04002258 RID: 8792
		public static string[] sm_KingdomTypes = new string[]
		{
			"regular",
			"great_power",
			"vassal",
			"vassal_great_power",
			"faction",
			"rebel",
			"rebel_famous",
			"crusade"
		};

		// Token: 0x04002259 RID: 8793
		private bool m_Initialzied;
	}
}
