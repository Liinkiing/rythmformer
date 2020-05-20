using System;
using UnityEngine;
using UnityEngine.UI;

public class DoubleCast {
    private readonly BoxCollider2D _collider;
    private readonly Vector2 _crop;
    private Vector2 _direction;
    private ContactFilter2D _filter;
    private float _distance;

    public Vector2 Direction
    {
        get => _direction;
        set => _direction = value;
    }
    public float Distance
    {
        get => _distance;
        set => _distance = value;
    }

    public DoubleCast(BoxCollider2D collider, Vector2 direction, float distance, Vector2 crop, LayerMask layer)
    {
        _collider = collider;
        _crop = crop;
        _direction = direction;
        _filter = new ContactFilter2D();
        _filter.SetLayerMask(layer);
        _distance = distance;
    }

    public bool Check(Func<RaycastHit2D , bool> method)
    {
        var result = false;
        var arrs = GetCast();
        foreach (RaycastHit2D[] hits in arrs)
        {
            foreach (var hit in hits)
            {
                if (method(hit))
                {
                    result = true;
                    break;
                }
            }
        }
        
        return result;
    }

    public float MinDistance(bool checkGround = false)
    {
        var result = _distance;
        var arrs = GetCast();
        foreach (RaycastHit2D[] hits in arrs)
        {
            foreach (var hit in hits)
            {
                if (hit.distance > 0 && hit.distance < result)
                {
                    if (!checkGround)
                    {
                        result = hit.distance;
                    } else if (hit.normal.y > 0.1f)
                    {
                        result = hit.distance;
                    }
                }
            }
        }

        return result;
    }
    
    public Array GetCast()
    {
        var origin = _collider.transform.position;
        var size = _collider.size;
        var cropSize = size * _crop;
        
        var hits = new RaycastHit2D[10];
        Physics2D.BoxCast(origin, size, 0, _direction, _filter, hits, _distance);
        var cropHits = new RaycastHit2D[10];
        Physics2D.BoxCast(origin, cropSize, 0, _direction, _filter, cropHits, _distance);
        var arr = new Array[2];
        arr[0] = hits;
        arr[1] = cropHits;
        
        return arr;
    }
    public bool Up(RaycastHit2D hit)
    {
        if (hit.distance <= 0 && hit.normal.y < -0.1f)
        {
            return true;
        }
        return false;
    }
    public bool Right(RaycastHit2D hit)
    {
        if (hit.distance <= 0 && hit.normal.x < -0.1f)
        {
            return true;
        }
        return false;
    }
    public bool Down(RaycastHit2D hit)
    {
        if (hit.distance <= 0 && hit.normal.y > 0.1f)
        {
            return true;
        }
        return false;
    }
    public bool Left(RaycastHit2D hit)
    {
        if (hit.distance <= 0 && hit.normal.x > 0.1f)
        {
            return true;
        }
        return false;
    }
}
