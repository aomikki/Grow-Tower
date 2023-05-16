using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Enums;

public class GameManager : SingletonMonoBehaviour<GameManager>
{

    [HideInInspector]
    public GameObject gameContent, objectPool, player = null;
    [HideInInspector] public Transform playerTower, enemyTower;

    [HideInInspector]
    public GameObject[] floors, enemies;

    Transform floorPool, playerPool, enemyPool;
    Transform enemyFloor, normalFloor, topFloor;

    public Tower s_playerTower { get; private set; }
    PlayerTower s_playerTowerEx;
    public Tower s_enemyTower { get; private set; }
    Player s_Player;
    Enemy[] s_Enemy;

    LevelData data; //set current level data
    //[HideInInspector]
    public List<LevelData> LevelDatas { get; private set; }
    LevelData[] mainLevels; //the reason is because main levels are what we add and no user should ever change it

    public int TotalMainLevels { get => mainLevels.Length; }
    public int CurrentMainLevel
    {
        get
        {
            if (gameMode.Equals(MODE.CUSTOM)) return -1;
            else return System.Array.IndexOf(mainLevels, data);
        }
    }

    public int MainCleared { get; private set; }
    public bool EndOfMainLevel { get => CurrentMainLevel.Equals(TotalMainLevels - 1); }

    [TextArea(3, 10)]
    [SerializeField] string[] tutorialTexts;

    public MODE gameMode;
    public RESTRICTION restriction;

    Coroutine coroutine;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.name = "(SINGLETON) Game Manager";
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region LEVEL

    public void BuildLevel(LevelData data)
    {
        this.data = data;
        BuildLevel();
    }

    public void BuildNextLevel()
    {
        if (gameMode.Equals(MODE.CUSTOM)) return;
        DemolishLevel();
        int n = CurrentMainLevel;
        n++;
        data = mainLevels[n];
        BuildLevel();
    }

    public void BuildLevel()
    {
        if (!data) return;
        //Player Tower First, since it's easier to detect
        GetFloor(playerTower, "", true);
        for (int i = 1; i < data.playerTower; i++)
            GetFloor(playerTower);
        s_playerTower.SetFloor();
        SetPlayerOnTopFloor();
        s_Player.power.Strength = data.playerStrength;
        player.SetActive(true);

        restriction = data.restriction;

        //Now Enemy Tower
        for (int i = 0; i < data.enemyTower.Count; i++)
            GetFloor(enemyTower, "Enemy");
        s_enemyTower.SetFloor();
        int count = 0;
        foreach (EnemyTowerDetails etd in data.enemyTower)
        {
            for (int i = 0; i < etd.enemies.Length; i++)
                GetEnemy(s_enemyTower.floors[count].transform, etd.enemies[i]);
            s_enemyTower.floors[count].SetPosition();
            count++;
        }
        s_playerTowerEx.Enable(true);

        if (tutorialTexts.Length > 0)
        {
            switch (CurrentMainLevel)
            {
                case 0:
                    UIManager.Instance.sc_Tutorial.AssignTutorial(tutorialTexts[0]);
                    break;
                case 5:
                    UIManager.Instance.sc_Tutorial.AssignTutorial(tutorialTexts[1]);
                    break;
                case 10:
                    UIManager.Instance.sc_Tutorial.AssignTutorial(tutorialTexts[2]);
                    break;
            }
        }
    }

    public void DemolishLevel()
    {
        s_playerTowerEx.Enable(false);
        ReturnPlayer();
        int top = s_playerTower.floors.Count - 1;
        int i = s_playerTower.floors.Count;
        while (i > 0)
        {
            --i;
            if (i.Equals(top)) ReturnFloor(playerTower, i, "", true);
            else ReturnFloor(playerTower, i, "", false);
        }
        i = s_enemyTower.floors.Count;
        while (i > 0)
        {
            --i;
            for (int j = s_enemyTower.floors[i].transform.childCount - 1; j >= 1; j--)
                ReturnEnemy(s_enemyTower.floors[i].transform, j);
            ReturnFloor(enemyTower, i, "Enemy");
        }
        restriction = 0;
    }

    public void CheckLevelClear()
    {
        if (s_enemyTower.floors.Count.Equals(0))
        {
            if (s_playerTower.floors.Count >= 5)
            {
                UIManager.Instance.sc_GameVerdict.GameVerdict(true);
                if (MainCleared < CurrentMainLevel + 1)
                {
                    MainCleared = CurrentMainLevel + 1;
                    PlayerPrefs.SetInt("MainCleared", MainCleared);
                }
            }
            else UIManager.Instance.sc_GameVerdict.LoseVerdict("God of Tower consider a tower to be at least 5 floors");
        }
    }

    #endregion

    #region PLAYER

    public float PlayerPower 
    { 
        get { return s_Player.power.Strength; }
        set { s_Player.power.Strength = value; } 
    }

    public bool PlayerGrabbed { get { return s_Player.IsGrabbed; } }

    public void SetPlayerOnTopFloor()
    {
        player.transform.SetParent(s_playerTower.floors[s_playerTower.floors.Count - 1].transform);
        player.transform.localPosition = new Vector3(0, -1.1f);
    }

    public void ReturnPlayer()
    {
        player.transform.SetParent(playerPool);
        player.transform.localPosition = Vector3.zero;
        player.SetActive(false);
    }

    #endregion

    #region FLOOR

    public void GetFloor(Transform targetParent, string sides = "", bool isTopTower = false)
    {
        if (floorPool.childCount.Equals(0)) return;
        Transform t;
        if (isTopTower) t = topFloor.GetChild(0);
        else if (sides.Equals("Enemy")) t = enemyFloor.GetChild(0);
        else t = normalFloor.GetChild(0);
        t.SetParent(targetParent);
        t.SetAsFirstSibling();
        var tower = targetParent.GetComponent<Tower>();
        var floor = t.GetComponent<Floor>();
        tower.floors.Insert(0, floor);
        t.gameObject.SetActive(true);
    }

    public void ReturnFloor(Transform originParent, int index, string sides = "", bool isTopTower = false)
    {
        if (originParent.childCount.Equals(0)) return;
        Transform t = originParent.GetChild(index);
        var tower = originParent.GetComponent<Tower>();
        var floor = t.GetComponent<Floor>();
        tower.floors.Remove(floor);
        if (isTopTower) t.SetParent(topFloor);
        else if (sides.Equals("Enemy")) t.SetParent(enemyFloor);
        else t.SetParent(normalFloor);
        t.localPosition = Vector3.zero;
        t.gameObject.tag = "Untagged";
        t.gameObject.SetActive(false);
    }

    #endregion

    #region ENEMY

    public void GetEnemy(Transform targetFloor, float strength)
    {
        if (enemyPool.childCount.Equals(0)) return;
        Transform t = enemyPool.GetChild(0);
        t.SetParent(targetFloor);
        var script = t.GetComponent<Enemy>();
        script.power.Strength = strength;
        t.gameObject.SetActive(true);
    }

    public void ReturnEnemy(Transform originFloor, int index)
    {
        if (originFloor.childCount <= 1) return;
        Transform t = originFloor.GetChild(index);
        t.SetParent(enemyPool);
        t.localPosition = Vector3.zero;
        t.gameObject.SetActive(false);
    }

    #endregion

    #region LOADING

    public void LoadContent() => coroutine = StartCoroutine(_loadContent());

    public void ReloadLevel() => coroutine = StartCoroutine(_resetLevel());

    public void StopCoroutine() => StopCoroutine(coroutine);

    IEnumerator _resetLevel()
    {
        DemolishLevel();
        yield return new WaitForFixedUpdate();
        BuildLevel();
        yield return new WaitForFixedUpdate();
    }

    IEnumerator _loadContent()
    {
        ResourcesManager.Instance.InstantiateAssets("Prefabs", "GameContent", x => gameContent = x);
        playerTower = gameContent.transform.GetChild(0);
        s_playerTower = playerTower.GetComponent<Tower>();
        s_playerTower.SetTags("Self");
        s_playerTowerEx = playerTower.gameObject.AddComponent<PlayerTower>();
        s_playerTowerEx.Enable(false);
        yield return new WaitForFixedUpdate();
        enemyTower = gameContent.transform.GetChild(1);
        s_enemyTower = enemyTower.GetComponent<Tower>();
        s_enemyTower.SetTags("Enemy");
        yield return new WaitForFixedUpdate();

        objectPool = new GameObject("ObjectPool");
        //objectPool.hideFlags = HideFlags.HideInHierarchy;
        GameObject a = new GameObject("FloorPool");
        GameObject b = new GameObject("PlayerPool");
        GameObject c = new GameObject("EnemyPool");
        floorPool = a.transform;
        playerPool = b.transform;
        enemyPool = c.transform;
        floorPool.SetParent(objectPool.transform);
        playerPool.SetParent(objectPool.transform);
        enemyPool.SetParent(objectPool.transform);
        yield return new WaitForFixedUpdate();

        enemyFloor = new GameObject("Enemy Floor").transform;
        enemyFloor.SetParent(floorPool);
        normalFloor = new GameObject("Normal Floor").transform;
        normalFloor.SetParent(floorPool);
        topFloor = new GameObject("Top Floor").transform;
        topFloor.SetParent(floorPool);
        yield return new WaitForFixedUpdate();

        floors = new GameObject[100]; //Object pool constant setup
        for (int i = 0; i < floors.Length / 2; i++)
        {
            ResourcesManager.Instance.InstantiateAssets("Prefabs", "EnemyFloor", x => floors[i] = x, enemyFloor, false);
            yield return new WaitForFixedUpdate();
        }
        for (int i = floors.Length / 2; i < floors.Length - 1; i++)
        {
            ResourcesManager.Instance.InstantiateAssets("Prefabs", "Floor", x => floors[i] = x, normalFloor, false);
            yield return new WaitForFixedUpdate();
        }
        ResourcesManager.Instance.InstantiateAssets("Prefabs", "TopFloor", x => floors[floors.Length - 1] = x, topFloor, false);
        yield return new WaitForFixedUpdate();

        enemies = new GameObject[50]; //Object pool constant setup
        s_Enemy = new Enemy[enemies.Length];
        for (int i = 0; i < enemies.Length; i++)
        {
            ResourcesManager.Instance.InstantiateAssets("Prefabs", "Enemy", x => enemies[i] = x, enemyPool, false);
            s_Enemy[i] = enemies[i].GetComponent<Enemy>();
            s_Enemy[i].Init();
            yield return new WaitForFixedUpdate();
        }

        ResourcesManager.Instance.InstantiateAssets("Prefabs", "Player", x => player = x, playerPool, false);
        s_Player = player.GetComponent<Player>();
        s_Player.Init();
        yield return new WaitForFixedUpdate();

        LevelDatas = new List<LevelData>(ResourcesManager.Instance.AssignLevelDataToArray("LevelData"));
        yield return new WaitForFixedUpdate();
        var _mainLevels = LevelDatas.Where(x => x.name.Split('(', ')')[1].
        Equals("main", System.StringComparison.OrdinalIgnoreCase)).ToList();
        _mainLevels.Sort((p1, p2) => int.Parse(p1.name.Split(' ')[1]).CompareTo(int.Parse(p2.name.Split(' ')[1])));
        mainLevels = _mainLevels.ToArray();
        yield return new WaitForFixedUpdate();
        UIManager.Instance.sc_LevelSelect.CreateExistingLevels();
        yield return new WaitForFixedUpdate();

        MainCleared = PlayerPrefs.GetInt("MainCleared", 0);
        //Debug.Log(MainCleared);

        UIManager.Instance.StateUpdate(0);
        AudioManager.Instance.PlayBGM(0);
    }

    #endregion

}
