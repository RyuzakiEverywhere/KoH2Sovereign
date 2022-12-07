using System;
using Logic;
using UnityEngine;

// Token: 0x0200006D RID: 109
public class CameraPaths : MonoBehaviour
{
	// Token: 0x06000321 RID: 801 RVA: 0x00029C45 File Offset: 0x00027E45
	private void OnEnable()
	{
		CameraPaths.instance = this;
		if (Application.isPlaying)
		{
			CameraPath.use_scene_view = false;
		}
	}

	// Token: 0x06000322 RID: 802 RVA: 0x00029C5A File Offset: 0x00027E5A
	private void OnDisable()
	{
		if (CameraPaths.instance == this)
		{
			CameraPaths.instance = null;
		}
	}

	// Token: 0x06000323 RID: 803 RVA: 0x00029C6F File Offset: 0x00027E6F
	public static CameraPaths Get(bool create_if_needed)
	{
		if (CameraPaths.instance != null)
		{
			return CameraPaths.instance;
		}
		if (!create_if_needed)
		{
			return null;
		}
		new GameObject("Camera Paths", new Type[]
		{
			typeof(CameraPaths)
		});
		return CameraPaths.instance;
	}

	// Token: 0x06000324 RID: 804 RVA: 0x00029CAC File Offset: 0x00027EAC
	private void Start()
	{
		CameraPath.LoadAll(base.gameObject);
		IMGUIHandler.Get();
	}

	// Token: 0x06000325 RID: 805 RVA: 0x00029CC0 File Offset: 0x00027EC0
	private void Update()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "camera paths", false))
		{
			return;
		}
		if (UICommon.GetKeyDown(KeyCode.M, UICommon.ModifierKey.Ctrl, UICommon.ModifierKey.None))
		{
			this.show = !this.show;
			if (this.show)
			{
				CameraPath.use_scene_view = false;
			}
			this.confirmAction = null;
		}
		if (UICommon.GetKey(KeyCode.LeftShift, false))
		{
			CameraPath cameraPath = CameraPath.First;
			while (cameraPath != null)
			{
				if (UICommon.GetKeyUp(cameraPath.hotkey, UICommon.ModifierKey.Shift, UICommon.ModifierKey.None))
				{
					this.cp = cameraPath;
					this.cp.t = 0f;
					this.cp.Play(!this.cp.IsPlaying(), true, true);
				}
				cameraPath = cameraPath.Next;
			}
		}
	}

	// Token: 0x06000326 RID: 806 RVA: 0x00029D74 File Offset: 0x00027F74
	private void OnGUI()
	{
		if (!this.show)
		{
			return;
		}
		Cursor.visible = true;
		Color color = GUI.color;
		CameraPath first = CameraPath.First;
		int num = 0;
		CameraPath cameraPath = first;
		while (cameraPath != null)
		{
			GUI.color = ((this.cp == cameraPath) ? Color.green : color);
			if (GUI.Button(new Rect(25f, (float)(num * 35 + 160), 100f, 30f), cameraPath.gameObject.name))
			{
				this.cp = cameraPath;
			}
			num++;
			cameraPath = cameraPath.Next;
		}
		GUI.color = color;
		this.newName = GUI.TextArea(new Rect(25f, (float)(num * 35 + 185), 100f, 30f), this.newName);
		if (!string.IsNullOrEmpty(this.newName) && !this.newName.Contains("\n") && GUI.Button(new Rect(25f, (float)(num * 35 + 215), 100f, 30f), "Add Path"))
		{
			GameObject gameObject = new GameObject(this.newName, new Type[]
			{
				typeof(CameraPath)
			});
			gameObject.transform.SetParent(base.transform, false);
			CameraPath component = gameObject.GetComponent<CameraPath>();
			this.cp = component;
		}
		IMGUIHandler.AddRect(new Rect(25f, 160f, 100f, (float)(num * 35 + 85)));
		if (!string.IsNullOrEmpty(this.confirmAction))
		{
			this.confirmWindow = GUI.Window(0, this.confirmWindow, new GUI.WindowFunction(this.ConfirmWindow), "Confirm?");
			IMGUIHandler.AddRect(this.confirmWindow);
			return;
		}
		if (this.cp != null)
		{
			this.pathEditWindow = GUI.Window(0, this.pathEditWindow, new GUI.WindowFunction(this.PathEditWindow), this.cp.gameObject.name + " options");
			IMGUIHandler.AddRect(this.pathEditWindow);
		}
	}

	// Token: 0x06000327 RID: 807 RVA: 0x00029F74 File Offset: 0x00028174
	private void PathEditWindow(int windowID)
	{
		if (GUI.Button(new Rect(420f, 0f, 20f, 20f), "X"))
		{
			this.cp = null;
			this.confirmAction = null;
			return;
		}
		Color color = GUI.color;
		this.cp.CalcHotKey();
		KeyCode hotkey = this.cp.hotkey;
		GUI.Label(new Rect(10f, 25f, 400f, 30f), "Play Hotkey: " + ((hotkey != KeyCode.None) ? ("Shift-" + hotkey.ToString()) : "none"));
		GUI.Label(new Rect(10f, 45f, 400f, 30f), string.Concat(new object[]
		{
			"Duration: ",
			(int)this.cp.duration,
			" / ",
			(int)CameraPath.TotalDuration(),
			" sec"
		}));
		GUI.Label(new Rect(10f, 70f, 100f, 30f), "Path progress:");
		GUI.color = (this.cp.IsPlaying() ? Color.green : color);
		if (GUI.Button(new Rect(220f, 35f, 180f, 30f), this.cp.IsPlaying() ? "Pause" : "Play"))
		{
			this.cp.Play(!this.cp.IsPlaying(), true, true);
		}
		float num = GUI.HorizontalSlider(new Rect(10f, 90f, 430f, 30f), this.cp.t, -0.1f, 1.1f);
		if (num != this.cp.t)
		{
			this.cp.MoveTo(num);
		}
		GUI.color = color;
		GUI.Label(new Rect(10f, 110f, 200f, 30f), "Waypoint:");
		if (this.cp.waypoints.Count > 0)
		{
			int num2 = (int)GUI.HorizontalSlider(new Rect(10f, 130f, 130f, 30f), (float)this.cp.idx, 0f, (float)(this.cp.waypoints.Count - 1));
			if (num2 != this.cp.idx)
			{
				this.cp.MoveToIdx(num2, 0f, true);
			}
			GUI.Label(new Rect(150f, 140f, 30f, 30f), num2.ToString() + "/" + (this.cp.waypoints.Count - 1));
		}
		GUI.Label(new Rect(200f, 110f, 200f, 30f), "Waypoint progress");
		if (this.cp.idx + 1 < this.cp.waypoints.Count)
		{
			float num3 = GUI.HorizontalSlider(new Rect(200f, 130f, 240f, 30f), this.cp.dt, 0f, 1f);
			if (num3 != this.cp.dt)
			{
				this.cp.MoveToIdx(this.cp.idx, num3, true);
			}
		}
		if (this.cp.waypoints.Count > 0 && GUI.Button(new Rect(10f, 160f, 100f, 30f), "Delete Point"))
		{
			this.cp.Delete();
		}
		string text;
		if (this.cp.waypoints.Count < 1 || (this.cp.waypoints.Count == 1 && this.cp.t > 0f) || this.cp.t > 1f)
		{
			text = "Add Point";
		}
		else if (this.cp.t < 0f || (this.cp.dt > 0f && this.cp.dt < 1f))
		{
			text = "Insert Point";
		}
		else
		{
			text = "Replace Point";
		}
		if (GUI.Button(new Rect(120f, 160f, 100f, 30f), text))
		{
			this.cp.Record();
		}
		GUI.Label(new Rect(10f, 200f, 40f, 30f), "Speed");
		float num4 = GUI.HorizontalSlider(new Rect(140f, 210f, 300f, 30f), this.cp.speed_mul, 0.1f, 10f);
		if (num4 != this.cp.speed_mul)
		{
			this.cp.speed_mul = num4;
		}
		if (float.TryParse(GUI.TextArea(new Rect(60f, 200f, 50f, 25f), this.cp.speed_mul.ToString("0.00")), out num4))
		{
			this.cp.speed_mul = num4;
		}
		CameraPath.h_smooth = GUI.Toggle(new Rect(10f, 230f, 200f, 25f), CameraPath.h_smooth, "Smooth XZ");
		CameraPath.v_smooth = GUI.Toggle(new Rect(180f, 230f, 200f, 25f), CameraPath.v_smooth, "Smooth Y");
		if (this.gameCamera == null)
		{
			Camera main = Camera.main;
			this.gameCamera = global::Common.GetComponent<GameCamera>((main != null) ? main.gameObject : null, null);
		}
		if (this.gameCamera != null)
		{
			color = GUI.color;
			string text2 = this.gameCamera.currentControllScheme.ToString();
			GUI.Label(new Rect(10f, 260f, 200f, 30f), "[F] Camera mode: " + text2);
			for (int i = 0; i < this.gameCamera.supportedModes.Count; i++)
			{
				string text3 = this.gameCamera.supportedModes[i];
				if (!string.IsNullOrEmpty(text3))
				{
					GUI.color = ((text3 == text2) ? Color.green : color);
					if (GUI.Button(new Rect((float)(10 + i * 110), 280f, 100f, 30f), text3))
					{
						this.SetCameraMode(text3, true);
					}
				}
			}
			GUI.color = color;
		}
		if (GUI.Button(new Rect(10f, 320f, 100f, 30f), "Delete Path"))
		{
			this.confirmAction = "Delete Path";
		}
		if (this.cp.waypoints.Count > 0 && GUI.Button(new Rect(120f, 320f, 100f, 30f), "Clear Path"))
		{
			this.confirmAction = "Clear Path";
		}
		if (GUI.Button(new Rect(230f, 320f, 100f, 30f), "Save All"))
		{
			this.confirmAction = "Save All";
		}
		if (GUI.Button(new Rect(340f, 320f, 100f, 30f), "Revert All"))
		{
			this.confirmAction = "Revert All";
		}
	}

	// Token: 0x06000328 RID: 808 RVA: 0x0002A6E0 File Offset: 0x000288E0
	private void SetCameraMode(string mode, bool keepWorldPosition = true)
	{
		if (this.gameCamera == null)
		{
			return;
		}
		Vector3 position = this.gameCamera.transform.position;
		Quaternion rotation = this.gameCamera.transform.rotation;
		this.gameCamera.SetScheme(mode);
		if (keepWorldPosition)
		{
			ICameraControllScheme currentControllScheme = this.gameCamera.currentControllScheme;
			if (currentControllScheme == null)
			{
				return;
			}
			currentControllScheme.Set(position, rotation);
		}
	}

	// Token: 0x06000329 RID: 809 RVA: 0x0002A744 File Offset: 0x00028944
	private void ConfirmWindow(int windowID)
	{
		if (GUI.Button(new Rect(30f, 30f, 100f, 30f), this.confirmAction))
		{
			string a = this.confirmAction;
			if (!(a == "Delete Path"))
			{
				if (!(a == "Clear Path"))
				{
					if (!(a == "Save All"))
					{
						if (a == "Revert All")
						{
							CameraPath.LoadAll(null);
						}
					}
					else
					{
						CameraPath.SaveAll();
					}
				}
				else
				{
					CameraPath cameraPath = this.cp;
					if (cameraPath != null)
					{
						cameraPath.Clear();
					}
				}
			}
			else if (this.cp != null)
			{
				UnityEngine.Object.Destroy(this.cp.gameObject);
			}
			this.confirmAction = null;
		}
		if (GUI.Button(new Rect(170f, 30f, 100f, 30f), "Cancel"))
		{
			this.confirmAction = null;
		}
	}

	// Token: 0x04000401 RID: 1025
	private CameraPath cp;

	// Token: 0x04000402 RID: 1026
	private Rect pathEditWindow = new Rect(160f, 160f, 450f, 360f);

	// Token: 0x04000403 RID: 1027
	private Rect confirmWindow = new Rect(200f, 400f, 300f, 70f);

	// Token: 0x04000404 RID: 1028
	public bool show;

	// Token: 0x04000405 RID: 1029
	public string confirmAction;

	// Token: 0x04000406 RID: 1030
	public static CameraPaths instance;

	// Token: 0x04000407 RID: 1031
	private string newName = "newName";

	// Token: 0x04000408 RID: 1032
	private GameCamera gameCamera;
}
