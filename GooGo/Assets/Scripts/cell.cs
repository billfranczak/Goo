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
     */
    int timer;
    public bool isPaintable;
    public int closestPlayer;
    public float grad;

    Renderer r;
	void Start () {
        r = GetComponent<Renderer>();
        r.material.color = Color.white;
        painter = 0;
        timer = 0;
        isPaintable = true;
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
            GetComponent<Collider2D>().isTrigger = false;
        }
    }

    public void updateClosestPlayer()
    {

    }
}
