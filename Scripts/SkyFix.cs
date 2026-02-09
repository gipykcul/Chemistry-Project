using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

[DefaultExecutionOrder(-10000)]
public class ForceHdrpCameraClearToBlack : MonoBehaviour
{
    void Awake() => Apply();
    void OnEnable() => Apply();

    void Apply()
    {
        var cam = GetComponent<Camera>();
        if (!cam) return;

        var hd = GetComponent<HDAdditionalCameraData>();
        if (!hd) hd = gameObject.AddComponent<HDAdditionalCameraData>();

        hd.clearColorMode = HDAdditionalCameraData.ClearColorMode.Color;
        hd.backgroundColorHDR = Color.black;

        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;
    }
}