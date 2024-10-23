using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meatcon : MonoBehaviour
{
    public Transform cloneObj; // Đối tượng thịt sẽ được tạo
    public AudioSource audioClick; // Âm thanh khi nhấp chuột
    public clickplace clickplace;

    private int clickCount = 0; // Số lần nhấp chuột
    private const int maxClicks = 4; // Giới hạn số lần nhấp chuột

    // Danh sách các vị trí mà đối tượng "Meat" có thể xuất hiện
    private Vector3[] spawnPositions = new Vector3[] {
        new Vector3(-3.24f, 0.33f, 0.43f),
        new Vector3(-2.24f, 0.33f, 0.43f),
        new Vector3(-3.24f, 0.33f, -0.53f),
        new Vector3(-2.24f, 0.33f, -0.53f)
    };

    void Start()
    {
        clickCount = 0; // Đặt lại số lần nhấp chuột
        audioClick.Stop();
    }

    private void OnMouseDown()
    {
        if (Time.timeScale == 0)
        {
            return; // Nếu game đang tạm dừng, không thực hiện bất kỳ hành động nào
        }
        // Kiểm tra xem số lần nhấp chuột đã đạt đến giới hạn chưa
        if (clickCount >= maxClicks)
        {
            Debug.Log("Đã đạt giới hạn số lần nhấp chuột cho Meat.");
            return; // Không thực hiện hành động nếu đã đạt giới hạn
        }

        if (gameObject.name == "Meat")
        {
            // Tạo đối tượng clone tại vị trí tương ứng với số lần nhấp chuột
            Transform clone = Instantiate(cloneObj, spawnPositions[clickCount], cloneObj.rotation);

            // Truyền meatcon (bản thân) vào đối tượng cookmove của clone để đặt lại clickCount
            cookmove cookMoveScript = clone.GetComponent<cookmove>();
            if (cookMoveScript != null)
            {
                cookMoveScript.meatController = this; // Gán meatcon vào cookmove
            }

            audioClick.Play(); // Phát âm thanh nhấp chuột
            clickCount++; // Tăng số lần nhấp chuột
        }
    }

    // Phương thức để đặt lại số lần nhấp chuột
    public void ResetClickCount()
    {
        clickCount = 0; // Đặt lại số lần nhấp chuột
    }
}
