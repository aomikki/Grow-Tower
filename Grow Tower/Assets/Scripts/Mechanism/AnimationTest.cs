using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTest : MonoBehaviour
{

    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        StartCoroutine(AnimTest());
    }

    private IEnumerator AnimTest()
    {
        while (true)
        {
            yield return new WaitForSeconds(5);
            anim.SetBool("isMoving", true);
            yield return new WaitForSeconds(5);
            anim.SetInteger("speed", 6);
            yield return new WaitForSeconds(2);
            anim.SetBool("isMoving", false);
            yield return new WaitForSeconds(2);
            anim.SetBool("isMoving", true);
            yield return new WaitForSeconds(3);
            anim.SetInteger("speed", 2);
            yield return new WaitForSeconds(3);
            anim.SetTrigger("attack");
            yield return new WaitForSeconds(3);
            anim.SetTrigger("block");
        }
    }
}
