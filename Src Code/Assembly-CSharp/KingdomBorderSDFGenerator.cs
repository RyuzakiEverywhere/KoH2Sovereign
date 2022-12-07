using System;
using System.Collections.Generic;
using System.IO;
using Logic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000114 RID: 276
public class KingdomBorderSDFGenerator
{
	// Token: 0x170000A3 RID: 163
	// (get) Token: 0x06000C9A RID: 3226 RVA: 0x0008C3E4 File Offset: 0x0008A5E4
	private List<global::Realm> Realms
	{
		get
		{
			WorldMap worldMap = WorldMap.Get();
			if (worldMap != null)
			{
				return worldMap.Realms;
			}
			TitleMap titleMap = TitleMap.Get();
			if (titleMap != null)
			{
				return titleMap.Realms;
			}
			return null;
		}
	}

	// Token: 0x06000C9B RID: 3227 RVA: 0x0008C420 File Offset: 0x0008A620
	private void LoadSettings()
	{
		global::Defs defs = global::Defs.Get(false);
		DT.Field field;
		if (defs == null)
		{
			field = null;
		}
		else
		{
			DT dt = defs.dt;
			field = ((dt != null) ? dt.Find("SDFSettings", null) : null);
		}
		DT.Field field2 = field;
		if (field2 == null)
		{
			return;
		}
		this.sdf_tex_res = field2.GetInt("sdf_tex_res", null, this.sdf_tex_res, true, true, true, '.');
		this.sdf_kingdom_scan_dist = field2.GetInt("sdf_kingdom_scan_dist", null, this.sdf_tex_res, true, true, true, '.');
		this.sdf_realm_scan_dist = field2.GetInt("sdf_realm_scan_dist", null, this.sdf_tex_res, true, true, true, '.');
	}

	// Token: 0x06000C9C RID: 3228 RVA: 0x0008C4B0 File Offset: 0x0008A6B0
	public void Init(bool force_reload = false)
	{
		bool flag = TitleMap.Get() != null;
		if (this.init && !flag && !force_reload)
		{
			return;
		}
		this.LoadSettings();
		if (this.k_border_cs == null)
		{
			this.k_border_cs = Resources.Load<ComputeShader>("Compute/BorderSDF");
		}
		if (force_reload || this.pos_rots_binary == null || this.pos_rots_binary.Count == 0)
		{
			Game game = GameLogic.Get(false);
			this.LoadPoints((game != null) ? game.game : null);
		}
		if (this.pos_rots_binary == null)
		{
			return;
		}
		this.compute_data = new LinesBatching.LineComputeData[this.pos_rots_binary.Count];
		this.segments.Clear();
		float num = 0f;
		LinesBatching.LineSegment item = default(LinesBatching.LineSegment);
		bool flag2 = true;
		BordersBatching.RealmBorders realmBorders = null;
		int num2 = -1;
		global::Realm realm = null;
		global::Realm realm2 = null;
		this.buff_idx = 0;
		this.realmBorders.Clear();
		for (int i = 0; i < this.pos_rots_binary.Count; i++)
		{
			float3 @float = this.pos_rots_binary[i];
			if (flag2)
			{
				if (@float.x != -1f)
				{
					int num3 = (int)@float.x;
					num2 = (int)@float.y;
					realm = global::Realm.Get(num3);
					realm2 = global::Realm.Get(num2);
					if (realm != null && realm2 != null)
					{
						if (!this.realmBorders.TryGetValue(num3, out realmBorders))
						{
							realmBorders = new BordersBatching.RealmBorders();
							realmBorders.r = num3;
							this.realmBorders[num3] = realmBorders;
						}
						item = default(LinesBatching.LineSegment);
						item.start_idx = this.buff_idx;
						flag2 = false;
					}
				}
			}
			else
			{
				int prev_idx = this.buff_idx - 1;
				if (i == this.pos_rots_binary.Count - 1 && !flag2)
				{
					@float = new float3(-1f, -1f, -1f);
				}
				if (@float.x == -1f)
				{
					if (!flag2)
					{
						item.end_idx = this.buff_idx;
					}
					num = 0f;
					flag2 = true;
					this.segments.Add(item);
					BordersBatching.RealmBorders.Border border = null;
					if (!realmBorders.borders.TryGetValue(num2, out border))
					{
						border = new BordersBatching.RealmBorders.Border();
						realmBorders.borders[num2] = border;
					}
					border.segments.Add(item);
				}
				if (realm != null && realm2 != null)
				{
					float3 v = @float;
					LinesBatching.LineComputeData lineComputeData = new LinesBatching.LineComputeData(i, v, 0f, false, prev_idx, this.buff_idx + 1);
					this.compute_data[this.buff_idx] = lineComputeData;
					if (this.buff_idx > 0 && @float.x != -1f)
					{
						LinesBatching.LineComputeData lineComputeData2 = this.compute_data[this.buff_idx - 1];
						num += Vector3.Distance(lineComputeData2.pos, v);
						if (num > this.max_path_len)
						{
							num = this.max_path_len;
						}
					}
					this.buff_idx++;
				}
			}
		}
		this.init = true;
	}

	// Token: 0x06000C9D RID: 3229 RVA: 0x0008C7A4 File Offset: 0x0008A9A4
	public bool LoadPoints(Game game = null)
	{
		string text;
		if (game != null)
		{
			text = game.map_name;
		}
		else
		{
			text = Path.GetFileNameWithoutExtension(SceneManager.GetActiveScene().path).ToLowerInvariant();
		}
		string path = string.Concat(new string[]
		{
			Game.maps_path,
			text,
			"/",
			this.dir,
			".bin"
		});
		if (!File.Exists(path))
		{
			return false;
		}
		byte[] array = File.ReadAllBytes(path);
		int num = array.Length / 4;
		this.pos_rots_binary = new List<float3>(num / 3);
		float[] array2 = Serialization.ToArray<float>(array, num);
		for (int i = 0; i < num; i += 3)
		{
			this.pos_rots_binary.Add(new float3(array2[i], array2[i + 1], array2[i + 2]));
		}
		return this.pos_rots_binary.Count > 0;
	}

	// Token: 0x06000C9E RID: 3230 RVA: 0x0008C870 File Offset: 0x0008AA70
	public void StitchBorders()
	{
		if (this.compute_data == null)
		{
			return;
		}
		foreach (KeyValuePair<int, BordersBatching.RealmBorders> keyValuePair in this.realmBorders)
		{
			BordersBatching.RealmBorders value = keyValuePair.Value;
			int r = value.r;
			foreach (KeyValuePair<int, BordersBatching.RealmBorders.Border> keyValuePair2 in value.borders)
			{
				BordersBatching.RealmBorders.Border value2 = keyValuePair2.Value;
				int key = keyValuePair2.Key;
				if (value2.enabled)
				{
					foreach (LinesBatching.LineSegment lineSegment in value2.segments)
					{
						float num = float.MaxValue;
						LinesBatching.LineComputeData lineComputeData = this.compute_data[lineSegment.start_idx];
						int num2 = lineComputeData.prev_idx;
						LinesBatching.LineSegment lineSegment2 = default(LinesBatching.LineSegment);
						bool flag = false;
						foreach (KeyValuePair<int, BordersBatching.RealmBorders.Border> keyValuePair3 in value.borders)
						{
							int key2 = keyValuePair3.Key;
							if (key2 != key)
							{
								BordersBatching.RealmBorders.Border value3 = keyValuePair3.Value;
								if (value3.enabled)
								{
									using (List<LinesBatching.LineSegment>.Enumerator enumerator5 = value3.segments.GetEnumerator())
									{
										while (enumerator5.MoveNext())
										{
											LinesBatching.LineSegment lineSegment3 = enumerator5.Current;
											if (lineSegment3.end_idx != 0)
											{
												LinesBatching.LineComputeData lineComputeData2 = this.compute_data[lineSegment3.end_idx - 1];
												float num3 = math.distance(lineComputeData.pos, lineComputeData2.pos);
												if (num3 < num)
												{
													num2 = lineSegment3.end_idx - 1;
													num = num3;
													lineSegment2 = lineSegment3;
													flag = true;
												}
											}
										}
										continue;
									}
								}
								BordersBatching.RealmBorders realmBorders = null;
								if (this.realmBorders.TryGetValue(key2, out realmBorders))
								{
									foreach (KeyValuePair<int, BordersBatching.RealmBorders.Border> keyValuePair4 in realmBorders.borders)
									{
										if (keyValuePair4.Value.enabled && keyValuePair4.Key != r)
										{
											foreach (LinesBatching.LineSegment lineSegment4 in keyValuePair4.Value.segments)
											{
												LinesBatching.LineComputeData lineComputeData3 = this.compute_data[lineSegment4.end_idx - 1];
												float num4 = math.distance(lineComputeData.pos, lineComputeData3.pos);
												if (num4 < num)
												{
													num2 = lineSegment4.end_idx - 1;
													num = num4;
													lineSegment2 = lineSegment4;
													flag = true;
												}
											}
										}
									}
								}
							}
						}
						if (num < 10f && flag)
						{
							int num5 = math.min(3, lineSegment2.end_idx - lineSegment2.start_idx);
							for (int i = lineSegment2.end_idx - num5; i < lineSegment2.end_idx; i++)
							{
								LinesBatching.LineComputeData lineComputeData4 = this.compute_data[i];
								lineComputeData4.enabled = 0f;
								this.compute_data[i] = lineComputeData4;
							}
							num2 = lineSegment2.end_idx - num5 - 1;
							num5 = math.min(3, lineSegment.end_idx - lineSegment.start_idx);
							for (int j = lineSegment.start_idx; j < lineSegment.start_idx + num5; j++)
							{
								LinesBatching.LineComputeData lineComputeData5 = this.compute_data[j];
								lineComputeData5.enabled = 0f;
								this.compute_data[j] = lineComputeData5;
							}
							lineComputeData = this.compute_data[lineSegment.start_idx + num5];
							lineComputeData.prev_idx = num2;
							this.compute_data[lineSegment.start_idx + num5] = lineComputeData;
							lineComputeData = this.compute_data[num2];
							lineComputeData.next_idx = lineSegment.start_idx + num5;
							this.compute_data[num2] = lineComputeData;
						}
					}
				}
			}
		}
	}

	// Token: 0x06000C9F RID: 3231 RVA: 0x0008CD44 File Offset: 0x0008AF44
	public void UpdateAllRealmBorders()
	{
		for (int i = 1; i <= this.Realms.Count; i++)
		{
			global::Realm r = this.Realms[i - 1];
			this.UpdateRealmBorders(r);
		}
	}

	// Token: 0x06000CA0 RID: 3232 RVA: 0x0008CD80 File Offset: 0x0008AF80
	public void SetActiveBorder(global::Realm r1, global::Realm r2, bool enabled, bool enabled_water)
	{
		if (r1 == null || r2 == null || this.compute_data == null)
		{
			return;
		}
		BordersBatching.RealmBorders realmBorders = null;
		if (!this.realmBorders.TryGetValue(r1.id, out realmBorders))
		{
			return;
		}
		BordersBatching.RealmBorders.Border border = null;
		if (!realmBorders.borders.TryGetValue(r2.id, out border))
		{
			return;
		}
		if (((border != null) ? border.segments : null) == null)
		{
			return;
		}
		if (border.enabled == enabled && border.enabled_water == enabled_water)
		{
			return;
		}
		border.enabled = (enabled && !enabled_water);
		border.enabled_water = enabled_water;
		for (int i = 0; i < border.segments.Count; i++)
		{
			border.segments[i].SetActive(enabled, this.compute_data);
		}
	}

	// Token: 0x06000CA1 RID: 3233 RVA: 0x0008CE3C File Offset: 0x0008B03C
	public void UpdateRealmBorders(global::Realm r)
	{
		if (r.IsSeaRealm())
		{
			return;
		}
		global::Kingdom kingdom = r.GetKingdom();
		foreach (global::Realm.Neighbor neighbor in r.Neighbors)
		{
			global::Realm realm = global::Realm.Get(neighbor.rid);
			if (realm != null)
			{
				bool enabled_water = realm.IsSeaRealm();
				global::Kingdom kingdom2 = realm.GetKingdom();
				bool flag = kingdom2 != null && kingdom2.id != kingdom.id;
				this.SetActiveBorder(r, realm, flag || this.include_realms, enabled_water);
			}
		}
	}

	// Token: 0x06000CA2 RID: 3234 RVA: 0x0008CEE8 File Offset: 0x0008B0E8
	public static Texture2D Rebuild(Texture2D old_tex, int channel, bool include_realms, string save_path = null, bool force_reload = false)
	{
		Texture2D result;
		using (Game.Profile("KingdomBorderSDFGenerator.Rebuild", false, 0f, null))
		{
			if (KingdomBorderSDFGenerator.instance == null)
			{
				KingdomBorderSDFGenerator.instance = new KingdomBorderSDFGenerator();
			}
			KingdomBorderSDFGenerator.instance.old_tex = old_tex;
			KingdomBorderSDFGenerator.instance.channel = channel;
			KingdomBorderSDFGenerator.instance.save_path = save_path;
			KingdomBorderSDFGenerator.instance.include_realms = include_realms;
			KingdomBorderSDFGenerator.instance.Init(force_reload);
			if (!KingdomBorderSDFGenerator.instance.init)
			{
				result = old_tex;
			}
			else
			{
				if (channel != 0)
				{
					if (channel != 1)
					{
					}
					KingdomBorderSDFGenerator.instance.max_circle = KingdomBorderSDFGenerator.instance.sdf_kingdom_scan_dist;
				}
				else
				{
					KingdomBorderSDFGenerator.instance.max_circle = KingdomBorderSDFGenerator.instance.sdf_realm_scan_dist;
				}
				KingdomBorderSDFGenerator.instance.border_tex_res = KingdomBorderSDFGenerator.instance.sdf_tex_res;
				KingdomBorderSDFGenerator.instance.UpdateAllRealmBorders();
				if (!include_realms)
				{
					KingdomBorderSDFGenerator.instance.StitchBorders();
				}
				result = KingdomBorderSDFGenerator.instance.RebuildKingdomBorderSDF();
			}
		}
		return result;
	}

	// Token: 0x06000CA3 RID: 3235 RVA: 0x0008CFEC File Offset: 0x0008B1EC
	public Texture2D RebuildKingdomBorderSDF()
	{
		if (this.k_border_cs == null)
		{
			return null;
		}
		List<LinesBatching.LineSegment> list = new List<LinesBatching.LineSegment>();
		foreach (KeyValuePair<int, BordersBatching.RealmBorders> keyValuePair in this.realmBorders)
		{
			foreach (KeyValuePair<int, BordersBatching.RealmBorders.Border> keyValuePair2 in keyValuePair.Value.borders)
			{
				if (keyValuePair2.Value.enabled || keyValuePair2.Value.enabled_water)
				{
					list.AddRange(keyValuePair2.Value.segments);
				}
			}
		}
		if (this.k_border_tex == null)
		{
			this.k_border_tex = RenderTexture.GetTemporary(this.border_tex_res, this.border_tex_res, 0, RenderTextureFormat.ARGB32);
			this.k_border_tex.useMipMap = false;
			this.k_border_tex.enableRandomWrite = true;
			this.k_border_tex.Create();
		}
		if (this.generate_sdf_kernel == -1)
		{
			this.generate_sdf_kernel = this.k_border_cs.FindKernel("GenerateSDF");
		}
		int num = (int)Mathf.Sqrt((float)this.border_tex_res);
		this.border_segments = new List<float4>[num * num];
		LinesBatching.LineComputeData[] array = this.compute_data;
		int num2 = array.Length;
		if (this.pixel_ids_arr == null)
		{
			this.pixel_ids_arr = new float2[num * num];
		}
		if (this.pixel_ids_buffer == null)
		{
			this.pixel_ids_buffer = new ComputeBuffer(num * num, sizeof(float2));
		}
		Vector3 size = MapData.GetPoliticalOnly().GetTerrainBounds().size;
		if (size.x == 0f || size.z == 0f)
		{
			size = this.cached_terrain_size;
		}
		else
		{
			this.cached_terrain_size = size;
		}
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num; j++)
			{
				this.pixel_ids_arr[j + i * num] = new float2(-1f, -1f);
			}
		}
		for (int k = 0; k < list.Count; k++)
		{
			LinesBatching.LineSegment lineSegment = list[k];
			float2 xz = array[lineSegment.start_idx].pos.xz;
			for (int l = lineSegment.start_idx + 1; l < lineSegment.end_idx; l++)
			{
				LinesBatching.LineComputeData lineComputeData = array[l];
				if ((lineComputeData.pos.x != 0f || lineComputeData.pos.z != 0f) && lineComputeData.pos.x != -1f)
				{
					int num3 = (int)(lineComputeData.pos.x * (float)num / size.x);
					int num4 = (int)(lineComputeData.pos.z * (float)num / size.z);
					int num5 = num3 + num4 * num;
					List<float4> list2 = this.border_segments[num5];
					if (list2 == null)
					{
						list2 = new List<float4>();
						this.border_segments[num5] = list2;
					}
					list2.Add(new float4(xz.x, xz.y, lineComputeData.pos.x, lineComputeData.pos.z));
					xz = lineComputeData.pos.xz;
				}
			}
		}
		int num6 = 0;
		for (int m = 0; m < this.border_segments.Length; m++)
		{
			List<float4> list3 = this.border_segments[m];
			if (list3 != null)
			{
				int num7 = num6;
				num6 += list3.Count;
				int num8 = num6;
				this.pixel_ids_arr[m] = new float2((float)num7, (float)num8);
			}
		}
		if (this.border_pos_buffer != null && this.border_pos_buffer.IsValid() && this.border_pos_buffer.count != num6)
		{
			this.border_pos_buffer.Dispose();
			this.border_pos_buffer = null;
		}
		if (num6 == 0)
		{
			return null;
		}
		if (this.border_pos_buffer == null)
		{
			this.border_pos_buffer = new ComputeBuffer(num6, sizeof(float4));
		}
		for (int n = 0; n < this.pixel_ids_arr.Length; n++)
		{
			List<float4> list4 = this.border_segments[n];
			if (list4 != null)
			{
				this.border_pos_buffer.SetData<float4>(list4, 0, (int)this.pixel_ids_arr[n].x, list4.Count);
			}
		}
		if (this.old_tex == null || this.old_tex.width != this.border_tex_res)
		{
			this.old_tex = new Texture2D(this.border_tex_res, this.border_tex_res, TextureFormat.ARGB32, false, false);
		}
		this.pixel_ids_buffer.SetData(this.pixel_ids_arr);
		this.k_border_cs.SetInt("channel", this.channel);
		this.k_border_cs.SetInt("border_res", this.border_tex_res);
		this.k_border_cs.SetInt("grid_res", num);
		this.k_border_cs.SetFloat("max_circle", (float)this.max_circle);
		float val = (float)this.border_tex_res / size.x;
		float val2 = (float)this.border_tex_res / size.z;
		this.k_border_cs.SetFloat("tile_size_x", val);
		this.k_border_cs.SetFloat("tile_size_y", val2);
		this.k_border_cs.SetBuffer(this.generate_sdf_kernel, "points", this.border_pos_buffer);
		this.k_border_cs.SetBuffer(this.generate_sdf_kernel, "pixel_ids", this.pixel_ids_buffer);
		this.k_border_cs.SetTexture(this.generate_sdf_kernel, "Result", this.k_border_tex);
		this.k_border_cs.SetTexture(this.generate_sdf_kernel, "Original", this.old_tex);
		uint num9;
		uint num10;
		uint num11;
		this.k_border_cs.GetKernelThreadGroupSizes(this.generate_sdf_kernel, out num9, out num10, out num11);
		this.k_border_cs.Dispatch(this.generate_sdf_kernel, Mathf.CeilToInt((float)this.border_tex_res / num9), Mathf.CeilToInt((float)this.border_tex_res / num10), 1);
		if (!string.IsNullOrEmpty(this.save_path))
		{
			RenderTexture.active = this.k_border_tex;
			this.old_tex.ReadPixels(new Rect(0f, 0f, (float)this.border_tex_res, (float)this.border_tex_res), 0, 0, false);
			this.old_tex.Apply();
			global::Common.SaveTexture(this.old_tex, this.save_path, true);
		}
		else
		{
			Graphics.CopyTexture(this.k_border_tex, this.old_tex);
		}
		MapData mapData = MapData.Get();
		if (mapData != null)
		{
			mapData.ResetSDFShaders(this.old_tex);
		}
		return this.old_tex;
	}

	// Token: 0x06000CA4 RID: 3236 RVA: 0x0008D670 File Offset: 0x0008B870
	public static void Dispose()
	{
		if (KingdomBorderSDFGenerator.instance == null || !KingdomBorderSDFGenerator.instance.init)
		{
			return;
		}
		RenderTexture.ReleaseTemporary(KingdomBorderSDFGenerator.instance.k_border_tex);
		KingdomBorderSDFGenerator.instance.k_border_cs = null;
		KingdomBorderSDFGenerator kingdomBorderSDFGenerator = KingdomBorderSDFGenerator.instance;
		if (((kingdomBorderSDFGenerator != null) ? kingdomBorderSDFGenerator.border_pos_buffer : null) != null)
		{
			KingdomBorderSDFGenerator.instance.border_pos_buffer.Dispose();
		}
		KingdomBorderSDFGenerator.instance.border_segments = null;
		KingdomBorderSDFGenerator.instance.compute_data = null;
		KingdomBorderSDFGenerator.instance.pixel_ids_arr = null;
		KingdomBorderSDFGenerator kingdomBorderSDFGenerator2 = KingdomBorderSDFGenerator.instance;
		if (((kingdomBorderSDFGenerator2 != null) ? kingdomBorderSDFGenerator2.pixel_ids_buffer : null) != null)
		{
			KingdomBorderSDFGenerator.instance.pixel_ids_buffer.Dispose();
		}
		KingdomBorderSDFGenerator.instance.pos_rots_binary = null;
		KingdomBorderSDFGenerator.instance.realmBorders = null;
		KingdomBorderSDFGenerator.instance.segments = null;
		KingdomBorderSDFGenerator.instance = null;
	}

	// Token: 0x040009BB RID: 2491
	private bool init;

	// Token: 0x040009BC RID: 2492
	private RenderTexture k_border_tex;

	// Token: 0x040009BD RID: 2493
	private int border_tex_res = 1024;

	// Token: 0x040009BE RID: 2494
	public ComputeShader k_border_cs;

	// Token: 0x040009BF RID: 2495
	private int generate_sdf_kernel = -1;

	// Token: 0x040009C0 RID: 2496
	private ComputeBuffer border_pos_buffer;

	// Token: 0x040009C1 RID: 2497
	private int max_circle = 25;

	// Token: 0x040009C2 RID: 2498
	private List<float4>[] border_segments;

	// Token: 0x040009C3 RID: 2499
	private float2[] pixel_ids_arr;

	// Token: 0x040009C4 RID: 2500
	private ComputeBuffer pixel_ids_buffer;

	// Token: 0x040009C5 RID: 2501
	public float max_path_len = 30f;

	// Token: 0x040009C6 RID: 2502
	public Texture2D old_tex;

	// Token: 0x040009C7 RID: 2503
	private int channel;

	// Token: 0x040009C8 RID: 2504
	private string save_path;

	// Token: 0x040009C9 RID: 2505
	private bool include_realms;

	// Token: 0x040009CA RID: 2506
	public int sdf_tex_res = 1024;

	// Token: 0x040009CB RID: 2507
	public int sdf_kingdom_scan_dist = 25;

	// Token: 0x040009CC RID: 2508
	public int sdf_realm_scan_dist = 25;

	// Token: 0x040009CD RID: 2509
	public List<LinesBatching.LineSegment> segments = new List<LinesBatching.LineSegment>();

	// Token: 0x040009CE RID: 2510
	public LinesBatching.LineComputeData[] compute_data;

	// Token: 0x040009CF RID: 2511
	public Dictionary<int, BordersBatching.RealmBorders> realmBorders = new Dictionary<int, BordersBatching.RealmBorders>();

	// Token: 0x040009D0 RID: 2512
	private int buff_idx;

	// Token: 0x040009D1 RID: 2513
	private const int stich_cut_count = 3;

	// Token: 0x040009D2 RID: 2514
	public static KingdomBorderSDFGenerator instance;

	// Token: 0x040009D3 RID: 2515
	public List<float3> pos_rots_binary;

	// Token: 0x040009D4 RID: 2516
	public string dir = "RealmBordersTrapezoidData";

	// Token: 0x040009D5 RID: 2517
	private Vector3 cached_terrain_size;

	// Token: 0x02000616 RID: 1558
	public struct LineComputeData
	{
		// Token: 0x060046C2 RID: 18114 RVA: 0x0020FD58 File Offset: 0x0020DF58
		public LineComputeData(int id, Vector3 pos, int prev_idx, int next_idx)
		{
			this.pos = pos;
			this.prev_idx = prev_idx;
			this.next_idx = next_idx;
		}

		// Token: 0x040033C8 RID: 13256
		public float3 pos;

		// Token: 0x040033C9 RID: 13257
		public int prev_idx;

		// Token: 0x040033CA RID: 13258
		public int next_idx;
	}
}
