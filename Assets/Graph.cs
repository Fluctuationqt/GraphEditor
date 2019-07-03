using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Graph : MonoBehaviour
{
    public List<GameObject> nodes = new List<GameObject>();
    public GameObject nodePrefab;
    public GameObject relationPrefab;
    int nodeIndex = 0;

    void Start()
    {
        dfsBlue = false;
        try
        {
            nodePrefab = (GameObject)Resources.Load("Prefabs/Node");
            relationPrefab = (GameObject)Resources.Load("Prefabs/Relation");
        }
        catch (System.Exception ex)
        {
            Debug.Log("Could not load resources! " + ex.Data);
        }
    }

    public void AddRelation(GameObject nodeFrom, GameObject nodeTo)
    {
        try
        {
            if (nodeFrom != null && nodeTo != null && nodeFrom != nodeTo && !nodeFrom.GetComponent<Node>().hasRelation(nodeTo))
            {
                GameObject relation =
                   GameObject.Instantiate(relationPrefab, nodeFrom.transform.position, Quaternion.identity, nodeFrom.transform);
                relation.GetComponent<Relation>().gameObjectTo = nodeTo;
                relation.transform.name = nodeTo.transform.name;
                nodeFrom.GetComponent<Node>().AddRelation(relation);
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("Could not create relation" + ex.Data);
        }
    }

    public GameObject FindNodeByName(string nodeName)
    {
        foreach (GameObject node in nodes)
        {
            if (node.transform.name == nodeName)
                return node;
        }

        return null;
    }
     
    public void AddRelation2(GameObject nodeFrom, string nodeToName)
    {
        try
        { 
            GameObject nodeTo = FindNodeByName(nodeToName);
            
            if (nodeTo != null && nodeFrom != null && nodeFrom != nodeTo)
            {
                GameObject relation =
                    GameObject.Instantiate(relationPrefab, nodeFrom.transform.position, Quaternion.identity, nodeFrom.transform);
                relation.GetComponent<Relation>().gameObjectTo = nodeTo;
                relation.transform.name = nodeTo.transform.name;
                nodeFrom.GetComponent<Node>().AddRelation(relation);
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("Could not create relation");
        }
    }

    public GameObject CreateNodeAtPosition(Vector3 position, string name = "")
    {

        GameObject node = GameObject.Instantiate(nodePrefab, position, Quaternion.identity, this.transform);
        if (name == "")
        {
            node.transform.name = "Node " + nodeIndex;
            nodeIndex++;
        }
        else
        {
            node.transform.name = name;
        }
        nodes.Add(node);
        return node;
    }

    public void AddNodesInScene()
    {
        foreach (GameObject sceneNode in GameObject.FindGameObjectsWithTag("Node"))
        {
            if (!nodes.Contains(sceneNode))
            {
                sceneNode.transform.name = "Node" + nodeIndex;
                nodeIndex++;
                nodes.Add(sceneNode);
                sceneNode.transform.parent = this.transform;
            }
        }
    }

    public void RelateAllNodes()
    {
        foreach (GameObject nodeFrom in nodes)
        {
            foreach (GameObject nodeTo in nodes)
            {
                if (nodeFrom == nodeTo)
                    continue;

                if (!nodeFrom.GetComponent<Node>().hasRelation(nodeTo))
                {
                    GameObject relation =
                        GameObject.Instantiate(relationPrefab, nodeFrom.transform.position, Quaternion.identity, nodeFrom.transform);
                    relation.GetComponent<Relation>().gameObjectTo = nodeTo;
                    nodeFrom.GetComponent<Node>().AddRelation(relation);
                }
            }
        }
    }

    // csv to graph
    public void CreateFromFile(string path)
    {
        try
        {
            using (var reader = new StreamReader(@path))
            {
                
                List<GameObject> createdNodes = new List<GameObject>();

                // read entire file and create nodes
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    string nodeName = values[0];
                    Vector3 position =
                        new Vector3(float.Parse(values[1], CultureInfo.InvariantCulture),
                                    float.Parse(values[2], CultureInfo.InvariantCulture),
                                    float.Parse(values[3], CultureInfo.InvariantCulture));
                    GameObject createdNode = CreateNodeAtPosition(position, nodeName);
                    createdNodes.Add(createdNode);
                }

                // reset stream reader
                reader.DiscardBufferedData();
                reader.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
                
                // read file again and create relations
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    foreach (GameObject createdNode in createdNodes)
                    {
                        if (createdNode.transform.name == values[0])
                        {
                            for (int i = 4; i < values.GetLength(0); i++)
                            {
                                AddRelation2(createdNode, values[i]);
                            }
                        }
                    }
                }

            }
        } catch(System.Exception ex)
        {
            Debug.Log("Could not load file at location: " + path + " " + ex.Message); //TODO: print this in label
        }
    }

    public void SaveGraphToFile(string path)
    {
        var csv = new StringBuilder();
        foreach(GameObject node in nodes)
        {
            string name = node.transform.name;
            string x = node.transform.position.x.ToString();
            string y = node.transform.position.y.ToString();
            string z = node.transform.position.z.ToString();
            string relationsString = "";
           
            foreach (GameObject relation in node.GetComponent<Node>().relations)
            {
                relationsString += "," + relation.GetComponent<Relation>().gameObjectTo.transform.name;
            }
           
            var newLine = string.Format("{0},{1},{2},{3}{4}", name, x, y, z, relationsString);
            csv.AppendLine(newLine);
        }
        File.WriteAllText(path, csv.ToString());
    }
    private bool dfsBlue;
    IEnumerator DFS(GameObject node, List<GameObject> visitedNodes, Color color, Text output)
    {
        if (!visitedNodes.Contains(node))
        {
            Debug.Log(node.transform.name);
            List<GameObject> relations = node.GetComponent<Node>().relations;
            visitedNodes.Add(node);
            output.text += node.transform.name + System.Environment.NewLine;
            foreach (GameObject next in relations)
            {
                MeshRenderer mr = next.transform.Find("Arrow").transform.Find("Cylinder").GetComponent<MeshRenderer>();
                mr.material.color = color;
                yield return new WaitForSeconds(0.1f);
                StartCoroutine(DFS(next.GetComponent<Relation>().gameObjectTo, visitedNodes, color, output));
                if (dfsBlue)
                {
                    mr.material.color = Color.blue;
                }
                else
                {
                    mr.material.color = Color.white;
                }
            }
            
        }
    }
    public void StartDFS(GameObject nodeFrom, Text output)
    {
        List<GameObject> visitedNodes = new List<GameObject>();
        StartCoroutine(DFS(nodeFrom, visitedNodes, Color.red, output));
        if (dfsBlue == false)
            dfsBlue = true;
        else
            dfsBlue = false;
    }

}
