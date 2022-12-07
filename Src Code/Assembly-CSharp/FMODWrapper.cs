using System;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

// Token: 0x0200010E RID: 270
public static class FMODWrapper
{
	// Token: 0x06000C5B RID: 3163 RVA: 0x0008A968 File Offset: 0x00088B68
	public static EventInstance CreateInstance(string path, bool set_3d = true)
	{
		EventInstance result;
		try
		{
			result = RuntimeManager.CreateInstance(path, set_3d);
		}
		catch (EventNotFoundException ex)
		{
			AudioLog.Error(ex.Message);
			result = default(EventInstance);
		}
		return result;
	}

	// Token: 0x06000C5C RID: 3164 RVA: 0x0008A9A4 File Offset: 0x00088BA4
	public static void PlayOneShot(string path, Vector3 position = default(Vector3))
	{
		try
		{
			RuntimeManager.PlayOneShot(path, position);
		}
		catch (EventNotFoundException ex)
		{
			AudioLog.Error(ex.Message);
		}
	}

	// Token: 0x06000C5D RID: 3165 RVA: 0x0008A9D8 File Offset: 0x00088BD8
	public static void PlayOneShotAttached(string path, GameObject go)
	{
		try
		{
			RuntimeManager.PlayOneShotAttached(path, go);
		}
		catch (EventNotFoundException ex)
		{
			AudioLog.Error(ex.Message);
		}
	}

	// Token: 0x06000C5E RID: 3166 RVA: 0x0008AA0C File Offset: 0x00088C0C
	public static EventDescription GetEventDescription(string path)
	{
		EventDescription result;
		try
		{
			result = RuntimeManager.GetEventDescription(path);
		}
		catch (EventNotFoundException ex)
		{
			AudioLog.Error(ex.Message);
			result = default(EventDescription);
		}
		return result;
	}

	// Token: 0x0200060D RID: 1549
	public class Snapshot
	{
		// Token: 0x060046B1 RID: 18097 RVA: 0x0020F92C File Offset: 0x0020DB2C
		[ConsoleMethod("active_snapshots")]
		public static void ActiveSnapshotLog()
		{
			for (int i = 0; i < FMODWrapper.Snapshot.active_snapshots.Count; i++)
			{
				UnityEngine.Debug.Log(FMODWrapper.Snapshot.active_snapshots[i].key);
			}
		}

		// Token: 0x060046B2 RID: 18098 RVA: 0x0020F963 File Offset: 0x0020DB63
		public Snapshot(string key)
		{
			this.key = key;
		}

		// Token: 0x060046B3 RID: 18099 RVA: 0x0020F974 File Offset: 0x0020DB74
		private bool InitSnapshot()
		{
			if (!this.snapshot_instance.isValid())
			{
				string @string = Defs.GetString("Sounds", this.key, null, "");
				if (string.IsNullOrEmpty(@string))
				{
					return false;
				}
				this.snapshot_instance = FMODWrapper.CreateInstance(@string, true);
				if (!this.snapshot_instance.isValid())
				{
					AudioLog.Error("Failed to create a valid snapshot for " + @string);
					RESULT result = this.snapshot_instance.release();
					if (result != RESULT.OK)
					{
						AudioLog.Error(string.Format("Failed to release {0} snapshot! Result: {1}", @string, result));
					}
					return false;
				}
			}
			return true;
		}

		// Token: 0x060046B4 RID: 18100 RVA: 0x0020FA04 File Offset: 0x0020DC04
		public bool IsRunning()
		{
			if (!this.snapshot_instance.isValid())
			{
				return false;
			}
			PLAYBACK_STATE playback_STATE;
			RESULT playbackState = this.snapshot_instance.getPlaybackState(out playback_STATE);
			if (playbackState != RESULT.OK)
			{
				AudioLog.Error(string.Format("Failed to get playback state for {0} snapshot: result={1}", this.key, playbackState));
			}
			return playback_STATE != PLAYBACK_STATE.STOPPED && playback_STATE != PLAYBACK_STATE.STOPPING;
		}

		// Token: 0x060046B5 RID: 18101 RVA: 0x0020FA5C File Offset: 0x0020DC5C
		public void StartSnapshot()
		{
			if (this.IsRunning())
			{
				return;
			}
			if (!this.InitSnapshot())
			{
				return;
			}
			RESULT result = this.snapshot_instance.start();
			if (result != RESULT.OK)
			{
				AudioLog.Error(string.Format("Failed to start {0} snapshot! Result: {1}", this.key, result));
				return;
			}
			AudioLog.Info("Started " + this.key + " snapshot");
			FMODWrapper.Snapshot.active_snapshots.Add(this);
		}

		// Token: 0x060046B6 RID: 18102 RVA: 0x0020FACC File Offset: 0x0020DCCC
		public void EndSnapshot()
		{
			if (!this.IsRunning())
			{
				return;
			}
			RESULT result = this.snapshot_instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			if (result != RESULT.OK)
			{
				AudioLog.Error(string.Format("Failed to stop {0} snapshot! Result: {1}", this.key, result));
			}
			RESULT result2 = this.snapshot_instance.release();
			if (result2 != RESULT.OK)
			{
				AudioLog.Error(string.Format("Failed to release {0} snapshot! Result: {1}", this.key, result2));
			}
			this.snapshot_instance.clearHandle();
			if (result == RESULT.OK && result2 == RESULT.OK)
			{
				AudioLog.Info("Stopped " + this.key + " snapshot");
			}
			FMODWrapper.Snapshot.active_snapshots.Remove(this);
		}

		// Token: 0x0400339F RID: 13215
		public static List<FMODWrapper.Snapshot> active_snapshots = new List<FMODWrapper.Snapshot>();

		// Token: 0x040033A0 RID: 13216
		public string key;

		// Token: 0x040033A1 RID: 13217
		private EventInstance snapshot_instance;
	}
}
