using System.Collections;
using UnityEngine;
using Cinemachine;

public class NoiseEvent : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtual;
    [SerializeField] private float noiseGain = 1f;  // Máxima intensidad del ruido
    [SerializeField] private float noiseDuration = 1f;  // Duración para reducir el ruido

    public void TriggerNoise(float noiseAmount)
    {
        StartCoroutine(NoiseCoroutine(noiseAmount));
    }

    private IEnumerator NoiseCoroutine(float noiseAmount)
    {
        // Obtén el componente Perlin
        var noise = cinemachineVirtual.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        if (noise == null)
        {
            Debug.LogWarning("CinemachineBasicMultiChannelPerlin no encontrado en la cámara.");
            yield break;
        }

        // Establece el ruido máximo
        noise.m_AmplitudeGain = noiseAmount;

        float elapsedTime = 0f;

        // Reduce progresivamente el ruido hasta 0
        while (elapsedTime < noiseDuration)
        {
            noise.m_AmplitudeGain = Mathf.Lerp(noiseAmount, 0f, elapsedTime / noiseDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Asegúrate que el ruido sea exactamente 0 al final
        noise.m_AmplitudeGain = 0f;
    }
}
