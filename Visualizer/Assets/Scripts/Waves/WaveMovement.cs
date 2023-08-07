using UnityEngine;
using System.Collections;

public class WaveMovement : MonoBehaviour
{
    public float amplitude = 1f;
    public float frequency = 1f;
    public float speed = 1f;
    public float offset = 0f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float y = amplitude * Mathf.Sin(Time.time * frequency + offset);
        Vector3 newPos = startPosition + new Vector3(speed * Time.time, y, 0f);
        transform.position = newPos;
    }

    public void SetWave(float newAmplitude, float newFrequency, float newSpeed, float newOffset)
    {
        amplitude = newAmplitude;
        frequency = newFrequency;
        speed = newSpeed;
        offset = newOffset;
    }
}
