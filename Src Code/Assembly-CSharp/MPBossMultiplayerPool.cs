using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x020002D0 RID: 720
public class MPBossMultiplayerPool : IListener
{
	// Token: 0x06002D8D RID: 11661 RVA: 0x00179E06 File Offset: 0x00178006
	public MPBossMultiplayerPool(MPBoss mpBoss)
	{
		this.virtualConnections = new List<MPBossMultiplayerPool.VirtualConnection>();
		if (mpBoss.meta_server == null)
		{
			MPBossMultiplayerPool.Error("Null server multiplayer!");
		}
		Logic.Multiplayer meta_server = mpBoss.meta_server;
		if (meta_server == null)
		{
			return;
		}
		meta_server.AddListener(this);
	}

	// Token: 0x06002D8E RID: 11662 RVA: 0x00179E3C File Offset: 0x0017803C
	public void CleanUp(bool quitting)
	{
		if (this.virtualConnections != null)
		{
			for (int i = 0; i < this.virtualConnections.Count; i++)
			{
				MPBossMultiplayerPool.VirtualConnection virtualConnection = this.virtualConnections[i];
				if (quitting)
				{
					Logic.Multiplayer client = virtualConnection.client;
					if (client != null)
					{
						client.OnQuit();
					}
					Logic.Multiplayer serverClient = virtualConnection.serverClient;
					if (serverClient != null)
					{
						serverClient.OnQuit();
					}
				}
				else
				{
					Logic.Multiplayer client2 = virtualConnection.client;
					if (client2 != null)
					{
						client2.ShutDown();
					}
					Logic.Multiplayer serverClient2 = virtualConnection.serverClient;
					if (serverClient2 != null)
					{
						serverClient2.ShutDown();
					}
				}
			}
			this.virtualConnections.Clear();
		}
		MPBoss mpboss = MPBoss.Get();
		if (mpboss == null)
		{
			return;
		}
		Logic.Multiplayer meta_server = mpboss.meta_server;
		if (meta_server == null)
		{
			return;
		}
		meta_server.DelListener(this);
	}

	// Token: 0x06002D8F RID: 11663 RVA: 0x00179EE4 File Offset: 0x001780E4
	public void OnUpdate()
	{
		int num = 0;
		for (;;)
		{
			int num2 = num;
			List<MPBossMultiplayerPool.VirtualConnection> list = this.virtualConnections;
			int? num3 = (list != null) ? new int?(list.Count) : null;
			if (!(num2 < num3.GetValueOrDefault() & num3 != null))
			{
				break;
			}
			Logic.Multiplayer client = this.virtualConnections[num].client;
			if (client != null)
			{
				client.OnUpdate();
			}
			num++;
		}
		MPBoss mpboss = MPBoss.Get();
		if (mpboss == null)
		{
			return;
		}
		Logic.Multiplayer meta_server = mpboss.meta_server;
		if (meta_server == null)
		{
			return;
		}
		meta_server.OnUpdate();
	}

	// Token: 0x06002D90 RID: 11664 RVA: 0x00179F64 File Offset: 0x00178164
	public void Add(MPBossMultiplayerPool.VirtualConnection virtualConnection)
	{
		if (virtualConnection == null)
		{
			MPBossMultiplayerPool.Error("Attempting to add null virtual connection");
			return;
		}
		if (string.IsNullOrEmpty(virtualConnection.player_handle))
		{
			MPBossMultiplayerPool.Error("Attempting to add a virtual connection with empty player handle");
			return;
		}
		if (this.ContainsVirtualConnection(virtualConnection.player_handle))
		{
			MPBossMultiplayerPool.Error("Attempting to add a virtual connection with player handle " + virtualConnection.player_handle + " that is already present in the pool");
			return;
		}
		this.virtualConnections.Add(virtualConnection);
	}

	// Token: 0x06002D91 RID: 11665 RVA: 0x00179FCC File Offset: 0x001781CC
	public void Remove(MPBossMultiplayerPool.VirtualConnection virtualConnection)
	{
		if (virtualConnection == null)
		{
			MPBossMultiplayerPool.Error("Attempting to remove null virtual connection");
			return;
		}
		this.virtualConnections.Remove(virtualConnection);
	}

	// Token: 0x06002D92 RID: 11666 RVA: 0x00179FEC File Offset: 0x001781EC
	public Logic.Multiplayer GetMultiplayer(string player_id, Logic.Multiplayer.ConnectionReason connection_reason)
	{
		if (string.IsNullOrEmpty(player_id))
		{
			MPBossMultiplayerPool.Error("GetMultiplayer() called with empty player id");
			return null;
		}
		if (connection_reason == Logic.Multiplayer.ConnectionReason.Invalid)
		{
			MPBossMultiplayerPool.Error("GetMultiplayer() called with invalid connection reason");
			return null;
		}
		string playerHandle = Logic.Multiplayer.GetPlayerHandle(player_id, connection_reason);
		if (string.IsNullOrEmpty(playerHandle))
		{
			MPBossMultiplayerPool.Error(string.Format("GetMultiplayer() couldn't extract player handle for player id {0} and connection reason {1}", player_id, connection_reason));
			return null;
		}
		foreach (MPBossMultiplayerPool.VirtualConnection virtualConnection in this.virtualConnections)
		{
			if (virtualConnection != null && virtualConnection.player_handle == playerHandle)
			{
				if (virtualConnection.client != null && virtualConnection.client.IsConnected())
				{
					return virtualConnection.client;
				}
				if (virtualConnection.serverClient != null && virtualConnection.serverClient.IsConnected())
				{
					return virtualConnection.serverClient;
				}
				return null;
			}
		}
		return null;
	}

	// Token: 0x06002D93 RID: 11667 RVA: 0x0017A0D8 File Offset: 0x001782D8
	public Logic.Multiplayer GetMultiplayer(string player_id, Logic.Multiplayer.ConnectionReason connection_reason, Logic.Multiplayer.Type type)
	{
		if (type == Logic.Multiplayer.Type.Server)
		{
			MPBossMultiplayerPool.Error(string.Format("GetMPOfType() called with type {0}. Cannot get multiplayer of type {1}", type, type));
			return null;
		}
		if (string.IsNullOrEmpty(player_id))
		{
			MPBossMultiplayerPool.Error("GetMultiplayer() called with empty player id");
		}
		if (connection_reason == Logic.Multiplayer.ConnectionReason.Invalid)
		{
			MPBossMultiplayerPool.Error("GetMultiplayer() called with invalid connection reason");
		}
		string playerHandle = Logic.Multiplayer.GetPlayerHandle(player_id, connection_reason);
		if (string.IsNullOrEmpty(playerHandle))
		{
			MPBossMultiplayerPool.Error(string.Format("GetMultiplayer() couldn't extract player handle for player id {0} and connection reason {1}", player_id, connection_reason));
			return null;
		}
		foreach (MPBossMultiplayerPool.VirtualConnection virtualConnection in this.virtualConnections)
		{
			if (virtualConnection != null && virtualConnection.player_handle == playerHandle)
			{
				if (type == Logic.Multiplayer.Type.Client)
				{
					return virtualConnection.client;
				}
				if (type == Logic.Multiplayer.Type.ServerClient)
				{
					return virtualConnection.serverClient;
				}
			}
		}
		return null;
	}

	// Token: 0x06002D94 RID: 11668 RVA: 0x0017A1BC File Offset: 0x001783BC
	public void ConnectToPlayer(string player_id, Logic.Multiplayer.ConnectionReason connection_reason = Logic.Multiplayer.ConnectionReason.Meta)
	{
		if (string.IsNullOrEmpty(player_id))
		{
			MPBossMultiplayerPool.Error("Empty player_id on ConnectToPlayer");
			return;
		}
		if (this.ContainsVirtualConnection(player_id, connection_reason))
		{
			return;
		}
		Logic.Multiplayer multiplayer = new Logic.Multiplayer(MPBoss.Get().game, Logic.Multiplayer.Type.Client);
		multiplayer.AddListener(this);
		MPBossMultiplayerPool.VirtualConnection virtualConnection = new MPBossMultiplayerPool.VirtualConnection(player_id, connection_reason);
		virtualConnection.client = multiplayer;
		this.virtualConnections.Add(virtualConnection);
		multiplayer.Connect(player_id, connection_reason, false);
	}

	// Token: 0x06002D95 RID: 11669 RVA: 0x0017A224 File Offset: 0x00178424
	public bool ContainsVirtualConnection(string player_id, Logic.Multiplayer.ConnectionReason connection_reason)
	{
		if (string.IsNullOrEmpty(player_id))
		{
			return false;
		}
		if (connection_reason == Logic.Multiplayer.ConnectionReason.Invalid)
		{
			MPBossMultiplayerPool.Error("ContainsVirtualConnection() called with invalid connection reason");
		}
		string playerHandle = Logic.Multiplayer.GetPlayerHandle(player_id, connection_reason);
		return !string.IsNullOrEmpty(playerHandle) && this.ContainsVirtualConnection(playerHandle);
	}

	// Token: 0x06002D96 RID: 11670 RVA: 0x0017A264 File Offset: 0x00178464
	public bool ContainsVirtualConnection(string player_handle)
	{
		if (string.IsNullOrEmpty(player_handle))
		{
			return false;
		}
		if (this.virtualConnections == null)
		{
			return false;
		}
		foreach (MPBossMultiplayerPool.VirtualConnection virtualConnection in this.virtualConnections)
		{
			if (virtualConnection != null && virtualConnection.player_handle == player_handle)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002D97 RID: 11671 RVA: 0x0017A2DC File Offset: 0x001784DC
	public MPBossMultiplayerPool.VirtualConnection Get(string player_id, Logic.Multiplayer.ConnectionReason connection_reason)
	{
		if (string.IsNullOrEmpty(player_id))
		{
			MPBossMultiplayerPool.Error("Get() called with empty player id");
			return null;
		}
		if (connection_reason == Logic.Multiplayer.ConnectionReason.Invalid)
		{
			MPBossMultiplayerPool.Error("Get() called with invalid connection reason");
			return null;
		}
		string playerHandle = Logic.Multiplayer.GetPlayerHandle(player_id, connection_reason);
		if (string.IsNullOrEmpty(playerHandle))
		{
			MPBossMultiplayerPool.Error(string.Format("Get() couldn't extract player handle for player id {0} and connection reason {1}", player_id, connection_reason));
			return null;
		}
		return this.Get(playerHandle);
	}

	// Token: 0x06002D98 RID: 11672 RVA: 0x0017A33C File Offset: 0x0017853C
	private MPBossMultiplayerPool.VirtualConnection Get(string player_handle)
	{
		if (string.IsNullOrEmpty(player_handle))
		{
			return null;
		}
		if (this.virtualConnections == null)
		{
			return null;
		}
		for (int i = 0; i < this.virtualConnections.Count; i++)
		{
			MPBossMultiplayerPool.VirtualConnection virtualConnection = this.virtualConnections[i];
			if (virtualConnection != null && virtualConnection.player_handle == player_handle)
			{
				return virtualConnection;
			}
		}
		return null;
	}

	// Token: 0x06002D99 RID: 11673 RVA: 0x0017A394 File Offset: 0x00178594
	private void NotifyOffline(string player_id)
	{
		if (PlayerInfo.Get(player_id, true).online_state == PlayerInfo.OnlineState.Offline)
		{
			return;
		}
		Value[] param = new Value[]
		{
			player_id,
			false
		};
		MPBoss mpboss = MPBoss.Get();
		if (mpboss == null)
		{
			return;
		}
		mpboss.OnMessage(this, "player_presence_changed", param);
	}

	// Token: 0x06002D9A RID: 11674 RVA: 0x0017A3E8 File Offset: 0x001785E8
	private void CleanUpConnection(MPBossMultiplayerPool.VirtualConnection connection)
	{
		if (connection == null)
		{
			return;
		}
		if (connection.client != null && !connection.client.IsValid() && connection.serverClient != null)
		{
			connection.client = null;
		}
		if (connection.serverClient != null && !connection.serverClient.IsValid())
		{
			connection.serverClient = null;
		}
		if (connection.client == null && connection.serverClient == null)
		{
			this.virtualConnections.Remove(connection);
		}
		if (connection.connection_reason != Logic.Multiplayer.ConnectionReason.Meta)
		{
			return;
		}
		if (connection.IsOnline())
		{
			return;
		}
		this.NotifyOffline(connection.player_id);
	}

	// Token: 0x06002D9B RID: 11675 RVA: 0x0017A478 File Offset: 0x00178678
	public void UpdateMetaConnections(List<string> player_list)
	{
		if (player_list == null)
		{
			MPBossMultiplayerPool.Error("UpdateMetaConnections called with null player_list");
			return;
		}
		if (this.virtualConnections == null)
		{
			MPBossMultiplayerPool.Error("MPBossMultiplayerPool's pool is null when calling UpdateMetaConnections");
			return;
		}
		for (int i = this.virtualConnections.Count - 1; i >= 0; i--)
		{
			MPBossMultiplayerPool.VirtualConnection virtualConnection = this.virtualConnections[i];
			if (virtualConnection != null)
			{
				string player_id = virtualConnection.player_id;
				if (!player_list.Contains(player_id) && virtualConnection.client != null)
				{
					if (virtualConnection.client.IsValid())
					{
						virtualConnection.client.ShutDown();
						if (virtualConnection.serverClient == null || !virtualConnection.serverClient.IsValid())
						{
							this.NotifyOffline(virtualConnection.player_id);
						}
					}
					virtualConnection.client = null;
				}
			}
		}
		for (int j = this.virtualConnections.Count - 1; j >= 0; j--)
		{
			MPBossMultiplayerPool.VirtualConnection connection = this.virtualConnections[j];
			this.CleanUpConnection(connection);
		}
		for (int k = 0; k < player_list.Count; k++)
		{
			string player_id2 = player_list[k];
			if (!this.ContainsVirtualConnection(player_id2, Logic.Multiplayer.ConnectionReason.Meta))
			{
				this.ConnectToPlayer(player_id2, Logic.Multiplayer.ConnectionReason.Meta);
			}
		}
	}

	// Token: 0x06002D9C RID: 11676 RVA: 0x0017A588 File Offset: 0x00178788
	void IListener.OnMessage(object obj, string message, object param)
	{
		if (message == "connection_established")
		{
			Logic.Multiplayer multiplayer = param as Logic.Multiplayer;
			if (MPBoss.LogEnabled(2))
			{
				MPBoss.Log(string.Format("connection_established for multiplayer {0}", multiplayer), 2);
			}
			if (((multiplayer != null) ? multiplayer.playerData : null) == null || string.IsNullOrEmpty(multiplayer.playerData.id))
			{
				MPBossMultiplayerPool.Error(string.Format("Invalid mp {0} on connection_established", multiplayer));
				return;
			}
			string target = multiplayer.GetTarget();
			Logic.Multiplayer.ConnectionReason connectionReason = multiplayer.connectionReason;
			string handle = multiplayer.GetHandle();
			if (string.IsNullOrEmpty(handle))
			{
				MPBossMultiplayerPool.Error(string.Format("Empty player handle for multiplayer {0} on connection_established", multiplayer));
				return;
			}
			MPBossMultiplayerPool.VirtualConnection virtualConnection = this.Get(handle);
			if (virtualConnection == null)
			{
				virtualConnection = new MPBossMultiplayerPool.VirtualConnection(target, connectionReason);
				this.virtualConnections.Add(virtualConnection);
			}
			if (multiplayer.type == Logic.Multiplayer.Type.Client)
			{
				if (virtualConnection.client != null && virtualConnection.client != multiplayer)
				{
					MPBossMultiplayerPool.Error("Pool already contains a client multiplayer for player_id:" + virtualConnection.player_handle + ". Destroying the old one and adding the new one.");
				}
				virtualConnection.client = multiplayer;
			}
			else if (multiplayer.type == Logic.Multiplayer.Type.ServerClient)
			{
				if (virtualConnection.serverClient != null && virtualConnection.serverClient != multiplayer)
				{
					MPBossMultiplayerPool.Error("Pool already contains a serverClient multiplayer for player_id:" + virtualConnection.player_handle + ". Destroying the old one and adding the new one.");
				}
				virtualConnection.serverClient = multiplayer;
			}
			multiplayer.AddListener(this);
			if (connectionReason != Logic.Multiplayer.ConnectionReason.Meta)
			{
				return;
			}
			if (PlayerInfo.Get(target, true).online_state == PlayerInfo.OnlineState.Online)
			{
				return;
			}
			Value[] param2 = new Value[]
			{
				target,
				true
			};
			MPBoss mpboss = MPBoss.Get();
			if (mpboss == null)
			{
				return;
			}
			mpboss.OnMessage(this, "player_presence_changed", param2);
			return;
		}
		else if (message == "connection_dropped")
		{
			Vars vars = param as Vars;
			Logic.Multiplayer multiplayer2 = vars.Get<Logic.Multiplayer>("multiplayer", null);
			if (vars.Get<bool>("is_ungraceful", false))
			{
				MPBoss mpboss2 = MPBoss.Get();
				if (mpboss2 != null)
				{
					mpboss2.OnMessage(this, "ungraceful_disconnect", param);
				}
			}
			multiplayer2.DelListener(this);
			if (MPBoss.LogEnabled(2))
			{
				MPBoss.Log(string.Format("connection_dropped for multiplayer {0}", multiplayer2), 2);
			}
			if (((multiplayer2 != null) ? multiplayer2.playerData : null) == null || string.IsNullOrEmpty(multiplayer2.playerData.id))
			{
				MPBossMultiplayerPool.Error(string.Format("Invalid multiplayer {0} on connection_dropped", multiplayer2));
				return;
			}
			string handle2 = multiplayer2.GetHandle();
			if (string.IsNullOrEmpty(handle2))
			{
				MPBossMultiplayerPool.Error(string.Format("Empty player handle for multiplayer {0} on connection_dropped", multiplayer2));
				return;
			}
			MPBossMultiplayerPool.VirtualConnection virtualConnection2 = this.Get(handle2);
			if (virtualConnection2 == null)
			{
				return;
			}
			if (multiplayer2.type == Logic.Multiplayer.Type.Client)
			{
				if (virtualConnection2.client != multiplayer2)
				{
					MPBossMultiplayerPool.Error("Pool contains a different client for player_handle: " + virtualConnection2.player_handle);
				}
			}
			else if (multiplayer2.type == Logic.Multiplayer.Type.ServerClient)
			{
				if (virtualConnection2.serverClient != multiplayer2)
				{
					MPBossMultiplayerPool.Error("Pool contains a different serverClient for player_handle: " + virtualConnection2.player_handle);
				}
				else
				{
					virtualConnection2.serverClient = null;
				}
			}
			this.CleanUpConnection(virtualConnection2);
			return;
		}
		else if (message == "data_changed")
		{
			MPBoss mpboss3 = MPBoss.Get();
			if (mpboss3 == null)
			{
				return;
			}
			mpboss3.OnMessage(this, "data_changed", param);
			return;
		}
		else if (message == "lobby_chat_received")
		{
			MPBoss mpboss4 = MPBoss.Get();
			if (mpboss4 == null)
			{
				return;
			}
			mpboss4.OnMessage(this, "lobby_chat_received", param);
			return;
		}
		else if (message == "join_request")
		{
			MPBoss mpboss5 = MPBoss.Get();
			if (mpboss5 == null)
			{
				return;
			}
			mpboss5.OnMessage(this, "join_request", param);
			return;
		}
		else if (message == "join_request_fail_response")
		{
			MPBoss mpboss6 = MPBoss.Get();
			if (mpboss6 == null)
			{
				return;
			}
			mpboss6.OnMessage(this, "join_request_fail_response", param);
			return;
		}
		else if (message == "on_request_political_data")
		{
			MPBoss mpboss7 = MPBoss.Get();
			if (mpboss7 == null)
			{
				return;
			}
			mpboss7.OnMessage(obj, "on_request_political_data", param);
			return;
		}
		else if (message == "on_political_data")
		{
			MPBoss mpboss8 = MPBoss.Get();
			if (mpboss8 == null)
			{
				return;
			}
			mpboss8.OnMessage(obj, "on_political_data", param);
			return;
		}
		else
		{
			if (!(message == "interupt_countdown"))
			{
				return;
			}
			MPBoss mpboss9 = MPBoss.Get();
			if (mpboss9 == null)
			{
				return;
			}
			mpboss9.OnMessage(this, "interupt_countdown", param);
			return;
		}
	}

	// Token: 0x06002D9D RID: 11677 RVA: 0x0017A958 File Offset: 0x00178B58
	public static void Log(string msg)
	{
		msg = DateTime.Now.ToString("HH:mm:ss.fff: ") + msg;
		Debug.Log(msg);
	}

	// Token: 0x06002D9E RID: 11678 RVA: 0x0017A988 File Offset: 0x00178B88
	public static void Warning(string msg)
	{
		msg = DateTime.Now.ToString("HH:mm:ss.fff: ") + msg;
		Debug.LogWarning(DateTime.Now.ToString("HH:mm:ss.fff: ") + msg);
	}

	// Token: 0x06002D9F RID: 11679 RVA: 0x0017A9CC File Offset: 0x00178BCC
	public static void Error(string msg)
	{
		msg = DateTime.Now.ToString("HH:mm:ss.fff: ") + msg;
		Debug.LogError(DateTime.Now.ToString("HH:mm:ss.fff: ") + msg);
	}

	// Token: 0x04001ED2 RID: 7890
	public List<MPBossMultiplayerPool.VirtualConnection> virtualConnections;

	// Token: 0x02000848 RID: 2120
	public class VirtualConnection
	{
		// Token: 0x0600509E RID: 20638 RVA: 0x0023A690 File Offset: 0x00238890
		public VirtualConnection(string player_id, Logic.Multiplayer.ConnectionReason connection_reason)
		{
			this.player_id = player_id;
			this.connection_reason = connection_reason;
			string playerHandle = Logic.Multiplayer.GetPlayerHandle(player_id, connection_reason);
			if (string.IsNullOrEmpty(playerHandle))
			{
				MPBossMultiplayerPool.Error(string.Format("Couldn't extract player handle for player id {0} and connection reason {1}", player_id, connection_reason));
			}
			this.player_handle = playerHandle;
		}

		// Token: 0x0600509F RID: 20639 RVA: 0x0023A6DE File Offset: 0x002388DE
		public bool IsOnline()
		{
			return (this.client != null && this.client.IsConnected()) || (this.serverClient != null && this.serverClient.IsConnected());
		}

		// Token: 0x060050A0 RID: 20640 RVA: 0x0023A710 File Offset: 0x00238910
		public override string ToString()
		{
			string text = (this.client == null) ? "null" : this.client.ToString();
			string text2 = (this.serverClient == null) ? "null" : this.serverClient.ToString();
			return string.Concat(new string[]
			{
				"[",
				this.player_handle,
				"] C: ",
				text,
				", SC: ",
				text2
			});
		}

		// Token: 0x04003E9E RID: 16030
		public string player_id;

		// Token: 0x04003E9F RID: 16031
		public Logic.Multiplayer.ConnectionReason connection_reason;

		// Token: 0x04003EA0 RID: 16032
		public string player_handle;

		// Token: 0x04003EA1 RID: 16033
		public Logic.Multiplayer client;

		// Token: 0x04003EA2 RID: 16034
		public Logic.Multiplayer serverClient;
	}
}
