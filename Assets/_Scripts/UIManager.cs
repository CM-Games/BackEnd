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
    public Text gravityValueText;

    [Header("Menu")]
    public Transform gravityPriceObj;     // 중력 구매버튼의 텍스트
    public Transform expandPriceObj;      // 범위 구매버튼의 텍스트
    public Transform MenuObj;             // 메뉴 최상단 오브젝트
    public Text versionText;              // 버전 표시
    Text gravityPriceText;                // 중력 가격 텍스트
    Text expandPriceText;                 // 범위 가격 텍스트
    Button menuButton;                    // 메뉴 버튼
    Image menuImage;                      // 메뉴 뒷배경
    bool isMenu;                          // 현재 매뉴가 사용중인가 판단
    int gravityNumberCount;               // 중력 가격이 몇자리 숫자인지
    int expandNumberCount;                // 확장 가격이 몇자리 숫자인지 

    StringBuilder stringBuilder;          // 스트링 합칠때 쓸려고 만듬

    private void Awake()
    {
        instance = this;
        versionText.text = "version : " + Application.version;
    }

    public void UIInit()
    {
        stringBuilder = new StringBuilder();

        isMenu = false;
        menuButton = MenuObj.GetChild(1).GetComponent<Button>();
        menuImage = MenuObj.GetChild(0).GetComponent<Image>();
        gravityPriceText = gravityPriceObj.GetChild(0).GetComponent<Text>();
        expandPriceText = expandPriceObj.GetChild(0).GetComponent<Text>();

        gravityNumberCount = info.gravityPrice.ToString().Length;
        expandNumberCount = info.expandPrice.ToString().Length;

        gravityValueText.text = "현재 중력 파워\n" + info.gravity * -1 + "%";
    }

    // 현재 모은 운석 갯수의 텍스트를 업데이트
    public void setRockCount()
    {
        stringBuilder.Append(info.rockCount);
        stringBuilder.Append(" 개");

        rockCount.text = stringBuilder.ToString();

        stringBuilder.Clear();
    }

    // 현재 중력값 또는 범위를 텍스트에 업데이트
    public void setPlanetValue(PlanetManager.Item item)
    {
        if (item == PlanetManager.Item.Gravity)
        {
            stringBuilder.Append("현재 중력 파워\n");
            stringBuilder.Append(-1 *info.gravity);
            stringBuilder.Append("%");
            gravityValueText.text = stringBuilder.ToString();

            stringBuilder.Clear();
        }
        else if (item == PlanetManager.Item.Expand)
        {

        }
    }

    // 현재 각 아이템의 업그레이드 비용을 업데이트 ( 0 == 중력 , 1 == 범위 )
    // 자릿수가 올라갈때마다 가운데로 정렬
    public void setItemPrice(bool init = false)
    {
        gravityPriceText.text = info.gravityPrice.ToString();
        expandPriceText.text = info.expandPrice.ToString();

        if (init)
        {
            gravityPriceObj.DOLocalMoveX(gravityPriceObj.localPosition.x - (2.5f * (gravityNumberCount -3)), 0.1f).SetEase(Ease.Linear);
            expandPriceObj.DOLocalMoveX(expandPriceObj.localPosition.x - (2.5f * (expandNumberCount - 3)), 0.1f).SetEase(Ease.Linear);
            return;
        }

        if (gravityNumberCount != info.gravityPrice.ToString().Length)
        {
            gravityNumberCount = info.gravityPrice.ToString().Length;
            gravityPriceObj.DOLocalMoveX(gravityPriceObj.localPosition.x - 2.5f, 0.1f).SetEase(Ease.Linear);
        }

        if (expandNumberCount != info.expandPrice.ToString().Length)
        {
            expandNumberCount = info.expandPrice.ToString().Length;
            expandPriceObj.DOLocalMoveX(expandPriceObj.localPosition.x - 2.5f, 0.1f).SetEase(Ease.Linear);
        }
    }

    // 메뉴를 껏다가 켜는 함수
    public void SettingUIManage()
    {
        if (!isMenu)
        {
            isMenu = true;
            menuButton.transform.DORotate(new Vector3(0, 0, 90), 0.5f).SetEase(Ease.OutQuart);
            menuButton.GetComponent<Image>().DOColor(new Color(1, 0.4745f, 0.4745f), 0.3f).SetEase(Ease.Linear);
            menuImage.transform.DOLocalMoveX(-325, 0.5f).SetEase(Ease.OutQuart);
        }
        else
        {
            isMenu = false;
            menuButton.transform.DORotate(new Vector3(0, 0, 00), 0.5f).SetEase(Ease.OutQuart);
            menuButton.GetComponent<Image>().DOColor(new Color(1, 1, 1), 0.3f).SetEase(Ease.Linear);
            menuImage.transform.DOLocalMoveX(0, 0.5f).SetEase(Ease.OutQuart);
        }
    }

}
