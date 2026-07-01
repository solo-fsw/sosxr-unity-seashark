using SOSXR.EnhancedLogger;
using SOSXR.SeaShark;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class PokeDeformer : MonoBehaviour
{
    [UnityEngine.Header("Poke Settings")]
    [SerializeField] private float m_pokeStrength = 0.5f;
    [SerializeField] private float m_pokeRadius = 0.05f;
    [SerializeField] [UnityEngine.TagSelector] private string m_targetTag = "Deformable";

    [SerializeField] private float m_rodLength = 0.475f;

    [SerializeField] private AudioSource m_pokeSoundSource;
    [SerializeField] private SkinnedMeshRenderer m_skinnedMeshRenderer;

    private static readonly int PokeCenter = Shader.PropertyToID("_PokeCenter");
    private static readonly int PokeRadius = Shader.PropertyToID("_PokeRadius");
    private static readonly int PokeStrength = Shader.PropertyToID("_PokeStrength");

    private Material _material;


    private void Awake()
    {
        if (m_skinnedMeshRenderer == null)
        {
            this.Warning("SkinnedMeshRenderer not assigned, will try to get from collided object.");
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(m_targetTag))
        {
            return;
        }

        if (m_pokeSoundSource != null && !m_pokeSoundSource.isPlaying)
        {
            m_pokeSoundSource.Play();
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag(m_targetTag))
        {
            return;
        }

        this.Verbose("Poke objects collided");

        if (m_skinnedMeshRenderer == null)
        {
            m_skinnedMeshRenderer = other.GetComponent<SkinnedMeshRenderer>();
        }

        if (m_skinnedMeshRenderer != null && _material == null)
        {
            _material = m_skinnedMeshRenderer.material;
            this.Info($"Got material {_material.name}");
        }

        if (_material == null)
        {
            this.Error("We couldn't get a material, which is needed for deformation. Which means we probably couldn't get the SkinnedMeshRenderer.");

            return;
        }

        var tipPos = transform.position + transform.forward * (m_rodLength * 0.5f);

        _material.SetVector("_PokeCenter", tipPos);
        _material.SetFloat("_PokeRadius", m_pokeRadius);
        _material.SetFloat("_PokeStrength", m_pokeStrength);
    }


    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(m_targetTag))
        {
            return;
        }

        if (_material != null)
        {
            _material.SetFloat("_PokeStrength", 0f);
        }

        _material = null;
    }
}