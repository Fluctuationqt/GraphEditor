using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Relation : MonoBehaviour {
    public GameObject gameObjectTo;
    public float weight;
    
    public Relation(GameObject to, float weight)
    {
        this.gameObjectTo = to;
        this.weight = weight;
    }

    public GameObject gameObjectFrom()
    {
        return this.transform.parent.gameObject;
    }
    
    public GameObject arrow;
    private Quaternion _facing;

    void Start()
    {
        arrow = transform.Find("Arrow").gameObject;
        _facing = arrow.transform.rotation;
    }

    void Update()
    {
        Vector3 origin = this.gameObjectFrom().transform.position;
        Vector3 end = this.gameObjectTo.transform.position;
        float distance = Vector3.Distance(origin, end);
        this.weight = distance;
        Vector3 position = Vector3.Lerp(origin, end, 0.5f);
        arrow.transform.position = position;
        
        Quaternion rotation = Quaternion.LookRotation((origin - end).normalized);
        
        rotation *= _facing;
        arrow.transform.rotation = rotation;
        arrow.transform.localScale = new Vector3(1, distance/2f - gameObjectTo.transform.localScale.x/2,1);
    }
}
