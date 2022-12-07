using System;

// Token: 0x0200003A RID: 58
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class ConsoleMethod : Attribute
{
	// Token: 0x06000149 RID: 329 RVA: 0x0000C6F4 File Offset: 0x0000A8F4
	public ConsoleMethod()
	{
		this.command = "";
		this.help = "";
	}

	// Token: 0x0600014A RID: 330 RVA: 0x0000C712 File Offset: 0x0000A912
	public ConsoleMethod(string command)
	{
		this.command = command;
		this.help = "";
	}

	// Token: 0x0600014B RID: 331 RVA: 0x0000C72C File Offset: 0x0000A92C
	public ConsoleMethod(string command, string help)
	{
		this.command = command;
		this.help = help;
	}

	// Token: 0x0400020D RID: 525
	public readonly string command;

	// Token: 0x0400020E RID: 526
	public readonly string help;
}
