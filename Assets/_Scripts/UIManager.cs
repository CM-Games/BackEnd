using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("User")]
    public Text rockCount;

    [Header("Setting")]
    public Transform gravityBuyObj;
    public Text grivatyPriceText;
    public Text expandPriceText;
    public Transform Setting;
    Button settingButton;
    Image settingImage;
    bool isSetting;

    StringBuilder stringBuilder;

    private void Awake()
    {
        instance = this;
        stringBuilder = new StringBuilder();
        isSetting = false;
        settingButton = Setting.GetChild(1).GetComponent<Button>();
        settingImage = Setting.GetChild(0).GetComponent<Image>();
    }

    // 현재 모은 운석 갯수의 텍스트를 업데이트
    public void setRockCount()
    {
        stringBuilder.Append(info.rockCount);
        stringBuilder.Append(" 개");

        rockCount.text = stringBuilder.ToString();

        stringBuilder.Clear();
    }

    // 현재 각 아이템의 업그레이드 비용을 업데이트
    public void setItemPrice()
    {
        grivatyPriceText.text = info.gravityPrice.ToString();
        expandPriceText.text = info.expandPrice.ToString();

        
    }

    // 메뉴를 껏다가 켜는 함수
    public void SettingUIManage()
    {
        if (!isSetting)
        {
            isSetting = true;
            settingButton.transform.DORotate(new Vector3(0, 0, 90), 0.5f).SetEase(Ease.OutQuart);
            settingButton.GetComponent<Image>().DOColor(new Color(1, 0.4745f, 0.4745f), 0.3f).SetEase(Ease.Linear);
            settingImage.transform.DOLocalMoveX(-325, 0.5f).SetEase(Ease.OutQuart);
        }
        else
        {
            isSetting = false;            
            settingButton.transform.DORotate(new Vector3(0, 0, 00), 0.5f).SetEase(Ease.OutQuart);
            settingButton.GetComponent<Image>().DOColor(new Color(1,1,1), 0.3f).SetEase(Ease.Linear);
            settingImage.transform.DOLocalMoveX(0, 0.5f).SetEase(Ease.OutQuart);
        }
    }


}
