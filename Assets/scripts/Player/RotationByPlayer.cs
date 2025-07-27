
using UnityEngine;

public class CameraSwipeControl : MonoBehaviour
{
    public float rotationSpeed = 0.2f; // Скорость вращения
    public float verticalLimit = 45f;   // Ограничение по вертикали в градусах

    private Vector2 lastTouchPosition;
    private bool isSwiping = false;

    // Текущий вертикальный угол камеры
    private float currentVerticalAngle = 0f;

    void Start()
    {
        // Инициализация текущего вертикального угла
        currentVerticalAngle = transform.eulerAngles.x;
        // Корректировка, если угол больше 180 (например, 350), чтобы получить -10
        if (currentVerticalAngle > 180f)
            currentVerticalAngle -= 360f;
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    lastTouchPosition = touch.position;
                    isSwiping = true;
                    break;

                case TouchPhase.Moved:
                    if (isSwiping)
                    {
                        Vector2 delta = touch.deltaPosition;

                        // Поворот по горизонтали
                        float horizontalRotation = delta.x * rotationSpeed;
                        transform.Rotate(0, horizontalRotation, 0, Space.World);

                        // Поворот по вертикали с ограничением
                        float verticalRotationDelta = -delta.y * rotationSpeed;
                        float newVerticalAngle = currentVerticalAngle + verticalRotationDelta;

                        // Ограничение по вертикали
                        newVerticalAngle = Mathf.Clamp(newVerticalAngle, -verticalLimit, verticalLimit);

                        // Применяем изменение только по оси X (наклон вверх/вниз)
                        float angleDifference = newVerticalAngle - currentVerticalAngle;
                        transform.Rotate(angleDifference, 0, 0, Space.Self);

                        // Обновляем текущий вертикальный угол
                        currentVerticalAngle = newVerticalAngle;
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    isSwiping = false;
                    break;
            }
        }
    }
}
