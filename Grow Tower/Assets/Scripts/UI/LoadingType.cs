using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingType : MonoBehaviour
{

    [SerializeField] TMPro.TextMeshProUGUI loadingTxt = null;
    Coroutine c;
    // Start is called before the first frame update
    void Awake()
    {
        if (!loadingTxt) loadingTxt = GetComponent<TMPro.TextMeshProUGUI>();
        c = StartCoroutine(_Typing());
    }

    IEnumerator _Typing()
    {
        while (true)
        {
            loadingTxt.text = "Now Loading";
            yield return new WaitForSeconds(0.67f);
            loadingTxt.text = "Now Loading.";
            yield return new WaitForSeconds(0.67f);
            loadingTxt.text = "Now Loading..";
            yield return new WaitForSeconds(0.67f);
            loadingTxt.text = "Now Loading...";
            yield return new WaitForSeconds(0.67f);
        }
    }

    public void StopLoad()
    {
        StopCoroutine(c);
        gameObject.SetActive(false);
    }

}
