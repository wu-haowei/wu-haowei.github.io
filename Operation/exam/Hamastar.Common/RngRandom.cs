using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

/// <summary>
/// RngRandom 的摘要描述
/// </summary>
public class RngRandom
{
    private const int BufferSize = 1024;  // must be a multiple of 4
    private byte[] RandomBuffer;
    private int BufferOffset;
    private RNGCryptoServiceProvider rng;
    public RngRandom()
    {
        RandomBuffer = new byte[BufferSize];
        rng = new RNGCryptoServiceProvider();
        BufferOffset = RandomBuffer.Length;
    }
    private void FillBuffer()
    {
        rng.GetBytes(RandomBuffer);
        BufferOffset = 0;
    }
    public int RND()
    {
        if (BufferOffset >= RandomBuffer.Length)
        {
            FillBuffer();
        }
        int val = BitConverter.ToInt32(RandomBuffer, BufferOffset) & 0x7fffffff;
        BufferOffset += sizeof(int);
        return val;
    }
    public int RND(int maxValue)
    {
        return RND() % maxValue;
    }
    public int RND(int minValue, int maxValue)
    {
        if (maxValue < minValue)
        {
            throw new ArgumentOutOfRangeException("最大值必須小於或等於最小值");
        }
        int range = maxValue - minValue;
        return minValue + RND(range);
    }
    public double NextDouble()
    {
        int val = RND();
        return (double)val / int.MaxValue;
    }
    public void GetBytes(byte[] buff)
    {
        rng.GetBytes(buff);
    }
}