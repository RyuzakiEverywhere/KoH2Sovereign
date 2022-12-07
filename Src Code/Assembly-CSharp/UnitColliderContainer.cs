using System;
using UnityEngine;

// Token: 0x02000155 RID: 341
public class UnitColliderContainer : MonoBehaviour
{
	// Token: 0x06001184 RID: 4484 RVA: 0x000B824C File Offset: 0x000B644C
	public static UnitColliderContainer Create(Unit unit, Transform parent = null)
	{
		GameObject gameObject = Common.SpawnTemplate("UnitColliderContainer", unit.logic.def.field.key + "_Container", parent, true, new Type[]
		{
			typeof(BoxCollider),
			typeof(UnitColliderContainer)
		});
		if (parent == null)
		{
			Common.SetObjectParent(gameObject, GameLogic.instance.transform, "Units");
		}
		gameObject.GetComponent<BoxCollider>().size = new Vector3(1.5f, 3f, 1.5f);
		UnitColliderContainer component = gameObject.GetComponent<UnitColliderContainer>();
		component.unit = unit;
		return component;
	}

	// Token: 0x06001185 RID: 4485 RVA: 0x000B82EF File Offset: 0x000B64EF
	public void SetPosition(Vector3 pos)
	{
		base.transform.localPosition = pos;
		this.unit.SetPosition(base.transform.position);
	}

	// Token: 0x06001186 RID: 4486 RVA: 0x000B8313 File Offset: 0x000B6513
	public void SetRotation(Vector3 rot)
	{
		base.transform.localEulerAngles = rot;
		this.unit.SetRotation(base.transform.rotation);
	}

	// Token: 0x04000BA1 RID: 2977
	public Unit unit;
}
