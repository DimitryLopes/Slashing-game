using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILatency : MonoBehaviour
{
    public const string LATENCY_TEXT_FORMAT = "{0} ms";
    public const float LATENCY_BEST_THRESHOLD = 50f;
    public const float LATENCY_GOOD_THRESHOLD = 100f;
    public const float LATENCY_AVERAGE_THRESHOLD = 150f;
    public const float LATENCY_BAD_THRESHOLD = 200f;
    public const float LATENCY_TERRIBLE_THRESHOLD = 250f;

    [SerializeField]
    private TextMeshProUGUI latencyText;
    [SerializeField]
    private Image latencyIconFill;


    public void UpdateLatency(float latency)
    {
        latencyText.text = string.Format(LATENCY_TEXT_FORMAT, latency);
        
    //    switch (latency)
    //    {
    //        case float l when l <= LATENCY_BEST_THRESHOLD:
    //            latencyIconFill.color = Color.cyan;
    //            break;
    //        case float l when l <= LATENCY_GOOD_THRESHOLD:
    //            latencyIconFill.color = Color.green;
    //            break;
    //        case float l when l <= LATENCY_AVERAGE_THRESHOLD:
    //            latencyIconFill.color = Color.yellow;
    //            break;
    //        case float l when l <= LATENCY_BAD_THRESHOLD:
    //            latencyIconFill.color = new Color(1f, 0.65f, 0f); // Orange;
    //            break;
    //        case float l when l <= LATENCY_TERRIBLE_THRESHOLD:
    //            latencyIconFill.color = new Color(0.75f, 0f, 0f); // Dark Red
    //            break;
    //        default:
    //            latencyIconFill.color = Color.black;
    //            break;
    //    }
    //}
}
