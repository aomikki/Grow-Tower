using UnityEditor;
using UnityEngine;

public class EditorCustomGameObject : Editor
{

    [MenuItem("Custom Editor/ActiveToggle _`")]
    static void ToggleActivationSelection()
    {
        if (Selection.gameObjects != null)
        {
            if (Selection.gameObjects.Length > 1)
            {
                var go = Selection.gameObjects;
                for (int i = 0; i < go.Length; i++)
                {
                    if (go != null)
                        go[i].SetActive(!go[i].activeSelf);
                }
                Undo.RegisterCompleteObjectUndo(go, "Set Active Multiple GO");
            }
            else
            {
                var go = Selection.activeGameObject;
                if (go != null)
                {
                    go.SetActive(!go.activeSelf);
                    Undo.RegisterCompleteObjectUndo(go, "Set Active -> " + go.name);
                }
            }
        }
    }

    //ALT+Tilde to set position on Selected GameObject to Zero
    [MenuItem("Custom Editor/PositionZero &_`")]
    static void PositionToZero()
    {
        if (Selection.gameObjects != null)
        {
            if(Selection.gameObjects.Length > 1)
            {
                var go = Selection.gameObjects;
                foreach (GameObject g in go)
                {
                    if (go != null)
                        g.transform.position = Vector3.zero;
                }
                Undo.RegisterCompleteObjectUndo(go, "Set Multiple Position GO to Zero");
            }
            else
            {
                var go = Selection.activeGameObject;
                if(go != null)
                {
                    go.transform.position = Vector3.zero;
                    Undo.RegisterCompleteObjectUndo(go, "Set Position Zero -> " + go.name);
                }
            }
        }
    }

    //SHIFT+Tilde to Auto Anchoring the UI
    //Credits to MAENTRUS
    [MenuItem("Custom Editor/Auto Anchor #_`")]
    static void uGUIAnchorAroundObject()
    {
        var o = Selection.activeGameObject;
        if (o != null && o.GetComponent<RectTransform>() != null)
        {
            var r = o.GetComponent<RectTransform>();
            var p = o.transform.parent.GetComponent<RectTransform>();

            var offsetMin = r.offsetMin;
            var offsetMax = r.offsetMax;
            var _anchorMin = r.anchorMin;
            var _anchorMax = r.anchorMax;

            var parent_width = p.rect.width;
            var parent_height = p.rect.height;

            var anchorMin = new Vector2(_anchorMin.x + (offsetMin.x / parent_width),
                                        _anchorMin.y + (offsetMin.y / parent_height));
            var anchorMax = new Vector2(_anchorMax.x + (offsetMax.x / parent_width),
                                        _anchorMax.y + (offsetMax.y / parent_height));

            r.anchorMin = anchorMin;
            r.anchorMax = anchorMax;

            r.offsetMin = new Vector2(0, 0);
            r.offsetMax = new Vector2(0, 0);
            r.pivot = new Vector2(0.5f, 0.5f);

            Undo.RegisterCompleteObjectUndo(o, "Auto Anchor -> " + o.name);

        }
    }

    [MenuItem("Custom Editor/Clear PlayerPrefs #_p")]
    static void ClearData()
    {
        PlayerPrefs.DeleteAll();
    }

    [MenuItem("Custom Editor/Count Selection %#&a")]
    static void CountSelection()
    {
        if (Selection.gameObjects != null)
        {
            if (Selection.gameObjects.Length > 1)
            {
                var go = Selection.gameObjects;
                int flag = 0;
                for (int i = 0; i < go.Length; i++)
                {
                    if (go[i] != null)
                        flag++;
                }
                //AoLog.D(flag.ToString() + " GameObjects Selected");
            }
            else
            {
                var go = Selection.activeGameObject;
                if (go != null)
                {
                    //AoLog.D("1 GameObject Selected");
                }
            }
        }
    }

    //CTRL+SHIFT+Tilde to Create empty GameObject
    [MenuItem("Custom Editor/Create EmptyGO %#`", false, 10)]
    static void CreateCustomGameObject(MenuCommand menuCommand)
    {
        // Create a custom game object
        GameObject go = new GameObject("GameObject");
        go.transform.position = Vector3.zero;
        // Ensure it gets reparented if this was a context click (otherwise does nothing)
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        if (Selection.activeGameObject != null)
            go.transform.SetParent(Selection.activeGameObject.transform);
        Selection.activeObject = go;
    }
    
}