using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MenuManager : MonoBehaviour
{
    // 아이템 구매
    // 0 : 중력 // 1 : 범위
    public void buyItem(int value)
    {
        PlanetManager.Item item = 0;
        bool reset = false;

        if (value == 0 && info.rockCount - info.gravityPrice >= 0)
        {
            info.rockCount -= info.gravityPrice;
            info.gravity -= 1;

            UIManager.instance.setItemPrice();
            info.gravityPrice += (int)info.gravity * -50;            

            item = PlanetManager.Item.Gravity;
            reset = true;
        }
        else if (value == 1 && info.rockCount - info.expandPrice >= 0)
        {
            info.rockCount -= info.expandPrice;
            info.radius += 0.03f;

            UIManager.instance.setItemPrice();
            info.expandPrice += (int)(info.radius * 100);

            item = PlanetManager.Item.Expand;
            reset = true;

            PlanetManager.instance.setRadius();
        }
        else print("재화 부족");

        if (reset)
        {
            info.itemUpgradeValue[value]++;
            UIManager.instance.setItemPrice();
            UIManager.instance.setRockCount();
            UIManager.instance.setPlanetValue(item);
        }
    }
}
