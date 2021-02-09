using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct OutlinePrefs
{
    [Range(0, 5)]
    public int Thickness;
    [ColorUsageAttribute(true, true)]
    public Color Color;
}

[ExecuteInEditMode]
public class PixelOutline : MonoBehaviour
{
    public SpriteRenderer Renderer;

    public Material DefaultMaterial;
    public Shader Shader;

    const string DefaultShader = @"Shader Graphs/Sprite Unlit Outline";

    [SerializeField]
    private OutlinePrefs prefs;

    public OutlinePrefs Prefs
    {
        get => prefs;
        set
        {
            prefs = value;
            SetMaterialValues();
        }
    }

    private Material OutlineMaterial;

    void Awake()
    {
        if (Shader == null)
            Shader = Shader.Find(DefaultShader);
        OutlineMaterial = new Material(Shader);
        SetOff();
    }

    public void SetOn()
    {
        Renderer.sharedMaterial = OutlineMaterial;
        SetMaterialValues();
    }

    public void SetOff()
    {
        Renderer.sharedMaterial = DefaultMaterial;
    }

    void SetMaterialValues()
    {
        if (!Renderer.sharedMaterial) return;
        Renderer.sharedMaterial.SetInt("_OutlineThickness", prefs.Thickness);
        Renderer.sharedMaterial.SetColor("_OutlineColor", prefs.Color);
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        Prefs = prefs;
    }
#endif

}
