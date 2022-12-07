using System;
using Logic;

// Token: 0x02000109 RID: 265
[Serializable]
public class CoATransform
{
	// Token: 0x06000C35 RID: 3125 RVA: 0x000894DC File Offset: 0x000876DC
	public CoATransform()
	{
	}

	// Token: 0x06000C36 RID: 3126 RVA: 0x0008954C File Offset: 0x0008774C
	public CoATransform(CoATransform transform)
	{
		this.tileX = transform.tileX;
		this.tileY = transform.tileY;
		this.offsetX = transform.offsetX;
		this.offsetY = transform.offsetY;
		this.position = transform.position;
		this.scale = transform.scale;
		this.rotation = transform.rotation;
		this.pivot = transform.pivot;
		this.isDefault = false;
	}

	// Token: 0x06000C37 RID: 3127 RVA: 0x00089624 File Offset: 0x00087824
	public CoATransform(DT.Field field)
	{
		this.tileX = field.GetFloat("tile_x", null, this.tileX, true, true, true, '.');
		this.tileY = field.GetFloat("tile_y", null, this.tileY, true, true, true, '.');
		this.offsetX = field.GetFloat("offset_x", null, this.offsetX, true, true, true, '.');
		this.offsetY = field.GetFloat("offset_y", null, this.offsetY, true, true, true, '.');
		Point point = field.GetPoint("position", null, true, true, true, '.');
		if (point != Point.Invalid)
		{
			this.position = new Point(point.x, point.y);
		}
		point = field.GetPoint("scale", null, true, true, true, '.');
		if (point != Point.Invalid)
		{
			this.scale = new Point(point.x, point.y);
		}
		point = field.GetPoint("pivot", null, true, true, true, '.');
		if (point != Point.Invalid)
		{
			this.pivot = new Point(point.x, point.y);
		}
		this.rotation = field.GetFloat("rotation", null, this.rotation, true, true, true, '.');
		this.isDefault = false;
	}

	// Token: 0x0400098C RID: 2444
	public float tileX = 1f;

	// Token: 0x0400098D RID: 2445
	public float tileY = 1f;

	// Token: 0x0400098E RID: 2446
	public float offsetX;

	// Token: 0x0400098F RID: 2447
	public float offsetY;

	// Token: 0x04000990 RID: 2448
	public Point position = new Point(0f, 0f);

	// Token: 0x04000991 RID: 2449
	public Point scale = new Point(1f, 1f);

	// Token: 0x04000992 RID: 2450
	public Point pivot = new Point(0.5f, 0.5f);

	// Token: 0x04000993 RID: 2451
	public float rotation;

	// Token: 0x04000994 RID: 2452
	public bool isDefault = true;
}
