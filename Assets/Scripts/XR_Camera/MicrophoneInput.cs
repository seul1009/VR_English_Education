using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrophoneInput : MonoBehaviour
{
    // Right Controller�� Ʈ������ (XR Origin�� Right Controller ����)
    public Transform ControllerTransform; // ������ ��Ʈ�ѷ��� Ʈ������ ����
    private AudioClip record; // ������ ����� Ŭ��
    private AudioSource aud;  // ����� �ҽ� ������Ʈ
    public int recordingDuration = 3; // ���� �ð� (��)
    public SpeechToText speechToText; // SpeechToText Ŭ������ �ν��Ͻ�

    void Start()
    {
        // AudioSource ������Ʈ ��������
        aud = GetComponent<AudioSource>();
        if (aud == null)
        {
            Debug.LogError("AudioSource ������Ʈ�� �ʿ��մϴ�."); // AudioSource�� ���� ��� ���� �α� ���
        }

        // SpeechToText �ν��Ͻ��� ã��
        speechToText = FindObjectOfType<SpeechToText>();
        if (speechToText == null)
        {
            Debug.LogError("SpeechToText �ν��Ͻ��� �����ϴ�. ���� �߰��ߴ��� Ȯ���ϼ���."); // SpeechToText �ν��Ͻ��� ���� ��� ���� �α� ���
        }
    }

    // WAV ���� ���� �޼ҵ�
    public byte[] CreateWavFile(byte[] audioData)
    {
        byte[] header = CreateWavHeader(44100, 1, audioData.Length / 2); // WAV ��� ����
        byte[] wavFile = new byte[header.Length + audioData.Length]; // ����� ����� ������ ����

        System.Buffer.BlockCopy(header, 0, wavFile, 0, header.Length); // ��� ����
        System.Buffer.BlockCopy(audioData, 0, wavFile, header.Length, audioData.Length); // ����� ������ ����

        return wavFile; // ������ WAV ���� ��ȯ
    }

    // WAV ��� ���� �޼ҵ�
    private byte[] CreateWavHeader(int sampleRate, int channels, int samplesCount)
    {
        int byteRate = sampleRate * channels * 2; // ����Ʈ ����Ʈ ��� (16 bits = 2 bytes)
        int blockAlign = channels * 2; // ��� ���� ���
        int subChunk2Size = samplesCount * blockAlign; // ������ ũ�� ���
        int chunkSize = 36 + subChunk2Size; // ûũ ũ�� ���

        byte[] header = new byte[44]; // 44����Ʈ�� WAV ���

        // WAV ��� �ʵ� ����
        System.Buffer.BlockCopy(System.Text.Encoding.ASCII.GetBytes("RIFF"), 0, header, 0, 4);
        System.Buffer.BlockCopy(System.BitConverter.GetBytes(chunkSize), 0, header, 4, 4);
        System.Buffer.BlockCopy(System.Text.Encoding.ASCII.GetBytes("WAVE"), 0, header, 8, 4);
        System.Buffer.BlockCopy(System.Text.Encoding.ASCII.GetBytes("fmt "), 0, header, 12, 4);
        System.Buffer.BlockCopy(System.BitConverter.GetBytes(16), 0, header, 16, 4); // Subchunk1Size (16 for PCM)
        System.Buffer.BlockCopy(System.BitConverter.GetBytes((short)1), 0, header, 20, 2); // AudioFormat (PCM)
        System.Buffer.BlockCopy(System.BitConverter.GetBytes((short)channels), 0, header, 22, 2); // NumChannels
        System.Buffer.BlockCopy(System.BitConverter.GetBytes(sampleRate), 0, header, 24, 4); // SampleRate
        System.Buffer.BlockCopy(System.BitConverter.GetBytes(byteRate), 0, header, 28, 4); // ByteRate
        System.Buffer.BlockCopy(System.BitConverter.GetBytes((short)blockAlign), 0, header, 32, 2); // BlockAlign
        System.Buffer.BlockCopy(System.BitConverter.GetBytes((short)16), 0, header, 34, 2); // BitsPerSample
        System.Buffer.BlockCopy(System.Text.Encoding.ASCII.GetBytes("data"), 0, header, 36, 4);
        System.Buffer.BlockCopy(System.BitConverter.GetBytes(subChunk2Size), 0, header, 40, 4); // Subchunk2Size

        return header; // ������ ��� ��ȯ
    }

    // ���ͳ� ���� ���� Ȯ�� �޼ҵ�
    void CheckInternetConnection()
    {
        bool isConnected = Application.internetReachability != NetworkReachability.NotReachable;

        if (isConnected)
        {
            Debug.Log("���ͳݿ� ����Ǿ� �ֽ��ϴ�."); // ���ͳ� ���� �� �α� ���
        }
        else
        {
            Debug.LogWarning("���ͳݿ� ����Ǿ� ���� �ʽ��ϴ�."); // ���ͳ� ���� ���� ��� �α� ���
        }
    }

    void Update()
    {
        // ������ ��Ʈ�ѷ��� A ��ư�� Ŭ������ �� ������ �����մϴ�.
        if (OVRInput.GetDown(OVRInput.Button.One)) // A ��ư Ŭ��
        {
            // CheckInternetConnection(); // ���ͳ� ���� Ȯ��
            RecSnd(); // ���� ����
            //Debug.Log("���� ����");
        }

        // B ��ư Ŭ�� �� ���� ���� �� ���
        if (OVRInput.GetDown(OVRInput.Button.Two)) // B ��ư Ŭ��
        {
            StopSnd(); // ���� ����
            // ������ Ŭ�� ��� Test��
            // PlaySnd(); 
            //Debug.Log("���� ���� �� ���");
        }
    }

    // ���� ���� �޼ҵ�
    public void RecSnd()
    {
        // ����ũ ��ġ�� �ִ��� Ȯ��
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("����ũ ��ġ�� �����ϴ�."); // ����ũ�� ���� ��� ���� �α� ���
            return;
        }

        // �̹� ���� ������ Ȯ��
        if (Microphone.IsRecording(Microphone.devices[0].ToString()))
        {
            Debug.LogWarning("�̹� ���� ���Դϴ�."); // �ߺ� ���� ���
            return;
        }

        // ���� ���� (������ �ð�, 44100Hz)
        record = Microphone.Start(Microphone.devices[0].ToString(), false, recordingDuration, 44100);
        aud.clip = record; // AudioSource�� Ŭ�� �Ҵ�
        //Debug.Log("���� ���۵�");
    }

    // ���� ���� �޼ҵ�
    public void StopSnd()
    {
        // ���� ������ Ȯ��
        if (!Microphone.IsRecording(Microphone.devices[0].ToString()))
        {
            Debug.Log("StopSnd�� ������ ���� ���ϰ� �ֽ��ϴ�."); // ���� ���� �ƴ� �� ��� �α� ���
            return; // ���� ���� �ƴ� ���� ����
        }

        Microphone.End(Microphone.devices[0].ToString()); // ���� ����

        if (record == null)
        {
            Debug.LogError("������ AudioClip�� �����ϴ�."); // ������ Ŭ���� ���� ��� ���� �α� ���
            return;
        }

        //Debug.Log("���� ���� "); // ���� ���� �α� ���
        // ������ AudioClip�� Ȯ��
        // Debug.Log("������ AudioClip: " + record.name);

        // SpeechToText �ν��Ͻ��� null���� Ȯ��
        if (speechToText != null)
        {
            // ������ AudioClip�� byte[]�� ��ȯ�ϰ� Google�� ����
            byte[] audioData = AudioClipToWav.ClipToWav(record); // WAV �������� ��ȯ
            byte[] wavFileData = CreateWavFile(audioData); // WAV ���� ����
            StartCoroutine(speechToText.SendAudioToGoogle(wavFileData)); // SendAudioToGoogle ȣ��
        }
        else
        {
            Debug.LogError("SpeechToText �ν��Ͻ��� null�Դϴ�. �Ҵ��� Ȯ���ϼ���."); // SpeechToText �ν��Ͻ��� ���� ��� ���� �α� ���
        }

        //Debug.Log("���� ����");
    }

    // ������ Ŭ�� ��� �޼ҵ�
    public void PlaySnd()
    {
        if (aud.clip != null)
        {
            aud.Play(); // ������ Ŭ�� ���
            Debug.Log("��� ��: " + aud.clip.name); // ��� ���� Ŭ�� �̸� �α� ���
        }
        else
        {
            Debug.LogError("������ Ŭ���� �����ϴ�."); // ������ Ŭ���� ���� ��� ���� �α� ���
        }
    }

    // ������ ����� Ŭ�� ��ȯ �޼ҵ�
    public AudioClip GetAudioClip()
    {
        return record; // ������ ����� Ŭ�� ��ȯ
    }
}
