using UnityEngine;

public class LiquidReceiver : MonoBehaviour
{
    public LiquidContainer receiver;

    void OnTriggerStay(Collider other)
    {
        if (receiver == null) return;

        if (other.TryGetComponent(out PourStream stream))
        {
            if (stream.frameMl > 0.0001f)
            {
                receiver.AddLiquid(stream.frameMl, stream.frameHclMl, stream.frameHno3Ml);
                stream.ConsumeFramePortion();
            }
        }
    }
}