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
    float timer = 0;
    int temp = 0;

    private void Awake()
    {
            
        rotateAround = false;
    }

    private void OnEnable()
    {
        coroutine = manage();
        speed = Random.Range(5, 50);
        rotateSpeed = speed;
        timer = 0;
        temp = 0;
        
        StartCoroutine(coroutine);
    }

    private void OnDisable()
    {
        StopCoroutine(coroutine);
    }

    public IEnumerator manage()
    {
        while (true)
        {
            transform.Translate(new Vector3(speed, 0, 0) * Time.deltaTime, Space.World);
            if (rotateAround)
            {
                if (timer <= 2)
                {
                    speed = (int)Mathf.Lerp(rotateSpeed, rotateSpeed * 0.5f, timer);
                    temp = (int)Mathf.Lerp(0, rotateSpeed, timer);
                    timer += Time.deltaTime;
                }

                transform.DOMoveY(posY, 15).SetEase(Ease.Linear);

                PlanetManager.instance.applyGravity(transform);
            }

            if (transform.position.x >= 100 || transform.position.z <= -0.4f || transform.position.z > 100 || transform.position.x < -140
                || transform.position.y > 50 || transform.position.y < -50) break;


            yield return null;
        }

        callReturnPool();
    }

    void callReturnPool()
    {
        transform.position = new Vector3(-95, 0, 0);
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        gameObject.GetComponent<Rigidbody>().angularVelocity = new Vector3(0f, 0f, 0f);
        //transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        StopCoroutine(coroutine);
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
