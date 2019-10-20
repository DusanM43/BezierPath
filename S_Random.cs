using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Random : MonoBehaviour
{
    System.Random rnd;
    
    // Start is called before the first frame update
    void Awake()
    {
        rnd = new System.Random();
    }    

    // Update is called once per frame
    void Update()
    {
        
    }

    public int Next()
    {
        return rnd.Next(4);
    }
}
