using System;
using UnityEngine;

// Token: 0x02000019 RID: 25
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Rendering/UnderWater Fog")]
public class UnderWaterFog : MonoBehaviour
{
	// Token: 0x06000045 RID: 69 RVA: 0x000035E6 File Offset: 0x000017E6
	private void OnEnable()
	{
		this.CheckResources();
	}

	// Token: 0x06000046 RID: 70 RVA: 0x000035F0 File Offset: 0x000017F0
	public bool CheckResources()
	{
		if (this.fogShader == null)
		{
			this.fogShader = Shader.Find("Hidden/UnderWaterFog");
		}
		if (this.fogMaterial == null)
		{
			this.fogMaterial = new Material(this.fogShader);
		}
		bool result = true;
		if (!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
		{
			return false;
		}
		base.GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
		return result;
	}

	// Token: 0x06000047 RID: 71 RVA: 0x0000365C File Offset: 0x0000185C
	[ImageEffectOpaque]
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!this.CheckResources())
		{
			Graphics.Blit(source, destination);
			return;
		}
		Camera component = base.GetComponent<Camera>();
		Transform transform = component.transform;
		float nearClipPlane = component.nearClipPlane;
		float farClipPlane = component.farClipPlane;
		float fieldOfView = component.fieldOfView;
		float aspect = component.aspect;
		Matrix4x4 identity = Matrix4x4.identity;
		float num = fieldOfView * 0.5f;
		Vector3 b = transform.right * nearClipPlane * Mathf.Tan(num * 0.017453292f) * aspect;
		Vector3 b2 = transform.up * nearClipPlane * Mathf.Tan(num * 0.017453292f);
		Vector3 vector = transform.forward * nearClipPlane - b + b2;
		float d = vector.magnitude * farClipPlane / nearClipPlane;
		vector.Normalize();
		vector *= d;
		Vector3 vector2 = transform.forward * nearClipPlane + b + b2;
		vector2.Normalize();
		vector2 *= d;
		Vector3 vector3 = transform.forward * nearClipPlane + b - b2;
		vector3.Normalize();
		vector3 *= d;
		Vector3 vector4 = transform.forward * nearClipPlane - b - b2;
		vector4.Normalize();
		vector4 *= d;
		identity.SetRow(0, vector);
		identity.SetRow(1, vector2);
		identity.SetRow(2, vector3);
		identity.SetRow(3, vector4);
		Vector3 position = transform.position;
		float num2 = position.y - this.height;
		float z = (num2 <= 0f) ? 1f : 0f;
		this.fogMaterial.SetColor("_Color", this.fogColor);
		this.fogMaterial.SetMatrix("_FrustumCornersWS", identity);
		this.fogMaterial.SetVector("_CameraWS", position);
		this.fogMaterial.SetVector("_HeightParams", new Vector4(this.height, num2, z, this.heightDensity * 0.5f));
		this.fogMaterial.SetVector("_DistanceParams", new Vector4(-Mathf.Max(this.startDistance, 0f), 1f, 0f, 0f));
		FogMode fogMode = RenderSettings.fogMode;
		float num3 = this.heightDensity;
		float fogStartDistance = RenderSettings.fogStartDistance;
		float fogEndDistance = RenderSettings.fogEndDistance;
		bool flag = fogMode == FogMode.Linear;
		float num4 = flag ? (fogEndDistance - fogStartDistance) : 0f;
		float num5 = (Mathf.Abs(num4) > 0.0001f) ? (1f / num4) : 0f;
		Vector4 value;
		value.x = num3 * 1.2011224f;
		value.y = num3 * 1.442695f;
		value.z = (flag ? (-num5) : 0f);
		value.w = (flag ? (fogEndDistance * num5) : 0f);
		this.fogMaterial.SetVector("_SceneFogParams", value);
		this.fogMaterial.SetVector("_SceneFogMode", new Vector4((float)fogMode, 0f, 0f, 0f));
		UnderWaterFog.CustomGraphicsBlit(source, destination, this.fogMaterial, 0);
	}

	// Token: 0x06000048 RID: 72 RVA: 0x000039B0 File Offset: 0x00001BB0
	private static void CustomGraphicsBlit(RenderTexture source, RenderTexture dest, Material fxMaterial, int passNr)
	{
		RenderTexture.active = dest;
		fxMaterial.SetTexture("_MainTex", source);
		GL.PushMatrix();
		GL.LoadOrtho();
		fxMaterial.SetPass(passNr);
		GL.Begin(7);
		GL.MultiTexCoord2(0, 0f, 0f);
		GL.Vertex3(0f, 0f, 3f);
		GL.MultiTexCoord2(0, 1f, 0f);
		GL.Vertex3(1f, 0f, 2f);
		GL.MultiTexCoord2(0, 1f, 1f);
		GL.Vertex3(1f, 1f, 1f);
		GL.MultiTexCoord2(0, 0f, 1f);
		GL.Vertex3(0f, 1f, 0f);
		GL.End();
		GL.PopMatrix();
	}

	// Token: 0x0400006F RID: 111
	public Color fogColor = Color.white;

	// Token: 0x04000070 RID: 112
	[Tooltip("Fog top Y coordinate")]
	public float height = 1f;

	// Token: 0x04000071 RID: 113
	[Range(0.001f, 10f)]
	public float heightDensity = 2f;

	// Token: 0x04000072 RID: 114
	[Tooltip("Push fog away from the camera by this amount")]
	public float startDistance;

	// Token: 0x04000073 RID: 115
	public Shader fogShader;

	// Token: 0x04000074 RID: 116
	private Material fogMaterial;
}
