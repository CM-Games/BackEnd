using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorManager : MonoBehaviour
{
    public Transform[] pos;

    // Start is called before the first frame update
    void Start()
    {
         StartCoroutine(GeneratorRock());      
    }

    IEnumerator GeneratorRock()
    {
        while (true)
        {            
            GameObject obj = PoolingManager.instance.getPool();
            obj.transform.position = new Vector3(-10, Random.Range(pos[0].position.y,pos[1].position.y), 0);
            yield return new WaitForSeconds(1f);
        }
    }
}
