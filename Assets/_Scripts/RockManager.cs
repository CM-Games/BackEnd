using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RockManager : MonoBehaviour
{
    public IEnumerator coroutine;

    private void Awake()
    {
        coroutine = destory();       
    }

    public IEnumerator destory()
    {       
        while (true)
        {
            transform.Translate(new Vector3(8, 0, 0) * Time.deltaTime, Space.World);
            if (transform.position.x >= 9.5)
            {
                transform.position = new Vector3(-20, 0, 0);
                PoolingManager.instance.returnPool(gameObject);
            }

            yield return null;
        }       
    }
}
