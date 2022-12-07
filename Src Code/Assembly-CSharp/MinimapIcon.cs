using System;
using Logic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200023A RID: 570
public abstract class MinimapIcon
{
	// Token: 0x060022F6 RID: 8950
	public abstract Sprite GetSprite();

	// Token: 0x060022F7 RID: 8951
	public abstract float GetMinScale();

	// Token: 0x060022F8 RID: 8952
	public abstract float GetMaxScale();

	// Token: 0x060022F9 RID: 8953
	public abstract void UpdateSprite(bool force = false);

	// Token: 0x060022FA RID: 8954 RVA: 0x0013D805 File Offset: 0x0013BA05
	protected void CheckNullSprite(Sprite spr)
	{
		if (spr == null)
		{
			this._sprite_is_null = true;
		}
	}

	// Token: 0x060022FB RID: 8955 RVA: 0x0013D817 File Offset: 0x0013BA17
	protected void FixNullSprite()
	{
		if (this._sprite_is_null)
		{
			this._sprite_is_null = false;
			this.UpdateSprite(true);
			this.UpdateSpriteSize();
		}
	}

	// Token: 0x060022FC RID: 8956 RVA: 0x0013D835 File Offset: 0x0013BA35
	public bool IsSelected()
	{
		return this.selected;
	}

	// Token: 0x060022FD RID: 8957 RVA: 0x0013D840 File Offset: 0x0013BA40
	protected virtual void UpdateSpriteSize()
	{
		Sprite sprite = this.image.overrideSprite;
		if (sprite == null)
		{
			sprite = this.image.sprite;
		}
		if (sprite == null)
		{
			return;
		}
		this.image.rectTransform.sizeDelta = new Vector2(sprite.rect.width / 2f, sprite.rect.height / 2f);
	}

	// Token: 0x060022FE RID: 8958 RVA: 0x0013D8B5 File Offset: 0x0013BAB5
	public virtual Vector3 GetPosition()
	{
		if (this.target != null)
		{
			return this.target.VisualPosition();
		}
		if (this.targetTransform == null)
		{
			return Vector3.zero;
		}
		return this.targetTransform.position;
	}

	// Token: 0x060022FF RID: 8959 RVA: 0x0013D8EF File Offset: 0x0013BAEF
	public virtual Quaternion GetRotation()
	{
		return Quaternion.identity;
	}

	// Token: 0x06002300 RID: 8960 RVA: 0x0013D8F8 File Offset: 0x0013BAF8
	public static MinimapIcon GetIconByType(MapObject target, MiniMap minimap)
	{
		if (target is Logic.Army)
		{
			return ArmyMinimapIcon.Create(target, minimap);
		}
		if (target is Logic.Squad)
		{
			return SquadMinimapIcon.Create(target, minimap);
		}
		if (target is Logic.CapturePoint)
		{
			return CapturePointMinimapIcon.Create(target, minimap);
		}
		if (target is Logic.Battle)
		{
			return BattleMinimapIcon.Create(target, minimap);
		}
		return null;
	}

	// Token: 0x06002301 RID: 8961 RVA: 0x0013D946 File Offset: 0x0013BB46
	public virtual void Destroy()
	{
		if (this.image == null)
		{
			return;
		}
		global::Common.DestroyObj(this.image.gameObject);
		this.targetTransform = null;
		this.minimap = null;
		this.target = null;
		this.image = null;
	}

	// Token: 0x06002302 RID: 8962 RVA: 0x0013D983 File Offset: 0x0013BB83
	public void Update()
	{
		this.FixNullSprite();
	}

	// Token: 0x04001770 RID: 6000
	public Transform targetTransform;

	// Token: 0x04001771 RID: 6001
	public Image image;

	// Token: 0x04001772 RID: 6002
	public MapObject target;

	// Token: 0x04001773 RID: 6003
	protected static int defVersion = -1;

	// Token: 0x04001774 RID: 6004
	protected MiniMap minimap;

	// Token: 0x04001775 RID: 6005
	protected bool selected;

	// Token: 0x04001776 RID: 6006
	protected bool _sprite_is_null;
}
