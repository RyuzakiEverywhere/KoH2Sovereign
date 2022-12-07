using System;
using Logic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

// Token: 0x020000D2 RID: 210
public struct TroopCollisions
{
	// Token: 0x06000A2B RID: 2603 RVA: 0x00074E00 File Offset: 0x00073000
	public unsafe TroopCollisions(Troops.TroopsPtrData* pdata, int initial_capacity, PathData.DataPointers pointers)
	{
		this.pdata = pdata;
		this.map = new NativeMultiHashMap<int2, int>(initial_capacity, Allocator.Persistent);
		this.path_data = pointers;
	}

	// Token: 0x06000A2C RID: 2604 RVA: 0x00074E1D File Offset: 0x0007301D
	public void Dispose()
	{
		this.map.Dispose();
	}

	// Token: 0x06000A2D RID: 2605 RVA: 0x00074E2A File Offset: 0x0007302A
	public void Clear()
	{
		this.map.Clear();
	}

	// Token: 0x06000A2E RID: 2606 RVA: 0x00074E37 File Offset: 0x00073037
	public static int2 WorldToGrid(float2 pos)
	{
		return new int2((int)(pos.x / 2f), (int)(pos.y / 2f));
	}

	// Token: 0x06000A2F RID: 2607 RVA: 0x00074E58 File Offset: 0x00073058
	public static float2 GridToWorld(int2 tile)
	{
		return new float2((float)tile.x * 2f, (float)tile.y * 2f);
	}

	// Token: 0x06000A30 RID: 2608 RVA: 0x00074E79 File Offset: 0x00073079
	public static float2 WorldToTileLocal(float2 pos, int2 tile)
	{
		return (pos - TroopCollisions.GridToWorld(tile)) / 2f;
	}

	// Token: 0x17000079 RID: 121
	// (get) Token: 0x06000A31 RID: 2609 RVA: 0x00074E91 File Offset: 0x00073091
	public TroopCollisions.Concurrent concurrent
	{
		get
		{
			return new TroopCollisions.Concurrent(this.pdata, this.map);
		}
	}

	// Token: 0x1700007A RID: 122
	// (get) Token: 0x06000A32 RID: 2610 RVA: 0x00074EA4 File Offset: 0x000730A4
	public TroopCollisions.ReadOnly read_only
	{
		get
		{
			return new TroopCollisions.ReadOnly(this.pdata, this.map, this.path_data);
		}
	}

	// Token: 0x06000A33 RID: 2611 RVA: 0x00074EC0 File Offset: 0x000730C0
	public static bool RayStep(ref int2 tile, ref float2 ptLocal, ref float2 destLocal, float r, ref TempList<int2> coords)
	{
		if (destLocal.x >= 0f && destLocal.x <= 1f && destLocal.y >= 0f && destLocal.y <= 1f)
		{
			return false;
		}
		float2 @float = destLocal - ptLocal;
		if (@float.x > 0f)
		{
			float num = 1f - ptLocal.x;
			float num2 = ptLocal.y + @float.y * num / @float.x;
			if (num2 > 1f)
			{
				num = 1f - ptLocal.y;
				num2 = ptLocal.x + @float.x * num / @float.y;
				ptLocal = new float2(num2, 0f);
				destLocal = new float2(destLocal.x, destLocal.y - 1f);
				if (ptLocal.x + r > 1f)
				{
					coords.Add(new int2(tile.x + 1, tile.y + 1));
					coords.Add(new int2(tile.x + 1, tile.y));
				}
				if (ptLocal.x - r < 0f)
				{
					coords.Add(new int2(tile.x - 1, tile.y + 1));
					coords.Add(new int2(tile.x - 1, tile.y));
				}
				tile = new int2(tile.x, tile.y + 1);
			}
			else if (num2 < 0f)
			{
				num = -ptLocal.y;
				num2 = ptLocal.x + @float.x * num / @float.y;
				ptLocal = new float2(num2, 1f);
				destLocal = new float2(destLocal.x, destLocal.y + 1f);
				if (ptLocal.x + r > 1f)
				{
					coords.Add(new int2(tile.x + 1, tile.y - 1));
					coords.Add(new int2(tile.x + 1, tile.y));
				}
				if (ptLocal.x - r < 0f)
				{
					coords.Add(new int2(tile.x - 1, tile.y - 1));
					coords.Add(new int2(tile.x - 1, tile.y));
				}
				tile = new int2(tile.x, tile.y - 1);
			}
			else
			{
				ptLocal = new float2(0f, num2);
				destLocal = new float2(destLocal.x - 1f, destLocal.y);
				if (ptLocal.y + r > 1f)
				{
					coords.Add(new int2(tile.x, tile.y + 1));
					coords.Add(new int2(tile.x + 1, tile.y + 1));
				}
				if (ptLocal.y - r < 0f)
				{
					coords.Add(new int2(tile.x, tile.y - 1));
					coords.Add(new int2(tile.x + 1, tile.y - 1));
				}
				tile = new int2(tile.x + 1, tile.y);
			}
		}
		else if (@float.x < 0f)
		{
			float num = -ptLocal.x;
			float num2 = ptLocal.y + @float.y * num / @float.x;
			if (num2 > 1f)
			{
				num = 1f - ptLocal.y;
				num2 = ptLocal.x + @float.x * num / @float.y;
				ptLocal = new float2(num2, 0f);
				destLocal = new float2(destLocal.x, destLocal.y - 1f);
				if (ptLocal.x + r > 1f)
				{
					coords.Add(new int2(tile.x + 1, tile.y + 1));
					coords.Add(new int2(tile.x + 1, tile.y));
				}
				if (ptLocal.x - r < 0f)
				{
					coords.Add(new int2(tile.x - 1, tile.y + 1));
					coords.Add(new int2(tile.x - 1, tile.y));
				}
				tile = new int2(tile.x, tile.y + 1);
			}
			else if (num2 < 0f)
			{
				num = -ptLocal.y;
				num2 = ptLocal.x + @float.x * num / @float.y;
				ptLocal = new float2(num2, 1f);
				destLocal = new float2(destLocal.x, destLocal.y + 1f);
				if (ptLocal.x + r > 1f)
				{
					coords.Add(new int2(tile.x + 1, tile.y - 1));
					coords.Add(new int2(tile.x + 1, tile.y));
				}
				if (ptLocal.x - r < 0f)
				{
					coords.Add(new int2(tile.x - 1, tile.y - 1));
					coords.Add(new int2(tile.x - 1, tile.y));
				}
				tile = new int2(tile.x, tile.y - 1);
			}
			else
			{
				ptLocal = new float2(1f, num2);
				destLocal = new float2(destLocal.x + 1f, destLocal.y);
				if (ptLocal.y + r > 1f)
				{
					coords.Add(new int2(tile.x, tile.y + 1));
					coords.Add(new int2(tile.x - 1, tile.y + 1));
				}
				if (ptLocal.y - r < 0f)
				{
					coords.Add(new int2(tile.x, tile.y - 1));
					coords.Add(new int2(tile.x - 1, tile.y - 1));
				}
				tile = new int2(tile.x - 1, tile.y);
			}
		}
		else if (@float.y > 0f)
		{
			ptLocal = new float2(ptLocal.x, 0f);
			if (ptLocal.x - r < 0f)
			{
				coords.Add(new int2(tile.x - 1, tile.y + 1));
				coords.Add(new int2(tile.x - 1, tile.y));
			}
			if (ptLocal.x + r > 1f)
			{
				coords.Add(new int2(tile.x + 1, tile.y + 1));
				coords.Add(new int2(tile.x + 1, tile.y));
			}
			tile = new int2(tile.x, tile.y + 1);
			destLocal = new float2(destLocal.x, destLocal.y - 1f);
		}
		else
		{
			if (@float.y >= 0f)
			{
				return false;
			}
			ptLocal = new float2(ptLocal.x, 1f);
			if (ptLocal.x - r < 0f)
			{
				coords.Add(new int2(tile.x - 1, tile.y - 1));
				coords.Add(new int2(tile.x - 1, tile.y));
			}
			if (ptLocal.x + r > 1f)
			{
				coords.Add(new int2(tile.x + 1, tile.y - 1));
				coords.Add(new int2(tile.x + 1, tile.y));
			}
			tile = new int2(tile.x, tile.y - 1);
			destLocal = new float2(destLocal.x, destLocal.y + 1f);
		}
		coords.Add(tile);
		return true;
	}

	// Token: 0x04000833 RID: 2099
	public const float tile_size = 2f;

	// Token: 0x04000834 RID: 2100
	[NativeDisableUnsafePtrRestriction]
	public unsafe Troops.TroopsPtrData* pdata;

	// Token: 0x04000835 RID: 2101
	private NativeMultiHashMap<int2, int> map;

	// Token: 0x04000836 RID: 2102
	[NativeDisableUnsafePtrRestriction]
	public PathData.DataPointers path_data;

	// Token: 0x020005C2 RID: 1474
	public struct Concurrent
	{
		// Token: 0x060044F1 RID: 17649 RVA: 0x00203631 File Offset: 0x00201831
		public unsafe Concurrent(Troops.TroopsPtrData* pdata, NativeMultiHashMap<int2, int> map)
		{
			this.pdata = pdata;
			this.map = map.AsParallelWriter();
		}

		// Token: 0x060044F2 RID: 17650 RVA: 0x00203648 File Offset: 0x00201848
		public unsafe void Add(Troops.Troop troop)
		{
			int2 @int = TroopCollisions.WorldToGrid(troop.pos);
			if (troop.def->radius <= 1f)
			{
				this.map.Add(@int, troop.id);
				return;
			}
			int num = (int)math.ceil(troop.def->radius);
			float num2 = troop.def->radius * troop.def->radius;
			int2 int2 = default(int2);
			int2.x = -num;
			while (int2.x <= num)
			{
				int2.y = -num;
				while (int2.y <= num)
				{
					if (math.distancesq(@int, @int + int2) < num2)
					{
						this.map.Add(@int + int2, troop.id);
					}
					int2.y++;
				}
				int2.x++;
			}
		}

		// Token: 0x060044F3 RID: 17651 RVA: 0x00203734 File Offset: 0x00201934
		public void Add(int2 tile, int id)
		{
			this.map.Add(tile, id);
		}

		// Token: 0x040031A1 RID: 12705
		[NativeDisableUnsafePtrRestriction]
		public unsafe Troops.TroopsPtrData* pdata;

		// Token: 0x040031A2 RID: 12706
		private NativeMultiHashMap<int2, int>.ParallelWriter map;
	}

	// Token: 0x020005C3 RID: 1475
	public struct ReadOnly
	{
		// Token: 0x060044F4 RID: 17652 RVA: 0x00203743 File Offset: 0x00201943
		public unsafe ReadOnly(Troops.TroopsPtrData* pdata, NativeMultiHashMap<int2, int> map, PathData.DataPointers pointers)
		{
			this.pdata = pdata;
			this.map = map;
			this.pointers = pointers;
		}

		// Token: 0x060044F5 RID: 17653 RVA: 0x0020375C File Offset: 0x0020195C
		public unsafe int EnumTroops(int2 tile, ref TempList<int> troop_ids, int battle_side = -1, bool check_duplicates = false)
		{
			Troops.Troop troop = new Troops.Troop(this.pdata, -2, 0);
			NativeMultiHashMapIterator<int2> nativeMultiHashMapIterator;
			if (!this.map.TryGetFirstValue(tile, out troop.id, out nativeMultiHashMapIterator))
			{
				return 0;
			}
			int num = 0;
			do
			{
				if (battle_side < 0 || troop.squad->battle_side == battle_side)
				{
					bool flag = false;
					if (check_duplicates)
					{
						for (int i = 0; i < troop_ids.Length; i++)
						{
							if (troop_ids[i] == troop.id)
							{
								flag = true;
								break;
							}
						}
					}
					if (!flag)
					{
						troop_ids.Add(troop.id);
						num++;
					}
				}
			}
			while (this.map.TryGetNextValue(out troop.id, ref nativeMultiHashMapIterator));
			return num;
		}

		// Token: 0x060044F6 RID: 17654 RVA: 0x00203800 File Offset: 0x00201A00
		public int EnumTroops(int2 min_tile, int2 max_tile, ref TempList<int> troop_ids, int battle_side = -1, bool check_duplicates = false)
		{
			int num = 0;
			int2 @int;
			@int.y = min_tile.y;
			while (@int.y <= max_tile.y)
			{
				@int.x = min_tile.x;
				while (@int.x <= max_tile.x)
				{
					num += this.EnumTroops(@int, ref troop_ids, battle_side, check_duplicates);
					@int.x++;
				}
				@int.y++;
			}
			return num;
		}

		// Token: 0x060044F7 RID: 17655 RVA: 0x00203874 File Offset: 0x00201A74
		public int EnumTroops(float2 pos, float r, ref TempList<int> troop_ids, int battle_side = -1, bool check_duplicates = false)
		{
			int2 min_tile = TroopCollisions.WorldToGrid(pos - r);
			int2 max_tile = TroopCollisions.WorldToGrid(pos + r);
			return this.EnumTroops(min_tile, max_tile, ref troop_ids, battle_side, check_duplicates);
		}

		// Token: 0x060044F8 RID: 17656 RVA: 0x002038A8 File Offset: 0x00201AA8
		private unsafe bool IgnoreCollision(Troops.Troop troop, Troops.Troop other)
		{
			if (other.HasFlags(Troops.Troop.Flags.Killed | Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed))
			{
				return true;
			}
			if (troop.HasFlags(Troops.Troop.Flags.ClimbingLadder))
			{
				return true;
			}
			if (!troop.HasFlags(Troops.Troop.Flags.AttackMove | Troops.Troop.Flags.Fighting))
			{
				return false;
			}
			if (troop.squad->battle_side != other.squad->battle_side)
			{
				return false;
			}
			if (!other.HasFlags(Troops.Troop.Flags.AttackMove | Troops.Troop.Flags.Fighting))
			{
				return true;
			}
			if (troop.enemy_id >= 0 && other.enemy_id >= 0)
			{
				float num = math.lengthsq(troop.pos2d - troop.enemy.pos2d);
				float num2 = math.lengthsq(other.pos2d - other.enemy.pos2d);
				if (num < num2)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060044F9 RID: 17657 RVA: 0x00203968 File Offset: 0x00201B68
		private unsafe void CheckCollision(Troops.Troop troop, Troops.Troop other, ref Troops.Troop nearest, ref float4 steer)
		{
			if (troop.id == other.id)
			{
				return;
			}
			if (troop.def->is_siege_eq && !other.def->is_siege_eq)
			{
				return;
			}
			if (this.IgnoreCollision(troop, other))
			{
				return;
			}
			float2 pos2d = troop.pos2d;
			float4 vel_spd_t = troop.vel_spd_t;
			float radius = troop.def->radius;
			float2 pos2d2 = other.pos2d;
			float4 vel_spd_t2 = other.vel_spd_t;
			float radius2 = other.def->radius;
			float num = radius + radius2;
			float2 @float = pos2d2 - pos2d;
			float num2 = math.lengthsq(@float);
			float num3 = math.lengthsq(@float);
			if (num3 < 0.0001f)
			{
				float2 rhs = new float2(0.01f, 0.01f);
				troop.pos2d += rhs;
				other.pos2d -= rhs;
				return;
			}
			float num4 = num * num;
			float num5 = 0.9f;
			if (troop.pa_id > 0 && !this.pointers.GetPA(troop.pa_id - 1).IsGround())
			{
				num5 = 0.5f;
			}
			if (other.def->is_siege_eq && troop.squad->def->is_siege_eq)
			{
				float num6 = 1.3f;
				float num7 = num * num6;
				float2 y = math.normalize(@float);
				float num8 = math.dot(math.normalize(troop.squad->dir), y);
				float num9 = math.length(@float);
				if (num9 < num7)
				{
					float num10 = troop.squad->tgtPos.Dist(other.pos);
					float num11 = troop.squad->tgtPos.Dist(troop.pos);
					if (num10 < 0.8f * num && num11 > 0.5f * radius && troop.squad->HasFlags(Troops.SquadData.Flags.Moving))
					{
						troop.SetFlags(Troops.Troop.Flags.Blocked);
					}
					float num12 = num * num5;
					float num13 = num7 - num12;
					float num14 = (num8 > 0f) ? ((num7 - num9) / num13 + (1f + num8) * 0.5f) : 0f;
					float2 lhs = default(float2);
					if (num14 > 0f)
					{
						num14 *= 0.5f;
						float2 float2 = new float2(@float.y, -@float.x);
						float num15 = math.dot(math.normalize(float2), math.normalize(troop.squad->dir));
						float2 x;
						if (other.HasFlags(Troops.Troop.Flags.Moving) && num15 > 0f && math.dot(math.normalize(float2), math.normalize(other.squad->dir)) > 0f)
						{
							if (math.dot(new float2(troop.squad->dir.y, -troop.squad->dir.x), y) > 0f)
							{
								x = float2;
							}
							else
							{
								x = -troop.vel_spd_t.xy;
								num14 = 0.1f * troop.vel_spd_t.z;
							}
						}
						else
						{
							x = ((num15 > 0f) ? float2 : (-float2));
						}
						lhs = math.normalize(x);
					}
					if (steer.z < 0f)
					{
						steer.xy += lhs * num14;
						steer.w += 1f;
					}
					else
					{
						steer = new float4(lhs * num14, -1f, 1f);
					}
				}
			}
			else if (num2 < num4 * num5)
			{
				if (other.def->is_siege_eq && troop.squad->HasFlags(Troops.SquadData.Flags.Moving) && math.dot(math.normalize(vel_spd_t.xy), math.normalize(@float)) > 0.75f)
				{
					nearest = other;
					float num16 = math.sqrt(num3);
					float2 float3 = new float2(@float.y, -@float.x);
					float3 = ((math.dot(math.normalize(float3), math.normalize(vel_spd_t.xy)) > 0f) ? (-float3) : float3);
					float2 float4 = math.normalize(@float + float3) * ((num16 - num) / num16);
					if (steer.z < 0f)
					{
						steer.xy += float4;
						steer.w += 1f;
						return;
					}
					steer = new float4(float4, -1f, 1f);
					float z = (troop.vel_spd_t.z < troop.def->walk_anim_speed) ? troop.vel_spd_t.z : troop.def->walk_anim_speed;
					troop.vel_spd_t = new float4(math.normalize(float4 + troop.vel_spd_t.xy), z, troop.vel_spd_t.w);
					return;
				}
				else
				{
					nearest = other;
					float num17 = math.sqrt(num3);
					float2 float5 = @float * ((num17 - num) / num17);
					if (steer.z < 0f)
					{
						steer.xy += float5;
						steer.w += 1f;
						return;
					}
					steer = new float4(float5, -1f, 1f);
					return;
				}
			}
			if (steer.z < 0f)
			{
				return;
			}
			float2 lhs2 = vel_spd_t.xy * vel_spd_t.z;
			float2 rhs2 = vel_spd_t2.xy * vel_spd_t2.z;
			float2 float6 = lhs2 - rhs2;
			float num18 = math.lengthsq(float6);
			float num19 = math.dot(float6, @float);
			if (num19 < 0f)
			{
				return;
			}
			if (num19 >= num18)
			{
				return;
			}
			float num20 = num19 / num18;
			if (steer.z <= num20 || vel_spd_t.w < num20 || vel_spd_t2.w < num20)
			{
				return;
			}
			float2 x2 = float6 * num20 - @float;
			if (math.lengthsq(x2) >= num4)
			{
				return;
			}
			float2 y2 = new float2(@float.y, -@float.x);
			bool flag = math.dot(x2, y2) < 0f;
			float num22;
			float num23;
			if (num3 > num4)
			{
				float num21 = math.sqrt(num3);
				num22 = num / num21;
				num23 = math.sqrt(1f - num22 * num22);
			}
			else
			{
				num22 = 1f;
				num23 = 0f;
			}
			if (!flag)
			{
				num22 = -num22;
			}
			steer.x = @float.x * num23 - @float.y * num22;
			steer.y = @float.x * num22 + @float.y * num23;
			steer.z = num20;
			steer.w = 1f;
			nearest = other;
		}

		// Token: 0x060044FA RID: 17658 RVA: 0x0020404C File Offset: 0x0020224C
		private void UpdateCollision(Troops.Troop troop, Coord tile, ref Troops.Troop nearest, ref float4 steer)
		{
			int troop_id;
			NativeMultiHashMapIterator<int2> nativeMultiHashMapIterator;
			if (!this.map.TryGetFirstValue(tile, out troop_id, out nativeMultiHashMapIterator))
			{
				return;
			}
			this.CheckCollision(troop, new Troops.Troop(this.pdata, troop.cur_thread_id, troop_id), ref nearest, ref steer);
			while (this.map.TryGetNextValue(out troop_id, ref nativeMultiHashMapIterator))
			{
				this.CheckCollision(troop, new Troops.Troop(this.pdata, troop.cur_thread_id, troop_id), ref nearest, ref steer);
			}
		}

		// Token: 0x060044FB RID: 17659 RVA: 0x002040BC File Offset: 0x002022BC
		private unsafe bool FindCollision(Troops.Troop troop, out Troops.Troop nearest, out float4 steer)
		{
			nearest = default(Troops.Troop);
			steer = new float4(0f, 0f, 1f, 0f);
			float2 @float = troop.pos;
			float num = troop.def->radius + 1f;
			Coord coord = Coord.WorldToGrid(@float - num, 2f);
			Coord coord2 = Coord.WorldToGrid(@float + num, 2f);
			Coord coord3;
			coord3.y = coord.y;
			while (coord3.y <= coord2.y)
			{
				coord3.x = coord.x;
				while (coord3.x <= coord2.x)
				{
					this.UpdateCollision(troop, coord3, ref nearest, ref steer);
					coord3.x++;
				}
				coord3.y++;
			}
			if (steer.w > 0f)
			{
				return true;
			}
			float4 vel_spd_t = troop.vel_spd_t;
			if (vel_spd_t.z == 0f || !math.any(vel_spd_t.xy))
			{
				return false;
			}
			float2 v = @float + vel_spd_t.xy * (vel_spd_t.z * vel_spd_t.w);
			coord3 = Coord.WorldToGrid(@float, 2f);
			Point point = Coord.WorldToLocal(coord3, @float, 2f);
			Point point2 = Coord.WorldToLocal(coord3, v, 2f);
			num /= 2f;
			int num2 = 0;
			Coord tile;
			Coord tile2;
			while (Coord.RayStep(ref coord3, ref point, ref point2, num, out tile, out tile2) && ++num2 <= 100)
			{
				if (coord3.x < coord.x || coord3.x > coord2.x || coord3.y < coord.y || coord3.y > coord2.y)
				{
					this.UpdateCollision(troop, coord3, ref nearest, ref steer);
					if (steer.w > 0f)
					{
						break;
					}
					if (tile.valid)
					{
						this.UpdateCollision(troop, tile, ref nearest, ref steer);
						if (steer.w > 0f)
						{
							break;
						}
					}
					if (tile2.valid)
					{
						this.UpdateCollision(troop, tile2, ref nearest, ref steer);
						if (steer.w > 0f)
						{
							break;
						}
					}
				}
			}
			return steer.w > 0f;
		}

		// Token: 0x060044FC RID: 17660 RVA: 0x00204300 File Offset: 0x00202500
		private unsafe void ValidatePosition(Troops.Troop troop, float2 pos, Troops.Troop other, ref float4 steer)
		{
			if (troop.id == other.id)
			{
				return;
			}
			if (troop.def->is_siege_eq && !other.def->is_siege_eq)
			{
				return;
			}
			float radius = troop.def->radius;
			float2 lhs = other.pos.pos;
			float radius2 = other.def->radius;
			float num = radius + radius2;
			float2 @float = lhs - pos;
			float num2 = math.lengthsq(@float);
			if (num2 < 0.0001f)
			{
				return;
			}
			float num3 = num * num;
			if (num2 >= num3 * 0.9f)
			{
				return;
			}
			float num4 = math.sqrt(num2);
			float2 float2 = @float * ((num4 - num) / num4);
			if (steer.z < 0f)
			{
				steer.xy += float2;
				steer.w += 1f;
				return;
			}
			steer = new float4(float2, -1f, 1f);
		}

		// Token: 0x060044FD RID: 17661 RVA: 0x002043F8 File Offset: 0x002025F8
		private void ValidatePosition(Troops.Troop troop, float2 pos, Coord tile, ref float4 steer)
		{
			int troop_id;
			NativeMultiHashMapIterator<int2> nativeMultiHashMapIterator;
			if (!this.map.TryGetFirstValue(tile, out troop_id, out nativeMultiHashMapIterator))
			{
				return;
			}
			this.ValidatePosition(troop, pos, new Troops.Troop(this.pdata, troop.cur_thread_id, troop_id), ref steer);
			while (this.map.TryGetNextValue(out troop_id, ref nativeMultiHashMapIterator))
			{
				this.ValidatePosition(troop, pos, new Troops.Troop(this.pdata, troop.cur_thread_id, troop_id), ref steer);
			}
		}

		// Token: 0x060044FE RID: 17662 RVA: 0x00204468 File Offset: 0x00202668
		public unsafe bool ValidatePosition(Troops.Troop troop, float2 pos, out float4 steer)
		{
			steer = 0;
			if (troop.fight_status == 0 || troop.pa_id > 0)
			{
				return true;
			}
			steer = new float4(0f, 0f, 1f, 0f);
			float rhs = troop.def->radius + 1f;
			Coord coord = Coord.WorldToGrid(pos - rhs, 2f);
			Coord coord2 = Coord.WorldToGrid(pos + rhs, 2f);
			Coord coord3;
			coord3.y = coord.y;
			while (coord3.y <= coord2.y)
			{
				coord3.x = coord.x;
				while (coord3.x <= coord2.x)
				{
					this.ValidatePosition(troop, pos, coord3, ref steer);
					coord3.x++;
				}
				coord3.y++;
			}
			pos += steer.xy;
			if (steer.w <= 0f)
			{
				return true;
			}
			float4 steer2 = troop.steer;
			if (steer2.z > 0f)
			{
				return false;
			}
			steer.xy /= steer.w;
			float num = math.lengthsq(steer.xy);
			float num2 = math.lengthsq(steer2.xy);
			return num <= num2;
		}

		// Token: 0x060044FF RID: 17663 RVA: 0x002045C0 File Offset: 0x002027C0
		public bool AvoidCollisions(Troops.Troop troop)
		{
			Troops.Troop troop2;
			float4 @float;
			if (!this.FindCollision(troop, out troop2, out @float))
			{
				troop.ClrFlags(Troops.Troop.Flags.Collided);
				troop.steer = new float4(0f, 0f, 1f, 0f);
				return false;
			}
			troop.SetFlags(Troops.Troop.Flags.Collided);
			if (@float.z >= 0f)
			{
				@float.xy = math.normalize(@float.xy);
			}
			else
			{
				@float.xy /= @float.w;
			}
			troop.steer = @float;
			return true;
		}

		// Token: 0x06004500 RID: 17664 RVA: 0x00204650 File Offset: 0x00202850
		public unsafe void CalculateSeparation(Troops.Troop troop)
		{
			float4 @float = default(float4);
			float num = troop.def->radius + 1f;
			float2 lhs = troop.pos;
			Coord coord = Coord.WorldToGrid(lhs - num, 2f);
			Coord coord2 = Coord.WorldToGrid(lhs + num, 2f);
			@float.z = num;
			Coord coord3;
			coord3.y = coord.y;
			while (coord3.y <= coord2.y)
			{
				coord3.x = coord.x;
				while (coord3.x <= coord2.x)
				{
					this.Separate(troop, coord3, ref @float);
					coord3.x++;
				}
				coord3.y++;
			}
			if (@float.w > 0f)
			{
				@float /= @float.w;
			}
			troop.separation = @float * -1f;
		}

		// Token: 0x06004501 RID: 17665 RVA: 0x00204744 File Offset: 0x00202944
		private void Separate(Troops.Troop troop, Coord tile, ref float4 separation)
		{
			int troop_id;
			NativeMultiHashMapIterator<int2> nativeMultiHashMapIterator;
			if (!this.map.TryGetFirstValue(tile, out troop_id, out nativeMultiHashMapIterator))
			{
				return;
			}
			this.CheckSeparation(troop, new Troops.Troop(this.pdata, troop.cur_thread_id, troop_id), ref separation);
			while (this.map.TryGetNextValue(out troop_id, ref nativeMultiHashMapIterator))
			{
				this.CheckSeparation(troop, new Troops.Troop(this.pdata, troop.cur_thread_id, troop_id), ref separation);
			}
		}

		// Token: 0x06004502 RID: 17666 RVA: 0x002047B0 File Offset: 0x002029B0
		private unsafe void CheckSeparation(Troops.Troop troop, Troops.Troop other, ref float4 separation)
		{
			if (troop.id == other.id || troop.squad->battle_side != other.squad->battle_side)
			{
				return;
			}
			float2 rhs = troop.pos;
			float2 x = other.pos - rhs;
			float num = math.length(x);
			if (num > separation.z || num <= 0.0001f)
			{
				return;
			}
			separation.xy += math.normalize(x) * math.clamp(separation.z - num, 0f, 1f);
			separation.w += 1f;
		}

		// Token: 0x06004503 RID: 17667 RVA: 0x00204864 File Offset: 0x00202A64
		public unsafe bool CheckFuturePosition(Troops.Troop troop, float2 pos, bool ignore_siege_eq = true)
		{
			float rhs = troop.def->radius * 3f;
			Coord coord = Coord.WorldToGrid(pos - rhs, 2f);
			Coord coord2 = Coord.WorldToGrid(pos + rhs, 2f);
			bool3 @bool = false;
			Coord coord3;
			coord3.y = coord.y;
			while (coord3.y <= coord2.y)
			{
				coord3.x = coord.x;
				while (coord3.x <= coord2.x)
				{
					if (!this.CheckFuturePosition(troop, pos, coord3, ref @bool, ignore_siege_eq))
					{
						return false;
					}
					coord3.x++;
				}
				coord3.y++;
			}
			return true;
		}

		// Token: 0x06004504 RID: 17668 RVA: 0x00204920 File Offset: 0x00202B20
		private bool CheckFuturePosition(Troops.Troop troop, float2 pos, Coord tile, ref bool3 blocked, bool ignore_siege_eq = true)
		{
			int troop_id;
			NativeMultiHashMapIterator<int2> nativeMultiHashMapIterator;
			if (!this.map.TryGetFirstValue(tile, out troop_id, out nativeMultiHashMapIterator))
			{
				return true;
			}
			for (;;)
			{
				this.CheckFuturePosition(troop, pos, new Troops.Troop(this.pdata, troop.cur_thread_id, troop_id), ref blocked, ignore_siege_eq);
				if ((blocked.x && blocked.y) || blocked.z)
				{
					break;
				}
				if (!this.map.TryGetNextValue(out troop_id, ref nativeMultiHashMapIterator))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004505 RID: 17669 RVA: 0x00204994 File Offset: 0x00202B94
		private unsafe void CheckFuturePosition(Troops.Troop troop, float2 pos, Troops.Troop other, ref bool3 blocked, bool ignore_siege_eq = true)
		{
			if (troop.id == other.id)
			{
				return;
			}
			if (troop.squad->battle_side != other.squad->battle_side)
			{
				return;
			}
			if (other.def->is_siege_eq && ignore_siege_eq)
			{
				return;
			}
			float2 pos2d = other.pos2d;
			float2 @float = math.normalize(pos - troop.pos2d);
			float radius = troop.def->radius;
			float radius2 = other.def->radius;
			float num = radius + radius2;
			if (!other.def->is_siege_eq && num > 1.5f)
			{
				num = 1.5f;
			}
			float2 float2 = pos2d - troop.pos2d;
			math.lengthsq(float2);
			float2 float3 = pos2d - pos;
			float num2 = math.lengthsq(float3);
			float num3 = num * num;
			if (num2 >= num3 * 0.9f)
			{
				return;
			}
			float3 = math.normalize(float3);
			if (math.dot(@float, float2) > 0.966f)
			{
				blocked.z = true;
				return;
			}
			if (math.dot(new float2(@float.y, -@float.x), float3) > 0f)
			{
				blocked.y = true;
				return;
			}
			blocked.x = true;
		}

		// Token: 0x040031A3 RID: 12707
		[NativeDisableUnsafePtrRestriction]
		public unsafe Troops.TroopsPtrData* pdata;

		// Token: 0x040031A4 RID: 12708
		[ReadOnly]
		private NativeMultiHashMap<int2, int> map;

		// Token: 0x040031A5 RID: 12709
		[NativeDisableUnsafePtrRestriction]
		public PathData.DataPointers pointers;
	}
}
