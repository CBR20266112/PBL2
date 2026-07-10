using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 메인 화면 UI 빌더
/// Canvas, 버튼, 텍스트 등 UI 요소 동적 생성
/// </summary>
public class MainScreenUIBuilder : MonoBehaviour
{
    private Canvas _mainCanvas;

    private void Start()
    {
        _mainCanvas = GetComponent<Canvas>();
        if (_mainCanvas == null)
        {
            Debug.LogError("MainScreenUIBuilder: Canvas not found!");
            return;
        }

        BuildUI();
    }

    private void BuildUI()
    {
        // 배경 이미지
        CreateBackground();

        // 상단 HUD
        CreateTopHUD();

        // 중앙 다방 표시
        CreateCafeDisplay();

        // 하단 네비게이션
        CreateBottomNavigation();

        // 튜토리얼 UI 추가
        _mainCanvas.gameObject.AddComponent<TutorialUIBuilder>();

        Debug.Log("Main Screen UI built successfully");
    }

    private void CreateBackground()
    {
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(_mainCanvas.transform, false);

        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = new Color(0.99f, 0.98f, 0.96f, 1f); // 크림색

        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
    }

    private void CreateTopHUD()
    {
        GameObject hudObj = new GameObject("TopHUD");
        hudObj.transform.SetParent(_mainCanvas.transform, false);

        Image hudBg = hudObj.AddComponent<Image>();
        hudBg.color = new Color(1f, 1f, 1f, 0.9f);

        RectTransform hudRect = hudObj.GetComponent<RectTransform>();
        hudRect.anchorMin = new Vector2(0, 1);
        hudRect.anchorMax = new Vector2(1, 1);
        hudRect.offsetMin = new Vector2(0, -80);
        hudRect.offsetMax = Vector2.zero;

        // 플레이어 정보
        CreateHUDText(hudObj, "LevelText", "Lv.1", new Vector2(20, -40), TextAnchor.MiddleLeft);
        CreateHUDText(hudObj, "MoneyText", "₩10,000", new Vector2(-20, -40), TextAnchor.MiddleRight);

        // PlayerHUD 스크립트 추가
        PlayerHUD playerHUD = hudObj.AddComponent<PlayerHUD>();
        playerHUD.enabled = true;
    }

    private void CreateCafeDisplay()
    {
        GameObject cafeObj = new GameObject("CafeDisplay");
        cafeObj.transform.SetParent(_mainCanvas.transform, false);

        Image cafeImage = cafeObj.AddComponent<Image>();
        cafeImage.color = new Color(1f, 0.8f, 0.6f, 0.5f); // 임시 색상 (주황)

        RectTransform cafeRect = cafeObj.GetComponent<RectTransform>();
        cafeRect.anchorMin = new Vector2(0, 0.15f);
        cafeRect.anchorMax = new Vector2(1, 0.85f);
        cafeRect.offsetMin = Vector2.zero;
        cafeRect.offsetMax = Vector2.zero;

        // 임시 텍스트
        Text cafeText = CreateText(cafeObj, "CafeText", "다방\n(Placeholder)", cafeRect.rect.center, TextAnchor.MiddleCenter);
        cafeText.fontSize = 30;
    }

    private void CreateBottomNavigation()
    {
        GameObject navObj = new GameObject("BottomNavigation");
        navObj.transform.SetParent(_mainCanvas.transform, false);

        Image navBg = navObj.AddComponent<Image>();
        navBg.color = new Color(1f, 0.98f, 0.85f, 1f);

        RectTransform navRect = navObj.GetComponent<RectTransform>();
        navRect.anchorMin = new Vector2(0, 0);
        navRect.anchorMax = new Vector2(1, 0.15f);
        navRect.offsetMin = Vector2.zero;
        navRect.offsetMax = Vector2.zero;

        // 4개 버튼 생성
        CreateNavButton(navObj, "WaitCustomerBtn", "손님\n대기", 0, 4);
        CreateNavButton(navObj, "ShopBtn", "상점", 1, 4);
        CreateNavButton(navObj, "CollectionBtn", "컬렉션", 2, 4);
        CreateNavButton(navObj, "SettingsBtn", "설정", 3, 4);

        // MainScreenManager 추가
        MainScreenManager manager = navObj.AddComponent<MainScreenManager>();
        manager.enabled = true;
    }

    private void CreateNavButton(GameObject parent, string name, string label, int index, int totalCount)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent.transform, false);

        Button button = btnObj.AddComponent<Button>();
        Image btnImage = btnObj.AddComponent<Image>();
        btnImage.color = new Color(1f, 0.63f, 0.26f, 1f); // 주황색

        RectTransform btnRect = btnObj.GetComponent<RectTransform>();
        float width = 1f / totalCount;
        btnRect.anchorMin = new Vector2(width * index, 0);
        btnRect.anchorMax = new Vector2(width * (index + 1), 1);
        btnRect.offsetMin = Vector2.zero;
        btnRect.offsetMax = Vector2.zero;

        // 버튼 텍스트
        CreateText(btnObj, "Label", label, Vector2.zero, TextAnchor.MiddleCenter);
    }

    private void CreateHUDText(GameObject parent, string name, string text, Vector2 position, TextAnchor anchor)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent.transform, false);

        Text textComponent = textObj.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        textComponent.fontSize = 18;
        textComponent.fontStyle = FontStyle.Bold;
        textComponent.alignment = anchor;
        textComponent.color = new Color(0.42f, 0.27f, 0.14f, 1f); // 갈색

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchoredPosition = position;
        textRect.sizeDelta = new Vector2(200, 60);
    }

    private Text CreateText(GameObject parent, string name, string text, Vector2 position, TextAnchor anchor)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent.transform, false);

        Text textComponent = textObj.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        textComponent.fontSize = 20;
        textComponent.alignment = anchor;
        textComponent.color = new Color(0.42f, 0.27f, 0.14f, 1f);

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchoredPosition = position;
        textRect.sizeDelta = new Vector2(400, 100);

        return textComponent;
    }
}
