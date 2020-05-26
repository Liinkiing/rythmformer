using System;
using UnityEngine;
using UnityEngine.UI;

public class RaycastGroup {
    private readonly BoxCollider2D _collider;
    private Vector2 _direction;
    private ContactFilter2D _filter;
    private float _distance;
    private readonly int _numberOfRays;
    private Vector2 _hitBoxMod;

    public RaycastGroup(BoxCollider2D collider, Vector2 direction, float distance, int numberOfRays, LayerMask layer, Vector2 hitBoxMod)
    {
        _collider = collider;
        _direction = direction;
        _numberOfRays = numberOfRays;
        _filter = new ContactFilter2D();
        _filter.SetLayerMask(layer);
        _distance = distance;
        _hitBoxMod = hitBoxMod;
    }
    
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
    
    public bool Check(Func<RaycastHit2D, bool> method)
    {
        var arrs = GetHits(GetOrigins(_direction), _direction, _distance);
        
        var result = false;
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

    public float MinDistance(Vector2 direction)
    {   
        var result = _distance;
        var arrs = GetHits(GetOrigins(direction), _direction, _distance);
        foreach (RaycastHit2D[] hits in arrs)
        {
            foreach (var hit in hits)
            {
                if (hit.distance > 0.01f && hit.distance < result)
                {
                    result = hit.distance - 0.01f;
                }
            }
        }

        return result;
    }

    private Vector2[] GetOrigins(Vector2 direction)
    {
        var origin = _collider.transform.position;
        var size = _collider.size * _hitBoxMod;
        
        var casts = new Vector2[_numberOfRays];
        if (Mathf.Abs(direction.x) > 0)
        {
            var posX = origin.x + direction.x * size.x / 2;
            var originY = origin.y - size.y / 2;
            for (var i = 0; i < _numberOfRays; i++)
            {
                casts[i] = new Vector2(posX, originY + size.y / (_numberOfRays - 1) * i);
            }
        } else if (Mathf.Abs(direction.y) > 0)
        {
            var posY = origin.y + direction.y * size.y / 2;
            var originX = origin.x - size.x / 2;
            for (var i = 0; i < _numberOfRays; i++)
            {
                casts[i] = new Vector2(originX + size.x / (_numberOfRays - 1) * i, posY);
            }
        }

        return casts;
    }

    private Array GetHits(Vector2[] casts, Vector2 direction, float distance)
    {
        var arrs = new Array[_numberOfRays];
        for (var i = 0; i < _numberOfRays; i++)
        {
            var hits = new RaycastHit2D[10];
            Physics2D.Raycast(casts[i], direction, _filter, hits, distance);
            arrs[i] = hits;
            
            Debug.DrawRay(casts[i], direction * distance, Color.green);
        }

        return arrs;
    }

    public bool TouchRight(RaycastHit2D hit)
    {
        if (hit.normal.x < -0.1f)
        {
            return true;
        }
        return false;
    }
    public bool TouchDown(RaycastHit2D hit)
    {
        if (hit.normal.y > 0.1f)
        {
            return true;
        }
        return false;
    }
    public bool TouchLeft(RaycastHit2D hit)
    {
        if (hit.normal.x > 0.1f)
        {
            return true;
        }
        return false;
    }
    public bool Up(RaycastHit2D hit)
    {
        if (hit.distance <= 0.01f && hit.normal.y < -0.1f)
        {
            return true;
        }
        return false;
    }
    public bool Right(RaycastHit2D hit)
    {
        if (hit.distance <= 0.01f && hit.normal.x < -0.1f)
        {
            return true;
        }
        return false;
    }
    public bool Down(RaycastHit2D hit)
    {
        if (hit.distance <= 0.01f && hit.normal.y > 0.1f)
        {
            return true;
        }
        return false;
    }
    public bool Left(RaycastHit2D hit)
    {
        if (hit.distance <= 0.01f && hit.normal.x > 0.1f)
        {
            return true;
        }
        return false;
    }
}