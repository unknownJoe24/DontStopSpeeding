using UnityEngine;

public class SkyboxRotator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 1.0f;

    private void Update()
    {
        RotateSkybox();
    }

    private void RotateSkybox()
    {
        float rotation = Time.deltaTime * rotationSpeed;
        RenderSettings.skybox.SetFloat("_Rotation", Mathf.Repeat(RenderSettings.skybox.GetFloat("_Rotation") + rotation, 360f));
    }
}