using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class info
{
    public static int rockCount;
    public static float gravity;
    public static float radius;

    public static int[] itemUpgradeValue;
    public static int gravityPrice;
    public static int expandPrice;
}


public class PlanetManager : MonoBehaviour
{
    public static PlanetManager instance;

    GameObject target;
    IEnumerator temp;

    public enum Item { Gravity, Expand }

    private void Awake()
    {
        instance = this;
        Init();
    }

    // 초기화
    void Init()
    {
        info.rockCount = 1000;
        info.gravity = -1f;
        info.radius = 0.8f;
        info.itemUpgradeValue = new int[System.Enum.GetValues(typeof(Item)).Length];
        info.gravityPrice = 300;
        info.expandPrice = 500;

        target = null;
        temp = updateTouch();

        UIManager.instance.UIInit();
        UIManager.instance.setRockCount();
        UIManager.instance.setItemPrice(true);

        StartCoroutine(temp);
    }

    // 업데이트 대신 작동
    IEnumerator updateTouch()
    {
        while (true)
        {
            if (Input.GetMouseButtonDown(0)) getTouchTarget();
            yield return null;
        }
    }

    // 행성 범위 안에들어오면 해당 운석에 해당 행성의 중력을 적용
    public void applyGravity(Transform rock)
    {
        Vector3 gravityUp = (rock.position - transform.position).normalized;
        Vector3 rockUp = rock.up;

        rock.GetComponent<Rigidbody>().AddForce(gravityUp * info.gravity);

        Quaternion targetRotation = Quaternion.FromToRotation(rockUp, gravityUp) * rock.rotation;
    }

    // 행성 번위 설정
    public void setRadius()
    {
        GetComponent<SphereCollider>().radius = info.radius;
    }

    // 현재 터치한 오브젝트가 어떤건지 가져옴
    public void getTouchTarget()
    {
      
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit);

        if(hit.collider != null)
        {
            target = hit.transform.gameObject;
            print(target);
        }
    }
}

