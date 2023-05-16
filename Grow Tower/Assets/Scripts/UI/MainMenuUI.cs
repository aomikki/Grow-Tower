using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{

    [SerializeField] Button playBtn, quitBtn, mainlvlBtn, customlvlBtn, creditsBtn = null;

    [SerializeField] GameObject mainSect, playSect = null;

    void Start()
    {
        playBtn.onClick.AddListener(Play);
        quitBtn.onClick.AddListener(Quit);
        creditsBtn.onClick.AddListener(Credits);
        mainlvlBtn.onClick.AddListener(MainLvl);
        customlvlBtn.onClick.AddListener(CustomLvl);
#if UNITY_WEBGL
        quitBtn.gameObject.SetActive(false);
#endif
    }

    void Play()
    {
        //gotta put this temporary to main level first
        //SwitchSection(false);
        CallLevel(0);
    }

    void Quit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    void Credits() =>
        UIManager.Instance.credits.SetActive(true);

    void MainLvl() => CallLevel(0);

    void CustomLvl() => CallLevel(1);

    void SwitchSection(bool isMain)
    {
        mainSect.SetActive(isMain);
        playSect.SetActive(!isMain);
    }

    public void CallLevel(int state)
    {
        UIManager.Instance.levelSelect.SetActive(true);
        UIManager.Instance.sc_LevelSelect.GetLevel(state);
        GameManager.Instance.gameMode = (Enums.MODE)state;
        SwitchSection(true);
        gameObject.SetActive(false);
    }

}
