using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class S_Route : S_BezierPath
{
    public GameObject controlObj;

    public GameObject ballPrefabe;

    public LinkedList<RouteSegment> objectsArray = new LinkedList<RouteSegment>();
    private LinkedListNode<RouteSegment> refrenceNode;

    public GameObject startNode;
    public GameObject endNode;

    public bool bConstPath = true;
    List<Vector3> constPathPositions = new List<Vector3>();

    public float forwardSpeed = 10f;
    public float backwardSpeed = 20f;
    public float spawnRate = 2f;
    public float errorMargin = 0.1f;

    public float ballSize = 2f;
    int index;
    public S_Random rngObj;

    public class RouteSegment
    {
        public GameObject obj;
        public int routeSegmentIndex;
        public float movementSpeed;
        public Movement movement;

        public RouteSegment(GameObject obj, int routeSegmentIndex, float movementSpeed, Movement movement = Movement.MoveForward)
        {
            this.obj = obj;
            this.routeSegmentIndex = routeSegmentIndex;
            this.movementSpeed = movementSpeed;
            this.movement = movement;
        }
    }

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        startNode.transform.position = nodesArray[1].transform.position;
        endNode.transform.position = nodesArray[nodesArray.Count - 1].transform.position;
        if (Application.isPlaying)
        {
            if (bConstPath)
            {
                for (int i = 0; i < numberOfNodes - 1; i++)
                {
                    float t = 0;
                    while (t <= 1)
                    {
                        Vector3 pos1 = nodesArray[i * 3 + 1].transform.position;
                        Vector3 pos2 = nodesArray[i * 3 + 2].transform.position;
                        Vector3 pos3 = nodesArray[(i + 1) * 3].transform.position;
                        Vector3 pos4 = nodesArray[(i + 1) * 3 + 1].transform.position;
                        constPathPositions.Add(GetPosition(t, pos1, pos2, pos3, pos4));
                        t += 1 / (float)lineResolution;
                    }

                }
                constPathPositions.Add(nodesArray[nodesArray.Count - 2].transform.position);
            }
            InvokeRepeating("SpawnBall", 0, 1f / spawnRate);
            Invoke("SpeedUp", .5f);
            Invoke("SlowDown", 10f);
        }
    }

    void SpeedUp()
    {
        Time.timeScale = 7;
    }

    void SlowDown()
    {
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        Update();
        if (Application.isPlaying)
        {
            if (objectsArray.First != null)
            {
                MoveBalls();
            }
        }
    }

    void SpawnBall()
    {
        objectsArray.AddLast(new RouteSegment(Instantiate(ballPrefabe, startNode.transform.position, new Quaternion(0, 0, 0, 1)), 1, forwardSpeed));
        S_Ball objComponent = objectsArray.Last.Value.obj.GetComponent<S_Ball>();
        objComponent.isInArray = true;
        objComponent.SetColor(rngObj.Next());
    }

    public void MoveObj(GameObject obj, ref int segmentID, float speed, Movement movement = Movement.MoveForward)
    {
        if (bConstPath)
        {
            if (segmentID < constPathPositions.Count && segmentID > 0)
            {
                
                if (movement == Movement.MoveForward || movement == Movement.ForceMove)
                {
                    obj.transform.position = Vector3.MoveTowards(obj.transform.position, constPathPositions[segmentID], speed * Time.deltaTime);
                    if (Vector3.Distance(obj.transform.position, constPathPositions[segmentID]) < errorMargin)
                    {
                        obj.transform.position = constPathPositions[segmentID];
                        segmentID++;
                    }
                }
                else
                {
                    obj.transform.position = Vector3.MoveTowards(obj.transform.position, constPathPositions[segmentID - 1], speed * Time.deltaTime);
                    if (Vector3.Distance(obj.transform.position, constPathPositions[segmentID - 1]) < errorMargin)
                    {
                        obj.transform.position = constPathPositions[segmentID -1];
                        segmentID--;
                    }
                }
            }
        }

    }

    public void AddNewBall(GameObject ballInArray, GameObject firedBall)
    {
        CancelInvoke("ReturnSpeedToNormal");
        for (LinkedListNode<RouteSegment> i = objectsArray.First; i != null; i = i.Next)
        {
            if (i.Value.obj == ballInArray)
            {
                refrenceNode = i;
                break;
            }
        }

        firedBall.GetComponent<Rigidbody>().velocity = Vector3.zero;
        firedBall.GetComponent<S_Ball>().isInArray = true;
        if (refrenceNode.Value.routeSegmentIndex > 1 && refrenceNode.Value.routeSegmentIndex < constPathPositions.Count)
        {
            if (Vector3.Distance(firedBall.transform.position, constPathPositions[refrenceNode.Value.routeSegmentIndex + 1]) >
            Vector3.Distance(firedBall.transform.position, constPathPositions[refrenceNode.Value.routeSegmentIndex - 1]))
            {
                refrenceNode = objectsArray.AddAfter(refrenceNode, new RouteSegment(firedBall, refrenceNode.Value.routeSegmentIndex, 30f, Movement.ForceMove));
            }
            else
            {
                refrenceNode = objectsArray.AddBefore(refrenceNode, new RouteSegment(firedBall, refrenceNode.Value.routeSegmentIndex, 30f, Movement.ForceMove));
            }
        }
        else
        {
            refrenceNode = objectsArray.AddAfter(refrenceNode, new RouteSegment(firedBall, refrenceNode.Value.routeSegmentIndex, 30f, Movement.ForceMove));
        }



        Invoke("ReturnSpeedToNormal", 0.2f);
    }

    private void ReturnSpeedToNormal()
    {
        if (refrenceNode.Next != null)
        {
            if (refrenceNode.Value.obj.GetComponent<S_Ball>().Color == refrenceNode.Next.Value.obj.GetComponent<S_Ball>().Color)
            {
                refrenceNode.Value.movementSpeed = backwardSpeed;
                refrenceNode.Value.movement = Movement.MoveBackward;
                DestroySameColoredBalls(refrenceNode);
            }
            else
            {
                refrenceNode.Value.movementSpeed = forwardSpeed;
                refrenceNode.Value.movement = Movement.MoveForward;
                DestroySameColoredBalls(refrenceNode);
            }
        }
        else
        {
            refrenceNode.Value.movementSpeed = forwardSpeed;
            refrenceNode.Value.movement = Movement.MoveForward;
            DestroySameColoredBalls(refrenceNode);
        }
    }

    private bool DestroySameColoredBalls(LinkedListNode<RouteSegment> node)
    {
        LinkedListNode<RouteSegment> firstNode = node, lastNode = node;
        int addedBallColor = node.Value.obj.GetComponent<S_Ball>().Color;


        int count = -1;

        // Finds the first node that isn't the same color before the refrence node
        while (firstNode.Previous != null && firstNode.Value.obj.GetComponent<S_Ball>().Color == addedBallColor)
        {
            firstNode = firstNode.Previous;
            count++;
        }

        if (firstNode.Value.obj.GetComponent<S_Ball>().Color == addedBallColor) { count++; };

        // Finds the first node that isnt' the same color after the refrence node
        while (lastNode.Next != null && lastNode.Value.obj.GetComponent<S_Ball>().Color == addedBallColor)
        {
            lastNode = lastNode.Next;
            count++;
        }


        if (lastNode.Value.obj.GetComponent<S_Ball>().Color == addedBallColor) { count++; };

        // Destroys objects and removes them from the array
        if (count >= 3)
        {
            controlObj.GetComponent<S_ControlObject>().IncreaseDestroyedBallCounter(count);

            LinkedListNode<RouteSegment> k = firstNode.Next;
            while (k != lastNode)
            {
                Destroy(k.Value.obj);
                LinkedListNode<RouteSegment> old = k;
                k = k.Next;
                objectsArray.Remove(old);
            }
            if (firstNode.Value.obj.GetComponent<S_Ball>().Color == addedBallColor && firstNode.Previous == null) {
                Destroy(objectsArray.First.Value.obj);
                objectsArray.RemoveFirst();
            };
            if (lastNode.Value.obj.GetComponent<S_Ball>().Color == addedBallColor && lastNode.Next == null)
            {
                Destroy(objectsArray.Last.Value.obj);
                objectsArray.RemoveLast();
                if(objectsArray.Last != null)
                    objectsArray.Last.Value.movement = Movement.MoveForward;
            };
            if (firstNode != null && lastNode != null)
                if (firstNode.Value.obj.GetComponent<S_Ball>().Color == lastNode.Value.obj.GetComponent<S_Ball>().Color)
                {
                    firstNode.Value.movement = Movement.MoveBackward;
                }

            
            if (objectsArray.Count == 0)
            {
                controlObj.GetComponent<S_ControlObject>().ShowGameOverMenu();
            }
            return true;
        }
        return false;
    }

    void MoveBalls()
    {
        for (LinkedListNode<RouteSegment> i = objectsArray.Last; i != null; i = i.Previous)
        {            
            // For the last element in the list (last spawned element)
            if (i.Next == null)
            {
                switch (i.Value.movement)
                {
                    case Movement.MoveForward:
                        i.Value.movement = Movement.MoveForward;
                        i.Value.movementSpeed = forwardSpeed;
                        MoveObj(i.Value.obj, ref i.Value.routeSegmentIndex, i.Value.movementSpeed);
                        break;

                    case Movement.MoveBackward:
                        i.Value.movementSpeed = backwardSpeed;
                        i.Value.movement = Movement.MoveBackward;
                        MoveObj(i.Value.obj, ref i.Value.routeSegmentIndex, i.Value.movementSpeed, i.Value.movement); ;
                        break;
                }
            }
            else
            {
                // Move back if there is enough space and if should move backwards
                if ((i.Value.movement == Movement.MoveBackward || i.Next.Value.movement == Movement.MoveBackward) &&
                    Vector3.Distance(i.Value.obj.transform.position, i.Next.Value.obj.transform.position) > (0.3f + ballSize))
                {
                    i.Value.movementSpeed = backwardSpeed;
                    i.Value.movement = Movement.MoveBackward;
                    MoveObj(i.Value.obj, ref i.Value.routeSegmentIndex, i.Value.movementSpeed, i.Value.movement);
                }
                // Forces moving
                else if (i.Value.movement == Movement.ForceMove)
                {
                    i.Value.movement = Movement.MoveForward;
                    i.Value.movementSpeed = forwardSpeed;
                    MoveObj(i.Value.obj, ref i.Value.routeSegmentIndex, i.Value.movementSpeed);
                }
                // Regular movement
                else
                {
                    if(i.Value.movement == Movement.MoveBackward)
                    {                        
                        if (DestroySameColoredBalls(i)) break;
                    }
                    i.Value.movement = Movement.MoveForward;
                    i.Value.movementSpeed = forwardSpeed;
                    if (Vector3.Distance(i.Value.obj.transform.position, i.Next.Value.obj.transform.position) < ballSize)
                    {
                        MoveObj(i.Value.obj, ref i.Value.routeSegmentIndex, i.Value.movementSpeed + 10f);
                    }
                    else if (!(Vector3.Distance(i.Value.obj.transform.position, i.Next.Value.obj.transform.position) < ballSize ||
                        Vector3.Distance(i.Value.obj.transform.position, i.Next.Value.obj.transform.position) > ballSize + 0.3f))
                    {
                        MoveObj(i.Value.obj, ref i.Value.routeSegmentIndex, i.Value.movementSpeed);
                    }
                }
            }
        }
    }

    public void StopSpawning()
    {
        CancelInvoke("SpawnBall");
    }
}

public enum Movement { MoveForward, MoveBackward, Stop, ForceMove };