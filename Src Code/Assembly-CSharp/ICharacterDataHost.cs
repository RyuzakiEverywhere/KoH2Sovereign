using System;
using Logic;

// Token: 0x020002FF RID: 767
public interface ICharacterDataHost
{
	// Token: 0x1400003C RID: 60
	// (add) Token: 0x0600300D RID: 12301
	// (remove) Token: 0x0600300E RID: 12302
	event Action<ICharacterDataHost> OnChange;

	// Token: 0x0600300F RID: 12303
	void SetCharacterData(Logic.Character data);

	// Token: 0x06003010 RID: 12304
	Logic.Character GetCharacterData();
}
