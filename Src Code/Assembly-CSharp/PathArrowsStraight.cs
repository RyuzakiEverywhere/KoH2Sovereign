using System;
using Logic;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x0200013C RID: 316
public class PathArrowsStraight : MonoBehaviour
{
	// Token: 0x060010BA RID: 4282 RVA: 0x000B224D File Offset: 0x000B044D
	public static PathArrowsStraight Create(Movement movement, float shooting_range, TextureBaker texture_baker, int step)
	{
		PathArrowsStraight pathArrowsStraight = new GameObject("PathArrows").AddComponent<PathArrowsStraight>();
		pathArrowsStraight.step = step;
		pathArrowsStraight.texture_baker = texture_baker;
		pathArrowsStraight.movement = movement;
		pathArrowsStraight.shooting_range = shooting_range;
		pathArrowsStraight.Update();
		return pathArrowsStraight;
	}

	// Token: 0x060010BB RID: 4283 RVA: 0x000B2280 File Offset: 0x000B0480
	private void RecalcPathPointsWithShootRange(ref TextureBaker.InstancedSelectionDrawerBatched.DrawCallData draw_call_data)
	{
		MapObject mapObject = this.movement.obj as MapObject;
		if (mapObject == null)
		{
			return;
		}
		Movement movement = this.movement;
		bool flag;
		if (movement == null)
		{
			flag = (null != null);
		}
		else
		{
			Path path = movement.path;
			flag = (((path != null) ? path.segments : null) != null);
		}
		if (!flag)
		{
			return;
		}
		this.start = mapObject.VisualPosition();
		if (this.movement.path.segments != null && this.movement.path.segments.Count > 0)
		{
			this.end = this.movement.path.segments[this.movement.path.segments.Count - 1].pt;
		}
		if (this.movement.path.dst_obj != null)
		{
			this.end = this.movement.path.dst_obj.VisualPosition();
		}
		float num = 0f;
		float num2 = 0f;
		Logic.Squad squad = mapObject as Logic.Squad;
		if (((squad != null) ? squad.formation : null) != null)
		{
			num = squad.formation.cur_height / 2f + 3f;
			global::Squad squad2 = squad.visuals as global::Squad;
			if (squad2 != null && squad2.Previewed)
			{
				num2 = num;
			}
		}
		if (this.shooting_range > 0f && this.movement.path.dst_obj != null)
		{
			float num3 = this.start.Dist(this.end);
			Point normalized = (this.end - this.start).GetNormalized();
			float num4 = num3 - this.shooting_range;
			float num5 = (float)this.step - this.shooting_range % (float)this.step;
			Point point = this.start + (num4 - num5) * normalized;
			if (num4 <= 0f)
			{
				point = this.start;
			}
			if (num4 > 0f)
			{
				this.RecalcPathPoints(this.start, point, ref draw_call_data, TextureBaker.InstancedSelectionDrawerBatched.SelectionType.PathArrow, num, 0f);
			}
			this.RecalcPathPoints(point, this.end, ref draw_call_data, TextureBaker.InstancedSelectionDrawerBatched.SelectionType.PathArrowShootRange, (num4 <= 0f) ? num : 1f, (((squad != null) ? squad.command_queue : null) != null && squad.command_queue.Count > 0) ? 0f : num2);
		}
		else
		{
			this.RecalcPathPoints(this.start, this.end, ref draw_call_data, TextureBaker.InstancedSelectionDrawerBatched.SelectionType.PathArrow, num, (((squad != null) ? squad.command_queue : null) != null && squad.command_queue.Count > 0) ? 0f : num2);
		}
		if (squad == null)
		{
			return;
		}
		int i = 0;
		while (i < squad.command_queue.Count)
		{
			Logic.Squad.MoveCommand moveCommand = squad.command_queue[i];
			this.start = this.end;
			if (moveCommand.target_obj == null)
			{
				this.end = moveCommand.target_pos;
				goto IL_3D3;
			}
			this.end = moveCommand.target_obj.VisualPosition();
			if (this.shooting_range <= 0f)
			{
				goto IL_3D3;
			}
			float num6 = this.start.Dist(this.end);
			Point normalized2 = (this.end - this.start).GetNormalized();
			float num7 = num6 - this.shooting_range;
			float num8 = (float)this.step - this.shooting_range % (float)this.step;
			Point point2 = this.start + (num7 - num8) * normalized2;
			if (num7 <= 0f)
			{
				point2 = this.start;
			}
			if (num7 > 0f)
			{
				this.RecalcPathPoints(this.start, point2, ref draw_call_data, TextureBaker.InstancedSelectionDrawerBatched.SelectionType.PathArrow, 0f, (i < squad.command_queue.Count - 1) ? 0f : num2);
			}
			this.RecalcPathPoints(point2, this.end, ref draw_call_data, TextureBaker.InstancedSelectionDrawerBatched.SelectionType.PathArrowShootRange, (float)((num7 <= 0f) ? 0 : 1), (i < squad.command_queue.Count - 1) ? 0f : num2);
			IL_405:
			i++;
			continue;
			IL_3D3:
			this.RecalcPathPoints(this.start, this.end, ref draw_call_data, TextureBaker.InstancedSelectionDrawerBatched.SelectionType.PathArrow, 0f, (i < squad.command_queue.Count - 1) ? 0f : num2);
			goto IL_405;
		}
	}

	// Token: 0x060010BC RID: 4284 RVA: 0x000B26AC File Offset: 0x000B08AC
	private void RecalcPathPoints(ref TextureBaker.InstancedSelectionDrawerBatched.DrawCallData draw_call_data)
	{
		MapObject mapObject = this.movement.obj as MapObject;
		if (mapObject == null)
		{
			return;
		}
		Movement movement = this.movement;
		bool flag;
		if (movement == null)
		{
			flag = (null != null);
		}
		else
		{
			Path path = movement.path;
			flag = (((path != null) ? path.segments : null) != null);
		}
		if (!flag)
		{
			return;
		}
		this.start = mapObject.VisualPosition();
		if (this.movement.path.segments != null && this.movement.path.segments.Count > 0)
		{
			this.end = this.movement.path.segments[this.movement.path.segments.Count - 1].pt;
		}
		if (this.movement.path.dst_obj != null)
		{
			this.end = this.movement.path.dst_obj.VisualPosition();
		}
		float num = 0f;
		float num2 = 0f;
		Logic.Squad squad = mapObject as Logic.Squad;
		if (((squad != null) ? squad.formation : null) != null)
		{
			num = squad.formation.cur_height / 2f + 3f;
			global::Squad squad2 = squad.visuals as global::Squad;
			if (squad2 != null && squad2.Previewed)
			{
				num2 = num;
			}
		}
		this.RecalcPathPoints(this.start, this.end, ref draw_call_data, TextureBaker.InstancedSelectionDrawerBatched.SelectionType.PathArrow, num, (((squad != null) ? squad.command_queue : null) != null && squad.command_queue.Count > 0) ? 0f : num2);
		if (squad == null)
		{
			return;
		}
		for (int i = 0; i < squad.command_queue.Count; i++)
		{
			Logic.Squad.MoveCommand moveCommand = squad.command_queue[i];
			this.start = this.end;
			if (moveCommand.target_obj == null)
			{
				this.end = moveCommand.target_pos;
			}
			else
			{
				this.end = moveCommand.target_obj.VisualPosition();
			}
			this.RecalcPathPoints(this.start, this.end, ref draw_call_data, TextureBaker.InstancedSelectionDrawerBatched.SelectionType.PathArrow, 0f, (i < squad.command_queue.Count - 1) ? 0f : num2);
		}
	}

	// Token: 0x060010BD RID: 4285 RVA: 0x000B28D4 File Offset: 0x000B0AD4
	private void RecalcPathPoints(Point start, Point end, ref TextureBaker.InstancedSelectionDrawerBatched.DrawCallData draw_call_data, TextureBaker.InstancedSelectionDrawerBatched.SelectionType selectionType, float offset_start = 0f, float offset_end = 0f)
	{
		if (!(this.movement.obj is MapObject))
		{
			return;
		}
		float rot;
		this.CalcRot(start, end, 0f, out rot);
		draw_call_data.rot = rot;
		Point pt = end - start;
		float num = pt.Length();
		pt /= num;
		for (float num2 = num - offset_end; num2 >= offset_start; num2 -= (float)this.step)
		{
			Point pt2 = start + pt * num2;
			draw_call_data.pos = pt2;
			if (selectionType == TextureBaker.InstancedSelectionDrawerBatched.SelectionType.PathArrowShootRange)
			{
				this.texture_baker.path_arrows_shooting_range_straight_drawer.model_data.Add(draw_call_data);
			}
			else
			{
				this.texture_baker.path_arrows_straight_drawer.model_data.Add(draw_call_data);
			}
		}
	}

	// Token: 0x060010BE RID: 4286 RVA: 0x000B299C File Offset: 0x000B0B9C
	private void Update()
	{
		Logic.Squad squad = this.movement.obj as Logic.Squad;
		if (squad == null)
		{
			return;
		}
		global::Squad squad2 = squad.visuals as global::Squad;
		if (squad2 == null || squad2.logic == null)
		{
			return;
		}
		TextureBaker.InstancedSelectionDrawerBatched.DrawCallData drawCallData = default(TextureBaker.InstancedSelectionDrawerBatched.DrawCallData);
		drawCallData.scale = 1f;
		Logic.Kingdom obj = BaseUI.LogicKingdom();
		if (squad.is_fleeing)
		{
			drawCallData.color_id = 7;
		}
		else if (squad2 != null && squad2.Previewed && !squad2.Selected)
		{
			drawCallData.color_id = 6;
		}
		else if (squad2.logic.IsOwnStance(obj))
		{
			drawCallData.color_id = 0;
		}
		else if (squad2.logic.IsEnemy(obj))
		{
			drawCallData.color_id = 1;
		}
		else
		{
			drawCallData.color_id = 2;
		}
		this.RecalcPathPointsWithShootRange(ref drawCallData);
	}

	// Token: 0x060010BF RID: 4287 RVA: 0x000B2A6C File Offset: 0x000B0C6C
	private bool CalcRot(float2 from, float2 to, float d2min, out float rot)
	{
		Point point = to - from;
		if (point.SqrLength() <= d2min)
		{
			rot = 0f;
			return false;
		}
		rot = 0.017453292f * -point.Heading();
		return true;
	}

	// Token: 0x04000B23 RID: 2851
	public int step = 1;

	// Token: 0x04000B24 RID: 2852
	private Point start;

	// Token: 0x04000B25 RID: 2853
	private Point end;

	// Token: 0x04000B26 RID: 2854
	private Movement movement;

	// Token: 0x04000B27 RID: 2855
	private float shooting_range;

	// Token: 0x04000B28 RID: 2856
	private TextureBaker texture_baker;
}
