using System;
using UnityEngine;

namespace CrazyMinnow.SALSA.Examples
{
	// Token: 0x02000490 RID: 1168
	public class CM_DialogSystem : MonoBehaviour
	{
		// Token: 0x06003DDD RID: 15837 RVA: 0x001D959C File Offset: 0x001D779C
		private void Start()
		{
			this.salsaTypObj = this.GetSalsaType(this.npcDialog[this.npcDialogIndexTracker].npc);
			if (this.salsaTypObj.salsaType == CM_SalsaTypeAndObject.SalsaTypeOf.Salsa3D)
			{
				this.salsa3D = this.salsaTypObj.salsaGameObject.GetComponent<Salsa3D>();
				this.salsa3D.SetAudioClip(this.npcDialog[this.npcDialogIndexTracker].npcAudio);
				this.salsa3D.Play();
			}
			if (this.salsaTypObj.salsaType == CM_SalsaTypeAndObject.SalsaTypeOf.Salsa2D)
			{
				this.salsa2D = this.salsaTypObj.salsaGameObject.GetComponent<Salsa2D>();
				this.salsa2D.SetAudioClip(this.npcDialog[this.npcDialogIndexTracker].npcAudio);
				this.salsa2D.Play();
			}
		}

		// Token: 0x06003DDE RID: 15838 RVA: 0x001D9660 File Offset: 0x001D7860
		private void Salsa_OnTalkStatusChanged(SalsaStatus status)
		{
			if (!status.isTalking && status.talkerName == this.npcDialog[this.npcDialogIndexTracker].npc.name)
			{
				if (this.npcDialog[this.npcDialogIndexTracker].endDialog)
				{
					this.EndDialog();
				}
				if (!this.endDialogNpc)
				{
					if (this.npcDialog[this.npcDialogIndexTracker].playerResponse.Length == 0)
					{
						if (this.npcDialogIndexTracker < this.npcDialog.Length - 1)
						{
							this.npcDialogIndexTracker++;
							this.showNPCDialog = true;
							this.Start();
						}
					}
					else
					{
						this.showPlayerResponses = true;
					}
				}
			}
			if (!status.isTalking && status.talkerName != this.npcDialog[this.npcDialogIndexTracker].npc.name && (!this.endDialogNpc || !this.endDialogPlayer))
			{
				this.showNPCDialog = true;
				this.Start();
			}
		}

		// Token: 0x06003DDF RID: 15839 RVA: 0x001D9754 File Offset: 0x001D7954
		private void OnGUI()
		{
			int num = 20;
			int num2 = 40;
			if ((!this.endDialogNpc || !this.endDialogPlayer) && this.showNPCDialog && !this.endDialogPlayer)
			{
				GUI.Label(new Rect(20f, (float)num, 300f, 35f), this.npcDialog[this.npcDialogIndexTracker].npcText);
			}
			if (this.showPlayerResponses)
			{
				int num3 = num;
				for (int i = 0; i < this.npcDialog[this.npcDialogIndexTracker].playerResponse.Length; i++)
				{
					if (GUI.Button(new Rect((float)(Screen.width - 320), (float)num3, 300f, 35f), this.npcDialog[this.npcDialogIndexTracker].playerResponse[i].playerText))
					{
						this.salsaTypObj = this.GetSalsaType(this.npcDialog[this.npcDialogIndexTracker].playerResponse[i].player);
						if (this.salsaTypObj.salsaType == CM_SalsaTypeAndObject.SalsaTypeOf.Salsa3D)
						{
							this.salsa3D = this.salsaTypObj.salsaGameObject.GetComponent<Salsa3D>();
							this.salsa3D.SetAudioClip(this.npcDialog[this.npcDialogIndexTracker].playerResponse[i].playerAudio);
							this.salsa3D.Play();
						}
						if (this.salsaTypObj.salsaType == CM_SalsaTypeAndObject.SalsaTypeOf.Salsa2D)
						{
							this.salsa2D = this.salsaTypObj.salsaGameObject.GetComponent<Salsa2D>();
							this.salsa2D.SetAudioClip(this.npcDialog[this.npcDialogIndexTracker].playerResponse[i].playerAudio);
							this.salsa2D.Play();
						}
						this.endDialogPlayer = this.npcDialog[this.npcDialogIndexTracker].playerResponse[i].endDialog;
						this.npcDialogIndexTracker = this.npcDialog[this.npcDialogIndexTracker].playerResponse[i].npcDialogIndex;
						this.showNPCDialog = false;
						this.showPlayerResponses = false;
					}
					num3 += num2;
				}
			}
		}

		// Token: 0x06003DE0 RID: 15840 RVA: 0x001D9944 File Offset: 0x001D7B44
		private CM_SalsaTypeAndObject GetSalsaType(GameObject character)
		{
			CM_SalsaTypeAndObject cm_SalsaTypeAndObject = new CM_SalsaTypeAndObject();
			if (character.GetComponent<Salsa2D>() != null)
			{
				cm_SalsaTypeAndObject.salsaGameObject = character.GetComponent<Salsa2D>().gameObject;
				cm_SalsaTypeAndObject.salsaType = CM_SalsaTypeAndObject.SalsaTypeOf.Salsa2D;
			}
			else if (character.GetComponent<Salsa3D>() != null)
			{
				cm_SalsaTypeAndObject.salsaGameObject = character.GetComponent<Salsa3D>().gameObject;
				cm_SalsaTypeAndObject.salsaType = CM_SalsaTypeAndObject.SalsaTypeOf.Salsa3D;
			}
			return cm_SalsaTypeAndObject;
		}

		// Token: 0x06003DE1 RID: 15841 RVA: 0x001D99A6 File Offset: 0x001D7BA6
		private void EndDialog()
		{
			this.endDialogNpc = true;
			this.endDialogPlayer = true;
			this.showNPCDialog = false;
			this.showPlayerResponses = false;
		}

		// Token: 0x06003DE2 RID: 15842 RVA: 0x001D99C4 File Offset: 0x001D7BC4
		public void ResetDialog()
		{
			this.npcDialogIndexTracker = 0;
			this.endDialogNpc = false;
			this.endDialogPlayer = false;
			this.showNPCDialog = true;
			this.showPlayerResponses = false;
			this.salsaTypObj = this.GetSalsaType(this.npcDialog[this.npcDialogIndexTracker].npc);
			if (this.salsaTypObj.salsaType == CM_SalsaTypeAndObject.SalsaTypeOf.Salsa3D)
			{
				this.salsa3D = this.salsaTypObj.salsaGameObject.GetComponent<Salsa3D>();
				this.salsa3D.Stop();
			}
			if (this.salsaTypObj.salsaType == CM_SalsaTypeAndObject.SalsaTypeOf.Salsa2D)
			{
				this.salsa2D = this.salsaTypObj.salsaGameObject.GetComponent<Salsa2D>();
				this.salsa2D.Stop();
			}
			if (this.npcDialog[this.npcDialogIndexTracker].playerResponse.Length != 0)
			{
				this.salsaTypObj = this.GetSalsaType(this.npcDialog[this.npcDialogIndexTracker].playerResponse[0].player);
				if (this.salsaTypObj.salsaType == CM_SalsaTypeAndObject.SalsaTypeOf.Salsa3D)
				{
					this.salsa3D = this.salsaTypObj.salsaGameObject.GetComponent<Salsa3D>();
					this.salsa3D.Stop();
				}
				if (this.salsaTypObj.salsaType == CM_SalsaTypeAndObject.SalsaTypeOf.Salsa2D)
				{
					this.salsa2D = this.salsaTypObj.salsaGameObject.GetComponent<Salsa2D>();
					this.salsa2D.Stop();
				}
			}
			this.Start();
			this.showNPCDialog = true;
		}

		// Token: 0x04002BD3 RID: 11219
		public CM_NPCDialog[] npcDialog;

		// Token: 0x04002BD4 RID: 11220
		private Salsa2D salsa2D;

		// Token: 0x04002BD5 RID: 11221
		private Salsa3D salsa3D;

		// Token: 0x04002BD6 RID: 11222
		private int npcDialogIndexTracker;

		// Token: 0x04002BD7 RID: 11223
		private bool showNPCDialog = true;

		// Token: 0x04002BD8 RID: 11224
		private bool showPlayerResponses;

		// Token: 0x04002BD9 RID: 11225
		private bool endDialogPlayer;

		// Token: 0x04002BDA RID: 11226
		private bool endDialogNpc;

		// Token: 0x04002BDB RID: 11227
		private CM_SalsaTypeAndObject salsaTypObj;
	}
}
