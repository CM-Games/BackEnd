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
            obj.transform.position = new Vector3(-95, Random.Range(pos[0].position.y,pos[1].position.y), Random.Range(20,80));
            yield return new WaitForSeconds(0.3f);
        }
    }
}
