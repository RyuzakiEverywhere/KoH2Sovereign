using System;
using UnityEngine;

namespace MalbersAnimations.Utilities
{
	// Token: 0x02000477 RID: 1143
	public class Messages : MonoBehaviour
	{
		// Token: 0x06003BCB RID: 15307 RVA: 0x001C8AB8 File Offset: 0x001C6CB8
		public virtual void SendMessage(Component component)
		{
			foreach (MesssageItem messsageItem in this.messages)
			{
				if (messsageItem.message == string.Empty || !messsageItem.Active)
				{
					break;
				}
				if (this.UseSendMessage)
				{
					this.DeliverMessage(messsageItem, component.transform.root);
				}
				else
				{
					IAnimatorListener componentInParent = component.GetComponentInParent<IAnimatorListener>();
					if (componentInParent != null)
					{
						this.DeliverListener(messsageItem, componentInParent);
					}
				}
			}
		}

		// Token: 0x06003BCC RID: 15308 RVA: 0x001C8B28 File Offset: 0x001C6D28
		private void DeliverMessage(MesssageItem m, Component component)
		{
			switch (m.typeM)
			{
			case TypeMessage.Bool:
				component.SendMessage(m.message, m.boolValue, SendMessageOptions.DontRequireReceiver);
				return;
			case TypeMessage.Int:
				component.SendMessage(m.message, m.intValue, SendMessageOptions.DontRequireReceiver);
				return;
			case TypeMessage.Float:
				component.SendMessage(m.message, m.floatValue, SendMessageOptions.DontRequireReceiver);
				return;
			case TypeMessage.String:
				component.SendMessage(m.message, m.stringValue, SendMessageOptions.DontRequireReceiver);
				return;
			case TypeMessage.Void:
				component.SendMessage(m.message, SendMessageOptions.DontRequireReceiver);
				return;
			case TypeMessage.IntVar:
				component.SendMessage(m.message, m.intVarValue, SendMessageOptions.DontRequireReceiver);
				return;
			default:
				return;
			}
		}

		// Token: 0x06003BCD RID: 15309 RVA: 0x001C8BE8 File Offset: 0x001C6DE8
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

		// Token: 0x04002B60 RID: 11104
		public MesssageItem[] messages;

		// Token: 0x04002B61 RID: 11105
		public bool UseSendMessage;

		// Token: 0x04002B62 RID: 11106
		public Component component;
	}
}
