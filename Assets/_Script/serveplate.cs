using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BurgerUIManager;

public class serveplate : MonoBehaviour
{
    public BurgerUIManager burgerUiManager;
    public int thisPlate; // Xác định số đĩa hiện tại

    //private float currentScore = 0f; // Điểm hiện tại

    public AudioClip correctSound; // Âm thanh cho nhấn đúng
    public AudioClip incorrectSound; // Âm thanh cho nhấn sai
    private AudioSource audioSource; // Đối tượng AudioSource

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>(); // Thêm AudioSource
        UpdateScoreUI();
    }

    public void OnServeButtonClicked()
    {
        Serve(gameflow.plateNum); // Sử dụng gameflow.plateNum
    }
    public void Serve(int plateNum)
    {

        // Kiểm tra xem đĩa được nhấn có phải là đĩa hiện tại hay không
        //if (!IsCurrentPlateSelected())
        //{
        //    Debug.Log("Không thể nhấn vào đĩa này vì không phải đĩa hiện tại.");
        //    return; // Nếu đĩa không phải đĩa hiện tại, thoát ra
        //}

        if (Time.timeScale == 0)
        {
            return; // Nếu game đang tạm dừng, không thực hiện bất kỳ hành động nào
        }

        // Xóa tất cả các đối tượng clone trên đĩa hiện tại
        RemoveAllFoodOnPlate(plateNum);

        // Tính toán tổng giá trị thức ăn trên đĩa hiện tại
        int totalPlateValue = gameflow.plateValue[plateNum];

        // Lấy các thành phần đã chọn từ burgerUiManager dựa trên vị trí đĩa
        List<BurgerComponent> selectedComponents = GetSelectedComponents(plateNum);

        // So sánh giá trị của burger với giá trị mong muốn (foodValue)
        if (CompareClickOrder(selectedComponents))
        {
            Debug.Log("Correct order!");
            CalculateScore(true, totalPlateValue);
            audioSource.PlayOneShot(correctSound);
            // Phát âm thanh đúng
        }
        else
        {
            Debug.Log("Incorrect order!");
            CalculateScore(false, totalPlateValue);
            audioSource.PlayOneShot(incorrectSound);
            // Phát âm thanh sai
        }

        // Làm mới giao diện và đặt lại giá trị
        gameflow.emptyPlateNow = transform.position.x;

        // Làm mới công thức tương ứng
        RefreshRecipe(plateNum);

        // Đặt lại giá trị thức ăn trên đĩa
        gameflow.plateValue[plateNum] = 0;

        // Reset lại số lần click cho đĩa hiện tại
        ResetClickCountsForCurrentPlate();

        StartCoroutine(platereset());
    }

        private bool IsCurrentPlateSelected()
    {
        // Kiểm tra xem đĩa hiện tại có được chọn hay không bằng cách so sánh vị trí
        return Mathf.Abs(transform.position.x - gameflow.plateXpos) < 0.1f;
    }

    private void RemoveAllFoodOnPlate(int plateNum)
    {
        // Tìm tất cả các đối tượng clone (food) trong scene
        foreach (GameObject food in GameObject.FindGameObjectsWithTag("Food"))
        {
            // Kiểm tra xem đối tượng này có nằm trên đĩa hiện tại hay không
            if (IsFoodOnPlate(food.transform.position.x, plateNum))
            {
                Destroy(food); // Xóa đối tượng này
            }
        }
    }

    private bool IsFoodOnPlate(float foodPositionX, int plateNum)
    {
        float plateXpos = plateNum * 2.5f; // Tính toán vị trí đĩa
        // Kiểm tra xem vị trí của thực phẩm có nằm trên đĩa không
        return Mathf.Abs(foodPositionX - plateXpos) < 0.2f; // Thay đổi khoảng cách nếu cần
    }

    private List<BurgerComponent> GetSelectedComponents(int plateNum)
    {
        if (plateNum == 0)
            return burgerUiManager.GetSelectedComponents1(); // Lấy các thành phần từ công thức 1
        else if (plateNum == 1)
            return burgerUiManager.GetSelectedComponents2(); // Lấy các thành phần từ công thức 2
        else if (plateNum == 2)
            return burgerUiManager.GetSelectedComponents3(); // Lấy các thành phần từ công thức 3

        return null; // Nếu không tìm thấy
    }

    private void RefreshRecipe(int plateNum)
    {
        if (plateNum == 0)
        {
            burgerUiManager.RefreshRecipe1();
        }
        else if (plateNum == 1)
        {
            burgerUiManager.RefreshRecipe2();
        }
        else if (plateNum == 2)
        {
            burgerUiManager.RefreshRecipe3();
        }
    }

    // Hàm reset số lần click cho một đĩa cụ thể
    private void ResetClickCountsForCurrentPlate()
    {
        clickplace[] allClickPlaces = FindObjectsOfType<clickplace>();

        foreach (clickplace cp in allClickPlaces)
        {
            cp.ResetClickCountsForPlate(thisPlate); // Reset cho đĩa hiện tại
        }
    }

    IEnumerator platereset()
    {
        yield return new WaitForSeconds(.2f);
        gameflow.emptyPlateNow = -1;
    }

    // Hàm so sánh thứ tự nhấp chuột với thứ tự của burger
    private bool CompareClickOrder(List<BurgerComponent> selectedComponents)
    {
        if (selectedComponents.Count == 0)
        {
            gameflow.globalClickOrder.Clear();
            return false;
        }
        if (selectedComponents.Count > gameflow.globalClickOrder.Count)
        {
            gameflow.globalClickOrder.Clear();
            return false;
        }

        gameflow.globalClickOrder.Reverse();

        for (int i = 0; i < selectedComponents.Count; i++)
        {
            if (gameflow.globalClickOrder[i] != selectedComponents[i].name) // So sánh tên của các thành phần
                return false;
        }
        gameflow.globalClickOrder.Clear();
        return true;
    }

    // Tính điểm dựa trên thời gian còn lại và giá trị của thức ăn trên đĩa
    private void CalculateScore(bool isCorrectOrder, int totalPlateValue)
    {
        float timeRemaining = 0f;

        if (transform.position.x == 0)
        {
            timeRemaining = burgerUiManager.timeRemaining1;
        }
        else if (transform.position.x == 2.5f)
        {
            timeRemaining = burgerUiManager.timeRemaining2;
        }
        else if (transform.position.x == 5)
        {
            timeRemaining = burgerUiManager.timeRemaining3;
        }

        float scoreToAdd = 0f; // Biến tạm thời để lưu điểm cần cộng thêm hoặc trừ

        if (isCorrectOrder)
        {
            float baseScore = totalPlateValue;
            float timeBonus = timeRemaining * 5;
            scoreToAdd = baseScore + timeBonus; // Cộng điểm dựa trên giá trị và thời gian
        }
        else
        {
            float penalty = (totalPlateValue) + (timeRemaining * 2);
            scoreToAdd = -penalty; // Trừ điểm nếu sai
        }
        gameflow.remainScore -= scoreToAdd;
        gameflow.totalScore += scoreToAdd; // Cộng điểm vào điểm tổng (có thể dương hoặc âm)
        UpdateScoreUI(); // Cập nhật UI với điểm mới
        UpdateRemainScoreUI();
        CheckScoreLimits(); // Kiểm tra giới hạn điểm
    }

    private void CheckScoreLimits()
    {
        if (gameflow.totalScore < 0)
        {
            ShowGameOverUI(); // Gọi hàm hiển thị UI Game Over
        }
        else if (gameflow.totalScore >= MenuController.selectedPointLimit) // Sử dụng mức điểm từ MenuController
        {
            ShowWinUI(); // Gọi hàm hiển thị UI Win
        }
    }


    private void ShowGameOverUI()
    {
        burgerUiManager.gameOverUI.SetActive(true);
        Time.timeScale = 0; // Dừng thời gian trong game
        Debug.Log("Game Over!"); // Debug để kiểm tra
    }

    private void ShowWinUI()
    {
        burgerUiManager.winUI.SetActive(true);
        Time.timeScale = 0; // Dừng thời gian trong game
        Debug.Log("You Win!"); // Debug để kiểm tra
    }

    // Cập nhật điểm trên UI
    private void UpdateScoreUI()
    {
        if (burgerUiManager != null && burgerUiManager.gameflowInstance != null)
        {
            burgerUiManager.gameflowInstance.UpdateScoreUI(); // Gọi từ instance của gameflow
        }
    }

    private void UpdateRemainScoreUI()
    {
        if (burgerUiManager != null && burgerUiManager.gameflowInstance != null)
        {
            burgerUiManager.gameflowInstance.UpdateRemainScoreUI(); // Gọi từ instance của gameflow
        }
    }
}
