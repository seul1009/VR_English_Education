using UnityEngine;

public class AudioClipToWav
{
    public static byte[] ClipToWav(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogError("AudioClip is null.");
            return null;
        }

        int samples = clip.samples * clip.channels;
        float[] data = new float[samples];
        clip.GetData(data, 0);

        // WAV 파일의 총 바이트 수 계산
        byte[] byteArray = new byte[samples * 2 + 44]; // 2바이트는 16비트 PCM 포맷
        System.Text.Encoding.ASCII.GetBytes("RIFF").CopyTo(byteArray, 0);
        System.BitConverter.GetBytes(byteArray.Length - 8).CopyTo(byteArray, 4);
        System.Text.Encoding.ASCII.GetBytes("WAVE").CopyTo(byteArray, 8);
        System.Text.Encoding.ASCII.GetBytes("fmt ").CopyTo(byteArray, 12);
        System.BitConverter.GetBytes(16).CopyTo(byteArray, 16); // fmt chunk의 크기
        System.BitConverter.GetBytes((short)1).CopyTo(byteArray, 20); // PCM 포맷
        System.BitConverter.GetBytes((short)clip.channels).CopyTo(byteArray, 22); // 채널 수
        System.BitConverter.GetBytes(44100).CopyTo(byteArray, 24); // 샘플 레이트를 44100으로 설정
        System.BitConverter.GetBytes(44100 * 2 * clip.channels).CopyTo(byteArray, 28); // 바이트 레이트
        System.BitConverter.GetBytes((short)(2 * clip.channels)).CopyTo(byteArray, 32); // 블록 정렬
        System.Text.Encoding.ASCII.GetBytes("data").CopyTo(byteArray, 36); // data 태그
        System.BitConverter.GetBytes(samples * 2).CopyTo(byteArray, 40); // 데이터 크기

        // PCM 데이터를 16비트 형식으로 변환하여 배열에 추가
        for (int i = 0; i < data.Length; i++)
        {
            short temp = (short)(data[i] * short.MaxValue);
            byteArray[44 + i * 2] = (byte)(temp & 0xff);
            byteArray[44 + i * 2 + 1] = (byte)((temp >> 8) & 0xff);
        }

        return byteArray;
    }
}