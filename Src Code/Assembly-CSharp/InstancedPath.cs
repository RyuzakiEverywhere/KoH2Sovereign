using System;
using System.Collections.Generic;
using Logic;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000187 RID: 391
public class InstancedPath : VisibilityDetector.IVisibilityChanged
{
	// Token: 0x06001587 RID: 5511 RVA: 0x000DB2EC File Offset: 0x000D94EC
	public void ClearTravellers()
	{
		if (this.travellers == null || this.travellers.Count == 0)
		{
			return;
		}
		int count = this.travellers.Count;
		for (int i = 0; i < count; i++)
		{
			InstancedPath.Traveller traveller = this.travellers[i];
			if (traveller.instances != null)
			{
				for (int j = traveller.instances.Count - 1; j >= 0; j--)
				{
					traveller.instances.RemoveAt(j);
				}
			}
		}
		this.travellers = null;
	}

	// Token: 0x06001588 RID: 5512 RVA: 0x000DB368 File Offset: 0x000D9568
	[ConsoleMethod("travellers")]
	public static void SetTravellersVisible(int visible)
	{
		InstancedPath.travellers_visible = (visible == 1);
		AutoRoads autoRoads = UnityEngine.Object.FindObjectOfType<AutoRoads>();
		if (autoRoads == null)
		{
			return;
		}
		if (!InstancedPath.travellers_visible)
		{
			for (int i = 0; i < autoRoads.instanced_roads.Count; i++)
			{
				autoRoads.instanced_roads[i].HideTravellers();
			}
		}
	}

	// Token: 0x06001589 RID: 5513 RVA: 0x000DB3BC File Offset: 0x000D95BC
	private void Init()
	{
		if (this.travellers != null)
		{
			return;
		}
		if (Application.isPlaying)
		{
			WorldMap worldMap = WorldMap.Get();
			VisibilityDetector.Add(this.bounds.center, this.bounds.extents.magnitude, null, this, this.animalLayer);
			this.animalLayer = LayerMask.NameToLayer("Animals");
			Game game = GameLogic.Get(true);
			if (game == null || this.path_points == null || this.path_points.Count <= 0)
			{
				return;
			}
			if (this.start_realm == null)
			{
				Logic.Settlement settlement = game.multiplayer.objects.Get(Serialization.ObjectType.Village, this.src) as Logic.Settlement;
				if (settlement == null)
				{
					settlement = (game.multiplayer.objects.Get(Serialization.ObjectType.Castle, this.src) as Logic.Settlement);
				}
				this.start_realm = ((settlement != null) ? settlement.GetRealm() : null);
			}
			if (this.start_realm != null)
			{
				global::Settlement settlement2 = this.start_realm.castle.visuals as global::Settlement;
				if (settlement2.citadel != null)
				{
					this.culture = settlement2.citadel.architecture;
				}
			}
			this.travellers = new List<InstancedPath.Traveller>();
			int total_travellers_per_road = Traveler.prefabs[0].total_travellers_per_road;
			List<Traveler.Def> list = new List<Traveler.Def>();
			if (this.culture != null)
			{
				for (int i = 0; i < Traveler.prefabs.Count; i++)
				{
					if (Traveler.prefabs[i].culture.Count == 0 || Traveler.prefabs[i].culture.Contains(this.culture))
					{
						list.Add(Traveler.prefabs[i]);
					}
				}
			}
			for (int j = 0; j < total_travellers_per_road; j++)
			{
				InstancedPath.Traveller traveller = new InstancedPath.Traveller();
				if (list.Count > 0)
				{
					traveller.def = list[UnityEngine.Random.Range(0, list.Count)];
				}
				else
				{
					traveller.def = Traveler.prefabs[UnityEngine.Random.Range(0, Traveler.prefabs.Count)];
				}
				traveller.instances = new List<InstancedPath.Traveller.Instance>();
				float num = this.path_len / traveller.def.movement_speed;
				for (int k = 0; k < traveller.def.travellers_per_road; k++)
				{
					InstancedPath.Traveller.Instance instance = new InstancedPath.Traveller.Instance();
					instance.src = traveller;
					instance.start_time = UnityEngine.Time.time - num * UnityEngine.Random.Range(0f, 1f) + UnityEngine.Random.Range(0f, traveller.def.min_spawn_interval);
					traveller.instances.Add(instance);
					instance.obj_instanced = new AnimatorInstancer(worldMap.texture_baker, traveller.def.field, null, 0, 1f);
					if (traveller.def.has_addon)
					{
						instance.addon = new InstancedPath.Traveller.Instance.TravellerAddon(instance, new AnimatorInstancer(worldMap.texture_baker, traveller.def.field, "_addon", 0, 1f));
					}
				}
				this.travellers.Add(traveller);
			}
		}
	}

	// Token: 0x0600158A RID: 5514 RVA: 0x000DB6DC File Offset: 0x000D98DC
	public void UpdateTravellers()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		this.Init();
		if (!InstancedPath.travellers_visible)
		{
			return;
		}
		if (InstancedPath.cam == null)
		{
			InstancedPath.cam = CameraController.MainCamera;
		}
		if (InstancedPath.fppl == null && InstancedPath.cam != null)
		{
			InstancedPath.fppl = InstancedPath.cam.GetComponent<FarPlanePerLayer>();
		}
		float num = InstancedPath.fppl.layerCullDistances[this.animalLayer];
		num *= num;
		float time = UnityEngine.Time.time;
		if (!this.visible && time < this.next_update_time)
		{
			return;
		}
		if (this.travellers == null)
		{
			return;
		}
		this.next_update_time = time + 10f;
		Game game = GameLogic.Get(true);
		int count = this.travellers.Count;
		for (int i = 0; i < count; i++)
		{
			InstancedPath.Traveller traveller = this.travellers[i];
			if (traveller.def.min_spawn_interval > 0f && traveller.def.min_spawn_interval <= traveller.def.max_spawn_interval && traveller.def.movement_speed > 0f)
			{
				for (int j = 0; j < traveller.instances.Count; j++)
				{
					InstancedPath.Traveller.Instance instance = traveller.instances[j];
					float num2 = (time - instance.start_time) * traveller.def.movement_speed;
					if (num2 >= this.path_len - traveller.def.finish_path_offset)
					{
						if (this.start_realm == null || game == null)
						{
							if (instance != null)
							{
								instance.Enable(false, false);
							}
						}
						else
						{
							if (this.start_realm != null && this.start_realm.castle != null && this.start_realm.castle.battle == null)
							{
								instance.start_time = time + UnityEngine.Random.Range(traveller.def.min_spawn_interval, traveller.def.max_spawn_interval);
							}
							if (instance != null)
							{
								instance.Enable(false, false);
							}
						}
					}
					else if (this.visible)
					{
						if (time > instance.start_time)
						{
							float deltaTime = UnityEngine.Time.deltaTime;
							Profile.BeginSection("TerrainPath.MoveTraveller");
							Vector3 vector;
							Vector3 a;
							TerrainPathFinder.GetPathPoint(this.path_points, num2, out vector, out a, traveller.def.look_ahead);
							bool flag = false;
							if (Vector3.SqrMagnitude(vector - InstancedPath.cam.transform.position) > num)
							{
								instance.Enable(false, false);
							}
							else
							{
								if (instance.Enable(true, false))
								{
									flag = true;
									Profile.BeginSection("TerrainPath.SetKingdomColor");
									Logic.Realm realm = this.start_realm;
									Logic.Kingdom kingdom = (realm != null) ? realm.GetKingdom() : null;
									if (kingdom != null)
									{
										global::Kingdom kingdom2 = kingdom.visuals as global::Kingdom;
										int color_id;
										if (kingdom2 != null)
										{
											color_id = kingdom2.unitColorID;
										}
										else
										{
											MapData mapData = MapData.Get();
											color_id = global::Kingdom.PickClosestColorID((mapData != null) ? mapData.unit_colors : null, global::Defs.GetColor(kingdom.def, "primary_color", null));
										}
										instance.obj_instanced.UpdateKingdomColor(color_id);
										if (instance.addon != null)
										{
											instance.addon.obj_instanced.UpdateKingdomColor(color_id);
										}
									}
									Profile.EndSection("TerrainPath.SetKingdomColor");
								}
								Vector3 forward = a - vector;
								forward.y = 0f;
								if (forward.sqrMagnitude > 0.01f)
								{
									Profile.BeginSection("TerrainPath.UpdateTraveller");
									if (flag && instance.addon != null)
									{
										instance.addon.ResetPos();
									}
									Quaternion quaternion = instance.obj_instanced.Rotation;
									quaternion = Quaternion.RotateTowards(quaternion, Quaternion.LookRotation(forward), UnityEngine.Time.deltaTime * 45f);
									instance.obj_instanced.Update(deltaTime, vector, quaternion, CameraController.GameCamera.f_planes, false);
									if (instance.addon != null)
									{
										instance.addon.Update(deltaTime);
									}
									Profile.EndSection("TerrainPath.UpdateTraveller");
								}
							}
							Profile.EndSection("TerrainPath.MoveTraveller");
						}
						else if (instance != null)
						{
							instance.Enable(false, false);
						}
					}
				}
			}
		}
	}

	// Token: 0x0600158B RID: 5515 RVA: 0x000DBAE0 File Offset: 0x000D9CE0
	public void HideTravellers()
	{
		if (this.travellers == null || this.travellers.Count == 0)
		{
			return;
		}
		int count = this.travellers.Count;
		for (int i = 0; i < count; i++)
		{
			InstancedPath.Traveller traveller = this.travellers[i];
			if (traveller.instances != null && traveller.instances.Count != 0)
			{
				for (int j = 0; j < this.travellers[i].instances.Count; j++)
				{
					this.travellers[i].instances[j].Enable(false, false);
				}
			}
		}
	}

	// Token: 0x0600158C RID: 5516 RVA: 0x000DBB7D File Offset: 0x000D9D7D
	public void VisibilityChanged(bool visible)
	{
		this.visible = (visible && InstancedPath.travellers_visible);
		if (!visible)
		{
			this.HideTravellers();
		}
	}

	// Token: 0x04000DE0 RID: 3552
	public List<Vector3> path_points;

	// Token: 0x04000DE1 RID: 3553
	public float path_len;

	// Token: 0x04000DE2 RID: 3554
	public List<InstancedPath.Traveller> travellers;

	// Token: 0x04000DE3 RID: 3555
	public int src;

	// Token: 0x04000DE4 RID: 3556
	public int dest;

	// Token: 0x04000DE5 RID: 3557
	public string culture;

	// Token: 0x04000DE6 RID: 3558
	private bool visible;

	// Token: 0x04000DE7 RID: 3559
	private float next_update_time;

	// Token: 0x04000DE8 RID: 3560
	private int animalLayer;

	// Token: 0x04000DE9 RID: 3561
	public Transform TravellersTransform;

	// Token: 0x04000DEA RID: 3562
	public Bounds bounds;

	// Token: 0x04000DEB RID: 3563
	private Logic.Realm start_realm;

	// Token: 0x04000DEC RID: 3564
	private static Camera cam;

	// Token: 0x04000DED RID: 3565
	private static FarPlanePerLayer fppl;

	// Token: 0x04000DEE RID: 3566
	public static bool travellers_visible = true;

	// Token: 0x020006C1 RID: 1729
	public class Traveller
	{
		// Token: 0x040036DE RID: 14046
		public Traveler.Def def;

		// Token: 0x040036DF RID: 14047
		[NonSerialized]
		public List<InstancedPath.Traveller.Instance> instances;

		// Token: 0x040036E0 RID: 14048
		[NonSerialized]
		public float next_spawn_time;

		// Token: 0x02000A0A RID: 2570
		public class Instance
		{
			// Token: 0x0600553F RID: 21823 RVA: 0x00248B35 File Offset: 0x00246D35
			public bool Enable(bool enable, bool force = false)
			{
				if (this.enabled == enable && !force)
				{
					return false;
				}
				if (this.addon != null)
				{
					this.addon.Enable(enable, false);
				}
				this.enabled = enable;
				return true;
			}

			// Token: 0x04004646 RID: 17990
			public InstancedPath.Traveller src;

			// Token: 0x04004647 RID: 17991
			public AnimatorInstancer obj_instanced;

			// Token: 0x04004648 RID: 17992
			public InstancedPath.Traveller.Instance.TravellerAddon addon;

			// Token: 0x04004649 RID: 17993
			public float start_time;

			// Token: 0x0400464A RID: 17994
			public bool enabled;

			// Token: 0x02000A4E RID: 2638
			public class TravellerAddon
			{
				// Token: 0x06005630 RID: 22064 RVA: 0x0024DD94 File Offset: 0x0024BF94
				public bool Enable(bool enable, bool force = false)
				{
					if (this.enabled == enable && !force)
					{
						return false;
					}
					this.enabled = enable;
					return true;
				}

				// Token: 0x06005631 RID: 22065 RVA: 0x0024DDAC File Offset: 0x0024BFAC
				private void GetFollowPosRot(out float3 pos, out Quaternion rot, out float3 forward)
				{
					pos = this.follow.obj_instanced.Position;
					rot = this.follow.obj_instanced.Rotation;
					forward = rot * Vector3.forward;
					pos += rot * this.follow.src.def.pivot_offset;
				}

				// Token: 0x06005632 RID: 22066 RVA: 0x0024DE36 File Offset: 0x0024C036
				public TravellerAddon(InstancedPath.Traveller.Instance follow, AnimatorInstancer obj_instanced)
				{
					this.follow = follow;
					this.obj_instanced = obj_instanced;
					this.min_dist = follow.src.def.min_dist;
					this.ResetPos();
				}

				// Token: 0x06005633 RID: 22067 RVA: 0x0024DE74 File Offset: 0x0024C074
				public void ResetPos()
				{
					float3 @float;
					Quaternion quaternion;
					float3 forward;
					this.GetFollowPosRot(out @float, out quaternion, out forward);
					this.cur_pos = @float;
					this.cur_rot = quaternion;
					this.FinalizePosition(0f, this.cur_pos, this.cur_rot, forward);
				}

				// Token: 0x06005634 RID: 22068 RVA: 0x0024DEB4 File Offset: 0x0024C0B4
				public void Update(float dt)
				{
					float3 @float;
					Quaternion rot;
					float3 forward;
					this.GetFollowPosRot(out @float, out rot, out forward);
					if (dt == 0f)
					{
						float3 x = @float - this.cur_pos;
						x.y = 0f;
						this.FinalizePosition(dt, this.cur_pos, this.cur_rot, math.normalize(x));
						return;
					}
					this.MoveTowards(dt, @float, rot, forward, false);
				}

				// Token: 0x06005635 RID: 22069 RVA: 0x0024DF14 File Offset: 0x0024C114
				private void MoveTowards(float dt, float3 pos, Quaternion rot, float3 forward, bool instant = false)
				{
					float3 @float = pos - this.cur_pos;
					@float.y = 0f;
					float num = math.length(@float);
					@float /= num;
					if (num <= 0.1f)
					{
						this.FinalizePosition(dt, this.cur_pos, this.cur_rot, @float);
						return;
					}
					if (num > this.min_dist * 2f)
					{
						this.cur_pos = pos;
						this.cur_rot = rot;
					}
					else if (num < this.min_dist)
					{
						this.FinalizePosition(dt, this.cur_pos, this.cur_rot, @float);
						return;
					}
					Quaternion to = Quaternion.LookRotation(@float);
					float3 v = pos - @float * this.min_dist;
					if (instant)
					{
						this.cur_pos = v;
						this.cur_rot = to;
					}
					else
					{
						this.cur_pos = Vector3.MoveTowards(this.cur_pos, v, dt * 2f * this.speed_mod);
						this.cur_rot = Quaternion.RotateTowards(this.cur_rot, to, dt * 60f);
					}
					this.FinalizePosition(dt, this.cur_pos, this.cur_rot, @float);
				}

				// Token: 0x06005636 RID: 22070 RVA: 0x0024E034 File Offset: 0x0024C234
				private void FinalizePosition(float dt, float3 pos, Quaternion rot, float3 forward)
				{
					GameCamera gameCamera = CameraController.GameCamera;
					pos = global::Common.SnapToTerrain(pos + forward * this.follow.src.def.addon_offset, 0f, null, -1f, false);
					this.obj_instanced.Update(dt, pos, rot, gameCamera.f_planes, false);
				}

				// Token: 0x0400474C RID: 18252
				public AnimatorInstancer obj_instanced;

				// Token: 0x0400474D RID: 18253
				public float min_dist;

				// Token: 0x0400474E RID: 18254
				public InstancedPath.Traveller.Instance follow;

				// Token: 0x0400474F RID: 18255
				private float3 cur_pos;

				// Token: 0x04004750 RID: 18256
				private Quaternion cur_rot;

				// Token: 0x04004751 RID: 18257
				public float speed_mod = 1f;

				// Token: 0x04004752 RID: 18258
				private bool enabled;
			}
		}
	}
}
