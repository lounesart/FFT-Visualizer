using UnityEngine;
using UnityEngine.Audio;

public class SpectrumManager : MonoBehaviour
{
    public static SpectrumManager Instance { get; private set; }

    [SerializeField] private AudioMixerGroup audioMixerGroup = null;
    [SerializeField] private int spectrumSize = 2048;
    [SerializeField] private AudioClip audioClip = null;

    public float[] Samples { get; private set; }

    private AudioSource audioSource;
    private float[] spectrumData;

    private void Awake()
    {
        Instance = this;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = audioMixerGroup;
        audioSource.clip = audioClip;
        audioSource.loop = true;
        audioSource.Play();

        Samples = new float[spectrumSize];
        spectrumData = new float[spectrumSize];
    }

    private void Update()
    {
        audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);

        for (int i = 0; i < spectrumSize; i++)
        {
            Samples[i] = spectrumData[i];
        }
    }
}
