using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 평가 화면 UI
/// 별점, 보상 (돈/경험치), 다음 버튼
/// </summary>
public class RatingUIBuilder
{
    private UIManager _uiManager;
    private BrewingManager _brewingManager;
    private CustomerManager _customerManager;
    private PlayerDataManager _playerDataManager;
    private Canvas _canvas;
    private GameConstants _constants;

    public RatingUIBuilder(Canvas canvas, UIManager uiManager)
    {
        _canvas = canvas;
        _uiManager = uiManager;
        _brewingManager = BrewingManager.Instance;
        _customerManager = CustomerManager.Instance;
        _playerDataManager = PlayerDataManager.Instance;
        _constants = GameConstants.Instance;
    }

    public void Build()
    {
        // 기존 패널 제거
        foreach (Transform child in _canvas.transform)
        {
            if (child.name == "RatingPanel")
            {
                Object.Destroy(child.gameObject);
            }
        }

        // 메인 패널
        GameObject panelObj = new GameObject("RatingPanel");
        panelObj.transform.SetParent(_canvas.transform, false);
        RectTransform panelRect = panelObj.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0.99f, 0.98f, 0.96f, 1f); // 크림 배경

        // 제목
        CreateTitle(panelObj);

        // 손님 이름
        CreateCustomerName(panelObj);

        // 별점 표시
        CreateRating(panelObj);

        // 보상 정보
        CreateRewards(panelObj);

        // 다음 버튼
        CreateNextButton(panelObj);
    }

    private void CreateTitle(GameObject parent)
    {
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(parent.transform, false);
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchoredPosition = new Vector2(0, 900);
        titleRect.sizeDelta = new Vector2(1080, 100);

        Text titleText = titleObj.AddComponent<Text>();
        titleText.text = "평가";
        titleText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        titleText.fontSize = 50;
        titleText.fontStyle = FontStyle.Bold;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = new Color(0.42f, 0.27f, 0.14f, 1f);
    }

    private void CreateCustomerName(GameObject parent)
    {
        Customer customer = _customerManager.GetCurrentCustomer();
        if (customer == null) return;

        GameObject nameObj = new GameObject("CustomerName");
        nameObj.transform.SetParent(parent.transform, false);
        RectTransform nameRect = nameObj.AddComponent<RectTransform>();
        nameRect.anchoredPosition = new Vector2(0, 750);
        nameRect.sizeDelta = new Vector2(600, 80);

        Text nameText = nameObj.AddComponent<Text>();
        nameText.text = $"{customer.data.customerName}님이 선택한 차:";
        nameText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        nameText.fontSize = 35;
        nameText.alignment = TextAnchor.MiddleCenter;
        nameText.color = new Color(0.42f, 0.27f, 0.14f, 1f);
    }

    private void CreateRating(GameObject parent)
    {
        int quality = _brewingManager.CurrentBrewingData.brewingQuality;

        // 별점 라벨
        GameObject labelObj = new GameObject("RatingLabel");
        labelObj.transform.SetParent(parent.transform, false);
        RectTransform labelRect = labelObj.AddComponent<RectTransform>();
        labelRect.anchoredPosition = new Vector2(0, 600);
        labelRect.sizeDelta = new Vector2(600, 60);

        Text labelText = labelObj.AddComponent<Text>();
        labelText.text = "평가";
        labelText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        labelText.fontSize = 40;
        labelText.fontStyle = FontStyle.Bold;
        labelText.alignment = TextAnchor.MiddleCenter;
        labelText.color = new Color(0.42f, 0.27f, 0.14f, 1f);

        // 별 표시 (텍스트로)
        GameObject starsObj = new GameObject("Stars");
        starsObj.transform.SetParent(parent.transform, false);
        RectTransform starsRect = starsObj.AddComponent<RectTransform>();
        starsRect.anchoredPosition = new Vector2(0, 480);
        starsRect.sizeDelta = new Vector2(600, 100);

        Text starsText = starsObj.AddComponent<Text>();
        string starDisplay = new string('★', quality) + new string('☆', 5 - quality);
        starsText.text = starDisplay;
        starsText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        starsText.fontSize = 80;
        starsText.alignment = TextAnchor.MiddleCenter;
        starsText.color = new Color(1f, 0.63f, 0.26f, 1f); // 주황색

        // 평가 메시지
        string message = quality switch
        {
            5 => "완벽해요! 또 와야겠어요!",
            4 => "정말 맛있어요! 감사합니다!",
            3 => "괜찮네요. 고마워요.",
            2 => "흠... 별로네요.",
            _ => "이게 뭐죠?"
        };

        GameObject messageObj = new GameObject("Message");
        messageObj.transform.SetParent(parent.transform, false);
        RectTransform messageRect = messageObj.AddComponent<RectTransform>();
        messageRect.anchoredPosition = new Vector2(0, 350);
        messageRect.sizeDelta = new Vector2(800, 80);

        Text messageText = messageObj.AddComponent<Text>();
        messageText.text = $"\"{message}\"";
        messageText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        messageText.fontSize = 32;
        messageText.alignment = TextAnchor.MiddleCenter;
        messageText.color = new Color(0.42f, 0.27f, 0.14f, 1f);
        messageText.horizontalOverflow = HorizontalWrapMode.Wrap;
    }

    private void CreateRewards(GameObject parent)
    {
        int quality = _brewingManager.CurrentBrewingData.brewingQuality;
        
        // 기본 보상 (GameConstants 참고)
        int baseReward = 1000; // TODO: GameConstants에서 가져오기
        int moneyReward = baseReward + (quality * 500);
        int expReward = 100 + (quality * 50);

        // 배경
        GameObject bgObj = new GameObject("RewardBg");
        bgObj.transform.SetParent(parent.transform, false);
        RectTransform bgRect = bgObj.AddComponent<RectTransform>();
        bgRect.anchoredPosition = new Vector2(0, 150);
        bgRect.sizeDelta = new Vector2(800, 200);

        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = new Color(1f, 0.9f, 0.8f, 1f);

        // 라벨
        GameObject labelObj = new GameObject("RewardLabel");
        labelObj.transform.SetParent(bgObj.transform, false);
        RectTransform labelRect = labelObj.AddComponent<RectTransform>();
        labelRect.anchoredPosition = new Vector2(0, 70);
        labelRect.sizeDelta = new Vector2(600, 50);

        Text labelText = labelObj.AddComponent<Text>();
        labelText.text = "보상";
        labelText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        labelText.fontSize = 35;
        labelText.fontStyle = FontStyle.Bold;
        labelText.alignment = TextAnchor.MiddleCenter;
        labelText.color = new Color(0.42f, 0.27f, 0.14f, 1f);

        // 돈
        GameObject moneyObj = new GameObject("Money");
        moneyObj.transform.SetParent(bgObj.transform, false);
        RectTransform moneyRect = moneyObj.AddComponent<RectTransform>();
        moneyRect.anchoredPosition = new Vector2(-200, 0);
        moneyRect.sizeDelta = new Vector2(300, 60);

        Text moneyText = moneyObj.AddComponent<Text>();
        moneyText.text = $"💰 {moneyReward} 원";
        moneyText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        moneyText.fontSize = 30;
        moneyText.fontStyle = FontStyle.Bold;
        moneyText.alignment = TextAnchor.MiddleCenter;
        moneyText.color = new Color(1f, 0.63f, 0.26f, 1f);

        // 경험치
        GameObject expObj = new GameObject("Exp");
        expObj.transform.SetParent(bgObj.transform, false);
        RectTransform expRect = expObj.AddComponent<RectTransform>();
        expRect.anchoredPosition = new Vector2(200, 0);
        expRect.sizeDelta = new Vector2(300, 60);

        Text expText = expObj.AddComponent<Text>();
        expText.text = $"⭐ {expReward} EXP";
        expText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        expText.fontSize = 30;
        expText.fontStyle = FontStyle.Bold;
        expText.alignment = TextAnchor.MiddleCenter;
        expText.color = new Color(1f, 0.63f, 0.26f, 1f);

        // 보상 적용
        ApplyRewards(moneyReward, expReward);
    }

    private void ApplyRewards(int money, int exp)
    {
        _playerDataManager.AddMoney(money);
        _playerDataManager.AddExp(exp);
        
        Customer customer = _customerManager.GetCurrentCustomer();
        if (customer != null)
        {
            customer.IncreaseFamiliarity();
        }

        Debug.Log($"Rewards applied: +{money} money, +{exp} exp");
    }

    private void CreateNextButton(GameObject parent)
    {
        GameObject btnObj = new GameObject("NextBtn");
        btnObj.transform.SetParent(parent.transform, false);
        RectTransform btnRect = btnObj.AddComponent<RectTransform>();
        btnRect.anchoredPosition = new Vector2(0, -900);
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

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        Text text = textObj.AddComponent<Text>();
        text.text = "계속";
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 35;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;

        btn.onClick.AddListener(() =>
        {
            Debug.Log("Next button clicked");
            
            // 손님 제거
            _customerManager.RemoveCurrentCustomer();
            
            // 제조 초기화
            _brewingManager.ResetBrewing();
            
            // 평가 화면 숨기기
            _uiManager.HidePanel("RatingPanel");
        });
    }
}
