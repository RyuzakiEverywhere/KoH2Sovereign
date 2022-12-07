using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x020000DA RID: 218
public class GameMode : CameraMode
{
	// Token: 0x06000ACD RID: 2765 RVA: 0x0007D2D8 File Offset: 0x0007B4D8
	public GameMode(GameCamera gameCamera) : base(gameCamera)
	{
	}

	// Token: 0x06000ACE RID: 2766 RVA: 0x0007D300 File Offset: 0x0007B500
	public void GetCurValues(out float dist, out float pitch, out float yaw)
	{
		dist = this.cam_dist;
		pitch = this.cam_pitch;
		yaw = this.cam_yaw;
	}

	// Token: 0x06000ACF RID: 2767 RVA: 0x0007D31C File Offset: 0x0007B51C
	public override void LookAt(Vector3 newPoint)
	{
		if (float.IsInfinity(newPoint.x) || float.IsInfinity(newPoint.y) || float.IsInfinity(newPoint.z))
		{
			Debug.LogError(string.Format("Invalid lookAt point: {0}", newPoint));
			return;
		}
		this.owner.Camera.transform.rotation = Quaternion.Euler(this.cam_pitch, this.cam_yaw, 0f);
		Vector3 vector2;
		if (this.owner.Settings.snapToTerrain)
		{
			newPoint.y = this.owner.Settings.lookAtHeight;
			this.ptLookAt = base.GetValidLookAtPoint(newPoint, this.owner);
			this.ptLookAtPreHeight = this.ptLookAt;
			Vector3 vector = this.ptLookAt;
			this.owner.Camera.transform.position = vector;
			this.ptLookAtPreHeight = this.owner.Camera.transform.TransformPoint(new Vector3(0f, 0f, -this.cam_dist - this.owner.Settings.lookAtHeight));
			vector = global::Common.SnapToTerrain(vector, 0f, null, -1f, true);
			this.owner.Camera.transform.position = vector;
			vector2 = this.owner.Camera.transform.TransformPoint(new Vector3(0f, 0f, -this.cam_dist - this.owner.Settings.lookAtHeight));
		}
		else
		{
			Vector3 vector3 = this.owner.Camera.transform.rotation * Vector3.forward;
			if (vector3.y != 0f)
			{
				float d = (this.owner.Settings.lookAtHeight - newPoint.y) / vector3.y;
				newPoint += vector3 * d;
			}
			newPoint.y = this.owner.Settings.lookAtHeight;
			this.ptLookAt = base.GetValidLookAtPoint(newPoint, this.owner);
			this.owner.Camera.transform.position = this.ptLookAt;
			vector2 = this.owner.Camera.transform.TransformPoint(new Vector3(0f, 0f, -this.cam_dist));
			this.ptLookAtPreHeight = vector2;
		}
		if (vector2.y < this.owner.Settings.minHeight)
		{
			vector2.y = this.owner.Settings.minHeight;
		}
		this.owner.Camera.transform.position = vector2;
	}

	// Token: 0x06000AD0 RID: 2768 RVA: 0x000023FD File Offset: 0x000005FD
	public override void Zoom(float zoom)
	{
	}

	// Token: 0x06000AD1 RID: 2769 RVA: 0x000023FD File Offset: 0x000005FD
	public override void Yaw(float yaw)
	{
	}

	// Token: 0x06000AD2 RID: 2770 RVA: 0x0007D5C0 File Offset: 0x0007B7C0
	public override void Set(Vector3 pos, Quaternion rot)
	{
		float lookAtHeight = this.owner.Settings.lookAtHeight;
		Vector3 vector = rot * Vector3.forward;
		if (vector.y != 0f)
		{
			float cam_dist = (lookAtHeight - pos.y) / vector.y;
			this.cam_dist = cam_dist;
		}
		this.ptLookAt = pos + vector * this.cam_dist;
		this.cam_yaw = rot.eulerAngles.y;
	}

	// Token: 0x06000AD3 RID: 2771 RVA: 0x0007D638 File Offset: 0x0007B838
	public override void UpdateInput(Transform cam, CameraSettings cameraSettings)
	{
		this.over_ui = !BaseUI.Get().IsMousePanEligable();
		if (this.mouse_btn_down < 0)
		{
			for (int i = 0; i <= 2; i++)
			{
				if (Input.GetMouseButtonDown(i))
				{
					this.mouse_btn_down = i;
					this.OnMouseDown(cam, Input.mousePosition, i);
					break;
				}
			}
		}
		if (this.mouse_btn_down >= 0 && Input.GetMouseButtonUp(this.mouse_btn_down))
		{
			this.OnMouseUp(cam, Input.mousePosition, this.mouse_btn_down);
			this.mouse_btn_down = -1;
		}
		this.OnMouseMove(cam, Input.mousePosition, this.mouse_btn_down);
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		if (!UICommon.IsInInput() && base.IsMouseInApplicationScreen())
		{
			this.KeybindCameraMovement(ref num, ref num2, ref num3);
		}
		this.UpdateScroll(ref num, ref num2, ref num3);
		float num4 = cameraSettings.GetScrollSpeed;
		if (cameraSettings.scrollSpeedFar > 0f)
		{
			float y = cameraSettings.dist.y;
			float num5 = Mathf.Lerp(cameraSettings.dist.x, cameraSettings.dist.y, this.cam_dist / y);
			num5 /= y;
			num4 = Mathf.Lerp(cameraSettings.GetScrollSpeed, cameraSettings.GetScrollSpeedFar, num5);
		}
		this.vScroll.x = num * num4;
		this.vScroll.z = num3 * num4;
		float num6 = base.GetInputKeys().zoom ? 15f : cameraSettings.zoomSpeed;
		this.vScroll.y = num2 * num6;
	}

	// Token: 0x06000AD4 RID: 2772 RVA: 0x0007D7C4 File Offset: 0x0007B9C4
	private void KeybindCameraMovement(ref float xScroll, ref float yScroll, ref float zScroll)
	{
		Vector2 zero = Vector2.zero;
		bool flag = false;
		bool flag2 = false;
		if (KeyBindings.GetBind("camera_move_up", false) || UICommon.GetKey(KeyCode.UpArrow, false))
		{
			zero.y += 1f;
			flag2 = true;
		}
		if (KeyBindings.GetBind("camera_move_down", false) || UICommon.GetKey(KeyCode.DownArrow, false))
		{
			zero.y -= 1f;
			flag2 = true;
		}
		if (KeyBindings.GetBind("camera_move_left", false) || UICommon.GetKey(KeyCode.LeftArrow, false))
		{
			zero.x -= 1f;
			flag = true;
		}
		if (KeyBindings.GetBind("camera_move_right", false) || UICommon.GetKey(KeyCode.RightArrow, false))
		{
			zero.x += 1f;
			flag = true;
		}
		if (flag && flag2)
		{
			zero.x *= 0.7071f;
			zero.y *= 0.7071f;
		}
		xScroll = zero.x;
		yScroll = (this.over_ui ? 0f : (-Input.GetAxis("Mouse ScrollWheel")));
		zScroll = zero.y;
	}

	// Token: 0x06000AD5 RID: 2773 RVA: 0x0007D8DC File Offset: 0x0007BADC
	public void OnMouseDown(Transform t, Vector2 pts, int btn)
	{
		if (btn >= 0 || btn < 6)
		{
			this.mouse_down_over_ui[btn] = this.over_ui;
			this.mouse_down_pos[this.mouse_btn_down] = pts;
		}
		bool flag = false;
		if (btn == 0 && BattleMap.Get() == null)
		{
			UserSettings.SettingData setting = UserSettings.GetSetting("left_mouse_map_drag");
			if (setting != null)
			{
				flag = setting.value;
			}
		}
		if ((btn == 2 || flag) && !this.over_ui)
		{
			this.ptPan = this.owner.ScreenToGroundPlane(pts, this.ptLookAtPreHeight, 0f);
			if (btn == 2)
			{
				CameraMode.InputKeys inputKeys = base.GetInputKeys();
				if (inputKeys.yaw)
				{
					this.cam_yaw = 0f;
				}
				if (inputKeys.zoom)
				{
					this.owner.Settings.dist = this.owner.org_dists;
					this.RecalcCameraSettings();
				}
				if (inputKeys.pitch)
				{
					this.owner.Settings.pitch = this.owner.org_pitches;
					this.RecalcCameraSettings();
				}
			}
		}
	}

	// Token: 0x06000AD6 RID: 2774 RVA: 0x0007D9E4 File Offset: 0x0007BBE4
	public void OnMouseUp(Transform t, Vector2 pts, int btn)
	{
		if (this.mouse_btn_down >= 0 || this.mouse_btn_down < 6)
		{
			this.mouse_down_over_ui[this.mouse_btn_down] = this.over_ui;
			this.mouse_down_pos[this.mouse_btn_down].Set(0f, 0f, 0f);
		}
	}

	// Token: 0x06000AD7 RID: 2775 RVA: 0x0007DA3C File Offset: 0x0007BC3C
	public void OnMouseMove(Transform t, Vector2 pts, int btn)
	{
		bool flag = false;
		if (this.mouse_btn_down == 0 && !this.mouse_down_over_ui[0] && BattleMap.Get() == null)
		{
			UserSettings.SettingData setting = UserSettings.GetSetting("left_mouse_map_drag");
			if (setting != null)
			{
				flag = setting.value;
			}
		}
		if ((this.mouse_btn_down == 2 && !this.mouse_down_over_ui[2]) || flag)
		{
			Vector3 vector = this.owner.ScreenToGroundPlane(Input.mousePosition, this.ptLookAtPreHeight, this.owner.Settings.snapToTerrain ? 0f : Mathf.Max(0f, this.ptLookAt.y - this.owner.Settings.lookAtHeight));
			if (vector == Vector3.zero)
			{
				return;
			}
			Vector3 b = this.ptPan - vector;
			if ((this.ptLookAt - (this.ptLookAt + b)).sqrMagnitude > 0.0022f * this.cam_dist)
			{
				this.LookAt(this.ptLookAt + b);
			}
		}
	}

	// Token: 0x06000AD8 RID: 2776 RVA: 0x0007DB58 File Offset: 0x0007BD58
	public override void UpdateCamera(Transform t, CameraSettings cameraSettings)
	{
		if (!(CameraPath.Playing != null))
		{
			float unscaledDeltaTime = UnityEngine.Time.unscaledDeltaTime;
			if (this.mouse_btn_down >= 0 && !BaseUI.Get().OverrideEdgeScroll())
			{
				this.vScroll.x = (this.vScroll.z = 0f);
			}
			CameraMode.InputKeys inputKeys = base.GetInputKeys();
			if (inputKeys.yaw_left)
			{
				this.cam_yaw += 30f * unscaledDeltaTime;
			}
			if (inputKeys.yaw_right)
			{
				this.cam_yaw -= 30f * unscaledDeltaTime;
			}
			if (this.vScroll.y != 0f)
			{
				if (inputKeys.yaw)
				{
					this.cam_yaw += this.vScroll.y;
				}
				else if (!inputKeys.pitch)
				{
					this.cam_dist += this.vScroll.y;
					if (!inputKeys.zoom)
					{
						this.RecalcCameraSettings();
					}
					else
					{
						if (this.cam_dist < 0f)
						{
							this.cam_dist = 0f;
						}
						if (this.cam_dist < this.owner.Settings.dist[0])
						{
							this.owner.Settings.dist[0] = this.cam_dist;
						}
						if (this.cam_dist > this.owner.Settings.dist[1])
						{
							this.owner.Settings.dist[1] = this.cam_dist;
						}
					}
				}
				else
				{
					this.cam_pitch += this.vScroll.y;
					if (this.cam_pitch < 0f)
					{
						this.cam_pitch = 0f;
					}
					else if (this.cam_pitch > 90f)
					{
						this.cam_pitch = 90f;
					}
					if (this.cam_dist <= this.owner.Settings.dist[0])
					{
						this.owner.Settings.pitch[0] = this.cam_pitch;
					}
					if (this.cam_dist >= this.owner.Settings.dist[1])
					{
						this.owner.Settings.pitch[1] = this.cam_pitch;
					}
				}
			}
			this.last_cam_dist = this.cam_dist;
			this.cam_yaw += this.orbit_speed * unscaledDeltaTime;
			if (this.cam_yaw >= 360f)
			{
				this.cam_yaw -= 360f;
			}
			else if (this.cam_yaw < 0f)
			{
				this.cam_yaw += 360f;
			}
			Vector3 right = t.right;
			right.y = 0f;
			right.Normalize();
			Vector3 a = -global::Common.GetRightVector(right, 0f);
			this.LookAt(this.ptLookAt + this.vScroll.x * unscaledDeltaTime * right + this.vScroll.z * unscaledDeltaTime * a);
			if (this.vScroll.y != 0f && ViewMode.IsPoliticalView())
			{
				ViewMode.current.RefreshPVFigures();
			}
			this.vScroll.y = 0f;
			return;
		}
		if (CameraPath.SetPhysicalCamera == 0)
		{
			this.owner.Camera.usePhysicalProperties = false;
			return;
		}
		if (CameraPath.SetPhysicalCamera == 1)
		{
			this.owner.Camera.usePhysicalProperties = true;
		}
	}

	// Token: 0x06000AD9 RID: 2777 RVA: 0x0007DEDC File Offset: 0x0007C0DC
	public override void RecalcCameraSettings()
	{
		if (this.cam_dist > this.owner.Settings.dist[1])
		{
			this.cam_dist = this.owner.Settings.dist[1];
		}
		else if (this.cam_dist < this.owner.Settings.dist[0])
		{
			this.cam_dist = this.owner.Settings.dist[0];
		}
		this.cam_pitch = global::Common.map(this.cam_dist, this.owner.Settings.dist[0], this.owner.Settings.dist[1], this.owner.Settings.pitch[0], this.owner.Settings.pitch[1], false);
		if (this.last_cam_dist == 0f)
		{
			this.last_cam_dist = this.cam_dist;
		}
		if (this.cam_dist != this.last_cam_dist && this.owner.focus_mouse_zoom)
		{
			Vector3 b = this.owner.ScreenToGroundPlane(Input.mousePosition, this.ptLookAtPreHeight, 0f);
			b.y = this.ptLookAt.y;
			float d = this.cam_dist - this.last_cam_dist;
			Vector3 a = this.ptLookAt - b;
			a /= this.last_cam_dist;
			this.ptLookAt += a * d;
		}
		this.owner.Camera.farClipPlane = global::Common.map(this.cam_dist, this.owner.Settings.dist[0], this.owner.Settings.dist[1], this.owner.Settings.farPlane[0], this.owner.Settings.farPlane[1], false);
	}

	// Token: 0x06000ADA RID: 2778 RVA: 0x0007E0E8 File Offset: 0x0007C2E8
	private void UpdateScroll(ref float xScroll, ref float yScroll, ref float zScroll)
	{
		if (!this.owner.EdgeScroll && !BaseUI.Get().OverrideEdgeScroll())
		{
			return;
		}
		bool flag = false;
		if (Application.isEditor && !flag)
		{
			return;
		}
		if (!Application.isFocused)
		{
			return;
		}
		BaseUI baseUI = BaseUI.Get();
		if (baseUI != null && baseUI.menu != null && baseUI.menu.activeSelf)
		{
			return;
		}
		this.EdgeScrollingCursorChange(ref xScroll, ref yScroll, ref zScroll);
	}

	// Token: 0x06000ADB RID: 2779 RVA: 0x0007E158 File Offset: 0x0007C358
	private void SetCursor(DT.Field field)
	{
		if (this.active_cursor != field)
		{
			if (field != null)
			{
				Cursor.SetCursor(field.GetValue("tex", null, true, true, true, '.').Get<Texture2D>(), field.GetPoint("hotspot", null, true, true, true, '.'), CursorMode.Auto);
			}
			this.active_cursor = field;
		}
	}

	// Token: 0x06000ADC RID: 2780 RVA: 0x0007E1B0 File Offset: 0x0007C3B0
	private void EdgeScrollingCursorChange(ref float xScroll, ref float yScroll, ref float zScroll)
	{
		Vector2 vector = Input.mousePosition;
		bool flag = vector.x <= this.deadZones.x;
		bool flag2 = vector.x >= (float)Screen.width - this.deadZones.z;
		bool flag3 = vector.y <= this.deadZones.y;
		bool flag4 = vector.y >= (float)Screen.height - this.deadZones.w;
		DT.Field defField = global::Defs.GetDefField("Cursors", null);
		if (xScroll <= 0f && flag2)
		{
			xScroll += 1f;
			this.SetCursor(defField.FindChild("East_Scroll_Cursor", null, true, true, true, '.'));
		}
		else if (xScroll >= 0f && flag)
		{
			xScroll -= 1f;
			this.SetCursor(defField.FindChild("West_Scroll_Cursor", null, true, true, true, '.'));
		}
		if (zScroll <= 0f && flag4)
		{
			zScroll += 1f;
			this.SetCursor(defField.FindChild("North_Scroll_Cursor", null, true, true, true, '.'));
		}
		else if (zScroll >= 0f && flag3)
		{
			zScroll -= 1f;
			this.SetCursor(defField.FindChild("South_Scroll_Cursor", null, true, true, true, '.'));
		}
		if (!flag && !flag2 && !flag3 && !flag4)
		{
			this.SetCursor(defField.FindChild("Normal_Cursor", null, true, true, true, '.'));
			return;
		}
		yScroll = 0f;
	}

	// Token: 0x06000ADD RID: 2781 RVA: 0x0007E33C File Offset: 0x0007C53C
	private void EdgeScrollingNoCursor(ref float xScroll, ref float yScroll, ref float zScroll)
	{
		Vector2 vector = Input.mousePosition;
		bool flag = vector.x <= this.deadZones.x;
		bool flag2 = vector.x >= (float)Screen.width - this.deadZones.z;
		bool flag3 = vector.y <= this.deadZones.y;
		bool flag4 = vector.y >= (float)Screen.height - this.deadZones.w;
		if (xScroll <= 0f && flag2)
		{
			xScroll += 1f;
		}
		else if (xScroll >= 0f && flag)
		{
			xScroll -= 1f;
		}
		if (zScroll <= 0f && flag4)
		{
			zScroll += 1f;
		}
		else if (zScroll >= 0f && flag3)
		{
			zScroll -= 1f;
		}
		if (flag || flag2 || flag3 || flag4)
		{
			yScroll = 0f;
		}
	}

	// Token: 0x06000ADE RID: 2782 RVA: 0x0007E441 File Offset: 0x0007C641
	public override float GetDistanceToAimPoint()
	{
		return this.cam_dist;
	}

	// Token: 0x06000ADF RID: 2783 RVA: 0x0007E449 File Offset: 0x0007C649
	public override Vector3 GetScreenToWorldPoint(Vector2 screenPoint)
	{
		return BaseUI.Get().ScreenToGroundPlane(screenPoint);
	}

	// Token: 0x06000AE0 RID: 2784 RVA: 0x0007E458 File Offset: 0x0007C658
	public override KeyValuePair<Vector3, Quaternion> CalcPositionAndRotations(Vector3 lookAtPoint, float zoom)
	{
		float num = Mathf.Lerp(this.owner.Settings.dist[0], this.owner.Settings.dist[1], zoom);
		float x = global::Common.map(num, this.owner.Settings.dist[0], this.owner.Settings.dist[1], this.owner.Settings.pitch[0], this.owner.Settings.pitch[1], false);
		lookAtPoint.y = this.owner.Settings.lookAtHeight;
		Quaternion quaternion = Quaternion.Euler(x, this.cam_yaw, 0f);
		Vector3 vector = lookAtPoint + quaternion * new Vector3(0f, 0f, -num);
		if (vector.y < this.owner.Settings.minHeight)
		{
			vector.y = this.owner.Settings.minHeight;
		}
		return new KeyValuePair<Vector3, Quaternion>(vector, quaternion);
	}

	// Token: 0x06000AE1 RID: 2785 RVA: 0x0007E574 File Offset: 0x0007C774
	public override ValueTuple<Vector3, Quaternion> GetAudioListenerPositionAndRotation()
	{
		Vector3 ptLookAt = this.ptLookAt;
		ptLookAt.y = this.owner.Camera.transform.position.y;
		return new ValueTuple<Vector3, Quaternion>(Vector3.Lerp(ptLookAt, this.owner.Camera.transform.position, 0.3f), Quaternion.LookRotation(Vector3.down, Vector3.up));
	}

	// Token: 0x04000878 RID: 2168
	private Vector3 ptPan;

	// Token: 0x04000879 RID: 2169
	private float cam_pitch;

	// Token: 0x0400087A RID: 2170
	private float cam_focal_length;

	// Token: 0x0400087B RID: 2171
	private float cam_sensor_size_x;

	// Token: 0x0400087C RID: 2172
	private float cam_sensor_size_y;

	// Token: 0x0400087D RID: 2173
	private float cam_lens_shift_x;

	// Token: 0x0400087E RID: 2174
	private float cam_lens_shift_y;

	// Token: 0x0400087F RID: 2175
	private float cam_yaw;

	// Token: 0x04000880 RID: 2176
	private float orbit_speed;

	// Token: 0x04000881 RID: 2177
	private float last_cam_dist;

	// Token: 0x04000882 RID: 2178
	private Vector4 deadZones = new Vector4(4f, 4f, 6f, 6f);
}
