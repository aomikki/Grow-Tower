using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : SingletonMonoBehaviour<UIManager>
{

    public Transform canvas;

    [HideInInspector]
    public GameObject loading, mainMenu, credits, levelSelect, inGame, tutorial, gameVerdict = null;

    [HideInInspector] public LoadingType sc_Loading;
    [HideInInspector] public MainMenuUI sc_MainMenu;
    [HideInInspector] public LevelSelectUI sc_LevelSelect;
    [HideInInspector] public TutorialText sc_Tutorial;
    [HideInInspector] public GameVerdictUI sc_GameVerdict;

    enum STATE
    {
        MAINMENU = 0,
        INGAME = 1,
        PAUSED = 2,
        VERDICT = 3
    }

    public bool IsPaused { get; private set; }

    public void StateUpdate(int state)
    {
        if (loading.activeInHierarchy) sc_Loading.StopLoad();
        switch ((STATE)state)
        {
            case STATE.MAINMENU:
                if (IsPaused) IsPaused = false;
                mainMenu.SetActive(true);
                levelSelect.SetActive(false);
                inGame.SetActive(false);
                gameVerdict.SetActive(false);
                sc_Tutorial.CloseTutorial();
                break;
            case STATE.INGAME:
                if (mainMenu.activeSelf) mainMenu.SetActive(false);
                if (levelSelect.activeSelf)
                {
                    sc_LevelSelect.ReturnToBox();
                    levelSelect.SetActive(false);
                }
                if (!inGame.activeSelf) inGame.SetActive(true);
                if (gameVerdict.activeSelf) gameVerdict.SetActive(false);
                //for now get the first level by index
                break;
            case STATE.PAUSED:
                IsPaused = !IsPaused;
                if (!IsPaused) StateUpdate(1);
                break;
            case STATE.VERDICT:
                gameVerdict.SetActive(true);
                break;
        }
    }

    private void Start()
    {
        gameObject.name = "(SINGLETON) UI Manager";
        DontDestroyOnLoad(this);
    }

    public void LoadUI()
    {
        loading = canvas.GetChild(0).gameObject;
        sc_Loading = loading.GetComponent<LoadingType>();
        ResourcesManager.Instance.InstantiateAssets("UI", "MainMenu", x => mainMenu = x, canvas);
        sc_MainMenu = mainMenu.GetComponent<MainMenuUI>();
        mainMenu.SetActive(false);
        ResourcesManager.Instance.InstantiateAssets("UI", "Credits", x => credits = x, canvas);
        credits.SetActive(false);
        ResourcesManager.Instance.InstantiateAssets("UI", "LevelSelect", x => levelSelect = x, canvas);
        sc_LevelSelect = levelSelect.GetComponent<LevelSelectUI>();
        sc_LevelSelect.Init();
        levelSelect.SetActive(false);
        ResourcesManager.Instance.InstantiateAssets("UI", "Tutorial", x => tutorial = x, canvas);
        sc_Tutorial = tutorial.GetComponent<TutorialText>();
        tutorial.SetActive(false);
        ResourcesManager.Instance.InstantiateAssets("UI", "InGame", x => inGame = x, canvas, false);
        ResourcesManager.Instance.InstantiateAssets("UI", "GameVerdict", x => gameVerdict = x, canvas);
        sc_GameVerdict = gameVerdict.GetComponent<GameVerdictUI>();
        gameVerdict.SetActive(false);
    }

}
