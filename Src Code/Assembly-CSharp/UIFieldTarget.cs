using System;

// Token: 0x020000B2 RID: 178
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class UIFieldTarget : Attribute
{
	// Token: 0x0600065E RID: 1630 RVA: 0x000443A4 File Offset: 0x000425A4
	public UIFieldTarget(string objectName)
	{
		this.objectName = objectName;
		this.findMultiple = false;
	}

	// Token: 0x0600065F RID: 1631 RVA: 0x000443BA File Offset: 0x000425BA
	public UIFieldTarget(string objectName, bool findMultiple)
	{
		this.objectName = objectName;
		this.findMultiple = findMultiple;
	}

	// Token: 0x040005D9 RID: 1497
	public string objectName;

	// Token: 0x040005DA RID: 1498
	public bool findMultiple;
}
