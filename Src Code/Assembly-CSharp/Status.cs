using System;
using Logic;

// Token: 0x02000180 RID: 384
public class Status : IListener
{
	// Token: 0x06001513 RID: 5395 RVA: 0x000D56E0 File Offset: 0x000D38E0
	public static void CreateVisuals(Object logic_obj)
	{
		Logic.Status status = logic_obj as Logic.Status;
		if (status == null)
		{
			return;
		}
		new global::Status().Init(status);
	}

	// Token: 0x06001514 RID: 5396 RVA: 0x000D5703 File Offset: 0x000D3903
	public void Init(Logic.Status logic)
	{
		this.logic = logic;
		logic.visuals = this;
	}

	// Token: 0x06001515 RID: 5397 RVA: 0x000D5714 File Offset: 0x000D3914
	public void OnButton(Logic.Status status, string id)
	{
		Action buttonAction = status.GetButtonAction(id);
		if (buttonAction == null)
		{
			status.OnButton(id);
			return;
		}
		if (!buttonAction.CheckCost(null))
		{
			return;
		}
		ActionVisuals actionVisuals = buttonAction.visuals as ActionVisuals;
		if (actionVisuals == null)
		{
			buttonAction.Execute(null);
			return;
		}
		if (buttonAction.Validate(true) != "ok")
		{
			return;
		}
		actionVisuals.Begin();
	}

	// Token: 0x06001516 RID: 5398 RVA: 0x000D5770 File Offset: 0x000D3970
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "leave_state")
		{
			return;
		}
		message == "enter_state";
	}

	// Token: 0x04000D81 RID: 3457
	public Logic.Status logic;
}
