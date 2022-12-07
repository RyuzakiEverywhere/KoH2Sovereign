using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using Logic;
using UnityEngine;

// Token: 0x02000126 RID: 294
public class BattleViewAudioRenderer : MonoBehaviour
{
	// Token: 0x06000DA3 RID: 3491 RVA: 0x0009927B File Offset: 0x0009747B
	private void OnEnable()
	{
		if (!this.initialzied)
		{
			base.StartCoroutine(this.Setup());
		}
	}

	// Token: 0x06000DA4 RID: 3492 RVA: 0x00099292 File Offset: 0x00097492
	private IEnumerator Setup()
	{
		yield return null;
		this.BindListener();
		this.CalcChannels();
		this.SetupTerrain();
		this.Analyze();
		this.initialzied = true;
		yield break;
	}

	// Token: 0x06000DA5 RID: 3493 RVA: 0x000992A1 File Offset: 0x000974A1
	private void OnDisable()
	{
		this.MuteAllChannels();
	}

	// Token: 0x06000DA6 RID: 3494 RVA: 0x000992AC File Offset: 0x000974AC
	private void LateUpdate()
	{
		if (!this.hasData)
		{
			return;
		}
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

	// Token: 0x06000DA7 RID: 3495 RVA: 0x00099307 File Offset: 0x00097507
	public void ExtractData()
	{
		this.terrainTypeMap = new BattleViewAudioRenderer.TerrainTypeMap();
		if (this.terrain == null)
		{
			this.SetupTerrain();
		}
		this.terrainTypeMap.Build(this.terrain);
		this.hasData = true;
		this.Analyze();
	}

	// Token: 0x06000DA8 RID: 3496 RVA: 0x00099348 File Offset: 0x00097548
	private void BindListener()
	{
		if (this.AudioListener != null)
		{
			return;
		}
		this.AudioListener = global::Common.GetParentComponent<StudioListener>(base.gameObject);
		if (this.AudioListener == null && RuntimeManager.Listeners.Count > 0)
		{
			this.AudioListener = RuntimeManager.Listeners[0];
		}
	}

	// Token: 0x06000DA9 RID: 3497 RVA: 0x000993A4 File Offset: 0x000975A4
	public void CalcChannels()
	{
		for (int i = 0; i < this.channels.Count; i++)
		{
			global::Common.DestroyObj(this.channels[i].source.gameObject);
		}
		this.channels.Clear();
		DT.Field defField = global::Defs.GetDefField("BattleViewAudioRenderSettings", null);
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

	// Token: 0x06000DAA RID: 3498 RVA: 0x00099487 File Offset: 0x00097687
	private void SetupTerrain()
	{
		if (this.terrain == null)
		{
			this.terrain = Terrain.activeTerrain;
			this.terrain == null;
			return;
		}
	}

	// Token: 0x06000DAB RID: 3499 RVA: 0x000994B0 File Offset: 0x000976B0
	private Rect CalcAnalyzedRect()
	{
		float num = (float)Screen.width;
		float num2 = (float)Screen.height;
		BaseUI baseUI = BaseUI.Get();
		Vector3 terrainSize = baseUI.GetTerrainSize();
		Vector3 vector = CameraController.GameCamera.TransormToGroundPlane();
		Vector3 lhs = baseUI.ptLookAt;
		if (lhs == Vector3.zero || lhs == vector)
		{
			lhs = vector + CameraController.GameCamera.transform.rotation * new Vector3(50f, 0f, 0f);
		}
		Vector3 a = Vector3.Lerp(vector, baseUI.ptLookAt, 0.5f);
		Vector3 vector2 = a + new Vector3(-50f, 0f, 0f);
		Vector3 vector3 = a + new Vector3(50f, 0f, 0f);
		this.ClampToTrerrain(ref vector2, terrainSize);
		this.ClampToTrerrain(ref vector3, terrainSize);
		float num3 = vector3.x - vector2.x;
		float num4 = num3 * num2 / num;
		float num5 = (float)this.terrainTypeMap.width / this.terrain.terrainData.size.x;
		float num6 = (float)this.terrainTypeMap.height / this.terrain.terrainData.size.z;
		return new Rect(vector2.x * num5, vector2.z * num6, num3 * num5, num4 * num6);
	}

	// Token: 0x06000DAC RID: 3500 RVA: 0x00099618 File Offset: 0x00097818
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

	// Token: 0x06000DAD RID: 3501 RVA: 0x0009968C File Offset: 0x0009788C
	public void Analyze()
	{
		if (!this.hasData)
		{
			return;
		}
		if (this.AudioListener == null)
		{
			return;
		}
		if (this.channels == null)
		{
			return;
		}
		if (GameLogic.Get(true) == null)
		{
			return;
		}
		if (this.terrain == null || this.terrain.terrainData == null)
		{
			return;
		}
		if (this.terrainTypeMap == null)
		{
			return;
		}
		int count = this.channels.Count;
		for (int i = 0; i < count; i++)
		{
			global::AudioRenderer.Channel channel = this.channels[i];
			if (((channel != null) ? channel.source : null) == null)
			{
				return;
			}
			channel.balance = 0;
			channel.cnt = 0;
		}
		float cameraDistance = this.GetCameraDistance();
		int width = this.terrainTypeMap.width;
		int width2 = this.terrainTypeMap.width;
		Rect rect = this.CalcAnalyzedRect();
		this.currnetSampleRect = rect;
		int num = 0;
		while ((float)num < rect.width)
		{
			int num2 = 0;
			while ((float)num2 < rect.height)
			{
				int num3 = Mathf.Clamp(num + (int)rect.x, 0, width - 1);
				int num4 = Mathf.Clamp(num2 + (int)rect.y, 0, width2 - 1);
				TerrainType terrainType = (TerrainType)this.terrainTypeMap.type[num3, num4];
				ClimateZoneType climateZone = this.terrainTypeMap.climateZone;
				this.GetChannelIndex(terrainType, climateZone, this.m_tempCannelResults);
				for (int j = 0; j < this.m_tempCannelResults.Count; j++)
				{
					int index = this.m_tempCannelResults[j];
					global::AudioRenderer.Channel channel2 = this.channels[index];
					channel2.balance += (int)(-rect.width / 2f + rect.width * ((float)num / rect.width));
					channel2.cnt++;
				}
				num2++;
			}
			num++;
		}
		for (int k = 0; k < count; k++)
		{
			global::AudioRenderer.Channel channel3 = this.channels[k];
			PLAYBACK_STATE playback_STATE;
			channel3.source.EventInstance.getPlaybackState(out playback_STATE);
			channel3.source.EventInstance.setParameterByName("CameraDistance", cameraDistance, false);
			if (this.enableOverride)
			{
				channel3.source.EventInstance.setParameterByName("TerrainCoverage", channel3.overrideTerrainCoverage, false);
				channel3.source.EventInstance.setParameterByName("ScreenPosition", channel3.overrideScreenPosition, false);
				if (playback_STATE == PLAYBACK_STATE.STOPPED)
				{
					channel3.source.Play();
				}
			}
			else if (channel3.cnt <= 0)
			{
				channel3.source.EventInstance.setParameterByName("TerrainCoverage", 0f, false);
				channel3.source.EventInstance.setParameterByName("ScreenPosition", 0f, false);
			}
			else
			{
				if (playback_STATE == PLAYBACK_STATE.STOPPED)
				{
					channel3.source.Play();
				}
				float num5 = rect.width * rect.height;
				float value = (num5 > 0f) ? (Mathf.Min((float)channel3.cnt, num5) / num5) : 0f;
				float num6 = (float)channel3.balance / (float)channel3.cnt;
				channel3.balance_normal = num6 / (rect.width / 2f);
				channel3.source.EventInstance.setParameterByName("TerrainCoverage", value, false);
				channel3.source.EventInstance.setParameterByName("ScreenPosition", num6, false);
			}
		}
	}

	// Token: 0x06000DAE RID: 3502 RVA: 0x00099A3C File Offset: 0x00097C3C
	private void MuteAllChannels()
	{
		int count = this.channels.Count;
		for (int i = 0; i < count; i++)
		{
			global::AudioRenderer.Channel channel = this.channels[i];
			channel.source.EventInstance.setParameterByName("TerrainCoverage ", 0f, false);
			channel.source.EventInstance.setParameterByName("ScreenPosition", 0f, false);
		}
	}

	// Token: 0x06000DAF RID: 3503 RVA: 0x00099AAC File Offset: 0x00097CAC
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

	// Token: 0x06000DB0 RID: 3504 RVA: 0x00099B3D File Offset: 0x00097D3D
	private float GetCameraDistance()
	{
		if (this.AudioListener == null)
		{
			return 0f;
		}
		return Vector3.Distance(this.AudioListener.gameObject.transform.position, BaseUI.Get().ptLookAt);
	}

	// Token: 0x06000DB1 RID: 3505 RVA: 0x00099B77 File Offset: 0x00097D77
	public Rect GetCurrentSampleRect()
	{
		return this.currnetSampleRect;
	}

	// Token: 0x04000A6C RID: 2668
	public StudioListener AudioListener;

	// Token: 0x04000A6D RID: 2669
	private Terrain terrain;

	// Token: 0x04000A6E RID: 2670
	public BattleViewAudioRenderer.TerrainTypeMap terrainTypeMap;

	// Token: 0x04000A6F RID: 2671
	[HideInInspector]
	public List<global::AudioRenderer.Channel> channels = new List<global::AudioRenderer.Channel>();

	// Token: 0x04000A70 RID: 2672
	public bool enableOverride;

	// Token: 0x04000A71 RID: 2673
	private Vector3 lastCameraPosition;

	// Token: 0x04000A72 RID: 2674
	public float lastCamDistance;

	// Token: 0x04000A73 RID: 2675
	private bool initialzied;

	// Token: 0x04000A74 RID: 2676
	private bool hasData;

	// Token: 0x04000A75 RID: 2677
	public Rect currnetSampleRect;

	// Token: 0x04000A76 RID: 2678
	private List<int> m_tempCannelResults = new List<int>();

	// Token: 0x02000637 RID: 1591
	public class TerrainTypeMap
	{
		// Token: 0x06004730 RID: 18224 RVA: 0x00212D9C File Offset: 0x00210F9C
		public void Build(Terrain terrain)
		{
			this.width = (int)terrain.terrainData.size.x / this.cellSize;
			this.height = (int)terrain.terrainData.size.z / this.cellSize;
			this.type = new byte[this.width, this.height];
			this.climateZone = BattleMap.Get().climate_type;
			this.BuidlNodes();
			this.Calculate();
		}

		// Token: 0x06004731 RID: 18225 RVA: 0x00212E18 File Offset: 0x00211018
		private void BuidlNodes()
		{
			this.nodes = new BattleViewAudioRenderer.TerrainTypeMap.Node[this.width, this.height];
			for (int i = 0; i < this.height; i++)
			{
				for (int j = 0; j < this.width; j++)
				{
					this.nodes[j, i] = new BattleViewAudioRenderer.TerrainTypeMap.Node();
				}
			}
			Logic.PathFinding logic = BattleMap.Get().pf.logic;
			for (int k = 0; k < logic.data.height; k++)
			{
				int num = k * this.width / logic.data.height;
				for (int l = 0; l < logic.data.width; l++)
				{
					PathData.Node node = logic.data.GetNode(l, k);
					int num2 = l * this.width / logic.data.width;
					BattleViewAudioRenderer.TerrainTypeMap.Node node2 = this.nodes[num2, num];
					if (node.town)
					{
						node2.Add(TerrainType.Town, true);
					}
					else if (node.ocean)
					{
						node2.Add(TerrainType.Ocean, true);
					}
					else
					{
						node2.Add(TerrainType.Plains, true);
					}
				}
			}
			Logic.Battle battle = BattleMap.battle;
			byte[,] tree_count_grid = battle.tree_count_grid;
			int tree_count_grid_width = battle.tree_count_grid_width;
			int tree_count_grid_height = battle.tree_count_grid_height;
			for (int m = 0; m < this.width; m++)
			{
				int num3 = m * tree_count_grid_width / this.width;
				for (int n = 0; n < this.height; n++)
				{
					int num4 = n * tree_count_grid_height / this.width;
					if (tree_count_grid[num4, num3] != 0)
					{
						this.nodes[n, m].Add(TerrainType.Forest, true);
					}
				}
			}
		}

		// Token: 0x06004732 RID: 18226 RVA: 0x00212FC4 File Offset: 0x002111C4
		private void Calculate()
		{
			for (int i = 0; i < this.height; i++)
			{
				for (int j = 0; j < this.width; j++)
				{
					BattleViewAudioRenderer.TerrainTypeMap.Node node = this.nodes[j, i];
					for (int k = 7; k > 0; k--)
					{
						if (node.counts[k] > 0)
						{
							int num = node.totals[k];
							node.type = (TerrainType)k;
							break;
						}
					}
					this.type[j, i] = (byte)node.type;
				}
			}
		}

		// Token: 0x040034A0 RID: 13472
		public int cellSize = 5;

		// Token: 0x040034A1 RID: 13473
		public byte[,] type;

		// Token: 0x040034A2 RID: 13474
		public ClimateZoneType climateZone;

		// Token: 0x040034A3 RID: 13475
		public int width;

		// Token: 0x040034A4 RID: 13476
		public int height;

		// Token: 0x040034A5 RID: 13477
		private BattleViewAudioRenderer.TerrainTypeMap.Node[,] nodes;

		// Token: 0x020009F6 RID: 2550
		private class Node
		{
			// Token: 0x06005511 RID: 21777 RVA: 0x0024846D File Offset: 0x0024666D
			public void Add(TerrainType type, bool present)
			{
				this.totals[(int)type]++;
				if (present)
				{
					this.counts[(int)type]++;
				}
			}

			// Token: 0x04004600 RID: 17920
			public TerrainType type;

			// Token: 0x04004601 RID: 17921
			public int[] counts = new int[8];

			// Token: 0x04004602 RID: 17922
			public int[] totals = new int[8];
		}
	}
}
