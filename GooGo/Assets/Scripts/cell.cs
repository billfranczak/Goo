using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cell : MonoBehaviour {

    
    int painter; 
    /*
     * 0: unpainted
     * 1: painted by p1
     * 2: painted by p2
     * 3: painted by both players
     * 4: map terrain, unpaintable and impassable
     */
    int timer;
    int hCellNum;
    int vCellNum;
    public int maxTime;
    public bool isPaintable;
    public bool impassable;
    public int closestPlayer;
    public float closestDist;
    public float grad;
    public bool marked;
    public Vector2[] bfs;
    public float[] dists;

    Renderer r;
	void Start () {
        hCellNum = 100;
        vCellNum = 100;
        impassable = false;
        r = GetComponent<Renderer>();
        r.material.color = Color.white;
        painter = 0;
        timer = 0;
        maxTime = 60;
        isPaintable = true;
        GetComponent<Renderer>().sortingLayerName = "LayerName";
        GetComponent<Renderer>().sortingOrder = 0;
        closestDist = Mathf.Infinity;
        //renderer.sortingLayerName = "LayerName";
        //renderer.sortingOrder = 0;
        bfs = new Vector2[hCellNum * vCellNum];
        dists = new float[hCellNum * vCellNum];
    }
	
	// Update is called once per frame
	void Update () {
        if (timer>0)
        {
            timer--;
        }
        if (timer==1)
        {
            isPaintable = false;
            GetComponent<Collider>().isTrigger = false;
            //Debug.Log("disabling cell");
        }
    }

    public void updateClosestPlayer()
    {

    }

    public void color(int i, Color c)
    {
        timer = maxTime; 
        if (painter ==0)
        {
            painter = i;
            r.material.color = c;
        } 
        else if (painter != i)
        {
            painter = 3;
            r.material.color = Color.black;
        }
    }

}
