using System;
using System.Collections.Generic;
using MalbersAnimations.Events;
using UnityEngine;
using UnityEngine.Events;

namespace MalbersAnimations
{
	// Token: 0x020003E9 RID: 1001
	public class MalbersInput : MonoBehaviour, IInputSource
	{
		// Token: 0x17000346 RID: 838
		// (get) Token: 0x060037BD RID: 14269 RVA: 0x001B99C9 File Offset: 0x001B7BC9
		// (set) Token: 0x060037BC RID: 14268 RVA: 0x001B99C0 File Offset: 0x001B7BC0
		public bool MoveCharacter { get; set; }

		// Token: 0x17000347 RID: 839
		// (get) Token: 0x060037BE RID: 14270 RVA: 0x001B99D1 File Offset: 0x001B7BD1
		// (set) Token: 0x060037BF RID: 14271 RVA: 0x001B99D9 File Offset: 0x001B7BD9
		public bool CameraBaseInput
		{
			get
			{
				return this.cameraBaseInput;
			}
			set
			{
				this.cameraBaseInput = value;
			}
		}

		// Token: 0x17000348 RID: 840
		// (get) Token: 0x060037C0 RID: 14272 RVA: 0x001B99E2 File Offset: 0x001B7BE2
		// (set) Token: 0x060037C1 RID: 14273 RVA: 0x001B99EA File Offset: 0x001B7BEA
		public bool AlwaysForward
		{
			get
			{
				return this.alwaysForward;
			}
			set
			{
				this.alwaysForward = value;
			}
		}

		// Token: 0x060037C2 RID: 14274 RVA: 0x001B99F4 File Offset: 0x001B7BF4
		private void Awake()
		{
			this.Input_System = DefaultInput.GetInputSystem(this.PlayerID);
			this.Horizontal.InputSystem = (this.Vertical.InputSystem = this.Input_System);
			foreach (InputRow inputRow in this.inputs)
			{
				inputRow.InputSystem = this.Input_System;
			}
			this.List_to_Dictionary();
			this.InitializeCharacter();
			this.MoveCharacter = true;
		}

		// Token: 0x060037C3 RID: 14275 RVA: 0x001B9A90 File Offset: 0x001B7C90
		private void InitializeCharacter()
		{
			this.mCharacter = base.GetComponent<IMCharacter>();
			this.mCharacterMove = base.GetComponent<ICharacterMove>();
			if (this.mCharacter != null)
			{
				Dictionary<string, BoolEvent> dictionary = new Dictionary<string, BoolEvent>();
				foreach (KeyValuePair<string, InputRow> keyValuePair in this.DInputs)
				{
					dictionary.Add(keyValuePair.Key, keyValuePair.Value.OnInputChanged);
				}
				this.mCharacter.InitializeInputs(dictionary);
			}
		}

		// Token: 0x060037C4 RID: 14276 RVA: 0x001B9B28 File Offset: 0x001B7D28
		private void OnEnable()
		{
			this.OnInputEnabled.Invoke();
		}

		// Token: 0x060037C5 RID: 14277 RVA: 0x001B9B35 File Offset: 0x001B7D35
		public virtual void EnableMovement(bool value)
		{
			this.MoveCharacter = value;
		}

		// Token: 0x060037C6 RID: 14278 RVA: 0x001B9B3E File Offset: 0x001B7D3E
		private void OnDisable()
		{
			if (this.mCharacterMove != null)
			{
				this.mCharacterMove.Move(Vector3.zero, true);
			}
			this.OnInputDisabled.Invoke();
		}

		// Token: 0x060037C7 RID: 14279 RVA: 0x001B9B64 File Offset: 0x001B7D64
		private void Start()
		{
			if (Camera.main != null)
			{
				this.m_Cam = Camera.main.transform;
				return;
			}
			this.m_Cam = Object.FindObjectOfType<Camera>().transform;
		}

		// Token: 0x060037C8 RID: 14280 RVA: 0x001B9B94 File Offset: 0x001B7D94
		private void Update()
		{
			this.SetInput();
		}

		// Token: 0x060037C9 RID: 14281 RVA: 0x001B9B9C File Offset: 0x001B7D9C
		protected virtual void SetInput()
		{
			this.h = this.Horizontal.GetAxis;
			this.v = (this.alwaysForward ? 1f : this.Vertical.GetAxis);
			this.CharacterMove();
			foreach (InputRow inputRow in this.inputs)
			{
				bool getInput = inputRow.GetInput;
			}
		}

		// Token: 0x060037CA RID: 14282 RVA: 0x001B9C24 File Offset: 0x001B7E24
		private void CharacterMove()
		{
			if (this.MoveCharacter && this.mCharacterMove != null)
			{
				if (this.cameraBaseInput)
				{
					this.mCharacterMove.Move(this.CameraInputBased(), true);
					return;
				}
				this.mCharacterMove.Move(new Vector3(this.h, 0f, this.v), false);
			}
		}

		// Token: 0x060037CB RID: 14283 RVA: 0x001B9C80 File Offset: 0x001B7E80
		protected Vector3 CameraInputBased()
		{
			if (this.m_Cam != null)
			{
				this.m_CamForward = Vector3.Scale(this.m_Cam.forward, Vector3.one).normalized;
				this.m_Move = this.v * this.m_CamForward + this.h * this.m_Cam.right;
			}
			else
			{
				this.m_Move = this.v * Vector3.forward + this.h * Vector3.right;
			}
			return this.m_Move;
		}

		// Token: 0x060037CC RID: 14284 RVA: 0x001B9D24 File Offset: 0x001B7F24
		public virtual void EnableInput(string inputName, bool value)
		{
			InputRow inputRow = this.inputs.Find((InputRow item) => item.name == inputName);
			if (inputRow != null)
			{
				inputRow.active = value;
			}
		}

		// Token: 0x060037CD RID: 14285 RVA: 0x001B9D60 File Offset: 0x001B7F60
		public virtual void EnableInput(string inputName)
		{
			InputRow inputRow = this.inputs.Find((InputRow item) => item.name == inputName);
			if (inputRow != null)
			{
				inputRow.active = true;
			}
		}

		// Token: 0x060037CE RID: 14286 RVA: 0x001B9D9C File Offset: 0x001B7F9C
		public virtual void DisableInput(string inputName)
		{
			InputRow inputRow = this.inputs.Find((InputRow item) => item.name == inputName);
			if (inputRow != null)
			{
				inputRow.active = false;
			}
		}

		// Token: 0x060037CF RID: 14287 RVA: 0x001B9DD8 File Offset: 0x001B7FD8
		public virtual bool IsActive(string name)
		{
			InputRow inputRow;
			return this.DInputs.TryGetValue(name, out inputRow) && inputRow.active;
		}

		// Token: 0x060037D0 RID: 14288 RVA: 0x001B9E00 File Offset: 0x001B8000
		public virtual InputRow FindInput(string name)
		{
			InputRow inputRow = this.inputs.Find((InputRow item) => item.name.ToUpperInvariant() == name.ToUpperInvariant());
			if (inputRow != null)
			{
				return inputRow;
			}
			return null;
		}

		// Token: 0x060037D1 RID: 14289 RVA: 0x001B9E38 File Offset: 0x001B8038
		private void Reset()
		{
			this.inputs = new List<InputRow>
			{
				new InputRow("Jump", "Jump", KeyCode.Space, InputButton.Press, InputType.Input),
				new InputRow("Shift", "Fire3", KeyCode.LeftShift, InputButton.Press, InputType.Input),
				new InputRow("Attack1", "Fire1", KeyCode.Mouse0, InputButton.Press, InputType.Input),
				new InputRow("Attack2", "Fire2", KeyCode.Mouse1, InputButton.Press, InputType.Input),
				new InputRow(false, "SpeedDown", "SpeedDown", KeyCode.Alpha1, InputButton.Down, InputType.Key),
				new InputRow(false, "SpeedUp", "SpeedUp", KeyCode.Alpha2, InputButton.Down, InputType.Key),
				new InputRow("Speed1", "Speed1", KeyCode.Alpha1, InputButton.Down, InputType.Key),
				new InputRow("Speed2", "Speed2", KeyCode.Alpha2, InputButton.Down, InputType.Key),
				new InputRow("Speed3", "Speed3", KeyCode.Alpha3, InputButton.Down, InputType.Key),
				new InputRow("Action", "Action", KeyCode.E, InputButton.Down, InputType.Key),
				new InputRow("Fly", "Fly", KeyCode.Q, InputButton.Down, InputType.Key),
				new InputRow("Dodge", "Dodge", KeyCode.R, InputButton.Down, InputType.Key),
				new InputRow("Down", "Down", KeyCode.C, InputButton.Press, InputType.Key),
				new InputRow("Up", "Jump", KeyCode.Space, InputButton.Press, InputType.Input),
				new InputRow("Stun", "Stun", KeyCode.H, InputButton.Press, InputType.Key),
				new InputRow("Damaged", "Damaged", KeyCode.J, InputButton.Down, InputType.Key),
				new InputRow("Death", "Death", KeyCode.K, InputButton.Down, InputType.Key)
			};
		}

		// Token: 0x060037D2 RID: 14290 RVA: 0x001BA004 File Offset: 0x001B8204
		private void List_to_Dictionary()
		{
			this.DInputs = new Dictionary<string, InputRow>();
			foreach (InputRow inputRow in this.inputs)
			{
				this.DInputs.Add(inputRow.name, inputRow);
			}
		}

		// Token: 0x04002800 RID: 10240
		private IMCharacter mCharacter;

		// Token: 0x04002801 RID: 10241
		private ICharacterMove mCharacterMove;

		// Token: 0x04002802 RID: 10242
		private IInputSystem Input_System;

		// Token: 0x04002803 RID: 10243
		private Vector3 m_CamForward;

		// Token: 0x04002804 RID: 10244
		private Vector3 m_Move;

		// Token: 0x04002805 RID: 10245
		private Transform m_Cam;

		// Token: 0x04002806 RID: 10246
		public List<InputRow> inputs = new List<InputRow>();

		// Token: 0x04002807 RID: 10247
		protected Dictionary<string, InputRow> DInputs = new Dictionary<string, InputRow>();

		// Token: 0x04002808 RID: 10248
		public InputAxis Horizontal = new InputAxis("Horizontal", true, true);

		// Token: 0x04002809 RID: 10249
		public InputAxis Vertical = new InputAxis("Vertical", true, true);

		// Token: 0x0400280B RID: 10251
		[SerializeField]
		private bool cameraBaseInput;

		// Token: 0x0400280C RID: 10252
		[SerializeField]
		private bool alwaysForward;

		// Token: 0x0400280D RID: 10253
		public bool showInputEvents;

		// Token: 0x0400280E RID: 10254
		public UnityEvent OnInputEnabled = new UnityEvent();

		// Token: 0x0400280F RID: 10255
		public UnityEvent OnInputDisabled = new UnityEvent();

		// Token: 0x04002810 RID: 10256
		private float h;

		// Token: 0x04002811 RID: 10257
		private float v;

		// Token: 0x04002812 RID: 10258
		public string PlayerID = "Player0";
	}
}
