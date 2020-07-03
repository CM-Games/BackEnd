using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RockManager : MonoBehaviour
{
    public IEnumerator coroutine;
    GameObject planet;
    bool rotateAround;
    public int speed;          // 운석 속도
    int rotateSpeed;           // 운석 공전 속도
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
        float timer = 0;
        int temp = 0;

        while (true)
        {
            transform.Translate(new Vector3(speed, 0, 0) * Time.deltaTime, Space.World);

            if (rotateAround)
            {
                if (timer <= 1)
                {
                    speed = (int)Mathf.Lerp(rotateSpeed, rotateSpeed * 0.7f, timer);
                    temp = (int)Mathf.Lerp(0, rotateSpeed, timer);
                    timer += Time.deltaTime;
                }

                transform.DOMoveY(posY, 15).SetEase(Ease.Linear);

                PlanetManager.instance.applyGravity(transform);
            }

            if (transform.position.x >= 100 || transform.position.z <= -0.4f || transform.position.z > 100 || transform.position.x < -140
                || transform.position.y > 50 || transform.position.y < -50) callReturnPool();


            yield return null;
        }
    }

    void callReturnPool()
    {
        transform.position = new Vector3(-95, 0, 0);
        speed = 0;
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

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("planet"))
        {
            if (other.name == "planetCollider")
            {
                info.rockCount++;
                UIManager.instance.setRockCount();
                callReturnPool();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("planet"))
        {
            rotateAround = false;
        }
    }
}
