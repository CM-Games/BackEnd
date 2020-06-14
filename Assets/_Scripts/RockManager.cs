using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RockManager : MonoBehaviour
{
    public IEnumerator coroutine;
    GameObject planet;
    bool rotateAround;
   public int speed;          // 운석 속도
    int rotateSpeed;    // 운석 공전 속도
    int posY;

    private void Awake()
    {
        coroutine = manage();
        speed = Random.Range(5, 50);
        rotateSpeed = speed;
        rotateAround = false;
    }

    public IEnumerator manage()
    {       
        while (true)
        {
          if(!rotateAround)  transform.Translate(new Vector3(speed, 0, 0) * Time.deltaTime, Space.World);

            if(rotateAround)
            {
             //   speed = (int)Mathf.Lerp(rotateSpeed, 0, Time.deltaTime);
                //transform.position = new Vector3(
                //    transform.position.x,
                //    Mathf.Lerp(transform.position.y,posY, Time.deltaTime),
                //    transform.position.z
                //    );
                transform.DOMoveY(posY, 15).SetEase(Ease.Linear);
                transform.RotateAround(planet.transform.position, Vector3.down, rotateSpeed * Time.deltaTime);
            }


            if (transform.position.x >= 100) callReturnPool();

            yield return null;
        }       
    }

    void callReturnPool()
    {
        transform.position = new Vector3(-95, 0, 0);
        PoolingManager.instance.returnPool(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("planet"))
        {
            planet = other.gameObject;
            posY = Random.Range(-5, 5);
            rotateAround = true;
        }
    }
}
