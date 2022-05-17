using UnityEngine;
using System;
using Random = UnityEngine.Random;

abstract class PerlinNoise
{
    protected int index;
    protected float[] noiseSeed;
    protected float[] perlinNoise;
    protected int octaves;
    protected float scalingBias;

    public PerlinNoise(int octaves, float scalingBias)
    {
        this.octaves = octaves;
        this.scalingBias = scalingBias;
    }

    public int Index { get { return index; } set { index = value; } }
    public int Octaves { get { return octaves; } set { octaves = value; } }
    public float ScalingBias { get { return scalingBias; } set { scalingBias = value; } }
    public abstract int Size { get; }

    public float this[int i] { get { return perlinNoise[i]; } }

    public float Next()
    {
        int oldIndex = index;
        index = (index + 1) % Size;
        return perlinNoise[oldIndex];
    }

    public abstract void NewSeed();
    public abstract void UpdateNoise();
}

class PerlinNoise1D : PerlinNoise
{
    private int size;

    public PerlinNoise1D(int octaves, float scalingBias, int size) : base(octaves, scalingBias)
    {
        NewSize(size);
        NewSeed();
        UpdateNoise();
    }

    public override int Size { get { return size; } }

    public void NewSize(int size)
    {
        this.size = size;
        noiseSeed = new float[size];
        perlinNoise = new float[size];
    }

    public override void NewSeed()
    {
        for (int i = 0; i < size; ++i)
            noiseSeed[i] = Random.Range(-1f, 1f);

        for (int i = 0; i < octaves; ++i)
            noiseSeed[(int)((float)size / octaves * i)] = 0;

        noiseSeed[size - 1] = 0;
    }

    public override void UpdateNoise()
    {
        for (int i = 0; i < size; ++i)
        {
            float noise = 0;
            float scale = 1;
            float scaleAccumulate = 0;

            for (int o = 0; o < octaves; ++o)
            {
                int pitch = size >> o;

                if (pitch == 0)
                {
                    octaves = 2;
                    UpdateNoise();
                    return;
                }

                int sample1 = (i / pitch) * pitch;
                int sample2 = (sample1 + pitch) % size;

                float blend = (float)(i - sample1) / pitch;
                float sample = (1f - blend) * noiseSeed[sample1] + blend * noiseSeed[sample2];

                scaleAccumulate += scale;
                noise += sample * scale;
                scale /= scalingBias;
            }

            perlinNoise[i] = noise * scalingBias / scaleAccumulate;
        }
    }
}

class PerlinNoise2D : PerlinNoise
{
    private int width;
    private int height;

    public PerlinNoise2D(int octaves, float scalingBias, int width, int height) : base(octaves, scalingBias)
    {
        NewSize(width, height);
        NewSeed();
        UpdateNoise();
    }

    public override int Size { get { return width * height; } }
    public int Width { get { return width; } }
    public int Height { get { return height; } }

    public void NewSize(int width, int height)
    {
        this.width = width;
        this.height = height;
        noiseSeed = new float[width * height];
        perlinNoise = new float[width * height];
    }

    public override void NewSeed()
    {
        for (int i = width * height - 1; i != 0; --i)
            noiseSeed[i] = Random.Range(-1f, 1f);
    }

    public override void UpdateNoise()
    {
        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                float noise = 0;
                float scale = 1;
                float scaleAccumulate = 0;

                for (int o = 0; o < octaves; ++o)
                {
                    int pitch = width >> o;

                    if (pitch == 0)
                    {
                        octaves = 2;
                        UpdateNoise();
                        return;
                    }

                    int sampleX1 = (x / pitch) * pitch;
                    int sampleY1 = (y / pitch) * pitch;

                    int sampleX2 = (sampleX1 + pitch) % width;
                    int sampleY2 = (sampleY1 + pitch) % width;

                    float blendX = (float)(x - sampleX1) / pitch;
                    float blendY = (float)(y - sampleY1) / pitch;

                    float sampleT = (1f - blendX) * noiseSeed[sampleY1 * width + sampleX1] + blendX * noiseSeed[sampleY1 * width + sampleX2];
                    float sampleB = (1f - blendX) * noiseSeed[sampleY2 * width + sampleX1] + blendX * noiseSeed[sampleY2 * width + sampleX2];

                    scaleAccumulate += scale;
                    noise += (blendY * (sampleB - sampleT) + sampleT) * scale;
                    scale /= scalingBias;
                }

                perlinNoise[y * width + x] = noise * scalingBias / scaleAccumulate;
            }
        }
    }
}
