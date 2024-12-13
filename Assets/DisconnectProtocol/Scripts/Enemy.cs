using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent m_meshAgent;
    private Transform m_target;

    public int Num { get; } = 5;

    private void Awake()
    {
        m_meshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (m_target)
        {
            if (Time.frameCount % 3 == 0)
            {
                m_meshAgent.destination = m_target.position;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent<Player>(out var player))
        {
            m_target = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<Player>(out var player))
        {
            m_target = player.transform;
        }
    }
}
