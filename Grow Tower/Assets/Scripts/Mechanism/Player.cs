using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Player : MonoBehaviour
{

    public Power power;
    [SerializeField] TextMeshPro powerTxt = null;

    Vector3 startPos; //could return if not attacking, and even so could decide whether return to local pos or stay
    Vector3 pointerPosWorld;
    public bool IsGrabbed { get; private set; }

    int contactCounter = 0;
    float enemyStrength = -1f;

    bool isInit = false;

    public void Init()
    {
        if (isInit) return;
        //actually init this one
        var rt = powerTxt.rectTransform;
        rt.sizeDelta = new Vector2(transform.localScale.x, transform.localScale.y);

        power = new Power();
        power.OnVariableChange += TextUpdate;

        isInit = true;
    }

    /*
     * wasPressedThisFrame = buttonDown
     * isPressed = buttonDown - Hold
     * wasReleasedThisFrame = buttonUp
     */

    private void Update()
    {
        if (Pointer.current.press.wasPressedThisFrame) startPos = transform.localPosition;
        if (Pointer.current.press.isPressed)
        {
            pointerPosWorld = Camera.main.ScreenToWorldPoint(Pointer.current.position.ReadValue());

            Vector2 ppw2d = new Vector2(pointerPosWorld.x, pointerPosWorld.y);

            RaycastHit2D hitInfo = Physics2D.Raycast(ppw2d, Camera.main.transform.forward, Mathf.Infinity, ~(1 << 7));

            if (hitInfo.collider && hitInfo.collider.gameObject.Equals(gameObject) && !IsGrabbed) IsGrabbed = true;
            if (IsGrabbed)
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Pointer.current.position.ReadValue()) - transform.position;
                transform.Translate(mousePos);
            }
        }
        if (Pointer.current.press.wasReleasedThisFrame)
        {
            if (enemyStrength >= 0) AttackResult(enemyStrength);
            transform.localPosition = startPos; //for now we'll return it
            IsGrabbed = false;
            contactCounter = 0;
        }
    }
    
    Floor enemyFloor; //temporary get enemy floor

    private void OnTriggerEnter2D(Collider2D collision)
    {        
        if (collision.tag.Equals("Enemy"))
        {
            enemyFloor = collision.GetComponent<Floor>();
            enemyStrength = enemyFloor.GetTotalStrength();
            contactCounter++;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("Enemy"))
        {
            var check = collision.GetComponent<Floor>();
            if (check.Equals(enemyFloor))
                enemyStrength = -1;
            contactCounter--;
            if (contactCounter < 0) contactCounter = 0;
        }
    }

    private void AttackResult(float totalEnemyPower)
    {
        if (contactCounter > 1) return;
        int floorIndex = GameManager.Instance.s_enemyTower.floors.IndexOf(enemyFloor);
        if (GameManager.Instance.s_playerTower.floors.Count < floorIndex + 1) return;
        var check = power.Strength.CompareTo(totalEnemyPower);
        switch (check)
        {
            case 1:
                //Debug.Log("Win");
                power.Strength += totalEnemyPower;
                enemyFloor.TransferFloor(GameManager.Instance.s_playerTower);
                GameManager.Instance.SetPlayerOnTopFloor();
                GameManager.Instance.CheckLevelClear();
                UIManager.Instance.sc_Tutorial.CloseTutorial();
                break;
            case -1:
                //Debug.Log("Lose");
                GameManager.Instance.ReturnPlayer();
                UIManager.Instance.sc_GameVerdict.LoseVerdict();
                break;
            case 0:
                //Debug.Log("Draw");
                AttackResult(power.Strength + 1); //just consider it as lose as well
                break;
        }
        enemyStrength = -1;
    }

    void TextUpdate()
    {
        powerTxt.text = power.Strength_str;
    }

}
