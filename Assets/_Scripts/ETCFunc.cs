using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ETCFunc : MonoBehaviour
{
    public Transform LoadingObj;
    public GameObject FriendUI;

    void Update()
    {
        rotateObj();
    }

    void rotateObj() => LoadingObj.Rotate(new Vector3(0, 0, -100 * Time.deltaTime));

    public void offPopUp() => EventSystem.current.currentSelectedGameObject.transform.parent.gameObject.SetActive(false);

    public void openUI(string type)
    {
        if (type == "Friend") FriendUI.SetActive(true);
    }
}
