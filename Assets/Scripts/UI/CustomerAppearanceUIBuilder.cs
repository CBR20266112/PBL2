using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 손님 등장 화면
/// 손님 스프라이트, 대사, 선호도 표시
/// </summary>
public class CustomerAppearanceUIBuilder
{
    private UIManager _uiManager;
    private CustomerManager _customerManager;
    private Canvas _canvas;

    public CustomerAppearanceUIBuilder(Canvas canvas, UIManager uiManager)
    {
        _canvas = canvas;
        _uiManager = uiManager;
        _customerManager = CustomerManager.Instance;
    }

    public void Build()
    {
        // 기존 패널 제거
        foreach (Transform child in _canvas.transform)
        {
            if (child.name == "CustomerPanel")
            {
                Object.Destroy(child.gameObject);
            }
        }

        // 메인 패널
        GameObject panelObj = new GameObject("CustomerPanel");
        panelObj.transform.SetParent(_canvas.transform, false);
        RectTransform panelRect = panelObj.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0.99f, 0.98f, 0.96f, 1f); // 크림 배경

        // 손님 호출
        Customer customer = _customerManager.CallNextCustomer();
        if (customer == null)
        {
            Debug.LogError("No customer to display!");
            return;
        }

        // 손님 캐릭터 표시 (플레이스홀더: 컬러 박스)
        CreateCustomerDisplay(panelObj, customer);

        // 손님 이름
        CreateCustomerName(panelObj, customer);

        // 손님 대사
        CreateCustomerDialogue(panelObj, customer);

        // 선호도 표시
        CreatePreferences(panelObj, customer);

        // 주방으로 가기 버튼
        CreateCookButton(panelObj);

        // 거절 버튼
        CreateDeclineButton(panelObj);
    }

    private void CreateCustomerDisplay(GameObject parent, Customer customer)
    {
        GameObject displayObj = new GameObject("CustomerDisplay");
        displayObj.transform.SetParent(parent.transform, false);
        RectTransform displayRect = displayObj.AddComponent<RectTransform>();
        displayRect.anchoredPosition = new Vector2(0, 400);
        displayRect.sizeDelta = new Vector2(300, 300);

        Image displayImage = displayObj.AddComponent<Image>();
        
        // 캐릭터 타입별 색상
        Color characterColor = GetCharacterColor(customer.data.characterType);
        displayImage.color = characterColor;

        // 캐릭터 이름 텍스트
        GameObject textObj = new GameObject("CharacterType");
        textObj.transform.SetParent(displayObj.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        Text text = textObj.AddComponent<Text>();
        text.text = customer.data.characterType.ToUpper();
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 40;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
    }

    private void CreateCustomerName(GameObject parent, Customer customer)
    {
        GameObject nameObj = new GameObject("CustomerName");
        nameObj.transform.SetParent(parent.transform, false);
        RectTransform nameRect = nameObj.AddComponent<RectTransform>();
        nameRect.anchoredPosition = new Vector2(0, 250);
        nameRect.sizeDelta = new Vector2(600, 80);

        Text nameText = nameObj.AddComponent<Text>();
        nameText.text = $"안녕하세요! {customer.data.customerName}입니다.";
        nameText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        nameText.fontSize = 40;
        nameText.fontStyle = FontStyle.Bold;
        nameText.alignment = TextAnchor.MiddleCenter;
        nameText.color = new Color(0.42f, 0.27f, 0.14f, 1f);
    }

    private void CreateCustomerDialogue(GameObject parent, Customer customer)
    {
        GameObject dialogueObj = new GameObject("Dialogue");
        dialogueObj.transform.SetParent(parent.transform, false);
        RectTransform dialogueRect = dialogueObj.AddComponent<RectTransform>();
        dialogueRect.anchoredPosition = new Vector2(0, 100);
        dialogueRect.sizeDelta = new Vector2(800, 150);

        Text dialogueText = dialogueObj.AddComponent<Text>();
        dialogueText.text = $"\"{customer.data.storyBit}\"";
        dialogueText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        dialogueText.fontSize = 32;
        dialogueText.alignment = TextAnchor.MiddleCenter;
        dialogueText.color = new Color(0.42f, 0.27f, 0.14f, 1f);
        
        // 텍스트 줄 바꿈 허용
        dialogueText.horizontalOverflow = HorizontalWrapMode.Wrap;
        dialogueText.verticalOverflow = VerticalWrapMode.Truncate;
    }

    private void CreatePreferences(GameObject parent, Customer customer)
    {
        GameObject prefObj = new GameObject("PreferencesPanel");
        prefObj.transform.SetParent(parent.transform, false);
        RectTransform prefRect = prefObj.AddComponent<RectTransform>();
        prefRect.anchoredPosition = new Vector2(0, -200);
        prefRect.sizeDelta = new Vector2(800, 200);

        // 라벨
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(prefObj.transform, false);
        RectTransform labelRect = labelObj.AddComponent<RectTransform>();
        labelRect.anchoredPosition = new Vector2(0, 80);
        labelRect.sizeDelta = new Vector2(600, 60);

        Text labelText = labelObj.AddComponent<Text>();
        labelText.text = "선호도";
        labelText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        labelText.fontSize = 35;
        labelText.fontStyle = FontStyle.Bold;
        labelText.alignment = TextAnchor.MiddleCenter;
        labelText.color = new Color(0.42f, 0.27f, 0.14f, 1f);

        // 차
        CreatePreferenceItem(prefObj, "선호하는 차", BrewingManager.GetTeaLabel(customer.data.preferredTea), -350, 0);

        // 온도
        CreatePreferenceItem(prefObj, "온도", BrewingManager.GetTemperatureLabel(customer.data.preferredTemperature), 0, 0);

        // 우림시간
        CreatePreferenceItem(prefObj, "우림시간", BrewingManager.GetSteepTimeLabel(customer.data.preferredSteepTime), 350, 0);

        // 성격
        CreatePreferenceItem(prefObj, "성격", customer.data.personality, 0, -80);
    }

    private void CreatePreferenceItem(GameObject parent, string title, string value, float x, float y)
    {
        GameObject itemObj = new GameObject("PrefItem");
        itemObj.transform.SetParent(parent.transform, false);
        RectTransform itemRect = itemObj.AddComponent<RectTransform>();
        itemRect.anchoredPosition = new Vector2(x, y);
        itemRect.sizeDelta = new Vector2(250, 100);

        // 제목
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(itemObj.transform, false);
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchoredPosition = new Vector2(0, 20);
        titleRect.sizeDelta = new Vector2(250, 40);

        Text titleText = titleObj.AddComponent<Text>();
        titleText.text = title;
        titleText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        titleText.fontSize = 22;
        titleText.fontStyle = FontStyle.Bold;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = new Color(0.42f, 0.27f, 0.14f, 1f);

        // 값
        GameObject valueObj = new GameObject("Value");
        valueObj.transform.SetParent(itemObj.transform, false);
        RectTransform valueRect = valueObj.AddComponent<RectTransform>();
        valueRect.anchoredPosition = new Vector2(0, -20);
        valueRect.sizeDelta = new Vector2(250, 40);

        Text valueText = valueObj.AddComponent<Text>();
        valueText.text = value;
        valueText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        valueText.fontSize = 28;
        valueText.fontStyle = FontStyle.Bold;
        valueText.alignment = TextAnchor.MiddleCenter;
        valueText.color = new Color(1f, 0.63f, 0.26f, 1f); // 주황색
    }

    private void CreateCookButton(GameObject parent)
    {
        GameObject btnObj = new GameObject("CookBtn");
        btnObj.transform.SetParent(parent.transform, false);
        RectTransform btnRect = btnObj.AddComponent<RectTransform>();
        btnRect.anchoredPosition = new Vector2(200, -900);
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
        text.text = "주방으로 가기";
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 35;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;

        btn.onClick.AddListener(() =>
        {
            Debug.Log("주방으로 가기");
            // 손님 등장 화면 숨기기
            _uiManager.HidePanel("CustomerPanel");
                
            // 주방 화면 표시
            Canvas canvas = UnityEngine.Object.FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                KitchenUIBuilder kitchenUI = new KitchenUIBuilder(canvas, _uiManager);
                kitchenUI.Build();
            }
        });
    }

    private void CreateDeclineButton(GameObject parent)
    {
        GameObject btnObj = new GameObject("DeclineBtn");
        btnObj.transform.SetParent(parent.transform, false);
        RectTransform btnRect = btnObj.AddComponent<RectTransform>();
        btnRect.anchoredPosition = new Vector2(-200, -900);
        btnRect.sizeDelta = new Vector2(400, 80);

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
        text.text = "거절";
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 35;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.black;

        btn.onClick.AddListener(() =>
        {
            Debug.Log("손님 거절");
            CustomerManager.Instance.RemoveCurrentCustomer();
            _uiManager.HidePanel("CustomerPanel");
        });
    }

    private Color GetCharacterColor(string characterType)
    {
        return characterType switch
        {
            "luna" => new Color(1f, 0.9f, 0.7f, 1f), // 황금색
            "hyuntae" => new Color(1f, 0.8f, 0.6f, 1f), // 주황색
            "wei" => new Color(0.8f, 0.9f, 0.8f, 1f), // 연초록색
            "sakura" => new Color(1f, 0.85f, 0.9f, 1f), // 분홍색
            "denu" => new Color(0.9f, 0.85f, 0.8f, 1f), // 베이지색
            _ => new Color(0.9f, 0.9f, 0.9f, 1f) // 회색
        };
    }
}
