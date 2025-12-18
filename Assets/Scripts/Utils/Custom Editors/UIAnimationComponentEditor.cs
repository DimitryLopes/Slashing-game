#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;

[CustomEditor(typeof(UIAnimationComponent))]
public class UIAnimationComponentEditor : Editor
{
    private SerializedProperty inAnimationsProperty;
    private SerializedProperty outAnimationsProperty;

    private bool isPreviewing = false;
    private GameObject targetObject;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 originalScale;
    private Color originalColor;

    private void OnEnable()
    {
        // Cache the serialized properties for inAnimations and outAnimations
        inAnimationsProperty = serializedObject.FindProperty("inAnimations");
        outAnimationsProperty = serializedObject.FindProperty("outAnimations");
        // Get the target object
        UIAnimationComponent animComponent = (target as UIAnimationComponent);
        targetObject = animComponent.gameObject;
        if (animComponent.AnimationTarget)
        {
            targetObject = animComponent.AnimationTarget;
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();


        // Draw 'In Animations'
        EditorGUILayout.LabelField("In Animations", EditorStyles.boldLabel);
        DrawAnimationsList(inAnimationsProperty);

        // Draw 'Out Animations'
        EditorGUILayout.LabelField("Out Animations", EditorStyles.boldLabel);
        DrawAnimationsList(outAnimationsProperty);

        EditorGUILayout.Space();
        // Target object settings
        EditorGUILayout.PropertyField(serializedObject.FindProperty("componentPriority"), new GUIContent("Priority"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("animationTarget"), new GUIContent("Custom animation target"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("playAllAtOnce"), new GUIContent("Play all at once", "Whether animations in the list will be played at once or one after the other"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("activateOnShow"), new GUIContent("Activate on Show"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("deactivateOnHide"), new GUIContent("Deactivate on Hide"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("affectedByGetComponent"), new GUIContent("Affected by GetComponent"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("overrideOtherAnimationsWhenPlayed"), new GUIContent("Override other animations when played"));
        
        // Preview controls
        EditorGUILayout.Space();
        if (!isPreviewing)
        {
            if (GUILayout.Button("Preview In Animations"))
            {
                StartPreview(true); // Preview 'In' animations
            }

            if (GUILayout.Button("Preview Out Animations"))
            {
                StartPreview(false); // Preview 'Out' animations
            }
        }
        else
        {
            if (GUILayout.Button("Stop Preview"))
            {
                StopPreview();
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// Draws the animation list in foldouts, including editing and previewing controls.
    /// </summary>
    private void DrawAnimationsList(SerializedProperty animationsProperty)
    {
        for (int i = 0; i < animationsProperty.arraySize; i++)
        {
            SerializedProperty animationProperty = animationsProperty.GetArrayElementAtIndex(i);

            // Create a box for each animation with mouse interaction
            EditorGUILayout.BeginVertical(GUI.skin.box);

            // Draw foldout for each animation
            animationProperty.isExpanded  = EditorGUILayout.BeginFoldoutHeaderGroup(animationProperty.isExpanded, $"{animationProperty.boxedValue}");

            // Check if the mouse is left-clicking in the area of the current animation
            Rect rect = GUILayoutUtility.GetLastRect(); // Get the area of the foldout header

            if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && rect.Contains(Event.current.mousePosition))
            {
                if (EditorUtility.DisplayDialog("Delete Animation", "Do you want to delete this animation?", "Delete", "Cancel"))
                {
                    // Delete the selected animation
                    animationsProperty.DeleteArrayElementAtIndex(i);
                    Event.current.Use(); // Consume the event to prevent further propagation
                    continue; // Skip the rest of the loop since the element was deleted
                }
            }

            if (animationProperty.isExpanded)
            {
                DrawAnimationFields(animationProperty);
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.EndVertical();
        }

        // Add/remove animation buttons
        if (GUILayout.Button("Add Animation"))
        {
            ShowAnimationTypeSelectionMenu(animationsProperty);
        }

        if (animationsProperty.arraySize > 0 && GUILayout.Button("Remove Last Animation"))
        {
            animationsProperty.DeleteArrayElementAtIndex(animationsProperty.arraySize - 1);
        }
    }

    /// <summary>
    /// Draws the fields for a specific animation instance.
    /// </summary>
    private void DrawAnimationFields(SerializedProperty animationProperty)
    {
        // Make a copy of the current property and store the end property
        SerializedProperty prop = animationProperty.Copy();
        SerializedProperty endProperty = animationProperty.GetEndProperty();

        // Iterate over the properties, ensuring we don't go beyond this object
        if (prop.NextVisible(true)) // Start from the first visible field (usually after script reference)
        {
            do
            {
                if (SerializedProperty.EqualContents(prop, endProperty))
                    break;

                // Draw each property in the current animation object
                EditorGUILayout.PropertyField(prop, true);

            } while (prop.NextVisible(false)); // Move to the next visible property
        }
    }


    /// <summary>
    /// Starts previewing the animations on the target object.
    /// </summary>
    private void StartPreview(bool isInAnimation)
    {
        if (targetObject == null)
        {
            Debug.LogError("Target object not set for preview.");
            return;
        }

        isPreviewing = true;

        // Save the original state of the target so we can reset it after the preview
        originalPosition = targetObject.transform.position;
        originalRotation = targetObject.transform.rotation;
        originalScale = targetObject.transform.localScale;
        Image img = targetObject.GetComponent<Image>();
        if(img) originalColor = img.color;

        // Start the preview using Editor Coroutines
        EditorApplication.update += OnEditorUpdate;

        // Play animations (inAnimations or outAnimations)
        var component = (UIAnimationComponent)target;
        if (isInAnimation)
        {
            component.PlayInAnimations(null, true);
        }
        else
        {
            component.PlayOutAnimations(null, true);
        }
    }

    /// <summary>
    /// Stops the preview and resets the target object to its original state.
    /// </summary>
    private void StopPreview()
    {
        isPreviewing = false;

        // Reset the position, rotation, and scale to the original state after the preview
        targetObject.transform.position = originalPosition;
        targetObject.transform.rotation = originalRotation;
        targetObject.transform.localScale = originalScale;
        var img = targetObject.GetComponent<Image>();
        if(img) img.color = originalColor;


        // Stop the Editor Coroutine
        EditorApplication.update -= OnEditorUpdate;

        // Cancel all LeanTween tweens on the target object
        LeanTween.cancel(targetObject);
    }

    /// <summary>
    /// Updates the editor every frame while previewing.
    /// </summary>
    private void OnEditorUpdate()
    {
        // Update LeanTween in the editor mode to animate the object
        LeanTween.update();

        // Optionally, more custom logic can be added here for real-time feedback
    }

    /// <summary>
    /// Shows a menu for selecting the type of animation to add to the list.
    /// </summary>
    private void ShowAnimationTypeSelectionMenu(SerializedProperty animationsProperty)
    {
        GenericMenu menu = new GenericMenu();

        // Get all subclasses of UIAnimation and display them in a menu
        var animationTypes = typeof(UIAnimation).Assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(UIAnimation)) && !t.IsAbstract);

        foreach (var type in animationTypes)
        {
            menu.AddItem(new GUIContent(type.Name), false, () => AddAnimation(animationsProperty, type));
        }

        menu.ShowAsContext();
    }

    /// <summary>
    /// Adds a new animation of the specified type to the list.
    /// </summary>
    private void AddAnimation(SerializedProperty animationsProperty, Type type)
    {
        // Create an instance of the selected animation type
        var newAnimation = Activator.CreateInstance(type) as UIAnimation;

        // Add a new element to the animations list
        animationsProperty.InsertArrayElementAtIndex(animationsProperty.arraySize);
        var newElement = animationsProperty.GetArrayElementAtIndex(animationsProperty.arraySize - 1);

        // Apply the new animation instance to the array using managed reference
        newElement.managedReferenceValue = newAnimation;

        serializedObject.ApplyModifiedProperties();
    }
}
#endif