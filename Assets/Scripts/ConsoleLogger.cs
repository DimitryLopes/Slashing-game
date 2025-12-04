using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleLogger : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI logText;
    [SerializeField] private int maxLines = 15;
    [SerializeField] private Button copyLogButton;

    private string logContent = ""; 
    void OnEnable()
    {
        copyLogButton.onClick.AddListener(CopyLog);
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        copyLogButton.onClick.RemoveListener(CopyLog);
        Application.logMessageReceived -= HandleLog;
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        logContent += logString + "\n";

        var lines = logContent.Split('\n');
        if (lines.Length > maxLines)
        {
            logContent = string.Join("\n", lines, lines.Length - maxLines, maxLines);
        }

        logText.text = logContent;
    }

    private void CopyLog()
    {
        GUIUtility.systemCopyBuffer = logContent;
    }
}