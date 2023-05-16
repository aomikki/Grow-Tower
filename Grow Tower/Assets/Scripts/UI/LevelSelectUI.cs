using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Enums;

public class LevelSelectUI : MonoBehaviour
{

    //[SerializeField] TMPro.TextMeshProUGUI titleText = null;

    [SerializeField] Transform levelContainer = null; //in the future, going to list it and set one by one to enable swipe scroll options?
    Transform ulContainer;

    List<GameObject> mainLevelButtons = new List<GameObject>();
    List<GameObject> customLevelButtons = new List<GameObject>();

    bool isInit = false;

    [SerializeField] Button backBtn = null;

    public void Init()
    {
        if (isInit) return;
        GameObject lContainer = new GameObject("Unused Level Container");
        lContainer.transform.SetParent(transform);
        lContainer.transform.localPosition = Vector3.zero;
        lContainer.transform.localScale = Vector3.one;
        lContainer.SetActive(false);
        //lContainer.hideFlags = HideFlags.HideInHierarchy;
        ulContainer = lContainer.transform;

        if (backBtn) backBtn.onClick.AddListener(Back);

        isInit = true;
    }

    //this one checks the data that exists in game before this session
    public void CreateExistingLevels()
    {
        var datas = GameManager.Instance.LevelDatas;
        foreach (LevelData ld in datas) CreateNewLevel(ld);
        //main level sort because it's static, happening once, and only we should able to control it.
        mainLevelButtons.Sort((p1, p2) => int.Parse(p1.name.Split(' ')[1]).CompareTo(int.Parse(p2.name.Split(' ')[1])));
    }

    //this one if someone made a new custom map, it's not yet registered
    public void CreateNewLevel(LevelData data)
    {
        GameObject go = null;
        ResourcesManager.Instance.InstantiateAssets("UI", "LevelBtn", x => go = x, ulContainer);
        var sc = go.GetComponent<LevelDataUI>();
        sc.Assign(data);
        if (string.Equals(sc.levelType, "main", System.StringComparison.OrdinalIgnoreCase)) mainLevelButtons.Add(go);
        else customLevelButtons.Add(go);
    }

    //gotta find another way to save the new custom map, but for now, finish the instantiation first

    public void GetLevel(int state)
    {
        ReturnToBox();
        switch ((MODE)state)
        {
            case MODE.MAIN:
                foreach (GameObject g in mainLevelButtons)
                {
                    g.transform.SetParent(levelContainer);
                    var sc = g.GetComponent<LevelDataUI>();
                    var n = GameManager.Instance.MainCleared;
                    if (sc.MainLevelNumber <= n + 1) sc.IsInteractable(true);
                    else sc.IsInteractable(false);
                }
                break;
            case MODE.CUSTOM:
                foreach (GameObject g in customLevelButtons) g.transform.SetParent(levelContainer);
                break;
        }
    }

    //please return it when not used
    public void ReturnToBox() { while (levelContainer.childCount > 0) levelContainer.GetChild(0).SetParent(ulContainer); }

    void Back()
    {
        ReturnToBox();
        UIManager.Instance.mainMenu.SetActive(true);
        gameObject.SetActive(false);
    }

}
