using UnityEngine;

[ExecuteAlways]
[DisallowMultipleComponent]
public class LiquidBoundsToShader : MonoBehaviour
{
    [Header("References")]
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] Renderer targetRenderer;

    [Header("Shader property names")]
    [SerializeField] string minYProp = "_MinY";
    [SerializeField] string maxYProp = "_MaxY";

    [Header("Options")]
    [SerializeField] bool usePropertyBlock = true;

    void Reset()
    {
        meshFilter = GetComponent<MeshFilter>();
        targetRenderer = GetComponent<Renderer>();
        Apply();
    }

    void OnEnable() => Apply();

#if UNITY_EDITOR
    void OnValidate() => Apply();
#endif

    public void Apply()
    {
        if (meshFilter == null) meshFilter = GetComponent<MeshFilter>();
        if (targetRenderer == null) targetRenderer = GetComponent<Renderer>();
        if (meshFilter == null || targetRenderer == null) return;

        var mesh = meshFilter.sharedMesh;
        if (mesh == null) return;

        float minY = mesh.bounds.min.y;
        float maxY = mesh.bounds.max.y;

        if (usePropertyBlock)
        {
            var mpb = new MaterialPropertyBlock();
            targetRenderer.GetPropertyBlock(mpb);
            mpb.SetFloat(minYProp, minY);
            mpb.SetFloat(maxYProp, maxY);
            targetRenderer.SetPropertyBlock(mpb);
        }
        else
        {
            var mat = Application.isPlaying ? targetRenderer.material : targetRenderer.sharedMaterial;
            if (mat == null) return;
            mat.SetFloat(minYProp, minY);
            mat.SetFloat(maxYProp, maxY);
        }
    }
}