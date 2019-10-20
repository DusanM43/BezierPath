using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Player : MonoBehaviour
{
    public S_Ball ballPrefabe;
    public float speed = 5f;
    public float force = 10f;
    
    public float errorMargin = 0.1f;
    private S_Ball ball;

    Rigidbody rb;
    public S_Random rngObject;

    BallColor currentColor, nextColor;

    public Material[] materials = new Material[4];

    // Start is called before the first frame update
    void Start()
    {
        currentColor = (BallColor)rngObject.Next();
        SpawnBall();

    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale != 0)
        {
            LookAtCursor();
            if (rb != null && Input.GetMouseButtonDown(0))
            {
                rb.AddForce(transform.forward * force);
                rb = null;
                Invoke("SpawnBall", 0.3f);
            }
        }
    } 

    void SpawnBall()
    {
        ball = Instantiate(ballPrefabe, transform.position, new Quaternion(0, 0, 0, 1));
        ball.SetColor((int)currentColor);

        Renderer materialRenderer = transform.GetChild(0).GetComponent<Renderer>();
        materialRenderer.material = materials[(int)currentColor];
        
        nextColor = (BallColor)rngObject.Next();

        materialRenderer = transform.GetChild(1).GetComponent<Renderer>();
        materialRenderer.material = materials[(int)nextColor];

        rb = ball.GetComponent<Rigidbody>();
        currentColor = nextColor;
    }

    private void LookAtCursor()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerPos = Camera.main.WorldToScreenPoint(transform.position);

        Vector3 dir = mousePos - playerPos;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        //transform.Rotate(Vector3.up, angle);
        transform.rotation = Quaternion.AngleAxis(-angle + 90f, Vector3.up);
    }
}
