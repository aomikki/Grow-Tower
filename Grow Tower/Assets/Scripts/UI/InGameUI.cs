using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{

    [SerializeField] GameObject pauseSection, returnSection = null;
    [Space(10f)]
    [SerializeField] Button pauseBtn = null;
    [SerializeField] Button restartBtn, returnMenuBtn, returnCancelBtn, returnConfirmBtn = null;

    // Start is called before the first frame update
    void Start()
    {
        pauseBtn.onClick.AddListener(Pause);
        restartBtn.onClick.AddListener(Restart);
        returnMenuBtn.onClick.AddListener(ReturnMenu);
        returnCancelBtn.onClick.AddListener(ReturnCancel);
        returnConfirmBtn.onClick.AddListener(ReturnConfirm);
    }

    void Pause()
    {
        UIManager.Instance.StateUpdate(2);
        pauseSection.SetActive(UIManager.Instance.IsPaused);
    }

    void Restart()
    {
        //reset game here
        GameManager.Instance.ReloadLevel();
        Pause();
    }

    void ReturnMenu() => returnSection.SetActive(true);
    void ReturnCancel() => returnSection.SetActive(false);
    void ReturnConfirm()
    {
        GameManager.Instance.DemolishLevel();
        UIManager.Instance.StateUpdate(0);
        pauseSection.SetActive(false);
        returnSection.SetActive(false);
    }
}
