using System;
using MalbersAnimations.Events;

namespace MalbersAnimations
{
	// Token: 0x020003EB RID: 1003
	[Serializable]
	public class InputAxis
	{
		// Token: 0x1700034B RID: 843
		// (get) Token: 0x060037DD RID: 14301 RVA: 0x001BA974 File Offset: 0x001B8B74
		public float GetAxis
		{
			get
			{
				if (this.inputSystem == null || !this.active)
				{
					return 0f;
				}
				this.currentAxisValue = (this.raw ? this.inputSystem.GetAxisRaw(this.input) : this.inputSystem.GetAxis(this.input));
				return this.currentAxisValue;
			}
		}

		// Token: 0x1700034C RID: 844
		// (get) Token: 0x060037DE RID: 14302 RVA: 0x001BA9CF File Offset: 0x001B8BCF
		// (set) Token: 0x060037DF RID: 14303 RVA: 0x001BA9D7 File Offset: 0x001B8BD7
		public IInputSystem InputSystem
		{
			get
			{
				return this.inputSystem;
			}
			set
			{
				this.inputSystem = value;
			}
		}

		// Token: 0x060037E0 RID: 14304 RVA: 0x001BA9E0 File Offset: 0x001B8BE0
		public InputAxis()
		{
			this.active = true;
			this.raw = true;
			this.input = "Value";
			this.name = "NewAxis";
			this.inputSystem = new DefaultInput();
		}

		// Token: 0x060037E1 RID: 14305 RVA: 0x001BAA5C File Offset: 0x001B8C5C
		public InputAxis(string value)
		{
			this.active = true;
			this.raw = false;
			this.input = value;
			this.name = "NewAxis";
			this.inputSystem = new DefaultInput();
		}

		// Token: 0x060037E2 RID: 14306 RVA: 0x001BAAD4 File Offset: 0x001B8CD4
		public InputAxis(string InputValue, bool active, bool isRaw)
		{
			this.active = active;
			this.raw = isRaw;
			this.input = InputValue;
			this.name = "NewAxis";
			this.inputSystem = new DefaultInput();
		}

		// Token: 0x060037E3 RID: 14307 RVA: 0x001BAB4C File Offset: 0x001B8D4C
		public InputAxis(string name, string InputValue, bool active, bool raw)
		{
			this.active = active;
			this.raw = raw;
			this.input = InputValue;
			this.name = name;
			this.inputSystem = new DefaultInput();
		}

		// Token: 0x04002828 RID: 10280
		public bool active = true;

		// Token: 0x04002829 RID: 10281
		public string name = "NewAxis";

		// Token: 0x0400282A RID: 10282
		public bool raw = true;

		// Token: 0x0400282B RID: 10283
		public string input = "Value";

		// Token: 0x0400282C RID: 10284
		private IInputSystem inputSystem = new DefaultInput();

		// Token: 0x0400282D RID: 10285
		public FloatEvent OnAxisValueChanged = new FloatEvent();

		// Token: 0x0400282E RID: 10286
		private float currentAxisValue;
	}
}
