using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Enums;

/*
 * Idea
 * 1. need box collider 2D, prob is trigger as well?
 * 2. resize the collider according to the current tower size?
 * 3. swipe down/up to decrease/increase tower height
 * 4. when floor only left one, can't decrease and the collider is 2 floor high
 * 5. can't go up if player power is 1
 */
public class PlayerTower : MonoBehaviour
{

    Vector3 startPos = Vector3.negativeInfinity, curPos;

    bool isEnable;

    RESTRICTION restriction { get { return GameManager.Instance.restriction; } }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Enable(bool isOn) => isEnable = isOn;

    // Update is called once per frame, untested
    void Update()
    {
        if (!isEnable) return;
        if (Pointer.current.press.wasPressedThisFrame)
        {
            var value = Pointer.current.position.ReadValue();
            if (RaycastChecking(value))
                startPos = value;
        }
        if (Pointer.current.press.isPressed)
        {
            curPos = Pointer.current.position.ReadValue();
            //Debug.Log(RaycastChecking(curPos));
        }
        if (Pointer.current.press.wasReleasedThisFrame && startPos != Vector3.negativeInfinity)
        {
            if (GameManager.Instance.PlayerGrabbed) startPos = Vector3.negativeInfinity;
            else if (restriction != RESTRICTION.NOSWIPE)
            {
                if (Vector3.Distance(startPos, curPos) > 5)
                {
                    var dirPos = (curPos - startPos).normalized;
                    if (dirPos.y >= 0.9f && GameManager.Instance.PlayerPower > 1 && restriction != RESTRICTION.NOUP)
                    {
                        GameManager.Instance.GetFloor(GameManager.Instance.playerTower);
                        var round = Mathf.FloorToInt(GameManager.Instance.PlayerPower / 2);
                        GameManager.Instance.PlayerPower = round;
                    }
                    else if (dirPos.y <= -0.9f && GameManager.Instance.s_playerTower.floors.Count > 1 && restriction != RESTRICTION.NODOWN)
                    {
                        GameManager.Instance.ReturnFloor(GameManager.Instance.playerTower, 0);
                        GameManager.Instance.PlayerPower *= 2;
                    }
                    GameManager.Instance.s_playerTower.SetFloor();
                    startPos = Vector3.negativeInfinity;
                }
            }
        }
    }

    bool RaycastChecking(Vector2 position)
    {
        Ray touchPos = Camera.main.ScreenPointToRay(position);
        RaycastHit2D hit = Physics2D.GetRayIntersection(touchPos);
        return hit.collider && hit.collider.CompareTag("Self");
    }

    /*
     * in terms of raycast
     * something is in the front of other
     * in here, parent collider seems to be in front of child?
     * but even so I still need box collider to get it
     */

}
