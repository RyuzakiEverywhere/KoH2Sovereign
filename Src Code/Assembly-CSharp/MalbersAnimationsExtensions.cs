using System;
using System.Collections;
using System.Reflection;
using Logic;
using UnityEngine;

// Token: 0x02000056 RID: 86
public static class MalbersAnimationsExtensions
{
	// Token: 0x06000203 RID: 515 RVA: 0x0001F868 File Offset: 0x0001DA68
	public static Transform FindGrandChild(this Transform aParent, string aName)
	{
		Transform transform = aParent.Find(aName);
		if (transform != null)
		{
			return transform;
		}
		foreach (object obj in aParent)
		{
			transform = ((Transform)obj).FindGrandChild(aName);
			if (transform != null)
			{
				return transform;
			}
		}
		return null;
	}

	// Token: 0x06000204 RID: 516 RVA: 0x0001F8E0 File Offset: 0x0001DAE0
	public static Vector3 DeltaPositionFromRotate(this Transform transform, Vector3 point, Vector3 axis, float deltaAngle)
	{
		Vector3 vector = transform.position;
		Vector3 vector2 = vector - point;
		vector2 = Quaternion.AngleAxis(deltaAngle, axis) * vector2;
		vector = point + vector2 - vector;
		vector.y = 0f;
		return vector;
	}

	// Token: 0x06000205 RID: 517 RVA: 0x0001F928 File Offset: 0x0001DB28
	public static void InvokeWithParams(this MonoBehaviour sender, string method, object args)
	{
		Type type = null;
		if (args != null)
		{
			type = args.GetType();
		}
		MethodInfo method2;
		if (type != null)
		{
			method2 = sender.GetType().GetMethod(method, new Type[]
			{
				type
			});
		}
		else
		{
			method2 = sender.GetType().GetMethod(method);
		}
		if (!(method2 != null))
		{
			PropertyInfo property = sender.GetType().GetProperty(method);
			if (property != null)
			{
				property.SetValue(sender, args, null);
			}
			return;
		}
		if (args != null)
		{
			object[] parameters = new object[]
			{
				args
			};
			method2.Invoke(sender, parameters);
			return;
		}
		method2.Invoke(sender, null);
	}

	// Token: 0x06000206 RID: 518 RVA: 0x0001F9BC File Offset: 0x0001DBBC
	public static void InvokeDelay(this MonoBehaviour behaviour, string method, object options, YieldInstruction wait)
	{
		behaviour.StartCoroutine(behaviour._invoke(method, wait, options));
	}

	// Token: 0x06000207 RID: 519 RVA: 0x0001F9CE File Offset: 0x0001DBCE
	private static IEnumerator _invoke(this MonoBehaviour behaviour, string method, YieldInstruction wait, object options)
	{
		yield return wait;
		behaviour.GetType().GetMethod(method).Invoke(behaviour, new object[]
		{
			options
		});
		yield return null;
		yield break;
	}

	// Token: 0x06000208 RID: 520 RVA: 0x0001F9F4 File Offset: 0x0001DBF4
	public static void Invoke(this ScriptableObject sender, string method, object args)
	{
		MethodInfo method2 = sender.GetType().GetMethod(method);
		if (method2 != null)
		{
			if (args != null)
			{
				object[] parameters = new object[]
				{
					args
				};
				method2.Invoke(sender, parameters);
				return;
			}
			method2.Invoke(sender, null);
		}
	}

	// Token: 0x06000209 RID: 521 RVA: 0x0001FA38 File Offset: 0x0001DC38
	public static void SetLayer(this GameObject parent, int layer, bool includeChildren = true)
	{
		parent.layer = layer;
		if (includeChildren)
		{
			Transform[] componentsInChildren = parent.transform.GetComponentsInChildren<Transform>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].gameObject.layer = layer;
			}
		}
	}

	// Token: 0x0600020A RID: 522 RVA: 0x0001FA78 File Offset: 0x0001DC78
	public static void SetKingdomColors(this GameObject parent, Logic.Kingdom k)
	{
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		global::Kingdom kingdom = k.visuals as global::Kingdom;
		Color value = Color.white;
		if (kingdom != null)
		{
			value = kingdom.unitColor;
		}
		else
		{
			MapData mapData = MapData.Get();
			value = global::Kingdom.PickClosestColor((mapData != null) ? mapData.unit_colors : null, global::Defs.GetColor(k.def, "primary_color", null));
		}
		materialPropertyBlock.SetColor("_Color", value);
		Renderer[] componentsInChildren = parent.transform.GetComponentsInChildren<Renderer>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].SetPropertyBlock(materialPropertyBlock);
		}
	}
}
