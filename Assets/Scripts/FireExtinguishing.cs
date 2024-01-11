using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireExtinguishing : MonoBehaviour
{
    public ParticleSystem fireParticles;
    public float extinguishRate = 1f; // Скорость, с которой огонь будет тушиться
    public bool IsFireActive { get; private set; } = true;

    public void ExtinguishFire(float amount)
    {
        // Вычисляем новый процент оставшихся частиц
        var emission = fireParticles.emission;
        var rateOverTime = emission.rateOverTime;
        rateOverTime.constantMax -= amount * extinguishRate * Time.deltaTime;
        rateOverTime.constantMax = Mathf.Max(0, rateOverTime.constantMax);
        emission.rateOverTime = rateOverTime;

        // Если количество частиц достигло нуля, останавливаем систему частиц
        if (rateOverTime.constantMax <= 0)
        {
            CompleteExtinguishing();
        }
    }


    public void CompleteExtinguishing()
    {
        if (!IsFireActive)
            return;
        fireParticles.Stop();
        fireParticles.Clear();
        IsFireActive = false;
    }
}

