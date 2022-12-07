using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200001A RID: 26
[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
public class MirrorReflection : MonoBehaviour
{
	// Token: 0x0600004A RID: 74 RVA: 0x00003AAA File Offset: 0x00001CAA
	private void OnEnable()
	{
		this.setMaterial();
		this.FindTheSun();
	}

	// Token: 0x0600004B RID: 75 RVA: 0x00003AAA File Offset: 0x00001CAA
	private void Start()
	{
		this.setMaterial();
		this.FindTheSun();
	}

	// Token: 0x0600004C RID: 76 RVA: 0x00003AB8 File Offset: 0x00001CB8
	private void FindTheSun()
	{
		if (this.Sun != null)
		{
			return;
		}
		GameObject gameObject = GameObject.Find("SUN");
		this.Sun = gameObject.GetComponent<Light>();
	}

	// Token: 0x0600004D RID: 77 RVA: 0x00003AEB File Offset: 0x00001CEB
	public void setMaterial()
	{
		this.m_SharedMaterial = base.GetComponent<Renderer>().sharedMaterial;
	}

	// Token: 0x0600004E RID: 78 RVA: 0x00003B00 File Offset: 0x00001D00
	private Camera CreateReflectionCameraFor(Camera cam)
	{
		string name = base.gameObject.name + "Reflection" + cam.name;
		GameObject gameObject = GameObject.Find(name);
		if (!gameObject)
		{
			gameObject = new GameObject(name, new Type[]
			{
				typeof(Camera)
			});
			gameObject.hideFlags = HideFlags.HideAndDontSave;
		}
		if (!gameObject.GetComponent(typeof(Camera)))
		{
			gameObject.AddComponent(typeof(Camera));
		}
		Camera component = gameObject.GetComponent<Camera>();
		component.backgroundColor = this.clearColor;
		component.clearFlags = (this.reflectSkybox ? CameraClearFlags.Skybox : CameraClearFlags.Color);
		this.SetStandardCameraParameter(component, this.reflectionMask);
		if (!component.targetTexture)
		{
			component.targetTexture = this.CreateTextureFor(cam);
		}
		return component;
	}

	// Token: 0x0600004F RID: 79 RVA: 0x00003BCF File Offset: 0x00001DCF
	private void SetStandardCameraParameter(Camera cam, LayerMask mask)
	{
		cam.cullingMask = (mask & ~(1 << LayerMask.NameToLayer("Water")));
		cam.backgroundColor = Color.black;
		cam.enabled = false;
	}

	// Token: 0x06000050 RID: 80 RVA: 0x00003C00 File Offset: 0x00001E00
	private RenderTexture CreateTextureFor(Camera cam)
	{
		return new RenderTexture(Mathf.FloorToInt((float)cam.pixelWidth * 0.5f), Mathf.FloorToInt((float)cam.pixelHeight * 0.5f), 24)
		{
			hideFlags = HideFlags.DontSave
		};
	}

	// Token: 0x06000051 RID: 81 RVA: 0x00003C38 File Offset: 0x00001E38
	public void RenderHelpCameras(Camera currentCam)
	{
		if (this.m_HelperCameras == null)
		{
			this.m_HelperCameras = new Dictionary<Camera, bool>();
		}
		if (!this.m_HelperCameras.ContainsKey(currentCam))
		{
			this.m_HelperCameras.Add(currentCam, false);
		}
		if (this.m_HelperCameras[currentCam] && !this.UpdateSceneView)
		{
			return;
		}
		if (!this.m_ReflectionCamera)
		{
			this.m_ReflectionCamera = this.CreateReflectionCameraFor(currentCam);
		}
		this.RenderReflectionFor(currentCam, this.m_ReflectionCamera);
		this.m_HelperCameras[currentCam] = true;
	}

	// Token: 0x06000052 RID: 82 RVA: 0x00003CBE File Offset: 0x00001EBE
	public void LateUpdate()
	{
		if (this.m_HelperCameras != null)
		{
			this.m_HelperCameras.Clear();
		}
	}

	// Token: 0x06000053 RID: 83 RVA: 0x00003CD3 File Offset: 0x00001ED3
	public void WaterTileBeingRendered(Transform tr, Camera currentCam)
	{
		this.RenderHelpCameras(currentCam);
		if (this.m_ReflectionCamera && this.m_SharedMaterial)
		{
			this.m_SharedMaterial.SetTexture(this.reflectionSampler, this.m_ReflectionCamera.targetTexture);
		}
	}

	// Token: 0x06000054 RID: 84 RVA: 0x00003D12 File Offset: 0x00001F12
	public void OnWillRenderObject()
	{
		this.WaterTileBeingRendered(base.transform, Camera.current);
	}

	// Token: 0x06000055 RID: 85 RVA: 0x00003D28 File Offset: 0x00001F28
	private void RenderReflectionFor(Camera cam, Camera reflectCamera)
	{
		if (!reflectCamera)
		{
			return;
		}
		if (this.m_SharedMaterial && !this.m_SharedMaterial.HasProperty(this.reflectionSampler))
		{
			return;
		}
		int pixelLightCount = QualitySettings.pixelLightCount;
		if (this.m_DisablePixelLights)
		{
			QualitySettings.pixelLightCount = 0;
		}
		reflectCamera.cullingMask = (-17 & this.reflectionMask.value);
		this.SaneCameraSettings(reflectCamera);
		reflectCamera.backgroundColor = this.clearColor;
		reflectCamera.clearFlags = (this.reflectSkybox ? CameraClearFlags.Skybox : CameraClearFlags.Color);
		if (this.reflectSkybox && cam.gameObject.GetComponent(typeof(Skybox)))
		{
			Skybox skybox = (Skybox)reflectCamera.gameObject.GetComponent(typeof(Skybox));
			if (!skybox)
			{
				skybox = (Skybox)reflectCamera.gameObject.AddComponent(typeof(Skybox));
			}
			skybox.material = ((Skybox)cam.GetComponent(typeof(Skybox))).material;
		}
		GL.invertCulling = true;
		Transform transform = base.transform;
		Vector3 eulerAngles = cam.transform.eulerAngles;
		reflectCamera.transform.eulerAngles = new Vector3(-eulerAngles.x, eulerAngles.y, eulerAngles.z);
		reflectCamera.transform.position = cam.transform.position;
		Vector3 position = transform.transform.position;
		position.y = transform.position.y;
		Vector3 up = transform.transform.up;
		float w = -Vector3.Dot(up, position) - this.clipPlaneOffset;
		Vector4 plane = new Vector4(up.x, up.y, up.z, w);
		Matrix4x4 matrix4x = Matrix4x4.zero;
		matrix4x = MirrorReflection.CalculateReflectionMatrix(matrix4x, plane);
		this.m_Oldpos = cam.transform.position;
		Vector3 position2 = matrix4x.MultiplyPoint(this.m_Oldpos);
		reflectCamera.worldToCameraMatrix = cam.worldToCameraMatrix * matrix4x;
		Vector4 clipPlane = this.CameraSpacePlane(reflectCamera, position, up, 1f);
		Matrix4x4 matrix4x2 = cam.projectionMatrix;
		matrix4x2 = MirrorReflection.CalculateObliqueMatrix(matrix4x2, clipPlane);
		reflectCamera.projectionMatrix = matrix4x2;
		reflectCamera.transform.position = position2;
		Vector3 eulerAngles2 = cam.transform.eulerAngles;
		reflectCamera.transform.eulerAngles = new Vector3(-eulerAngles2.x, eulerAngles2.y, eulerAngles2.z);
		LightShadows shadows = LightShadows.None;
		if (this.DisableShadows && this.Sun != null)
		{
			shadows = this.Sun.shadows;
			this.Sun.shadows = LightShadows.None;
		}
		reflectCamera.Render();
		if (this.DisableShadows && this.Sun != null)
		{
			this.Sun.shadows = shadows;
		}
		GL.invertCulling = false;
		if (this.m_DisablePixelLights)
		{
			QualitySettings.pixelLightCount = pixelLightCount;
		}
	}

	// Token: 0x06000056 RID: 86 RVA: 0x00003FFF File Offset: 0x000021FF
	private void SaneCameraSettings(Camera helperCam)
	{
		helperCam.depthTextureMode = DepthTextureMode.None;
		helperCam.backgroundColor = Color.black;
		helperCam.clearFlags = CameraClearFlags.Color;
		helperCam.renderingPath = RenderingPath.Forward;
	}

	// Token: 0x06000057 RID: 87 RVA: 0x00004024 File Offset: 0x00002224
	private static Matrix4x4 CalculateObliqueMatrix(Matrix4x4 projection, Vector4 clipPlane)
	{
		Vector4 b = projection.inverse * new Vector4(MirrorReflection.Sgn(clipPlane.x), MirrorReflection.Sgn(clipPlane.y), 1f, 1f);
		Vector4 vector = clipPlane * (2f / Vector4.Dot(clipPlane, b));
		projection[2] = vector.x - projection[3];
		projection[6] = vector.y - projection[7];
		projection[10] = vector.z - projection[11];
		projection[14] = vector.w - projection[15];
		return projection;
	}

	// Token: 0x06000058 RID: 88 RVA: 0x000040D8 File Offset: 0x000022D8
	private static Matrix4x4 CalculateReflectionMatrix(Matrix4x4 reflectionMat, Vector4 plane)
	{
		reflectionMat.m00 = 1f - 2f * plane[0] * plane[0];
		reflectionMat.m01 = -2f * plane[0] * plane[1];
		reflectionMat.m02 = -2f * plane[0] * plane[2];
		reflectionMat.m03 = -2f * plane[3] * plane[0];
		reflectionMat.m10 = -2f * plane[1] * plane[0];
		reflectionMat.m11 = 1f - 2f * plane[1] * plane[1];
		reflectionMat.m12 = -2f * plane[1] * plane[2];
		reflectionMat.m13 = -2f * plane[3] * plane[1];
		reflectionMat.m20 = -2f * plane[2] * plane[0];
		reflectionMat.m21 = -2f * plane[2] * plane[1];
		reflectionMat.m22 = 1f - 2f * plane[2] * plane[2];
		reflectionMat.m23 = -2f * plane[3] * plane[2];
		reflectionMat.m30 = 0f;
		reflectionMat.m31 = 0f;
		reflectionMat.m32 = 0f;
		reflectionMat.m33 = 1f;
		return reflectionMat;
	}

	// Token: 0x06000059 RID: 89 RVA: 0x00004290 File Offset: 0x00002490
	private static float Sgn(float a)
	{
		if (a > 0f)
		{
			return 1f;
		}
		if (a < 0f)
		{
			return -1f;
		}
		return 0f;
	}

	// Token: 0x0600005A RID: 90 RVA: 0x000042B4 File Offset: 0x000024B4
	private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
	{
		Vector3 point = pos + normal * this.clipPlaneOffset;
		Matrix4x4 worldToCameraMatrix = cam.worldToCameraMatrix;
		Vector3 lhs = worldToCameraMatrix.MultiplyPoint(point);
		Vector3 vector = worldToCameraMatrix.MultiplyVector(normal).normalized * sideSign;
		return new Vector4(vector.x, vector.y, vector.z, -Vector3.Dot(lhs, vector));
	}

	// Token: 0x04000075 RID: 117
	public LayerMask reflectionMask = -1;

	// Token: 0x04000076 RID: 118
	[Tooltip("Color used instead of skybox if you choose to not render it.")]
	public Color clearColor = Color.grey;

	// Token: 0x04000077 RID: 119
	public bool reflectSkybox = true;

	// Token: 0x04000078 RID: 120
	public bool m_DisablePixelLights;

	// Token: 0x04000079 RID: 121
	[Tooltip("You won't be able to select objects in the scene when thi is active.")]
	public bool UpdateSceneView;

	// Token: 0x0400007A RID: 122
	[Tooltip("You can disable shadows rendering ...")]
	public bool DisableShadows;

	// Token: 0x0400007B RID: 123
	public Light Sun;

	// Token: 0x0400007C RID: 124
	public float clipPlaneOffset = 0.07f;

	// Token: 0x0400007D RID: 125
	private string reflectionSampler = "_ReflectionTex";

	// Token: 0x0400007E RID: 126
	private Vector3 m_Oldpos;

	// Token: 0x0400007F RID: 127
	private Camera m_ReflectionCamera;

	// Token: 0x04000080 RID: 128
	private Material m_SharedMaterial;

	// Token: 0x04000081 RID: 129
	private Dictionary<Camera, bool> m_HelperCameras;
}
