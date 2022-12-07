using System;
using System.Collections.Generic;
using System.Threading;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x0200011E RID: 286
[ExecuteInEditMode]
public class LabelUpdater : MonoBehaviour
{
	// Token: 0x06000D04 RID: 3332 RVA: 0x00093A27 File Offset: 0x00091C27
	public static LabelUpdater Get(bool create = true)
	{
		if (LabelUpdater.instance == null && create)
		{
			LabelUpdater.instance = new GameObject("LabelUpdater").AddComponent<LabelUpdater>();
		}
		return LabelUpdater.instance;
	}

	// Token: 0x06000D05 RID: 3333 RVA: 0x00093A54 File Offset: 0x00091C54
	private void LoadPrefabsDef()
	{
		DT.Field defField = global::Defs.GetDefField("PoliticalView", null);
		if (defField == null)
		{
			return;
		}
		this.RealmLabelPrefab = LabelUpdater.GetlabelPrefabLocalzied(defField, "realm_label_prefab");
		this.KingdomLabelPrefab = LabelUpdater.GetlabelPrefabLocalzied(defField, "kingdom_label_prefab");
		this.GoldIncomePrefab = global::Defs.GetObj<GameObject>(defField, "gold_income_prefab", null);
	}

	// Token: 0x06000D06 RID: 3334 RVA: 0x00093AA8 File Offset: 0x00091CA8
	private static GameObject GetlabelPrefabLocalzied(DT.Field field, string key)
	{
		GameObject gameObject = null;
		if (!string.IsNullOrEmpty(global::Defs.Language))
		{
			gameObject = global::Defs.GetObj<GameObject>(field, key + "." + global::Defs.Language, null);
		}
		if (gameObject == null)
		{
			gameObject = global::Defs.GetObj<GameObject>(field, key, null);
		}
		return gameObject;
	}

	// Token: 0x06000D07 RID: 3335 RVA: 0x00093AEE File Offset: 0x00091CEE
	public void SetThreads()
	{
		this.SetThreads(Environment.ProcessorCount);
	}

	// Token: 0x06000D08 RID: 3336 RVA: 0x00093AFC File Offset: 0x00091CFC
	public void SetThreads(int number = 1)
	{
		using (Game.Profile("LabelUpdater.SetThreads", false, 0f, null))
		{
			if (number != this.numActiveThreads || this.thread == null || this.extraThreads.Count != number - 1)
			{
				if (number < 1)
				{
					number = 1;
				}
				this.AbortThreads();
				this.thread = new Thread(new ThreadStart(this.ThreadFunc));
				this.thread.Name = "Label Updater Main";
				this.thread.Start();
				this.numActiveThreads = number;
				while (number > 1)
				{
					Thread thread = new Thread(new ThreadStart(this.ThreadFunc));
					thread.Name = "Label Updater Extra" + (number - 1);
					thread.Start();
					this.extraThreads.Add(thread);
					number--;
				}
			}
		}
	}

	// Token: 0x06000D09 RID: 3337 RVA: 0x00093BF0 File Offset: 0x00091DF0
	private void Awake()
	{
		if (LabelUpdater.instance != null && LabelUpdater.instance != this)
		{
			UnityEngine.Object.Destroy(this);
			return;
		}
		if (LabelUpdater.instance == null)
		{
			LabelUpdater.instance = this;
		}
		if (Application.isPlaying)
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
		MapData politicalOnly = MapData.GetPoliticalOnly();
		this.terrainBounds = ((politicalOnly != null) ? politicalOnly.GetTerrainBounds() : default(Bounds));
		this.SetThreads();
		this.layer = LayerMask.NameToLayer("Political");
	}

	// Token: 0x06000D0A RID: 3338 RVA: 0x00093C78 File Offset: 0x00091E78
	private void OnEnable()
	{
		this.LoadPrefabsDef();
	}

	// Token: 0x06000D0B RID: 3339 RVA: 0x00093C80 File Offset: 0x00091E80
	public void AbortThreads()
	{
		using (Game.Profile("LabelUpdater.AbortThreads", false, 0f, null))
		{
			if (this.thread != null)
			{
				this.thread.Abort();
				this.thread = null;
			}
			while (this.extraThreads.Count > 0)
			{
				this.extraThreads[0].Abort();
				this.extraThreads.RemoveAt(0);
			}
			this.processing.Clear();
		}
	}

	// Token: 0x06000D0C RID: 3340 RVA: 0x00093D14 File Offset: 0x00091F14
	public static void PrintThreadsStates()
	{
		LabelUpdater.Get(true);
		Debug.Log(LabelUpdater.instance.thread.Name + " : " + LabelUpdater.instance.thread.ThreadState.ToString());
		for (int i = 0; i < LabelUpdater.instance.extraThreads.Count; i++)
		{
			Debug.Log(LabelUpdater.instance.extraThreads[i].Name + " : " + LabelUpdater.instance.extraThreads[i].ThreadState.ToString());
		}
	}

	// Token: 0x06000D0D RID: 3341 RVA: 0x00093DC4 File Offset: 0x00091FC4
	public static bool IsProcessing()
	{
		LabelUpdater labelUpdater = LabelUpdater.Get(true);
		if (labelUpdater.thread == null)
		{
			return false;
		}
		if (labelUpdater.thread.ThreadState.Equals(ThreadState.Running))
		{
			return true;
		}
		for (int i = 0; i < labelUpdater.extraThreads.Count; i++)
		{
			if (labelUpdater.extraThreads[i].ThreadState.Equals(ThreadState.Running))
			{
				return true;
			}
		}
		return (labelUpdater.pending.Count > 0 && labelUpdater.enabled) || (labelUpdater.enqueued.Count > 0 && labelUpdater.enabled);
	}

	// Token: 0x06000D0E RID: 3342 RVA: 0x00093E75 File Offset: 0x00092075
	private void OnDestroy()
	{
		this.AbortThreads();
		LabelUpdater.instance = null;
	}

	// Token: 0x06000D0F RID: 3343 RVA: 0x00093E83 File Offset: 0x00092083
	private void ThreadFunc()
	{
		for (;;)
		{
			if (!this.Process())
			{
				this.resume.WaitOne();
			}
		}
	}

	// Token: 0x06000D10 RID: 3344 RVA: 0x00093E9C File Offset: 0x0009209C
	public bool Process()
	{
		LabelUpdater.PendingInfo info = new LabelUpdater.PendingInfo(null, null, null);
		object @lock = this.Lock;
		lock (@lock)
		{
			if (this.pending.Count <= 0)
			{
				return false;
			}
			int i = 0;
			Predicate<LabelUpdater.PendingInfo> <>9__0;
			Predicate<LabelUpdater.PendingInfo> <>9__1;
			while (i < this.pending.Count)
			{
				info = this.pending[i];
				if (info.k == null)
				{
					goto IL_91;
				}
				List<LabelUpdater.PendingInfo> list = this.processing;
				Predicate<LabelUpdater.PendingInfo> match;
				if ((match = <>9__0) == null)
				{
					match = (<>9__0 = ((LabelUpdater.PendingInfo inf) => inf.k == info.k));
				}
				if (list.FindIndex(match) == -1)
				{
					goto IL_91;
				}
				IL_ED:
				i++;
				continue;
				IL_91:
				if (info.r != null)
				{
					List<LabelUpdater.PendingInfo> list2 = this.processing;
					Predicate<LabelUpdater.PendingInfo> match2;
					if ((match2 = <>9__1) == null)
					{
						match2 = (<>9__1 = ((LabelUpdater.PendingInfo inf) => inf.r == info.r));
					}
					if (list2.FindIndex(match2) != -1)
					{
						goto IL_ED;
					}
				}
				this.pending.RemoveAt(i);
				this.processing.Add(info);
				break;
			}
		}
		this.DecideLabelPlacement(info);
		return true;
	}

	// Token: 0x06000D11 RID: 3345 RVA: 0x00093FDC File Offset: 0x000921DC
	private static void Enque(global::Realm r, Action<LabelUpdater.CompletedInfo> createIconAction)
	{
		LabelUpdater.instance = LabelUpdater.Get(false);
		if (LabelUpdater.instance == null || r == null || createIconAction == null)
		{
			return;
		}
		if (LabelUpdater.instance.enqueued.FindIndex((LabelUpdater.PendingInfo p) => ((p.r == null) ? 0 : p.r.id) == r.id) != -1)
		{
			return;
		}
		LabelUpdater labelUpdater = LabelUpdater.instance;
		MapData politicalOnly = MapData.GetPoliticalOnly();
		labelUpdater.terrainBounds = ((politicalOnly != null) ? politicalOnly.GetTerrainBounds() : default(Bounds));
		LabelUpdater.instance.enqueued.Add(new LabelUpdater.PendingInfo(r, null, createIconAction));
		LabelUpdater.instance.enabled = true;
	}

	// Token: 0x06000D12 RID: 3346 RVA: 0x00094084 File Offset: 0x00092284
	private static void Enque(global::Kingdom k, Action<LabelUpdater.CompletedInfo> createIconAction)
	{
		LabelUpdater.instance = LabelUpdater.Get(true);
		if (LabelUpdater.instance == null || k == null || createIconAction == null)
		{
			return;
		}
		if (LabelUpdater.instance.enqueued.FindIndex((LabelUpdater.PendingInfo p) => ((p.k == null) ? 0 : p.k.id) == k.id) != -1)
		{
			return;
		}
		LabelUpdater labelUpdater = LabelUpdater.instance;
		MapData politicalOnly = MapData.GetPoliticalOnly();
		labelUpdater.terrainBounds = ((politicalOnly != null) ? politicalOnly.GetTerrainBounds() : default(Bounds));
		LabelUpdater.instance.enqueued.Insert(0, new LabelUpdater.PendingInfo(null, k, createIconAction));
		LabelUpdater.instance.enabled = true;
	}

	// Token: 0x06000D13 RID: 3347 RVA: 0x0009412C File Offset: 0x0009232C
	public static int NumPending()
	{
		LabelUpdater.instance = LabelUpdater.Get(true);
		object @lock = LabelUpdater.instance.Lock;
		int count;
		lock (@lock)
		{
			count = LabelUpdater.instance.pending.Count;
		}
		return count;
	}

	// Token: 0x06000D14 RID: 3348 RVA: 0x00094188 File Offset: 0x00092388
	public void Update()
	{
		object @lock = this.Lock;
		lock (@lock)
		{
			for (int i = 0; i < this.completed.Count; i++)
			{
				LabelUpdater.CompletedInfo completedInfo = this.completed[i];
				if (completedInfo.pendingInfo.createLabelAction != null)
				{
					completedInfo.pendingInfo.createLabelAction(completedInfo);
				}
			}
			this.completed.Clear();
		}
		if (!LabelUpdater.IsProcessing())
		{
			LabelUpdater.instance.enabled = false;
		}
	}

	// Token: 0x06000D15 RID: 3349 RVA: 0x00094220 File Offset: 0x00092420
	public void ProcessEnqueued()
	{
		if (this.enqueued.Count == 0)
		{
			return;
		}
		object @lock = this.Lock;
		lock (@lock)
		{
			this.pending.AddRange(this.enqueued);
			this.enqueued.Clear();
			int num = Math.Min(this.pending.Count, this.extraThreads.Count + 1);
			for (int i = 0; i < num; i++)
			{
				LabelUpdater.instance.resume.Set();
			}
		}
	}

	// Token: 0x06000D16 RID: 3350 RVA: 0x000942C0 File Offset: 0x000924C0
	private void LateUpdate()
	{
		this.ProcessEnqueued();
	}

	// Token: 0x06000D17 RID: 3351 RVA: 0x000942C8 File Offset: 0x000924C8
	public bool DecideLabelPlacement(LabelUpdater.PendingInfo info)
	{
		object @lock;
		if ((info.k == null && info.r == null) || (info.k != null && info.r != null))
		{
			@lock = this.Lock;
			lock (@lock)
			{
				this.processing.Remove(info);
			}
			return false;
		}
		if (info.createLabelAction == null)
		{
			@lock = this.Lock;
			lock (@lock)
			{
				this.processing.Remove(info);
			}
			return false;
		}
		List<LabelUpdater.CompletedInfo> list = new List<LabelUpdater.CompletedInfo>();
		float num = 5f;
		float d = 5f;
		float num2 = 2.5f;
		List<Bounds> list2;
		int id;
		string name;
		bool bKingdom;
		if (info.k != null)
		{
			list2 = info.k.CalcRegionsBounds();
			id = info.k.id;
			name = info.k.Name;
			bKingdom = true;
		}
		else
		{
			list2 = new List<Bounds>();
			list2.Add(info.r.bounds);
			id = info.r.id;
			name = info.r.Name;
			bKingdom = false;
		}
		if (name.Length < 1)
		{
			@lock = this.Lock;
			lock (@lock)
			{
				this.processing.Remove(info);
			}
			return false;
		}
		for (int i = 0; i < list2.Count; i++)
		{
			Bounds bounds = list2[i];
			if (bounds.size == Vector3.zero)
			{
				@lock = this.Lock;
				lock (@lock)
				{
					this.processing.Remove(info);
				}
				return false;
			}
			Vector3 vector = new Vector3(0.5f, 0f, 0.5f * num2 / (float)name.Length);
			Vector3 vector2 = vector;
			vector2.z = -vector2.z;
			Vector3 v = vector;
			v.x = 0f;
			float vmax = Mathf.Max(bounds.extents.x, bounds.extents.z);
			Vector3 ptCenter = Vector3.zero;
			float angle = 0f;
			float num3 = 0f;
			float num4 = 0f;
			for (float num5 = -90f; num5 < 90f; num5 += 5f)
			{
				Vector3 vector3 = MeshUtils.RotateVector2D(Vector3.right * d, num5);
				Vector3 a = MeshUtils.RotateVector2D(vector, num5);
				Vector3 a2 = MeshUtils.RotateVector2D(vector2, num5);
				Vector3 a3 = MeshUtils.RotateVector2D(v, num5);
				if (vector3.x != 0f)
				{
					for (float num6 = bounds.min.z + num; num6 <= bounds.max.z - num; num6 += num)
					{
						Vector3 pt = new Vector3(bounds.min.x, 0f, num6);
						LabelUpdater.RDRayTracer rdrayTracer = new LabelUpdater.RDRayTracer(bounds, pt, vector3, id, bKingdom, this.terrainBounds);
						Vector3 vector4 = rdrayTracer.ptEnter + (rdrayTracer.ptLeave - rdrayTracer.ptEnter) / 2f;
						if (rdrayTracer.len > num3)
						{
							float num7 = 0f;
							for (float num8 = 1f; num8 >= 0.5f; num8 -= 0.1f)
							{
								float num9 = rdrayTracer.len * num8;
								if (num9 <= num3)
								{
									break;
								}
								Vector3 b = a * num9;
								Vector3 pt2 = vector4 + b;
								if (rdrayTracer.MatchAt(pt2, id, false, this.terrainBounds))
								{
									pt2 = vector4 - b;
									if (rdrayTracer.MatchAt(pt2, id, false, this.terrainBounds))
									{
										b = a2 * num9;
										pt2 = vector4 + b;
										if (rdrayTracer.MatchAt(pt2, id, false, this.terrainBounds))
										{
											pt2 = vector4 - b;
											if (rdrayTracer.MatchAt(pt2, id, false, this.terrainBounds))
											{
												Vector3 b2 = a3 * num9;
												pt2 = vector4 + b2;
												if (rdrayTracer.MatchAt(pt2, id, true, this.terrainBounds))
												{
													pt2 = vector4 - b2;
													if (rdrayTracer.MatchAt(pt2, id, true, this.terrainBounds))
													{
														num7 = num9;
														break;
													}
												}
											}
										}
									}
								}
							}
							if (num7 > 0f)
							{
								float magnitude = (vector4 - bounds.center).magnitude;
								float num10 = num7 * global::Common.map(magnitude, 0f, vmax, 2f, 1f, false);
								if (num10 > num4)
								{
									num4 = num10;
									num3 = num7;
									ptCenter = vector4;
									angle = num5;
								}
							}
						}
					}
				}
				if (vector3.z != 0f)
				{
					for (float num11 = bounds.min.x + num; num11 <= bounds.max.x - num; num11 += num)
					{
						Vector3 pt3 = new Vector3(num11, 0f, (vector3.z < 0f) ? bounds.max.z : bounds.min.z);
						LabelUpdater.RDRayTracer rdrayTracer2 = new LabelUpdater.RDRayTracer(bounds, pt3, vector3, id, bKingdom, this.terrainBounds);
						Vector3 vector5 = rdrayTracer2.ptEnter + (rdrayTracer2.ptLeave - rdrayTracer2.ptEnter) / 2f;
						if (rdrayTracer2.len > num3)
						{
							float num12 = 0f;
							for (float num13 = 0.9f; num13 >= 0.5f; num13 -= 0.1f)
							{
								float num14 = rdrayTracer2.len * num13;
								if (num14 <= num3)
								{
									break;
								}
								Vector3 b3 = a * num14;
								Vector3 pt4 = vector5 + b3;
								if (rdrayTracer2.MatchAt(pt4, id, false, this.terrainBounds))
								{
									pt4 = vector5 - b3;
									if (rdrayTracer2.MatchAt(pt4, id, false, this.terrainBounds))
									{
										b3 = a2 * num14;
										pt4 = vector5 + b3;
										if (rdrayTracer2.MatchAt(pt4, id, false, this.terrainBounds))
										{
											pt4 = vector5 - b3;
											if (rdrayTracer2.MatchAt(pt4, id, false, this.terrainBounds))
											{
												Vector3 b4 = a3 * num14;
												pt4 = vector5 + b4;
												if (rdrayTracer2.MatchAt(pt4, id, true, this.terrainBounds))
												{
													pt4 = vector5 - b4;
													if (rdrayTracer2.MatchAt(pt4, id, true, this.terrainBounds))
													{
														num12 = num14;
														break;
													}
												}
											}
										}
									}
								}
							}
							if (num12 > 0f)
							{
								float magnitude2 = (vector5 - bounds.center).magnitude;
								float num15 = num12 * global::Common.map(magnitude2, 0f, vmax, 2f, 1f, false);
								if (num15 > num4)
								{
									num4 = num15;
									num3 = num12;
									ptCenter = vector5;
									angle = num5;
								}
							}
						}
					}
				}
			}
			if (num3 <= 0f)
			{
				@lock = this.Lock;
				lock (@lock)
				{
					this.processing.Remove(info);
				}
				return false;
			}
			LabelInfo labelInfo = new LabelInfo();
			labelInfo.ptCenter = ptCenter;
			labelInfo.ptCenter.y = 0.1f;
			labelInfo.width = num3 * 0.8f;
			labelInfo.height = labelInfo.width * vector.z * 2f;
			labelInfo.angle = angle;
			LabelUpdater.CompletedInfo item = new LabelUpdater.CompletedInfo(labelInfo, info, i + 1 == list2.Count);
			list.Add(item);
		}
		@lock = this.Lock;
		lock (@lock)
		{
			this.completed.AddRange(list);
			this.processing.Remove(info);
		}
		return true;
	}

	// Token: 0x06000D18 RID: 3352 RVA: 0x00094AF0 File Offset: 0x00092CF0
	public GameObject CreateLabelObject(GameObject prefab, LabelInfo li, string name)
	{
		GameObject gameObject = global::Common.Spawn(prefab, true, false);
		gameObject.name = name;
		gameObject.transform.position = li.ptCenter;
		TextMeshPro component = gameObject.GetComponent<TextMeshPro>();
		if (component != null)
		{
			UIText.SetText(component, name.Replace('_', ' '));
			component.rectTransform.sizeDelta = new Vector2(li.width, li.height);
			component.rectTransform.localRotation = Quaternion.Euler(90f, li.angle, 0f);
		}
		return gameObject;
	}

	// Token: 0x06000D19 RID: 3353 RVA: 0x00094B7C File Offset: 0x00092D7C
	private void SetupContainers()
	{
		MapData politicalOnly = MapData.GetPoliticalOnly();
		Transform transform = ((politicalOnly != null) ? politicalOnly.gameObject.transform : null) ?? base.transform;
		this.labelContainer = transform.Find("_labels");
		if (this.labelContainer == null)
		{
			GameObject gameObject = new GameObject("_labels");
			gameObject.transform.SetParent(transform);
			gameObject.layer = this.layer;
			this.labelContainer = gameObject.transform;
		}
		this.realmLabelContainer = this.labelContainer.Find("_realms");
		if (this.realmLabelContainer == null)
		{
			GameObject gameObject2 = new GameObject("_realms");
			gameObject2.transform.SetParent(this.labelContainer.transform);
			gameObject2.layer = this.layer;
			this.realmLabelContainer = gameObject2.transform;
		}
		this.kingdomLabelContainer = this.labelContainer.Find("_kingdoms");
		if (this.kingdomLabelContainer == null)
		{
			GameObject gameObject3 = new GameObject("_kingdoms");
			gameObject3.transform.SetParent(this.labelContainer.transform);
			gameObject3.layer = this.layer;
			this.kingdomLabelContainer = gameObject3.transform;
		}
	}

	// Token: 0x06000D1A RID: 3354 RVA: 0x00094CB4 File Offset: 0x00092EB4
	public void ClearKingdomOldLabels(global::Kingdom k, bool instant = false)
	{
		if (k == null)
		{
			return;
		}
		if (this.kingdomLabelContainer == null)
		{
			return;
		}
		List<GameObject> list = new List<GameObject>();
		global::Common.FindChildrenByName(list, this.kingdomLabelContainer.gameObject, global::Defs.Localize(k.GetNameKey(), null, null, true, true), true, true);
		for (int i = 0; i < list.Count; i++)
		{
			GameObject gameObject = list[i];
			if (instant)
			{
				global::Common.DestroyObj(gameObject);
			}
			else
			{
				gameObject.GetComponent<LabelFadeAndDestroy>().FadeOutAndDestroy(true, 1f);
			}
		}
	}

	// Token: 0x06000D1B RID: 3355 RVA: 0x00094D34 File Offset: 0x00092F34
	private void RenameKingdomNewLabels(global::Kingdom k, bool instant = false)
	{
		if (k == null)
		{
			return;
		}
		if (this.kingdomLabelContainer == null)
		{
			return;
		}
		string text = global::Defs.Localize(k.GetNameKey(), null, null, true, true);
		List<GameObject> list = new List<GameObject>();
		global::Common.FindChildrenByName(list, this.kingdomLabelContainer.gameObject, text + "_new", true, true);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].name = text;
		}
	}

	// Token: 0x06000D1C RID: 3356 RVA: 0x00094DA8 File Offset: 0x00092FA8
	private void ClearLabels()
	{
		this.AbortThreads();
		this.enqueued.Clear();
		this.processing.Clear();
		this.completed.Clear();
		if (this.kingdomLabelContainer != null)
		{
			for (int i = this.kingdomLabelContainer.childCount - 1; i >= 0; i--)
			{
				global::Common.DestroyObj(this.kingdomLabelContainer.GetChild(i).gameObject);
			}
		}
		if (this.realmLabelContainer != null)
		{
			for (int j = this.realmLabelContainer.childCount - 1; j >= 0; j--)
			{
				global::Common.DestroyObj(this.realmLabelContainer.GetChild(j).gameObject);
			}
		}
		this.SetThreads();
	}

	// Token: 0x06000D1D RID: 3357 RVA: 0x00094E5C File Offset: 0x0009305C
	public void GenerateLabels(bool blocking = false)
	{
		using (Game.Profile("LabelUpdater.GenerateLabels", false, 0f, null))
		{
			this.SetupContainers();
			this.ClearLabels();
			int num = 0;
			if (global::Common.EditorProgress("Generating Labels", "Enqueing labels", 0f, true))
			{
				List<global::Realm> realms = MapData.GetRealms();
				List<global::Kingdom> kingdoms = MapData.GetKingdoms();
				if (!(this.RealmLabelPrefab == null) || !(this.KingdomLabelPrefab == null))
				{
					if (this.RealmLabelPrefab != null && realms != null)
					{
						for (int i = 1; i <= realms.Count; i++)
						{
							global::Realm r = realms[i - 1];
							Action<LabelUpdater.CompletedInfo> createIconAction = delegate(LabelUpdater.CompletedInfo ci)
							{
								if (this.realmLabelContainer != null)
								{
									this.CreateRealmLabel(ci, r, this.realmLabelContainer, this.layer);
									return;
								}
								Debug.LogWarning("Label Generation: missing goRealmLabels object - " + r.Name + " (Ignore if it happened during game exit)");
							};
							LabelUpdater.Enque(r, createIconAction);
							num++;
						}
					}
					if (this.KingdomLabelPrefab != null)
					{
						for (int j = 1; j <= kingdoms.Count; j++)
						{
							global::Kingdom k = kingdoms[j - 1];
							Action<LabelUpdater.CompletedInfo> createIconAction2 = delegate(LabelUpdater.CompletedInfo ci)
							{
								if (this.kingdomLabelContainer != null)
								{
									this.CreateKingdomLabel(ci, k, this.kingdomLabelContainer, this.layer);
									return;
								}
								Debug.LogWarning("Label Generation: missing goKingdomLabels object - " + k.Name + " (Ignore if it happened during game exit)");
							};
							LabelUpdater.Enque(k, createIconAction2);
							num++;
						}
					}
					if (!Application.isPlaying || blocking)
					{
						LabelUpdater.Get(true).ProcessEnqueued();
						for (;;)
						{
							int num2 = LabelUpdater.NumPending();
							if (!global::Common.EditorProgress("Generating Labels", string.Concat(new object[]
							{
								"Waiting for label generation ",
								num - num2,
								"/",
								num
							}), ((float)num - (float)num2) / (float)num, true))
							{
								break;
							}
							if (!LabelUpdater.IsProcessing())
							{
								goto Block_11;
							}
						}
						return;
						Block_11:
						if (!global::Common.EditorProgress("Generating Labels", "Updating labels", 1f, true))
						{
							return;
						}
						LabelUpdater.Get(true).Update();
					}
					this.UpdateLabels();
					global::Common.EditorProgress(null, null, 0f, false);
				}
			}
		}
	}

	// Token: 0x06000D1E RID: 3358 RVA: 0x00095064 File Offset: 0x00093264
	public GameObject GetSimilarLabel(global::Kingdom k, LabelInfo li)
	{
		if (k == null || k.logic == null || k.logic.IsDefeated())
		{
			return null;
		}
		this.labels.Clear();
		string name = global::Defs.Localize(k.GetNameKey(), null, null, true, true);
		global::Common.FindChildrenByName(this.labels, this.kingdomLabelContainer.gameObject, name, true, true);
		for (int i = 0; i < this.labels.Count; i++)
		{
			if (this.IsLabelSimilar(this.labels[i], li))
			{
				return this.labels[i];
			}
		}
		return null;
	}

	// Token: 0x06000D1F RID: 3359 RVA: 0x000950F8 File Offset: 0x000932F8
	public bool IsLabelSimilar(GameObject label, LabelInfo li)
	{
		if (label == null || li == null)
		{
			return false;
		}
		if (Vector3.Distance(label.transform.position, li.ptCenter) < 5f)
		{
			TextMeshPro component = label.GetComponent<TextMeshPro>();
			if (component != null && Vector2.Distance(component.rectTransform.sizeDelta, new Vector2(li.width, li.height)) < 50f && Vector3.Distance(Quaternion.Euler(90f, li.angle, 0f).eulerAngles, component.rectTransform.localRotation.eulerAngles) < 5f)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000D20 RID: 3360 RVA: 0x000951A8 File Offset: 0x000933A8
	public GameObject CreateRealmLabel(LabelUpdater.CompletedInfo ci, global::Realm r, Transform parent, int layer)
	{
		LabelInfo info = ci.info;
		if (info == null || r == null)
		{
			return null;
		}
		GameObject gameObject = this.CreateLabelObject(this.RealmLabelPrefab, info, global::Defs.Localize(r.GetNameKey(), null, null, true, true));
		if (gameObject != null)
		{
			gameObject.transform.SetParent(parent);
			gameObject.layer = layer;
			gameObject.GetComponent<LabelFadeAndDestroy>().FadeIn(false, 0f);
		}
		return gameObject;
	}

	// Token: 0x06000D21 RID: 3361 RVA: 0x00095210 File Offset: 0x00093410
	public void CreateKingdomLabel(LabelUpdater.CompletedInfo ci, global::Kingdom k, Transform parent, int layer)
	{
		LabelInfo info = ci.info;
		if (info == null || k == null)
		{
			return;
		}
		Logic.Kingdom logic = k.logic;
		bool flag = logic != null && logic.IsDefeated();
		if (!flag)
		{
			GameObject similarLabel = this.GetSimilarLabel(k, info);
			if (similarLabel == null)
			{
				GameObject gameObject = this.CreateLabelObject(this.KingdomLabelPrefab, info, global::Defs.Localize(k.GetNameKey(), null, null, true, true));
				if (gameObject != null)
				{
					gameObject.transform.SetParent(parent);
					gameObject.layer = layer;
					gameObject.name += "_new";
					gameObject.GetComponent<LabelFadeAndDestroy>().FadeIn(false, 0f);
				}
			}
			else
			{
				similarLabel.name += "_new";
				similarLabel.GetComponent<LabelFadeAndDestroy>().FadeIn(true, 0f);
			}
		}
		if (ci.isLastLabel)
		{
			if (flag)
			{
				this.RenameKingdomNewLabels(k, false);
				this.ClearKingdomOldLabels(k, false);
				return;
			}
			this.ClearKingdomOldLabels(k, false);
			this.RenameKingdomNewLabels(k, false);
		}
	}

	// Token: 0x06000D22 RID: 3362 RVA: 0x0009530C File Offset: 0x0009350C
	public void RecreateKingdomLabel(global::Kingdom k)
	{
		if (k != null && k.logic != null)
		{
			if (k.logic.IsDefeated())
			{
				this.ClearKingdomOldLabels(k, false);
				return;
			}
			Action<LabelUpdater.CompletedInfo> createIconAction = delegate(LabelUpdater.CompletedInfo ci)
			{
				this.CreateKingdomLabel(ci, k, this.kingdomLabelContainer, this.layer);
			};
			LabelUpdater.Enque(k, createIconAction);
		}
	}

	// Token: 0x06000D23 RID: 3363 RVA: 0x0009537C File Offset: 0x0009357C
	public void UpdateLabels()
	{
		using (Game.Profile("LabelUpdater.UpdateLabels", false, 0f, null))
		{
			if (this.labelContainer != null)
			{
				this.labelContainer.gameObject.SetActive(ViewMode.IsPoliticalView());
				ViewMode current = ViewMode.current;
				if (ViewMode.IsPoliticalView())
				{
					bool flag = false;
					if (current != null)
					{
						flag = current.ShowRealmLabels();
					}
					if (this.realmLabelContainer != null)
					{
						this.realmLabelContainer.gameObject.SetActive(flag);
					}
					if (this.kingdomLabelContainer != null)
					{
						this.kingdomLabelContainer.gameObject.SetActive(!flag);
					}
				}
			}
			this.GenerateGoldIncomeLabels();
			this.UpdateIncomeLabels();
		}
	}

	// Token: 0x06000D24 RID: 3364 RVA: 0x00095444 File Offset: 0x00093644
	private void GenerateGoldIncomeLabels()
	{
		if (!ViewMode.GoldIncome.IsActive())
		{
			return;
		}
		if (this.GoldIncomePrefab == null)
		{
			return;
		}
		MapData politicalOnly = MapData.GetPoliticalOnly();
		GameObject gameObject = global::Common.FindChildByName(((politicalOnly != null) ? politicalOnly.gameObject : null) ?? base.gameObject, "_goldIncomes", true, true);
		if (gameObject == null)
		{
			return;
		}
		global::Kingdom selected_kingdom = WorldUI.Get().selected_kingdom;
		if (selected_kingdom == null)
		{
			return;
		}
		for (int i = 0; i < selected_kingdom.realms.Count; i++)
		{
			global::Realm realm = selected_kingdom.realms[i];
			if (realm != null && realm.logic != null)
			{
				global::Kingdom kingdom = realm.GetKingdom();
				if (kingdom != null && kingdom.logic != null && realm.logic.castle != null && realm.logic.castle.visuals != null)
				{
					global::Settlement settlement = realm.logic.castle.visuals as global::Settlement;
					if (!(settlement == null))
					{
						if (settlement.goldIncomeLabel == null)
						{
							settlement.goldIncomeLabel = global::Common.Spawn(this.GoldIncomePrefab, false, false);
							settlement.goldIncomeLabel.transform.SetParent(gameObject.transform);
						}
						TextMeshPro component = settlement.goldIncomeLabel.GetComponent<TextMeshPro>();
						if (component == null)
						{
							return;
						}
						float num = Mathf.Round(settlement.logic.GetRealm().income[ResourceType.Gold] * 10f) / 10f;
						if (!ViewMode.Resources.IsActive())
						{
							component.SetText(num.ToString());
						}
						settlement.goldIncomeLabel.transform.position = new Vector3(settlement.transform.position.x, settlement.transform.position.y + 7f, settlement.transform.position.z);
					}
				}
			}
		}
		this.UpdateGoldIncomeLabelsText();
	}

	// Token: 0x06000D25 RID: 3365 RVA: 0x00095638 File Offset: 0x00093838
	public void UpdateGoldIncomeLabelsText()
	{
		if (ViewMode.Resources.IsActive())
		{
			return;
		}
		if (!ViewMode.GoldIncome.IsActive())
		{
			return;
		}
		global::Kingdom selected_kingdom = WorldUI.Get().selected_kingdom;
		if (selected_kingdom == null)
		{
			return;
		}
		for (int i = 0; i < selected_kingdom.realms.Count; i++)
		{
			if (selected_kingdom.realms[i] != null && selected_kingdom.realms[i].logic != null && selected_kingdom.realms[i].logic.castle != null && selected_kingdom.realms[i].logic.castle.visuals != null)
			{
				global::Settlement settlement = selected_kingdom.realms[i].logic.castle.visuals as global::Settlement;
				if (settlement.goldIncomeLabel == null)
				{
					return;
				}
				TextMeshPro component = settlement.goldIncomeLabel.GetComponent<TextMeshPro>();
				if (component == null)
				{
					return;
				}
				component.SetText((settlement.logic.GetRealm().income[ResourceType.Gold] * 10f / 10f).ToString());
			}
		}
	}

	// Token: 0x06000D26 RID: 3366 RVA: 0x00095760 File Offset: 0x00093960
	public void UpdateIncomeLabels()
	{
		if (ViewMode.Resources.IsActive())
		{
			return;
		}
		MapData politicalOnly = MapData.GetPoliticalOnly();
		Transform transform = ((politicalOnly != null) ? politicalOnly.gameObject.transform : null) ?? base.transform;
		GameObject gameObject = global::Common.FindChildByName(transform.gameObject, "_goldIncomes", true, true);
		if (gameObject == null)
		{
			gameObject = new GameObject("_goldIncomes");
			gameObject.transform.SetParent(transform);
		}
		if (gameObject == null)
		{
			return;
		}
		bool flag = ViewMode.GoldIncome.IsActive();
		gameObject.SetActive(flag);
		if (flag)
		{
			global::Kingdom kingdom = global::Kingdom.Get(WorldMap.Get().SrcKingdom);
			if (kingdom == null)
			{
				return;
			}
			List<global::Realm> realms = MapData.GetRealms();
			if (realms != null)
			{
				for (int i = 0; i < realms.Count; i++)
				{
					global::Realm realm = realms[i];
					if (realm != null && realm.logic != null && realm.logic.castle != null)
					{
						global::Settlement settlement = realm.logic.castle.visuals as global::Settlement;
						if (!(settlement.goldIncomeLabel == null))
						{
							settlement.goldIncomeLabel.SetActive(kingdom == realm.GetKingdom());
						}
					}
				}
			}
		}
	}

	// Token: 0x06000D27 RID: 3367 RVA: 0x00095890 File Offset: 0x00093A90
	public void UpdateResourceLabels(List<int> resources, List<int> unResources)
	{
		MapData politicalOnly = MapData.GetPoliticalOnly();
		GameObject gameObject = global::Common.FindChildByName(((politicalOnly != null) ? politicalOnly.gameObject : null) ?? base.gameObject, "_goldIncomes", true, true);
		if (gameObject == null)
		{
			return;
		}
		bool flag = ViewMode.Resources.IsActive();
		gameObject.SetActive(flag);
		if (flag)
		{
			List<global::Realm> realms = MapData.GetRealms();
			if (realms != null)
			{
				for (int i = 0; i < realms.Count; i++)
				{
					global::Realm realm = realms[i];
					if (realm != null && realm.logic != null && realm.logic.castle != null && resources.Count > i)
					{
						global::Settlement settlement = realm.logic.castle.visuals as global::Settlement;
						if (!(settlement.goldIncomeLabel == null))
						{
							settlement.goldIncomeLabel.GetComponent<TextMeshPro>().SetText(resources[i].ToString() + "-" + unResources[i]);
							settlement.goldIncomeLabel.SetActive(true);
						}
					}
				}
			}
		}
	}

	// Token: 0x04000A10 RID: 2576
	private static LabelUpdater instance;

	// Token: 0x04000A11 RID: 2577
	private int numActiveThreads;

	// Token: 0x04000A12 RID: 2578
	private GameObject RealmLabelPrefab;

	// Token: 0x04000A13 RID: 2579
	private GameObject KingdomLabelPrefab;

	// Token: 0x04000A14 RID: 2580
	private GameObject GoldIncomePrefab;

	// Token: 0x04000A15 RID: 2581
	private Bounds terrainBounds;

	// Token: 0x04000A16 RID: 2582
	private Thread thread;

	// Token: 0x04000A17 RID: 2583
	private List<Thread> extraThreads = new List<Thread>(0);

	// Token: 0x04000A18 RID: 2584
	private object Lock = new object();

	// Token: 0x04000A19 RID: 2585
	private AutoResetEvent resume = new AutoResetEvent(false);

	// Token: 0x04000A1A RID: 2586
	public List<LabelUpdater.PendingInfo> enqueued = new List<LabelUpdater.PendingInfo>();

	// Token: 0x04000A1B RID: 2587
	public List<LabelUpdater.PendingInfo> pending = new List<LabelUpdater.PendingInfo>();

	// Token: 0x04000A1C RID: 2588
	public List<LabelUpdater.PendingInfo> processing = new List<LabelUpdater.PendingInfo>();

	// Token: 0x04000A1D RID: 2589
	public List<LabelUpdater.CompletedInfo> completed = new List<LabelUpdater.CompletedInfo>();

	// Token: 0x04000A1E RID: 2590
	private int layer;

	// Token: 0x04000A1F RID: 2591
	public Transform labelContainer;

	// Token: 0x04000A20 RID: 2592
	public Transform kingdomLabelContainer;

	// Token: 0x04000A21 RID: 2593
	public Transform realmLabelContainer;

	// Token: 0x04000A22 RID: 2594
	private List<GameObject> labels = new List<GameObject>();

	// Token: 0x02000628 RID: 1576
	public struct PendingInfo
	{
		// Token: 0x06004709 RID: 18185 RVA: 0x0021239C File Offset: 0x0021059C
		public PendingInfo(global::Realm r, global::Kingdom k, Action<LabelUpdater.CompletedInfo> createLabelAction)
		{
			this.r = r;
			this.r = r;
			this.k = k;
			this.createLabelAction = createLabelAction;
		}

		// Token: 0x04003459 RID: 13401
		public global::Realm r;

		// Token: 0x0400345A RID: 13402
		public global::Kingdom k;

		// Token: 0x0400345B RID: 13403
		public Action<LabelUpdater.CompletedInfo> createLabelAction;
	}

	// Token: 0x02000629 RID: 1577
	public struct CompletedInfo
	{
		// Token: 0x0600470A RID: 18186 RVA: 0x002123BA File Offset: 0x002105BA
		public CompletedInfo(LabelInfo info, LabelUpdater.PendingInfo pendingInfo, bool isLastLabel)
		{
			this.info = info;
			this.pendingInfo = pendingInfo;
			this.isLastLabel = isLastLabel;
		}

		// Token: 0x0400345C RID: 13404
		public LabelInfo info;

		// Token: 0x0400345D RID: 13405
		public LabelUpdater.PendingInfo pendingInfo;

		// Token: 0x0400345E RID: 13406
		public bool isLastLabel;
	}

	// Token: 0x0200062A RID: 1578
	private class RDRayTracer
	{
		// Token: 0x0600470B RID: 18187 RVA: 0x002123D4 File Offset: 0x002105D4
		public RDRayTracer(Bounds bounds, Vector3 pt, Vector3 step, int id, bool bKingdom, Bounds terrainBounds)
		{
			this.world_map = WorldMap.Get();
			this.ptMin = bounds.min;
			this.ptMax = bounds.max;
			this.pt = pt;
			this.step = step;
			this.id = id;
			this.bKingdom = bKingdom;
			this.inside = false;
			this.Trace(terrainBounds);
		}

		// Token: 0x0600470C RID: 18188 RVA: 0x00212450 File Offset: 0x00210650
		public int GetAt(Vector3 pt, Bounds terrainBounds)
		{
			if (this.world_map == null)
			{
				int num = TitleMap.Get().RealmIDAt(pt.x, pt.z, terrainBounds);
				if (num < 1 || num > TitleMap.Get().Realms.Count)
				{
					return 0;
				}
				if (!this.bKingdom)
				{
					return num;
				}
				return TitleMap.Get().Realms[num - 1].kingdom.id;
			}
			else
			{
				int num2 = this.world_map.RealmIDAt(pt.x, pt.z, terrainBounds);
				if (num2 < 1 || num2 > this.world_map.Realms.Count)
				{
					return 0;
				}
				if (!this.bKingdom)
				{
					return num2;
				}
				return this.world_map.Realms[num2 - 1].kingdom.id;
			}
		}

		// Token: 0x0600470D RID: 18189 RVA: 0x0021251C File Offset: 0x0021071C
		public bool MatchAt(Vector3 pt, int id, bool allow_sea, Bounds terrainBounds)
		{
			if (pt.x < terrainBounds.min.x || pt.z < terrainBounds.min.z)
			{
				return false;
			}
			if (pt.x > terrainBounds.max.x || pt.z > terrainBounds.max.z)
			{
				return false;
			}
			int at = this.GetAt(pt, terrainBounds);
			return at == id || (allow_sea && at == 0);
		}

		// Token: 0x0600470E RID: 18190 RVA: 0x00212598 File Offset: 0x00210798
		private void Trace(Bounds terrainBounds)
		{
			while (this.pt.x >= this.ptMin.x && this.pt.z >= this.ptMin.z && this.pt.x <= this.ptMax.x && this.pt.z <= this.ptMax.z)
			{
				int at = this.GetAt(this.pt, terrainBounds);
				bool flag = at == this.id;
				if (flag || (this.inside && at == 0))
				{
					if (!this.inside)
					{
						this.Enter();
					}
				}
				else if (this.inside)
				{
					this.Leave();
				}
				if (flag)
				{
					this.ptLast = this.pt;
				}
				this.pt += this.step;
			}
			if (this.inside)
			{
				this.Leave();
				return;
			}
		}

		// Token: 0x0600470F RID: 18191 RVA: 0x00212681 File Offset: 0x00210881
		private void Enter()
		{
			this.inside = true;
			this.pte = this.pt;
		}

		// Token: 0x06004710 RID: 18192 RVA: 0x00212698 File Offset: 0x00210898
		private void Leave()
		{
			this.inside = false;
			Vector3 a = this.ptLast;
			float magnitude = (a - this.pte).magnitude;
			if (magnitude <= this.len)
			{
				return;
			}
			this.ptEnter = this.pte;
			this.ptLeave = a;
			this.len = magnitude;
		}

		// Token: 0x0400345F RID: 13407
		public Vector3 ptEnter = Vector3.zero;

		// Token: 0x04003460 RID: 13408
		public Vector3 ptLeave = Vector3.zero;

		// Token: 0x04003461 RID: 13409
		public float len;

		// Token: 0x04003462 RID: 13410
		private WorldMap world_map;

		// Token: 0x04003463 RID: 13411
		private Vector3 ptMin;

		// Token: 0x04003464 RID: 13412
		private Vector3 ptMax;

		// Token: 0x04003465 RID: 13413
		private Vector3 pt;

		// Token: 0x04003466 RID: 13414
		private Vector3 step;

		// Token: 0x04003467 RID: 13415
		private int id;

		// Token: 0x04003468 RID: 13416
		private bool bKingdom;

		// Token: 0x04003469 RID: 13417
		private Vector3 pte;

		// Token: 0x0400346A RID: 13418
		private Vector3 ptLast;

		// Token: 0x0400346B RID: 13419
		private bool inside;
	}
}
