using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MenuManager : MonoBehaviour
{
    public void upPrice(int value)
    {
        if (value == 0) PlanetManager.instance.setGravity(PlanetManager.Item.Gravity);
        else if (value == 1) PlanetManager.instance.setGravity(PlanetManager.Item.Expand);
    }

}
