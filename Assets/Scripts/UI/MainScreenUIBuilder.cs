using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 硫붿씤 ?붾㈃ UI 鍮뚮뜑
/// Canvas, 踰꾪듉, ?띿뒪????UI ?붿냼 ?숈쟻 ?앹꽦
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

        EnsureEventSystemExists();
        BuildUI();
    }

    private void BuildUI()
    {
        // 諛곌꼍 ?대?吏
        CreateBackground();

        // ?곷떒 HUD
        CreateTopHUD();

        // 以묒븰 ?ㅻ갑 ?쒖떆
        CreateCafeDisplay();

        // ?섎떒 ?ㅻ퉬寃뚯씠??
        CreateBottomNavigation();

        // ?쒗넗由ъ뼹 UI 異붽?
        _mainCanvas.gameObject.AddComponent<TutorialUIBuilder>();

        Debug.Log("Main Screen UI built successfully");
    }

    private void EnsureEventSystemExists()
    {
        if (FindObjectOfType<EventSystem>() != null)
        {
            return;
        }

        GameObject eventSystemObj = new GameObject("EventSystem");
        eventSystemObj.AddComponent<EventSystem>();
        eventSystemObj.AddComponent<StandaloneInputModule>();
        DontDestroyOnLoad(eventSystemObj);
        Debug.Log("MainScreenUIBuilder: Created missing EventSystem for UI interaction.");
    }

    private void CreateBackground()
    {
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(_mainCanvas.transform, false);

        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = new Color(0.99f, 0.98f, 0.96f, 1f); // ?щ┝??
        bgImage.raycastTarget = false;

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

        // ?뚮젅?댁뼱 ?뺣낫
        Text levelText = CreateHUDText(hudObj, "LevelText", "Lv.1", new Vector2(20, -40), TextAnchor.MiddleLeft);
        Text moneyText = CreateHUDText(hudObj, "MoneyText", "₩0", new Vector2(-20, -40), TextAnchor.MiddleRight);

        // 寃쏀뿕移?諛?
        GameObject expBarBg = new GameObject("ExpBarBg");
        expBarBg.transform.SetParent(hudObj.transform, false);
        Image expBarBgImage = expBarBg.AddComponent<Image>();
        expBarBgImage.color = new Color(0.95f, 0.95f, 0.95f, 1f);

        RectTransform expBarBgRect = expBarBg.GetComponent<RectTransform>();
        expBarBgRect.anchorMin = new Vector2(0.25f, 0);
        expBarBgRect.anchorMax = new Vector2(0.75f, 0);
        expBarBgRect.anchoredPosition = new Vector2(0, 10);
        expBarBgRect.sizeDelta = new Vector2(500, 10);

        GameObject expBar = new GameObject("ExpBarFill");
        expBar.transform.SetParent(expBarBg.transform, false);
        Image expBarImage = expBar.AddComponent<Image>();
        expBarImage.color = new Color(1f, 0.63f, 0.26f, 1f);

        RectTransform expBarRect = expBar.GetComponent<RectTransform>();
        expBarRect.anchorMin = new Vector2(0, 0);
        expBarRect.anchorMax = new Vector2(0, 1);
        expBarRect.offsetMin = Vector2.zero;
        expBarRect.offsetMax = Vector2.zero;
        expBarRect.sizeDelta = new Vector2(0, 10);

        PlayerHUD playerHUD = hudObj.AddComponent<PlayerHUD>();
        playerHUD.Initialize(levelText, moneyText, expBarImage);
    }

    private void CreateCafeDisplay()
    {
        GameObject cafeObj = new GameObject("CafeDisplay");
        cafeObj.transform.SetParent(_mainCanvas.transform, false);

        Image cafeImage = cafeObj.AddComponent<Image>();
        cafeImage.color = new Color(1f, 0.8f, 0.6f, 0.5f); // ?꾩떆 ?됱긽 (二쇳솴)

        RectTransform cafeRect = cafeObj.GetComponent<RectTransform>();
        cafeRect.anchorMin = new Vector2(0, 0.15f);
        cafeRect.anchorMax = new Vector2(1, 0.85f);
        cafeRect.offsetMin = Vector2.zero;
        cafeRect.offsetMax = Vector2.zero;

        // ?꾩떆 ?띿뒪??
        Text cafeText = CreateText(cafeObj, "CafeText", "Cafe\n(Placeholder)", cafeRect.rect.center, TextAnchor.MiddleCenter);
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

        // Create 4 buttons
        Button waitButton = CreateNavButton(navObj, "WaitCustomerBtn", "Wait", 0, 4);
        Button shopButton = CreateNavButton(navObj, "ShopBtn", "Shop", 1, 4);
        Button collectionButton = CreateNavButton(navObj, "CollectionBtn", "Collection", 2, 4);
        Button settingsButton = CreateNavButton(navObj, "SettingsBtn", "Settings", 3, 4);

        // MainScreenManager 異붽?
        MainScreenManager manager = navObj.AddComponent<MainScreenManager>();
        manager.Initialize(waitButton, shopButton, collectionButton, settingsButton);
        manager.enabled = true;
    }

    private Button CreateNavButton(GameObject parent, string name, string label, int index, int totalCount)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent.transform, false);

        Button button = btnObj.AddComponent<Button>();
        Image btnImage = btnObj.AddComponent<Image>();
        btnImage.color = new Color(1f, 0.63f, 0.26f, 1f); // 二쇳솴??

        RectTransform btnRect = btnObj.GetComponent<RectTransform>();
        float width = 1f / totalCount;
        btnRect.anchorMin = new Vector2(width * index, 0);
        btnRect.anchorMax = new Vector2(width * (index + 1), 1);
        btnRect.offsetMin = Vector2.zero;
        btnRect.offsetMax = Vector2.zero;

        // 踰꾪듉 ?띿뒪??
        CreateText(btnObj, "Label", label, Vector2.zero, TextAnchor.MiddleCenter);
        return button;
    }

    private Text CreateHUDText(GameObject parent, string name, string text, Vector2 position, TextAnchor anchor)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent.transform, false);

        Text textComponent = textObj.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = FontHelper.GetDefaultFont();
        textComponent.fontSize = 18;
        textComponent.fontStyle = FontStyle.Bold;
        textComponent.alignment = anchor;
        textComponent.color = new Color(0.42f, 0.27f, 0.14f, 1f); // 媛덉깋
        textComponent.raycastTarget = false;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchoredPosition = position;
        textRect.sizeDelta = new Vector2(200, 60);

        return textComponent;
    }

    private Text CreateText(GameObject parent, string name, string text, Vector2 position, TextAnchor anchor)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent.transform, false);

        Text textComponent = textObj.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = FontHelper.GetDefaultFont();
        textComponent.fontSize = 20;
        textComponent.alignment = anchor;
        textComponent.color = new Color(0.42f, 0.27f, 0.14f, 1f);
        textComponent.raycastTarget = false;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchoredPosition = position;
        textRect.sizeDelta = new Vector2(400, 100);

        return textComponent;
    }
}

