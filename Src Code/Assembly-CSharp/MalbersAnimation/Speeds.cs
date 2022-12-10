using System;

namespace MalbersAnimations
{
	// Token: 0x020003C0 RID: 960
	[Serializable]
	public struct Speeds
	{
		// Token: 0x060036A4 RID: 13988 RVA: 0x001B324C File Offset: 0x001B144C
		public Speeds(int defaultt)
		{
			this.position = 0f;
			this.animator = 1f;
			this.lerpPosition = 2f;
			this.lerpAnimator = 2f;
			this.rotation = 0f;
			this.lerpRotation = 2f;
			this.name = string.Empty;
		}

		// Token: 0x060036A5 RID: 13989 RVA: 0x001B32A8 File Offset: 0x001B14A8
		public Speeds(float lerpPos, float lerpanim, float lerpTurn)
		{
			this.position = 0f;
			this.animator = 1f;
			this.rotation = 0f;
			this.lerpPosition = lerpPos;
			this.lerpAnimator = lerpanim;
			this.lerpRotation = lerpTurn;
			this.name = string.Empty;
		}

		// Token: 0x0400266A RID: 9834
		public string name;

		// Token: 0x0400266B RID: 9835
		public float position;

		// Token: 0x0400266C RID: 9836
		public float animator;

		// Token: 0x0400266D RID: 9837
		public float lerpPosition;

		// Token: 0x0400266E RID: 9838
		public float lerpAnimator;

		// Token: 0x0400266F RID: 9839
		public float rotation;

		// Token: 0x04002670 RID: 9840
		public float lerpRotation;
	}
}
