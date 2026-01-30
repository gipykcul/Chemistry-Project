using UnityEngine;

public class PouringBottle : MonoBehaviour
{
    public LiquidContainer bottle;
    public Transform spout;
    public GameObject pourStreamPrefab;

    [Header("Pouring")]
    public float emptyTimeSeconds = 5f;
    public float startPourFill01AtNeck = 0.92f;

    GameObject streamInstance;
    float MlPerSecond => bottle.capacityMl / Mathf.Max(0.1f, emptyTimeSeconds);

    void Update()
    {
        if (bottle == null || spout == null) return;

        bool shouldPour = bottle.fill01 >= startPourFill01AtNeck && bottle.CurrentMl > 0.01f;

        if (shouldPour)
        {
            if (streamInstance == null && pourStreamPrefab != null)
                streamInstance = Instantiate(pourStreamPrefab, spout.position, spout.rotation);

            if (streamInstance != null)
                streamInstance.transform.SetPositionAndRotation(spout.position, spout.rotation);

            float remove = MlPerSecond * Time.deltaTime;
            bottle.RemoveLiquid(remove, out float outHcl, out float outHno3);

            if (streamInstance != null && streamInstance.TryGetComponent(out PourStream stream))
                stream.SetComposition(outHcl, outHno3, remove);
        }
        else
        {
            if (streamInstance != null)
            {
                Destroy(streamInstance);
                streamInstance = null;
            }
        }
    }
}