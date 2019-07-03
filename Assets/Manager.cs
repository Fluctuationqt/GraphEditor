using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour {
    public GameObject graphGameObject;
    private Graph graph;
    private bool relateAll = false;
	// Use this for initialization
	void Start ()
    {
        graph = graphGameObject.GetComponent<Graph>();
        //graph.CreateNodeAtPosition(new Vector3(0, 0, 0));
        //graph.CreateNodeAtPosition(new Vector3(5, 0, 0));
        //graph.CreateNodeAtPosition(new Vector3(0, 5, 0));
        //graph.CreateNodeAtPosition(new Vector3(0, 0, 5));
        //
        //graph.RelateAllNodes();

        /* TODO: Spawn Node via space bar
         * TODO: Create Relation via: Select a node -> Press R -> Select a second node 
         * TODO: MoveNodes along axes left,right,up,down (draw a dragger)
         * TODO: Dijkstra
         * Profit.
         */


    }

    public void LoadFile()
    {
        string fileName = GameObject.Find("FileName").GetComponent<InputField>().text;
        graph.CreateFromFile(fileName); // Ex: "Assets/SavedGraph.csv"
    } 

    public void SaveFile()
    {
        string fileName = GameObject.Find("FileName").GetComponent<InputField>().text;
        graph.SaveGraphToFile(fileName);
    }

    public void RelateAllSwitch()
    {
        if(GameObject.Find("RelateAllToggle").GetComponent<Toggle>().isOn)
        {
            relateAll = true;
        }
        else
        {
            relateAll = false;
        }
    }

	// Update is called once per frame
	void Update ()
    {
        
        if (relateAll)
        {
            graph.RelateAllNodes();
        }

        graph.AddNodesInScene();
    }
}
