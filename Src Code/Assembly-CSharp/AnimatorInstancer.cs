using System;
using System.Collections.Generic;
using Logic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x0200010F RID: 271
public class AnimatorInstancer
{
	// Token: 0x17000091 RID: 145
	// (get) Token: 0x06000C5F RID: 3167 RVA: 0x0008AA48 File Offset: 0x00088C48
	// (set) Token: 0x06000C60 RID: 3168 RVA: 0x0008AA50 File Offset: 0x00088C50
	public string model_key { get; private set; }

	// Token: 0x17000092 RID: 146
	// (get) Token: 0x06000C61 RID: 3169 RVA: 0x0008AA59 File Offset: 0x00088C59
	// (set) Token: 0x06000C62 RID: 3170 RVA: 0x0008AA61 File Offset: 0x00088C61
	public int model_type { get; private set; }

	// Token: 0x17000093 RID: 147
	// (get) Token: 0x06000C63 RID: 3171 RVA: 0x0008AA6C File Offset: 0x00088C6C
	private TextureBaker.PerModelData per_model_data
	{
		get
		{
			if (this._per_model_data != null)
			{
				return this._per_model_data;
			}
			List<TextureBaker.PerModelData> list;
			if (!this.texture_baker.skinning_drawers.TryGetValue(this.model_key, out list))
			{
				return null;
			}
			TextureBaker.PerModelData per_model_data = list[this.model_type];
			this._per_model_data = per_model_data;
			return this._per_model_data;
		}
	}

	// Token: 0x17000094 RID: 148
	// (get) Token: 0x06000C64 RID: 3172 RVA: 0x0008AAC0 File Offset: 0x00088CC0
	private KeyframeTextureBaker.BakedData baked_data
	{
		get
		{
			TextureBaker.PerModelData per_model_data = this.per_model_data;
			if (per_model_data == null)
			{
				return null;
			}
			return per_model_data.drawers[0].baked_data;
		}
	}

	// Token: 0x06000C65 RID: 3173 RVA: 0x0008AAEC File Offset: 0x00088CEC
	public static List<TextureBaker.PerModelData> LoadBakedSkinningData(TextureBaker texture_baker, DT.Field def, string key_addon = null, float scale_mod = 1f, int layer = 0)
	{
		string text = def.key;
		if (!string.IsNullOrEmpty(key_addon))
		{
			text += key_addon;
		}
		string text2 = "model";
		if (!string.IsNullOrEmpty(key_addon))
		{
			text2 += key_addon;
		}
		foreach (KeyValuePair<string, List<TextureBaker.PerModelData>> keyValuePair in texture_baker.skinning_drawers)
		{
			if (keyValuePair.Key == text)
			{
				return keyValuePair.Value;
			}
		}
		List<TextureBaker.PerModelData> list2;
		using (Game.Profile("AnimatorInstancer.LoadBakedData", false, 0f, null))
		{
			List<KeyframeTextureBaker.BakedData[]> list = new List<KeyframeTextureBaker.BakedData[]>();
			DT.Field field = def.FindChild(text2, null, true, true, true, '.');
			if (field == null)
			{
				list2 = null;
			}
			else
			{
				int num = field.NumValues();
				if (num == 0)
				{
					list2 = null;
				}
				else
				{
					for (int i = 0; i < num; i++)
					{
						Assets.AssetInfo asset = Assets.GetAsset(DT.Unquote(field.ValueStr(i)).Replace(".prefab", ".asset"), false);
						SerializedAnimData serializedAnimData = ((asset != null) ? asset.GetAsset() : null) as SerializedAnimData;
						if (!(serializedAnimData == null))
						{
							list.Add(serializedAnimData.data);
						}
					}
					List<TextureBaker.PerModelData> list3 = texture_baker.skinning_drawers[text] = new List<TextureBaker.PerModelData>();
					for (int j = 0; j < list.Count; j++)
					{
						KeyframeTextureBaker.BakedData[] array = list[j];
						TextureBaker.PerModelData perModelData = new TextureBaker.PerModelData();
						perModelData.draw_layer = layer;
						perModelData.model_data_buffer = new GrowBuffer<TextureBaker.InstancedSkinningDrawerBatched.DrawCallData>(Allocator.Persistent, 64);
						perModelData.model_compute_buffer = new ComputeBuffer(10000, TextureBaker.InstancedSkinningDrawerBatched.DrawCallDataSize);
						for (int k = 0; k < array.Length; k++)
						{
							TextureBaker.InstancedSkinningDrawerBatched item = new TextureBaker.InstancedSkinningDrawerBatched(array[k], texture_baker.kingdom_colors, scale_mod, false);
							perModelData.drawers.Add(item);
						}
						perModelData.key_to_idx = new Dictionary<string, int>();
						for (int l = 0; l < perModelData.drawers[0].baked_data.state_names.Length; l++)
						{
							perModelData.key_to_idx[perModelData.drawers[0].baked_data.state_names[l]] = l;
						}
						list3.Add(perModelData);
					}
					list2 = list3;
				}
			}
		}
		return list2;
	}

	// Token: 0x06000C66 RID: 3174 RVA: 0x0008AD74 File Offset: 0x00088F74
	private KeyframeTextureBaker.AnimationClipDataBaked GetAnimInfo(int idx)
	{
		KeyframeTextureBaker.BakedData baked_data = this.baked_data;
		if (baked_data == null)
		{
			return default(KeyframeTextureBaker.AnimationClipDataBaked);
		}
		return baked_data.Animations[idx];
	}

	// Token: 0x17000095 RID: 149
	// (get) Token: 0x06000C67 RID: 3175 RVA: 0x0008ADA1 File Offset: 0x00088FA1
	private KeyframeTextureBaker.AnimationClipDataBaked cur_anim_info
	{
		get
		{
			return this.GetAnimInfo(this.cur_anim_idx);
		}
	}

	// Token: 0x17000096 RID: 150
	// (get) Token: 0x06000C68 RID: 3176 RVA: 0x0008ADAF File Offset: 0x00088FAF
	private KeyframeTextureBaker.AnimationClipDataBaked prev_anim_info
	{
		get
		{
			return this.GetAnimInfo(this.prev_anim_idx);
		}
	}

	// Token: 0x17000097 RID: 151
	// (get) Token: 0x06000C69 RID: 3177 RVA: 0x0008ADBD File Offset: 0x00088FBD
	public float cur_anim_length
	{
		get
		{
			return this.cur_anim_info.AnimationLength;
		}
	}

	// Token: 0x17000098 RID: 152
	// (get) Token: 0x06000C6A RID: 3178 RVA: 0x0008ADCA File Offset: 0x00088FCA
	public float cur_anim_blend_time
	{
		get
		{
			return this.cur_anim_info.BlendTime;
		}
	}

	// Token: 0x17000099 RID: 153
	// (get) Token: 0x06000C6B RID: 3179 RVA: 0x0008ADD7 File Offset: 0x00088FD7
	public float cur_anim_range
	{
		get
		{
			return this.cur_anim_info.TextureRange;
		}
	}

	// Token: 0x1700009A RID: 154
	// (get) Token: 0x06000C6C RID: 3180 RVA: 0x0008ADE4 File Offset: 0x00088FE4
	public float cur_anim_offset
	{
		get
		{
			return this.cur_anim_info.TextureStart;
		}
	}

	// Token: 0x1700009B RID: 155
	// (get) Token: 0x06000C6D RID: 3181 RVA: 0x0008ADF1 File Offset: 0x00088FF1
	public float prev_anim_length
	{
		get
		{
			return this.prev_anim_info.AnimationLength;
		}
	}

	// Token: 0x1700009C RID: 156
	// (get) Token: 0x06000C6E RID: 3182 RVA: 0x0008ADFE File Offset: 0x00088FFE
	public float prev_anim_blend_time
	{
		get
		{
			return this.prev_anim_info.BlendTime;
		}
	}

	// Token: 0x1700009D RID: 157
	// (get) Token: 0x06000C6F RID: 3183 RVA: 0x0008AE0B File Offset: 0x0008900B
	public float prev_anim_range
	{
		get
		{
			return this.prev_anim_info.TextureRange;
		}
	}

	// Token: 0x1700009E RID: 158
	// (get) Token: 0x06000C70 RID: 3184 RVA: 0x0008AE18 File Offset: 0x00089018
	public float prev_anim_offset
	{
		get
		{
			return this.prev_anim_info.TextureStart;
		}
	}

	// Token: 0x06000C71 RID: 3185 RVA: 0x0008AE28 File Offset: 0x00089028
	public AnimatorInstancer(TextureBaker texture_baker, DT.Field def, string key_addon = null, int layer = 0, float scale = 1f)
	{
		using (Game.Profile("AnimatorInstancer.Constructor", false, 0f, null))
		{
			this.texture_baker = texture_baker;
			this.SetData(texture_baker, def, key_addon, layer, scale);
		}
	}

	// Token: 0x06000C72 RID: 3186 RVA: 0x0008AE94 File Offset: 0x00089094
	public void SetData(TextureBaker texture_baker, DT.Field def, string key_addon = null, int layer = 0, float scale = 1f)
	{
		this.model_key = def.key;
		if (!string.IsNullOrEmpty(key_addon))
		{
			this.model_key += key_addon;
		}
		List<TextureBaker.PerModelData> list = AnimatorInstancer.LoadBakedSkinningData(texture_baker, def, key_addon, scale, layer);
		this.model_type = UnityEngine.Random.Range(0, list.Count);
	}

	// Token: 0x06000C73 RID: 3187 RVA: 0x0008AEE8 File Offset: 0x000890E8
	public void Play(string anim_name, bool loop = true, bool start_finished = false, float offset = 0f)
	{
		int idx;
		if (!this.per_model_data.key_to_idx.TryGetValue(anim_name, out idx))
		{
			return;
		}
		this.Play(idx, loop, start_finished, offset);
	}

	// Token: 0x06000C74 RID: 3188 RVA: 0x0008AF16 File Offset: 0x00089116
	public void Play(int idx, bool loop = true, bool start_finished = false, float offset = 0f)
	{
		this.loop = loop;
		this.OnBeginAnimation(idx, false, offset, start_finished);
	}

	// Token: 0x1700009F RID: 159
	// (get) Token: 0x06000C75 RID: 3189 RVA: 0x0008AF2A File Offset: 0x0008912A
	// (set) Token: 0x06000C76 RID: 3190 RVA: 0x0008AF37 File Offset: 0x00089137
	public float3 Position
	{
		get
		{
			return this.result.pos;
		}
		set
		{
			if (this.last_position_update_frame != UnityEngine.Time.frameCount)
			{
				this.last_position_update_frame = UnityEngine.Time.frameCount;
				this.result.prevPos = this.result.pos;
			}
			this.result.pos = value;
		}
	}

	// Token: 0x170000A0 RID: 160
	// (get) Token: 0x06000C77 RID: 3191 RVA: 0x0008AF73 File Offset: 0x00089173
	// (set) Token: 0x06000C78 RID: 3192 RVA: 0x0008AF94 File Offset: 0x00089194
	public Quaternion Rotation
	{
		get
		{
			return Quaternion.Euler(0f, this.result.rot.y, 0f);
		}
		set
		{
			this.result.rot.y = value.eulerAngles.y;
		}
	}

	// Token: 0x06000C79 RID: 3193 RVA: 0x0008AFB2 File Offset: 0x000891B2
	public void UpdateTransform(Vector3 pos, Quaternion rot)
	{
		this.Position = pos;
		this.Rotation = rot;
	}

	// Token: 0x06000C7A RID: 3194 RVA: 0x0008AFC7 File Offset: 0x000891C7
	public void UpdateKingdomColor(int color_id)
	{
		this.result.kingdom_color_id = color_id;
	}

	// Token: 0x06000C7B RID: 3195 RVA: 0x0008AFD5 File Offset: 0x000891D5
	public void Update(float dt, float4[] f_planes, bool force_visible = false)
	{
		this.Update(dt, this.Position, this.Rotation, f_planes, force_visible);
	}

	// Token: 0x06000C7C RID: 3196 RVA: 0x0008AFF4 File Offset: 0x000891F4
	public void Update(float dt, Vector3 pos, Quaternion rot, float4[] f_planes, bool force_visible = false)
	{
		int frameCount = UnityEngine.Time.frameCount;
		if (frameCount == this.last_frame)
		{
			return;
		}
		this.last_frame = frameCount;
		this.UpdateTransform(pos, rot);
		if (!force_visible && !TextureBaker.Visible(pos, f_planes))
		{
			return;
		}
		this.OnUpdateAnimation(this.cur_anim_idx, dt * this.anim_speed, 1f);
		this.blend_time = math.clamp(this.blend_time + dt, 0f, 0.25f);
		List<TextureBaker.PerModelData> list;
		if (!this.texture_baker.skinning_drawers.TryGetValue(this.model_key, out list))
		{
			return;
		}
		list[this.model_type].model_data_buffer.Add(this.result);
	}

	// Token: 0x06000C7D RID: 3197 RVA: 0x0008B0A4 File Offset: 0x000892A4
	public void CalcAnimData()
	{
		float x = math.clamp(this.cur_anim_time / this.cur_anim_length, 0f, 1f) * this.cur_anim_range + this.cur_anim_offset;
		float y = math.clamp(this.prev_anim_time / this.prev_anim_length, 0f, 1f) * this.prev_anim_range + this.prev_anim_offset;
		this.result.anim_data = new float3(x, y, this.blend_time / this.cur_anim_blend_time);
	}

	// Token: 0x06000C7E RID: 3198 RVA: 0x0008B128 File Offset: 0x00089328
	private void OnBeginAnimation(int anim_idx, bool backwards = false, float ofs = 0f, bool start_finished = false)
	{
		if (anim_idx != this.cur_anim_idx)
		{
			this.blend_time = 0f;
			this.prev_anim_time = this.cur_anim_time;
			this.prev_anim_idx = this.cur_anim_idx;
			this.cur_anim_idx = anim_idx;
		}
		if (start_finished)
		{
			this.blend_time = 0.25f;
			this.cur_anim_time = this.cur_anim_length;
			return;
		}
		this.cur_anim_time = (backwards ? (this.cur_anim_length - ofs) : ofs);
	}

	// Token: 0x06000C7F RID: 3199 RVA: 0x0008B198 File Offset: 0x00089398
	private void OnUpdateAnimation(int anim_state, float dt, float anim_speed)
	{
		this.cur_anim_time += dt * anim_speed;
		bool flag = this.cur_anim_time >= this.cur_anim_length;
		bool flag2 = this.cur_anim_time < 0f;
		if (flag || flag2)
		{
			if (flag)
			{
				if (this.loop)
				{
					this.cur_anim_time -= this.cur_anim_length;
				}
				else
				{
					this.cur_anim_time = this.cur_anim_length;
				}
			}
			if (flag2)
			{
				if (this.loop)
				{
					this.cur_anim_time += this.cur_anim_length;
				}
				else
				{
					this.cur_anim_time = 0f;
				}
			}
			this.OnBeginAnimation(anim_state, anim_speed < 0f, this.cur_anim_time, false);
		}
		this.CalcAnimData();
	}

	// Token: 0x040009A7 RID: 2471
	public int last_frame;

	// Token: 0x040009A9 RID: 2473
	private TextureBaker texture_baker;

	// Token: 0x040009AB RID: 2475
	private TextureBaker.PerModelData _per_model_data;

	// Token: 0x040009AC RID: 2476
	private int cur_anim_idx;

	// Token: 0x040009AD RID: 2477
	private float cur_anim_time;

	// Token: 0x040009AE RID: 2478
	private int prev_anim_idx;

	// Token: 0x040009AF RID: 2479
	private float prev_anim_time;

	// Token: 0x040009B0 RID: 2480
	private float blend_time;

	// Token: 0x040009B1 RID: 2481
	public float anim_speed = 1f;

	// Token: 0x040009B2 RID: 2482
	public bool loop = true;

	// Token: 0x040009B3 RID: 2483
	private TextureBaker.InstancedSkinningDrawerBatched.DrawCallData result;

	// Token: 0x040009B4 RID: 2484
	private int last_position_update_frame;
}
