using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Kino
{
	// Token: 0x020004A0 RID: 1184
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	[AddComponentMenu("Kino Image Effects/Obscurance")]
	public class Obscurance : MonoBehaviour
	{
		// Token: 0x1700041B RID: 1051
		// (get) Token: 0x06003E1D RID: 15901 RVA: 0x001DBFF2 File Offset: 0x001DA1F2
		// (set) Token: 0x06003E1E RID: 15902 RVA: 0x001DBFFA File Offset: 0x001DA1FA
		public float intensity
		{
			get
			{
				return this._intensity;
			}
			set
			{
				this._intensity = value;
			}
		}

		// Token: 0x1700041C RID: 1052
		// (get) Token: 0x06003E1F RID: 15903 RVA: 0x001DC003 File Offset: 0x001DA203
		// (set) Token: 0x06003E20 RID: 15904 RVA: 0x001DC015 File Offset: 0x001DA215
		public float radius
		{
			get
			{
				return Mathf.Max(this._radius, 0.0001f);
			}
			set
			{
				this._radius = value;
			}
		}

		// Token: 0x1700041D RID: 1053
		// (get) Token: 0x06003E21 RID: 15905 RVA: 0x001DC01E File Offset: 0x001DA21E
		// (set) Token: 0x06003E22 RID: 15906 RVA: 0x001DC026 File Offset: 0x001DA226
		public Obscurance.SampleCount sampleCount
		{
			get
			{
				return this._sampleCount;
			}
			set
			{
				this._sampleCount = value;
			}
		}

		// Token: 0x1700041E RID: 1054
		// (get) Token: 0x06003E23 RID: 15907 RVA: 0x001DC030 File Offset: 0x001DA230
		// (set) Token: 0x06003E24 RID: 15908 RVA: 0x001DC077 File Offset: 0x001DA277
		public int sampleCountValue
		{
			get
			{
				switch (this._sampleCount)
				{
				case Obscurance.SampleCount.Lowest:
					return 3;
				case Obscurance.SampleCount.Low:
					return 6;
				case Obscurance.SampleCount.Medium:
					return 12;
				case Obscurance.SampleCount.High:
					return 20;
				default:
					return Mathf.Clamp(this._sampleCountValue, 1, 256);
				}
			}
			set
			{
				this._sampleCountValue = value;
			}
		}

		// Token: 0x1700041F RID: 1055
		// (get) Token: 0x06003E25 RID: 15909 RVA: 0x001DC080 File Offset: 0x001DA280
		// (set) Token: 0x06003E26 RID: 15910 RVA: 0x001DC088 File Offset: 0x001DA288
		public int blurIterations
		{
			get
			{
				return this._blurIterations;
			}
			set
			{
				this._blurIterations = value;
			}
		}

		// Token: 0x17000420 RID: 1056
		// (get) Token: 0x06003E27 RID: 15911 RVA: 0x001DC091 File Offset: 0x001DA291
		// (set) Token: 0x06003E28 RID: 15912 RVA: 0x001DC099 File Offset: 0x001DA299
		public bool downsampling
		{
			get
			{
				return this._downsampling;
			}
			set
			{
				this._downsampling = value;
			}
		}

		// Token: 0x17000421 RID: 1057
		// (get) Token: 0x06003E29 RID: 15913 RVA: 0x001DC0A2 File Offset: 0x001DA2A2
		// (set) Token: 0x06003E2A RID: 15914 RVA: 0x001DC0C1 File Offset: 0x001DA2C1
		public bool ambientOnly
		{
			get
			{
				return this._ambientOnly && this.targetCamera.allowHDR && this.IsGBufferAvailable;
			}
			set
			{
				this._ambientOnly = value;
			}
		}

		// Token: 0x17000422 RID: 1058
		// (get) Token: 0x06003E2B RID: 15915 RVA: 0x001DC0CC File Offset: 0x001DA2CC
		private Material aoMaterial
		{
			get
			{
				if (this._aoMaterial == null)
				{
					Shader shader = Shader.Find("Hidden/Kino/Obscurance");
					this._aoMaterial = new Material(shader);
					this._aoMaterial.hideFlags = HideFlags.DontSave;
				}
				return this._aoMaterial;
			}
		}

		// Token: 0x17000423 RID: 1059
		// (get) Token: 0x06003E2C RID: 15916 RVA: 0x001DC111 File Offset: 0x001DA311
		private CommandBuffer aoCommands
		{
			get
			{
				if (this._aoCommands == null)
				{
					this._aoCommands = new CommandBuffer();
					this._aoCommands.name = "Kino.Obscurance";
				}
				return this._aoCommands;
			}
		}

		// Token: 0x17000424 RID: 1060
		// (get) Token: 0x06003E2D RID: 15917 RVA: 0x00035EC0 File Offset: 0x000340C0
		private Camera targetCamera
		{
			get
			{
				return base.GetComponent<Camera>();
			}
		}

		// Token: 0x17000425 RID: 1061
		// (get) Token: 0x06003E2E RID: 15918 RVA: 0x001DC13C File Offset: 0x001DA33C
		// (set) Token: 0x06003E2F RID: 15919 RVA: 0x001DC144 File Offset: 0x001DA344
		private Obscurance.PropertyObserver propertyObserver { get; set; }

		// Token: 0x17000426 RID: 1062
		// (get) Token: 0x06003E30 RID: 15920 RVA: 0x001DC14D File Offset: 0x001DA34D
		private bool IsGBufferAvailable
		{
			get
			{
				return this.targetCamera.actualRenderingPath == RenderingPath.DeferredShading;
			}
		}

		// Token: 0x06003E31 RID: 15921 RVA: 0x001DC160 File Offset: 0x001DA360
		private void BuildAOCommands()
		{
			CommandBuffer aoCommands = this.aoCommands;
			int num = this.targetCamera.pixelWidth;
			int num2 = this.targetCamera.pixelHeight;
			RenderTextureFormat format = RenderTextureFormat.R8;
			RenderTextureReadWrite readWrite = RenderTextureReadWrite.Linear;
			if (this.downsampling)
			{
				num /= 2;
				num2 /= 2;
			}
			Material aoMaterial = this.aoMaterial;
			int nameID = Shader.PropertyToID("_ObscuranceTexture");
			aoCommands.GetTemporaryRT(nameID, num, num2, 0, FilterMode.Bilinear, format, readWrite);
			aoCommands.Blit(null, nameID, aoMaterial, 0);
			if (this.blurIterations > 0)
			{
				int nameID2 = Shader.PropertyToID("_ObscuranceBlurTexture");
				aoCommands.GetTemporaryRT(nameID2, num, num2, 0, FilterMode.Bilinear, format, readWrite);
				for (int i = 0; i < this.blurIterations; i++)
				{
					aoCommands.SetGlobalVector("_BlurVector", Vector2.right);
					aoCommands.Blit(nameID, nameID2, aoMaterial, 1);
					aoCommands.SetGlobalVector("_BlurVector", Vector2.up);
					aoCommands.Blit(nameID2, nameID, aoMaterial, 1);
				}
				aoCommands.ReleaseTemporaryRT(nameID2);
			}
			RenderTargetIdentifier[] colors = new RenderTargetIdentifier[]
			{
				BuiltinRenderTextureType.GBuffer0,
				BuiltinRenderTextureType.CameraTarget
			};
			aoCommands.SetRenderTarget(colors, BuiltinRenderTextureType.CameraTarget);
			aoCommands.DrawMesh(this._quadMesh, Matrix4x4.identity, aoMaterial, 0, 3);
			aoCommands.ReleaseTemporaryRT(nameID);
		}

		// Token: 0x06003E32 RID: 15922 RVA: 0x001DC2C0 File Offset: 0x001DA4C0
		private void ExecuteAOPass(RenderTexture source, RenderTexture destination)
		{
			int num = source.width;
			int num2 = source.height;
			RenderTextureFormat format = RenderTextureFormat.R8;
			RenderTextureReadWrite readWrite = RenderTextureReadWrite.Linear;
			if (this.downsampling)
			{
				num /= 2;
				num2 /= 2;
			}
			Material aoMaterial = this.aoMaterial;
			RenderTexture temporary = RenderTexture.GetTemporary(num, num2, 0, format, readWrite);
			Graphics.Blit(null, temporary, aoMaterial, 0);
			if (this.blurIterations > 0)
			{
				RenderTexture temporary2 = RenderTexture.GetTemporary(num, num2, 0, format, readWrite);
				for (int i = 0; i < this.blurIterations; i++)
				{
					aoMaterial.SetVector("_BlurVector", Vector2.right);
					Graphics.Blit(temporary, temporary2, aoMaterial, 1);
					aoMaterial.SetVector("_BlurVector", Vector2.up);
					Graphics.Blit(temporary2, temporary, aoMaterial, 1);
				}
				RenderTexture.ReleaseTemporary(temporary2);
			}
			aoMaterial.SetTexture("_ObscuranceTexture", temporary);
			Graphics.Blit(source, destination, aoMaterial, 2);
			RenderTexture.ReleaseTemporary(temporary);
		}

		// Token: 0x06003E33 RID: 15923 RVA: 0x001DC3A4 File Offset: 0x001DA5A4
		private void UpdateMaterialProperties()
		{
			Material aoMaterial = this.aoMaterial;
			aoMaterial.shaderKeywords = null;
			aoMaterial.SetFloat("_Intensity", this.intensity);
			aoMaterial.SetFloat("_Radius", this.radius);
			aoMaterial.SetFloat("_TargetScale", this.downsampling ? 0.5f : 1f);
			if (this.IsGBufferAvailable)
			{
				aoMaterial.EnableKeyword("_SOURCE_GBUFFER");
			}
			if (this.sampleCount == Obscurance.SampleCount.Lowest)
			{
				aoMaterial.EnableKeyword("_SAMPLECOUNT_LOWEST");
				return;
			}
			aoMaterial.SetInt("_SampleCount", this.sampleCountValue);
		}

		// Token: 0x06003E34 RID: 15924 RVA: 0x001DC438 File Offset: 0x001DA638
		private void OnEnable()
		{
			if (this.ambientOnly)
			{
				this.targetCamera.AddCommandBuffer(CameraEvent.BeforeReflections, this.aoCommands);
			}
			if (!this.IsGBufferAvailable)
			{
				this.targetCamera.depthTextureMode |= DepthTextureMode.DepthNormals;
			}
		}

		// Token: 0x06003E35 RID: 15925 RVA: 0x001DC470 File Offset: 0x001DA670
		private void OnDisable()
		{
			if (this._aoMaterial != null)
			{
				Object.DestroyImmediate(this._aoMaterial);
			}
			this._aoMaterial = null;
			if (this._aoCommands != null)
			{
				this.targetCamera.RemoveCommandBuffer(CameraEvent.BeforeReflections, this._aoCommands);
			}
			this._aoCommands = null;
		}

		// Token: 0x06003E36 RID: 15926 RVA: 0x001DC4C0 File Offset: 0x001DA6C0
		private void Update()
		{
			if (this.propertyObserver.CheckNeedsReset(this, this.targetCamera))
			{
				this.OnDisable();
				this.OnEnable();
				if (this.ambientOnly)
				{
					this.aoCommands.Clear();
					this.BuildAOCommands();
				}
				this.propertyObserver.Update(this, this.targetCamera);
			}
			if (this.ambientOnly)
			{
				this.UpdateMaterialProperties();
			}
		}

		// Token: 0x06003E37 RID: 15927 RVA: 0x001DC52C File Offset: 0x001DA72C
		[ImageEffectOpaque]
		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (this.ambientOnly)
			{
				Graphics.Blit(source, destination);
				return;
			}
			this.UpdateMaterialProperties();
			this.ExecuteAOPass(source, destination);
		}

		// Token: 0x04002C38 RID: 11320
		[SerializeField]
		[Range(0f, 4f)]
		[Tooltip("Degree of darkness produced by the effect.")]
		private float _intensity = 1f;

		// Token: 0x04002C39 RID: 11321
		[SerializeField]
		[Tooltip("Radius of sample points, which affects extent of darkened areas.")]
		private float _radius = 0.3f;

		// Token: 0x04002C3A RID: 11322
		[SerializeField]
		[Tooltip("Number of sample points, which affects quality and performance.")]
		private Obscurance.SampleCount _sampleCount = Obscurance.SampleCount.Medium;

		// Token: 0x04002C3B RID: 11323
		[SerializeField]
		private int _sampleCountValue = 24;

		// Token: 0x04002C3C RID: 11324
		[SerializeField]
		[Range(0f, 4f)]
		[Tooltip("Number of iterations of the blur filter.")]
		private int _blurIterations = 2;

		// Token: 0x04002C3D RID: 11325
		[SerializeField]
		[Tooltip("Halves the resolution of the effect to increase performance.")]
		private bool _downsampling;

		// Token: 0x04002C3E RID: 11326
		[SerializeField]
		[Tooltip("If checked, the effect only affects ambient lighting.")]
		private bool _ambientOnly;

		// Token: 0x04002C3F RID: 11327
		[SerializeField]
		private Shader _aoShader;

		// Token: 0x04002C40 RID: 11328
		private Material _aoMaterial;

		// Token: 0x04002C41 RID: 11329
		private CommandBuffer _aoCommands;

		// Token: 0x04002C43 RID: 11331
		[SerializeField]
		private Mesh _quadMesh;

		// Token: 0x0200097D RID: 2429
		public enum SampleCount
		{
			// Token: 0x0400441B RID: 17435
			Lowest,
			// Token: 0x0400441C RID: 17436
			Low,
			// Token: 0x0400441D RID: 17437
			Medium,
			// Token: 0x0400441E RID: 17438
			High,
			// Token: 0x0400441F RID: 17439
			Variable
		}

		// Token: 0x0200097E RID: 2430
		private struct PropertyObserver
		{
			// Token: 0x06005410 RID: 21520 RVA: 0x00245308 File Offset: 0x00243508
			public bool CheckNeedsReset(Obscurance target, Camera camera)
			{
				return this._blurIterations != target.blurIterations || this._downsampling != target.downsampling || this._ambientOnly != target.ambientOnly || this._pixelWidth != camera.pixelWidth || this._pixelHeight != camera.pixelHeight;
			}

			// Token: 0x06005411 RID: 21521 RVA: 0x00245360 File Offset: 0x00243560
			public void Update(Obscurance target, Camera camera)
			{
				this._blurIterations = target.blurIterations;
				this._downsampling = target.downsampling;
				this._ambientOnly = target.ambientOnly;
				this._pixelWidth = camera.pixelWidth;
				this._pixelHeight = camera.pixelHeight;
			}

			// Token: 0x04004420 RID: 17440
			private int _blurIterations;

			// Token: 0x04004421 RID: 17441
			private bool _downsampling;

			// Token: 0x04004422 RID: 17442
			private bool _ambientOnly;

			// Token: 0x04004423 RID: 17443
			private int _pixelWidth;

			// Token: 0x04004424 RID: 17444
			private int _pixelHeight;
		}
	}
}
