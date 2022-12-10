using System;
using UnityEngine;

namespace Kino
{
	// Token: 0x0200049F RID: 1183
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	[AddComponentMenu("Kino Image Effects/Fog")]
	public class Fog : MonoBehaviour
	{
		// Token: 0x17000418 RID: 1048
		// (get) Token: 0x06003E13 RID: 15891 RVA: 0x001DBB27 File Offset: 0x001D9D27
		// (set) Token: 0x06003E14 RID: 15892 RVA: 0x001DBB2F File Offset: 0x001D9D2F
		public float startDistance
		{
			get
			{
				return this._startDistance;
			}
			set
			{
				this._startDistance = value;
			}
		}

		// Token: 0x17000419 RID: 1049
		// (get) Token: 0x06003E15 RID: 15893 RVA: 0x001DBB38 File Offset: 0x001D9D38
		// (set) Token: 0x06003E16 RID: 15894 RVA: 0x001DBB40 File Offset: 0x001D9D40
		public bool useRadialDistance
		{
			get
			{
				return this._useRadialDistance;
			}
			set
			{
				this._useRadialDistance = value;
			}
		}

		// Token: 0x1700041A RID: 1050
		// (get) Token: 0x06003E17 RID: 15895 RVA: 0x001DBB49 File Offset: 0x001D9D49
		// (set) Token: 0x06003E18 RID: 15896 RVA: 0x001DBB51 File Offset: 0x001D9D51
		public bool fadeToSkybox
		{
			get
			{
				return this._fadeToSkybox;
			}
			set
			{
				this._fadeToSkybox = value;
			}
		}

		// Token: 0x06003E19 RID: 15897 RVA: 0x001DBB5A File Offset: 0x001D9D5A
		private void OnEnable()
		{
			base.GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
		}

		// Token: 0x06003E1A RID: 15898 RVA: 0x001DBB6F File Offset: 0x001D9D6F
		private void OnDestroy()
		{
			if (this._material != null)
			{
				Object.Destroy(this._material);
				this._material = null;
			}
		}

		// Token: 0x06003E1B RID: 15899 RVA: 0x001DBB94 File Offset: 0x001D9D94
		[ImageEffectOpaque]
		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (this._material == null)
			{
				this._material = new Material(this._shader);
				this._material.hideFlags = HideFlags.DontSave;
			}
			this._startDistance = Mathf.Max(this._startDistance, 0f);
			this._material.SetFloat("_DistanceOffset", this._startDistance);
			FogMode fogMode = RenderSettings.fogMode;
			if (fogMode == FogMode.Linear)
			{
				float fogStartDistance = RenderSettings.fogStartDistance;
				float fogEndDistance = RenderSettings.fogEndDistance;
				float num = 1f / Mathf.Max(fogEndDistance - fogStartDistance, 1E-06f);
				this._material.SetFloat("_LinearGrad", -num);
				this._material.SetFloat("_LinearOffs", fogEndDistance * num);
				this._material.DisableKeyword("FOG_EXP");
				this._material.DisableKeyword("FOG_EXP2");
			}
			else if (fogMode == FogMode.Exponential)
			{
				float fogDensity = RenderSettings.fogDensity;
				this._material.SetFloat("_Density", 1.442695f * fogDensity);
				this._material.EnableKeyword("FOG_EXP");
				this._material.DisableKeyword("FOG_EXP2");
			}
			else
			{
				float fogDensity2 = RenderSettings.fogDensity;
				this._material.SetFloat("_Density", 1.2011224f * fogDensity2);
				this._material.DisableKeyword("FOG_EXP");
				this._material.EnableKeyword("FOG_EXP2");
			}
			if (this._useRadialDistance)
			{
				this._material.EnableKeyword("RADIAL_DIST");
			}
			else
			{
				this._material.DisableKeyword("RADIAL_DIST");
			}
			if (this._fadeToSkybox)
			{
				this._material.EnableKeyword("USE_SKYBOX");
				Material skybox = RenderSettings.skybox;
				this._material.SetTexture("_SkyCubemap", skybox.GetTexture("_Tex"));
				this._material.SetColor("_SkyTint", skybox.GetColor("_Tint"));
				this._material.SetFloat("_SkyExposure", skybox.GetFloat("_Exposure"));
				this._material.SetFloat("_SkyRotation", skybox.GetFloat("_Rotation"));
			}
			else
			{
				this._material.DisableKeyword("USE_SKYBOX");
				this._material.SetColor("_FogColor", RenderSettings.fogColor);
			}
			Camera component = base.GetComponent<Camera>();
			Transform transform = component.transform;
			float nearClipPlane = component.nearClipPlane;
			float farClipPlane = component.farClipPlane;
			float d = Mathf.Tan(component.fieldOfView * 0.017453292f / 2f);
			Vector3 b = transform.right * nearClipPlane * d * component.aspect;
			Vector3 b2 = transform.up * nearClipPlane * d;
			Vector3 vector = transform.forward * nearClipPlane - b + b2;
			Vector3 vector2 = transform.forward * nearClipPlane + b + b2;
			Vector3 vector3 = transform.forward * nearClipPlane + b - b2;
			Vector3 vector4 = transform.forward * nearClipPlane - b - b2;
			float d2 = vector.magnitude * farClipPlane / nearClipPlane;
			RenderTexture.active = destination;
			this._material.SetTexture("_MainTex", source);
			this._material.SetPass(0);
			GL.PushMatrix();
			GL.LoadOrtho();
			GL.Begin(7);
			GL.MultiTexCoord2(0, 0f, 0f);
			GL.MultiTexCoord(1, vector4.normalized * d2);
			GL.Vertex3(0f, 0f, 0.1f);
			GL.MultiTexCoord2(0, 1f, 0f);
			GL.MultiTexCoord(1, vector3.normalized * d2);
			GL.Vertex3(1f, 0f, 0.1f);
			GL.MultiTexCoord2(0, 1f, 1f);
			GL.MultiTexCoord(1, vector2.normalized * d2);
			GL.Vertex3(1f, 1f, 0.1f);
			GL.MultiTexCoord2(0, 0f, 1f);
			GL.MultiTexCoord(1, vector.normalized * d2);
			GL.Vertex3(0f, 1f, 0.1f);
			GL.End();
			GL.PopMatrix();
		}

		// Token: 0x04002C33 RID: 11315
		[SerializeField]
		private float _startDistance = 1f;

		// Token: 0x04002C34 RID: 11316
		[SerializeField]
		private bool _useRadialDistance;

		// Token: 0x04002C35 RID: 11317
		[SerializeField]
		private bool _fadeToSkybox;

		// Token: 0x04002C36 RID: 11318
		[SerializeField]
		private Shader _shader;

		// Token: 0x04002C37 RID: 11319
		private Material _material;
	}
}
