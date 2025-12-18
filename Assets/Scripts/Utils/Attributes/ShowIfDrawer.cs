#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
[CustomPropertyDrawer(typeof(ShowIfAttribute))]
public class ShowIfDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Cast the attribute to access the fields
        ShowIfAttribute showIf = (ShowIfAttribute)attribute;

        // Evaluate the condition to see if the property should be shown
        bool shouldShow = ShouldShow(property, showIf);

        // Draw the property if the condition is met
        if (shouldShow)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ShowIfAttribute showIf = (ShowIfAttribute)attribute;

        // Determine if the property should be shown and set the height accordingly
        bool shouldShow = ShouldShow(property, showIf);
        return shouldShow ? EditorGUI.GetPropertyHeight(property, label, true) : 0f;
    }

    private bool ShouldShow(SerializedProperty property, ShowIfAttribute showIf)
    {
        // Attempt to find the condition property
        SerializedProperty conditionProperty = FindPropertyRelative(property, showIf.conditionField);

        if (conditionProperty == null)
        {
            Debug.LogWarning($"Field '{showIf.conditionField}' not found on '{property.serializedObject.targetObject}'. Trying reflection...");

            // Use reflection as a fallback to find the field
            var targetObject = property.serializedObject.targetObject;
            var targetField = targetObject.GetType().GetField(showIf.conditionField,
                                System.Reflection.BindingFlags.Instance |
                                System.Reflection.BindingFlags.NonPublic |
                                System.Reflection.BindingFlags.Public);

            if (targetField != null)
            {
                var fieldValue = targetField.GetValue(targetObject);
                return EvaluateCondition(fieldValue, showIf.compareValues);
            }

            // If we still can't find the property, return true (always show)
            return true;
        }

        // Evaluate the condition if the property was found
        return EvaluateCondition(conditionProperty, showIf.compareValues);
    }

    private SerializedProperty FindPropertyRelative(SerializedProperty property, string propertyName)
    {
        // Look for the property in the current serialized object
        SerializedProperty foundProperty = property.serializedObject.FindProperty(propertyName);
        if (foundProperty != null)
        {
            return foundProperty;
        }

        // Try to search within the relative hierarchy of the current property (if it's a class or struct)
        var path = property.propertyPath.Contains(".")
                   ? property.propertyPath.Substring(0, property.propertyPath.LastIndexOf(".")) + "." + propertyName
                   : propertyName;

        return property.serializedObject.FindProperty(path);
    }

    private bool EvaluateCondition(object conditionValue, object[] compareValues)
    {
        if (conditionValue is bool boolValue)
        {
            foreach(var compareValue in compareValues)
            {
                if (boolValue.Equals(compareValue))
                {
                    return true;
                }
            }
        }
        if (conditionValue is int intValue)
        {
            foreach(var compareValue in compareValues)
            {
                if (intValue.Equals(compareValue))
                {
                    return true;
                }
            }
        }

        // Add more type checks here if needed (e.g., for floats, strings)

        Debug.LogWarning("Unsupported property type for ShowIf condition.");
        return true;
    }

    private bool EvaluateCondition(SerializedProperty conditionProperty, object[] compareValues)
    {
        // If compareValues is null or empty, just check the condition property itself
        if (compareValues == null || compareValues.Length == 0)
        {
            switch (conditionProperty.propertyType)
            {
                case SerializedPropertyType.Boolean:
                    return conditionProperty.boolValue;
                case SerializedPropertyType.Enum:
                    return conditionProperty.enumValueIndex != 0; // Return true if it's not the first enum value
                default:
                    Debug.LogWarning("Unsupported property type for ShowIf condition.");
                    return true;
            }
        }

        // If compareValues is provided, compare the condition property against it
        switch (conditionProperty.propertyType)
        {
            case SerializedPropertyType.Boolean:
                foreach(var compareValue in compareValues)
                {
                    if (conditionProperty.boolValue.Equals(compareValue))
                    {
                        return true;
                    }
                }
                return false;
            case SerializedPropertyType.Enum:
                foreach(var compareValue in compareValues)
                {
                    if (conditionProperty.enumValueIndex.Equals((int)compareValue))
                    {
                        return true;
                    }
                }
                return false;
            default:
                Debug.LogWarning("Unsupported property type for ShowIf condition.");
                return true;
        }
    }
}
#endif