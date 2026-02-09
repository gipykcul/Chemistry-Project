using UnityEngine;

public class DemoChemTest : MonoBehaviour
{
    [Header("Liquid color test")]
    public LiquidContainer beaker;     // LiquidContainer колбы
    [Range(0f, 1f)] public float mix01 = 0.5f; // 0 = чистая HCl, 1 = чистая HNO3
    public float totalMl = 200f;       // сколько "налито" в колбе для теста

    [Header("Gold dissolve test (stages)")]
    public SkinnedMeshRenderer goldRenderer;

    // Впиши индексы или поставь autoFind=true и он найдёт по именам
    public bool autoFindByName = true;
    public int stage1Index = -1;
    public int stage2DeltaIndex = -1;
    public int stage3DeltaIndex = -1;

    public string stage1Name = "Stage1";
    public string stage2DeltaName = "Stage2Delta";
    public string stage3DeltaName = "Stage3Delta";

    [Header("Timing")]
    public bool debugPlayDissolve = false;
    public float stage1Seconds = 4f;
    public float stage2Seconds = 4f;
    public float stage3Seconds = 4f;

    float t;

    void Start()
    {
        if (autoFindByName && goldRenderer != null)
            FindBlendShapeIndices();

        ApplyMixture();
        ResetStages();
    }

    void Update()
    {
        // 1) Тест цвета: менять mix01 ползунком в инспекторе во время Play
        // Чтобы обновлялось сразу:
        ApplyMixture();

        // 2) Тест растворения: флажок debugPlayDissolve
        if (debugPlayDissolve)
        {
            PlayStagedDissolve();
        }

        // Быстрые горячие клавиши (удобно без VR):
        if (Input.GetKeyDown(KeyCode.Alpha1)) { mix01 = 0f; ApplyMixture(); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { mix01 = 0.5f; ApplyMixture(); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { mix01 = 1f; ApplyMixture(); }

        if (Input.GetKeyDown(KeyCode.R)) ResetStages();
        if (Input.GetKeyDown(KeyCode.Space)) { debugPlayDissolve = !debugPlayDissolve; t = 0f; }
    }

    void ApplyMixture()
    {
        if (beaker == null) return;

        // задаём "сколько всего" и как делится на HCl/HNO3
        float hno3 = totalMl * Mathf.Clamp01(mix01);
        float hcl = totalMl - hno3;

        // напрямую записываем и обновляем визуал
        beaker.hclMl = hcl;
        beaker.hno3Ml = hno3;
        beaker.SetAmount(totalMl); // важно: пересчитает fill и цвет
    }

    void FindBlendShapeIndices()
    {
        var mesh = goldRenderer.sharedMesh;
        if (mesh == null) return;
        stage1Index = mesh.GetBlendShapeIndex(stage1Name);
        stage2DeltaIndex = mesh.GetBlendShapeIndex(stage2DeltaName);
        stage3DeltaIndex = mesh.GetBlendShapeIndex(stage3DeltaName);

        Debug.Log($"BlendShapes found: Stage1={stage1Index}, Stage2Delta={stage2DeltaIndex}, Stage3Delta={stage3DeltaIndex}");
    }

    void ResetStages()
    {
        t = 0f;
        SetWeightSafe(stage1Index, 0f);
        SetWeightSafe(stage2DeltaIndex, 0f);
        SetWeightSafe(stage3DeltaIndex, 0f);
    }

    void PlayStagedDissolve()
    {
        if (goldRenderer == null) return;

        float total = Mathf.Max(0.01f, stage1Seconds + stage2Seconds + stage3Seconds);
        t += Time.deltaTime;

        float time = t;

        // Stage1 0->100
        if (time <= stage1Seconds)
        {
            float u = time / Mathf.Max(0.01f, stage1Seconds);
            SetWeightSafe(stage1Index, u * 100f);
            SetWeightSafe(stage2DeltaIndex, 0f);
            SetWeightSafe(stage3DeltaIndex, 0f);
            return;
        }

        // Stage2Delta 0->100 (Stage1 держим 100)
        time -= stage1Seconds;
        if (time <= stage2Seconds)
        {
            float u = time / Mathf.Max(0.01f, stage2Seconds);
            SetWeightSafe(stage1Index, 100f);
            SetWeightSafe(stage2DeltaIndex, u * 100f);
            SetWeightSafe(stage3DeltaIndex, 0f);
            return;
        }

        // Stage3Delta 0->100 (Stage1 и Stage2Delta держим 100)
        time -= stage2Seconds;
        if (time <= stage3Seconds)
        {
            float u = time / Mathf.Max(0.01f, stage3Seconds);
            SetWeightSafe(stage1Index, 100f);
            SetWeightSafe(stage2DeltaIndex, 100f);
            SetWeightSafe(stage3DeltaIndex, u * 100f);
            return;
        }

        // конец
        SetWeightSafe(stage1Index, 100f);
        SetWeightSafe(stage2DeltaIndex, 100f);
        SetWeightSafe(stage3DeltaIndex, 100f);
        debugPlayDissolve = false;
    }

    void SetWeightSafe(int index, float w)
    {
        if (goldRenderer == null) return;
        if (index < 0) return;
        goldRenderer.SetBlendShapeWeight(index, Mathf.Clamp(w, 0f, 100f));
    }
}