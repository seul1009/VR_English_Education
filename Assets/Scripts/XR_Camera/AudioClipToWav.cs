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

        // WAV ������ �� ����Ʈ �� ���
        byte[] byteArray = new byte[samples * 2 + 44]; // 2����Ʈ�� 16��Ʈ PCM ����
        System.Text.Encoding.ASCII.GetBytes("RIFF").CopyTo(byteArray, 0);
        System.BitConverter.GetBytes(byteArray.Length - 8).CopyTo(byteArray, 4);
        System.Text.Encoding.ASCII.GetBytes("WAVE").CopyTo(byteArray, 8);
        System.Text.Encoding.ASCII.GetBytes("fmt ").CopyTo(byteArray, 12);
        System.BitConverter.GetBytes(16).CopyTo(byteArray, 16); // fmt chunk�� ũ��
        System.BitConverter.GetBytes((short)1).CopyTo(byteArray, 20); // PCM ����
        System.BitConverter.GetBytes((short)clip.channels).CopyTo(byteArray, 22); // ä�� ��
        System.BitConverter.GetBytes(44100).CopyTo(byteArray, 24); // ���� ����Ʈ�� 44100���� ����
        System.BitConverter.GetBytes(44100 * 2 * clip.channels).CopyTo(byteArray, 28); // ����Ʈ ����Ʈ
        System.BitConverter.GetBytes((short)(2 * clip.channels)).CopyTo(byteArray, 32); // ��� ����
        System.Text.Encoding.ASCII.GetBytes("data").CopyTo(byteArray, 36); // data �±�
        System.BitConverter.GetBytes(samples * 2).CopyTo(byteArray, 40); // ������ ũ��

        // PCM �����͸� 16��Ʈ �������� ��ȯ�Ͽ� �迭�� �߰�
        for (int i = 0; i < data.Length; i++)
        {
            short temp = (short)(data[i] * short.MaxValue);
            byteArray[44 + i * 2] = (byte)(temp & 0xff);
            byteArray[44 + i * 2 + 1] = (byte)((temp >> 8) & 0xff);
        }

        return byteArray;
    }
}