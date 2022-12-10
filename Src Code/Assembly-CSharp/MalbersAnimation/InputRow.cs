using System;
using MalbersAnimations.Events;
using UnityEngine;
using UnityEngine.Events;

namespace MalbersAnimations
{
	// Token: 0x020003EA RID: 1002
	[Serializable]
	public class InputRow
	{
		// Token: 0x17000349 RID: 841
		// (get) Token: 0x060037D4 RID: 14292 RVA: 0x001BA0E0 File Offset: 0x001B82E0
		public virtual bool GetInput
		{
			get
			{
				if (!this.active)
				{
					return false;
				}
				if (this.inputSystem == null)
				{
					return false;
				}
				bool inputValue = this.InputValue;
				switch (this.GetPressed)
				{
				case InputButton.Press:
					this.InputValue = ((this.type == InputType.Input) ? this.InputSystem.GetButton(this.input) : Input.GetKey(this.key));
					if (inputValue != this.InputValue)
					{
						if (this.InputValue)
						{
							this.OnInputDown.Invoke();
						}
						else
						{
							this.OnInputUp.Invoke();
						}
						this.OnInputChanged.Invoke(this.InputValue);
					}
					if (this.InputValue)
					{
						this.OnInputPressed.Invoke();
					}
					return this.InputValue;
				case InputButton.Down:
					this.InputValue = ((this.type == InputType.Input) ? this.InputSystem.GetButtonDown(this.input) : Input.GetKeyDown(this.key));
					if (inputValue != this.InputValue)
					{
						if (this.InputValue)
						{
							this.OnInputDown.Invoke();
						}
						this.OnInputChanged.Invoke(this.InputValue);
					}
					return this.InputValue;
				case InputButton.Up:
					this.InputValue = ((this.type == InputType.Input) ? this.InputSystem.GetButtonUp(this.input) : Input.GetKeyUp(this.key));
					if (inputValue != this.InputValue)
					{
						if (this.InputValue)
						{
							this.OnInputUp.Invoke();
						}
						this.OnInputChanged.Invoke(this.InputValue);
					}
					return this.InputValue;
				case InputButton.LongPress:
					this.InputValue = ((this.type == InputType.Input) ? this.InputSystem.GetButton(this.input) : Input.GetKey(this.key));
					if (this.InputValue)
					{
						if (!this.InputCompleted)
						{
							if (!this.FirstInputPress)
							{
								this.InputCurrentTime = Time.time;
								this.FirstInputPress = true;
								this.OnInputDown.Invoke();
							}
							else
							{
								if (Time.time - this.InputCurrentTime >= this.LongPressTime)
								{
									this.OnLongPress.Invoke();
									this.OnPressedNormalized.Invoke(1f);
									this.InputCompleted = true;
									return this.InputValue = true;
								}
								this.OnPressedNormalized.Invoke((Time.time - this.InputCurrentTime) / this.LongPressTime);
							}
						}
					}
					else
					{
						if (!this.InputCompleted && this.FirstInputPress)
						{
							this.OnInputUp.Invoke();
						}
						this.FirstInputPress = (this.InputCompleted = false);
					}
					return this.InputValue = false;
				case InputButton.DoubleTap:
					this.InputValue = ((this.type == InputType.Input) ? this.InputSystem.GetButtonDown(this.input) : Input.GetKeyDown(this.key));
					if (this.InputValue)
					{
						if (this.InputCurrentTime != 0f && Time.time - this.InputCurrentTime > this.DoubleTapTime)
						{
							this.FirstInputPress = false;
						}
						if (!this.FirstInputPress)
						{
							this.OnInputDown.Invoke();
							this.InputCurrentTime = Time.time;
							this.FirstInputPress = true;
						}
						else
						{
							if (Time.time - this.InputCurrentTime <= this.DoubleTapTime)
							{
								this.FirstInputPress = false;
								this.InputCurrentTime = 0f;
								this.OnDoubleTap.Invoke();
								return this.InputValue = true;
							}
							this.FirstInputPress = false;
						}
					}
					return this.InputValue = false;
				default:
					return false;
				}
			}
		}

		// Token: 0x1700034A RID: 842
		// (get) Token: 0x060037D5 RID: 14293 RVA: 0x001BA446 File Offset: 0x001B8646
		// (set) Token: 0x060037D6 RID: 14294 RVA: 0x001BA44E File Offset: 0x001B864E
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

		// Token: 0x060037D7 RID: 14295 RVA: 0x001BA458 File Offset: 0x001B8658
		public InputRow(KeyCode k)
		{
			this.active = true;
			this.type = InputType.Key;
			this.key = k;
			this.GetPressed = InputButton.Down;
			this.inputSystem = new DefaultInput();
		}

		// Token: 0x060037D8 RID: 14296 RVA: 0x001BA528 File Offset: 0x001B8728
		public InputRow(string input, KeyCode key)
		{
			this.active = true;
			this.type = InputType.Key;
			this.key = key;
			this.input = input;
			this.GetPressed = InputButton.Down;
			this.inputSystem = new DefaultInput();
		}

		// Token: 0x060037D9 RID: 14297 RVA: 0x001BA5FC File Offset: 0x001B87FC
		public InputRow(string unityInput, KeyCode k, InputButton pressed)
		{
			this.active = true;
			this.type = InputType.Key;
			this.key = k;
			this.input = unityInput;
			this.GetPressed = InputButton.Down;
			this.inputSystem = new DefaultInput();
		}

		// Token: 0x060037DA RID: 14298 RVA: 0x001BA6D0 File Offset: 0x001B88D0
		public InputRow(string name, string unityInput, KeyCode k, InputButton pressed, InputType itype)
		{
			this.name = name;
			this.active = true;
			this.type = itype;
			this.key = k;
			this.input = unityInput;
			this.GetPressed = pressed;
			this.inputSystem = new DefaultInput();
		}

		// Token: 0x060037DB RID: 14299 RVA: 0x001BA7B0 File Offset: 0x001B89B0
		public InputRow(bool active, string name, string unityInput, KeyCode k, InputButton pressed, InputType itype)
		{
			this.name = name;
			this.active = active;
			this.type = itype;
			this.key = k;
			this.input = unityInput;
			this.GetPressed = pressed;
			this.inputSystem = new DefaultInput();
		}

		// Token: 0x060037DC RID: 14300 RVA: 0x001BA890 File Offset: 0x001B8A90
		public InputRow()
		{
			this.active = true;
			this.name = "InputName";
			this.type = InputType.Input;
			this.input = "Value";
			this.key = KeyCode.A;
			this.GetPressed = InputButton.Press;
			this.inputSystem = new DefaultInput();
		}

		// Token: 0x04002813 RID: 10259
		public bool active = true;

		// Token: 0x04002814 RID: 10260
		public string name = "InputName";

		// Token: 0x04002815 RID: 10261
		public InputType type;

		// Token: 0x04002816 RID: 10262
		public string input = "Value";

		// Token: 0x04002817 RID: 10263
		public KeyCode key = KeyCode.A;

		// Token: 0x04002818 RID: 10264
		public InputButton GetPressed;

		// Token: 0x04002819 RID: 10265
		public bool InputValue;

		// Token: 0x0400281A RID: 10266
		public UnityEvent OnInputDown = new UnityEvent();

		// Token: 0x0400281B RID: 10267
		public UnityEvent OnInputUp = new UnityEvent();

		// Token: 0x0400281C RID: 10268
		public UnityEvent OnLongPress = new UnityEvent();

		// Token: 0x0400281D RID: 10269
		public UnityEvent OnDoubleTap = new UnityEvent();

		// Token: 0x0400281E RID: 10270
		public BoolEvent OnInputChanged = new BoolEvent();

		// Token: 0x0400281F RID: 10271
		protected IInputSystem inputSystem = new DefaultInput();

		// Token: 0x04002820 RID: 10272
		public bool ShowEvents;

		// Token: 0x04002821 RID: 10273
		public float DoubleTapTime = 0.3f;

		// Token: 0x04002822 RID: 10274
		public float LongPressTime = 0.5f;

		// Token: 0x04002823 RID: 10275
		private bool FirstInputPress;

		// Token: 0x04002824 RID: 10276
		private bool InputCompleted;

		// Token: 0x04002825 RID: 10277
		private float InputCurrentTime;

		// Token: 0x04002826 RID: 10278
		public UnityEvent OnInputPressed = new UnityEvent();

		// Token: 0x04002827 RID: 10279
		public FloatEvent OnPressedNormalized = new FloatEvent();
	}
}
