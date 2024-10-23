using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject mainMenu;
    public Dropdown difficultyDropdown;

    // Biến static để lưu độ khó được chọn
    public static int selectedDifficulty = 0;


    public InputField pointInputField; // Hộp văn bản để nhập điểm
    public Text targetPointText; // Text để hiển thị điểm mục tiêu trên UI
    public static float selectedPointLimit = 200000; // Mức điểm giới hạn mặc định

    void Start()
    {
        ShowMainMenu();
        InitializeDropdown();
        InitializePointInputField(); // Khởi tạo hộp nhập điểm
    }

    public void ShowMainMenu()
    {
        mainMenu.SetActive(true);
        Time.timeScale = 0; // Tạm dừng thời gian trong trò chơi
    }

    public void StartGame()
    {
        Time.timeScale = 1; // Tiếp tục thời gian trong trò chơi
        mainMenu.SetActive(false);
        //SceneManager.LoadScene("food"); // Tải scene trò chơi (food)
        SceneManager.LoadScene("food", LoadSceneMode.Single);

    }

    private void InitializeDropdown()
    {
        if (difficultyDropdown != null)
        {
            difficultyDropdown.ClearOptions();
            List<string> options = new List<string> { "Easy", "Hard" };
            difficultyDropdown.AddOptions(options);

            // Đăng ký sự kiện thay đổi độ khó
            difficultyDropdown.onValueChanged.AddListener(OnDifficultyChanged);
        }
        else
        {
            Debug.LogError("Difficulty Dropdown chưa được gán trong Inspector!");
        }
    }

    public void OnDifficultyChanged(int index)
    {
        selectedDifficulty = index; // Lưu độ khó được chọn
    }


    private void InitializePointInputField()
    {
        if (pointInputField != null)
        {
            pointInputField.onEndEdit.AddListener(OnPointLimitChanged); // Đăng ký sự kiện khi người dùng nhập xong
            pointInputField.onValueChanged.AddListener(OnPointInputChanged); // Đăng ký sự kiện khi giá trị thay đổi
        }
        else
        {
            Debug.LogError("Point InputField chưa được gán trong Inspector!");
        }

        // Cập nhật giá trị mặc định của điểm mục tiêu lên UI
        //UpdateTargetPointText();
    }

    // Hàm kiểm tra giá trị nhập
    private void OnPointInputChanged(string value)
    {
        // Nếu không phải số, xóa ký tự không hợp lệ
        foreach (char c in value)
        {
            if (!char.IsDigit(c) && c != '.')
            {
                pointInputField.text = value.Replace(c.ToString(), ""); // Loại bỏ ký tự không phải số
                break;
            }
        }
    }

    public void OnPointLimitChanged(string value)
    {
        // Chuyển đổi giá trị nhập từ văn bản thành số
        if (float.TryParse(value, out float pointLimit))
        {
            selectedPointLimit = pointLimit; // Lưu giá trị điểm giới hạn
            //UpdateTargetPointText(); // Cập nhật lại UI
        }
        else
        {
            Debug.LogError("Giá trị nhập không hợp lệ!");
        }
    }

    private void UpdateTargetPointText()
    {
        if (targetPointText != null)
        {
            targetPointText.text = "Mức điểm mục tiêu: " + selectedPointLimit.ToString(); // Hiển thị điểm mục tiêu
        }
    }
}



