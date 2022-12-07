using System;

// Token: 0x02000038 RID: 56
[Serializable]
public class ConsoleLog
{
	// Token: 0x06000145 RID: 325 RVA: 0x0000C690 File Offset: 0x0000A890
	public string GetPrefix()
	{
		string text = this.time.ToShortTimeString();
		return string.Concat(new string[]
		{
			"[",
			text,
			"][",
			this.type.ToString(),
			"] "
		});
	}

	// Token: 0x06000146 RID: 326 RVA: 0x0000C6E4 File Offset: 0x0000A8E4
	public string GetMessage()
	{
		return this.log;
	}

	// Token: 0x06000147 RID: 327 RVA: 0x0000C6EC File Offset: 0x0000A8EC
	public string GetStackTrace()
	{
		return this.stacktrace;
	}

	// Token: 0x04000202 RID: 514
	public DateTime time;

	// Token: 0x04000203 RID: 515
	public string log;

	// Token: 0x04000204 RID: 516
	public string stacktrace;

	// Token: 0x04000205 RID: 517
	public ELogType type;
}
