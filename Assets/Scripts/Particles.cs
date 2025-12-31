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
