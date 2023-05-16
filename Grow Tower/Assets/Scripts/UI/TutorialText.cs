using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialText : MonoBehaviour
{

    [SerializeField] Button tutorialBtn = null;
    [SerializeField] TextMeshProUGUI tutorialText = null;

    // Start is called before the first frame update
    void Awake()
    {
        if (!tutorialBtn) tutorialBtn = GetComponentInChildren<Button>();
        if (!tutorialText) tutorialText = GetComponentInChildren<TextMeshProUGUI>();

        tutorialBtn.onClick.AddListener(CloseTutorial);
    }

    public void CloseTutorial()
    {
        if (gameObject.activeInHierarchy)
            gameObject.SetActive(false);
    }

    public void AssignTutorial(string message)
    {
        tutorialText.text = message;
        gameObject.SetActive(true);
    }
}
