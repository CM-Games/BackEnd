using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingManager : MonoBehaviour
{
    public static PoolingManager instance;

    Queue<GameObject> Rock;
    public GameObject Rock_pref;

    private void Awake()
    {
        instance = this;
        init();
    }

    void init()
    {
        Rock = new Queue<GameObject>();

        for (int i = 0; i < 30; i++)
        {
            GameObject obj = Instantiate(Rock_pref, transform);
            Rock.Enqueue(obj);
            obj.SetActive(false);
        }
    }

    public GameObject getPool()
    {
        GameObject obj = Rock.Dequeue();
       
        obj.SetActive(true);
      //  StartCoroutine(obj.GetComponent<RockManager>().coroutine);
        return obj;
    }

    public void returnPool(GameObject obj)
    {
        Rock.Enqueue(obj);
       // StopCoroutine(obj.GetComponent<RockManager>().coroutine);
        obj.SetActive(false);
    }
}
