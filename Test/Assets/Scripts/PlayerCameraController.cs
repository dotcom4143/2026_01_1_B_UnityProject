using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    public static float LocalCameraYaw { get; private set; }

    [Header("Camera")]
    [SerializeField] private GameObject cameraRoot;
    [SerializeField] private float sensitivity = 3f;
    [SerializeField] private float minPitch = -30f;
    [SerializeField] private float maxPitch = 60f;

    private Transform cameraTransform;
    private bool isLocalPlayer;

    private float yaw;
    private float pitch = 15f;

    public void Setup(bool hasInputAuthority)
    {
        isLocalPlayer = hasInputAuthority;

        if (cameraRoot == null)
            return;

        cameraRoot.SetActive(isLocalPlayer);

        if (!isLocalPlayer)
            return;

        Camera camera = cameraRoot.GetComponentInChildren<Camera>(true);

        if (camera != null)
        {
            cameraTransform = camera.transform;
        }

        yaw = transform.eulerAngles.y;
        pitch = 15f;
        LocalCameraYaw = yaw;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        if (!isLocalPlayer)
            return;

        if (cameraRoot == null || cameraTransform == null)
            return;

        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        yaw += mouseX;
        pitch -= mouseY;

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        cameraRoot.transform.localRotation = Quaternion.Euler(0f, yaw, 0f);
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        LocalCameraYaw = yaw;
    }
}