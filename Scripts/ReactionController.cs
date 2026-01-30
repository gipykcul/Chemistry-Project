using UnityEngine;

public class ReactionController : MonoBehaviour
{
    public LiquidContainer beaker;

    [Header("Bubble prefab")]
    public GameObject bubblePrefab;

    [Header("Bubbles")]
    public float bubblesPerSecond = 5f;
    public float bubbleRiseSpeed = 0.12f;
    public float bubbleLifetime = 2.0f;

    [Header("Bubble spawn on gold ingot")]
    public Transform goldTransform;

    // Заглушки под размеры слитка (в локальных координатах goldTransform)
    // Заменишь потом на реальные числа.
    public float HHHHHHHHHH = 0f;   // высота нижней плоскости слитка (local Y)
    public float LLLLLLLLLL = 0.10f; // длина по X (local)
    public float WWWWWWWWWW = 0.05f; // ширина по Z (local)

    [Header("Gold dissolve (BlendShape)")]
    public SkinnedMeshRenderer goldRenderer;
    public int blendShapeIndex = 0;
    public float dissolveSeconds = 12f;

    float bubbleAcc;
    float dissolveT;

    void Update()
    {
        if (beaker == null) return;

        bool hasMixture = (beaker.hclMl > 1f && beaker.hno3Ml > 1f);

        if (hasMixture)
        {
            SpawnBubblesOnGold();
            DissolveGold();
        }
    }

    void SpawnBubblesOnGold()
    {
        if (bubblePrefab == null || goldTransform == null) return;

        bubbleAcc += bubblesPerSecond * Time.deltaTime;
        while (bubbleAcc >= 1f)
        {
            bubbleAcc -= 1f;

            // Случайная точка на прямоугольнике L x W
            float x = Random.Range(-LLLLLLLLLL * 0.5f, LLLLLLLLLL * 0.5f);
            float z = Random.Range(-WWWWWWWWWW * 0.5f, WWWWWWWWWW * 0.5f);

            // Нижняя плоскость слитка
            Vector3 localPos = new Vector3(x, HHHHHHHHHH, z);
            Vector3 worldPos = goldTransform.TransformPoint(localPos);

            var b = Instantiate(bubblePrefab, worldPos, Quaternion.identity);
            b.AddComponent<BubbleMove>().Init(bubbleRiseSpeed, bubbleLifetime);
        }
    }

    void DissolveGold()
    {
        if (goldRenderer == null) return;

        dissolveT += Time.deltaTime / Mathf.Max(0.1f, dissolveSeconds);
        float w = Mathf.Clamp01(dissolveT) * 100f;
        goldRenderer.SetBlendShapeWeight(blendShapeIndex, w);
    }
}

public class BubbleMove : MonoBehaviour
{
    float speed;
    float life;

    public void Init(float riseSpeed, float lifetime)
    {
        speed = riseSpeed;
        life = lifetime;
        Destroy(gameObject, life);
    }

    void Update()
    {
        transform.position += Vector3.up * speed * Time.deltaTime;
    }
}