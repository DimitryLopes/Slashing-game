using UnityEngine;

public class ShowIfAttribute : PropertyAttribute
{
    public string conditionField;
    public object[] compareValues;

    public ShowIfAttribute(string conditionField, params object[] compareValues)
    {
        this.conditionField = conditionField;
        this.compareValues = compareValues;
    }
}