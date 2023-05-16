using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelDataUI : MonoBehaviour
{

    Button button;
    LevelData data;
    [SerializeField] Image levelImg;
    [SerializeField] TextMeshProUGUI levelName, replacement;

    [HideInInspector] public string levelType;

    public int MainLevelNumber { get; private set; }

    public void Assign(LevelData data)
    {
        if (!button)
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(ButtonClick);
        }
        this.data = data;
        string[] split = data.name.Split('(', ')');
        levelType = split[1];
        levelName.text = split[2];
        gameObject.name = $"{levelType}-{levelName.text}";
        if (!levelImg.sprite)
        {
            if (string.Equals(levelType, "main", System.StringComparison.OrdinalIgnoreCase))
            {
                var result = System.Text.RegularExpressions.Regex.Match(data.name, @"\d+$", System.Text.RegularExpressions.RegexOptions.RightToLeft);
                if (result.Success)
                {
                    replacement.text = result.Value;
                    MainLevelNumber = int.Parse(result.Value);
                }
                else replacement.text = "";
            }
            else replacement.text = levelType;
            if (replacement.text != "")
                levelImg.enabled = false;
        }
        else
        {
            replacement.text = ""; 
            levelImg.enabled = true;
        }
    }

    void ButtonClick()
    {
        UIManager.Instance.StateUpdate(1);
        GameManager.Instance.BuildLevel(data);
    }

    public void IsInteractable(bool isOn) => button.interactable = isOn;

}
