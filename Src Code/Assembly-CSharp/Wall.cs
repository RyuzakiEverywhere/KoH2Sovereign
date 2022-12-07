using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x02000318 RID: 792
public class Wall : MonoBehaviour
{
	// Token: 0x06003189 RID: 12681 RVA: 0x0018FF88 File Offset: 0x0018E188
	public void LoadArchitectureSet()
	{
		this.towers_PGI = PrefabGrid.Info.Get("Towers", this.towers_architecture, false);
		this.segments_PGI = PrefabGrid.Info.Get("Walls", this.segments_architecture, false);
		this.begin_segments_PGI = PrefabGrid.Info.Get("WallsStart", this.segments_architecture, false);
		this.end_segments_PGI = PrefabGrid.Info.Get("WallsEnd", this.segments_architecture, false);
	}

	// Token: 0x0600318A RID: 12682 RVA: 0x0018FFF1 File Offset: 0x0018E1F1
	public int GetMaxLevel()
	{
		if (this.segments_PGI == null)
		{
			this.LoadArchitectureSet();
			if (this.segments_PGI == null)
			{
				return 0;
			}
		}
		return this.segments_PGI.max_level;
	}

	// Token: 0x0600318B RID: 12683 RVA: 0x00190016 File Offset: 0x0018E216
	public int GetMaxVariant()
	{
		if (this.segments_PGI == null)
		{
			this.LoadArchitectureSet();
			if (this.segments_PGI == null)
			{
				return 0;
			}
		}
		return this.segments_PGI.max_variant;
	}

	// Token: 0x0600318C RID: 12684 RVA: 0x0019003B File Offset: 0x0018E23B
	public int GetMaxTowersVariant()
	{
		if (this.towers_PGI == null)
		{
			this.LoadArchitectureSet();
			if (this.towers_PGI == null)
			{
				return 0;
			}
		}
		return this.towers_PGI.max_variant;
	}

	// Token: 0x0600318D RID: 12685 RVA: 0x00190060 File Offset: 0x0018E260
	public List<WallCorner> GetCorners()
	{
		if (this.corners == null || this.corners.Count <= 0)
		{
			this.FindCorners();
			this.SortCorners();
			this.CalcCurves();
		}
		return this.corners;
	}

	// Token: 0x0600318E RID: 12686 RVA: 0x00190090 File Offset: 0x0018E290
	public void FindCorners()
	{
		this.aabb = new Bounds(base.transform.position, Vector3.zero);
		this.corners.Clear();
		foreach (object obj in base.transform)
		{
			WallCorner component = ((Transform)obj).GetComponent<WallCorner>();
			if (!(component == null))
			{
				if (component.type == WallCorner.Type.Disabled)
				{
					component.gameObject.SetActive(false);
				}
				else
				{
					this.corners.Add(component);
					this.aabb.Encapsulate(component.transform.position);
				}
			}
		}
	}

	// Token: 0x0600318F RID: 12687 RVA: 0x00190150 File Offset: 0x0018E350
	public int CompareCorners(WallCorner a, WallCorner b)
	{
		Vector3 vector = a.transform.position - base.transform.position;
		Vector3 vector2 = b.transform.position - base.transform.position;
		float num = Mathf.Atan2(vector.z, vector.x);
		float num2 = Mathf.Atan2(vector2.z, vector2.x);
		if (num > num2)
		{
			return 1;
		}
		if (num >= num2)
		{
			return 0;
		}
		return -1;
	}

	// Token: 0x06003190 RID: 12688 RVA: 0x001901C6 File Offset: 0x0018E3C6
	private void SortCorners()
	{
		this.corners.Sort(new Comparison<WallCorner>(this.CompareCorners));
	}

	// Token: 0x06003191 RID: 12689 RVA: 0x001901E0 File Offset: 0x0018E3E0
	private void RotateCorners()
	{
		if (this.corners.Count <= 2)
		{
			return;
		}
		for (int i = 0; i < this.corners.Count; i++)
		{
			WallCorner wallCorner = this.corners[i];
			if (wallCorner.AutoRotate)
			{
				UnityEngine.Component component = this.corners[(i + 1 >= this.corners.Count) ? 0 : (i + 1)];
				WallCorner wallCorner2 = this.corners[(i == 0) ? (this.corners.Count - 1) : (i - 1)];
				Vector3 point = wallCorner.transform.position - wallCorner2.transform.position;
				point.y = 0f;
				point.Normalize();
				Vector3 point2 = component.transform.position - wallCorner.transform.position;
				point2.y = 0f;
				point2.Normalize();
				Vector3 a = Quaternion.AngleAxis(-wallCorner.InCurve, Vector3.up) * point;
				Vector3 b = Quaternion.AngleAxis(wallCorner.OutCurve, Vector3.up) * point2;
				wallCorner.transform.rotation = Quaternion.LookRotation(a + b);
				wallCorner.transform.Rotate(0f, 270f, 0f);
			}
		}
	}

	// Token: 0x06003192 RID: 12690 RVA: 0x00190338 File Offset: 0x0018E538
	private void CalcCurves()
	{
		if (this.corners.Count < 3)
		{
			foreach (WallCorner wallCorner in this.corners)
			{
				if (wallCorner.AutoCurve)
				{
					wallCorner.OutCurve = (wallCorner.InCurve = 0f);
				}
			}
			return;
		}
		for (int i = 0; i < this.corners.Count; i++)
		{
			WallCorner wallCorner2 = this.corners[i];
			WallCorner wallCorner3 = this.corners[(i == 0) ? (this.corners.Count - 1) : (i - 1)];
			if (wallCorner2.AutoCurve)
			{
				UnityEngine.Component component = this.corners[(i + 1 >= this.corners.Count) ? 0 : (i + 1)];
				Vector3 vector = wallCorner2.transform.position - wallCorner3.transform.position;
				vector.y = 0f;
				vector.Normalize();
				float num = Mathf.Atan2(vector.z, vector.x);
				Vector3 vector2 = component.transform.position - wallCorner2.transform.position;
				vector2.y = 0f;
				vector2.Normalize();
				float num2 = Mathf.Atan2(vector2.z, vector2.x);
				Vector3 vector3 = vector + vector2;
				vector3.Normalize();
				float num3 = Mathf.Atan2(vector3.z, vector3.x);
				wallCorner2.OutCurve = global::Common.NormalizeAngle180((num3 - num) * 57.29578f);
				wallCorner2.InCurve = global::Common.NormalizeAngle180((num2 - num3) * 57.29578f);
			}
			if (wallCorner3.Straight)
			{
				wallCorner2.InCurve = 0f;
			}
			if (wallCorner2.Straight)
			{
				wallCorner2.OutCurve = 0f;
			}
		}
	}

	// Token: 0x06003193 RID: 12691 RVA: 0x00190534 File Offset: 0x0018E734
	public void Load(DT.Field field)
	{
		if (field.value.obj_val is Point)
		{
			base.transform.localPosition = (Point)field.value.obj_val;
		}
		else
		{
			base.transform.localPosition = Vector3.zero;
		}
		float @float = field.GetFloat("rotation", null, 0f, true, true, true, '.');
		if (@float != 0f)
		{
			base.transform.localEulerAngles = new Vector3(0f, @float, 0f);
		}
		this.towers_architecture = field.GetString("towers_architecture", null, this.towers_architecture, true, true, true, '.');
		this.segments_architecture = field.GetString("segments_architecture", null, this.segments_architecture, true, true, true, '.');
		this.level = field.GetInt("level", null, 0, true, true, true, '.');
		this.towers_variant = field.GetInt("towers_variant", null, 1, true, true, true, '.');
		this.segments_variant = field.GetInt("segments_variant", null, 1, true, true, true, '.');
		this.HeightScale = field.GetFloat("height_scale", null, this.HeightScale, true, true, true, '.');
		this.ThicknessScale = field.GetFloat("thickness_scale", null, this.ThicknessScale, true, true, true, '.');
		this.MaxLengthScale = field.GetFloat("max_len_scale", null, this.MaxLengthScale, true, true, true, '.');
		this.HeightOfs = field.GetFloat("height_ofs", null, this.HeightOfs, true, true, true, '.');
		this.Curveness = field.GetFloat("curveness", null, this.Curveness, true, true, true, '.');
		DT.Field field2 = field.FindChild("corners", null, true, true, true, '.');
		if (field2 == null || field2.children == null || field2.children.Count == 0)
		{
			return;
		}
		for (int i = 0; i < field2.children.Count; i++)
		{
			DT.Field field3 = field2.children[i];
			if (!string.IsNullOrEmpty(field3.key) && !(field3.key == "prefab"))
			{
				WallCorner wallCorner = WallCorner.Load(field3, base.transform, this.towers_architecture, this.towers_variant, 0);
				if (wallCorner != null)
				{
					this.corners.Add(wallCorner);
				}
			}
		}
	}

	// Token: 0x06003194 RID: 12692 RVA: 0x00190771 File Offset: 0x0018E971
	public static Wall Load(DT.Field field, Transform parent)
	{
		if (field == null)
		{
			return null;
		}
		Wall wall = global::Common.SpawnTemplate<Wall>("Wall", null, parent, false, new Type[]
		{
			typeof(Wall)
		});
		wall.Load(field);
		wall.gameObject.SetActive(true);
		return wall;
	}

	// Token: 0x06003195 RID: 12693 RVA: 0x000023FD File Offset: 0x000005FD
	private void OnDrawGizmos()
	{
	}

	// Token: 0x06003196 RID: 12694 RVA: 0x001907AC File Offset: 0x0018E9AC
	public void UpdateCornerLevels(bool force_architecture = false)
	{
		foreach (WallCorner wallCorner in this.corners)
		{
			Transform transform = wallCorner.transform;
			PrefabGrid prefabGrid = null;
			if (wallCorner.type == WallCorner.Type.Hidden)
			{
				prefabGrid = wallCorner.GetComponent<PrefabGrid>();
				if (prefabGrid != null)
				{
					prefabGrid.Despawn(true);
					UnityEngine.Object.DestroyImmediate(prefabGrid);
				}
			}
			else
			{
				if (wallCorner.type == WallCorner.Type.Tower)
				{
					prefabGrid = PrefabGrid.ApplyToTransform("Towers", this.towers_architecture, true, force_architecture, transform, false);
				}
				else if (wallCorner.type == WallCorner.Type.Gate)
				{
					prefabGrid = PrefabGrid.ApplyToTransform("Gates", this.towers_architecture, true, force_architecture, transform, false);
				}
				if (!(prefabGrid == null))
				{
					prefabGrid.SetParent(this, false);
					if (prefabGrid.set_variant <= 0)
					{
						prefabGrid.cur_variant = this.towers_variant;
					}
					prefabGrid.set_level = 0;
					prefabGrid.cur_level = Mathf.Max(this.level, 1);
					if (this.level > 0)
					{
						prefabGrid.Refresh(force_architecture, true);
						prefabGrid.gameObject.SetActive(true);
					}
					else
					{
						prefabGrid.gameObject.SetActive(false);
						prefabGrid.Despawn(true);
					}
				}
			}
		}
	}

	// Token: 0x06003197 RID: 12695 RVA: 0x001908F0 File Offset: 0x0018EAF0
	private Vector3 GetSegmentSize(GameObject segment)
	{
		BoxCollider boxCollider = Wall.BoundsCollider(segment.transform);
		if (boxCollider == null)
		{
			boxCollider = segment.AddComponent<BoxCollider>();
		}
		if (boxCollider != null)
		{
			Vector3 vector = Vector3.Scale(boxCollider.size, boxCollider.transform.lossyScale);
			if (vector.x < 0f)
			{
				vector.x *= -1f;
			}
			if (vector.y < 0f)
			{
				vector.y *= -1f;
			}
			if (vector.z < 0f)
			{
				vector.z *= -1f;
			}
			float x = boxCollider.transform.eulerAngles.x;
			if (x == 90f || x == 270f)
			{
				vector = new Vector3(vector.x, vector.z, vector.y);
			}
			return vector;
		}
		return Vector3.one;
	}

	// Token: 0x06003198 RID: 12696 RVA: 0x001909D4 File Offset: 0x0018EBD4
	private GameObject ChooseSegmentPrefab(bool isFirstSegment = false, bool isLastSegment = false)
	{
		PrefabGrid.Info info = this.segments_PGI;
		if (isFirstSegment && this.begin_segments_PGI.IsValid())
		{
			info = this.begin_segments_PGI;
		}
		else if (isLastSegment && this.end_segments_PGI.IsValid())
		{
			info = this.end_segments_PGI;
		}
		if (info.max_variant < 1)
		{
			return null;
		}
		GameObject gameObject = info.GetPrefab(this.segments_variant, this.level);
		if (gameObject == null)
		{
			return null;
		}
		if (gameObject.transform.childCount == 1)
		{
			gameObject = gameObject.transform.GetChild(0).gameObject;
		}
		return gameObject;
	}

	// Token: 0x06003199 RID: 12697 RVA: 0x00190A64 File Offset: 0x0018EC64
	private GameObject CreateSegment(Transform parent, bool isFirstSegment = false, bool isLastSegment = false)
	{
		GameObject gameObject = this.ChooseSegmentPrefab(isFirstSegment, isLastSegment);
		if (gameObject == null)
		{
			return null;
		}
		GameObject gameObject2 = global::Common.Spawn(gameObject, false, false);
		gameObject2.transform.SetParent(parent, false);
		gameObject2.hideFlags |= HideFlags.NotEditable;
		return gameObject2;
	}

	// Token: 0x0600319A RID: 12698 RVA: 0x00190AA8 File Offset: 0x0018ECA8
	private void PlaceSegment(GameObject segment, Vector3 size, Vector3 pt1, Vector3 pt2)
	{
		Vector3 vector = pt2 - pt1;
		float magnitude = vector.magnitude;
		if (this.ThicknessScale < 0f)
		{
			segment.transform.rotation = Quaternion.Euler(new Vector3(0f, 180f + -Mathf.Atan2(vector.z, vector.x) * 57.29578f, 0f));
		}
		else
		{
			segment.transform.rotation = Quaternion.Euler(new Vector3(0f, -Mathf.Atan2(vector.z, vector.x) * 57.29578f, 0f));
		}
		segment.transform.position = (pt1 + pt2) / 2f;
		Vector3 localScale = segment.transform.localScale;
		localScale.Scale(new Vector3(magnitude / size.x, this.HeightScale, Mathf.Abs(this.ThicknessScale)));
		segment.transform.localScale = localScale;
	}

	// Token: 0x0600319B RID: 12699 RVA: 0x00190BA4 File Offset: 0x0018EDA4
	private List<GameObject> CreateSegments(WallCorner corner, WallCorner next, Transform parent)
	{
		List<GameObject> list = new List<GameObject>();
		if (corner.HideWall)
		{
			return list;
		}
		Vector3 vector;
		Vector3 vector2;
		corner.GetExtents(out vector, out vector2);
		Vector3 a;
		Vector3 vector3;
		next.GetExtents(out a, out vector3);
		Vector3 vector4 = vector2;
		Vector3 vector5 = a - vector4;
		vector5.y = 0f;
		float magnitude = vector5.magnitude;
		if (magnitude < 0.1f)
		{
			return list;
		}
		vector5.Normalize();
		Vector3 rightVector = global::Common.GetRightVector(vector5, 0f);
		rightVector.y = 0f;
		rightVector.Normalize();
		List<Vector3> list2 = new List<Vector3>();
		float num = 0f;
		GameObject gameObject = this.CreateSegment(parent, true, false);
		if (gameObject != null)
		{
			Vector3 vector6 = this.GetSegmentSize(gameObject);
			float num2 = vector6.x * this.MaxLengthScale;
			if (num2 < 0.01f)
			{
				num2 = 0.01f;
			}
			num += num2;
			list.Add(gameObject);
			list2.Add(vector6);
		}
		GameObject gameObject2 = null;
		if (num < magnitude)
		{
			gameObject2 = this.CreateSegment(parent, false, true);
			if (gameObject2 != null)
			{
				Vector3 vector6 = this.GetSegmentSize(gameObject2);
				float num2 = vector6.x * this.MaxLengthScale;
				if (num2 < 0.01f)
				{
					num2 = 0.01f;
				}
				num += num2;
				list.Add(gameObject2);
				list2.Add(vector6);
			}
		}
		while (num < magnitude)
		{
			gameObject = this.CreateSegment(parent, false, false);
			if (gameObject == null)
			{
				break;
			}
			Vector3 vector6 = this.GetSegmentSize(gameObject);
			float num2 = vector6.x * this.MaxLengthScale;
			if (num2 < 0.01f)
			{
				num2 = 0.01f;
			}
			num += num2;
			list.Insert(list.Count - 1, gameObject);
			list2.Insert(list2.Count - 1, vector6);
		}
		this.segments_towers.Add(corner.gameObject);
		this.segments_towers.AddRange(list);
		int childCount = parent.childCount;
		if (gameObject2 != null)
		{
			gameObject2.transform.SetSiblingIndex(childCount);
		}
		float num3 = this.MaxLengthScale * magnitude / num;
		Keyframe[] array = new Keyframe[2];
		array[0] = new Keyframe(0f, 0f);
		array[0].outTangent = corner.OutCurve * 0.017453292f;
		array[1] = new Keyframe(magnitude, 0f);
		array[1].inTangent = -next.InCurve * 0.017453292f;
		AnimationCurve animationCurve = new AnimationCurve(array);
		float num4 = 0f;
		Vector3 pt = vector4;
		for (int i = 0; i < list.Count; i++)
		{
			gameObject = list[i];
			Vector3 vector6 = list2[i];
			num4 += vector6.x * num3;
			Vector3 vector7 = vector4 + vector5 * num4;
			float d = animationCurve.Evaluate(num4) * this.Curveness;
			vector7 += rightVector * d;
			this.PlaceSegment(gameObject, vector6, pt, vector7);
			pt = vector7;
		}
		return list;
	}

	// Token: 0x0600319C RID: 12700 RVA: 0x00190EA8 File Offset: 0x0018F0A8
	private void ConnectFortifications()
	{
		if (!this.CheckFortificationLinks)
		{
			return;
		}
		for (int i = 0; i < this.segments_towers.Count; i++)
		{
			GameObject gameObject = this.segments_towers[i];
			global::Fortification fortification = gameObject.GetComponent<global::Fortification>();
			if (fortification == null)
			{
				fortification = gameObject.GetComponentInChildren<global::Fortification>();
			}
			if (fortification != null)
			{
				global::Fortification fortification2 = null;
				global::Fortification fortification3 = null;
				if (fortification.fortification_def_id == "Gate")
				{
					List<global::Fortification> list = new List<global::Fortification>(fortification.GetComponentsInChildren<global::Fortification>());
					list.Remove(fortification);
					if (list.Count == 2)
					{
						if (list[0].transform.localPosition.x < list[1].transform.localPosition.x)
						{
							fortification.prev = list[0];
							fortification.next = list[1];
						}
						else
						{
							fortification.prev = list[1];
							fortification.next = list[0];
						}
						fortification.prev.next = fortification;
						fortification.next.prev = fortification;
						if (i == 0)
						{
							fortification2 = this.segments_towers[this.segments_towers.Count - 1].GetComponentInChildren<global::Fortification>();
							fortification3 = this.segments_towers[1].GetComponentInChildren<global::Fortification>();
						}
						else if (i == this.segments_towers.Count - 1)
						{
							fortification2 = this.segments_towers[i - 1].GetComponentInChildren<global::Fortification>();
							fortification3 = this.segments_towers[0].GetComponentInChildren<global::Fortification>();
						}
						else if (this.segments_towers.Count > 2)
						{
							fortification2 = this.segments_towers[i - 1].GetComponentInChildren<global::Fortification>();
							fortification3 = this.segments_towers[i + 1].GetComponentInChildren<global::Fortification>();
						}
						if (fortification2 != null)
						{
							fortification.prev.prev = fortification2;
							fortification2.next = fortification.prev;
						}
						if (fortification3 != null)
						{
							fortification.next.next = fortification3;
							fortification3.prev = fortification.next;
						}
					}
				}
				else
				{
					if (i == 0)
					{
						fortification2 = this.segments_towers[this.segments_towers.Count - 1].GetComponentInChildren<global::Fortification>();
						fortification3 = this.segments_towers[1].GetComponentInChildren<global::Fortification>();
					}
					else if (i == this.segments_towers.Count - 1)
					{
						fortification2 = this.segments_towers[i - 1].GetComponentInChildren<global::Fortification>();
						fortification3 = this.segments_towers[0].GetComponentInChildren<global::Fortification>();
					}
					else if (this.segments_towers.Count > 2)
					{
						fortification2 = this.segments_towers[i - 1].GetComponentInChildren<global::Fortification>();
						fortification3 = this.segments_towers[i + 1].GetComponentInChildren<global::Fortification>();
					}
					if (fortification2 != null && fortification2.fortification_def_id != "Gate")
					{
						fortification.prev = fortification2;
					}
					if (fortification3 != null && fortification3.fortification_def_id != "Gate")
					{
						fortification.next = fortification3;
					}
				}
			}
		}
	}

	// Token: 0x0600319D RID: 12701 RVA: 0x001911AC File Offset: 0x0018F3AC
	private void DestroySegments()
	{
		Transform transform = base.transform.Find("_segments");
		if (transform != null)
		{
			UnityEngine.Object.DestroyImmediate(transform.gameObject);
		}
	}

	// Token: 0x0600319E RID: 12702 RVA: 0x001911E0 File Offset: 0x0018F3E0
	private void CreateSegments()
	{
		this.DestroySegments();
		this.segments_towers.Clear();
		if (this.corners.Count < 2)
		{
			return;
		}
		GameObject gameObject = new GameObject("_segments");
		gameObject.transform.SetParent(base.transform, false);
		gameObject.hideFlags = HideFlags.HideAndDontSave;
		this.segments.Clear();
		for (int i = 0; i < this.corners.Count; i++)
		{
			WallCorner corner = this.corners[i];
			WallCorner next = this.corners[(i + 1) % this.corners.Count];
			List<GameObject> collection = this.CreateSegments(corner, next, gameObject.transform);
			this.segments.AddRange(collection);
		}
		this.ConnectFortifications();
	}

	// Token: 0x0600319F RID: 12703 RVA: 0x001912A0 File Offset: 0x0018F4A0
	public void SnapCorners()
	{
		foreach (WallCorner wallCorner in this.corners)
		{
			wallCorner.HeightOfs = 0f;
			wallCorner.RestoreHeight();
		}
	}

	// Token: 0x060031A0 RID: 12704 RVA: 0x001912FC File Offset: 0x0018F4FC
	public void Snap()
	{
		foreach (WallCorner wallCorner in this.corners)
		{
			wallCorner.RestoreHeight();
		}
		Transform transform = base.transform.Find("_segments");
		if (transform == null)
		{
			return;
		}
		foreach (object obj in transform)
		{
			Transform transform2 = (Transform)obj;
			global::Common.SnapToTerrain(transform2, this.HeightOfs, null, -1f);
			transform2.hasChanged = false;
		}
		this.SetupSegmentShaders();
	}

	// Token: 0x060031A1 RID: 12705 RVA: 0x001913C0 File Offset: 0x0018F5C0
	private void SetupSegmentShaders()
	{
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		int count = this.segments_towers.Count;
		Transform transform = null;
		if (count != 0)
		{
			transform = this.segments_towers[count - 1].transform;
		}
		for (int i = 0; i < count; i++)
		{
			Transform transform2 = this.segments_towers[i].transform;
			WallCorner component = transform2.GetComponent<WallCorner>();
			if (component != null && !component.CanBend)
			{
				if (component.type != WallCorner.Type.Hidden)
				{
					transform = transform2;
				}
			}
			else
			{
				BoxCollider boxCollider = Wall.BoundsCollider(transform2);
				if (!(boxCollider == null))
				{
					Vector3 vector = boxCollider.size * 0.5f;
					vector.z = 0f;
					Vector3 vector2 = boxCollider.transform.TransformPoint(boxCollider.center - vector);
					Vector3 vector3 = boxCollider.transform.TransformPoint(boxCollider.center + vector);
					float terrainHeight = global::Common.GetTerrainHeight(vector2, null, false);
					float terrainHeight2 = global::Common.GetTerrainHeight(vector3, null, false);
					Vector4 vector4 = new Vector4(vector2.x, terrainHeight, vector2.z, vector2.y);
					Vector4 vector5 = new Vector4(vector3.x, terrainHeight2, vector3.z, this.HeightOfs - (boxCollider.transform.position.y - vector2.y));
					materialPropertyBlock.SetVector("_v1", vector4);
					materialPropertyBlock.SetVector("_v2", vector5);
					float num = 0f;
					float num2 = 0f;
					float z = 0f;
					float z2 = 0f;
					float y = 0f;
					float y2 = 0f;
					float y3 = transform2.rotation.eulerAngles.y;
					if (transform != null)
					{
						BoxCollider boxCollider2 = Wall.BoundsCollider(transform);
						if (boxCollider2 != null)
						{
							Vector3 b = boxCollider2.size * 0.5f;
							b.z = 0f;
							Vector3 vector6 = boxCollider2.transform.TransformPoint(boxCollider2.center + b);
							Vector3 vector7 = vector2 - vector6;
							vector7.y = 0f;
							if (vector7.sqrMagnitude < 0.7f)
							{
								WallCorner component2 = transform.GetComponent<WallCorner>();
								if (component2 != null && !component2.CanBend)
								{
									num = 0.008726646f * (180f + (-transform.rotation.eulerAngles.y + y3) * 2f) % 360f;
								}
								else
								{
									num = 0.008726646f * (180f - transform.rotation.eulerAngles.y + y3) % 360f;
								}
								y = 1f / Mathf.Tan(num);
								num = 3.1415927f - num;
								Vector3 b2 = vector2;
								b2.y = this.HeightOfs - (boxCollider.transform.position.y - vector2.y);
								vector6.y = this.HeightOfs - (boxCollider2.transform.position.y - vector6.y);
								Vector3 lhs = vector6 - b2;
								Vector3 rhs = new Vector3(vector5.x, vector5.w, vector5.z) - b2;
								z = Mathf.Acos(Vector3.Dot(lhs, rhs) / (lhs.magnitude * rhs.magnitude));
							}
						}
					}
					Transform transform3;
					if (i != count - 1)
					{
						transform3 = this.segments_towers[i + 1].transform;
						WallCorner wallCorner = (transform3 != null) ? transform3.GetComponent<WallCorner>() : null;
						if (wallCorner != null && wallCorner.type == WallCorner.Type.Hidden)
						{
							transform3 = this.segments_towers[i + 2].transform;
						}
					}
					else
					{
						transform3 = this.segments_towers[0].transform;
					}
					if (transform3 != null)
					{
						BoxCollider boxCollider3 = Wall.BoundsCollider(transform3);
						if (boxCollider3 != null)
						{
							Vector3 b3 = boxCollider3.size * 0.5f;
							b3.z = 0f;
							Vector3 vector8 = boxCollider3.transform.TransformPoint(boxCollider3.center - b3);
							Vector3 vector9 = vector8 - vector3;
							vector9.y = 0f;
							if (vector9.sqrMagnitude < 0.7f)
							{
								WallCorner component3 = transform3.GetComponent<WallCorner>();
								if (component3 != null && !component3.CanBend)
								{
									num2 = 0.008726646f * ((180f + (-y3 + transform3.rotation.eulerAngles.y) * 2f) % 360f);
								}
								else
								{
									num2 = 0.008726646f * ((180f - y3 + transform3.rotation.eulerAngles.y) % 360f);
								}
								y2 = 1f / Mathf.Tan(num2);
								num2 = 3.1415927f - num2;
								Vector3 b4 = vector3;
								b4.y = this.HeightOfs - (boxCollider.transform.position.y - vector3.y);
								vector8.y = this.HeightOfs - (boxCollider3.transform.position.y - vector8.y);
								Vector3 lhs2 = new Vector3(vector4.x, vector4.w, vector4.z) - b4;
								Vector3 rhs2 = vector8 - b4;
								z2 = Mathf.Acos(Vector3.Dot(lhs2, rhs2) / (rhs2.magnitude * lhs2.magnitude));
							}
						}
					}
					materialPropertyBlock.SetVector("_PrevData", new Vector4(num, y, z, transform2.transform.localScale.x / transform2.transform.localScale.z));
					materialPropertyBlock.SetVector("_NextData", new Vector4(num2, y2, z2, vector.x));
					if (component == null || component.type != WallCorner.Type.Hidden)
					{
						transform = transform2;
					}
					Renderer[] componentsInChildren = transform2.GetComponentsInChildren<Renderer>(this.CheckFortificationLinks);
					for (int j = 0; j < componentsInChildren.Length; j++)
					{
						componentsInChildren[j].SetPropertyBlock(materialPropertyBlock);
					}
				}
			}
		}
	}

	// Token: 0x060031A2 RID: 12706 RVA: 0x001919F8 File Offset: 0x0018FBF8
	private void DisableColliders(List<BoxCollider> cols)
	{
		for (int i = 0; i < cols.Count; i++)
		{
			cols[i].enabled = false;
		}
	}

	// Token: 0x060031A3 RID: 12707 RVA: 0x00191A24 File Offset: 0x0018FC24
	public static BoxCollider BoundsCollider(Transform t)
	{
		BoxCollider boxCollider = global::Common.FindChildComponent<BoxCollider>(t.gameObject, "ColliderHolder");
		if (boxCollider == null)
		{
			boxCollider = t.GetComponentInChildren<BoxCollider>();
		}
		return boxCollider;
	}

	// Token: 0x060031A4 RID: 12708 RVA: 0x00191A54 File Offset: 0x0018FC54
	public bool AlignPassableAreas(bool snap = false)
	{
		int count = this.segments_towers.Count;
		Transform transform = null;
		if (count != 0)
		{
			transform = this.segments_towers[count - 1].transform;
		}
		List<BoxCollider> list = new List<BoxCollider>();
		for (int i = 0; i < count; i++)
		{
			Transform transform2 = this.segments_towers[i].transform;
			WallCorner component = transform2.GetComponent<WallCorner>();
			if (component != null && !component.CanBend)
			{
				transform = transform2;
			}
			else
			{
				BoxCollider boxCollider = Wall.BoundsCollider(transform2);
				if (!(boxCollider == null))
				{
					if (component != null && component.type == WallCorner.Type.Gate)
					{
						list.Add(boxCollider);
					}
					Vector3 vector = boxCollider.size * 0.5f;
					vector.z = 0f;
					Vector3 vector2 = boxCollider.transform.TransformPoint(boxCollider.center - vector);
					Vector3 vector3 = boxCollider.transform.TransformPoint(boxCollider.center + vector);
					float terrainHeight = global::Common.GetTerrainHeight(vector2, null, false);
					float terrainHeight2 = global::Common.GetTerrainHeight(vector3, null, false);
					Vector4 vector4 = new Vector4(vector2.x, terrainHeight, vector2.z, vector2.y);
					Vector4 vector5 = new Vector4(vector3.x, terrainHeight2, vector3.z, this.HeightOfs - (boxCollider.transform.position.y - vector2.y));
					float num = 0f;
					float num2 = 0f;
					float z = 0f;
					float z2 = 0f;
					float y = 0f;
					float y2 = 0f;
					float y3 = transform2.rotation.eulerAngles.y;
					if (transform != null)
					{
						BoxCollider boxCollider2 = Wall.BoundsCollider(transform);
						if (boxCollider2 != null)
						{
							Vector3 b = boxCollider2.size * 0.5f;
							b.z = 0f;
							Vector3 vector6 = boxCollider2.transform.TransformPoint(boxCollider2.center + b);
							Vector3 vector7 = vector2 - vector6;
							vector7.y = 0f;
							if (vector7.sqrMagnitude < 0.7f)
							{
								WallCorner component2 = transform.GetComponent<WallCorner>();
								if (component2 != null && component2.type == WallCorner.Type.Gate)
								{
									list.Add(boxCollider2);
								}
								if (component2 != null && !component2.CanBend)
								{
									num = 0.008726646f * (180f + (-transform.rotation.eulerAngles.y + y3) * 2f) % 360f;
								}
								else
								{
									num = 0.008726646f * (180f - transform.rotation.eulerAngles.y + y3) % 360f;
								}
								y = 1f / Mathf.Tan(num);
								num = 3.1415927f - num;
								Vector3 b2 = vector2;
								b2.y = this.HeightOfs - (boxCollider.transform.position.y - vector2.y);
								vector6.y = this.HeightOfs - (boxCollider2.transform.position.y - vector6.y);
								Vector3 lhs = vector6 - b2;
								Vector3 rhs = new Vector3(vector5.x, vector5.w, vector5.z) - b2;
								z = Mathf.Acos(Vector3.Dot(lhs, rhs) / (lhs.magnitude * rhs.magnitude));
							}
						}
					}
					Vector3 normal = transform2.forward;
					Transform transform3;
					if (i != count - 1)
					{
						transform3 = this.segments_towers[i + 1].transform;
					}
					else
					{
						transform3 = this.segments_towers[0].transform;
					}
					if (transform3 != null)
					{
						normal = Vector3.Cross((transform3.position - transform2.position).normalized, Vector3.up).normalized;
						BoxCollider boxCollider3 = Wall.BoundsCollider(transform3);
						if (boxCollider3 != null)
						{
							Vector3 b3 = boxCollider3.size * 0.5f;
							b3.z = 0f;
							Vector3 vector8 = boxCollider3.transform.TransformPoint(boxCollider3.center - b3);
							Vector3 vector9 = vector8 - vector3;
							vector9.y = 0f;
							if (vector9.sqrMagnitude < 0.7f)
							{
								WallCorner component3 = transform3.GetComponent<WallCorner>();
								if (component3 != null && component3.type == WallCorner.Type.Gate)
								{
									list.Add(boxCollider3);
								}
								if (component3 != null && !component3.CanBend)
								{
									num2 = 0.008726646f * ((180f + (-y3 + transform3.rotation.eulerAngles.y) * 2f) % 360f);
								}
								else
								{
									num2 = 0.008726646f * ((180f - y3 + transform3.rotation.eulerAngles.y) % 360f);
								}
								y2 = 1f / Mathf.Tan(num2);
								num2 = 3.1415927f - num2;
								Vector3 b4 = vector3;
								b4.y = this.HeightOfs - (boxCollider.transform.position.y - vector3.y);
								vector8.y = this.HeightOfs - (boxCollider3.transform.position.y - vector8.y);
								Vector3 lhs2 = new Vector3(vector4.x, vector4.w, vector4.z) - b4;
								Vector3 rhs2 = vector8 - b4;
								z2 = Mathf.Acos(Vector3.Dot(lhs2, rhs2) / (rhs2.magnitude * lhs2.magnitude));
							}
						}
					}
					Vector4 vector10 = new Vector4(num, y, z, transform2.transform.localScale.x / transform2.transform.localScale.z);
					Vector4 vector11 = new Vector4(num2, y2, z2, vector.x);
					foreach (PassableArea passableArea in transform2.GetComponentsInChildren<PassableArea>(true))
					{
						passableArea.normal = normal;
						if (!passableArea.gameObject.activeInHierarchy)
						{
							BattleMap battleMap = BattleMap.Get();
							PassableAreaManager passableAreaManager = (battleMap != null) ? battleMap.paManager : null;
							if (passableAreaManager != null)
							{
								passableAreaManager.AddArea(passableArea);
							}
						}
						for (int k = 0; k < 4; k++)
						{
							Transform cornerTransform = passableArea.GetCornerTransform(k);
							if (cornerTransform == null)
							{
								return false;
							}
							Vector4 vector12 = cornerTransform.position;
							if (vector4.x != 0f)
							{
								float num3 = vector12.y - vector4.w;
								Vector3 rhs3 = new Vector3(vector12.x, 0f, vector12.z) - new Vector3(vector4.x, 0f, vector4.z);
								Vector3 vector13 = new Vector3(vector5.x, 0f, vector5.z) - new Vector3(vector4.x, 0f, vector4.z);
								float t = Vector3.Dot(vector13, rhs3) / Vector3.Dot(vector13, vector13);
								float num4 = Mathf.Lerp(vector4.y, vector5.y, t);
								vector12.y = num4 + num3 + vector5.w;
								cornerTransform.position = vector12;
							}
							if (vector10.w != 0f)
							{
								vector12 = cornerTransform.position;
								Vector4 vector14 = transform2.InverseTransformPoint(vector12);
								float y4 = vector10.y;
								float y5 = vector11.y;
								float w = vector10.w;
								float w2 = vector11.w;
								float z3 = vector14.z;
								float num5 = z3 / Mathf.Abs(z3) / w;
								float num6 = -Mathf.Abs(z3) * y5 * num5;
								float num7 = -Mathf.Abs(z3) * y4 * num5;
								if (vector14.x >= 0f)
								{
									vector14.x = vector14.x * (w2 + num6) / w2;
								}
								else
								{
									vector14.x = vector14.x * (w2 + num7) / w2;
								}
								cornerTransform.position = transform2.TransformPoint(vector14);
							}
						}
					}
					transform = transform2;
				}
			}
		}
		count = this.segments_towers.Count;
		transform = this.segments_towers[count - 1].transform;
		for (int l = 0; l < count; l++)
		{
			Transform transform4 = this.segments_towers[l].transform;
			WallCorner component4 = transform4.GetComponent<WallCorner>();
			if (component4 != null && !component4.CanBend)
			{
				transform = transform4;
			}
			else
			{
				Transform transform5;
				if (l != count - 1)
				{
					transform5 = this.segments_towers[l + 1].transform;
				}
				else
				{
					transform5 = this.segments_towers[0].transform;
				}
				foreach (PassableArea passableArea2 in transform4.GetComponentsInChildren<PassableArea>(true))
				{
					for (int n = 0; n < 4; n++)
					{
						PassableArea.Node node = passableArea2.nodePoints[n];
						Point pt = passableArea2.GetCornerTransform(n).position;
						Point pt2 = passableArea2.GetCornerTransform((n + 1) % 4).position;
						Point pt3 = (pt + pt2) / 2f;
						if (node.type == PathData.PassableAreaNode.Type.Edge)
						{
							PassableArea[] componentsInChildren3 = transform5.GetComponentsInChildren<PassableArea>();
							PassableArea[] componentsInChildren4 = transform.GetComponentsInChildren<PassableArea>();
							PassableArea passableArea3 = null;
							int num8 = -1;
							float num9 = float.MaxValue;
							foreach (PassableArea passableArea4 in componentsInChildren3)
							{
								for (int num11 = 0; num11 < 4; num11++)
								{
									if (passableArea4.nodePoints[num11].type == PathData.PassableAreaNode.Type.Edge)
									{
										Point pt4 = passableArea4.GetCornerTransform(num11).position;
										Point pt5 = passableArea4.GetCornerTransform((n + 1) % 4).position;
										float num12 = ((pt4 + pt5) / 2f).SqrDist(pt3);
										if (num12 < num9)
										{
											num9 = num12;
											num8 = num11;
											passableArea3 = passableArea4;
										}
									}
								}
							}
							foreach (PassableArea passableArea5 in componentsInChildren4)
							{
								for (int num14 = 0; num14 < 4; num14++)
								{
									if (passableArea5.nodePoints[num14].type == PathData.PassableAreaNode.Type.Edge)
									{
										Point pt6 = passableArea5.GetCornerTransform(num14).position;
										Point pt7 = passableArea5.GetCornerTransform((n + 1) % 4).position;
										float num15 = ((pt6 + pt7) / 2f).SqrDist(pt3);
										if (num15 < num9)
										{
											num9 = num15;
											num8 = num14;
											passableArea3 = passableArea5;
										}
									}
								}
							}
							if (passableArea3 != null)
							{
								int num16 = num8;
								int num17 = (num8 + 1) % 4;
								Transform cornerTransform2 = passableArea3.GetCornerTransform(num16);
								Transform cornerTransform3 = passableArea3.GetCornerTransform(num17);
								Point pt8 = cornerTransform2.position;
								Point pt9 = cornerTransform3.position;
								float num18 = pt.SqrDist(pt8);
								float num19 = pt.SqrDist(pt9);
								float num20 = pt2.SqrDist(pt8);
								float num21 = pt2.SqrDist(pt9);
								if (num19 < num18 || num21 > num20)
								{
									int num22 = num16;
									num16 = num17;
									num17 = num22;
								}
								PassableArea.ConnectNodes(passableArea2, passableArea3, n, num16, false);
								PassableArea.ConnectNodes(passableArea2, passableArea3, (n + 1) % 4, num17, false);
							}
						}
					}
				}
				transform = transform4;
				PassableArea[] componentsInChildren2;
				if (snap && componentsInChildren2.Length != 0 && !PassableArea.WeldAllCorners(null))
				{
					return false;
				}
			}
		}
		this.DisableColliders(list);
		this.FixAllDoors();
		return true;
	}

	// Token: 0x060031A5 RID: 12709 RVA: 0x0019263C File Offset: 0x0019083C
	private void FixAllDoors()
	{
		if (!this.FixDoorHeight)
		{
			return;
		}
		int count = this.segments_towers.Count;
		for (int i = 0; i < count; i++)
		{
			Transform transform = this.segments_towers[i].transform;
			WallCorner component = transform.GetComponent<WallCorner>();
			if (!(component == null) && (component.type == WallCorner.Type.Gate || component.type == WallCorner.Type.Tower))
			{
				Transform transform2;
				Transform transform3;
				if (i == 0)
				{
					transform2 = this.segments_towers[count - 1].transform;
					transform3 = this.segments_towers[1].transform;
				}
				else if (i != count - 1)
				{
					transform2 = this.segments_towers[i - 1].transform;
					transform3 = this.segments_towers[i + 1].transform;
				}
				else
				{
					transform2 = this.segments_towers[i - 1].transform;
					transform3 = this.segments_towers[0].transform;
				}
				foreach (PassableArea passableArea in transform.GetComponentsInChildren<PassableArea>(true))
				{
					for (int k = 0; k < 4; k++)
					{
						PassableArea.Node node = passableArea.nodePoints[k];
						Point pt = passableArea.GetCornerTransform(k).position;
						Point pt2 = passableArea.GetCornerTransform((k + 1) % 4).position;
						Point pt3 = (pt + pt2) / 2f;
						if (node.type == PathData.PassableAreaNode.Type.Edge)
						{
							PassableArea[] componentsInChildren2 = transform3.GetComponentsInChildren<PassableArea>();
							PassableArea[] componentsInChildren3 = transform2.GetComponentsInChildren<PassableArea>();
							PassableArea passableArea2 = null;
							int num = -1;
							float num2 = float.MaxValue;
							Transform transform4 = null;
							foreach (PassableArea passableArea3 in componentsInChildren2)
							{
								for (int m = 0; m < 4; m++)
								{
									if (passableArea3.nodePoints[m].type == PathData.PassableAreaNode.Type.Edge)
									{
										Point pt4 = passableArea3.GetCornerTransform(m).position;
										Point pt5 = passableArea3.GetCornerTransform((k + 1) % 4).position;
										float num3 = ((pt4 + pt5) / 2f).SqrDist(pt3);
										if (num3 < num2)
										{
											num2 = num3;
											num = m;
											passableArea2 = passableArea3;
											transform4 = transform3;
										}
									}
								}
							}
							foreach (PassableArea passableArea4 in componentsInChildren3)
							{
								for (int num4 = 0; num4 < 4; num4++)
								{
									if (passableArea4.nodePoints[num4].type == PathData.PassableAreaNode.Type.Edge)
									{
										Point pt6 = passableArea4.GetCornerTransform(num4).position;
										Point pt7 = passableArea4.GetCornerTransform((k + 1) % 4).position;
										float num5 = ((pt6 + pt7) / 2f).SqrDist(pt3);
										if (num5 < num2)
										{
											num2 = num5;
											num = num4;
											passableArea2 = passableArea4;
											transform4 = transform2;
										}
									}
								}
							}
							if (passableArea2 != null)
							{
								int corner = num;
								int corner2 = (num + 1) % 4;
								Transform cornerTransform = passableArea2.GetCornerTransform(corner);
								Transform cornerTransform2 = passableArea2.GetCornerTransform(corner2);
								this.SnapDoors(transform.gameObject, cornerTransform, cornerTransform2, "_door_obj");
								this.SnapDoors(transform4.gameObject, cornerTransform, cornerTransform2, "_door_obj");
								this.SnapDoors(transform.gameObject, cornerTransform, cornerTransform2, "_door_obj_destroyed");
								this.SnapDoors(transform4.gameObject, cornerTransform, cornerTransform2, "_door_obj_destroyed");
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x060031A6 RID: 12710 RVA: 0x001929D4 File Offset: 0x00190BD4
	private void SnapDoors(GameObject t, Transform c1, Transform c2, string name)
	{
		List<GameObject> list = new List<GameObject>();
		global::Common.FindChildrenByName(list, t.gameObject, name, true, true);
		GameObject gameObject = null;
		float num = float.MaxValue;
		Vector3 vector = (c1.transform.position + c2.transform.position) / 2f;
		for (int i = 0; i < list.Count; i++)
		{
			GameObject gameObject2 = list[i];
			if (gameObject2 != null)
			{
				float num2 = Vector3.Distance(gameObject2.transform.position, vector);
				if (num2 < num)
				{
					gameObject = gameObject2;
					num = num2;
				}
			}
		}
		if (gameObject != null)
		{
			float y = vector.y;
			Vector3 position = gameObject.transform.position;
			position.y = y;
			gameObject.transform.position = position;
		}
	}

	// Token: 0x060031A7 RID: 12711 RVA: 0x00192AA4 File Offset: 0x00190CA4
	private void DisableColliders(List<Collider> to_disable)
	{
		if (to_disable != null)
		{
			for (int i = 0; i < to_disable.Count; i++)
			{
				to_disable[i].enabled = false;
			}
		}
	}

	// Token: 0x060031A8 RID: 12712 RVA: 0x000023FD File Offset: 0x000005FD
	private void Batch()
	{
	}

	// Token: 0x060031A9 RID: 12713 RVA: 0x00192AD4 File Offset: 0x00190CD4
	public void Refresh(bool forced = false)
	{
		this.started = true;
		this.LoadArchitectureSet();
		this.FindCorners();
		this.SortCorners();
		this.CalcCurves();
		this.UpdateCornerLevels(forced);
		this.RotateCorners();
		this.CreateSegments();
		this.Snap();
		this.avg_radius = this.CalcRadius();
		this.Batch();
	}

	// Token: 0x060031AA RID: 12714 RVA: 0x00192B2C File Offset: 0x00190D2C
	public float CalcRadius()
	{
		if (this.corners.Count <= 0)
		{
			return 0f;
		}
		float num = 0f;
		foreach (WallCorner wallCorner in this.corners)
		{
			Vector3 vector = wallCorner.transform.position - base.transform.position;
			vector.y = 0f;
			float magnitude = vector.magnitude;
			num += magnitude;
		}
		num /= (float)this.corners.Count;
		return num;
	}

	// Token: 0x060031AB RID: 12715 RVA: 0x00192BD4 File Offset: 0x00190DD4
	public void MoveCorner(WallCorner corner, Vector3 new_pos)
	{
		Vector3 position = corner.transform.position;
		float num = position.y - global::Common.GetTerrainHeight(position, null, false);
		new_pos.y = global::Common.GetTerrainHeight(new_pos, null, false) + num;
		corner.transform.position = new_pos;
		corner.transform.hasChanged = false;
	}

	// Token: 0x060031AC RID: 12716 RVA: 0x00192C28 File Offset: 0x00190E28
	public void Resize(float scale)
	{
		this.FindCorners();
		if (this.corners.Count < 2)
		{
			return;
		}
		foreach (WallCorner wallCorner in this.corners)
		{
			Vector3 a = wallCorner.transform.position - base.transform.position;
			Vector3 new_pos = base.transform.position + a * scale;
			this.MoveCorner(wallCorner, new_pos);
		}
		this.Refresh(false);
	}

	// Token: 0x060031AD RID: 12717 RVA: 0x00192CCC File Offset: 0x00190ECC
	public void Circle(float radius = 0f)
	{
		this.FindCorners();
		if (this.corners.Count < 2)
		{
			return;
		}
		float num = 0f;
		float num2 = 6.2831855f / (float)this.corners.Count;
		foreach (WallCorner wallCorner in this.corners)
		{
			float num3;
			if (radius > 0f)
			{
				num3 = radius;
			}
			else
			{
				Vector3 vector = wallCorner.transform.position - base.transform.position;
				vector.y = 0f;
				num3 = vector.magnitude;
			}
			Vector3 new_pos = base.transform.position + new Vector3(Mathf.Cos(num) * num3, 0f, Mathf.Sin(num) * num3);
			this.MoveCorner(wallCorner, new_pos);
			num += num2;
		}
		this.Refresh(false);
	}

	// Token: 0x060031AE RID: 12718 RVA: 0x00192DD0 File Offset: 0x00190FD0
	private void OnEnable()
	{
		if (this.started && Application.isPlaying)
		{
			return;
		}
		if (base.GetComponentInParent<global::Settlement>() == null)
		{
			this.Refresh(false);
		}
	}

	// Token: 0x060031AF RID: 12719 RVA: 0x00192DF7 File Offset: 0x00190FF7
	private void Start()
	{
		this.started = true;
	}

	// Token: 0x060031B0 RID: 12720 RVA: 0x000023FD File Offset: 0x000005FD
	private void OnDisable()
	{
	}

	// Token: 0x0400211C RID: 8476
	public string towers_architecture = "";

	// Token: 0x0400211D RID: 8477
	public string segments_architecture = "";

	// Token: 0x0400211E RID: 8478
	public int level = 1;

	// Token: 0x0400211F RID: 8479
	public int towers_variant = 1;

	// Token: 0x04002120 RID: 8480
	public int segments_variant = 1;

	// Token: 0x04002121 RID: 8481
	public float HeightScale = 1f;

	// Token: 0x04002122 RID: 8482
	public float ThicknessScale = 1f;

	// Token: 0x04002123 RID: 8483
	public float MaxLengthScale = 2f;

	// Token: 0x04002124 RID: 8484
	public float HeightOfs;

	// Token: 0x04002125 RID: 8485
	public float Curveness = 1f;

	// Token: 0x04002126 RID: 8486
	public bool FixDoorHeight;

	// Token: 0x04002127 RID: 8487
	public bool CheckFortificationLinks;

	// Token: 0x04002128 RID: 8488
	private List<GameObject> segments_towers = new List<GameObject>();

	// Token: 0x04002129 RID: 8489
	private PrefabGrid.Info towers_PGI;

	// Token: 0x0400212A RID: 8490
	private PrefabGrid.Info segments_PGI;

	// Token: 0x0400212B RID: 8491
	private PrefabGrid.Info begin_segments_PGI;

	// Token: 0x0400212C RID: 8492
	private PrefabGrid.Info end_segments_PGI;

	// Token: 0x0400212D RID: 8493
	[NonSerialized]
	public List<WallCorner> corners = new List<WallCorner>();

	// Token: 0x0400212E RID: 8494
	[NonSerialized]
	public List<GameObject> segments = new List<GameObject>();

	// Token: 0x0400212F RID: 8495
	[NonSerialized]
	public float avg_radius = -1f;

	// Token: 0x04002130 RID: 8496
	private bool started;

	// Token: 0x04002131 RID: 8497
	private Bounds aabb;

	// Token: 0x04002132 RID: 8498
	private const float min_dist_between_shader_segs = 0.7f;
}
