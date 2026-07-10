using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// 주방 화면 UI
/// 차 제조 화면 (온도, 우림시간, 재료 선택)
/// </summary>
public class KitchenUIBuilder
{
    private UIManager _uiManager;
    private BrewingManager _brewingManager;
    private Canvas _canvas;
    private GameConstants _constants;

    public KitchenUIBuilder(Canvas canvas, UIManager uiManager)
    {
        _canvas = canvas;
        _uiManager = uiManager;
        _brewingManager = BrewingManager.Instance;
        _constants = GameConstants.Instance;
    }

    public void Build()
    {
        // 기존 패널 제거
        foreach (Transform child in _canvas.transform)
        {
            if (child.name == "KitchenPanel")
            {
                Object.Destroy(child.gameObject);
            }
        }

        // 메인 패널
        GameObject panelObj = new GameObject("KitchenPanel");
        panelObj.transform.SetParent(_canvas.transform, false);
        RectTransform panelRect = panelObj.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0.99f, 0.98f, 0.96f, 1f); // 크림 배경

        // 제목
        CreateTitle(panelObj, "주방");

        // 온도 선택 섹션
        CreateTemperatureSection(panelObj);

        // 우림시간 선택 섹션
        CreateSteepTimeSection(panelObj);

        // 차 선택 섹션
        CreateTeaSelectionSection(panelObj);

        // 진행률 표시
        CreateProgressBar(panelObj);

        // 제조 시작 버튼
        CreateStartBrewingButton(panelObj);

        // 돌아가기 버튼
        CreateBackButton(panelObj);
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
        titleText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        titleText.fontSize = 50;
        titleText.fontStyle = FontStyle.Bold;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = new Color(0.42f, 0.27f, 0.14f, 1f); // 갈색
    }

    private void CreateTemperatureSection(GameObject parent)
    {
        // 라벨
        GameObject labelObj = new GameObject("TempLabel");
        labelObj.transform.SetParent(parent.transform, false);
        RectTransform labelRect = labelObj.AddComponent<RectTransform>();
        labelRect.anchoredPosition = new Vector2(-400, 700);
        labelRect.sizeDelta = new Vector2(300, 60);

        Text labelText = labelObj.AddComponent<Text>();
        labelText.text = "온도";
        labelText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        labelText.fontSize = 30;
        labelText.alignment = TextAnchor.MiddleCenter;
        labelText.color = new Color(0.42f, 0.27f, 0.14f, 1f);

        // 버튼 그룹 (낮음, 중간, 높음)
        string[] tempLabels = { "낮음", "중간", "높음" };
        for (int i = 0; i < 3; i++)
        {
            CreateToggleButton(parent, $"TempBtn_{i}", tempLabels[i], -300 + i * 250, 600, i, true);
        }
    }

    private void CreateSteepTimeSection(GameObject parent)
    {
        // 라벨
        GameObject labelObj = new GameObject("TimeLabel");
        labelObj.transform.SetParent(parent.transform, false);
        RectTransform labelRect = labelObj.AddComponent<RectTransform>();
        labelRect.anchoredPosition = new Vector2(-400, 400);
        labelRect.sizeDelta = new Vector2(300, 60);

        Text labelText = labelObj.AddComponent<Text>();
        labelText.text = "우림시간";
        labelText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        labelText.fontSize = 30;
        labelText.alignment = TextAnchor.MiddleCenter;
        labelText.color = new Color(0.42f, 0.27f, 0.14f, 1f);

        // 버튼 그룹 (짧음, 중간, 길음)
        string[] timeLabels = { "짧음", "중간", "길음" };
        for (int i = 0; i < 3; i++)
        {
            CreateToggleButton(parent, $"TimeBtn_{i}", timeLabels[i], -300 + i * 250, 300, i, false);
        }
    }

    private void CreateTeaSelectionSection(GameObject parent)
    {
        // 라벨
        GameObject labelObj = new GameObject("TeaLabel");
        labelObj.transform.SetParent(parent.transform, false);
        RectTransform labelRect = labelObj.AddComponent<RectTransform>();
        labelRect.anchoredPosition = new Vector2(0, 50);
        labelRect.sizeDelta = new Vector2(600, 60);

        Text labelText = labelObj.AddComponent<Text>();
        labelText.text = "차 선택";
        labelText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        labelText.fontSize = 30;
        labelText.alignment = TextAnchor.MiddleCenter;
        labelText.color = new Color(0.42f, 0.27f, 0.14f, 1f);

        // 차 버튼들 (가로로 배치)
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
        // 배경
        GameObject bgObj = new GameObject("ProgressBg");
        bgObj.transform.SetParent(parent.transform, false);
        RectTransform bgRect = bgObj.AddComponent<RectTransform>();
        bgRect.anchoredPosition = new Vector2(0, -250);
        bgRect.sizeDelta = new Vector2(800, 40);

        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = new Color(0.9f, 0.9f, 0.9f, 1f);

        // 진행률 바
        GameObject barObj = new GameObject("ProgressBar");
        barObj.transform.SetParent(bgObj.transform, false);
        RectTransform barRect = barObj.AddComponent<RectTransform>();
        barRect.anchorMin = new Vector2(0, 0.5f);
        barRect.anchorMax = new Vector2(0, 0.5f);
        barRect.anchoredPosition = Vector2.zero;
        barRect.sizeDelta = new Vector2(0, 40);

        Image barImage = barObj.AddComponent<Image>();
        barImage.color = new Color(1f, 0.63f, 0.26f, 1f); // 주황색

        // 진행률 업데이트 컴포넌트
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
        btnImage.color = new Color(1f, 0.63f, 0.26f, 1f); // 주황색

        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = btnImage;
        ColorBlock colors = btn.colors;
        colors.normalColor = new Color(1f, 0.63f, 0.26f, 1f);
        colors.highlightedColor = new Color(1f, 0.53f, 0.16f, 1f);
        colors.pressedColor = new Color(0.9f, 0.53f, 0.16f, 1f);
        btn.colors = colors;

        // 버튼 텍스트
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        Text text = textObj.AddComponent<Text>();
        text.text = "제조 시작";
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 35;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;

        // 버튼 이벤트
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
        text.text = "돌아가기";
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
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
        btnImage.color = new Color(0.95f, 0.95f, 0.95f, 1f); // 연회색

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
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 28;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = new Color(0.42f, 0.27f, 0.14f, 1f);

        // 클릭 이벤트
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
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 20;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = new Color(0.42f, 0.27f, 0.14f, 1f);

        // 클릭 이벤트
        btn.onClick.AddListener(() =>
        {
            _brewingManager.SelectTea(teaName);
            Debug.Log($"Tea selected: {teaName}");
        });
    }
}

/// <summary>
/// 진행률 바 업데이터
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
