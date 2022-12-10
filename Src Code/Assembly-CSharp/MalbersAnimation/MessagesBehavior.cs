using System;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x020003DB RID: 987
	public class MessagesBehavior : StateMachineBehaviour
	{
		// Token: 0x0600376E RID: 14190 RVA: 0x001B7200 File Offset: 0x001B5400
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this.listeners = animator.GetComponents<IAnimatorListener>();
			MesssageItem[] array = this.onTimeMessage;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].sent = false;
			}
			foreach (MesssageItem messsageItem in this.onEnterMessage)
			{
				if (messsageItem.Active && messsageItem.message != string.Empty)
				{
					if (this.UseSendMessage)
					{
						this.DeliverMessage(messsageItem, animator);
					}
					else
					{
						foreach (IAnimatorListener listener in this.listeners)
						{
							this.DeliverListener(messsageItem, listener);
						}
					}
				}
			}
		}

		// Token: 0x0600376F RID: 14191 RVA: 0x001B72A4 File Offset: 0x001B54A4
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			foreach (MesssageItem messsageItem in this.onExitMessage)
			{
				if (messsageItem.Active && messsageItem.message != string.Empty)
				{
					if (this.UseSendMessage)
					{
						this.DeliverMessage(messsageItem, animator);
					}
					else
					{
						foreach (IAnimatorListener listener in this.listeners)
						{
							this.DeliverListener(messsageItem, listener);
						}
					}
				}
			}
		}

		// Token: 0x06003770 RID: 14192 RVA: 0x001B7320 File Offset: 0x001B5520
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			foreach (MesssageItem messsageItem in this.onTimeMessage)
			{
				if (messsageItem.Active && messsageItem.message != string.Empty && !messsageItem.sent && stateInfo.normalizedTime % 1f >= messsageItem.time)
				{
					messsageItem.sent = true;
					if (this.UseSendMessage)
					{
						this.DeliverMessage(messsageItem, animator);
					}
					else
					{
						foreach (IAnimatorListener listener in this.listeners)
						{
							this.DeliverListener(messsageItem, listener);
						}
					}
				}
			}
		}

		// Token: 0x06003771 RID: 14193 RVA: 0x001B73C4 File Offset: 0x001B55C4
		private void DeliverMessage(MesssageItem m, Animator anim)
		{
			switch (m.typeM)
			{
			case TypeMessage.Bool:
				anim.SendMessage(m.message, m.boolValue, SendMessageOptions.DontRequireReceiver);
				return;
			case TypeMessage.Int:
				anim.SendMessage(m.message, m.intValue, SendMessageOptions.DontRequireReceiver);
				return;
			case TypeMessage.Float:
				anim.SendMessage(m.message, m.floatValue, SendMessageOptions.DontRequireReceiver);
				return;
			case TypeMessage.String:
				anim.SendMessage(m.message, m.stringValue, SendMessageOptions.DontRequireReceiver);
				return;
			case TypeMessage.Void:
				anim.SendMessage(m.message, SendMessageOptions.DontRequireReceiver);
				return;
			case TypeMessage.IntVar:
				anim.SendMessage(m.message, m.intVarValue, SendMessageOptions.DontRequireReceiver);
				return;
			default:
				return;
			}
		}

		// Token: 0x06003772 RID: 14194 RVA: 0x001B7484 File Offset: 0x001B5684
		private void DeliverListener(MesssageItem m, IAnimatorListener listener)
		{
			switch (m.typeM)
			{
			case TypeMessage.Bool:
				listener.OnAnimatorBehaviourMessage(m.message, m.boolValue);
				return;
			case TypeMessage.Int:
				listener.OnAnimatorBehaviourMessage(m.message, m.intValue);
				return;
			case TypeMessage.Float:
				listener.OnAnimatorBehaviourMessage(m.message, m.floatValue);
				return;
			case TypeMessage.String:
				listener.OnAnimatorBehaviourMessage(m.message, m.stringValue);
				return;
			case TypeMessage.Void:
				listener.OnAnimatorBehaviourMessage(m.message, null);
				return;
			case TypeMessage.IntVar:
				listener.OnAnimatorBehaviourMessage(m.message, m.intVarValue);
				return;
			default:
				return;
			}
		}

		// Token: 0x0400276E RID: 10094
		public bool UseSendMessage;

		// Token: 0x0400276F RID: 10095
		public MesssageItem[] onEnterMessage;

		// Token: 0x04002770 RID: 10096
		public MesssageItem[] onExitMessage;

		// Token: 0x04002771 RID: 10097
		public MesssageItem[] onTimeMessage;

		// Token: 0x04002772 RID: 10098
		private IAnimatorListener[] listeners;
	}
}
