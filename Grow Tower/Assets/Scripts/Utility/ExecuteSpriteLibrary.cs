#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;

public class ExecuteSpriteLibrary : MonoBehaviour
{

    void Start() => StartCoroutine(ExecuteScript());

    IEnumerator ExecuteScript()
    {
        var charfolder = "Assets/Packages/Characters";
        var folders = AssetDatabase.GetSubFolders(charfolder);
        foreach (var folder in folders)
        {
            //Debug.Log(folder);
            var genderFolder = AssetDatabase.GetSubFolders(folder);
            foreach (var gender in genderFolder)
            {
                //Debug.Log(gender);
                var animationFolder = AssetDatabase.GetSubFolders(gender);
                string name = folder.Substring(charfolder.Length + 1) + gender.Substring(folder.Length + 1, 1) + ".asset";
                //Debug.Log(name);
                SpriteLibraryAsset asset = ScriptableObject.CreateInstance<SpriteLibraryAsset>();
                var spriteFolder = AssetDatabase.GetSubFolders(gender);
                foreach (var spriteName in spriteFolder)
                {
                    //Debug.Log(spriteName);
                    //
                    var animName = spriteName.Substring(gender.Length + 1);
                    //Debug.Log(animName);

                    string[] spriteFiles = Directory.GetFiles(spriteName, "*.png");

                    List<Sprite> sprList = new List<Sprite>();

                    foreach (string sFile in spriteFiles)
                    {
                        //Directory.GetFiles uses backslash '\', thus need to change it back
                        string assetP = sFile.Replace('\\', '/');

                        Sprite sourceSpr = (Sprite)AssetDatabase.LoadAssetAtPath(assetP, typeof(Sprite));
                        sprList.Add(sourceSpr);
                    }
                    sprList.Sort((p1, p2) => int.Parse(p1.name).CompareTo(int.Parse(p2.name)));
                    foreach (Sprite sp in sprList) asset.AddCategoryLabel(sp, animName, sp.name);

                }
                //ACTIVATE ONLY DURING CREATION
                //AssetDatabase.CreateAsset(asset, "Assets/Settings/Tests/" + name);
                //AssetDatabase.SaveAssets();
            }
        }
        yield return null;
    }

}
#endif