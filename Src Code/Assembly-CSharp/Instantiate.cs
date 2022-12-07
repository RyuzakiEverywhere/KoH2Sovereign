using System;
using UnityEngine;

// Token: 0x02000043 RID: 67
public class Instantiate : MonoBehaviour
{
	// Token: 0x0600018F RID: 399 RVA: 0x0000FBC4 File Offset: 0x0000DDC4
	private void Update()
	{
		Vector3 vector = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
		if (Input.GetMouseButtonDown(0))
		{
			RaycastHit raycastHit;
			Vector3 position;
			if (Physics.Raycast(Camera.main.ScreenPointToRay(vector), out raycastHit, 1000f))
			{
				position = raycastHit.point;
			}
			else
			{
				position = Camera.main.ScreenToWorldPoint(vector);
			}
			Object.Instantiate<Transform>(this.ObjectInst, position, Quaternion.identity);
		}
	}

	// Token: 0x040002AF RID: 687
	public Transform ObjectInst;

	// Token: 0x040002B0 RID: 688
	public float distance = 2f;
}
