using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x020000D9 RID: 217
public abstract class CameraMode : ICameraControllScheme
{
	// Token: 0x06000ABC RID: 2748 RVA: 0x0007CEAC File Offset: 0x0007B0AC
	public CameraMode(GameCamera gameCamera)
	{
		this.owner = gameCamera;
		if (this.owner.Settings != null)
		{
			this.cam_dist = Mathf.Clamp(this.owner.Settings.initialDist, this.owner.Settings.dist[0], this.owner.Settings.dist[1]);
		}
	}

	// Token: 0x06000ABD RID: 2749 RVA: 0x0007CF50 File Offset: 0x0007B150
	public CameraMode.InputKeys GetInputKeys()
	{
		bool flag = Game.CheckCheatLevelNoWarning(Game.CheatLevel.Low);
		bool flag2 = BattleMap.Get() != null;
		CameraMode.InputKeys result = default(CameraMode.InputKeys);
		if (flag2 || flag)
		{
			result.yaw = UICommon.GetModifierKey(UICommon.ModifierKey.Ctrl);
			result.yaw_left = KeyBindings.GetBind("camera_rotate_left", false);
			result.yaw_right = KeyBindings.GetBind("camera_rotate_right", false);
		}
		if (flag)
		{
			result.pitch = UICommon.GetModifierKey(UICommon.ModifierKey.RightShift);
			result.zoom = UICommon.GetModifierKey(UICommon.ModifierKey.RightAlt);
		}
		return result;
	}

	// Token: 0x06000ABE RID: 2750 RVA: 0x0007CFD4 File Offset: 0x0007B1D4
	protected bool IsMouseInApplicationScreen()
	{
		Vector2 vector = Input.mousePosition;
		return vector.x > 0f && vector.x < (float)Screen.width && vector.y > 0f && vector.y < (float)Screen.height;
	}

	// Token: 0x06000ABF RID: 2751
	public abstract void UpdateInput(Transform camTransform, CameraSettings cameraSettings);

	// Token: 0x06000AC0 RID: 2752
	public abstract void UpdateCamera(Transform camTransform, CameraSettings cameraSettings);

	// Token: 0x06000AC1 RID: 2753
	public abstract void LookAt(Vector3 point);

	// Token: 0x06000AC2 RID: 2754
	public abstract void Zoom(float zoom);

	// Token: 0x06000AC3 RID: 2755
	public abstract void Yaw(float yaw);

	// Token: 0x06000AC4 RID: 2756
	public abstract void Set(Vector3 pos, Quaternion rot);

	// Token: 0x06000AC5 RID: 2757
	public abstract void RecalcCameraSettings();

	// Token: 0x06000AC6 RID: 2758
	public abstract float GetDistanceToAimPoint();

	// Token: 0x06000AC7 RID: 2759
	public abstract Vector3 GetScreenToWorldPoint(Vector2 screenPoint);

	// Token: 0x06000AC8 RID: 2760
	public abstract KeyValuePair<Vector3, Quaternion> CalcPositionAndRotations(Vector3 lookAtPoint, float zoom);

	// Token: 0x06000AC9 RID: 2761
	public abstract ValueTuple<Vector3, Quaternion> GetAudioListenerPositionAndRotation();

	// Token: 0x06000ACA RID: 2762 RVA: 0x0007D024 File Offset: 0x0007B224
	public void Reset()
	{
		this.mouse_btn_down = -1;
	}

	// Token: 0x06000ACB RID: 2763 RVA: 0x0007D030 File Offset: 0x0007B230
	public Vector3 GetValidLookAtPoint(Vector3 pt, GameCamera camera)
	{
		CameraSettings settings = camera.Settings;
		if (settings == null || !settings.hasBounds)
		{
			return pt;
		}
		if (camera.Camera == null)
		{
			return pt;
		}
		Game game = GameLogic.Get(true);
		if (game.subgames != null && game.subgames.Count > 0 && game.subgames[0].IsValid())
		{
			game = game.subgames[0];
		}
		Logic.PathFinding path_finding = game.path_finding;
		int num = 0;
		if (((path_finding != null) ? path_finding.settings : null) != null)
		{
			num = path_finding.settings.map_bounds_width;
		}
		Terrain activeTerrain = Terrain.activeTerrain;
		if (activeTerrain != null && activeTerrain.terrainData != null && activeTerrain.transform != null)
		{
			Transform transform = activeTerrain.transform;
			if (transform == null)
			{
				return pt;
			}
			Vector3 result = pt;
			float num2 = (float)num;
			float num3 = (float)num;
			if (pt.x - num2 < transform.position.x)
			{
				result.x = transform.position.x + num2;
			}
			if (pt.x + num2 > transform.position.x + activeTerrain.terrainData.size.x)
			{
				result.x = transform.position.x + activeTerrain.terrainData.size.x - num2;
			}
			if (pt.z - num3 < transform.position.z)
			{
				result.z = transform.position.z + num3;
			}
			if (pt.z + num3 > transform.position.z + activeTerrain.terrainData.size.z)
			{
				result.z = transform.position.z + activeTerrain.terrainData.size.z - num3;
			}
			return result;
		}
		else
		{
			MapData mapData = MapData.Get();
			if (mapData != null)
			{
				Bounds terrainBounds = mapData.GetTerrainBounds();
				Vector3 result2 = pt;
				float num4 = (float)num;
				float num5 = (float)num;
				if (pt.x - num4 < 0f)
				{
					result2.x = num4;
				}
				if (pt.x + num4 > terrainBounds.size.x)
				{
					result2.x = terrainBounds.size.x - num4;
				}
				if (pt.z - num5 < 0f)
				{
					result2.z = num5;
				}
				if (pt.z + num5 > terrainBounds.size.z)
				{
					result2.z = terrainBounds.size.z - num5;
				}
				return result2;
			}
			return pt;
		}
	}

	// Token: 0x06000ACC RID: 2764 RVA: 0x0007D2D0 File Offset: 0x0007B4D0
	public Vector3 GetLookAtPoint()
	{
		return this.ptLookAt;
	}

	// Token: 0x0400086C RID: 2156
	public const int mouse_botton_cnt = 6;

	// Token: 0x0400086D RID: 2157
	protected bool over_ui;

	// Token: 0x0400086E RID: 2158
	protected bool[] mouse_down_over_ui = new bool[6];

	// Token: 0x0400086F RID: 2159
	protected GameCamera owner;

	// Token: 0x04000870 RID: 2160
	public Vector3 ptLookAt;

	// Token: 0x04000871 RID: 2161
	public Vector3 ptLookAtPreHeight;

	// Token: 0x04000872 RID: 2162
	protected int mouse_btn_down = -1;

	// Token: 0x04000873 RID: 2163
	protected Vector3 ptsMouseDown = Vector3.zero;

	// Token: 0x04000874 RID: 2164
	protected Vector3[] mouse_down_pos = new Vector3[6];

	// Token: 0x04000875 RID: 2165
	protected Vector3 vScroll = Vector3.zero;

	// Token: 0x04000876 RID: 2166
	protected float cam_dist;

	// Token: 0x04000877 RID: 2167
	protected DT.Field active_cursor;

	// Token: 0x020005FD RID: 1533
	public struct InputKeys
	{
		// Token: 0x0400336C RID: 13164
		public bool yaw;

		// Token: 0x0400336D RID: 13165
		public bool yaw_left;

		// Token: 0x0400336E RID: 13166
		public bool yaw_right;

		// Token: 0x0400336F RID: 13167
		public bool pitch;

		// Token: 0x04003370 RID: 13168
		public bool zoom;
	}
}
