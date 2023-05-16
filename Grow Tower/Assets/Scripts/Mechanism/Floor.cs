using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Floor : MonoBehaviour
{

    //Level starts from bottom to up so 0 is the lowest level
    public int Level { get; private set; }

    public float Height
    {
        get
        {
            var render = transform.GetChild(0).GetChild(2).GetComponent<Tilemap>();
            return render.size.y;
        }
    }

    public float Width
    {
        get
        {
            var render = transform.GetChild(0).GetChild(2).GetComponent<Tilemap>();
            return render.size.x;
        }
    }

    public void AddLevel() => Level++;
    public void SubtractLevel() => Level--;

    public void SetLevel(int level) => Level = level;

    public void TransferFloor(Tower tower)
    {
        if (tower.floors.Contains(this)) return; //counter measure if the same floor is added for no reason

        //if (Level >= tower.floors.Count) tower.floors.Add(this); //commented for newer idea bellow
        //else tower.floors.Insert(Level, this);

        var parentTower = gameObject.GetComponentInParent<Tower>(); //need a better clean
        GameManager.Instance.ReturnFloor(parentTower.transform, Level, "Enemy");
        parentTower.floors.Remove(this);
        parentTower.UpdateFloor();


        //need a better despawn
        while(transform.childCount>1)
        {
            var getEnemy = transform.GetChild(1).GetComponent<Enemy>();
            if (getEnemy) GameManager.Instance.ReturnEnemy(transform, 1);
        }

        //transform.SetParent(tower.transform);
        //newer idea, due to tile thing and we want a uniform tiled tower, gotta make a new one instead
        GameManager.Instance.GetFloor(tower.transform);

        //need a more automatic update
        tower.UpdateFloor();
    }

    public float GetTotalStrength()
    {
        float z = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            var enemy = transform.GetChild(i).GetComponent<Enemy>();
            if (enemy)
                z += enemy.Strength;
        }
        return z;
    }

    //This is to control the enemy position
    public void SetPosition()
    {
        var newWidth = transform.GetChild(0).GetChild(0).GetComponent<Tilemap>().size.x;
        var enemyCount = transform.childCount - 1;
        var division = newWidth / enemyCount;
        var height = -1f; //this is constant but better change it here so that the bottom follows
        for (int i = 1; i < transform.childCount; i++)
        {
            var t = transform.GetChild(i);
            if (enemyCount % 2 == 1)
                t.localPosition = new Vector3(division * (i - 1) - division * (enemyCount / 2), height);
            else
                t.localPosition = new Vector3((division * (i - 1)) - (newWidth / 2) + (division / 2), height);
        }
    }

}
