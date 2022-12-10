using System;
using System.Collections.Generic;
using UnityEngine.Sprites;

namespace UnityEngine.UI
{
	// Token: 0x0200034B RID: 843
	[AddComponentMenu("UI/SpriteAnimation", 11)]
	public class SpriteAnimation : MaskableGraphic, ILayoutElement, ICanvasRaycastFilter, IMeshModifier
	{
		// Token: 0x1700027E RID: 638
		// (get) Token: 0x060032D9 RID: 13017 RVA: 0x0019C435 File Offset: 0x0019A635
		// (set) Token: 0x060032DA RID: 13018 RVA: 0x0019C43D File Offset: 0x0019A63D
		public Sprite sprite
		{
			get
			{
				return this.m_Sprite;
			}
			set
			{
				if (SetPropertyUtility.SetClass<Sprite>(ref this.m_Sprite, value))
				{
					this.SetAllDirty();
				}
			}
		}

		// Token: 0x1700027F RID: 639
		// (get) Token: 0x060032DB RID: 13019 RVA: 0x0019C453 File Offset: 0x0019A653
		// (set) Token: 0x060032DC RID: 13020 RVA: 0x0019C45B File Offset: 0x0019A65B
		public Sprite overrideSprite
		{
			get
			{
				return this.activeSprite;
			}
			set
			{
				if (SetPropertyUtility.SetClass<Sprite>(ref this.m_OverrideSprite, value))
				{
					this.SetAllDirty();
				}
			}
		}

		// Token: 0x17000280 RID: 640
		// (get) Token: 0x060032DD RID: 13021 RVA: 0x0019C471 File Offset: 0x0019A671
		private Sprite activeSprite
		{
			get
			{
				if (!(this.m_OverrideSprite != null))
				{
					return this.sprite;
				}
				return this.m_OverrideSprite;
			}
		}

		// Token: 0x17000281 RID: 641
		// (get) Token: 0x060032DE RID: 13022 RVA: 0x0019C48E File Offset: 0x0019A68E
		// (set) Token: 0x060032DF RID: 13023 RVA: 0x0019C496 File Offset: 0x0019A696
		public bool preserveAspect
		{
			get
			{
				return this.m_PreserveAspect;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<bool>(ref this.m_PreserveAspect, value))
				{
					this.SetVerticesDirty();
				}
			}
		}

		// Token: 0x17000282 RID: 642
		// (get) Token: 0x060032E0 RID: 13024 RVA: 0x0019C4AC File Offset: 0x0019A6AC
		// (set) Token: 0x060032E1 RID: 13025 RVA: 0x0019C4B4 File Offset: 0x0019A6B4
		public int Rows
		{
			get
			{
				return this.m_Rows;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<int>(ref this.m_Rows, value))
				{
					this.RebildAnimationData();
				}
			}
		}

		// Token: 0x17000283 RID: 643
		// (get) Token: 0x060032E2 RID: 13026 RVA: 0x0019C4CA File Offset: 0x0019A6CA
		// (set) Token: 0x060032E3 RID: 13027 RVA: 0x0019C4D2 File Offset: 0x0019A6D2
		public int Cols
		{
			get
			{
				return this.m_Cols;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<int>(ref this.m_Cols, value))
				{
					this.RebildAnimationData();
				}
			}
		}

		// Token: 0x17000284 RID: 644
		// (get) Token: 0x060032E4 RID: 13028 RVA: 0x0019C4E8 File Offset: 0x0019A6E8
		// (set) Token: 0x060032E5 RID: 13029 RVA: 0x0019C4F0 File Offset: 0x0019A6F0
		public int FramesPerSec
		{
			get
			{
				return this.m_FramesPerSec;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<int>(ref this.m_FramesPerSec, value))
				{
					this.RebildAnimationData();
				}
			}
		}

		// Token: 0x17000285 RID: 645
		// (get) Token: 0x060032E6 RID: 13030 RVA: 0x0019C506 File Offset: 0x0019A706
		// (set) Token: 0x060032E7 RID: 13031 RVA: 0x0019C50E File Offset: 0x0019A70E
		public float Speed
		{
			get
			{
				return this.m_Speed;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<float>(ref this.m_Speed, value))
				{
					this.RebildAnimationData();
				}
			}
		}

		// Token: 0x17000286 RID: 646
		// (get) Token: 0x060032E8 RID: 13032 RVA: 0x0019C524 File Offset: 0x0019A724
		// (set) Token: 0x060032E9 RID: 13033 RVA: 0x0019C52C File Offset: 0x0019A72C
		public float alphaHitTestMinimumThreshold
		{
			get
			{
				return this.m_AlphaHitTestMinimumThreshold;
			}
			set
			{
				this.m_AlphaHitTestMinimumThreshold = value;
			}
		}

		// Token: 0x060032EA RID: 13034 RVA: 0x0019C535 File Offset: 0x0019A735
		protected SpriteAnimation()
		{
			base.useLegacyMeshGeneration = false;
		}

		// Token: 0x17000287 RID: 647
		// (get) Token: 0x060032EB RID: 13035 RVA: 0x0019C564 File Offset: 0x0019A764
		public static Material defaultETC1GraphicMaterial
		{
			get
			{
				if (SpriteAnimation.s_ETC1DefaultUI == null)
				{
					SpriteAnimation.s_ETC1DefaultUI = SpriteAnimation.CreateDefaulMaterial();
				}
				return SpriteAnimation.s_ETC1DefaultUI;
			}
		}

		// Token: 0x060032EC RID: 13036 RVA: 0x0019C584 File Offset: 0x0019A784
		private static Material CreateDefaulMaterial()
		{
			Debug.Log("CreateDefaulMaterial");
			Shader shader = Shader.Find("UI/Animation/Default");
			if (shader != null)
			{
				return new Material(shader);
			}
			return null;
		}

		// Token: 0x17000288 RID: 648
		// (get) Token: 0x060032ED RID: 13037 RVA: 0x0019C5B8 File Offset: 0x0019A7B8
		public override Texture mainTexture
		{
			get
			{
				if (!(this.activeSprite == null))
				{
					return this.activeSprite.texture;
				}
				if (this.material != null && this.material.mainTexture != null)
				{
					return this.material.mainTexture;
				}
				return Graphic.s_WhiteTexture;
			}
		}

		// Token: 0x17000289 RID: 649
		// (get) Token: 0x060032EE RID: 13038 RVA: 0x0019C614 File Offset: 0x0019A814
		public bool hasBorder
		{
			get
			{
				return this.activeSprite != null && this.activeSprite.border.sqrMagnitude > 0f;
			}
		}

		// Token: 0x1700028A RID: 650
		// (get) Token: 0x060032EF RID: 13039 RVA: 0x0019C64C File Offset: 0x0019A84C
		public float pixelsPerUnit
		{
			get
			{
				float num = 100f;
				if (this.activeSprite)
				{
					num = this.activeSprite.pixelsPerUnit;
				}
				float num2 = 100f;
				if (base.canvas)
				{
					num2 = base.canvas.referencePixelsPerUnit;
				}
				return num / num2;
			}
		}

		// Token: 0x1700028B RID: 651
		// (get) Token: 0x060032F0 RID: 13040 RVA: 0x0019C69C File Offset: 0x0019A89C
		// (set) Token: 0x060032F1 RID: 13041 RVA: 0x0019C6EA File Offset: 0x0019A8EA
		public override Material material
		{
			get
			{
				if (this.m_Material != null)
				{
					return this.m_Material;
				}
				if (this.activeSprite && this.activeSprite.associatedAlphaSplitTexture != null)
				{
					return SpriteAnimation.defaultETC1GraphicMaterial;
				}
				return this.defaultMaterial;
			}
			set
			{
				base.material = value;
			}
		}

		// Token: 0x1700028C RID: 652
		// (get) Token: 0x060032F2 RID: 13042 RVA: 0x0019C6F3 File Offset: 0x0019A8F3
		public override Material defaultMaterial
		{
			get
			{
				if (SpriteAnimation.s_DefaultAnimUI == null)
				{
					SpriteAnimation.s_DefaultAnimUI = SpriteAnimation.CreateDefaulMaterial();
				}
				return Graphic.s_DefaultUI;
			}
		}

		// Token: 0x060032F3 RID: 13043 RVA: 0x0019C714 File Offset: 0x0019A914
		private Vector4 GetDrawingDimensions(bool shouldPreserveAspect)
		{
			Vector4 vector = (this.activeSprite == null) ? Vector4.zero : DataUtility.GetPadding(this.activeSprite);
			Vector2 vector2 = (this.activeSprite == null) ? Vector2.zero : new Vector2(this.activeSprite.rect.width, this.activeSprite.rect.height);
			Rect pixelAdjustedRect = base.GetPixelAdjustedRect();
			int num = Mathf.RoundToInt(vector2.x);
			int num2 = Mathf.RoundToInt(vector2.y);
			Vector4 vector3 = new Vector4(vector.x / (float)num, vector.y / (float)num2, ((float)num - vector.z) / (float)num, ((float)num2 - vector.w) / (float)num2);
			if (shouldPreserveAspect && vector2.sqrMagnitude > 0f)
			{
				float num3 = vector2.x / vector2.y;
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
			vector3 = new Vector4(pixelAdjustedRect.x + pixelAdjustedRect.width * vector3.x, pixelAdjustedRect.y + pixelAdjustedRect.height * vector3.y, pixelAdjustedRect.x + pixelAdjustedRect.width * vector3.z, pixelAdjustedRect.y + pixelAdjustedRect.height * vector3.w);
			return vector3;
		}

		// Token: 0x060032F4 RID: 13044 RVA: 0x0019C90C File Offset: 0x0019AB0C
		public override void SetNativeSize()
		{
			if (this.activeSprite != null)
			{
				float x = this.activeSprite.rect.width / this.pixelsPerUnit;
				float y = this.activeSprite.rect.height / this.pixelsPerUnit;
				base.rectTransform.anchorMax = base.rectTransform.anchorMin;
				base.rectTransform.sizeDelta = new Vector2(x, y);
				this.SetAllDirty();
			}
		}

		// Token: 0x060032F5 RID: 13045 RVA: 0x0019C98B File Offset: 0x0019AB8B
		protected override void OnPopulateMesh(VertexHelper toFill)
		{
			if (this.activeSprite == null)
			{
				base.OnPopulateMesh(toFill);
				return;
			}
			this.GenerateSimpleSprite(toFill, this.m_PreserveAspect);
		}

		// Token: 0x060032F6 RID: 13046 RVA: 0x0019C9B0 File Offset: 0x0019ABB0
		protected override void UpdateMaterial()
		{
			base.UpdateMaterial();
			if (this.activeSprite == null)
			{
				base.canvasRenderer.SetAlphaTexture(null);
				return;
			}
			Texture2D associatedAlphaSplitTexture = this.activeSprite.associatedAlphaSplitTexture;
			if (associatedAlphaSplitTexture != null)
			{
				base.canvasRenderer.SetAlphaTexture(associatedAlphaSplitTexture);
			}
		}

		// Token: 0x060032F7 RID: 13047 RVA: 0x0019CA00 File Offset: 0x0019AC00
		private void GenerateSimpleSprite(VertexHelper vh, bool lPreserveAspect)
		{
			Vector4 drawingDimensions = this.GetDrawingDimensions(lPreserveAspect);
			Vector4 vector = (this.activeSprite != null) ? DataUtility.GetOuterUV(this.activeSprite) : Vector4.zero;
			Color color = this.color;
			vh.Clear();
			vh.AddVert(new Vector3(drawingDimensions.x, drawingDimensions.y), color, new Vector2(vector.x, vector.y));
			vh.AddVert(new Vector3(drawingDimensions.x, drawingDimensions.w), color, new Vector2(vector.x, vector.w));
			vh.AddVert(new Vector3(drawingDimensions.z, drawingDimensions.w), color, new Vector2(vector.z, vector.w));
			vh.AddVert(new Vector3(drawingDimensions.z, drawingDimensions.y), color, new Vector2(vector.z, vector.y));
			vh.AddTriangle(0, 1, 2);
			vh.AddTriangle(2, 3, 0);
		}

		// Token: 0x060032F8 RID: 13048 RVA: 0x000023FD File Offset: 0x000005FD
		public virtual void CalculateLayoutInputHorizontal()
		{
		}

		// Token: 0x060032F9 RID: 13049 RVA: 0x000023FD File Offset: 0x000005FD
		public virtual void CalculateLayoutInputVertical()
		{
		}

		// Token: 0x1700028D RID: 653
		// (get) Token: 0x060032FA RID: 13050 RVA: 0x0007EB68 File Offset: 0x0007CD68
		public virtual float minWidth
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x1700028E RID: 654
		// (get) Token: 0x060032FB RID: 13051 RVA: 0x0019CB10 File Offset: 0x0019AD10
		public virtual float preferredWidth
		{
			get
			{
				if (this.activeSprite == null)
				{
					return 0f;
				}
				return this.activeSprite.rect.size.x / this.pixelsPerUnit;
			}
		}

		// Token: 0x1700028F RID: 655
		// (get) Token: 0x060032FC RID: 13052 RVA: 0x000DF448 File Offset: 0x000DD648
		public virtual float flexibleWidth
		{
			get
			{
				return -1f;
			}
		}

		// Token: 0x17000290 RID: 656
		// (get) Token: 0x060032FD RID: 13053 RVA: 0x0007EB68 File Offset: 0x0007CD68
		public virtual float minHeight
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x17000291 RID: 657
		// (get) Token: 0x060032FE RID: 13054 RVA: 0x0019CB50 File Offset: 0x0019AD50
		public virtual float preferredHeight
		{
			get
			{
				if (this.activeSprite == null)
				{
					return 0f;
				}
				return this.activeSprite.rect.size.y / this.pixelsPerUnit;
			}
		}

		// Token: 0x17000292 RID: 658
		// (get) Token: 0x060032FF RID: 13055 RVA: 0x000DF448 File Offset: 0x000DD648
		public virtual float flexibleHeight
		{
			get
			{
				return -1f;
			}
		}

		// Token: 0x17000293 RID: 659
		// (get) Token: 0x06003300 RID: 13056 RVA: 0x0002C538 File Offset: 0x0002A738
		public virtual int layoutPriority
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x06003301 RID: 13057 RVA: 0x0019CB90 File Offset: 0x0019AD90
		public virtual bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
		{
			if (this.alphaHitTestMinimumThreshold <= 0f)
			{
				return true;
			}
			if (this.alphaHitTestMinimumThreshold > 1f)
			{
				return false;
			}
			if (this.activeSprite == null)
			{
				return true;
			}
			Vector2 vector;
			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(base.rectTransform, screenPoint, eventCamera, out vector))
			{
				return false;
			}
			Rect pixelAdjustedRect = base.GetPixelAdjustedRect();
			vector.x += base.rectTransform.pivot.x * pixelAdjustedRect.width;
			vector.y += base.rectTransform.pivot.y * pixelAdjustedRect.height;
			vector = this.MapCoordinate(vector, pixelAdjustedRect);
			Rect textureRect = this.activeSprite.textureRect;
			Vector2 vector2 = new Vector2(vector.x / textureRect.width, vector.y / textureRect.height);
			float u = Mathf.Lerp(textureRect.x, textureRect.xMax, vector2.x) / (float)this.activeSprite.texture.width;
			float v = Mathf.Lerp(textureRect.y, textureRect.yMax, vector2.y) / (float)this.activeSprite.texture.height;
			bool result;
			try
			{
				result = (this.activeSprite.texture.GetPixelBilinear(u, v).a >= this.alphaHitTestMinimumThreshold);
			}
			catch (UnityException ex)
			{
				Debug.LogError("Using alphaHitTestMinimumThreshold greater than 0 on Image whose sprite texture cannot be read. " + ex.Message + " Also make sure to disable sprite packing for this sprite.", this);
				result = true;
			}
			return result;
		}

		// Token: 0x06003302 RID: 13058 RVA: 0x0019CD1C File Offset: 0x0019AF1C
		private Vector2 MapCoordinate(Vector2 local, Rect rect)
		{
			return new Vector2(local.x * this.activeSprite.rect.width / rect.width, local.y * this.activeSprite.rect.height / rect.height);
		}

		// Token: 0x06003303 RID: 13059 RVA: 0x0019CD72 File Offset: 0x0019AF72
		protected override void OnEnable()
		{
			base.OnEnable();
		}

		// Token: 0x06003304 RID: 13060 RVA: 0x0019CD7A File Offset: 0x0019AF7A
		protected override void OnDisable()
		{
			base.OnDisable();
		}

		// Token: 0x06003305 RID: 13061 RVA: 0x0019CD84 File Offset: 0x0019AF84
		public void RebildAnimationData()
		{
			if (this.m_Cols <= 1)
			{
				this.m_Cols = 1;
			}
			if (this.m_Rows <= 1)
			{
				this.m_Rows = 1;
			}
			if (this.m_FramesPerSec < 0)
			{
				this.m_FramesPerSec = 0;
			}
			if (this.m_Speed < 0f)
			{
				this.m_Speed = 0f;
			}
			this.SetVerticesDirty();
		}

		// Token: 0x06003306 RID: 13062 RVA: 0x0019CDE0 File Offset: 0x0019AFE0
		public void ModifyMesh(Mesh mesh)
		{
			if (!this.IsActive())
			{
				return;
			}
			using (VertexHelper vertexHelper = new VertexHelper(mesh))
			{
				this.ModifyMesh(vertexHelper);
				vertexHelper.FillMesh(mesh);
			}
		}

		// Token: 0x06003307 RID: 13063 RVA: 0x0019CE28 File Offset: 0x0019B028
		public void ModifyMesh(VertexHelper _vh)
		{
			if (!this.IsActive())
			{
				return;
			}
			List<UIVertex> list = new List<UIVertex>();
			_vh.GetUIVertexStream(list);
			Color color = new Color((float)this.m_Cols / 256f, (float)this.m_Rows / 256f, Common.map(this.m_Speed, 0f, 100f, 0f, 1f, false), (float)this.FramesPerSec / 256f);
			for (int i = 0; i < list.Count; i++)
			{
				UIVertex value = list[i];
				value.color = color.gamma;
				list[i] = value;
			}
			_vh.Clear();
			_vh.AddUIVertexTriangleStream(list);
		}

		// Token: 0x0400225A RID: 8794
		protected static Material s_ETC1DefaultUI;

		// Token: 0x0400225B RID: 8795
		protected static Material s_DefaultAnimUI;

		// Token: 0x0400225C RID: 8796
		[SerializeField]
		private Sprite m_Sprite;

		// Token: 0x0400225D RID: 8797
		[NonSerialized]
		private Sprite m_OverrideSprite;

		// Token: 0x0400225E RID: 8798
		[SerializeField]
		private bool m_PreserveAspect;

		// Token: 0x0400225F RID: 8799
		[SerializeField]
		private int m_Rows = 1;

		// Token: 0x04002260 RID: 8800
		[SerializeField]
		private int m_Cols = 1;

		// Token: 0x04002261 RID: 8801
		[SerializeField]
		private int m_FramesPerSec = 1;

		// Token: 0x04002262 RID: 8802
		[Range(0f, 100f)]
		[SerializeField]
		private float m_Speed = 1f;

		// Token: 0x04002263 RID: 8803
		private float m_AlphaHitTestMinimumThreshold;
	}
}
