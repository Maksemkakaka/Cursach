using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireExtinguishing : MonoBehaviour
{
    public ParticleSystem fireParticles;
    public float extinguishRate = 1f; // ��������, � ������� ����� ����� ��������
    public bool IsFireActive { get; private set; } = true;

    public void ExtinguishFire(float amount)
    {
        // ��������� ����� ������� ���������� ������
        var emission = fireParticles.emission;
        var rateOverTime = emission.rateOverTime;
        rateOverTime.constantMax -= amount * extinguishRate * Time.deltaTime;
        rateOverTime.constantMax = Mathf.Max(0, rateOverTime.constantMax);
        emission.rateOverTime = rateOverTime;

        // ���� ���������� ������ �������� ����, ������������� ������� ������
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

