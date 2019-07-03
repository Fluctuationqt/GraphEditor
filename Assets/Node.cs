using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {
    public List<GameObject> relations = new List<GameObject>();

    public void AddRelation(GameObject relation)
    {
        if(relation != null && !relations.Contains(relation))
        {
            relations.Add(relation);
        }
    }

    public void RemoveRelationToGameObject(GameObject gameObjectTo)
    {
        if(gameObjectTo != null)
        {
            List<GameObject> relationsCopy = new List<GameObject>();
            List<GameObject> trash = new List<GameObject>();

            foreach(GameObject relation in relations)
            {
                if(relation.GetComponent<Relation>().gameObjectTo != gameObjectTo)
                {
                    relationsCopy.Add(relation);
                }else
                {
                    trash.Add(relation);
                }
            }

            relations = relationsCopy;

            foreach(GameObject obj in trash)
            {
                GameObject.Destroy(obj);
            }
        }
    }

    public bool hasRelation(GameObject gameObjectTo)
    {
        if (relations.Count > 0)
        {
            foreach (GameObject relation in relations)
            {
                if (relation.GetComponent<Relation>().gameObjectTo == gameObjectTo)
                    return true;
            }
        }
        return false;
    }
    
    void OnDestroy()
    {
        Graph graph = this.transform.parent.GetComponent<Graph>();
        graph.nodes.Remove(this.gameObject);
        foreach(GameObject node in graph.nodes)
        {
            node.GetComponent<Node>().RemoveRelationToGameObject(this.gameObject);

        }
    }
}
