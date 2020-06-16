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
        info.rockCount = 0;
        info.gravity = -1f;
        info.radius = 0.8f;
        info.itemUpgradeValue = new int[System.Enum.GetValues(typeof(Item)).Length];
        info.gravityPrice = 500;
        info.expandPrice = 500;

        UIManager.instance.setRockCount();
        UIManager.instance.setItemPrice();
    }

    // 행성 범위 안에들어오면 해당 운석에 해당 행성의 중력을 적용
    public void applyGravity(Transform rock)
    {
        Vector3 gravityUp = (rock.position - transform.position).normalized;
        Vector3 rockUp = rock.up;

        rock.GetComponent<Rigidbody>().AddForce(gravityUp * info.gravity);

        Quaternion targetRotation = Quaternion.FromToRotation(rockUp, gravityUp) * rock.rotation;
    }

    // 아이템 구매시 해당 옵션 재 정의
    public void setGravity(string name, int value)
    {
        if (name == "gravity") info.gravityPrice += value;
        else if (name == "radius") info.expandPrice += value;

        UIManager.instance.setItemPrice();
    }
}

