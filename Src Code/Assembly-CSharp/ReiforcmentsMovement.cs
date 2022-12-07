using System;
using Logic;
using UnityEngine;

// Token: 0x02000171 RID: 369
public class ReiforcmentsMovement
{
	// Token: 0x060012D4 RID: 4820 RVA: 0x000C4383 File Offset: 0x000C2583
	public ReiforcmentsMovement(GameObject go)
	{
		this.game = GameLogic.Get(true);
		this.obj = go;
	}

	// Token: 0x060012D5 RID: 4821 RVA: 0x000C43A9 File Offset: 0x000C25A9
	public Point GetPosition()
	{
		return new Point(this.obj.transform.position.x, this.obj.transform.position.z);
	}

	// Token: 0x060012D6 RID: 4822 RVA: 0x000C43DA File Offset: 0x000C25DA
	public void Stop(bool send_state = true, bool clearPendingPf = true)
	{
		this.CalcPosition();
		if (clearPendingPf && this.pf_path != null)
		{
			this.pf_path.Clear();
			this.pf_path = null;
		}
		if (this.path != null)
		{
			this.path.Clear();
			this.path = null;
		}
	}

	// Token: 0x060012D7 RID: 4823 RVA: 0x000C4419 File Offset: 0x000C2619
	public void StopPathfinding()
	{
		if (this.pf_path != null)
		{
			this.pf_path.Clear();
			this.pf_path = null;
		}
	}

	// Token: 0x060012D8 RID: 4824 RVA: 0x000C4438 File Offset: 0x000C2638
	public void MoveTo(Point pt, float range = 0f)
	{
		if (this.game == null || this.game.path_finding == null)
		{
			return;
		}
		this.StopPathfinding();
		Action action = this.onPathChanged;
		if (action != null)
		{
			action();
		}
		if (this.GetPosition().InRange(pt, range))
		{
			this.Stop(true, true);
			Action action2 = this.onBeginMoving;
			if (action2 != null)
			{
				action2();
			}
			Action<MapObject> action3 = this.onDestinationReached;
			if (action3 == null)
			{
				return;
			}
			action3(null);
			return;
		}
		else
		{
			if (this.ChangeDestination(pt, 0.1f, range))
			{
				return;
			}
			this.pf_path = new Path(this.game, this.GetPosition(), PathData.PassableArea.Type.All, false);
			this.pf_path.min_radius = this.min_radius;
			this.pf_path.max_radius = this.max_radius;
			Path path = this.pf_path;
			path.onPathFindingComplete = (Action)Delegate.Combine(path.onPathFindingComplete, new Action(this.OnPathFindingComplete));
			this.pf_path.Find(pt, range, false);
			return;
		}
	}

	// Token: 0x060012D9 RID: 4825 RVA: 0x000C453C File Offset: 0x000C273C
	public void MoveTo(MapObject dst_obj, float range = 0f)
	{
		if (this.game == null || this.game.path_finding == null)
		{
			return;
		}
		this.StopPathfinding();
		if (this.GetPosition().InRange(dst_obj.position, range))
		{
			this.Stop(true, true);
			Action action = this.onBeginMoving;
			if (action != null)
			{
				action();
			}
			Action<MapObject> action2 = this.onDestinationReached;
			if (action2 == null)
			{
				return;
			}
			action2(null);
			return;
		}
		else
		{
			if (this.ChangeDestination(dst_obj, 0.1f, range))
			{
				return;
			}
			this.pf_path = new Path(this.game, this.GetPosition(), PathData.PassableArea.Type.All, false);
			this.pf_path.min_radius = this.min_radius;
			this.pf_path.max_radius = this.max_radius;
			this.pf_path.Find(dst_obj, range, false);
			return;
		}
	}

	// Token: 0x060012DA RID: 4826 RVA: 0x000C460C File Offset: 0x000C280C
	private bool ChangeDestination(MapObject newDst_obj, float future_t, float range)
	{
		if (this.game == null || this.game.path_finding == null)
		{
			return false;
		}
		if (this.path == null)
		{
			return false;
		}
		this.StopPathfinding();
		this.CalcPosition();
		if (newDst_obj == null)
		{
			Game.Log("Destination object not valid!", Game.LogType.Warning);
			return false;
		}
		float cut_t = this.path.t + future_t;
		PPos src_pt = this.path.CutTo(cut_t);
		this.pf_path = new Path(this.game, this.GetPosition(), PathData.PassableArea.Type.All, false);
		this.pf_path.src_pt = src_pt;
		this.pf_path.min_radius = this.min_radius;
		this.pf_path.max_radius = this.max_radius;
		this.pf_path.Find(newDst_obj, range, false);
		return true;
	}

	// Token: 0x060012DB RID: 4827 RVA: 0x000C46CC File Offset: 0x000C28CC
	private bool ChangeDestination(Point newDst_pt, float future_t, float range)
	{
		if (this.game == null || this.game.path_finding == null)
		{
			return false;
		}
		if (this.path == null)
		{
			return false;
		}
		this.StopPathfinding();
		this.CalcPosition();
		float cut_t = this.path.t + future_t;
		PPos src_pt = this.path.CutTo(cut_t);
		this.pf_path = new Path(this.game, this.GetPosition(), PathData.PassableArea.Type.All, false);
		this.pf_path.src_pt = src_pt;
		this.pf_path.min_radius = this.min_radius;
		this.pf_path.max_radius = this.max_radius;
		this.pf_path.Find(newDst_pt, range, false);
		return true;
	}

	// Token: 0x060012DC RID: 4828 RVA: 0x000C4784 File Offset: 0x000C2984
	public void OnPathFindingComplete()
	{
		if (!this.pf_path.IsValid())
		{
			if (this.path == null)
			{
				this.Stop(true, true);
				return;
			}
			this.ChangeDestination(this.path.dst_pt, 1f, this.path.range);
			return;
		}
		else
		{
			if (this.path != null)
			{
				this.path.Append(this.pf_path, true);
			}
			else
			{
				this.path = this.pf_path;
				Action action = this.onBeginMoving;
				if (action != null)
				{
					action();
				}
				this.tmLastMove = this.game.time;
			}
			this.pf_path = null;
			Action action2 = this.onPathChanged;
			if (action2 == null)
			{
				return;
			}
			action2();
			return;
		}
	}

	// Token: 0x060012DD RID: 4829 RVA: 0x000C483C File Offset: 0x000C2A3C
	private bool UpdatePath()
	{
		if (this.game == null || this.game.path_finding == null)
		{
			return false;
		}
		if (this.path == null)
		{
			return false;
		}
		if (this.path.dst_obj == null)
		{
			return true;
		}
		if (!this.path.dst_obj.IsValid())
		{
			this.Stop(true, true);
			return false;
		}
		if (this.pf_path != null)
		{
			return true;
		}
		float num = this.path.dst_pt.Dist(this.path.dst_obj.position);
		if (num < 0.5f)
		{
			return true;
		}
		if (num < Math.Min(this.path.path_len - this.path.t, 100f) / 10f)
		{
			return true;
		}
		if (this.GetPosition().InRange(this.path.dst_obj.position, this.path.range))
		{
			MapObject dst_obj = this.path.dst_obj;
			this.Stop(true, true);
			this.onDestinationReached(dst_obj);
			return false;
		}
		return this.ChangeDestination(this.path.dst_obj, 1f, this.path.range);
	}

	// Token: 0x060012DE RID: 4830 RVA: 0x000C496C File Offset: 0x000C2B6C
	private void CalcPosition()
	{
		PPos ppos;
		float num;
		this.CalcPosition(true, out ppos, out num);
	}

	// Token: 0x060012DF RID: 4831 RVA: 0x000C4984 File Offset: 0x000C2B84
	public void CalcPosition(out PPos pt, out float path_t)
	{
		this.CalcPosition(false, out pt, out path_t);
	}

	// Token: 0x060012E0 RID: 4832 RVA: 0x000C4990 File Offset: 0x000C2B90
	private void CalcPosition(bool advance_path, out PPos pt, out float path_t)
	{
		if (this.game == null || this.game.path_finding == null)
		{
			pt = PPos.Invalid;
			path_t = 0f;
			return;
		}
		PPos ppos = this.GetPosition();
		if (this.path == null)
		{
			pt = ppos;
			path_t = 0f;
			return;
		}
		float num = this.game.time - this.tmLastMove;
		if (num <= 0f)
		{
			pt = ppos;
			path_t = 0f;
			return;
		}
		float num2 = this.speed;
		if (this.game.path_finding.data.GetNode(ppos).road)
		{
			num2 *= this.game.path_finding.settings.road_stickiness;
		}
		if (this.path.flee)
		{
			num2 *= 1.5f;
		}
		path_t = this.path.t + num2 * num;
		PPos ppos2;
		this.path.GetPathPoint(path_t, out pt, out ppos2, advance_path, 0f);
		if (advance_path)
		{
			this.tmLastMove = this.game.time;
			Action<Point> action = this.onMove;
			if (action == null)
			{
				return;
			}
			action(pt);
		}
	}

	// Token: 0x060012E1 RID: 4833 RVA: 0x000C4AC2 File Offset: 0x000C2CC2
	public bool IsMoving()
	{
		return this.path != null || this.pf_path != null;
	}

	// Token: 0x060012E2 RID: 4834 RVA: 0x000C4AD8 File Offset: 0x000C2CD8
	public void OnUpdate()
	{
		if (this.path == null)
		{
			return;
		}
		this.CalcPosition();
		if (!this.UpdatePath())
		{
			return;
		}
		if (this.path == null)
		{
			return;
		}
		if (!this.path.IsDone())
		{
			return;
		}
		this.Stop(true, false);
		Action<MapObject> action = this.onDestinationReached;
		if (action == null)
		{
			return;
		}
		action(null);
	}

	// Token: 0x04000C9F RID: 3231
	public Path path;

	// Token: 0x04000CA0 RID: 3232
	public Path pf_path;

	// Token: 0x04000CA1 RID: 3233
	public float speed = 1.5f;

	// Token: 0x04000CA2 RID: 3234
	public float min_radius;

	// Token: 0x04000CA3 RID: 3235
	public float max_radius;

	// Token: 0x04000CA4 RID: 3236
	public Logic.Time tmLastMove;

	// Token: 0x04000CA5 RID: 3237
	private Game game;

	// Token: 0x04000CA6 RID: 3238
	private GameObject obj;

	// Token: 0x04000CA7 RID: 3239
	public Action<Point> onMove;

	// Token: 0x04000CA8 RID: 3240
	public Action onPathChanged;

	// Token: 0x04000CA9 RID: 3241
	public Action onBeginMoving;

	// Token: 0x04000CAA RID: 3242
	public Action<MapObject> onDestinationReached;
}
