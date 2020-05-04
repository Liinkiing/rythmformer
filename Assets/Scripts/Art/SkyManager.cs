using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyManager : MonoBehaviour
{
    #region Fields
    
    [SerializeField] private Material _skyMaterial;
    [SerializeField] private float gradientRadius;
    [SerializeField] private Vector2 gradientPosition;
    [SerializeField] private Color32 _gradientColorTop;
    [SerializeField] private Color32 _gradientColorBottom;

    [Header("Key colors")]
    [SerializeField] private Color32 _gradientColorTopRed;
    [SerializeField] private Color32 _gradientColorBottomRed;
    [SerializeField] private Color32 _gradientColorTopBlue;
    [SerializeField] private Color32 _gradientColorBottomBlue;
    
    #endregion
    
    #region ShaderProertiesIDs
    
    private static readonly int GradientColorBottom = Shader.PropertyToID("gradientColorBottom");
    private static readonly int GradientColorTop = Shader.PropertyToID("gradientColorTop");
    private static readonly int RadialScale = Shader.PropertyToID("radialScale");
    private static readonly int Position = Shader.PropertyToID("position");
    
    #endregion

    void Awake()
    {
        _skyMaterial.SetColor(GradientColorTop, _gradientColorTop);
        _skyMaterial.SetColor(GradientColorBottom, _gradientColorBottom);

        _skyMaterial.SetFloat(RadialScale, gradientRadius);
        _skyMaterial.SetVector(Position, gradientPosition);
    }
}