using System;
using Logic;
using UnityEngine;

// Token: 0x02000319 RID: 793
public class WallCorner : MonoBehaviour
{
	// Token: 0x060031B2 RID: 12722 RVA: 0x00192E98 File Offset: 0x00191098
	public void GetExtents(out Vector3 pt1, out Vector3 pt2)
	{
		BoxCollider boxCollider = Wall.BoundsCollider(base.transform);
		pt1 = (pt2 = base.transform.position);
		if (boxCollider == null)
		{
			return;
		}
		Vector3 b = boxCollider.size * 0.5f;
		b.z = 0f;
		pt1 = boxCollider.transform.TransformPoint(boxCollider.center - b);
		pt2 = boxCollider.transform.TransformPoint(boxCollider.center + b);
	}

	// Token: 0x060031B3 RID: 12723 RVA: 0x00192F2C File Offset: 0x0019112C
	public void Load(DT.Field field, string default_architecture, int default_variant, int default_level)
	{
		Enum.TryParse<WallCorner.Type>(field.GetString("corner_type", null, "Tower", true, true, true, '.'), out this.type);
		if (this.type != WallCorner.Type.Hidden)
		{
			PrefabGrid prefabGrid = base.GetComponent<PrefabGrid>();
			if (prefabGrid == null)
			{
				prefabGrid = base.gameObject.AddComponent<PrefabGrid>();
			}
			if (this.type == WallCorner.Type.Tower)
			{
				prefabGrid.Load(field, "Towers", default_architecture, default_variant, default_level, false, false);
			}
			else if (this.type == WallCorner.Type.Gate)
			{
				prefabGrid.Load(field, "Gates", default_architecture, default_variant, default_level, false, false);
			}
		}
		else
		{
			PrefabGrid component = base.GetComponent<PrefabGrid>();
			if (component != null)
			{
				component.Despawn(true);
				UnityEngine.Object.DestroyImmediate(component);
			}
		}
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
		this.HideWall = field.GetBool("hide_wall", null, this.HideWall, true, true, true, '.');
		this.AutoCurve = field.GetBool("auto_curve", null, this.AutoCurve, true, true, true, '.');
		this.AutoRotate = field.GetBool("auto_rotate", null, this.AutoRotate, true, true, true, '.');
		this.Straight = field.GetBool("straight", null, this.Straight, true, true, true, '.');
		this.InCurve = field.GetFloat("in_curve", null, this.InCurve, true, true, true, '.');
		this.OutCurve = field.GetFloat("out_curve", null, this.OutCurve, true, true, true, '.');
		this.HeightOfs = field.GetFloat("height_ofs", null, this.HeightOfs, true, true, true, '.');
	}

	// Token: 0x060031B4 RID: 12724 RVA: 0x00193120 File Offset: 0x00191320
	public static WallCorner Load(DT.Field field, Transform parent, string default_architecture, int default_variant, int default_level)
	{
		if (field == null)
		{
			return null;
		}
		WallCorner wallCorner = global::Common.SpawnTemplate<WallCorner>("Tower", null, parent, false, new System.Type[]
		{
			typeof(WallCorner),
			typeof(PrefabGrid)
		});
		wallCorner.Load(field, default_architecture, default_variant, default_level);
		wallCorner.gameObject.SetActive(true);
		return wallCorner;
	}

	// Token: 0x060031B5 RID: 12725 RVA: 0x00193178 File Offset: 0x00191378
	public void RememberHeight()
	{
		float terrainHeight = global::Common.GetTerrainHeight(base.transform.position, null, false);
		this.HeightOfs = base.transform.position.y - terrainHeight;
	}

	// Token: 0x060031B6 RID: 12726 RVA: 0x001931B0 File Offset: 0x001913B0
	public void RestoreHeight()
	{
		Vector3 position = base.transform.position;
		float terrainHeight = global::Common.GetTerrainHeight(position, null, false);
		position.y = terrainHeight + this.HeightOfs;
		base.transform.position = position;
		base.transform.hasChanged = false;
	}

	// Token: 0x060031B7 RID: 12727 RVA: 0x001931FC File Offset: 0x001913FC
	public void RefreshWall()
	{
		Wall componentInParent = base.GetComponentInParent<Wall>();
		if (componentInParent == null)
		{
			return;
		}
		componentInParent.Refresh(false);
	}

	// Token: 0x04002133 RID: 8499
	public WallCorner.Type type = WallCorner.Type.Tower;

	// Token: 0x04002134 RID: 8500
	public bool HideWall;

	// Token: 0x04002135 RID: 8501
	public bool AutoCurve = true;

	// Token: 0x04002136 RID: 8502
	public bool AutoRotate = true;

	// Token: 0x04002137 RID: 8503
	public bool Straight;

	// Token: 0x04002138 RID: 8504
	public bool CanBend;

	// Token: 0x04002139 RID: 8505
	public float OutCurve;

	// Token: 0x0400213A RID: 8506
	public float InCurve;

	// Token: 0x0400213B RID: 8507
	public float HeightOfs;

	// Token: 0x02000880 RID: 2176
	public enum Type
	{
		// Token: 0x04003FB5 RID: 16309
		Hidden,
		// Token: 0x04003FB6 RID: 16310
		Tower,
		// Token: 0x04003FB7 RID: 16311
		Gate,
		// Token: 0x04003FB8 RID: 16312
		Disabled
	}
}
