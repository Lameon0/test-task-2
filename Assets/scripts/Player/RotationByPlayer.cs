
using UnityEngine;

public class CameraSwipeControl : MonoBehaviour
{
    public float rotationSpeed = 0.2f; // �������� ��������
    public float verticalLimit = 45f;   // ����������� �� ��������� � ��������

    private Vector2 lastTouchPosition;
    private bool isSwiping = false;

    // ������� ������������ ���� ������
    private float currentVerticalAngle = 0f;

    void Start()
    {
        // ������������� �������� ������������� ����
        currentVerticalAngle = transform.eulerAngles.x;
        // �������������, ���� ���� ������ 180 (��������, 350), ����� �������� -10
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

                        // ������� �� �����������
                        float horizontalRotation = delta.x * rotationSpeed;
                        transform.Rotate(0, horizontalRotation, 0, Space.World);

                        // ������� �� ��������� � ������������
                        float verticalRotationDelta = -delta.y * rotationSpeed;
                        float newVerticalAngle = currentVerticalAngle + verticalRotationDelta;

                        // ����������� �� ���������
                        newVerticalAngle = Mathf.Clamp(newVerticalAngle, -verticalLimit, verticalLimit);

                        // ��������� ��������� ������ �� ��� X (������ �����/����)
                        float angleDifference = newVerticalAngle - currentVerticalAngle;
                        transform.Rotate(angleDifference, 0, 0, Space.Self);

                        // ��������� ������� ������������ ����
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
