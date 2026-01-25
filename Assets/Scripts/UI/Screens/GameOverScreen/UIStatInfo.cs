using TMPro;
using UnityEngine;

public class UIStatInfo : MonoBehaviour
{
    //I was gonna make thi dynamic with icon and name but it didn't seem necessary
    [SerializeField]
    private TextMeshProUGUI statText;

    public void SetStatValue(string value)
    {
        statText.text = value;
    }

}
