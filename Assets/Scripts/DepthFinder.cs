using TMPro;
using UnityEngine;

public class DepthFinder : MonoBehaviour
{
    public Transform Player;
    public TextMeshProUGUI text;
    float startY;
    float currentY;
    float fakeY;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startY = Player.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        currentY = Player.position.y;
        fakeY = startY - currentY;
        fakeY = fakeY * 2;
        text.text = "Depth: " + fakeY.ToString() + "M";
    }
}
