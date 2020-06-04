using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class Door : MonoBehaviour {
    
    [SerializeField] private float offset;
    [SerializeField] private float timeToOpen;
    [SerializeField] private List<Lock> locks;
    
    private bool _closed = true;
    private bool _opening;
    private Vector2 _destination;
    private Vector2 _origin;
    
    void Start()
    {
        var pos = transform.position;
        _origin = pos;
        _destination = pos;
        _destination.y += offset;
    }

    // Update is called once per frame
    void Update()
    {
        if (locks.TrueForAll(l => !l.Locked))
        {
            _closed = false;
            _opening = true;
        }

        if (!_closed && _opening)
        {
            Open();
        }
    }

    private void Open()
    {
        var offsetY = Mathf.MoveTowards(transform.position.y, _destination.y, (_destination.y - _origin.y) * (Time.deltaTime / timeToOpen));
        transform.position = new Vector2(_origin.x, offsetY);
        if (transform.position.y == _destination.y) _opening = false;
    }
}
