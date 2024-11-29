using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestInput : MonoBehaviour
{
    private bool firstUpdate = false;
    
    void Start()
    {
        StartCoroutine(Test());
    }

    void Update()
    {
        if (!firstUpdate)
        {
            Debug.Log("Update");
        }
    }

    IEnumerator Test()
    {
        for(int i = 0; i < 3; ++i) 
        {
            Debug.Log($"Test Func {i}");
            yield return new WaitForSeconds(1);
            Debug.Log($"Test Func {i} end");
        }
        
        firstUpdate = true;
    }

    private void LateUpdate()
    {
        if (!firstUpdate)
        {
            Debug.Log("LateUpdate");
        }
    }
}
