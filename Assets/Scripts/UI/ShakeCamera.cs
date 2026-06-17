using UnityEngine;

public class ShakeCamera : MonoBehaviour
{
    public Transform camTransform;

    static public float shakeDuration = 0f;

    public float shakeAmount = 0.7f;
    public float decreaseFactor = 1.0f;

    private Vector3 originalPos;

    private void Awake()
    {
        if (camTransform == null)
            camTransform = transform;
    }

    private void Start()
    {
        originalPos = camTransform.localPosition;
    }

    private void Update()
    {
        // Si el juego está pausado por overlay, nunca dejes la cámara en offset.
        if (Game.isPaused || Time.timeScale == 0f)
        {
            StopShake();
            return;
        }

        if (shakeDuration > 0f)
        {
            Vector3 offset = Random.insideUnitSphere * shakeAmount;

            camTransform.localPosition = new Vector3(
                originalPos.x + offset.x,
                originalPos.y + offset.y,
                originalPos.z
            );

            // Usamos unscaledDeltaTime para que el shake pueda terminar aunque haya cambios de timeScale.
            shakeDuration -= Time.unscaledDeltaTime * decreaseFactor;
        }
        else
        {
            StopShake();
        }
    }

    public static void ActivateShake()
    {
        // No activar shake si el juego ya está pausado o cerrando ronda.
        if (Game.isPaused || Time.timeScale == 0f)
            return;

        shakeDuration = 0.3f;
    }

    public static void StopShake()
    {
        shakeDuration = 0f;

        ShakeCamera shaker = FindFirstObjectByType<ShakeCamera>();

        if (shaker != null && shaker.camTransform != null)
            shaker.camTransform.localPosition = shaker.originalPos;
    }
}