using UnityEngine;

public class PourStream : MonoBehaviour
{
    public float frameMl;
    public float frameHclMl;
    public float frameHno3Ml;

    public void SetComposition(float hcl, float hno3, float total)
    {
        frameMl = total;
        frameHclMl = hcl;
        frameHno3Ml = hno3;
    }

    public void ConsumeFramePortion()
    {
        frameMl = 0f;
        frameHclMl = 0f;
        frameHno3Ml = 0f;
    }
}