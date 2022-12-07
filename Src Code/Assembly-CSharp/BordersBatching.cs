using System;
using System.Collections.Generic;
using Logic;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000135 RID: 309
public class BordersBatching : LinesBatching
{
	// Token: 0x0600106F RID: 4207 RVA: 0x000AED6C File Offset: 0x000ACF6C
	public static void FindBorderRenderers()
	{
		MapData mapData = MapData.Get();
		if (mapData == null)
		{
			return;
		}
		mapData.h_realm_borders = BordersBatching.FindBorderRenderer(mapData.transform, BordersBatching.BorderType.H_Realm_Border);
		mapData.h_kingdom_borders = BordersBatching.FindBorderRenderer(mapData.transform, BordersBatching.BorderType.H_Kingdom_Border);
		mapData.wv_realm_borders = BordersBatching.FindBorderRenderer(mapData.transform, BordersBatching.BorderType.WV_Realm_Border);
		mapData.wv_kingdom_borders = BordersBatching.FindBorderRenderer(mapData.transform, BordersBatching.BorderType.WV_Kingdom_Border);
		mapData.pv_realm_borders = BordersBatching.FindBorderRenderer(mapData.transform, BordersBatching.BorderType.PV_Realm_Border);
		mapData.pv_kingdom_borders = BordersBatching.FindBorderRenderer(mapData.transform, BordersBatching.BorderType.PV_Kingdom_Border);
		BordersBatching.Init();
	}

	// Token: 0x06001070 RID: 4208 RVA: 0x000AEDFA File Offset: 0x000ACFFA
	private static void Init()
	{
		if (BordersBatching._init)
		{
			return;
		}
		BordersBatching._init = true;
		Camera.onPreCull = (Camera.CameraCallback)Delegate.Combine(Camera.onPreCull, new Camera.CameraCallback(BordersBatching.DrawBorders));
	}

	// Token: 0x06001071 RID: 4209 RVA: 0x000AEE2A File Offset: 0x000AD02A
	private static void Dispose()
	{
		if (!BordersBatching._init)
		{
			return;
		}
		BordersBatching._init = false;
		Camera.onPreCull = (Camera.CameraCallback)Delegate.Remove(Camera.onPreCull, new Camera.CameraCallback(BordersBatching.DrawBorders));
	}

	// Token: 0x06001072 RID: 4210 RVA: 0x000AEE5C File Offset: 0x000AD05C
	public static void DrawBorders(Camera cam)
	{
		for (int i = BordersBatching.border_renderers.Count - 1; i >= 0; i--)
		{
			if (BordersBatching.border_renderers[i] == null)
			{
				BordersBatching.border_renderers.RemoveAt(i);
			}
		}
		for (int j = 0; j < BordersBatching.border_renderers.Count; j++)
		{
			BordersBatching bordersBatching = BordersBatching.border_renderers[j];
			if (bordersBatching.draw)
			{
				bordersBatching.Draw(cam);
			}
		}
	}

	// Token: 0x06001073 RID: 4211 RVA: 0x000AEED0 File Offset: 0x000AD0D0
	public static BordersBatching FindBorderRenderer(Transform transform, BordersBatching.BorderType type)
	{
		for (int i = BordersBatching.border_renderers.Count - 1; i >= 0; i--)
		{
			BordersBatching bordersBatching = BordersBatching.border_renderers[i];
			if (bordersBatching == null)
			{
				BordersBatching.border_renderers.RemoveAt(i);
			}
			else if (bordersBatching.type == type)
			{
				return bordersBatching;
			}
		}
		Transform transform2 = transform.Find("BordersBatching");
		if (transform2 == null)
		{
			return null;
		}
		string text = null;
		switch (type)
		{
		case BordersBatching.BorderType.WV_Realm_Border:
			text = "_wv_realm_borders";
			break;
		case BordersBatching.BorderType.WV_Kingdom_Border:
			text = "_wv_kingdom_borders";
			break;
		case BordersBatching.BorderType.PV_Realm_Border:
			text = "_pv_realm_borders";
			break;
		case BordersBatching.BorderType.PV_Kingdom_Border:
			text = "_pv_kingdom_borders";
			break;
		case BordersBatching.BorderType.H_Realm_Border:
			text = "_h_realm_borders";
			break;
		case BordersBatching.BorderType.H_Kingdom_Border:
			text = "_h_kingdom_borders";
			break;
		}
		if (text == null)
		{
			return null;
		}
		Transform transform3 = transform2.Find(text);
		BordersBatching bordersBatching2 = (transform3 != null) ? transform3.GetComponent<BordersBatching>() : null;
		if (bordersBatching2 != null)
		{
			BordersBatching.border_renderers.Add(bordersBatching2);
			bordersBatching2.render_queue = 2002 + BordersBatching.border_renderers.Count;
		}
		return bordersBatching2;
	}

	// Token: 0x06001074 RID: 4212 RVA: 0x000AEFD0 File Offset: 0x000AD1D0
	public void SetActiveBorder(global::Realm r1, global::Realm r2, bool enabled, bool enabled_water)
	{
		if (r1 != null && r2 != null)
		{
			LinesBatching.LineInfo line_info = this.line_info;
			if (((line_info != null) ? line_info.compute_data : null) != null)
			{
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
				this.dirty = true;
				border.enabled = (enabled && !enabled_water);
				border.enabled_water = enabled_water;
				for (int i = 0; i < border.segments.Count; i++)
				{
					border.segments[i].SetActive(border.enabled, this.line_info.compute_data);
				}
				return;
			}
		}
	}

	// Token: 0x06001075 RID: 4213 RVA: 0x000AF0A8 File Offset: 0x000AD2A8
	public void StitchBorders()
	{
		using (Game.Profile("LinesBatching.StitchBorders", false, 0f, null))
		{
			LinesBatching.LineInfo line_info = this.line_info;
			if (((line_info != null) ? line_info.compute_data : null) != null)
			{
				if (this.type == BordersBatching.BorderType.WV_Kingdom_Border || this.type == BordersBatching.BorderType.H_Kingdom_Border || this.type == BordersBatching.BorderType.H_Realm_Border)
				{
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
									LinesBatching.LineComputeData lineComputeData = this.line_info.compute_data[lineSegment.start_idx];
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
															LinesBatching.LineComputeData lineComputeData2 = this.line_info.compute_data[lineSegment3.end_idx - 1];
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
															LinesBatching.LineComputeData lineComputeData3 = this.line_info.compute_data[lineSegment4.end_idx - 1];
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
										int num5 = math.min(this.stich_cut_count, lineSegment2.end_idx - lineSegment2.start_idx);
										for (int i = lineSegment2.end_idx - num5; i < lineSegment2.end_idx; i++)
										{
											LinesBatching.LineComputeData lineComputeData4 = this.line_info.compute_data[i];
											lineComputeData4.enabled = 0f;
											this.line_info.compute_data[i] = lineComputeData4;
										}
										num2 = lineSegment2.end_idx - num5 - 1;
										num5 = math.min(this.stich_cut_count, lineSegment.end_idx - lineSegment.start_idx);
										for (int j = lineSegment.start_idx; j < lineSegment.start_idx + num5; j++)
										{
											LinesBatching.LineComputeData lineComputeData5 = this.line_info.compute_data[j];
											lineComputeData5.enabled = 0f;
											this.line_info.compute_data[j] = lineComputeData5;
										}
										lineComputeData = this.line_info.compute_data[lineSegment.start_idx + num5];
										lineComputeData.prev_idx = num2;
										this.line_info.compute_data[lineSegment.start_idx + num5] = lineComputeData;
										lineComputeData = this.line_info.compute_data[num2];
										lineComputeData.next_idx = lineSegment.start_idx + num5;
										this.line_info.compute_data[num2] = lineComputeData;
									}
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06001076 RID: 4214 RVA: 0x000023FD File Offset: 0x000005FD
	protected override void OnEnable()
	{
	}

	// Token: 0x06001077 RID: 4215 RVA: 0x000AF628 File Offset: 0x000AD828
	protected void OnDestroy()
	{
		BordersBatching.Dispose();
	}

	// Token: 0x06001078 RID: 4216 RVA: 0x000AF62F File Offset: 0x000AD82F
	public override void OnBuffersChanged()
	{
		this.dirty = false;
		this.StitchBorders();
		base.OnBuffersChanged();
	}

	// Token: 0x06001079 RID: 4217 RVA: 0x000AF644 File Offset: 0x000AD844
	protected override void InitLineSegments()
	{
		this.segments.Clear();
		float num = 0f;
		LinesBatching.LineSegment lineSegment = default(LinesBatching.LineSegment);
		bool flag = true;
		BordersBatching.RealmBorders realmBorders = null;
		int num2 = 0;
		global::Realm realm = null;
		global::Realm realm2 = null;
		this.line_info.buff_idx = 0U;
		this.realmBorders.Clear();
		for (int i = 0; i < this.pos_rots_binary.Count; i++)
		{
			float3 @float = this.pos_rots_binary[i];
			if (flag)
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
						lineSegment = default(LinesBatching.LineSegment);
						flag = false;
						lineSegment.start_idx = (int)this.line_info.buff_idx;
					}
				}
			}
			else
			{
				int prev_idx = (int)(this.line_info.buff_idx - 1U);
				if (i == this.pos_rots_binary.Count - 1 && !flag)
				{
					@float = new float3(-1f, -1f, -1f);
				}
				if (@float.x == -1f)
				{
					if (!flag)
					{
						lineSegment.end_idx = (int)this.line_info.buff_idx;
						for (int j = lineSegment.start_idx; j < lineSegment.end_idx; j++)
						{
							LinesBatching.LineComputeData lineComputeData = this.line_info.compute_data[j];
							lineComputeData.uv = (float)(j - lineSegment.start_idx) / num;
							this.line_info.compute_data[j] = lineComputeData;
						}
						this.segments.Add(lineSegment);
						prev_idx = -1;
						BordersBatching.RealmBorders.Border border = null;
						if (!realmBorders.borders.TryGetValue(num2, out border))
						{
							border = new BordersBatching.RealmBorders.Border();
							realmBorders.borders[num2] = border;
						}
						border.segments.Add(lineSegment);
					}
					num = 0f;
					flag = true;
				}
				if (realm != null && realm2 != null)
				{
					float3 v = @float;
					LinesBatching.LineComputeData lineComputeData2 = new LinesBatching.LineComputeData(i, v, 0f, false, prev_idx, (int)(this.line_info.buff_idx + 1U));
					this.line_info.compute_data[(int)this.line_info.buff_idx] = lineComputeData2;
					if (this.line_info.buff_idx > 0U && @float.x != -1f)
					{
						LinesBatching.LineComputeData lineComputeData3 = this.line_info.compute_data[(int)(this.line_info.buff_idx - 1U)];
						num += Vector3.Distance(lineComputeData3.pos, v);
						if (num > this.max_path_len)
						{
							num = this.max_path_len;
						}
					}
					this.line_info.buff_idx += 1U;
				}
			}
		}
	}

	// Token: 0x0600107A RID: 4218 RVA: 0x000AF92E File Offset: 0x000ADB2E
	public override bool UpdateLinesData(LinesBatching.LineShaderData[] shader_data, LinesBatching.LineComputeData[] compute_data)
	{
		bool result = base.UpdateLinesData(shader_data, compute_data);
		if (this.type == BordersBatching.BorderType.WV_Kingdom_Border)
		{
			this.CreateKingdomColors();
		}
		return result;
	}

	// Token: 0x0600107B RID: 4219 RVA: 0x000AF948 File Offset: 0x000ADB48
	public void CreateStripeTexture()
	{
		if (this.kingdomColors == null || this.kingdomColors.Length == 0)
		{
			this.kingdomColors = new Color[1024];
		}
		this.colorsTexture = new Texture2D(32, 32, TextureFormat.RGBA32, false, true);
		this.colorsTexture.filterMode = FilterMode.Point;
		this.colorsTexture.wrapMode = TextureWrapMode.Clamp;
	}

	// Token: 0x0600107C RID: 4220 RVA: 0x000AF9A0 File Offset: 0x000ADBA0
	protected void CreateKingdomColors()
	{
		WorldMap worldMap = WorldMap.Get();
		if (worldMap == null)
		{
			return;
		}
		List<global::Realm> realms = worldMap.Realms;
		if (this.colorsTexture == null)
		{
			this.CreateStripeTexture();
		}
		Color red = Color.red;
		Color black = Color.black;
		for (int i = 1; i <= realms.Count; i++)
		{
			global::Realm r = realms[i - 1];
			this.OnRealmKingdomChanged(r, false);
		}
		this.colorsTexture.SetPixels(this.kingdomColors);
		this.colorsTexture.Apply();
		this.line_info.material.SetTexture("_ColorsTex", this.colorsTexture);
	}

	// Token: 0x0600107D RID: 4221 RVA: 0x000AFA3E File Offset: 0x000ADC3E
	protected override void HandleProjectChanged()
	{
		base.MarkDirty(false, true, false, true);
	}

	// Token: 0x0600107E RID: 4222 RVA: 0x000AFA4C File Offset: 0x000ADC4C
	public void OnRealmKingdomChanged(global::Realm r, bool apply = false)
	{
		global::Kingdom kingdom = r.GetKingdom();
		int num = Math.Abs(r.id);
		Color color = (kingdom == null) ? Color.black : kingdom.MapColor;
		this.kingdomColors[num] = color;
		if (apply)
		{
			this.colorsTexture.SetPixels(this.kingdomColors);
			this.colorsTexture.Apply();
		}
		MapData.Get().RegenerateSDF();
	}

	// Token: 0x0600107F RID: 4223 RVA: 0x000AFAB4 File Offset: 0x000ADCB4
	public void LoadNeighbors(Game game = null)
	{
		base.LoadPoints(game, false);
		int num = 0;
		int id = 0;
		int num2 = 0;
		for (int i = 0; i < this.pos_rots_binary.Count; i++)
		{
			float3 @float = this.pos_rots_binary[i];
			if (i == num)
			{
				id = (int)@float.x;
				num2 = (int)@float.y;
			}
			if (@float.x == -1f || i == this.pos_rots_binary.Count - 1)
			{
				global::Realm realm = global::Realm.Get(id);
				global::Realm realm2 = global::Realm.Get(num2);
				if (realm != null && realm2 != null)
				{
					bool flag = false;
					for (int j = 0; j < realm.Neighbors.Count; j++)
					{
						if (realm.Neighbors[j].rid == num2)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						realm.Neighbors.Add(new global::Realm.Neighbor
						{
							rid = num2
						});
					}
				}
				num = i + 1;
			}
		}
	}

	// Token: 0x04000AD8 RID: 2776
	public bool draw = true;

	// Token: 0x04000AD9 RID: 2777
	private static bool _init;

	// Token: 0x04000ADA RID: 2778
	public static List<BordersBatching> border_renderers = new List<BordersBatching>();

	// Token: 0x04000ADB RID: 2779
	private Color[] kingdomColors;

	// Token: 0x04000ADC RID: 2780
	private Texture2D colorsTexture;

	// Token: 0x04000ADD RID: 2781
	[Tooltip("How many segments to remove when stiching kindom borders")]
	public int stich_cut_count = 3;

	// Token: 0x04000ADE RID: 2782
	public BordersBatching.BorderType type;

	// Token: 0x04000ADF RID: 2783
	public bool dirty = true;

	// Token: 0x04000AE0 RID: 2784
	public Dictionary<int, BordersBatching.RealmBorders> realmBorders = new Dictionary<int, BordersBatching.RealmBorders>();

	// Token: 0x02000650 RID: 1616
	public enum BorderType
	{
		// Token: 0x040034F2 RID: 13554
		WV_Realm_Border,
		// Token: 0x040034F3 RID: 13555
		WV_Kingdom_Border,
		// Token: 0x040034F4 RID: 13556
		PV_Realm_Border,
		// Token: 0x040034F5 RID: 13557
		PV_Kingdom_Border,
		// Token: 0x040034F6 RID: 13558
		H_Realm_Border,
		// Token: 0x040034F7 RID: 13559
		H_Kingdom_Border,
		// Token: 0x040034F8 RID: 13560
		COUNT
	}

	// Token: 0x02000651 RID: 1617
	public class RealmBorders
	{
		// Token: 0x040034F9 RID: 13561
		public int r;

		// Token: 0x040034FA RID: 13562
		public Dictionary<int, BordersBatching.RealmBorders.Border> borders = new Dictionary<int, BordersBatching.RealmBorders.Border>();

		// Token: 0x020009FB RID: 2555
		public class Border
		{
			// Token: 0x04004611 RID: 17937
			public bool enabled_water;

			// Token: 0x04004612 RID: 17938
			public bool enabled;

			// Token: 0x04004613 RID: 17939
			public List<LinesBatching.LineSegment> segments = new List<LinesBatching.LineSegment>();
		}
	}
}
