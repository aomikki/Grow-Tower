using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Enums;

public class GameVerdictUI : MonoBehaviour
{

    [SerializeField] GameObject winGO, loseGO = null;
    [Space(10f)]
    [SerializeField] Button[] returnBtn;
    [SerializeField] Button nextBtn, retryBtn = null;
    [SerializeField] TMPro.TextMeshProUGUI loseMessage = null;

    private void Awake()
    {
        if (!loseMessage) loseMessage = GetComponentInChildren<TMPro.TextMeshProUGUI>();
    }

    // Start is called before the first frame update
    void Start()
    {
        nextBtn.onClick.AddListener(Next);
        retryBtn.onClick.AddListener(Retry);
        if (returnBtn.Length > 0)
            foreach (var item in returnBtn)
                item.onClick.AddListener(ReturnMenu);
    }

    public void GameVerdict(bool isWin)
    {
        gameObject.SetActive(true);
        DeactiveGO(false);
        if (isWin)
        {
            winGO.SetActive(true);
        }
        else
        {
            loseGO.SetActive(true);
        }
    }

    public void LoseVerdict(string messages = "just like the ads, lol")
    {
        GameVerdict(false);
        loseMessage.text = $"FAIL\n<size=20%>{messages}";
    }

    void DeactiveGO(bool includeParent)
    {
        winGO.SetActive(false);
        loseGO.SetActive(false);
        gameObject.SetActive(!includeParent);
    }

    void Next()
    {
        //gets the next level GO
        if (GameManager.Instance.gameMode.Equals(MODE.MAIN))
        {
            if (GameManager.Instance.EndOfMainLevel) ReturnLevelSelect();
            else GameManager.Instance.BuildNextLevel();
        }
        else ReturnLevelSelect();
        DeactiveGO(true);
    }

    void Retry()
    {
        //reset level
        GameManager.Instance.ReloadLevel();
        DeactiveGO(true);
    }

    void ReturnMenu()
    {
        GameManager.Instance.DemolishLevel();
        UIManager.Instance.StateUpdate(0);
        DeactiveGO(true);
    }

    void ReturnLevelSelect()
    {
        ReturnMenu();
        //temporarily on 0 because we just make main...
        UIManager.Instance.sc_MainMenu.CallLevel(0);
    }

}
