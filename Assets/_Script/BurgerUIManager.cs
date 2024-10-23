using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BurgerUIManager : MonoBehaviour
{
    public gameflow gameflowInstance; // Tham chiếu đến gameflow
    [System.Serializable]
    public class BurgerComponent
    {
        public string name;
        public Sprite componentSprite;
        public int value;
    }


    public Sprite bunBottom;
    public Sprite Meat;
    public Sprite Salad;
    public Sprite Tomato;
    public Sprite bunTop;

    public List<BurgerComponent> allComponents;

    public Transform uiContainer1;
    public Transform uiContainer2;
    public Transform uiContainer3;

    public GameObject componentUIPrefab;

    public Animator recipePanelAnimator1;
    public Animator recipePanelAnimator2;
    public Animator recipePanelAnimator3;

    private List<BurgerComponent> selectedComponents1;
    private List<BurgerComponent> selectedComponents2;
    private List<BurgerComponent> selectedComponents3;

    public Slider timeSlider1;
    public Slider timeSlider2;
    public Slider timeSlider3;

    public Text timeText1;
    public Text timeText2;
    public Text timeText3;

    public float timeRemaining1 = 30f;
    public float timeRemaining2 = 30f;
    public float timeRemaining3 = 30f;

    private bool timerRunning1 = false;
    private bool timerRunning2 = false;
    private bool timerRunning3 = false;

    public int totalValuePlate1 = 0;
    public int totalValuePlate2 = 0;
    public int totalValuePlate3 = 0;

    public GameObject[] characterPrefabs; // Mảng chứa prefab nhân vật
    private GameObject character1; // Nhân vật 1
    private GameObject character2; // Nhân vật 2
    private GameObject character3; // Nhân vật 3

    private bool isRefreshing1 = false; // Kiểm tra xem công thức 1 đang được làm mới hay không
    private bool isRefreshing2 = false; // Kiểm tra xem công thức 2 đang được làm mới hay không
    private bool isRefreshing3 = false; // Kiểm tra xem công thức 3 đang được làm mới hay không

    public AudioClip backgroundMusic; // Âm thanh nền
    public AudioClip refreshSound; // Âm thanh phát khi công thức được làm mới
    private AudioSource audioSource; // Nguồn âm thanh để phát
    private AudioSource audioSource1; // Nguồn âm thanh để phát
    public static BurgerUIManager Instance { get; private set; }
    public int numberOfComponents = 5;

    public GameObject gameOverUI; // Panel Game Over
    public GameObject winUI; // Panel Win
    public GameObject pauseMenu; // Menu tạm dừng
    public Button continueButton; // Nút "Tiếp tục"
    public Button quitButton; // Nút "Thoát"
    public GameObject settingsIcon; // Icon cài đặt
    private bool isPaused = false; // Biến kiểm tra trạng thái tạm dừng

    public Slider volumeSlider; // Slider điều chỉnh âm lượng
    public Text volumeText; // Text hiển thị giá trị âm lượng
    private float volumeValue = 1f; // Giá trị âm lượng mặc định

    private void Start()
    {
        // Kiểm tra nếu audioSource chưa được khởi tạo
        if (audioSource == null)
        {
            audioSource1 = gameObject.AddComponent<AudioSource>();
        }

        // Thiết lập âm thanh nền
        audioSource1.clip = backgroundMusic;
        audioSource1.loop = true;
        audioSource1.Play();

        // Đảm bảo các thiết lập âm lượng ban đầu
        volumeSlider.value = volumeValue;
        UpdateVolumeText();

        // Đăng ký sự kiện khi giá trị slider thay đổi
        volumeSlider.onValueChanged.AddListener(OnVolumeSliderChanged);

        gameflowInstance = FindObjectOfType<gameflow>(); // Lấy instance của gameflow
        allComponents = new List<BurgerComponent>()
        {
            new BurgerComponent() { name = "bunBottom", componentSprite = bunBottom, value = 10000 },
            new BurgerComponent() { name = "Meat", componentSprite = Meat, value = 1000 },
            new BurgerComponent() { name = "Salad", componentSprite = Salad, value = 100 },
            new BurgerComponent() { name = "Tomato", componentSprite = Tomato, value = 10 },
            new BurgerComponent() { name = "bunTop", componentSprite = bunTop, value = 1 },
        };

        GenerateRandomBurger(uiContainer1, out selectedComponents1, recipePanelAnimator1);
        GenerateRandomBurger(uiContainer2, out selectedComponents2, recipePanelAnimator2);
        GenerateRandomBurger(uiContainer3, out selectedComponents3, recipePanelAnimator3);

        // Tạo nhân vật
        CreateCharacters(out character1, new Vector3(-0.2f, -2.88f, 2.9f));
        CreateCharacters(out character2, new Vector3(2.4f, -2.88f, 2.9f));
        CreateCharacters(out character3, new Vector3(4.8f, -2.88f, 2.9f));

        StartTimerForRecipe1();
        StartTimerForRecipe2();
        StartTimerForRecipe3();

        audioSource = gameObject.AddComponent<AudioSource>();

        if (MenuController.selectedDifficulty == 0)
        {
            SetBurgerComponentCount(5); // Chế độ "Easy" - 5 thành phần
        }
        else
        {
            SetBurgerComponentCount(7); // Chế độ "Hard" - 7 thành phần
        }
    }

    private void OnVolumeSliderChanged(float value)
    {
        volumeValue = value; // Cập nhật giá trị âm lượng
        audioSource1.volume = volumeValue; // Cập nhật âm lượng âm thanh
        UpdateVolumeText(); // Cập nhật hiển thị âm lượng
    }

    private void UpdateVolumeText()
    {
        volumeText.text = "Volume: " + Mathf.RoundToInt(volumeValue * 100) + "%"; // Cập nhật hiển thị âm lượng
    }

    public void OnSettingsIconClick()
    {
        if (!isPaused)
        {
            PauseGame(); // Tạm dừng game
        }
        else
        {
            ContinueGame(); // Tiếp tục game nếu đã tạm dừng
        }
    }

    // Hàm để tạm dừng game
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0; // Dừng thời gian
        pauseMenu.SetActive(true); // Hiển thị menu tạm dừng
        settingsIcon.SetActive(false); // Ẩn icon cài đặt
    }

    // Hàm để tiếp tục game
    public void ContinueGame()
    {
        isPaused = false;
        Time.timeScale = 1; // Tiếp tục thời gian
        pauseMenu.SetActive(false); // Ẩn menu tạm dừng
        settingsIcon.SetActive(true); // Hiển thị lại icon cài đặt
    }

    // Hàm để quay lại menu chính
    public void QuitToMainMenu()
    {
        Time.timeScale = 1; // Khôi phục thời gian khi quay lại menu
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single); // Quay lại main menu
    }
    public void ResetGame()
    {
        // Đặt lại các biến liên quan đến thời gian và trạng thái UI
        timeRemaining1 = 30f;
        timeRemaining2 = 30f;
        timeRemaining3 = 30f;
        timerRunning1 = false;
        timerRunning2 = false;
        timerRunning3 = false;

        // Xóa các thành phần burger đã chọn
        selectedComponents1 = new List<BurgerComponent>();
        selectedComponents2 = new List<BurgerComponent>();
        selectedComponents3 = new List<BurgerComponent>();

        // Reset giá trị đĩa
        totalValuePlate1 = 0;
        totalValuePlate2 = 0;
        totalValuePlate3 = 0;

        // Khởi tạo lại các burger
        GenerateRandomBurger(uiContainer1, out selectedComponents1, recipePanelAnimator1);
        GenerateRandomBurger(uiContainer2, out selectedComponents2, recipePanelAnimator2);
        GenerateRandomBurger(uiContainer3, out selectedComponents3, recipePanelAnimator3);
    }

    public void StartGame()
    {
        // Đảm bảo reset game trước khi bắt đầu lại
        if (BurgerUIManager.Instance != null)
        {
            BurgerUIManager.Instance.ResetGame();
        }

        Time.timeScale = 1;  // Khôi phục thời gian
        SceneManager.LoadScene("food", LoadSceneMode.Single);  // Tải lại scene trò chơi
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);  // Nếu đã có instance, xóa đối tượng hiện tại
        }
    }


    private void CreateCharacters(out GameObject character, Vector3 position)
    {
        // Tạo nhân vật mới tại vị trí cố định
        character = Instantiate(characterPrefabs[Random.Range(0, characterPrefabs.Length)], position, Quaternion.Euler(0, 180, 0));
        character.transform.localScale = new Vector3(1.5f, 2f, 1.5f); // Đặt scale cho nhân vật
    }

    private void Update()
    {
        UpdateTimers();
    }

    private void UpdateTimers()
    {
        if (timerRunning1)
        {
            UpdateTimer(ref timeRemaining1, timeSlider1, timeText1, RefreshRecipe1);
        }
        if (timerRunning2)
        {
            UpdateTimer(ref timeRemaining2, timeSlider2, timeText2, RefreshRecipe2);
        }
        if (timerRunning3)
        {
            UpdateTimer(ref timeRemaining3, timeSlider3, timeText3, RefreshRecipe3);
        }
    }

    private void UpdateTimer(ref float timeRemaining, Slider slider, Text text, System.Action refreshMethod)
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerUI(timeRemaining, slider, text);
        }
        else
        {
            timeRemaining = 0;
            refreshMethod(); // Gọi hàm làm mới công thức
        }
    }

    void UpdateTimerUI(float time, Slider slider, Text text)
    {
        slider.value = time;
        text.text = Mathf.Round(time).ToString() + "s";
    }

    public void StartTimerForRecipe1()
    {
        timerRunning1 = true;
        timeRemaining1 = 30f; // Thiết lập thời gian ban đầu
    }

    public void StartTimerForRecipe2()
    {
        timerRunning2 = true;
        timeRemaining2 = 30f; // Thiết lập thời gian ban đầu
    }

    public void StartTimerForRecipe3()
    {
        timerRunning3 = true;
        timeRemaining3 = 30f; // Thiết lập thời gian ban đầu
    }

    public void RefreshRecipe1()
    {
        if (isRefreshing1) return; // Kiểm tra xem có đang làm mới hay không
        isRefreshing1 = true; // Đánh dấu đang làm mới


        // Xóa nhân vật khi làm mới công thức
        Destroy(character1);
        StartCoroutine(WaitAndGenerate(uiContainer1, recipePanelAnimator1, timeSlider1, timeText1));
    }

    public void RefreshRecipe2()
    {
        if (isRefreshing2) return; // Kiểm tra xem có đang làm mới hay không
        isRefreshing2 = true; // Đánh dấu đang làm mới


        // Xóa nhân vật khi làm mới công thức
        Destroy(character2);
        StartCoroutine(WaitAndGenerate(uiContainer2, recipePanelAnimator2, timeSlider2, timeText2));
    }

    public void RefreshRecipe3()
    {
        if (isRefreshing3) return; // Kiểm tra xem có đang làm mới hay không
        isRefreshing3 = true; // Đánh dấu đang làm mới


        // Xóa nhân vật khi làm mới công thức
        Destroy(character3);
        StartCoroutine(WaitAndGenerate(uiContainer3, recipePanelAnimator3, timeSlider3, timeText3));
    }

    private IEnumerator WaitAndGenerate(Transform uiContainer, Animator panelAnimator, Slider slider, Text text)
    {
        // Ẩn UI thời gian và công thức
        HideRecipeAndTimeUI(uiContainer, slider.gameObject, text.gameObject);

        float waitTime = Random.Range(1f, 10f); // Thời gian chờ ngẫu nhiên
        yield return new WaitForSeconds(waitTime); // Đợi

        // Xóa UI hiện tại trước khi tạo mới
        foreach (Transform child in uiContainer)
        {
            Destroy(child.gameObject);
        }

        audioSource.PlayOneShot(refreshSound);

        List<BurgerComponent> selectedComponents = new List<BurgerComponent>(); // Biến cục bộ để lưu thành phần
        GenerateRandomBurger(uiContainer, out selectedComponents, panelAnimator); // Sinh ra công thức mới

        // Thiết lập lại thời gian cho UI đang được làm mới
        ResetTimer(slider, text);

        // Tạo lại nhân vật mới
        if (uiContainer == uiContainer1)
        {
            CreateCharacters(out character1, new Vector3(-0.2f, -2.88f, 2.9f));
        }
        else if (uiContainer == uiContainer2)
        {
            CreateCharacters(out character2, new Vector3(2.4f, -2.88f, 2.9f));
        }
        else if (uiContainer == uiContainer3)
        {
            CreateCharacters(out character3, new Vector3(4.8f, -2.88f, 2.9f));
        }

        // Hiện lại UI thời gian và công thức
        ShowRecipeAndTimeUI(uiContainer, slider.gameObject, text.gameObject);

        // Đánh dấu đã hoàn thành việc làm mới
        if (uiContainer == uiContainer1) isRefreshing1 = false;
        else if (uiContainer == uiContainer2) isRefreshing2 = false;
        else if (uiContainer == uiContainer3) isRefreshing3 = false;

        Debug.Log("Công thức được làm mới sau " + waitTime + " giây!");
    }

    private void ResetTimer(Slider slider, Text text)
    {
        // Thiết lập lại thời gian cho UI cụ thể
        if (slider == timeSlider1)
        {
            timeRemaining1 = 30f;
            timerRunning1 = true;
        }
        else if (slider == timeSlider2)
        {
            timeRemaining2 = 30f;
            timerRunning2 = true;
        }
        else if (slider == timeSlider3)
        {
            timeRemaining3 = 30f;
            timerRunning3 = true;
        }
        UpdateTimerUI(30f, slider, text);
    }

    private void GenerateRandomBurger(Transform uiContainer, out List<BurgerComponent> selectedComponents, Animator panelAnimator)
    {
        selectedComponents = new List<BurgerComponent>();

        foreach (Transform child in uiContainer)
        {
            Destroy(child.gameObject); // Xóa các thành phần cũ
        }

        int totalValue = 0;
        float yOffset = 0f;
        float spacing = 100f;
        selectedComponents.Add(allComponents.Find(c => c.name == "bunTop"));

        for (int i = 0; i <= numberOfComponents - 2; i++)
        {
            BurgerComponent componentToAdd;

            if (i % 2 == 1)
            {
                componentToAdd = allComponents.Find(c => c.name == "bunBottom");
            }
            else
            {
                int randomIndex = Random.Range(1, allComponents.Count - 1);
                componentToAdd = allComponents[randomIndex];
            }

            selectedComponents.Add(componentToAdd);
            totalValue += componentToAdd.value;
        }

        // Hiển thị các thành phần trong UI
        foreach (BurgerComponent component in selectedComponents)
        {
            GameObject newUIComponent = Instantiate(componentUIPrefab, uiContainer);
            newUIComponent.GetComponent<Image>().sprite = component.componentSprite;
            newUIComponent.name = component.name;

            RectTransform rectTransform = newUIComponent.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0, -yOffset);
            yOffset += spacing;
        }

        // Lưu giá trị công thức cho từng đĩa
        if (uiContainer == uiContainer1)
        {
            selectedComponents1 = selectedComponents; // Cập nhật danh sách cho công thức 1
            totalValuePlate1 = totalValue + 1;
        }
        else if (uiContainer == uiContainer2)
        {
            selectedComponents2 = selectedComponents; // Cập nhật danh sách cho công thức 2
            totalValuePlate2 = totalValue + 1;
        }
        else if (uiContainer == uiContainer3)
        {
            selectedComponents3 = selectedComponents; // Cập nhật danh sách cho công thức 3
            totalValuePlate3 = totalValue + 1;
        }

        //panelAnimator.SetTrigger("Show"); // Hiện công thức
    }


    private void HideRecipeAndTimeUI(Transform uiContainer, GameObject slider, GameObject text)
    {
        slider.SetActive(false);
        text.SetActive(false);
        foreach (Transform child in uiContainer)
        {
            child.gameObject.SetActive(false);
        }
    }

    private void ShowRecipeAndTimeUI(Transform uiContainer, GameObject slider, GameObject text)
    {
        slider.SetActive(true);
        text.SetActive(true);
        foreach (Transform child in uiContainer)
        {
            child.gameObject.SetActive(true);
        }
    }
    public void SetBurgerComponentCount(int count)
    {
        numberOfComponents = count; // Cập nhật số lượng thành phần
                                    // Nếu cần, bạn có thể gọi hàm để tạo lại burger với số lượng mới
        GenerateRandomBurger(uiContainer1, out selectedComponents1, recipePanelAnimator1);
        GenerateRandomBurger(uiContainer2, out selectedComponents2, recipePanelAnimator2);
        GenerateRandomBurger(uiContainer3, out selectedComponents3, recipePanelAnimator3);
    }



    public List<BurgerComponent> GetSelectedComponents1() => selectedComponents1;
    public List<BurgerComponent> GetSelectedComponents2() => selectedComponents2;
    public List<BurgerComponent> GetSelectedComponents3() => selectedComponents3;
}
