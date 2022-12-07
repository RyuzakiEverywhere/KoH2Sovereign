using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using Logic;
using UnityEngine;

// Token: 0x02000124 RID: 292
public class AudioRenderer : MonoBehaviour
{
	// Token: 0x06000D6D RID: 3437 RVA: 0x0009735E File Offset: 0x0009555E
	public static global::AudioRenderer Get()
	{
		return global::AudioRenderer.instance;
	}

	// Token: 0x06000D6E RID: 3438 RVA: 0x00097365 File Offset: 0x00095565
	private void OnEnable()
	{
		global::AudioRenderer.instance = this;
		if (!this.initialzied)
		{
			base.StartCoroutine(this.Setup());
		}
	}

	// Token: 0x06000D6F RID: 3439 RVA: 0x00097382 File Offset: 0x00095582
	private void OnDisable()
	{
		this.MuteAllChannels();
		this.StopAllChannels();
		global::AudioRenderer.instance = null;
	}

	// Token: 0x06000D70 RID: 3440 RVA: 0x00097396 File Offset: 0x00095596
	private IEnumerator Setup()
	{
		yield return null;
		this.CalcChannels();
		this.SetupTerrain();
		this.Analyze();
		this.initialzied = true;
		yield break;
	}

	// Token: 0x06000D71 RID: 3441 RVA: 0x000973A8 File Offset: 0x000955A8
	private void LateUpdate()
	{
		if (this.terrain == null)
		{
			this.SetupTerrain();
		}
		Vector3 position = CameraController.MainCamera.transform.position;
		if (Vector3.Distance(this.lastCameraPosition, position) < 1f)
		{
			return;
		}
		this.lastCameraPosition = position;
		this.Analyze();
	}

	// Token: 0x06000D72 RID: 3442 RVA: 0x000973FC File Offset: 0x000955FC
	public void CalcChannels()
	{
		for (int i = 0; i < this.channels.Count; i++)
		{
			global::Common.DestroyObj(this.channels[i].source.gameObject);
		}
		this.channels.Clear();
		DT.Field defField = global::Defs.GetDefField("AudioRenderSettings", null);
		if (defField != null)
		{
			DT.Field field = defField.FindChild("ambient_channels", null, true, true, true, '.');
			if (field != null)
			{
				List<DT.Field> list = field.Children();
				for (int j = 0; j < list.Count; j++)
				{
					DT.Field field2 = list[j];
					if (!string.IsNullOrEmpty(field2.key))
					{
						global::AudioRenderer.Channel channel = new global::AudioRenderer.Channel();
						channel.CreateEmmiter(field2.key, field2.GetString("event", null, "", true, true, true, '.'), base.gameObject);
						this.channels.Add(channel);
					}
				}
			}
		}
	}

	// Token: 0x06000D73 RID: 3443 RVA: 0x000974DF File Offset: 0x000956DF
	private void SetupTerrain()
	{
		this.terrain = base.GetComponent<Terrain>();
		if (this.terrain == null)
		{
			this.terrain = Terrain.activeTerrain;
			this.terrain == null;
			return;
		}
	}

	// Token: 0x06000D74 RID: 3444 RVA: 0x00097514 File Offset: 0x00095714
	private Rect CalcAnalyzedRect()
	{
		float num = (float)Screen.width;
		float num2 = (float)Screen.height;
		BaseUI baseUI = BaseUI.Get();
		Vector3 terrainSize = baseUI.GetTerrainSize();
		Vector3 vector = baseUI.ScreenToGroundPlane(0f, num2);
		Vector3 vector2 = baseUI.ScreenToGroundPlane(num, num2);
		Vector3 vector3 = baseUI.ScreenToGroundPlane(0f, 0f);
		Vector3 vector4 = baseUI.ScreenToGroundPlane(num, 0f);
		this.ClampToTrerrain(ref vector, terrainSize);
		this.ClampToTrerrain(ref vector2, terrainSize);
		this.ClampToTrerrain(ref vector3, terrainSize);
		this.ClampToTrerrain(ref vector4, terrainSize);
		Game game = GameLogic.Get(true);
		int length = game.terrain_types.data.GetLength(0);
		float length2 = (float)game.terrain_types.data.GetLength(1);
		float num3 = (float)length / this.terrain.terrainData.size.x;
		float num4 = length2 / this.terrain.terrainData.size.z;
		float num5 = vector2.x - vector.x;
		float num6 = num5 * num2 / num;
		Rect result = new Rect(vector.x * num3, vector.z * num4 - num6 * num4, num5 * num3, num6 * num4);
		this.viewProjection[0].Set(vector.x * num3, vector.z * num4);
		this.viewProjection[1].Set(vector2.x * num3, vector2.z * num4);
		this.viewProjection[2].Set(vector4.x * num3, vector4.z * num4);
		this.viewProjection[3].Set(vector3.x * num3, vector3.z * num4);
		return result;
	}

	// Token: 0x06000D75 RID: 3445 RVA: 0x000976C4 File Offset: 0x000958C4
	private void ClampToTrerrain(ref Vector3 o, Vector3 size)
	{
		if (o.x < 0f)
		{
			o.x = 0f;
		}
		if (o.x > size.x)
		{
			o.x = size.x;
		}
		if (o.z < 0f)
		{
			o.z = 0f;
		}
		if (o.z > size.z)
		{
			o.z = size.z;
		}
	}

	// Token: 0x06000D76 RID: 3446 RVA: 0x00097738 File Offset: 0x00095938
	public void Analyze()
	{
		if (this.channels == null)
		{
			return;
		}
		if (this.terrain == null || this.terrain.terrainData == null)
		{
			return;
		}
		Game game = GameLogic.Get(true);
		if (game == null)
		{
			return;
		}
		TerrainTypesInfo terrain_types = game.terrain_types;
		ClimateZoneInfo climate_zones = game.climate_zones;
		if (terrain_types == null)
		{
			return;
		}
		int length = game.terrain_types.data.GetLength(0);
		int length2 = game.terrain_types.data.GetLength(1);
		if (this.AudioListener == null)
		{
			this.AudioListener = UnityEngine.Object.FindObjectOfType<StudioListener>();
			if (this.AudioListener == null)
			{
				return;
			}
		}
		bool flag = ViewMode.IsPoliticalView();
		int count = this.channels.Count;
		for (int i = 0; i < count; i++)
		{
			global::AudioRenderer.Channel channel = this.channels[i];
			channel.balance = 0;
			channel.cnt = 0;
		}
		this.lastCamDistance = this.GetCameraDistance();
		if (flag)
		{
			this.MuteAllChannels();
			return;
		}
		Rect rect = this.CalcAnalyzedRect();
		this.currnetSampleRect = rect;
		this.maxVisiblePixels = 0f;
		int num = 0;
		while ((float)num < rect.width)
		{
			int num2 = 0;
			while ((float)num2 < rect.height)
			{
				int num3 = Mathf.Clamp(num + (int)rect.x, 0, length - 1);
				int num4 = Mathf.Clamp(num2 + (int)rect.y, 0, length2 - 1);
				TerrainType terrainType = (TerrainType)terrain_types.data[num3, num4];
				ClimateZoneType climateZoneType = (ClimateZoneType)climate_zones.data[num3, num4];
				if (global::AudioRenderer.IsInsidePoligon(this.viewProjection, new Vector2((float)num3, (float)num4)))
				{
					this.maxVisiblePixels += 1f;
					this.GetChannelIndex(terrainType, climateZoneType, this.m_tempCannelResults);
					for (int j = 0; j < this.m_tempCannelResults.Count; j++)
					{
						int index = this.m_tempCannelResults[j];
						global::AudioRenderer.Channel channel2 = this.channels[index];
						channel2.balance += (int)(-rect.width / 2f + rect.width * ((float)num / rect.width));
						channel2.cnt++;
					}
				}
				num2++;
			}
			num++;
		}
		for (int k = 0; k < count; k++)
		{
			global::AudioRenderer.Channel channel3 = this.channels[k];
			PLAYBACK_STATE playback_STATE = PLAYBACK_STATE.STOPPED;
			if (channel3.source.EventInstance.isValid())
			{
				channel3.source.EventInstance.getPlaybackState(out playback_STATE);
			}
			if (this.enableOverride)
			{
				if (playback_STATE == PLAYBACK_STATE.STOPPED)
				{
					channel3.source.Play();
				}
				if (channel3.source.EventInstance.isValid())
				{
					channel3.source.EventInstance.setParameterByName("CameraDistance", this.lastCamDistance, false);
					channel3.source.EventInstance.setParameterByName("TerrainCoverage", channel3.overrideTerrainCoverage, false);
					channel3.source.EventInstance.setParameterByName("ScreenPosition", channel3.overrideScreenPosition, false);
				}
			}
			else if (channel3.cnt <= 0)
			{
				if (channel3.source.EventInstance.isValid())
				{
					channel3.source.EventInstance.setParameterByName("CameraDistance", this.lastCamDistance, false);
					channel3.source.EventInstance.setParameterByName("TerrainCoverage", 0f, false);
					channel3.source.EventInstance.setParameterByName("ScreenPosition", 0f, false);
				}
			}
			else
			{
				if (playback_STATE == PLAYBACK_STATE.STOPPED)
				{
					channel3.source.Play();
				}
				if (channel3.source.EventInstance.isValid())
				{
					float value = (this.maxVisiblePixels > 0f) ? (Mathf.Min((float)channel3.cnt, this.maxVisiblePixels) / this.maxVisiblePixels) : 0f;
					float num5 = (float)channel3.balance / (float)channel3.cnt;
					channel3.balance_normal = num5 / (rect.width / 2f);
					channel3.source.EventInstance.setParameterByName("CameraDistance", this.lastCamDistance, false);
					channel3.source.EventInstance.setParameterByName("TerrainCoverage", value, false);
					channel3.source.EventInstance.setParameterByName("ScreenPosition", num5, false);
				}
			}
		}
	}

	// Token: 0x06000D77 RID: 3447 RVA: 0x00097BF4 File Offset: 0x00095DF4
	public void MuteAllChannels()
	{
		int i = 0;
		int count = this.channels.Count;
		while (i < count)
		{
			global::AudioRenderer.Channel channel = this.channels[i];
			if (channel.source.EventInstance.isValid())
			{
				channel.source.EventInstance.setParameterByName("TerrainCoverage ", 0f, false);
				channel.source.EventInstance.setParameterByName("ScreenPosition", 0f, false);
			}
			i++;
		}
	}

	// Token: 0x06000D78 RID: 3448 RVA: 0x00097C7C File Offset: 0x00095E7C
	public void StopAllChannels()
	{
		int i = 0;
		int count = this.channels.Count;
		while (i < count)
		{
			this.channels[i].source.Stop();
			i++;
		}
	}

	// Token: 0x06000D79 RID: 3449 RVA: 0x00097CB8 File Offset: 0x00095EB8
	private void GetChannelIndex(TerrainType terrainType, ClimateZoneType climateZoneType, List<int> result)
	{
		int count = this.channels.Count;
		result.Clear();
		for (int i = 0; i < count; i++)
		{
			global::AudioRenderer.Channel channel = this.channels[i];
			if (channel.terrainType == terrainType && channel.climateZoneType == climateZoneType)
			{
				result.Add(i);
				break;
			}
		}
		for (int j = 0; j < count; j++)
		{
			global::AudioRenderer.Channel channel2 = this.channels[j];
			if (channel2.terrainType == terrainType)
			{
				result.Add(j);
				return;
			}
			if (channel2.climateZoneType == climateZoneType)
			{
				result.Add(j);
				return;
			}
		}
	}

	// Token: 0x06000D7A RID: 3450 RVA: 0x00097D49 File Offset: 0x00095F49
	private float GetCameraDistance()
	{
		if (this.AudioListener == null)
		{
			return 0f;
		}
		return Vector3.Distance(this.AudioListener.gameObject.transform.position, BaseUI.Get().ptLookAt);
	}

	// Token: 0x06000D7B RID: 3451 RVA: 0x00097D83 File Offset: 0x00095F83
	public Rect GetCurrentSampleRect()
	{
		return this.currnetSampleRect;
	}

	// Token: 0x06000D7C RID: 3452 RVA: 0x00097D8C File Offset: 0x00095F8C
	private void DrawRect()
	{
		Rect currentSampleRect = this.GetCurrentSampleRect();
		int num = 10;
		Vector3 vector = new Vector3(currentSampleRect.x * (float)num, 20f, currentSampleRect.y * (float)num);
		Vector3 vector2 = new Vector3(currentSampleRect.x * (float)num + currentSampleRect.width * (float)num, 20f, currentSampleRect.y * (float)num);
		Vector3 vector3 = new Vector3(currentSampleRect.x * (float)num + currentSampleRect.width * (float)num, 20f, currentSampleRect.y * (float)num + currentSampleRect.height * (float)num);
		Vector3 vector4 = new Vector3(currentSampleRect.x * (float)num, 20f, currentSampleRect.y * (float)num + currentSampleRect.height * (float)num);
		Debug.DrawLine(vector, vector2, Color.cyan);
		Debug.DrawLine(vector2, vector3, Color.cyan);
		Debug.DrawLine(vector3, vector4, Color.cyan);
		Debug.DrawLine(vector4, vector, Color.cyan);
	}

	// Token: 0x06000D7D RID: 3453 RVA: 0x00097E84 File Offset: 0x00096084
	private void DrawProjection()
	{
		if (this.viewProjection == null)
		{
			return;
		}
		Vector2 vector = this.viewProjection[0];
		float num = 10f;
		for (int i = 1; i < this.viewProjection.Length + 1; i++)
		{
			Vector3 vector2 = (i < this.viewProjection.Length) ? this.viewProjection[i] : this.viewProjection[0];
			Vector3 start = new Vector3(vector.x * num, 20f, vector.y * num);
			Vector3 end = new Vector3(vector2.x * num, 20f, vector2.y * num);
			Debug.DrawLine(start, end, Color.magenta);
			vector = vector2;
		}
	}

	// Token: 0x06000D7E RID: 3454 RVA: 0x00097F3C File Offset: 0x0009613C
	private static bool IsInsidePoligon(Vector2[] poly, Vector2 pnt)
	{
		int num = poly.Length;
		bool flag = false;
		int i = 0;
		int num2 = num - 1;
		while (i < num)
		{
			if (poly[i].y > pnt.y != poly[num2].y > pnt.y && pnt.x < (poly[num2].x - poly[i].x) * (pnt.y - poly[i].y) / (poly[num2].y - poly[i].y) + poly[i].x)
			{
				flag = !flag;
			}
			num2 = i++;
		}
		return flag;
	}

	// Token: 0x04000A4F RID: 2639
	public StudioListener AudioListener;

	// Token: 0x04000A50 RID: 2640
	private Terrain terrain;

	// Token: 0x04000A51 RID: 2641
	[HideInInspector]
	public List<global::AudioRenderer.Channel> channels = new List<global::AudioRenderer.Channel>();

	// Token: 0x04000A52 RID: 2642
	private Vector3 lastCameraPosition;

	// Token: 0x04000A53 RID: 2643
	public float lastCamDistance;

	// Token: 0x04000A54 RID: 2644
	public bool enableOverride;

	// Token: 0x04000A55 RID: 2645
	public float maxVisiblePixels;

	// Token: 0x04000A56 RID: 2646
	public Rect currnetSampleRect;

	// Token: 0x04000A57 RID: 2647
	public Vector2[] viewProjection = new Vector2[4];

	// Token: 0x04000A58 RID: 2648
	private bool initialzied;

	// Token: 0x04000A59 RID: 2649
	private static global::AudioRenderer instance;

	// Token: 0x04000A5A RID: 2650
	private List<int> m_tempCannelResults = new List<int>();

	// Token: 0x02000632 RID: 1586
	public class Channel
	{
		// Token: 0x06004721 RID: 18209 RVA: 0x00212A74 File Offset: 0x00210C74
		public void CreateEmmiter(string name, string eventKey, GameObject host)
		{
			this.layer = name;
			GameObject gameObject = new GameObject(name);
			gameObject.hideFlags = HideFlags.DontSaveInEditor;
			gameObject.transform.SetParent(host.transform);
			this.source = gameObject.AddComponent<StudioEventEmitter>();
			this.source.Event = eventKey;
			this.source.PlayEvent = EmitterGameEvent.None;
			this.source.StopEvent = EmitterGameEvent.ObjectDestroy;
			for (int i = 0; i < 8; i++)
			{
				TerrainType terrainType = (TerrainType)i;
				if (name.Contains(terrainType.ToString()))
				{
					this.terrainType = terrainType;
				}
			}
			for (int j = 0; j < 5; j++)
			{
				ClimateZoneType climateZoneType = (ClimateZoneType)j;
				if (name.Contains(climateZoneType.ToString()))
				{
					this.climateZoneType = climateZoneType;
				}
			}
		}

		// Token: 0x0400347E RID: 13438
		public StudioEventEmitter source;

		// Token: 0x0400347F RID: 13439
		public string layer = "";

		// Token: 0x04003480 RID: 13440
		public int balance;

		// Token: 0x04003481 RID: 13441
		public float balance_normal;

		// Token: 0x04003482 RID: 13442
		public int cnt;

		// Token: 0x04003483 RID: 13443
		public float overrideTerrainCoverage;

		// Token: 0x04003484 RID: 13444
		public float overrideScreenPosition;

		// Token: 0x04003485 RID: 13445
		public TerrainType terrainType = TerrainType.COUNT;

		// Token: 0x04003486 RID: 13446
		public ClimateZoneType climateZoneType = ClimateZoneType.COUNT;
	}
}
