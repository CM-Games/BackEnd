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

    public enum Item { Gravity, Expand }

    private void Awake()
    {
        instance = this;
        Init();
    }

    void Init()
    {
        info.rockCount = 1000;
        info.gravity = -1f;
        info.radius = 0.8f;
        info.itemUpgradeValue = new int[System.Enum.GetValues(typeof(Item)).Length];
        info.gravityPrice = 300;
        info.expandPrice = 500;

        UIManager.instance.UIInit();
        UIManager.instance.setRockCount();
        UIManager.instance.setItemPrice(true);
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
    void setRadius()
    {
        GetComponent<SphereCollider>().radius = info.radius;
    }

    // 아이템 구매시 해당 옵션 재 정의
    public void setGravity(Item item)
    {
        if (item == Item.Gravity && info.rockCount - info.gravityPrice >= 0)
        {
            info.rockCount -= info.gravityPrice;
            info.gravity -= 1;
            UIManager.instance.setItemPrice();
            info.gravityPrice += (int)info.gravity * -50;           
           
        }
        else if (item == Item.Expand && info.rockCount - info.expandPrice >= 0)
        {
            info.rockCount -= info.expandPrice;
            info.radius += 0.03f;
            UIManager.instance.setItemPrice();
            info.expandPrice += (int)(info.radius * 100);
            setRadius();
        }
        else print("재화 부족");

        UIManager.instance.setItemPrice();
        UIManager.instance.setRockCount();
        UIManager.instance.setPlanetValue(item);
    }
}

