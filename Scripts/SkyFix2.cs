using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

[ExecuteAlways]
public class ForceHDRPCameraBlackClear : MonoBehaviour
{
    public Camera targetCamera;

    void OnEnable() => Apply();
    void Update() { if (!Application.isPlaying) Apply(); }

    void Apply()
    {
        var cam = targetCamera ? targetCamera : Camera.main;
        if (!cam) return;

        var hd = cam.GetComponent<HDAdditionalCameraData>();
        if (!hd) hd = cam.gameObject.AddComponent<HDAdditionalCameraData>();

        hd.clearColorMode = HDAdditionalCameraData.ClearColorMode.Color;
        hd.backgroundColorHDR = Color.black;
        hd.clearDepth = true;

        // На всякий случай и стандартную камеру тоже:
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;
    }
}