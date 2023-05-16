using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{

    //should be initialized with what floor do we have now..?
    public List<Floor> floors;

    string tags;

    public void SetTags(string s) => tags = s;

    public void SetFloor()
    {
        if (floors.Count.Equals(0)) return;
        for (int i = 0; i < floors.Count; i++)
        {
            floors[i].SetLevel(i);
            floors[i].tag = tags;
        }
        UpdateFloor();
    }

    public void UpdateFloor()
    {
        if (floors.Count.Equals(0)) return;
        //get floor height first, either constant or standard from source
        var height = floors[0].Height;

        //update, I think now I have to get it based on tile height
        //fortunately the size is constant with the world scale, for now

        //there was a y point start, that is the first floor y position, but for now we set it all to 0 for first floor
        //also make a constant number for y point start so both equally starts at the same level
        var yPoint = -23f;

        for (int i = 0; i < floors.Count; i++)
        {
            var obj = floors[i];
            //just to make sure those from other tower to go here, future tween
            //obj.transform.localPosition = new Vector3(0, obj.transform.localPosition.y);

            //now checking the tower height by checking the level according to the index too
            obj.SetLevel(i);
            obj.transform.SetSiblingIndex(i);
            var formula = height * obj.Level + yPoint;
            obj.transform.localPosition = new Vector3(0, formula);
            if (!obj.CompareTag(tags)) obj.tag = tags;
            //Debug.Log("Tower " + gameObject.name + "\nFloor " + obj.Level + " Height: " + formula);
            //Debug.Log(height * obj.Level + yPoint);
        }
    }

}
