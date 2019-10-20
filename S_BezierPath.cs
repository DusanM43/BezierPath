using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteAlways]
public class S_BezierPath : MonoBehaviour
{
    [Range(2, 30)]
    public int numberOfNodes = 2;

    [Range(2, 30)]
    public int lineResolution = 10;

    // Meshes/Sprites that represent nodes and control points
    public GameObject nodePrefabe;
    public GameObject inControlPointPrefabe;
    public GameObject outControlPointPrefabe;

    public List<GameObject> nodesArray = new List<GameObject>();

    LineRenderer lr;

    public bool bDrawPath = true;

    // Start is called before the first frame update
    protected void Start()
    {
        if (Application.isPlaying)
        {
            if (!bDrawPath)
            {
                foreach(GameObject node in nodesArray)
                    node.GetComponent<MeshRenderer>().enabled = false;
                if ((lr = GetComponent<LineRenderer>()))
                    lr.enabled = false;
            }
        }
    }

    // Update is called once per frame
    protected void Update()
    {

        if (!Application.isPlaying)
        {
            if (nodePrefabe && inControlPointPrefabe && outControlPointPrefabe)
            {
                if (transform.childCount == 0)
                {
                    CreatePath();
                }
                else
                {
                    if (numberOfNodes * 3 > nodesArray.Count)
                        AddNodes();
                    if (numberOfNodes * 3 < nodesArray.Count)
                        DeleteNodes();
                }
            }
            DrawPath();
        }
        if (Application.isPlaying)
        {
        }

    }
    void CreateNode(int index)
    {
        GameObject node = Instantiate(nodePrefabe, new Vector3((float)index, 0), new Quaternion());
        GameObject inControlPoint = Instantiate(inControlPointPrefabe, new Vector3((float)index, 0, 1), new Quaternion());
        GameObject outControlPoint = Instantiate(outControlPointPrefabe, new Vector3((float)index, 0, -1), new Quaternion());

        node.transform.SetParent(transform);
        inControlPoint.transform.SetParent(node.transform);
        outControlPoint.transform.SetParent(node.transform);

        node.name = "Node " + (index + 1).ToString();
        inControlPoint.name = "In Control Point";
        outControlPoint.name = "Out Control Point";


        nodesArray.Add(inControlPoint);
        nodesArray.Add(node);
        nodesArray.Add(outControlPoint);
    }

    private void CreatePath()
    {
        DeletePath();
        for (int i = 0; i < numberOfNodes; i++)
            CreateNode(i);
    }

    private void AddNodes()
    {
        for (int i = nodesArray.Count / 3; i < numberOfNodes; i++)
            CreateNode(i);
    }

    private void DeletePath()
    {
        while (nodesArray.Count > 0)
        {
            DestroyImmediate(nodesArray[nodesArray.Count - 1]);
            nodesArray.Remove(nodesArray[nodesArray.Count - 1]);
        }
    }

    private void DeleteNodes()
    {
        while (nodesArray.Count > numberOfNodes * 3)
        {
            DestroyImmediate(nodesArray[nodesArray.Count - 1]);
            nodesArray.Remove(nodesArray[nodesArray.Count - 1]);
        }
    }

    public void DrawPath()
    {
        if (!(lr = GetComponent<LineRenderer>()))
        {
            lr = gameObject.AddComponent<LineRenderer>();
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
        }


        lr.positionCount = (numberOfNodes - 1) * lineResolution + 1;

        Vector3[] linePositions = new Vector3[numberOfNodes * 30];
        int lrCount = 0;

        for (int i = 0; i < numberOfNodes - 1; i++)
        {
            float t = 0;
            while (t <= 1)
            {
                Vector3 pos1 = nodesArray[i * 3 + 1].transform.position;
                Vector3 pos2 = nodesArray[i * 3 + 2].transform.position;
                Vector3 pos3 = nodesArray[(i + 1) * 3].transform.position;
                Vector3 pos4 = nodesArray[(i + 1) * 3 + 1].transform.position;
                linePositions[lrCount++] = GetPosition(t, pos1, pos2, pos3, pos4);
                t += 1 / (float)lineResolution;
            }

        }
        linePositions[lrCount++] = nodesArray[nodesArray.Count - 2].transform.position;
        lr.SetPositions(linePositions);
    }
    
    protected Vector3 GetPosition(float t, Vector3 pos1, Vector3 pos2, Vector3 pos3, Vector3 pos4)
    {
        return Mathf.Pow(1 - t, 3) * pos1 + 3 * Mathf.Pow(1 - t, 2) * t * pos2 + 3 * (1 - t) * Mathf.Pow(t, 2) * pos3 + Mathf.Pow(t, 3) * pos4;
    }

}
