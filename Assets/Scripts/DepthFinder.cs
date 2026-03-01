using TMPro;
using UnityEngine;

public class DepthFinder : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float depthScale = 2f;
    [SerializeField, Range(0, 5)] private int roundDecimals = 1;

    private float startY;
    private bool initialized;
    static float savedDepth = 0f;
    float depth = 0f;

    void Start()
    {
        if (player == null)
        {
            Debug.LogWarning("DepthFinder: Player transform is not assigned.");
            initialized = false;
            return;
        }

        startY = player.position.y;
        initialized = true;

        if (savedDepth > 0)
        {
            startY = savedDepth / 2;
        }

        if (text == null)
            Debug.LogWarning("DepthFinder: text (TextMeshProUGUI) is not assigned.");
    }

    void LateUpdate()
    {
        if (!initialized || player == null || text == null)
            return;

        float playerY = player.position.y;
        depth = (startY - playerY) * depthScale;
        depth = Mathf.Max(0f, depth); // prevent negative depth if player goes above start
        if (depth > 0f && depth < 500f)
        {
            savedDepth = depth + 490;
        }

        float pow = Mathf.Pow(10f, roundDecimals);
        float rounded = Mathf.Round(depth * pow) / pow;

        text.text = "Depth: " + rounded.ToString("F" + roundDecimals) + "m";
    }
}
