using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class clickplace : MonoBehaviour
{
    public Transform cloneObj; // Đối tượng sẽ được tạo
    public int foodValue; // Giá trị của thực phẩm
    public AudioSource audioClick; // Âm thanh khi nhấp chuột
    private const int maxClicks = 5; // Giới hạn số lần nhấp chuột

    // Tạo một Dictionary để lưu số lần click cho từng đĩa cho từng thành phần
    private Dictionary<int, Dictionary<string, int>> plateClickCounts = new Dictionary<int, Dictionary<string, int>>();
    //public List<string> clickOrder = new List<string>(); // Danh sách để lưu thứ tự nhấn

    void Start()
    {
        audioClick.Stop();
        InitializePlateClickCounts();
        for (int i = 0; i < 3; i++)
        {
            ResetClickCountsForPlate(i);
        }
    }

    // Hàm khởi tạo số lần click cho mỗi đĩa
    private void InitializePlateClickCounts()
    {
        for (int i = 0; i < 3; i++) // Giả sử có 3 đĩa
        {
            plateClickCounts[i] = new Dictionary<string, int>
            {
                { "bunBottom", 0 },
                { "bunTop", 0 },
                { "Tomato", 0 },
                { "Salad", 0 }
            };
        }
    }

    private void OnMouseDown()
    {
        if (Time.timeScale == 0)
        {
            return; // Nếu game đang tạm dừng, không thực hiện bất kỳ hành động nào
        }
        int currentPlate = gameflow.plateNum;

        // Kiểm tra số lần nhấp chuột cho thành phần của đĩa hiện tại
        if (plateClickCounts[currentPlate][gameObject.name] >= maxClicks)
        {
            Debug.Log($"{gameObject.name} trên đĩa {currentPlate} đã đạt giới hạn số lần nhấp chuột.");
            return;
        }

        // Thêm tên thành phần vào danh sách nhấn toàn cầu
        gameflow.globalClickOrder.Add(gameObject.name); // Sử dụng danh sách chung

        // Thực hiện hành động khi nhấp vào thành phần
        switch (gameObject.name)
        {
            case "bunBottom":
            case "bunTop":
            case "Tomato":
            case "Salad":
                Instantiate(cloneObj, new Vector3(gameflow.plateXpos, 1f, 0), cloneObj.rotation);
                break;
        }

        // Tăng số lần nhấp chuột cho thành phần của đĩa hiện tại
        plateClickCounts[currentPlate][gameObject.name]++;
        audioClick.Play();

        // Cập nhật giá trị thức ăn trên đĩa
        gameflow.plateValue[currentPlate] += foodValue;
        Debug.Log($"{gameflow.plateValue[currentPlate]} {gameflow.orderValue[currentPlate]}");
        //foreach (var name in gameflow.globalClickOrder)
        //{
        //    Debug.Log($"Tên: {name}");
        //}
    }




    // Hàm reset số lần click cho một đĩa cụ thể
    public void ResetClickCountsForPlate(int plateNum)
    {
        plateClickCounts[plateNum]["bunBottom"] = 0;
        plateClickCounts[plateNum]["bunTop"] = 0;
        plateClickCounts[plateNum]["Tomato"] = 0;
        plateClickCounts[plateNum]["Salad"] = 0;
    }
}