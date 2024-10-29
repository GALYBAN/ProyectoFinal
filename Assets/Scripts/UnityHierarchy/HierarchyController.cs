using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class HierarchyController : Editor
{
    public static Color DEFAULT_COLOR_HIERARCHY_SELECTED = Color.green;
    static HierarchyController()
    {
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
    }

    private static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
    {
        GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

        Color backgroundColor = DEFAULT_COLOR_HIERARCHY_SELECTED;

        if (gameObject != null && gameObject.CompareTag("EditorOnly"))
        {
            gameObject.name = gameObject.name.ToUpper();

            HierarchyHeaderEditor header = gameObject.GetComponent<HierarchyHeaderEditor>();

            if (header != null && Event.current.type == EventType.Repaint)
            {
                backgroundColor = header.backgroundColor;
                EditorGUI.DrawRect(selectionRect, backgroundColor); 
                EditorGUI.DropShadowLabel(selectionRect, gameObject.name);
            }
            else
            {
                EditorGUI.DrawRect(selectionRect, backgroundColor);
                EditorGUI.DropShadowLabel(selectionRect, gameObject.name);
            }

            
        }
       
            EditorApplication.RepaintHierarchyWindow();

    }

}
