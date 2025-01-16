using Unity.AI.Navigation;
using UnityEngine;

public class NavMeshBake : MonoBehaviour
{
    public NavMeshSurface m_Surface;
    void Start()
    {
        m_Surface.BuildNavMesh();
    }
}
