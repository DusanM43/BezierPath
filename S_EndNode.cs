using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_EndNode : MonoBehaviour
{

    public S_ControlObject controlObject;
    private SphereCollider sphereCollider;

    // Start is called before the first frame update
    void Start() => sphereCollider = GetComponent<SphereCollider>();

    // Update is called once per frame
    void OnTriggerEnter(Collider collider)
    {/*
        if (collider.gameObject.GetComponent<S_Ball>().isInArray)
        {
            Time.timeScale = 0;
            controlObject.ShowGameOverMenu();
        }*/
    }
}
