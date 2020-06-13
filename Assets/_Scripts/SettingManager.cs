using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SettingManager : MonoBehaviour
{
    public Transform Setting;
    Button settingButton;
    Image settingImage;
    bool isSetting;
    

    public void SettingUI()
    {
        if (!isSetting)
        {
            isSetting = true;
            settingButton.transform.DORotate(new Vector3(0, 0, 90), 0.5f).SetEase(Ease.OutQuart);
            settingImage.transform.DOLocalMoveX(384, 0.5f).SetEase(Ease.OutQuart);            
        }
        else
        {
            isSetting = false;
            settingButton.transform.DORotate(new Vector3(0, 0, 00), 0.5f).SetEase(Ease.OutQuart);
            settingImage.transform.DOLocalMoveX(712, 0.5f).SetEase(Ease.OutQuart);
        }
    }

    private void Awake()
    {
        isSetting = false;
        settingButton = Setting.GetChild(1).GetComponent<Button>();
        settingImage = Setting.GetChild(0).GetComponent<Image>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
