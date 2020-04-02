using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyManager : MonoBehaviour
{
    [SerializeField] private Material _skyMaterial;
    [SerializeField] private float gradientRadius;
    [SerializeField] private Vector2 gradientPosition;
    private Color32 _gradientColorTopRed;
    private Color32 _gradientColorBottomRed;
    private Color32 _gradientColorTopBlue;
    private Color32 _gradientColorBottomBlue;
    private static readonly int GradientColorBottom = Shader.PropertyToID("gradientColorBottom");
    private static readonly int GradientColorTop = Shader.PropertyToID("gradientColorTop");
    private static readonly int RadialScale = Shader.PropertyToID("radialScale");
    private static readonly int Position = Shader.PropertyToID("position");

    void Awake()
    {
        _gradientColorTopRed = new Color32(255, 51, 156, 255);
        _gradientColorBottomRed = new Color32(254, 215, 177, 255);
        _gradientColorTopBlue = new Color32(4, 46, 168, 255);
        _gradientColorBottomBlue = new Color32(75, 78, 238, 255);
        
        _skyMaterial.SetColor(GradientColorTop, _gradientColorTopRed);
        _skyMaterial.SetColor(GradientColorBottom, _gradientColorBottomRed);
        
        _skyMaterial.SetFloat(RadialScale, gradientRadius);
        _skyMaterial.SetVector(Position, gradientPosition);
    }
}