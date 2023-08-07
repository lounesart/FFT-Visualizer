using UnityEngine;

public class WaveController : MonoBehaviour
{
    public WaveMovement waveMovement;
    public float amplitudeIncrement = 0.1f;
    public float frequencyIncrement = 0.1f;
    public float speedIncrement = 0.1f;
    public float offsetIncrement = 0.1f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            waveMovement.SetWave(waveMovement.amplitude + amplitudeIncrement, waveMovement.frequency, waveMovement.speed, waveMovement.offset);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            waveMovement.SetWave(waveMovement.amplitude, waveMovement.frequency + frequencyIncrement, waveMovement.speed, waveMovement.offset);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            waveMovement.SetWave(waveMovement.amplitude, waveMovement.frequency, waveMovement.speed + speedIncrement, waveMovement.offset);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            waveMovement.SetWave(waveMovement.amplitude, waveMovement.frequency, waveMovement.speed, waveMovement.offset + offsetIncrement);
        }
    }
}
