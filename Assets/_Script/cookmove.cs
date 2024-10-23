using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cookmove : MonoBehaviour
{
    private int foodValue = 0; // Giá trị của thịt
    private MeshRenderer meatMat;
    private string stillcooking = "y"; // Biến trạng thái thịt đang chín
    public AudioSource audioClick;

    // Biến meatcon để truy cập phương thức ResetClickCount
    public meatcon meatController;

    // Thời gian tối đa để biến mất sau khi chín
    private float disappearTime = 5f;
    private Coroutine disappearCoroutine; // Biến để lưu coroutine biến mất

    private bool isOnPlate = false; // Biến để kiểm tra xem thịt đã được di chuyển vào đĩa hay chưa

    // Thêm tham chiếu tới FryingPanController
    public FryingPanController fryingPanController;

    void Start()
    {
        fryingPanController = FindObjectOfType<FryingPanController>();
        audioClick.Stop();
        meatMat = GetComponent<MeshRenderer>();
        StartCoroutine(cookTimer());
    }

    private void OnMouseDown()
    {
        if (Time.timeScale == 0)
        {
            return; // Nếu game đang tạm dừng, không thực hiện bất kỳ hành động nào
        }
        // Kiểm tra xem thịt đã được đặt lên đĩa chưa
        if (isOnPlate)
        {
            Debug.Log("Thịt đã được di chuyển vào đĩa, không thể nhấn vào nữa.");
            return; // Nếu đã đặt lên đĩa thì không làm gì
        }

        // Kiểm tra xem thịt đã chín chưa
        if (stillcooking == "n")
        {
            Debug.Log("Di chuyển thịt vào đĩa. Giá trị X: " + gameflow.plateXpos);

            // Di chuyển thịt vào đĩa
            transform.position = new Vector3(gameflow.plateXpos, 1f, 0);

            gameflow.globalClickOrder.Add("Meat");
            // Cập nhật giá trị thức ăn của đĩa
            gameflow.plateValue[gameflow.plateNum] += foodValue;
            audioClick.Play();

            // Gọi phương thức ResetClickCount để đặt lại số lần nhấp
            meatController?.ResetClickCount();

            // Đặt cờ isOnPlate để đánh dấu rằng thịt đã được di chuyển lên đĩa
            isOnPlate = true;

            // Hủy coroutine biến mất nếu thịt đã chín
            if (disappearCoroutine != null)
            {
                StopCoroutine(disappearCoroutine);
                disappearCoroutine = null;
            }

            // Không gọi TurnOffFire ở đây
            Debug.Log("Thịt đã di chuyển lên đĩa."); // Thêm log
        }
        else
        {
            Debug.Log("Thịt vẫn đang chín.");
        }
    }

    IEnumerator cookTimer()
    {
        yield return new WaitForSeconds(3); // Chờ 3 giây để thịt chín
        foodValue = 1000; // Giá trị của thịt

        if (stillcooking == "y")
        {
            meatMat.material.color = new Color(.6f, .2f, .2f);
            stillcooking = "n"; // Đặt biến thành "n" để cho biết thịt đã chín
            disappearCoroutine = StartCoroutine(WaitAndDisappear());
        }
    }

    IEnumerator WaitAndDisappear()
    {
        yield return new WaitForSeconds(disappearTime);

        // Gọi phương thức ResetClickCount khi thịt biến mất
        meatController?.ResetClickCount();

        // Không gọi TurnOffFire ở đây
        Debug.Log("Thịt đã biến mất."); // Thêm log

        // Gọi TurnOffFire chỉ khi không còn miếng thịt nào trên chảo
        fryingPanController?.TurnOffFire();

        Destroy(gameObject); // Biến mất sau thời gian đã định
    }
}
