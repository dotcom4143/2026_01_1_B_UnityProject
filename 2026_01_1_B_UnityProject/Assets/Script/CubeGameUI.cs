using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CubeGameUI : MonoBehaviour
{
    public TextMeshProUGUI TimerText;
    public float Timer;


    void Start()
    {
        
    }

    void Update()
    {
        Timer += Time.deltaTime;
        TimerText.text = "생존 시간 : " + Timer.ToString("0.00");
    }
}
