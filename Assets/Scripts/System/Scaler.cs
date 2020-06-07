using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scaler : MonoBehaviour
{
    // Start is called before the first frame update
    public bool KeepAspectRatio;
    public bool InversedX;
    public bool InversedY;

    void Awake()
    {
        var topRightCorner = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        var worldSpaceWidth = topRightCorner.x * 2;
        var worldSpaceHeight = topRightCorner.y * 2;

        var spriteSize = gameObject.GetComponent<SpriteRenderer>().bounds.size;

        var scaleFactorX = worldSpaceWidth / spriteSize.x;
        scaleFactorX = (InversedX && scaleFactorX > 0f) ? scaleFactorX * -1 : scaleFactorX;
        var scaleFactorY = worldSpaceHeight / spriteSize.y;
        scaleFactorY = (InversedY && scaleFactorY > 0f) ? scaleFactorY * -1 : scaleFactorY;

        if (KeepAspectRatio)
        {
            if (scaleFactorX > scaleFactorY)
            {
                scaleFactorY = scaleFactorX;
            }
            else
            {
                scaleFactorX = scaleFactorY;
            }
        }

        gameObject.transform.localScale = new Vector3(scaleFactorX * 1.2f, scaleFactorY * 1.2f, 1);
    }
}
