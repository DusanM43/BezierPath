using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Ball : MonoBehaviour
{

    private BallColor color;

    public Material[] materials = new Material[4];

    private Renderer materialRenderer ;
    private SphereCollider sphereCollider;

    public bool isInArray = false;

    public float errorMargin = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 1)
            transform.Rotate(transform.up, 1f);
    }

    public void SetColor(int color) {
        this.color = (BallColor)color;
        materialRenderer = GetComponent<Renderer>();
        materialRenderer.material = materials[color];
    }

    public int Color => (int)color;

    public void OnTriggerEnter(Collider other)
    {
        if(!isInArray)
        {
            S_Ball otherObj;
            if (otherObj = other.gameObject.GetComponent<S_Ball>())
            {
                if (otherObj.isInArray)
                {
                    S_Route route = (S_Route)FindObjectOfType(typeof(S_Route));
                    route.AddNewBall(other.gameObject, gameObject);
                }
            }
        }
    }



}

public enum BallColor { Red, Green, Blue, Purple, Yellow, Cyan };

