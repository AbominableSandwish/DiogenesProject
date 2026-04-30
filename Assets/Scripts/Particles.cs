/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

using UnityEngine;

public class Particles : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem m_ParticleSystem;

    public void PlayParticles()
    {
        m_ParticleSystem.Play();
    }
}
