using UnityEngine;

public class FryingPanController : MonoBehaviour
{
    public GameObject fireEffectPrefab;   // Prefab cho hiệu ứng lửa
    public AudioSource fireSound;         // Âm thanh lửa cháy (gắn từ Inspector)

    private GameObject currentFireEffect; // Đối tượng hiệu ứng lửa hiện tại
    private int foodCount = 0;            // Đếm số lượng miếng thịt đang trên chảo

    private void Start()
    {
        // Khởi tạo không có miếng thịt nào trên chảo
        foodCount = 0;
    }

    void OnTriggerEnter(Collider other)
    {
        // Khi đối tượng với tag "Food" chạm vào chảo
        if (other.CompareTag("Food"))
        {
            // Tăng số lượng miếng thịt trên chảo
            foodCount++;

            // Nếu hiệu ứng lửa chưa được tạo, tạo hiệu ứng lửa và phát âm thanh
            if (currentFireEffect == null)
            {
                // Tạo hiệu ứng lửa
                currentFireEffect = Instantiate(fireEffectPrefab, transform.position, Quaternion.identity);
                currentFireEffect.transform.parent = transform;

                // Phát âm thanh nếu có AudioSource
                if (fireSound != null)
                {
                    fireSound.Play();
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Khi đối tượng "Food" rời khỏi chảo
        if (other.CompareTag("Food"))
        {
            // Giảm số lượng miếng thịt trên chảo
            foodCount--;

            // Nếu không còn miếng thịt nào trên chảo
            if (foodCount <= 0 && currentFireEffect != null)
            {
                // Hủy hiệu ứng lửa
                Destroy(currentFireEffect);
                currentFireEffect = null;

                // Dừng âm thanh nếu có AudioSource
                if (fireSound != null)
                {
                    fireSound.Stop();
                }
            }
        }
    }

    // Phương thức để tắt lửa từ cookmove khi thịt biến mất
    public void TurnOffFire()
    {
        if (currentFireEffect != null)
        {
            Destroy(currentFireEffect);
            currentFireEffect = null;
        }

        // Dừng âm thanh nếu có AudioSource
        if (fireSound != null)
        {
            fireSound.Stop();
        }
    }
}
