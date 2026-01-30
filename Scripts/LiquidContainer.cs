using UnityEngine;

public class LiquidContainer : MonoBehaviour
{
    [Header("Capacity")]
    public float capacityMl = 500f;
    [SerializeField] float currentMl = 0f;

    [Header("Liquid visuals")]
    public Renderer liquidRenderer;
    [Range(0f, 1f)] public float fill01;

    [Header("Mixture (HCl / HNO3)")]
    public float hclMl;
    public float hno3Ml;

    public Color hclColor = new Color32(0xFC, 0xFC, 0xCA, 0x00);
    public Color hno3Color = new Color32(0x8B, 0x45, 0x13, 0xFF);

    static readonly int FillProp = Shader.PropertyToID("_FillLevel");
    static readonly int ColorProp = Shader.PropertyToID("_BaseColor");

    public float CurrentMl => currentMl;

    void Awake()
    {
        ApplyVisuals();
    }

    public void SetAmount(float ml)
    {
        currentMl = Mathf.Clamp(ml, 0f, capacityMl);

        float sum = hclMl + hno3Ml;
        if (sum > currentMl && sum > 0.0001f)
        {
            float k = currentMl / sum;
            hclMl *= k;
            hno3Ml *= k;
        }

        ApplyVisuals();
    }

    public void AddLiquid(float ml, float addHclMl, float addHno3Ml)
    {
        float newMl = Mathf.Clamp(currentMl + ml, 0f, capacityMl);
        float actuallyAdded = newMl - currentMl;
        if (actuallyAdded <= 0f) return;

        float incomingSum = addHclMl + addHno3Ml;
        float k = incomingSum > 0.0001f ? actuallyAdded / incomingSum : 0f;

        hclMl = Mathf.Clamp(hclMl + addHclMl * k, 0f, capacityMl);
        hno3Ml = Mathf.Clamp(hno3Ml + addHno3Ml * k, 0f, capacityMl);

        currentMl = newMl;
        ApplyVisuals();
    }

    public float RemoveLiquid(float ml, out float outHclMl, out float outHno3Ml)
    {
        float removed = Mathf.Clamp(ml, 0f, currentMl);
        if (removed <= 0f)
        {
            outHclMl = 0f;
            outHno3Ml = 0f;
            return 0f;
        }

        float sum = hclMl + hno3Ml;
        float hclShare = sum > 0.0001f ? hclMl / sum : 0f;
        float hno3Share = sum > 0.0001f ? hno3Ml / sum : 0f;

        outHclMl = removed * hclShare;
        outHno3Ml = removed * hno3Share;

        hclMl -= outHclMl;
        hno3Ml -= outHno3Ml;

        currentMl -= removed;
        ApplyVisuals();
        return removed;
    }

    void ApplyVisuals()
    {
        fill01 = capacityMl > 0 ? currentMl / capacityMl : 0f;
        if (liquidRenderer == null) return;

        var mpb = new MaterialPropertyBlock();
        liquidRenderer.GetPropertyBlock(mpb);

        mpb.SetFloat(FillProp, fill01);

        float sum = hclMl + hno3Ml;
        float t = sum > 0.0001f ? Mathf.Clamp01(hno3Ml / sum) : 0f;
        Color mixed = Color.Lerp(hclColor, hno3Color, t);

        mpb.SetColor(ColorProp, mixed);
        liquidRenderer.SetPropertyBlock(mpb);
    }
}
