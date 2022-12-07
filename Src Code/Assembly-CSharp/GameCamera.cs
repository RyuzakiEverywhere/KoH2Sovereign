using System;
using System.Collections.Generic;
using System.Reflection;
using FMODUnity;
using Logic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020000E0 RID: 224
[RequireComponent(typeof(Camera))]
public class GameCamera : MonoBehaviour
{
	// Token: 0x06000B26 RID: 2854 RVA: 0x0007FDD8 File Offset: 0x0007DFD8
	static GameCamera()
	{
		Type typeFromHandle = typeof(ICameraControllScheme);
		foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
		{
			if (typeFromHandle.IsAssignableFrom(type) && type.IsPublic && !type.IsInterface && !type.IsAbstract)
			{
				GameCamera.sm_AvaliableTypes.Add(type);
			}
		}
	}

	// Token: 0x17000087 RID: 135
	// (get) Token: 0x06000B27 RID: 2855 RVA: 0x0007FE43 File Offset: 0x0007E043
	public CameraSettings Settings
	{
		get
		{
			return this.cameraSettings;
		}
	}

	// Token: 0x17000088 RID: 136
	// (get) Token: 0x06000B28 RID: 2856 RVA: 0x0007FE4B File Offset: 0x0007E04B
	public Camera Camera
	{
		get
		{
			if (this.m_Camera == null)
			{
				this.Init();
			}
			return this.m_Camera;
		}
	}

	// Token: 0x17000089 RID: 137
	// (get) Token: 0x06000B29 RID: 2857 RVA: 0x0007FE67 File Offset: 0x0007E067
	public StudioListener FmodAudioListener
	{
		get
		{
			return this.m_fmodAudioListener;
		}
	}

	// Token: 0x1700008A RID: 138
	// (get) Token: 0x06000B2A RID: 2858 RVA: 0x0007FE6F File Offset: 0x0007E06F
	// (set) Token: 0x06000B2B RID: 2859 RVA: 0x0007FE77 File Offset: 0x0007E077
	public bool EdgeScroll { get; set; }

	// Token: 0x06000B2C RID: 2860 RVA: 0x0007FE80 File Offset: 0x0007E080
	private void OnValidate()
	{
		if (this.supportedModes.Count == 0)
		{
			foreach (Type type in GameCamera.sm_AvaliableTypes)
			{
				this.supportedModes.Add(type.ToString());
			}
		}
	}

	// Token: 0x06000B2D RID: 2861 RVA: 0x0007FEEC File Offset: 0x0007E0EC
	private void Init()
	{
		if (this.m_Camera != null)
		{
			return;
		}
		this.f_planes = new float4[6];
		this.m_Camera = base.GetComponent<Camera>();
		this.m_Camera.clearStencilAfterLightingPass = true;
		this.m_Camera.opaqueSortMode = OpaqueSortMode.NoDistanceSort;
		foreach (Type type in GameCamera.sm_AvaliableTypes)
		{
			if (this.supportedModes.Contains(type.ToString()))
			{
				this.availableSchemes.Add(type.ToString(), Activator.CreateInstance(type, new object[]
				{
					this
				}) as ICameraControllScheme);
			}
		}
		this.currentControllScheme = this.availableSchemes[this.supportedModes[0]];
		this.CurrentControlMode = this.currentControllScheme.ToString();
		this.m_Camera.clearStencilAfterLightingPass = true;
		this.EdgeScroll = UserSettings.EdgeScroll;
		UserSettings.OnSettingChange += this.OnSettingChange;
		this.currentControllScheme.RecalcCameraSettings();
		GameObject gameObject = global::Common.FindChildByName(base.transform.parent.gameObject, "_AudioListener", true, true);
		if (gameObject != null)
		{
			this.m_fmodAudioListener = gameObject.GetComponent<StudioListener>();
		}
	}

	// Token: 0x06000B2E RID: 2862 RVA: 0x00080044 File Offset: 0x0007E244
	private void Awake()
	{
		this.Init();
	}

	// Token: 0x06000B2F RID: 2863 RVA: 0x0008004C File Offset: 0x0007E24C
	private void OnDestroy()
	{
		UserSettings.OnSettingChange -= this.OnSettingChange;
	}

	// Token: 0x06000B30 RID: 2864 RVA: 0x0008005F File Offset: 0x0007E25F
	private void OnSettingChange()
	{
		this.EdgeScroll = UserSettings.EdgeScroll;
	}

	// Token: 0x06000B31 RID: 2865 RVA: 0x0008006C File Offset: 0x0007E26C
	public void ViewModeChanged()
	{
		if (this.currentControllScheme != null)
		{
			this.currentControllScheme.RecalcCameraSettings();
			this.currentControllScheme.Reset();
		}
	}

	// Token: 0x06000B32 RID: 2866 RVA: 0x0008008C File Offset: 0x0007E28C
	private void OnEnable()
	{
		this.Init();
		this.org_dists = this.Settings.dist;
		this.org_pitches = this.Settings.pitch;
		if (this.syncWith != null && this.currentControllScheme != null)
		{
			this.currentControllScheme.LookAt(this.syncWith.GetLookAtPoint());
		}
	}

	// Token: 0x06000B33 RID: 2867 RVA: 0x000800ED File Offset: 0x0007E2ED
	private void Start()
	{
		this.currentControllScheme.RecalcCameraSettings();
	}

	// Token: 0x06000B34 RID: 2868 RVA: 0x000800FC File Offset: 0x0007E2FC
	private void CalcFrustrum()
	{
		this.frustum_planes = GeometryUtility.CalculateFrustumPlanes(this.Camera);
		for (int i = 0; i < 6; i++)
		{
			Vector3 normal = this.frustum_planes[i].normal;
			float distance = this.frustum_planes[i].distance;
			float4 @float;
			@float.x = normal.x;
			@float.y = normal.y;
			@float.z = normal.z;
			@float.w = distance;
			this.f_planes[i] = @float;
		}
	}

	// Token: 0x06000B35 RID: 2869 RVA: 0x00080188 File Offset: 0x0007E388
	private Vector3 GetDefaultLookAtPt()
	{
		Vector3 vector = this.ScreenToGroundPlane(new Vector2((float)(Screen.width / 2), (float)(Screen.height / 2)), Vector3.zero, 0f);
		if (vector == Vector3.zero)
		{
			vector = this.GetTerrainSize() / 2f;
		}
		return vector;
	}

	// Token: 0x06000B36 RID: 2870 RVA: 0x000801DC File Offset: 0x0007E3DC
	private void Update()
	{
		if (this.m_Locked)
		{
			return;
		}
		if (this.currentControllScheme == null)
		{
			return;
		}
		this.CalcFrustrum();
		if (UICommon.GetKeyUp(KeyCode.F, UICommon.ModifierKey.None, UICommon.ModifierKey.None) && Game.CheckCheatLevel(Game.CheatLevel.Low, "Change camera type", true))
		{
			ICameraControllScheme nextScheme = this.GetNextScheme();
			this.SetControlScheme(nextScheme);
		}
		if (!this.m_LockedUserInput)
		{
			this.currentControllScheme.UpdateInput(this.m_Camera.transform, this.cameraSettings);
		}
		if (this.sync)
		{
			this.m_Camera.transform.position = this.syncWith.transform.position;
			this.LookAt(this.syncWith.currentControllScheme.GetLookAtPoint(), false);
			this.sync = false;
		}
		this.currentControllScheme.UpdateCamera(this.m_Camera.transform, this.cameraSettings);
		this.ApplyInterpolators();
		this.UpdateAudioListeners();
	}

	// Token: 0x06000B37 RID: 2871 RVA: 0x000802BA File Offset: 0x0007E4BA
	private void SetControlScheme(ICameraControllScheme next)
	{
		if (next != this.currentControllScheme)
		{
			next.LookAt(this.currentControllScheme.GetLookAtPoint());
			this.currentControllScheme = next;
			this.CurrentControlMode = this.currentControllScheme.ToString();
		}
	}

	// Token: 0x06000B38 RID: 2872 RVA: 0x000802F0 File Offset: 0x0007E4F0
	public void SetScheme(string name)
	{
		if (!this.supportedModes.Contains(name))
		{
			return;
		}
		ICameraControllScheme cameraControllScheme = this.availableSchemes[name];
		if (cameraControllScheme != this.currentControllScheme)
		{
			cameraControllScheme.LookAt(this.currentControllScheme.GetLookAtPoint());
			this.currentControllScheme = cameraControllScheme;
			this.CurrentControlMode = this.currentControllScheme.ToString();
		}
	}

	// Token: 0x06000B39 RID: 2873 RVA: 0x0008034C File Offset: 0x0007E54C
	public void LookAt(Vector3 pt, bool force_update = false)
	{
		if (this.m_Camera == null)
		{
			this.Init();
		}
		this.currentControllScheme.LookAt(pt);
		if (force_update)
		{
			this.currentControllScheme.UpdateCamera(this.m_Camera.transform, this.cameraSettings);
		}
	}

	// Token: 0x06000B3A RID: 2874 RVA: 0x00080398 File Offset: 0x0007E598
	public void Zoom(float zoom, bool force_update = false)
	{
		if (this.m_Camera == null)
		{
			this.Init();
		}
		this.currentControllScheme.Zoom(zoom);
		if (force_update)
		{
			this.currentControllScheme.UpdateCamera(this.m_Camera.transform, this.cameraSettings);
		}
	}

	// Token: 0x06000B3B RID: 2875 RVA: 0x000803E4 File Offset: 0x0007E5E4
	public void Yaw(float yaw, bool force_update = false)
	{
		if (this.m_Camera == null)
		{
			this.Init();
		}
		this.currentControllScheme.Yaw(yaw);
		if (force_update)
		{
			this.currentControllScheme.UpdateCamera(this.m_Camera.transform, this.cameraSettings);
		}
	}

	// Token: 0x06000B3C RID: 2876 RVA: 0x00080430 File Offset: 0x0007E630
	public void Set(Vector3 position, Quaternion rotation, bool force_update = false)
	{
		if (this.m_Camera == null)
		{
			this.Init();
		}
		this.currentControllScheme.Set(position, rotation);
		if (force_update)
		{
			this.currentControllScheme.UpdateCamera(this.m_Camera.transform, this.cameraSettings);
		}
	}

	// Token: 0x06000B3D RID: 2877 RVA: 0x0008047D File Offset: 0x0007E67D
	public Vector3 GetLookAtPoint()
	{
		if (this.currentControllScheme != null)
		{
			return this.currentControllScheme.GetLookAtPoint();
		}
		return new Vector3(0f, 0f, 0f);
	}

	// Token: 0x06000B3E RID: 2878 RVA: 0x0008047D File Offset: 0x0007E67D
	public Vector3 GetScreenToWorldPoint(Vector2 screenPoint)
	{
		if (this.currentControllScheme != null)
		{
			return this.currentControllScheme.GetLookAtPoint();
		}
		return new Vector3(0f, 0f, 0f);
	}

	// Token: 0x06000B3F RID: 2879 RVA: 0x000804A7 File Offset: 0x0007E6A7
	public KeyValuePair<Vector3, Quaternion> CalcPositionAndRotation(Vector3 lookAtPoint, float zoom)
	{
		if (this.currentControllScheme != null)
		{
			return this.currentControllScheme.CalcPositionAndRotations(lookAtPoint, zoom);
		}
		return new KeyValuePair<Vector3, Quaternion>(new Vector3(0f, 0f, 0f), Quaternion.identity);
	}

	// Token: 0x06000B40 RID: 2880 RVA: 0x000804E0 File Offset: 0x0007E6E0
	public void UpdateAudioListeners()
	{
		if (this.m_fmodAudioListener != null)
		{
			ValueTuple<Vector3, Quaternion> audioListenerPositionAndRotation = this.currentControllScheme.GetAudioListenerPositionAndRotation();
			this.m_fmodAudioListener.gameObject.transform.position = audioListenerPositionAndRotation.Item1;
			this.m_fmodAudioListener.gameObject.transform.rotation = audioListenerPositionAndRotation.Item2;
		}
	}

	// Token: 0x06000B41 RID: 2881 RVA: 0x0008053D File Offset: 0x0007E73D
	public void Lock(bool locked)
	{
		this.m_Locked = locked;
	}

	// Token: 0x06000B42 RID: 2882 RVA: 0x00080546 File Offset: 0x0007E746
	public void LockUserInput(bool locked)
	{
		this.m_LockedUserInput = locked;
	}

	// Token: 0x06000B43 RID: 2883 RVA: 0x000023FD File Offset: 0x000005FD
	public void Enabled(bool enabled)
	{
	}

	// Token: 0x06000B44 RID: 2884 RVA: 0x00080550 File Offset: 0x0007E750
	private ICameraControllScheme GetNextScheme()
	{
		if (this.supportedModes.Count <= 1)
		{
			return this.currentControllScheme;
		}
		int num = this.supportedModes.FindIndex((string x) => x == this.currentControllScheme.GetType().ToString());
		if (num == -1)
		{
			return this.currentControllScheme;
		}
		num = ((num < this.supportedModes.Count - 1) ? (num + 1) : 0);
		return this.availableSchemes[this.supportedModes[num]];
	}

	// Token: 0x06000B45 RID: 2885 RVA: 0x000805C4 File Offset: 0x0007E7C4
	public void ApplyInterpolators()
	{
		foreach (CameraInterpolator cameraInterpolator in this.CameraInterpolators)
		{
			if (cameraInterpolator.enabled)
			{
				float num = global::Common.map(this.currentControllScheme.GetDistanceToAimPoint(), this.cameraSettings.dist[0], this.cameraSettings.dist[1], cameraInterpolator.min, cameraInterpolator.max, false);
				PropertyChanger.SetVar(PropertyChanger.GetObj(cameraInterpolator.obj, cameraInterpolator.obj_name), cameraInterpolator.component, cameraInterpolator.variable, num);
			}
		}
	}

	// Token: 0x06000B46 RID: 2886 RVA: 0x00080684 File Offset: 0x0007E884
	[ConsoleMethod("es", "enable/disable edge scroll")]
	public void SetScrollSpeed(int es)
	{
		this.EdgeScroll = (es != 0);
	}

	// Token: 0x06000B47 RID: 2887 RVA: 0x00080690 File Offset: 0x0007E890
	[ConsoleMethod("ss", "set scroll speed")]
	public void SetScrollSpeed(float speed)
	{
		this.cameraSettings.scrollSpeed = speed;
	}

	// Token: 0x06000B48 RID: 2888 RVA: 0x000023FD File Offset: 0x000005FD
	[ConsoleMethod("yaw", "set camera yaw")]
	public void SetYaw(float yaw)
	{
	}

	// Token: 0x06000B49 RID: 2889 RVA: 0x000023FD File Offset: 0x000005FD
	[ConsoleMethod("orbit", "orbit camera")]
	public void Orbit(float speed)
	{
	}

	// Token: 0x06000B4A RID: 2890 RVA: 0x000806A0 File Offset: 0x0007E8A0
	public static Vector3 ScreenToTerrain(Vector2 pts, Camera cam)
	{
		Ray ray = cam.ScreenPointToRay(pts);
		int layerMask = 1 << LayerMask.NameToLayer("Terrain");
		RaycastHit raycastHit;
		if (Physics.Raycast(ray, out raycastHit, float.PositiveInfinity, layerMask))
		{
			return raycastHit.point;
		}
		return Vector3.zero;
	}

	// Token: 0x06000B4B RID: 2891 RVA: 0x000806E8 File Offset: 0x0007E8E8
	public Vector3 TransormToGroundPlane()
	{
		Vector3 position = this.m_Camera.transform.position;
		Ray ray = new Ray(position, Vector3.down);
		Plane plane = new Plane(Vector3.up, -this.cameraSettings.lookAtHeight);
		float distance = 0f;
		if (!plane.Raycast(ray, out distance))
		{
			return Vector3.zero;
		}
		return ray.GetPoint(distance);
	}

	// Token: 0x06000B4C RID: 2892 RVA: 0x0008074C File Offset: 0x0007E94C
	public Vector3 ScreenToGroundPlane(Vector2 pts, Vector3 ptLookat, float offs = 0f)
	{
		Vector3 position = this.m_Camera.transform.position;
		if (ptLookat != Vector3.zero)
		{
			Vector3 position2 = position;
			position2.y = ptLookat.y;
			this.m_Camera.transform.position = position2;
		}
		Ray ray = this.m_Camera.ScreenPointToRay(pts);
		this.m_Camera.transform.position = position;
		Plane plane = new Plane(Vector3.up, -(this.cameraSettings.lookAtHeight + offs));
		float distance = 0f;
		if (!plane.Raycast(ray, out distance))
		{
			return Vector3.zero;
		}
		return ray.GetPoint(distance);
	}

	// Token: 0x06000B4D RID: 2893 RVA: 0x000807F8 File Offset: 0x0007E9F8
	public Vector3 GetTerrainSize()
	{
		if (this.vTerrainSize != null)
		{
			return this.vTerrainSize.Value;
		}
		MapData mapData = MapData.Get();
		if (mapData != null)
		{
			this.vTerrainSize = new Vector3?(mapData.GetTerrainBounds().size);
			return this.vTerrainSize.Value;
		}
		this.vTerrainSize = new Vector3?(new Vector3(0f, 0f, 0f));
		for (int i = 0; i < Terrain.activeTerrains.Length; i++)
		{
			Terrain terrain = Terrain.activeTerrains[i];
			Vector3 rhs = terrain.GetPosition() + terrain.terrainData.size;
			this.vTerrainSize = new Vector3?(Vector3.Max(this.vTerrainSize.Value, rhs));
		}
		return this.vTerrainSize.Value;
	}

	// Token: 0x040008A0 RID: 2208
	public static List<Type> sm_AvaliableTypes = new List<Type>();

	// Token: 0x040008A1 RID: 2209
	private Plane[] frustum_planes = new Plane[6];

	// Token: 0x040008A2 RID: 2210
	public float4[] f_planes;

	// Token: 0x040008A3 RID: 2211
	public string CurrentControlMode;

	// Token: 0x040008A4 RID: 2212
	public ICameraControllScheme currentControllScheme;

	// Token: 0x040008A5 RID: 2213
	private Dictionary<string, ICameraControllScheme> availableSchemes = new Dictionary<string, ICameraControllScheme>();

	// Token: 0x040008A6 RID: 2214
	[SerializeField]
	private CameraSettings cameraSettings;

	// Token: 0x040008A7 RID: 2215
	[NonSerialized]
	public Vector2 org_dists;

	// Token: 0x040008A8 RID: 2216
	[NonSerialized]
	public Vector2 org_pitches;

	// Token: 0x040008A9 RID: 2217
	public List<CameraInterpolator> CameraInterpolators;

	// Token: 0x040008AA RID: 2218
	private Camera m_Camera;

	// Token: 0x040008AB RID: 2219
	private StudioListener m_fmodAudioListener;

	// Token: 0x040008AD RID: 2221
	public GameCamera syncWith;

	// Token: 0x040008AE RID: 2222
	private bool sync;

	// Token: 0x040008AF RID: 2223
	public bool focus_mouse_zoom;

	// Token: 0x040008B0 RID: 2224
	[HideInInspector]
	public List<string> supportedModes = new List<string>();

	// Token: 0x040008B1 RID: 2225
	private bool m_Locked;

	// Token: 0x040008B2 RID: 2226
	private bool m_LockedUserInput;

	// Token: 0x040008B3 RID: 2227
	[NonSerialized]
	public bool syncGameCam;

	// Token: 0x040008B4 RID: 2228
	private Vector3? vTerrainSize;
}
