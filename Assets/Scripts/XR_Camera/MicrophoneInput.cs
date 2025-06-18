using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrophoneInput : MonoBehaviour
{
    // Right Controller의 트랜스폼 (XR Origin의 Right Controller 참조)
    public Transform ControllerTransform; // 오른쪽 컨트롤러의 트랜스폼 참조
    private AudioClip record; // 녹음된 오디오 클립
    private AudioSource aud;  // 오디오 소스 컴포넌트
    public int recordingDuration = 3; // 녹음 시간 (초)
    public SpeechToText speechToText; // SpeechToText 클래스의 인스턴스

    void Start()
    {
        // AudioSource 컴포넌트 가져오기
        aud = GetComponent<AudioSource>();
        if (aud == null)
        {
            Debug.LogError("AudioSource 컴포넌트가 필요합니다."); // AudioSource가 없을 경우 오류 로그 출력
        }

        // SpeechToText 인스턴스를 찾기
        speechToText = FindObjectOfType<SpeechToText>();
        if (speechToText == null)
        {
            Debug.LogError("SpeechToText 인스턴스가 없습니다. 씬에 추가했는지 확인하세요."); // SpeechToText 인스턴스가 없을 경우 오류 로그 출력
        }
    }

    // WAV 파일 생성 메소드
    public byte[] CreateWavFile(byte[] audioData)
    {
        byte[] header = CreateWavHeader(44100, 1, audioData.Length / 2); // WAV 헤더 생성
        byte[] wavFile = new byte[header.Length + audioData.Length]; // 헤더와 오디오 데이터 결합

        System.Buffer.BlockCopy(header, 0, wavFile, 0, header.Length); // 헤더 복사
        System.Buffer.BlockCopy(audioData, 0, wavFile, header.Length, audioData.Length); // 오디오 데이터 복사

        return wavFile; // 생성된 WAV 파일 반환
    }

    // WAV 헤더 생성 메소드
    private byte[] CreateWavHeader(int sampleRate, int channels, int samplesCount)
    {
        int byteRate = sampleRate * channels * 2; // 바이트 레이트 계산 (16 bits = 2 bytes)
        int blockAlign = channels * 2; // 블록 정렬 계산
        int subChunk2Size = samplesCount * blockAlign; // 데이터 크기 계산
        int chunkSize = 36 + subChunk2Size; // 청크 크기 계산

        byte[] header = new byte[44]; // 44바이트의 WAV 헤더

        // WAV 헤더 필드 설정
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

        return header; // 생성된 헤더 반환
    }

    // 인터넷 연결 상태 확인 메소드
    void CheckInternetConnection()
    {
        bool isConnected = Application.internetReachability != NetworkReachability.NotReachable;

        if (isConnected)
        {
            Debug.Log("인터넷에 연결되어 있습니다."); // 인터넷 연결 시 로그 출력
        }
        else
        {
            Debug.LogWarning("인터넷에 연결되어 있지 않습니다."); // 인터넷 연결 없음 경고 로그 출력
        }
    }

    void Update()
    {
        // 오른쪽 컨트롤러의 A 버튼을 클릭했을 때 녹음을 시작합니다.
        if (OVRInput.GetDown(OVRInput.Button.One)) // A 버튼 클릭
        {
            // CheckInternetConnection(); // 인터넷 연결 확인
            RecSnd(); // 녹음 시작
            //Debug.Log("녹음 시작");
        }

        // B 버튼 클릭 시 녹음 종료 및 재생
        if (OVRInput.GetDown(OVRInput.Button.Two)) // B 버튼 클릭
        {
            StopSnd(); // 녹음 종료
            // 녹음된 클립 재생 Test용
            // PlaySnd(); 
            //Debug.Log("녹음 종료 및 재생");
        }
    }

    // 녹음 시작 메소드
    public void RecSnd()
    {
        // 마이크 장치가 있는지 확인
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("마이크 장치가 없습니다."); // 마이크가 없을 경우 오류 로그 출력
            return;
        }

        // 이미 녹음 중인지 확인
        if (Microphone.IsRecording(Microphone.devices[0].ToString()))
        {
            Debug.LogWarning("이미 녹음 중입니다."); // 중복 녹음 경고
            return;
        }

        // 녹음 시작 (지정된 시간, 44100Hz)
        record = Microphone.Start(Microphone.devices[0].ToString(), false, recordingDuration, 44100);
        aud.clip = record; // AudioSource에 클립 할당
        //Debug.Log("녹음 시작됨");
    }

    // 녹음 종료 메소드
    public void StopSnd()
    {
        // 녹음 중인지 확인
        if (!Microphone.IsRecording(Microphone.devices[0].ToString()))
        {
            Debug.Log("StopSnd에 진입을 하지 못하고 있습니다."); // 녹음 중이 아닐 때 경고 로그 출력
            return; // 녹음 중이 아닐 때는 종료
        }

        Microphone.End(Microphone.devices[0].ToString()); // 녹음 종료

        if (record == null)
        {
            Debug.LogError("녹음된 AudioClip이 없습니다."); // 녹음된 클립이 없을 경우 오류 로그 출력
            return;
        }

        //Debug.Log("녹음 종료 "); // 녹음 종료 로그 출력
        // 녹음된 AudioClip을 확인
        // Debug.Log("녹음된 AudioClip: " + record.name);

        // SpeechToText 인스턴스가 null인지 확인
        if (speechToText != null)
        {
            // 녹음된 AudioClip을 byte[]로 변환하고 Google에 전송
            byte[] audioData = AudioClipToWav.ClipToWav(record); // WAV 형식으로 변환
            byte[] wavFileData = CreateWavFile(audioData); // WAV 파일 생성
            StartCoroutine(speechToText.SendAudioToGoogle(wavFileData)); // SendAudioToGoogle 호출
        }
        else
        {
            Debug.LogError("SpeechToText 인스턴스가 null입니다. 할당을 확인하세요."); // SpeechToText 인스턴스가 없을 경우 오류 로그 출력
        }

        //Debug.Log("녹음 종료");
    }

    // 녹음된 클립 재생 메소드
    public void PlaySnd()
    {
        if (aud.clip != null)
        {
            aud.Play(); // 녹음된 클립 재생
            Debug.Log("재생 중: " + aud.clip.name); // 재생 중인 클립 이름 로그 출력
        }
        else
        {
            Debug.LogError("녹음된 클립이 없습니다."); // 녹음된 클립이 없을 경우 오류 로그 출력
        }
    }

    // 녹음된 오디오 클립 반환 메소드
    public AudioClip GetAudioClip()
    {
        return record; // 녹음된 오디오 클립 반환
    }
}
