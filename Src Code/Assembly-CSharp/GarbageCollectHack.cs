using System;
using Logic;

// Token: 0x02000131 RID: 305
public static class GarbageCollectHack
{
	// Token: 0x06001054 RID: 4180 RVA: 0x000AD384 File Offset: 0x000AB584
	public static void Collect()
	{
		Game game = GameLogic.Get(false);
		if (game == null)
		{
			return;
		}
		if (game.time_unscaled > GarbageCollectHack.nextCollect)
		{
			GarbageCollectHack.nextCollect = game.time_unscaled + 900f;
			GC.Collect(0, GCCollectionMode.Optimized, false);
		}
	}

	// Token: 0x04000AB7 RID: 2743
	public const int cooldown = 900;

	// Token: 0x04000AB8 RID: 2744
	public static Time nextCollect = Time.Zero + 900f;
}
