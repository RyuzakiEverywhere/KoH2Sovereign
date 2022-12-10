using System;
using UnityEngine;

namespace Kino
{
	// Token: 0x020004A1 RID: 1185
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	[AddComponentMenu("Kino Image Effects/Ramp")]
	public class Ramp : MonoBehaviour
	{
		// Token: 0x17000427 RID: 1063
		// (get) Token: 0x06003E39 RID: 15929 RVA: 0x001DC580 File Offset: 0x001DA780
		// (set) Token: 0x06003E3A RID: 15930 RVA: 0x001DC588 File Offset: 0x001DA788
		public Color color1
		{
			get
			{
				return this._color1;
			}
			set
			{
				this._color1 = value;
			}
		}

		// Token: 0x17000428 RID: 1064
		// (get) Token: 0x06003E3B RID: 15931 RVA: 0x001DC591 File Offset: 0x001DA791
		// (set) Token: 0x06003E3C RID: 15932 RVA: 0x001DC599 File Offset: 0x001DA799
		public Color color2
		{
			get
			{
				return this._color2;
			}
			set
			{
				this._color2 = value;
			}
		}

		// Token: 0x17000429 RID: 1065
		// (get) Token: 0x06003E3D RID: 15933 RVA: 0x001DC5A2 File Offset: 0x001DA7A2
		// (set) Token: 0x06003E3E RID: 15934 RVA: 0x001DC5AA File Offset: 0x001DA7AA
		public float angle
		{
			get
			{
				return this._angle;
			}
			set
			{
				this._angle = value;
			}
		}

		// Token: 0x1700042A RID: 1066
		// (get) Token: 0x06003E3F RID: 15935 RVA: 0x001DC5B3 File Offset: 0x001DA7B3
		// (set) Token: 0x06003E40 RID: 15936 RVA: 0x001DC5BB File Offset: 0x001DA7BB
		public float opacity
		{
			get
			{
				return this._opacity;
			}
			set
			{
				this._opacity = value;
			}
		}

		// Token: 0x1700042B RID: 1067
		// (get) Token: 0x06003E41 RID: 15937 RVA: 0x001DC5C4 File Offset: 0x001DA7C4
		// (set) Token: 0x06003E42 RID: 15938 RVA: 0x001DC5CC File Offset: 0x001DA7CC
		public Ramp.BlendMode blendMode
		{
			get
			{
				return this._blendMode;
			}
			set
			{
				this._blendMode = value;
			}
		}

		// Token: 0x06003E43 RID: 15939 RVA: 0x001DC5D5 File Offset: 0x001DA7D5
		private void OnDisable()
		{
			if (this._material != null)
			{
				Object.Destroy(this._material);
				this._material = null;
			}
		}

		// Token: 0x06003E44 RID: 15940 RVA: 0x001DC5F7 File Offset: 0x001DA7F7
		private void OnEnable()
		{
			if (this._material == null)
			{
				this._material = new Material(this._shader);
				this._material.hideFlags = HideFlags.DontSave;
			}
		}

		// Token: 0x06003E45 RID: 15941 RVA: 0x001DC628 File Offset: 0x001DA828
		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			Color a;
			if (this._blendMode == Ramp.BlendMode.Multiply)
			{
				a = Color.white;
			}
			else if (this._blendMode == Ramp.BlendMode.Screen)
			{
				a = Color.black;
			}
			else
			{
				a = Color.gray;
			}
			float t = this._debug ? 1f : this._opacity;
			this._material.SetColor("_Color1", Color.Lerp(a, this._color1, t));
			this._material.SetColor("_Color2", Color.Lerp(a, this._color2, t));
			float f = 0.017453292f * this._angle;
			Vector2 v = new Vector2(Mathf.Cos(f), Mathf.Sin(f));
			this._material.SetVector("_Direction", v);
			this._material.shaderKeywords = null;
			this._material.EnableKeyword(Ramp._blendModeKeywords[(int)this._blendMode]);
			if (QualitySettings.activeColorSpace == ColorSpace.Linear)
			{
				this._material.EnableKeyword("_LINEAR");
			}
			else
			{
				this._material.DisableKeyword("_LINEAR");
			}
			if (this._debug)
			{
				this._material.EnableKeyword("_DEBUG");
			}
			else
			{
				this._material.DisableKeyword("_DEBUG");
			}
			Graphics.Blit(source, destination, this._material, 0);
		}

		// Token: 0x04002C44 RID: 11332
		[SerializeField]
		private Color _color1 = Color.blue;

		// Token: 0x04002C45 RID: 11333
		[SerializeField]
		private Color _color2 = Color.red;

		// Token: 0x04002C46 RID: 11334
		[SerializeField]
		[Range(-180f, 180f)]
		private float _angle = 90f;

		// Token: 0x04002C47 RID: 11335
		[SerializeField]
		[Range(0f, 1f)]
		private float _opacity = 1f;

		// Token: 0x04002C48 RID: 11336
		[SerializeField]
		private Ramp.BlendMode _blendMode = Ramp.BlendMode.Overlay;

		// Token: 0x04002C49 RID: 11337
		[SerializeField]
		private bool _debug;

		// Token: 0x04002C4A RID: 11338
		[SerializeField]
		private Shader _shader;

		// Token: 0x04002C4B RID: 11339
		private Material _material;

		// Token: 0x04002C4C RID: 11340
		private static string[] _blendModeKeywords = new string[]
		{
			"_MULTIPLY",
			"_SCREEN",
			"_OVERLAY",
			"_HARDLIGHT",
			"_SOFTLIGHT"
		};

		// Token: 0x0200097F RID: 2431
		public enum BlendMode
		{
			// Token: 0x04004426 RID: 17446
			Multiply,
			// Token: 0x04004427 RID: 17447
			Screen,
			// Token: 0x04004428 RID: 17448
			Overlay,
			// Token: 0x04004429 RID: 17449
			HardLight,
			// Token: 0x0400442A RID: 17450
			SoftLight
		}
	}
}
