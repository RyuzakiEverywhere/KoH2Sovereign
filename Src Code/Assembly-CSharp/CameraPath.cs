using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Logic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x0200006C RID: 108
[ExecuteInEditMode]
public class CameraPath : MonoBehaviour
{
	// Token: 0x1700002A RID: 42
	// (get) Token: 0x060002FA RID: 762 RVA: 0x000287A8 File Offset: 0x000269A8
	public static CameraPath First
	{
		get
		{
			return CameraPath.first;
		}
	}

	// Token: 0x1700002B RID: 43
	// (get) Token: 0x060002FB RID: 763 RVA: 0x000287AF File Offset: 0x000269AF
	public static CameraPath Last
	{
		get
		{
			return CameraPath.last;
		}
	}

	// Token: 0x1700002C RID: 44
	// (get) Token: 0x060002FC RID: 764 RVA: 0x000287B6 File Offset: 0x000269B6
	public CameraPath Prev
	{
		get
		{
			return this.prev;
		}
	}

	// Token: 0x1700002D RID: 45
	// (get) Token: 0x060002FD RID: 765 RVA: 0x000287BE File Offset: 0x000269BE
	public CameraPath Next
	{
		get
		{
			return this.next;
		}
	}

	// Token: 0x1700002E RID: 46
	// (get) Token: 0x060002FE RID: 766 RVA: 0x000287C6 File Offset: 0x000269C6
	public float duration
	{
		get
		{
			return this.len / (CameraPath.global_speed * this.speed_mul);
		}
	}

	// Token: 0x060002FF RID: 767 RVA: 0x000287DC File Offset: 0x000269DC
	public static float TotalDuration()
	{
		float num = 0f;
		CameraPath cameraPath = CameraPath.first;
		while (cameraPath != null)
		{
			num += cameraPath.duration;
			cameraPath = cameraPath.next;
		}
		return num;
	}

	// Token: 0x06000300 RID: 768 RVA: 0x00028811 File Offset: 0x00026A11
	private void OnEnable()
	{
		this.CalcHotKey();
		this.Recalc();
		this.Register();
	}

	// Token: 0x06000301 RID: 769 RVA: 0x00028825 File Offset: 0x00026A25
	private void OnDisable()
	{
		this.Play(false, true, true);
		this.Unregister();
	}

	// Token: 0x06000302 RID: 770 RVA: 0x00028836 File Offset: 0x00026A36
	private Vector3 W2L(Vector3 pos)
	{
		return base.transform.InverseTransformPoint(pos);
	}

	// Token: 0x06000303 RID: 771 RVA: 0x00028844 File Offset: 0x00026A44
	private Vector3 L2W(Vector3 pos)
	{
		return base.transform.TransformPoint(pos);
	}

	// Token: 0x06000304 RID: 772 RVA: 0x00028852 File Offset: 0x00026A52
	private Quaternion W2L(Quaternion rot)
	{
		return Quaternion.Inverse(base.transform.rotation) * rot;
	}

	// Token: 0x06000305 RID: 773 RVA: 0x0002886A File Offset: 0x00026A6A
	private Quaternion L2W(Quaternion rot)
	{
		return base.transform.rotation * rot;
	}

	// Token: 0x06000306 RID: 774 RVA: 0x00028880 File Offset: 0x00026A80
	public void SetPivot(float t)
	{
		this.pivot = t;
		if (this.waypoints.Count < 1)
		{
			return;
		}
		int num;
		float num2;
		this.Resolve(t, out num, out num2);
		Vector3 position = this.CalcPos(num, num2, CameraPath.h_smooth, CameraPath.v_smooth);
		for (int i = 0; i < this.waypoints.Count; i++)
		{
			CameraPath.Waypoint waypoint = this.waypoints[i];
			waypoint.pos = this.L2W(waypoint.pos);
			this.waypoints[i] = waypoint;
		}
		base.transform.position = position;
		for (int j = 0; j < this.waypoints.Count; j++)
		{
			CameraPath.Waypoint waypoint2 = this.waypoints[j];
			waypoint2.pos = this.W2L(waypoint2.pos);
			this.waypoints[j] = waypoint2;
		}
	}

	// Token: 0x06000307 RID: 775 RVA: 0x00028960 File Offset: 0x00026B60
	public void Update()
	{
		if (!Application.isPlaying && this.hotkey != KeyCode.None && UICommon.GetKey(KeyCode.LeftShift, false) && UICommon.GetKeyUp(this.hotkey, UICommon.ModifierKey.None, UICommon.ModifierKey.None))
		{
			this.t = 0f;
			this.Play(!this.playing, true, true);
		}
		float unscaledTime = UnityEngine.Time.unscaledTime;
		if (this.last_time < 0f)
		{
			this.last_time = unscaledTime;
			return;
		}
		float num = unscaledTime - this.last_time;
		if (num <= 0f)
		{
			return;
		}
		this.last_time = unscaledTime;
		if (!this.playing || this.len <= 0f)
		{
			return;
		}
		this.MoveTo(this.t + num * CameraPath.global_speed * this.speed_mul / this.len);
		if (this.t >= 1f)
		{
			this.Play(false, false, true);
			if (CameraPath.auto_play)
			{
				CameraPath cameraPath = this.next;
				if (cameraPath == null)
				{
					return;
				}
				cameraPath.Play(true, false, true);
			}
		}
	}

	// Token: 0x06000308 RID: 776 RVA: 0x00028A50 File Offset: 0x00026C50
	public void Clear()
	{
		this.waypoints.Clear();
		this.len = (this.len2d = 0f);
		this.t = 0f;
		this.idx = 0;
		this.dt = 0f;
	}

	// Token: 0x06000309 RID: 777 RVA: 0x00028A9C File Offset: 0x00026C9C
	public void Recalc()
	{
		int count = this.waypoints.Count;
		this.len = (this.len2d = 0f);
		if (count < 1)
		{
			return;
		}
		for (int i = 0; i < count; i++)
		{
			CameraPath.Waypoint waypoint = this.waypoints[i];
			waypoint.t = this.len;
			Vector3 vector;
			if (i + 1 >= count)
			{
				vector = Vector3.zero;
				waypoint.len = (waypoint.len2d = 0f);
				if (i > 0)
				{
					CameraPath.Waypoint waypoint2 = this.waypoints[i - 1];
					waypoint.yaw = waypoint2.yaw;
					waypoint.pitch = waypoint2.pitch;
				}
				else
				{
					waypoint.pitch = (waypoint.yaw = 0f);
				}
				this.waypoints[i] = waypoint;
				break;
			}
			vector = this.waypoints[i + 1].pos - waypoint.pos;
			waypoint.len = vector.magnitude;
			this.len += waypoint.len;
			Vector3 vector2 = new Vector3(vector.x, 0f, vector.z);
			waypoint.len2d = vector2.magnitude;
			this.len2d += waypoint.len2d;
			float num = (waypoint.len2d > 0f) ? Mathf.Atan2(vector.z, vector.x) : 0f;
			float num2 = (waypoint.len > 0f) ? Mathf.Atan2(vector.y, waypoint.len2d) : 0f;
			waypoint.yaw = global::Common.NormalizeAngle180(num * 57.29578f);
			waypoint.pitch = global::Common.NormalizeAngle180(num2 * 57.29578f);
			this.waypoints[i] = waypoint;
		}
		this.SetPivot(this.pivot);
	}

	// Token: 0x0600030A RID: 778 RVA: 0x00028C88 File Offset: 0x00026E88
	public void Record(Vector3 pos, Quaternion rot)
	{
		CameraPath.Waypoint waypoint = default(CameraPath.Waypoint);
		waypoint.pos = this.W2L(pos);
		waypoint.rot = this.W2L(rot);
		if (this.waypoints.Count < 1 || (this.waypoints.Count == 1 && this.t > 0f) || this.t > 1f)
		{
			this.waypoints.Add(waypoint);
			this.Recalc();
			this.MoveTo(1.1f);
			return;
		}
		if (this.t < 0f)
		{
			this.waypoints.Insert(0, waypoint);
			this.Recalc();
			this.MoveTo(-0.1f);
			return;
		}
		if (this.dt > 0f && this.dt < 1f)
		{
			this.waypoints.Insert(this.idx + 1, waypoint);
			this.Recalc();
			this.MoveToIdx(this.idx + 1, 0f, true);
			return;
		}
		this.waypoints[this.idx] = waypoint;
		this.Recalc();
		this.MoveToIdx(this.idx, 0f, true);
	}

	// Token: 0x0600030B RID: 779 RVA: 0x00028DAC File Offset: 0x00026FAC
	public void Delete()
	{
		if (this.waypoints.Count < 1)
		{
			return;
		}
		this.waypoints.RemoveAt(this.idx);
		this.Recalc();
		if (this.waypoints.Count < 1)
		{
			this.t = 0f;
			this.idx = 0;
			this.dt = 0f;
			return;
		}
		if (this.idx < this.waypoints.Count)
		{
			this.MoveToIdx(this.idx, 0f, true);
			return;
		}
		this.MoveToIdx(this.idx - 1, 0f, true);
	}

	// Token: 0x0600030C RID: 780 RVA: 0x00028E48 File Offset: 0x00027048
	public void Record()
	{
		Camera main = Camera.main;
		if (main != null)
		{
			this.Record(main.transform.position, main.transform.rotation);
			return;
		}
		Debug.LogError("No active camera");
	}

	// Token: 0x0600030D RID: 781 RVA: 0x00028E8C File Offset: 0x0002708C
	public void Set(Vector3 pos, Quaternion rot)
	{
		Camera main = Camera.main;
		if (!(CameraPath.Playing == null) || !Application.isPlaying)
		{
			if (main != null)
			{
				main.transform.SetPositionAndRotation(pos, rot);
			}
			return;
		}
		GameCamera component = global::Common.GetComponent<GameCamera>(main.gameObject, null);
		if (component != null && component.currentControllScheme != null)
		{
			component.currentControllScheme.Set(pos, rot);
			return;
		}
		Debug.LogError("Camera error!", main.gameObject);
	}

	// Token: 0x0600030E RID: 782 RVA: 0x00028F08 File Offset: 0x00027108
	public Vector3 CalcPos(int idx, float dt, bool h_smooth, bool v_smooth)
	{
		CameraPath.Waypoint waypoint = this.waypoints[idx];
		if (waypoint.len <= 0f)
		{
			return this.L2W(waypoint.pos);
		}
		if (idx + 1 >= this.waypoints.Count)
		{
			return this.L2W(waypoint.pos);
		}
		float num = 0f;
		float num2 = 0f;
		if (idx > 0)
		{
			CameraPath.Waypoint waypoint2 = this.waypoints[idx - 1];
			num = global::Common.NormalizeAngle180(waypoint.yaw - waypoint2.yaw);
			num2 = global::Common.NormalizeAngle180(waypoint.pitch - waypoint2.pitch);
		}
		CameraPath.Waypoint waypoint3 = this.waypoints[idx + 1];
		float num3 = global::Common.NormalizeAngle180(waypoint3.yaw - waypoint.yaw);
		float num4 = global::Common.NormalizeAngle180(waypoint3.pitch - waypoint.pitch);
		float d = 0f;
		float d2 = 0f;
		if (h_smooth)
		{
			Keyframe[] array = new Keyframe[]
			{
				new Keyframe(0f, 0f),
				new Keyframe(waypoint.len, 0f)
			};
			array[0].outTangent = num * 0.017453292f * 0.5f;
			array[1].inTangent = -num3 * 0.017453292f * 0.5f;
			d = new AnimationCurve(array).Evaluate(dt * waypoint.len);
		}
		if (v_smooth)
		{
			Keyframe[] array2 = new Keyframe[]
			{
				new Keyframe(0f, 0f),
				new Keyframe(waypoint.len, 0f)
			};
			array2[0].outTangent = -num2 * 0.017453292f * 0.5f;
			array2[1].inTangent = num4 * 0.017453292f * 0.5f;
			d2 = new AnimationCurve(array2).Evaluate(dt * waypoint.len);
		}
		Vector3 a = waypoint3.pos - waypoint.pos;
		Vector3 normalized = a.normalized;
		Vector3 rightVector = global::Common.GetRightVector(normalized, 0f);
		Vector3 a2 = Vector3.Cross(normalized, rightVector);
		return this.L2W(waypoint.pos + a * dt + rightVector * d + a2 * d2);
	}

	// Token: 0x0600030F RID: 783 RVA: 0x00029154 File Offset: 0x00027354
	public Quaternion CalcRot(int idx, float dt)
	{
		CameraPath.Waypoint waypoint = this.waypoints[idx];
		if (idx + 1 >= this.waypoints.Count)
		{
			return this.L2W(waypoint.rot);
		}
		CameraPath.Waypoint waypoint2 = this.waypoints[idx + 1];
		return this.L2W(Quaternion.Lerp(waypoint.rot, waypoint2.rot, dt));
	}

	// Token: 0x06000310 RID: 784 RVA: 0x000291B4 File Offset: 0x000273B4
	public void MoveToIdx(int idx, float dt = 0f, bool update_t = true)
	{
		if (idx < 0 || idx >= this.waypoints.Count)
		{
			return;
		}
		bool flag = idx != this.idx;
		this.idx = idx;
		this.dt = dt;
		CameraPath.Waypoint waypoint = this.waypoints[idx];
		if (update_t)
		{
			if (this.len > 0f)
			{
				this.t = (waypoint.t + dt * waypoint.len) / this.len;
			}
			else
			{
				this.t = 0f;
			}
		}
		if (dt <= 0f || idx + 1 >= this.waypoints.Count)
		{
			this.Set(this.L2W(waypoint.pos), this.L2W(waypoint.rot));
		}
		else
		{
			Vector3 pos = this.CalcPos(idx, dt, CameraPath.h_smooth, CameraPath.v_smooth);
			Quaternion rot = this.CalcRot(idx, dt);
			this.Set(pos, rot);
		}
		if (flag && this.playing && !string.IsNullOrEmpty(waypoint.console_command))
		{
			AttributeConsoleManager instance = AttributeConsoleManager.instance;
			if (instance == null)
			{
				return;
			}
			instance.AttemptExecute(waypoint.console_command);
		}
	}

	// Token: 0x06000311 RID: 785 RVA: 0x000292C0 File Offset: 0x000274C0
	private void Resolve(float t, out int idx, out float dt)
	{
		if (this.waypoints.Count <= 0)
		{
			idx = 0;
			dt = 0f;
			return;
		}
		t = Mathf.Clamp01(t);
		t *= this.len;
		if (t <= 0f)
		{
			idx = 0;
			dt = 0f;
			return;
		}
		for (int i = 0; i < this.waypoints.Count - 1; i++)
		{
			CameraPath.Waypoint waypoint = this.waypoints[i];
			float num = t - waypoint.t;
			if (num < waypoint.len)
			{
				idx = i;
				dt = num / waypoint.len;
				return;
			}
		}
		idx = this.waypoints.Count - 1;
		dt = 0f;
	}

	// Token: 0x06000312 RID: 786 RVA: 0x00029368 File Offset: 0x00027568
	public void MoveTo(float t)
	{
		this.t = t;
		int num;
		float num2;
		this.Resolve(t, out num, out num2);
		this.MoveToIdx(num, num2, false);
	}

	// Token: 0x06000313 RID: 787 RVA: 0x00029390 File Offset: 0x00027590
	public bool IsPlaying()
	{
		return this.playing;
	}

	// Token: 0x06000314 RID: 788 RVA: 0x00029398 File Offset: 0x00027598
	public void Play(bool play, bool disable_auto_play = true, bool hide_cursor = true)
	{
		if (disable_auto_play)
		{
			CameraPath.auto_play = false;
		}
		if (this.playing == play)
		{
			return;
		}
		if (MovieRecorder.instance != null && MovieRecorder.instance.AutoRecord)
		{
			if (play)
			{
				MovieRecorder.instance.BeginRecording(base.gameObject.name + " ");
			}
			else
			{
				MovieRecorder.instance.EndRecording();
			}
		}
		Cursor.visible = !play;
		if (play)
		{
			if (CameraPath.Playing != null)
			{
				CameraPath.Playing.Play(false, true, true);
			}
			CameraPath.Playing = this;
		}
		else if (CameraPath.Playing == this)
		{
			CameraPath.Playing = null;
		}
		if (play && (this.t < 0f || this.t >= 1f))
		{
			this.t = 0f;
		}
		this.playing = play;
	}

	// Token: 0x06000315 RID: 789 RVA: 0x00029470 File Offset: 0x00027670
	public void CalcHotKey()
	{
		int siblingIndex = base.transform.GetSiblingIndex();
		if (siblingIndex > 11)
		{
			this.hotkey = KeyCode.None;
			return;
		}
		this.hotkey = KeyCode.F1 + siblingIndex;
	}

	// Token: 0x06000316 RID: 790 RVA: 0x000294A3 File Offset: 0x000276A3
	private void Register()
	{
		this.prev = CameraPath.last;
		if (CameraPath.last != null)
		{
			CameraPath.last.next = this;
		}
		else
		{
			CameraPath.first = this;
		}
		CameraPath.last = this;
	}

	// Token: 0x06000317 RID: 791 RVA: 0x000294D8 File Offset: 0x000276D8
	private void Unregister()
	{
		if (this.next != null)
		{
			this.next.prev = this.prev;
		}
		else
		{
			CameraPath.last = this.prev;
		}
		if (this.prev != null)
		{
			this.prev.next = this.next;
		}
		else
		{
			CameraPath.first = this.next;
		}
		this.prev = (this.next = null);
	}

	// Token: 0x06000318 RID: 792 RVA: 0x0002954D File Offset: 0x0002774D
	private static void DeleteAll()
	{
		while (CameraPath.first != null)
		{
			UnityEngine.Object.DestroyImmediate(CameraPath.first.gameObject);
		}
	}

	// Token: 0x06000319 RID: 793 RVA: 0x00029570 File Offset: 0x00027770
	private DT.Field SaveWaypoint(CameraPath.Waypoint wpt, int idx)
	{
		DT.Field field = new DT.Field(null);
		field.type = "waypoint";
		field.key = idx.ToString();
		Vector3 vector = this.L2W(wpt.pos);
		Quaternion quaternion = this.L2W(wpt.rot);
		field.SetValue("pos", string.Concat(new object[]
		{
			vector.x,
			"/",
			vector.y,
			"/",
			vector.z
		}), null);
		field.SetValue("rot", string.Concat(new object[]
		{
			quaternion.x,
			"/",
			quaternion.y,
			"/",
			quaternion.z,
			"/",
			quaternion.w
		}), null);
		if (!string.IsNullOrEmpty(wpt.console_command))
		{
			field.SetValue("console_command", DT.Enquote(wpt.console_command), null);
		}
		return field;
	}

	// Token: 0x0600031A RID: 794 RVA: 0x00029698 File Offset: 0x00027898
	private CameraPath.Waypoint LoadWaypoint(DT.Field f)
	{
		CameraPath.Waypoint result = default(CameraPath.Waypoint);
		DT.Field field = f.FindChild("pos", null, true, true, true, '.');
		if (field != null)
		{
			result.pos = this.W2L(new Vector3(field.Float(0, null, 0f), field.Float(1, null, 0f), field.Float(2, null, 0f)));
		}
		DT.Field field2 = f.FindChild("rot", null, true, true, true, '.');
		if (field2 != null)
		{
			result.rot = this.W2L(new Quaternion(field2.Float(0, null, 0f), field2.Float(1, null, 0f), field2.Float(2, null, 0f), field2.Float(3, null, 0f)));
		}
		result.console_command = f.GetString("console_command", null, null, true, true, true, '.');
		return result;
	}

	// Token: 0x0600031B RID: 795 RVA: 0x00029770 File Offset: 0x00027970
	public DT.Field ToDT(int idx)
	{
		DT.Field field = new DT.Field(null);
		field.type = "CameraPath";
		field.key = idx.ToString();
		field.SetValue("name", DT.Enquote(base.name), base.name);
		if (this.speed_mul != 1f)
		{
			field.SetValue("speed_mul", this.speed_mul.ToString(), this.speed_mul);
		}
		if (this.pivot != 0f)
		{
			field.SetValue("pivot", this.pivot.ToString(), this.pivot);
		}
		for (int i = 0; i < this.waypoints.Count; i++)
		{
			CameraPath.Waypoint wpt = this.waypoints[i];
			DT.Field child = this.SaveWaypoint(wpt, i + 1);
			field.AddChild(child);
		}
		return field;
	}

	// Token: 0x0600031C RID: 796 RVA: 0x00029850 File Offset: 0x00027A50
	public void Load(DT.Field f)
	{
		this.speed_mul = f.GetFloat("speed_mul", null, 1f, true, true, true, '.');
		this.pivot = f.GetFloat("pivot", null, 0f, true, true, true, '.');
		if (f.children != null)
		{
			for (int i = 0; i < f.children.Count; i++)
			{
				DT.Field field = f.children[i];
				if (!(field.type != "waypoint"))
				{
					CameraPath.Waypoint item = this.LoadWaypoint(field);
					this.waypoints.Add(item);
				}
			}
		}
		this.Recalc();
	}

	// Token: 0x0600031D RID: 797 RVA: 0x000298EC File Offset: 0x00027AEC
	public static void SaveAll()
	{
		string str = Path.GetFileNameWithoutExtension(SceneManager.GetActiveScene().path).ToLowerInvariant();
		string path = "Maps/" + str + "/camera_paths.def";
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("CameraPaths " + str + "\r\n{");
		int num = 0;
		CameraPath cameraPath = CameraPath.first;
		Transform transform = (cameraPath != null) ? cameraPath.transform.parent : null;
		if (transform != null)
		{
			for (int i = 0; i < transform.childCount; i++)
			{
				CameraPath component = transform.GetChild(i).GetComponent<CameraPath>();
				if (!(component == null))
				{
					string value = global::Defs.Save(component.ToDT(++num), "\t", null);
					stringBuilder.Append(value);
				}
			}
		}
		CameraPath cameraPath2 = CameraPath.first;
		while (cameraPath2 != null)
		{
			if (!(transform != null) || !(cameraPath2.transform.parent == transform))
			{
				string value2 = global::Defs.Save(cameraPath2.ToDT(++num), "\t", null);
				stringBuilder.Append(value2);
			}
			cameraPath2 = cameraPath2.next;
		}
		stringBuilder.Append("\r\n}");
		File.WriteAllText(path, stringBuilder.ToString());
		Debug.Log("Saved " + num + " camera path(s)");
	}

	// Token: 0x0600031E RID: 798 RVA: 0x00029A4C File Offset: 0x00027C4C
	public static int LoadAll(GameObject parent = null)
	{
		if (parent == null)
		{
			CameraPaths instance = CameraPaths.instance;
			GameObject gameObject;
			if ((gameObject = ((instance != null) ? instance.gameObject : null)) == null)
			{
				CameraPath cameraPath = CameraPath.first;
				gameObject = ((cameraPath != null) ? cameraPath.gameObject.transform.parent.gameObject : null);
			}
			parent = gameObject;
		}
		CameraPath.DeleteAll();
		string str = Path.GetFileNameWithoutExtension(SceneManager.GetActiveScene().path).ToLowerInvariant();
		string path = "Maps/" + str + "/camera_paths.def";
		List<DT.Field> list = DT.Parser.ReadFile(null, path, null);
		if (list == null || list.Count != 1 || list[0] == null || list[0].children == null)
		{
			return 0;
		}
		list = list[0].children;
		int num = 0;
		for (int i = 0; i < list.Count; i++)
		{
			DT.Field field = list[i];
			if (!(field.type != "CameraPath"))
			{
				if (parent == null)
				{
					parent = new GameObject("Camera Paths", new Type[]
					{
						typeof(CameraPaths)
					});
				}
				GameObject gameObject2 = new GameObject(field.GetString("name", null, field.key, true, true, true, '.'), new Type[]
				{
					typeof(CameraPath)
				});
				gameObject2.transform.SetParent(parent.transform, false);
				CameraPath component = gameObject2.GetComponent<CameraPath>();
				component.CalcHotKey();
				component.Load(field);
				num++;
			}
		}
		Debug.Log("Loaded " + num + " camera path(s)");
		return num;
	}

	// Token: 0x040003EB RID: 1003
	private static CameraPath first = null;

	// Token: 0x040003EC RID: 1004
	private static CameraPath last = null;

	// Token: 0x040003ED RID: 1005
	private CameraPath prev;

	// Token: 0x040003EE RID: 1006
	private CameraPath next;

	// Token: 0x040003EF RID: 1007
	public static bool auto_play = false;

	// Token: 0x040003F0 RID: 1008
	public static CameraPath Playing = null;

	// Token: 0x040003F1 RID: 1009
	public static int SetPhysicalCamera = -1;

	// Token: 0x040003F2 RID: 1010
	public List<CameraPath.Waypoint> waypoints = new List<CameraPath.Waypoint>();

	// Token: 0x040003F3 RID: 1011
	public float len;

	// Token: 0x040003F4 RID: 1012
	public float len2d;

	// Token: 0x040003F5 RID: 1013
	public float t;

	// Token: 0x040003F6 RID: 1014
	public int idx;

	// Token: 0x040003F7 RID: 1015
	public float dt;

	// Token: 0x040003F8 RID: 1016
	public float pivot;

	// Token: 0x040003F9 RID: 1017
	public static float global_speed = 7.5f;

	// Token: 0x040003FA RID: 1018
	public float speed_mul = 1f;

	// Token: 0x040003FB RID: 1019
	public KeyCode hotkey;

	// Token: 0x040003FC RID: 1020
	public static bool use_scene_view = true;

	// Token: 0x040003FD RID: 1021
	public static bool h_smooth = true;

	// Token: 0x040003FE RID: 1022
	public static bool v_smooth = true;

	// Token: 0x040003FF RID: 1023
	private bool playing;

	// Token: 0x04000400 RID: 1024
	private float last_time = -1f;

	// Token: 0x02000536 RID: 1334
	[Serializable]
	public struct Waypoint
	{
		// Token: 0x04002F75 RID: 12149
		public Vector3 pos;

		// Token: 0x04002F76 RID: 12150
		public Quaternion rot;

		// Token: 0x04002F77 RID: 12151
		public float t;

		// Token: 0x04002F78 RID: 12152
		public float len;

		// Token: 0x04002F79 RID: 12153
		public float len2d;

		// Token: 0x04002F7A RID: 12154
		public float yaw;

		// Token: 0x04002F7B RID: 12155
		public float pitch;

		// Token: 0x04002F7C RID: 12156
		public string console_command;
	}
}
