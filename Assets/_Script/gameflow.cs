using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Import để chuyển đổi scene

public class gameflow : MonoBehaviour
{
    public static int[] orderValue = { 0, 0, 0 }; // Giá trị đơn hàng cho từng đĩa
    public static int[] plateValue = { 0, 0, 0 }; // Giá trị thức ăn trên từng đĩa
    public static int plateNum = 0; // Số đĩa hiện tại
    public static float plateXpos = 0; // Vị trí X của đĩa
    public Transform plateSelector; // Đối tượng chỉ định đĩa

    public static float emptyPlateNow = -1; // Giá trị để kiểm tra đĩa rỗng

    private const int maxPlates = 3; // Số lượng đĩa tối đa
    public static List<string> globalClickOrder = new List<string>();

    public Text scoreText; // Text để hiển thị điểm
    public static float totalScore; // Điểm tổng
    public Text targetPointText; // Text để hiển thị điểm mục tiêu trên UI
    public static float remainScore; // Điểm còn lại

    // Thêm hai UI cho "Game Over" và "Win"
    public GameObject gameOverUI;
    public GameObject winUI;
    public Button backToMenuButton; // Nút quay lại menu
    public Button backToMenuButton1;

    void Start()
    {
        // Khởi tạo giá trị cho các đĩa và điểm
        ResetPlates();
        totalScore = 0f; // Khởi tạo điểm
        UpdateScoreUI(); // Cập nhật UI cho điểm
        remainScore = MenuController.selectedPointLimit;
        UpdateRemainScoreUI();

        // Ẩn UI "Game Over" và "Win"
        gameOverUI.SetActive(false);
        winUI.SetActive(false);

        // Gắn sự kiện nút "Back to Menu"
        backToMenuButton.onClick.AddListener(BackToMenu);
        backToMenuButton1.onClick.AddListener(BackToMenu);
        gameflow.globalClickOrder.Clear();  // Reset thứ tự click khi bắt đầu lại trò chơi
    }

    void Update()
    {
        //HandlePlateSelection();
        //OnPlateButtonClicked();
        UpdatePlateSelectorPosition();
    }

    // Hàm điều chỉnh số đĩa
    //private void HandlePlateSelection()
    //{
    //    if (Input.GetKeyDown("tab"))
    //    {
    //        plateNum++;
    //        if (plateNum >= maxPlates)
    //        {
    //            plateNum = 0; // Quay lại đĩa đầu tiên
    //        }
    //        plateXpos = plateNum * 2.5f; // Cập nhật vị trí X dựa trên số đĩa

    //        // Cập nhật giá trị thức ăn trên đĩa khi chọn đĩa
    //        UpdatePlateValue();
    //    }
    //}


    // Hàm gọi khi nhấn nút trên BurgerUI
    public void OnPlateButtonClicked()
    {
        plateNum++;
        if (plateNum >= maxPlates)
        {
            plateNum = 0; // Quay lại đĩa đầu tiên
        }
        plateXpos = plateNum * 2.5f; // Cập nhật vị trí X dựa trên số đĩa

        // Cập nhật giá trị thức ăn trên đĩa khi chọn đĩa
        UpdatePlateValue();
    }

    // Cập nhật giá trị thức ăn trên đĩa
    private void UpdatePlateValue()
    {
        // Ví dụ, bạn có thể làm gì đó với giá trị thức ăn hiện tại
        Debug.Log($"Giá trị thức ăn trên đĩa {plateNum}: {plateValue[plateNum]}");
    }

    // Cập nhật vị trí của đối tượng chỉ định đĩa
    private void UpdatePlateSelectorPosition()
    {
        plateSelector.transform.position = new Vector3(plateXpos, -0.15f, 0);
    }

    // Hàm khôi phục trạng thái cho tất cả các đĩa
    public static void ResetPlates()
    {
        for (int i = 0; i < plateValue.Length; i++)
        {
            plateValue[i] = 0; // Đặt lại giá trị thức ăn trên từng đĩa
            orderValue[i] = 0; // Đặt lại giá trị đơn hàng cho từng đĩa
        }
    }

    // Cập nhật điểm
    public void UpdateScore(float score)
    {
        totalScore += score; // Cộng dồn điểm
        remainScore -= totalScore; // Cập nhật điểm còn lại
        UpdateScoreUI(); // Cập nhật UI cho điểm
        UpdateRemainScoreUI(); // Cập nhật UI cho điểm còn lại
        CheckScoreLimits(); // Kiểm tra điều kiện giới hạn điểm
    }

    // Cập nhật điểm trên UI
    public void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + Mathf.Round(totalScore).ToString(); // Cập nhật điểm tổng
        }
        else
        {
            Debug.LogError("scoreText is not assigned in the Inspector!"); // Thông báo lỗi nếu chưa gán
        }
    }

    public void UpdateRemainScore(float score)
    {
        remainScore = MenuController.selectedPointLimit;
        UpdateRemainScoreUI(); // Cập nhật UI cho điểm
    }

    // Cập nhật điểm trên UI
    public void UpdateRemainScoreUI()
    {
        if (targetPointText != null)
        {
            targetPointText.text = "Target: " + Mathf.Round(remainScore).ToString(); // Cập nhật điểm tổng
        }
        else
        {
            Debug.LogError("remainText is not assigned in the Inspector!"); // Thông báo lỗi nếu chưa gán
        }
    }

    // Kiểm tra giới hạn điểm
    private void CheckScoreLimits()
    {
        if (totalScore < 0)
        {
            GameOver(); // Hiển thị UI "Game Over"
        }
        else if (totalScore > 20000)
        {
            Win(); // Hiển thị UI "You Win"
        }
    }

    // Hiển thị UI Game Over và tạm dừng game
    private void GameOver()
    {
        Time.timeScale = 0; // Dừng thời gian
        gameOverUI.SetActive(true); // Hiển thị UI Game Over
    }

    // Hiển thị UI You Win và tạm dừng game
    private void Win()
    {
        Time.timeScale = 0; // Dừng thời gian
        winUI.SetActive(true); // Hiển thị UI Win
    }

    // Hàm quay lại menu
    public void BackToMenu()
    {
        Time.timeScale = 1; // Khôi phục thời gian khi quay lại menu
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
