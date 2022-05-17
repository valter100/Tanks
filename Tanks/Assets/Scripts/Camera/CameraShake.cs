using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float maxRotation;
    [SerializeField] private int maxOffset;
    [SerializeField] private int noiseSize;
    [SerializeField] private float maxTrauma;

    [SerializeField] private float trauma;
    [SerializeField] private float rotation;
    [SerializeField] private Vector2 offset;

    private PerlinNoise1D rotationNoise;
    private PerlinNoise1D offsetNoise;
    private PerlinNoise1D offsetAngleNoise;

    private static CameraShake cameraShake;

    public float Rotation => rotation;
    public Vector2 Offset => offset;

    private void Start()
    {
        cameraShake = this;

        rotationNoise    = new PerlinNoise1D(6, 0.5f, noiseSize);
        offsetNoise      = new PerlinNoise1D(6, 0.5f, noiseSize);
        offsetAngleNoise = new PerlinNoise1D(6, 0.5f, noiseSize);

        rotationNoise.Index    = 0;
        offsetNoise.Index      = noiseSize / 3;
        offsetAngleNoise.Index = noiseSize / 3 * 2;
    }

    private void Update()
    {
        DecreaseTrauma();
        CalculateEffectOfTrauma();
        UpdateNoise();
    }

    public static void AddTrauma(float amount)
    {
        cameraShake.trauma += amount;
        cameraShake.trauma = Mathf.Clamp(cameraShake.trauma, 0f, cameraShake.maxTrauma);
    }

    public static void ClearTrauma()
    {
        cameraShake.trauma = 0f;
    }

    private void DecreaseTrauma()
    {
        if (trauma > 0f)
        {
            trauma -= Time.deltaTime;

            if (trauma < 0f)
                trauma = 0f;
        }
    }

    private void CalculateEffectOfTrauma()
    {
        if (trauma == 0f)
        {
            rotation = 0f;
            offset.x = 0f;
            offset.y = 0f;
            return;
        }

        // 0   trauma = 0 shake
        // 1.5 trauma ~ 1 shake
        // 2   trauma = 1 shake
        // inf trauma = 1 shake

        float shake = 2f / (1f + Mathf.Pow(4f, -(trauma * trauma))) - 1f;

        rotation = maxRotation * shake * rotationNoise.Next();
        offset = new Vector2(0, maxOffset * shake * offsetNoise.Next()).RotateAroundZero(Mathf.PI * 2f * offsetAngleNoise.Next());
    }

    private void UpdateNoise()
    {
        if (rotationNoise.Index == 0)
        {
            rotationNoise.NewSeed();
            rotationNoise.UpdateNoise();
        }

        else if (offsetNoise.Index == 0)
        {
            offsetNoise.NewSeed();
            offsetNoise.UpdateNoise();
        }

        else if (offsetAngleNoise.Index == 0)
        {
            offsetAngleNoise.NewSeed();
            offsetAngleNoise.UpdateNoise();
        }
    }

}
