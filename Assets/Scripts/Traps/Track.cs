using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Track : MonoBehaviour {

    [SerializeField, Tooltip("Time in s for the object to return to its initial position.")]
    private float duration;
    [SerializeField, Tooltip("If looped, the object will find its path from the last coordinate to the first, else it will travel the whole path in reverse")]
    private bool loop;
    [SerializeField, Tooltip("Item to be moved on track.")]
    private Transform item;
    
    private List<Vector2> _coordinates;
    private List<float> _segments;
    private List<float> _timings;
    private int _previousTarget = 0;
    private int _currentTarget = 1;
    private int _currentSegment = 0;
    private float _totalLength;
    
    void Start()
    {
        _coordinates = new List<Vector2>();
        _segments = new List<float>();
        _timings = new List<float>();
        InitTrack();
    }

    private void InitTrack()
    {
        foreach (Transform child in transform)
        {
            _coordinates.Add(child.position);
        }
        
        for (int i = 0; i < _coordinates.Count; i++)
        {
            Vector2 d;
            if (loop && i == _coordinates.Count - 1)
            {
                d = _coordinates[0] - _coordinates[i];
            } else
            {
                if (i == _coordinates.Count - 1) break;
                d = _coordinates[i + 1] - _coordinates[i];
            }
            _segments.Add(Mathf.Sqrt(d.x * d.x + d.y * d.y));
        }

        if (loop)
        {
            _coordinates.Add(transform.GetChild(0).position);
        } else
        {
            var initialCount = _coordinates.Count;
            for (int i = initialCount - 2; i >= 0; i--)
            {
                _coordinates.Add(_coordinates[i]);
            }
            var initialCount2 = _segments.Count;
            for (int i = initialCount2 - 1; i >= 0; i--)
            {
                _segments.Add(_segments[i]);
            }
        }
        
        _totalLength = _segments.Sum();
        for (int i = 0; i < _segments.Count; i++)
        {
            _timings.Add(duration / _totalLength * _segments[i]);
        }
    }

    void Update()
    {
        item.position = Vector2.MoveTowards(item.position, _coordinates[_currentTarget], _segments[_currentSegment] * (Time.deltaTime / _timings[_currentSegment]));
        if (item.position.x == _coordinates[_currentTarget].x && item.position.y == _coordinates[_currentTarget].y)
        {
            _previousTarget++;
            _currentTarget++;
            _currentSegment++;

            if (_currentTarget == _coordinates.Count)
            {
                _currentTarget = 1;
                _previousTarget = 0;
                _currentSegment = 0;
            }
        }
    }
}
