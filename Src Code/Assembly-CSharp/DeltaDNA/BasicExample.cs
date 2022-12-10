using System;
using System.Collections.Generic;
using DeltaDNA.MiniJSON;
using UnityEngine;
using UnityEngine.UI;

namespace DeltaDNA
{
	// Token: 0x0200047C RID: 1148
	public class BasicExample : MonoBehaviour
	{
		// Token: 0x06003BE1 RID: 15329 RVA: 0x001C8FAC File Offset: 0x001C71AC
		private void Start()
		{
			Singleton<DDNA>.Instance.SetLoggingLevel(Logger.Level.DEBUG);
			Singleton<DDNA>.Instance.IosNotifications.OnDidRegisterForPushNotifications += delegate(string n)
			{
				Debug.Log("Got an iOS push token: " + n);
			};
			Singleton<DDNA>.Instance.IosNotifications.OnDidReceivePushNotification += delegate(string n)
			{
				Debug.Log("Got an iOS push notification! " + n);
			};
			Singleton<DDNA>.Instance.IosNotifications.OnDidLaunchWithPushNotification += delegate(string n)
			{
				Debug.Log("Launched with an iOS push notification: " + n);
			};
			Singleton<DDNA>.Instance.IosNotifications.RegisterForPushNotifications();
			Singleton<DDNA>.Instance.AndroidNotifications.OnDidRegisterForPushNotifications += delegate(string n)
			{
				Debug.Log("Got an Android registration token: " + n);
			};
			Singleton<DDNA>.Instance.AndroidNotifications.OnDidFailToRegisterForPushNotifications += delegate(string n)
			{
				Debug.Log("Failed getting an Android registration token: " + n);
			};
			Singleton<DDNA>.Instance.AndroidNotifications.OnDidReceivePushNotification += delegate(string n)
			{
				Debug.Log("Got an Android push notification: " + n);
			};
			Singleton<DDNA>.Instance.AndroidNotifications.OnDidLaunchWithPushNotification += delegate(string n)
			{
				Debug.Log("Launched with an Android push notification: " + n);
			};
			Singleton<DDNA>.Instance.AndroidNotifications.RegisterForPushNotifications(false);
			Singleton<DDNA>.Instance.Settings.DefaultImageMessageHandler = new ImageMessageHandler(Singleton<DDNA>.Instance, delegate(ImageMessage imageMessage)
			{
				imageMessage.Show();
			});
			Singleton<DDNA>.Instance.Settings.DefaultGameParameterHandler = new GameParametersHandler(delegate(Dictionary<string, object> gameParameters)
			{
				Debug.Log("Received game parameters from event trigger: " + gameParameters);
			});
			Singleton<DDNA>.Instance.StartSDK(new Configuration
			{
				environmentKeyDev = "48380028118965502444250662515743",
				environmentKey = 0,
				collectUrl = "https://collect16056nwdmf.deltadna.net/collect/api",
				engageUrl = "https://engage16056nwdmf.deltadna.net",
				useApplicationVersion = true
			});
			Debug.LogWarning("DeltaDNA has started with a default configuration. To use your own config, edit the BasicExample script.");
		}

		// Token: 0x06003BE2 RID: 15330 RVA: 0x001C91D8 File Offset: 0x001C73D8
		private void FixedUpdate()
		{
			if (Singleton<DDNA>.Instance.HasStarted)
			{
				this.cubeObj.Rotate(new Vector3(15f, 30f, 45f) * Time.deltaTime);
			}
		}

		// Token: 0x06003BE3 RID: 15331 RVA: 0x001C9210 File Offset: 0x001C7410
		public void OnSimpleEventBtn_Clicked()
		{
			GameEvent gameEvent = new GameEvent("options").AddParam("option", "sword").AddParam("action", "sell");
			Singleton<DDNA>.Instance.RecordEvent<GameEvent>(gameEvent);
		}

		// Token: 0x06003BE4 RID: 15332 RVA: 0x001C9254 File Offset: 0x001C7454
		public void OnAchievementEventBtn_Clicked()
		{
			GameEvent gameEvent = new GameEvent("achievement").AddParam("achievementName", "Sunday Showdown Tournament Win").AddParam("achievementID", "SS-2014-03-02-01").AddParam("reward", new Params().AddParam("rewardName", "Medal").AddParam("rewardProducts", new Product().AddVirtualCurrency("VIP Points", "GRIND", 20L).AddItem("Sunday Showdown Medal", "Victory Badge", 1)));
			Singleton<DDNA>.Instance.RecordEvent<GameEvent>(gameEvent).Run();
		}

		// Token: 0x06003BE5 RID: 15333 RVA: 0x001C92EC File Offset: 0x001C74EC
		public void OnTransactionEventBtn_Clicked()
		{
			Transaction gameEvent = new Transaction("Weapon type 11 manual repair", "PURCHASE", new Product().AddItem("WeaponsMaxConditionRepair:11", "WeaponMaxConditionRepair", 5).AddVirtualCurrency("Credit", "PREMIUM", 710L), new Product().SetRealCurrency("USD", Product<Product>.ConvertCurrency("USD", 12.34m))).SetTransactorId("2.212.91.84:15116").SetProductId("4019").AddParam("paymentCountry", "GB");
			Singleton<DDNA>.Instance.RecordEvent<Transaction>(gameEvent);
		}

		// Token: 0x06003BE6 RID: 15334 RVA: 0x001C938C File Offset: 0x001C758C
		public void OnEngagementBtn_Clicked()
		{
			Params parameters = new Params().AddParam("userLevel", 4).AddParam("experience", 1000).AddParam("missionName", "Disco Volante");
			Singleton<DDNA>.Instance.EngageFactory.RequestGameParameters("gameLoaded", parameters, delegate(Dictionary<string, object> gameParameters)
			{
				this.popUpContent.text = Json.Serialize(gameParameters);
			});
			this.popUpTitle.text = "Engage returned";
			this.popUpObj.SetActive(true);
		}

		// Token: 0x06003BE7 RID: 15335 RVA: 0x001C9410 File Offset: 0x001C7610
		public void OnImageMessageBtn_Clicked()
		{
			Params parameters = new Params().AddParam("userLevel", 4).AddParam("experience", 1000).AddParam("missionName", "Disco Volante");
			Singleton<DDNA>.Instance.EngageFactory.RequestImageMessage("testImageMessage", parameters, delegate(ImageMessage imageMessage)
			{
				if (imageMessage != null)
				{
					Debug.Log("Engage returned a valid image message.");
					imageMessage.OnDidReceiveResources += delegate()
					{
						Debug.Log("Image Message loaded resources.");
						imageMessage.Show();
					};
					imageMessage.OnDismiss += delegate(ImageMessage.EventArgs obj)
					{
						Debug.Log("Image Message dismissed by " + obj.ID);
					};
					imageMessage.OnAction += delegate(ImageMessage.EventArgs obj)
					{
						Debug.Log("Image Message actioned by " + obj.ID + " with command " + obj.ActionValue);
					};
					imageMessage.FetchResources();
					return;
				}
				Debug.Log("Engage didn't return an image message.");
			});
		}

		// Token: 0x06003BE8 RID: 15336 RVA: 0x001C948A File Offset: 0x001C768A
		public void OnUploadEventsBtn_Clicked()
		{
			Singleton<DDNA>.Instance.Upload();
		}

		// Token: 0x06003BE9 RID: 15337 RVA: 0x001C9498 File Offset: 0x001C7698
		public void OnStartSDKBtn_Clicked()
		{
			Singleton<DDNA>.Instance.StartSDK(new Configuration
			{
				environmentKeyDev = "48380028118965502444250662515743",
				environmentKey = 0,
				collectUrl = "https://collect16056nwdmf.deltadna.net/collect/api",
				engageUrl = "https://engage16056nwdmf.deltadna.net",
				useApplicationVersion = true
			});
			Debug.LogWarning("DeltaDNA has started with a default configuration. To use your own config, edit the BasicExample script.");
		}

		// Token: 0x06003BEA RID: 15338 RVA: 0x001C94F0 File Offset: 0x001C76F0
		public void OnStartSDKNewUserBtn_Clicked()
		{
			Singleton<DDNA>.Instance.StartSDK(new Configuration
			{
				environmentKeyDev = "48380028118965502444250662515743",
				environmentKey = 0,
				collectUrl = "https://collect16056nwdmf.deltadna.net/collect/api",
				engageUrl = "https://engage16056nwdmf.deltadna.net",
				useApplicationVersion = true
			}, Guid.NewGuid().ToString());
			Debug.LogWarning("DeltaDNA has started with a default configuration. To use your own config, edit the BasicExample script.");
		}

		// Token: 0x06003BEB RID: 15339 RVA: 0x001C9558 File Offset: 0x001C7758
		public void OnStopSDKBtn_Clicked()
		{
			Singleton<DDNA>.Instance.StopSDK();
		}

		// Token: 0x06003BEC RID: 15340 RVA: 0x001C9564 File Offset: 0x001C7764
		public void OnNewSessionBtn_Clicked()
		{
			Singleton<DDNA>.Instance.NewSession();
		}

		// Token: 0x06003BED RID: 15341 RVA: 0x001C9570 File Offset: 0x001C7770
		public void OnForgetMeBtn_Clicked()
		{
			Singleton<DDNA>.Instance.ForgetMe();
		}

		// Token: 0x06003BEE RID: 15342 RVA: 0x001C957C File Offset: 0x001C777C
		public void OnClearPersistentDataBtn_Clicked()
		{
			Singleton<DDNA>.Instance.ClearPersistentData();
		}

		// Token: 0x04002B72 RID: 11122
		[SerializeField]
		private Transform cubeObj;

		// Token: 0x04002B73 RID: 11123
		[SerializeField]
		private GameObject popUpObj;

		// Token: 0x04002B74 RID: 11124
		[SerializeField]
		private Text popUpContent;

		// Token: 0x04002B75 RID: 11125
		[SerializeField]
		private Text popUpTitle;
	}
}
