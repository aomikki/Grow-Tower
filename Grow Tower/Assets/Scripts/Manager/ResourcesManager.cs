using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ResourcesManager : SingletonMonoBehaviour<ResourcesManager>
{    
    public AssetLabelReference[] references;
    public AssetLabelReference[] nonGameObjectReferences;

    [SerializeField]
    private Dictionary<string, List<GameObject>> _ListLoadedAssets = new Dictionary<string, List<GameObject>>();
    [SerializeField]
    private Dictionary<string, List<Object>> _ListData = new Dictionary<string, List<Object>>();
    //can delete this because I just want to make sure the data is in
    //public GameObject[] test;
    //public GameObject[] test2;
    //public LevelData[] dataTest; //it can be LevelData only but just in case there's another data in the dictionary

    [HideInInspector]
    public bool isInit;

    void Start()
    {
        gameObject.name = "(SINGLETON) Resources Manager";
        DontDestroyOnLoad(this);

        isInit = false;
        var initTask = Init();
        initTask.ConfigureAwait(true).GetAwaiter().OnCompleted(async () =>
        {
            //just in case I need to add some
            //also there's a thing to continue task if there's any connected async
            //also prefer for UIManager to wait this instead of getting called
            await LevelLoad();
            //Debug.Log("Completed");

            UIManager.Instance.LoadUI();
            AudioManager.Instance.LoadAudio();
            GameManager.Instance.LoadContent();

            isInit = true;
        });
    }

    private async Task Init()
    {
        foreach (AssetLabelReference reference in references)
        {
            List<GameObject> _list = new List<GameObject>();
            await AddressableLoader.InitAssets(reference, _list);
            _ListLoadedAssets.Add(reference.labelString, _list);
        }
    }

    private async Task LevelLoad()
    {
        foreach (AssetLabelReference reference in nonGameObjectReferences)
        {
            List<Object> _list = new List<Object>();
            await AddressableLoader.InitAssets(reference, _list);
            _ListData.Add(reference.labelString, _list);
        }
    }

    //this is literally the same thing I've done on resources preload, kinda ironic to put it that way
    public void InstantiateAssets(string labelReference, string objectName, System.Action<GameObject> act, Transform parent = null, bool isActive = true, int repeat = 1)
    {
        int n = 0;
        do
        {
            GameObject tmp = null, g = null;
            g = _ListLoadedAssets[labelReference].Find(x => x.gameObject.name.Equals(objectName));
            if (g) tmp = Instantiate(g, parent);
            if (tmp)
            {
                tmp.SetActive(isActive);
                act(tmp);
            }
            else act(null);
            n++;
        } while (n < repeat);        
    }

    public AudioClip[] AssignClipsToArray(string labelReference)
    {
        AudioClip[] clip = new AudioClip[_ListData[labelReference].Count];
        _ListData[labelReference].CopyTo(clip);
        return clip;
    }

    public LevelData[] AssignLevelDataToArray(string labelReference)
    {
        //convert object to type through copying
        //dataTest = new LevelData[_ListData["LevelData"].Count];
        //_ListData["LevelData"].CopyTo(dataTest as LevelData[]); //this can be simplified and becomes a safe cast, which will return to null if failed

        LevelData[] data = new LevelData[_ListData[labelReference].Count];
        _ListData[labelReference].CopyTo(data);
        return data;
    }

}
