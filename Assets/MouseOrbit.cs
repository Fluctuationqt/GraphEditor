using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class MouseOrbit : MonoBehaviour
{
    public Transform target;
    public float distance = 5.0f;
    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;

    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;

    public float distanceMin = .5f;
    public float distanceMax = 15f;

    float x = 0.0f;
    float y = 0.0f;

    public Graph graph;
    // Use this for initialization
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        try
        {
            graph = GameObject.Find("Graph").GetComponent<Graph>();
        }
        catch(System.Exception ex)
        {
            Debug.Log("Main Camera Orbit script cannot find graph");
        }
        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>() != null)
        {
            GetComponent<Rigidbody>().freezeRotation = true;
        }
    }
    void TargetInfoPrint()
    {
        string info = target.name + System.Environment.NewLine;
        int i = 0;
        foreach (GameObject relation in target.GetComponent<Node>().relations)
        {
            info += "Relation[" + i + "]: (" +
                relation.GetComponent<Relation>().gameObjectTo.transform.name +
                ", " +
                System.Math.Round(relation.GetComponent<Relation>().weight, 2) +
                ");" +
                System.Environment.NewLine;

            i++;
        }
        GameObject.Find("Panel Info").GetComponent<Text>().text = info;
    }
    void GetTarget()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits;
            hits = Physics.RaycastAll(ray, 1000f);
            foreach(RaycastHit hit in hits)
            {
                if (hit.collider.tag == "Node")
                {
                    target = hit.transform;
                    TargetInfoPrint();
                }
            }
        }
    }

    void BFS()
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            Text output = GameObject.Find("Output").GetComponent<Text>();
            output.text = "DFS:" + System.Environment.NewLine;
            graph.StartDFS(target.gameObject, output);
        }
    }
    
    void CreateNode()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Vector2 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    graph.CreateNodeAtPosition(hit.point);
                }
        }
    }
    private GameObject relFrom;
    void CreateRelation()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            if(!relFrom)
            {
                relFrom = target.gameObject;
            }
            else
            {
                GameObject relTo = target.gameObject;
                graph.AddRelation(relFrom, relTo);
                relFrom = null;
            }
        }
    }

    void LateUpdate()
    {
        BFS();
        CreateNode();
        GetTarget();

        if (target)
        {
            CreateRelation();
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                GameObject.Destroy(target.gameObject);
            }
            if (Input.GetMouseButton(1))
            {
                x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.005f;
                y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
            }
            y = ClampAngle(y, yMinLimit, yMaxLimit);

            Quaternion rotation = Quaternion.Euler(y, x, 0);

            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);
            
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + target.position;

            transform.rotation = rotation;
            transform.position = position;
        }
        else
        {
            GetRandomTarget();            
        }
    }
    void GetRandomTarget()
    {
        GameObject[] nodes = GameObject.FindGameObjectsWithTag("Node");
        try
        {
            target = nodes[Random.Range(0, nodes.Length - 1)].transform;
            TargetInfoPrint();

        }
        catch (System.Exception ex)
        {

        }
    }
    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}