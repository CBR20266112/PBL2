using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// 二쇰갑 ?붾㈃ UI
/// 李??쒖“ ?붾㈃ (?⑤룄, ?곕┝?쒓컙, ?щ즺 ?좏깮)
/// </summary>
public class KitchenUIBuilder
{
    private UIManager _uiManager;
    private BrewingManager _brewingManager;
    private Canvas _canvas;

    public KitchenUIBuilder(Canvas canvas, UIManager uiManager)
    {
        _canvas = canvas;
        _uiManager = uiManager;
        _brewingManager = BrewingManager.Instance;
    }

    public void Build()
    {
        // 湲곗〈 ?⑤꼸 ?쒓굅
        foreach (Transform child in _canvas.transform)
        {
            if (child.name == "KitchenPanel")
            {
                UnityEngine.Object.Destroy(child.gameObject);
            }
        }

        // 硫붿씤 ?⑤꼸
        GameObject panelObj = new GameObject("KitchenPanel");
        panelObj.transform.SetParent(_canvas.transform, false);
        RectTransform panelRect = panelObj.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0.99f, 0.98f, 0.96f, 1f); // ?щ┝ 諛곌꼍

        // ?쒕ぉ
        CreateTitle(panelObj, "차 끓이기");

        // ?먮떂 二쇰Ц ?뺣낫
        CreateCustomerOrderSection(panelObj);

        // ?⑤룄 ?좏깮 ?뱀뀡
        CreateTemperatureSection(panelObj);

        // ?곕┝?쒓컙 ?좏깮 ?뱀뀡
        CreateSteepTimeSection(panelObj);

        // 李??좏깮 ?뱀뀡
        CreateTeaSelectionSection(panelObj);

        // 吏꾪뻾瑜??쒖떆
        CreateProgressBar(panelObj);

        // ?쒖“ ?쒖옉 踰꾪듉
        CreateStartBrewingButton(panelObj);

        // ?뚯븘媛湲?踰꾪듉
        CreateBackButton(panelObj);

        // ?쒖“ ?꾨즺 ?대깽??由ъ뒪???깅줉
        _brewingManager.OnBrewingComplete += OnBrewingComplete;
    }

    private void OnBrewingComplete()
    {
        Debug.Log("Brewing complete! Showing rating screen");

        // ?대깽??由ъ뒪???쒓굅
        _brewingManager.OnBrewingComplete -= OnBrewingComplete;

        // 二쇰갑 ?붾㈃ ?④린湲?
        _uiManager.HidePanel("KitchenPanel");

        // ?됯? ?붾㈃ ?쒖떆
        RatingUIBuilder ratingUI = new RatingUIBuilder(_canvas, _uiManager);
        ratingUI.Build();
    }

    private void CreateTitle(GameObject parent, string title)
    {
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(parent.transform, false);
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchoredPosition = new Vector2(0, 900);
        titleRect.sizeDelta = new Vector2(1080, 100);

        Text titleText = titleObj.AddComponent<Text>();
        titleText.text = title;
        titleText.font = FontHelper.GetDefaultFont();
        titleText.fontSize = 50;
        titleText.fontStyle = FontStyle.Bold;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = new Color(0.42f, 0.27f, 0.14f, 1f); // 媛덉깋
    }

    private void CreateCustomerOrderSection(GameObject parent)
    {
        Customer customer = CustomerManager.Instance.GetCurrentCustomer();
        if (customer == null) return;

        // 諛곌꼍
        GameObject bgObj = new GameObject("OrderBg");
        bgObj.transform.SetParent(parent.transform, false);
        RectTransform bgRect = bgObj.AddComponent<RectTransform>();
        bgRect.anchoredPosition = new Vector2(0, 760);
        bgRect.sizeDelta = new Vector2(900, 100);

        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = new Color(1f, 0.9f, 0.8f, 1f); // ?곗＜?⑹깋 諛곌꼍

        // ?쇰꺼
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(bgObj.transform, false);
        RectTransform labelRect = labelObj.AddComponent<RectTransform>();
        labelRect.anchoredPosition = new Vector2(-300, 30);
        labelRect.sizeDelta = new Vector2(200, 50);

        Text labelText = labelObj.AddComponent<Text>();
        labelText.text = $"Order for {customer.data.customerName}:";
        labelText.font = FontHelper.GetDefaultFont();
        labelText.fontSize = 28;
        labelText.fontStyle = FontStyle.Bold;
        labelText.alignment = TextAnchor.MiddleLeft;
        labelText.color = new Color(0.42f, 0.27f, 0.14f, 1f);

        // 二쇰Ц ?댁슜
        string orderText = $"{BrewingManager.GetTeaLabel(customer.data.preferredTea)} " +
                          $"({BrewingManager.GetTemperatureLabel(customer.data.preferredTemperature)}, " +
                          $"{BrewingManager.GetSteepTimeLabel(customer.data.preferredSteepTime)})";

        GameObject orderObj = new GameObject("Order");
        orderObj.transform.SetParent(bgObj.transform, false);
        RectTransform orderRect = orderObj.AddComponent<RectTransform>();
        orderRect.anchoredPosition = new Vector2(200, 30);
        orderRect.sizeDelta = new Vector2(500, 50);

        Text orderTextComponent = orderObj.AddComponent<Text>();
        orderTextComponent.text = orderText;
        orderTextComponent.font = FontHelper.GetDefaultFont();
        orderTextComponent.fontSize = 28;
        orderTextComponent.fontStyle = FontStyle.Bold;
        orderTextComponent.alignment = TextAnchor.MiddleLeft;
        orderTextComponent.color = new Color(1f, 0.63f, 0.26f, 1f); // 二쇳솴??
    }

    private void CreateTemperatureSection(GameObject parent)
    {
        // ?쇰꺼
        GameObject labelObj = new GameObject("TempLabel");
        labelObj.transform.SetParent(parent.transform, false);
        RectTransform labelRect = labelObj.AddComponent<RectTransform>();
        labelRect.anchoredPosition = new Vector2(-400, 600);
        labelRect.sizeDelta = new Vector2(300, 60);

        Text labelText = labelObj.AddComponent<Text>();
        labelText.text = "차 종류";
        labelText.font = FontHelper.GetDefaultFont();
        labelText.fontSize = 30;
        labelText.alignment = TextAnchor.MiddleCenter;
        labelText.color = new Color(0.42f, 0.27f, 0.14f, 1f);

        // 踰꾪듉 洹몃９ (??쓬, 以묎컙, ?믪쓬)
        string[] tempLabels = { "뜨겁게", "미온", "차갑게" };
        for (int i = 0; i < 3; i++)
        {
            CreateToggleButton(parent, $"TempBtn_{i}", tempLabels[i], -300 + i * 250, 500, i, true);
        }
    }

    private void CreateSteepTimeSection(GameObject parent)
    {
        // ?쇰꺼
        GameObject labelObj = new GameObject("TimeLabel");
        labelObj.transform.SetParent(parent.transform, false);
        RectTransform labelRect = labelObj.AddComponent<RectTransform>();
        labelRect.anchoredPosition = new Vector2(-400, 300);
        labelRect.sizeDelta = new Vector2(300, 60);

        Text labelText = labelObj.AddComponent<Text>();
        labelText.text = "우림 시간";
        labelText.font = FontHelper.GetDefaultFont();
        labelText.fontSize = 30;
        labelText.alignment = TextAnchor.MiddleCenter;
        labelText.color = new Color(0.42f, 0.27f, 0.14f, 1f);

        // 踰꾪듉 洹몃９ (吏㏃쓬, 以묎컙, 湲몄쓬)
        string[] timeLabels = { "짧게", "보통", "길게" };
        for (int i = 0; i < 3; i++)
        {
            CreateToggleButton(parent, $"TimeBtn_{i}", timeLabels[i], -300 + i * 250, 200, i, false);
        }
    }

    private void CreateTeaSelectionSection(GameObject parent)
    {
        // ?쇰꺼
        GameObject labelObj = new GameObject("TeaLabel");
        labelObj.transform.SetParent(parent.transform, false);
        RectTransform labelRect = labelObj.AddComponent<RectTransform>();
        labelRect.anchoredPosition = new Vector2(0, 50);
        labelRect.sizeDelta = new Vector2(600, 60);

        Text labelText = labelObj.AddComponent<Text>();
        labelText.text = "설명";
        labelText.font = FontHelper.GetDefaultFont();
        labelText.fontSize = 30;
        labelText.alignment = TextAnchor.MiddleCenter;
        labelText.color = new Color(0.42f, 0.27f, 0.14f, 1f);

        // 李?踰꾪듉??(媛濡쒕줈 諛곗튂)
        string[] teas = { "yuzu", "matcha", "puerh", "lotus", "chai" };
        float spacing = 200;
        float startX = -(teas.Length - 1) * spacing / 2;

        for (int i = 0; i < teas.Length; i++)
        {
            CreateTeaButton(parent, teas[i], startX + i * spacing, -100);
        }
    }

    private void CreateProgressBar(GameObject parent)
    {
        // 諛곌꼍
        GameObject bgObj = new GameObject("ProgressBg");
        bgObj.transform.SetParent(parent.transform, false);
        RectTransform bgRect = bgObj.AddComponent<RectTransform>();
        bgRect.anchoredPosition = new Vector2(0, -250);
        bgRect.sizeDelta = new Vector2(800, 40);

        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = new Color(0.9f, 0.9f, 0.9f, 1f);

        // 吏꾪뻾瑜?諛?
        GameObject barObj = new GameObject("ProgressBar");
        barObj.transform.SetParent(bgObj.transform, false);
        RectTransform barRect = barObj.AddComponent<RectTransform>();
        barRect.anchorMin = new Vector2(0, 0.5f);
        barRect.anchorMax = new Vector2(0, 0.5f);
        barRect.anchoredPosition = Vector2.zero;
        barRect.sizeDelta = new Vector2(0, 40);

        Image barImage = barObj.AddComponent<Image>();
        barImage.color = new Color(1f, 0.63f, 0.26f, 1f); // 二쇳솴??

        // 吏꾪뻾瑜??낅뜲?댄듃 而댄룷?뚰듃
        ProgressBarUpdater updater = barObj.AddComponent<ProgressBarUpdater>();
        updater.Initialize(barRect);
    }

    private void CreateStartBrewingButton(GameObject parent)
    {
        GameObject btnObj = new GameObject("StartBrewingBtn");
        btnObj.transform.SetParent(parent.transform, false);
        RectTransform btnRect = btnObj.AddComponent<RectTransform>();
        btnRect.anchoredPosition = new Vector2(0, -400);
        btnRect.sizeDelta = new Vector2(400, 80);

        Image btnImage = btnObj.AddComponent<Image>();
        btnImage.color = new Color(1f, 0.63f, 0.26f, 1f); // 二쇳솴??

        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = btnImage;
        ColorBlock colors = btn.colors;
        colors.normalColor = new Color(1f, 0.63f, 0.26f, 1f);
        colors.highlightedColor = new Color(1f, 0.53f, 0.16f, 1f);
        colors.pressedColor = new Color(0.9f, 0.53f, 0.16f, 1f);
        btn.colors = colors;

        // 踰꾪듉 ?띿뒪??
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        Text text = textObj.AddComponent<Text>();
        text.text = "완료";
        text.font = FontHelper.GetDefaultFont();
        text.fontSize = 35;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;

        // 踰꾪듉 ?대깽??
        btn.onClick.AddListener(() =>
        {
            Debug.Log("Start Brewing clicked");
            BrewingManager.Instance.StartBrewing();
        });
    }

    private void CreateBackButton(GameObject parent)
    {
        GameObject btnObj = new GameObject("BackBtn");
        btnObj.transform.SetParent(parent.transform, false);
        RectTransform btnRect = btnObj.AddComponent<RectTransform>();
        btnRect.anchoredPosition = new Vector2(-400, -1050);
        btnRect.sizeDelta = new Vector2(200, 60);

        Image btnImage = btnObj.AddComponent<Image>();
        btnImage.color = new Color(0.8f, 0.8f, 0.8f, 1f);

        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = btnImage;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        Text text = textObj.AddComponent<Text>();
        text.text = "뒤로";
        text.font = FontHelper.GetDefaultFont();
        text.fontSize = 25;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.black;

        btn.onClick.AddListener(() =>
        {
            Debug.Log("Back clicked");
            _uiManager.HidePanel("KitchenPanel");
        });
    }

    private void CreateToggleButton(GameObject parent, string name, string label, float x, float y, int index, bool isTemp)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent.transform, false);
        RectTransform btnRect = btnObj.AddComponent<RectTransform>();
        btnRect.anchoredPosition = new Vector2(x, y);
        btnRect.sizeDelta = new Vector2(200, 80);

        Image btnImage = btnObj.AddComponent<Image>();
        btnImage.color = new Color(0.95f, 0.95f, 0.95f, 1f); // ?고쉶??

        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = btnImage;
        ColorBlock colors = btn.colors;
        colors.normalColor = new Color(0.95f, 0.95f, 0.95f, 1f);
        colors.highlightedColor = new Color(1f, 0.63f, 0.26f, 1f);
        colors.pressedColor = new Color(1f, 0.53f, 0.16f, 1f);
        btn.colors = colors;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        Text text = textObj.AddComponent<Text>();
        text.text = label;
        text.font = FontHelper.GetDefaultFont();
        text.fontSize = 28;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = new Color(0.42f, 0.27f, 0.14f, 1f);

        // ?대┃ ?대깽??
        btn.onClick.AddListener(() =>
        {
            if (isTemp)
            {
                _brewingManager.SetTemperature(index);
            }
            else
            {
                _brewingManager.SetSteepTime(index);
            }
            Debug.Log($"Toggle button clicked: {label}");
        });
    }

    private void CreateTeaButton(GameObject parent, string teaName, float x, float y)
    {
        GameObject btnObj = new GameObject($"Tea_{teaName}");
        btnObj.transform.SetParent(parent.transform, false);
        RectTransform btnRect = btnObj.AddComponent<RectTransform>();
        btnRect.anchoredPosition = new Vector2(x, y);
        btnRect.sizeDelta = new Vector2(150, 80);

        Image btnImage = btnObj.AddComponent<Image>();
        btnImage.color = new Color(0.95f, 0.95f, 0.95f, 1f);

        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = btnImage;
        ColorBlock colors = btn.colors;
        colors.normalColor = new Color(0.95f, 0.95f, 0.95f, 1f);
        colors.highlightedColor = new Color(1f, 0.63f, 0.26f, 1f);
        colors.pressedColor = new Color(1f, 0.53f, 0.16f, 1f);
        btn.colors = colors;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        Text text = textObj.AddComponent<Text>();
        text.text = BrewingManager.GetTeaLabel(teaName);
        text.font = FontHelper.GetDefaultFont();
        text.fontSize = 20;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = new Color(0.42f, 0.27f, 0.14f, 1f);

        // ?대┃ ?대깽??
        btn.onClick.AddListener(() =>
        {
            _brewingManager.SelectTea(teaName);
            Debug.Log($"Tea selected: {teaName}");
        });
    }
}

/// <summary>
/// 吏꾪뻾瑜?諛??낅뜲?댄꽣
/// </summary>
public class ProgressBarUpdater : MonoBehaviour
{
    private RectTransform _barRect;

    public void Initialize(RectTransform barRect)
    {
        _barRect = barRect;
    }

    private void Update()
    {
        if (BrewingManager.Instance == null) return;

        float progress = BrewingManager.Instance.CurrentBrewingData.brewingProgress;
        _barRect.sizeDelta = new Vector2(800 * progress, 40);
    }
}

