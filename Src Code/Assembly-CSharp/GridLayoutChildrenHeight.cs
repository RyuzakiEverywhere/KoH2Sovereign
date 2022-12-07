using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002E6 RID: 742
public class GridLayoutChildrenHeight : MonoBehaviour
{
	// Token: 0x06002F0A RID: 12042 RVA: 0x001835D8 File Offset: 0x001817D8
	private void Update()
	{
		List<GameObject> list = new List<GameObject>();
		if (string.IsNullOrEmpty(this.childsName))
		{
			using (IEnumerator enumerator = base.gameObject.transform.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					Transform transform = (Transform)obj;
					list.Add(InstanceHolder.Resolve(transform.gameObject));
				}
				goto IL_7B;
			}
		}
		Common.FindChildrenByName(list, base.gameObject, this.childsName, true, true);
		IL_7B:
		Vector2 cellSize = base.gameObject.GetComponent<GridLayoutGroup>().cellSize;
		float num = cellSize.x;
		float num2 = cellSize.y;
		for (int i = 0; i < list.Count; i++)
		{
			num = Mathf.Max(list[i].GetComponent<RectTransform>().rect.width, num);
			num2 = Mathf.Max(list[i].GetComponent<RectTransform>().rect.height, num2);
		}
		cellSize = new Vector2(num, num2);
		base.gameObject.GetComponent<GridLayoutGroup>().cellSize = cellSize;
	}

	// Token: 0x04001FBF RID: 8127
	public string childsName = "Text";
}
