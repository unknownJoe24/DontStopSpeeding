using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform car; // assign in the inspector
    public float zoomSpeed = 5f;
    public float minZoom = 20f;
    public float maxZoom = 80f;
    public float cameraHeight = 20f;

    private Camera cam;
    private CarController carController;

    void Start()
    {
        cam = GetComponent<Camera>();
        carController = car.GetComponent<CarController>();
        transform.position = new Vector3(car.position.x, car.position.y + cameraHeight, car.position.z + 10);
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }

    void LateUpdate()
    {
        transform.position = new Vector3(car.position.x, car.position.y + cameraHeight, car.position.z);
        transform.rotation = Quaternion.Euler(90f, car.eulerAngles.y, 0f);

        float carSpeed = carController.currentSpeed;
        float targetZoom = Mathf.Lerp(minZoom, maxZoom, carSpeed / 30f);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetZoom, Time.deltaTime * zoomSpeed);
    }
}