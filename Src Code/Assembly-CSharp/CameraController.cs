using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

// Token: 0x020000DE RID: 222
[ExecuteInEditMode]
public class CameraController : MonoBehaviour
{
	// Token: 0x17000083 RID: 131
	// (get) Token: 0x06000B0D RID: 2829 RVA: 0x0007F7BF File Offset: 0x0007D9BF
	public static GameCamera GameCamera
	{
		get
		{
			if (!(CameraController.sm_Instance != null))
			{
				return null;
			}
			return CameraController.sm_Instance.CurrentGameCamera;
		}
	}

	// Token: 0x17000084 RID: 132
	// (get) Token: 0x06000B0E RID: 2830 RVA: 0x0007F7DA File Offset: 0x0007D9DA
	public static Camera MainCamera
	{
		get
		{
			if (CameraController.sm_Instance == null)
			{
				return Camera.main;
			}
			if (!(CameraController.sm_Instance.CurrentGameCamera != null))
			{
				return null;
			}
			return CameraController.sm_Instance.CurrentGameCamera.Camera;
		}
	}

	// Token: 0x17000085 RID: 133
	// (get) Token: 0x06000B0F RID: 2831 RVA: 0x0007F812 File Offset: 0x0007DA12
	public static StudioListener FMODAudioListener
	{
		get
		{
			if (CameraController.sm_Instance == null)
			{
				return null;
			}
			if (!(CameraController.sm_Instance.CurrentGameCamera != null))
			{
				return null;
			}
			return CameraController.sm_Instance.CurrentGameCamera.FmodAudioListener;
		}
	}

	// Token: 0x17000086 RID: 134
	// (get) Token: 0x06000B10 RID: 2832 RVA: 0x0007F846 File Offset: 0x0007DA46
	// (set) Token: 0x06000B11 RID: 2833 RVA: 0x0007F84E File Offset: 0x0007DA4E
	public GameCamera CurrentGameCamera { get; private set; }

	// Token: 0x06000B12 RID: 2834 RVA: 0x0007F857 File Offset: 0x0007DA57
	public static CameraController Get()
	{
		return CameraController.sm_Instance;
	}

	// Token: 0x06000B13 RID: 2835 RVA: 0x0007F85E File Offset: 0x0007DA5E
	public static bool IsReady()
	{
		return CameraController.sm_Instance != null && CameraController.sm_Instance.initted && CameraController.sm_Instance.CurrentGameCamera != null;
	}

	// Token: 0x06000B14 RID: 2836 RVA: 0x0007F88B File Offset: 0x0007DA8B
	private void Awake()
	{
		if (!this.initted)
		{
			this.Init();
		}
	}

	// Token: 0x06000B15 RID: 2837 RVA: 0x0007F89B File Offset: 0x0007DA9B
	private void OnDestroy()
	{
		this.ReleaseControl();
	}

	// Token: 0x06000B16 RID: 2838 RVA: 0x0007F8A3 File Offset: 0x0007DAA3
	private void OnEnable()
	{
		if (!this.initted)
		{
			this.Init();
		}
		this.AssumeControl();
	}

	// Token: 0x06000B17 RID: 2839 RVA: 0x0007F8B9 File Offset: 0x0007DAB9
	private void OnDisable()
	{
		if (this.initted)
		{
			this.ReleaseControl();
		}
	}

	// Token: 0x06000B18 RID: 2840 RVA: 0x0007F8C9 File Offset: 0x0007DAC9
	private void AssumeControl()
	{
		if (CameraController.sm_Instance != null)
		{
			this.ReleaseControl();
		}
		CameraController.sm_Instance = this;
		if (ViewMode.current != null)
		{
			ViewMode.current.Apply();
		}
		else
		{
			ViewMode.WorldView.Apply();
		}
		this.ViewModeChanged();
	}

	// Token: 0x06000B19 RID: 2841 RVA: 0x0007F907 File Offset: 0x0007DB07
	private void ReleaseControl()
	{
		if (CameraController.sm_Instance == this)
		{
			CameraController.sm_Instance = null;
			this.CurrentGameCamera = null;
		}
	}

	// Token: 0x06000B1A RID: 2842 RVA: 0x0007F924 File Offset: 0x0007DB24
	private void Init()
	{
		this.initted = true;
		this.AllCameras.Clear();
		this.WVCamera = Common.FindChildComponent<GameCamera>(base.gameObject, "WVCamera");
		if (this.WVCamera != null)
		{
			this.AllCameras.Add(this.WVCamera);
		}
		this.PVCamera = Common.FindChildComponent<GameCamera>(base.gameObject, "PVCamera");
		if (this.PVCamera != null)
		{
			this.AllCameras.Add(this.PVCamera);
		}
		this.BVCamera = Common.FindChildComponent<GameCamera>(base.gameObject, "BVCamera");
		if (this.BVCamera != null)
		{
			this.AllCameras.Add(this.BVCamera);
		}
		if (base.GetComponentInChildren<VisibilityDetector>() == null)
		{
			GameObject gameObject = new GameObject("VisibilityDetector");
			gameObject.transform.SetParent(base.transform, false);
			gameObject.AddComponent<VisibilityDetector>();
		}
	}

	// Token: 0x06000B1B RID: 2843 RVA: 0x0007FA14 File Offset: 0x0007DC14
	public static void LookAt(Vector3 pt)
	{
		if (CameraController.sm_Instance == null)
		{
			return;
		}
		for (int i = 0; i < CameraController.sm_Instance.AllCameras.Count; i++)
		{
			CameraController.sm_Instance.AllCameras[i].LookAt(pt, false);
		}
	}

	// Token: 0x06000B1C RID: 2844 RVA: 0x0007FA60 File Offset: 0x0007DC60
	public static void Zoom(float zoom)
	{
		if (CameraController.sm_Instance == null)
		{
			return;
		}
		for (int i = 0; i < CameraController.sm_Instance.AllCameras.Count; i++)
		{
			CameraController.sm_Instance.AllCameras[i].Zoom(zoom, false);
		}
	}

	// Token: 0x06000B1D RID: 2845 RVA: 0x0007FAAC File Offset: 0x0007DCAC
	public static void Yaw(float yaw)
	{
		Debug.Log(yaw);
		if (CameraController.sm_Instance == null)
		{
			return;
		}
		for (int i = 0; i < CameraController.sm_Instance.AllCameras.Count; i++)
		{
			CameraController.sm_Instance.AllCameras[i].Yaw(yaw, false);
		}
	}

	// Token: 0x06000B1E RID: 2846 RVA: 0x0007FB04 File Offset: 0x0007DD04
	public static void Set(Vector3 position, Quaternion rotation)
	{
		if (CameraController.sm_Instance == null)
		{
			return;
		}
		for (int i = 0; i < CameraController.sm_Instance.AllCameras.Count; i++)
		{
			CameraController.sm_Instance.AllCameras[i].Set(position, rotation, false);
		}
	}

	// Token: 0x06000B1F RID: 2847 RVA: 0x0007FB54 File Offset: 0x0007DD54
	public void ViewModeChanged()
	{
		if (this.BVCamera != null)
		{
			this.BVCamera.gameObject.SetActive(true);
			if (this.CurrentGameCamera == null || this.CurrentGameCamera != this.BVCamera)
			{
				this.CurrentGameCamera = this.BVCamera;
				this.CurrentGameCamera.ViewModeChanged();
			}
			if (Application.isPlaying)
			{
				Camera cullingCamera = VisibilityDetector.GetCullingCamera("BattleView");
				if (cullingCamera == null || cullingCamera != this.CurrentGameCamera.Camera)
				{
					VisibilityDetector.SetCamera(this.CurrentGameCamera.Camera, "BattleView");
					return;
				}
			}
		}
		else
		{
			bool flag = ViewMode.IsPoliticalView();
			if (this.WVCamera != null)
			{
				this.WVCamera.gameObject.SetActive(!flag);
			}
			if (this.PVCamera != null)
			{
				this.PVCamera.gameObject.SetActive(flag);
			}
			GameCamera currentGameCamera = this.CurrentGameCamera;
			this.CurrentGameCamera = (flag ? this.PVCamera : this.WVCamera);
			if (this.CurrentGameCamera != currentGameCamera && this.CurrentGameCamera != null)
			{
				this.CurrentGameCamera.ViewModeChanged();
			}
			if (Application.isPlaying && this.CurrentGameCamera != null && !flag)
			{
				VisibilityDetector.SetCamera(this.CurrentGameCamera.Camera, "WorldView");
			}
		}
	}

	// Token: 0x06000B20 RID: 2848 RVA: 0x0007FCC0 File Offset: 0x0007DEC0
	public void EnableCameraDepthMode(DepthTextureMode mode)
	{
		foreach (GameCamera gameCamera in this.AllCameras)
		{
			if (gameCamera.Camera != null)
			{
				gameCamera.Camera.depthTextureMode |= mode;
			}
		}
	}

	// Token: 0x06000B21 RID: 2849 RVA: 0x0007FD30 File Offset: 0x0007DF30
	public void DisableCameraDepthMode(DepthTextureMode mode)
	{
		this.DoAfterFrames(2, delegate
		{
			foreach (GameCamera gameCamera in this.AllCameras)
			{
				if (gameCamera.Camera != null)
				{
					gameCamera.Camera.depthTextureMode &= ~mode;
				}
			}
		});
	}

	// Token: 0x06000B22 RID: 2850 RVA: 0x0007FD64 File Offset: 0x0007DF64
	public void DoAfterFrames(int frames_count, Action action)
	{
		base.StartCoroutine(this.DoAfterFramesIE(frames_count, action));
	}

	// Token: 0x06000B23 RID: 2851 RVA: 0x0007FD75 File Offset: 0x0007DF75
	public IEnumerator DoAfterFramesIE(int frames_count, Action action)
	{
		for (;;)
		{
			int num = frames_count;
			frames_count = num - 1;
			if (num <= 0)
			{
				break;
			}
			yield return null;
		}
		if (action != null)
		{
			action();
		}
		yield break;
	}

	// Token: 0x04000890 RID: 2192
	private static CameraController sm_Instance;

	// Token: 0x04000892 RID: 2194
	[NonSerialized]
	public GameCamera WVCamera;

	// Token: 0x04000893 RID: 2195
	[NonSerialized]
	public GameCamera PVCamera;

	// Token: 0x04000894 RID: 2196
	[NonSerialized]
	public GameCamera BVCamera;

	// Token: 0x04000895 RID: 2197
	[NonSerialized]
	public List<GameCamera> AllCameras = new List<GameCamera>();

	// Token: 0x04000896 RID: 2198
	private bool initted;

	// Token: 0x04000897 RID: 2199
	public bool autoSync;
}
