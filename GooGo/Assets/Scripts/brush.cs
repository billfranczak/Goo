using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class brush : MonoBehaviour {

    public Transform pos;
    public Renderer r;

    public int ammo;
    public int playerNum;
    public Color myColor;
    public bool isPainting;
    public bool onBucket;
    public bool onGrid;
    public int maxAmmo;
    public int paintTypeThreshold;

	// Use this for initialization
	void Start () {
        pos = GetComponent<Transform>();
        r = GetComponent<Renderer>();
        r.material.color = Color.cyan;
        myColor = Color.blue;
        ammo = 1;
        playerNum = 0;
        onBucket = false;
        onGrid = false;
        maxAmmo = 360;
        paintTypeThreshold = 180;
	}
	
	// Update is called once per frame
	void Update () {
		if (onBucket && isPainting)
        {
            if (ammo <maxAmmo)
            {
                ammo++;
            }

        }
	}

    public void moveToXY(float x, float y)
    {
        pos.position = new Vector3(x,y,0);
    }

    public void startPainting()
    {
        if (ammo>0)
        {
            r.material.color = Color.blue;
            isPainting = true;
        }
        
    }

    public void stopPainting()
    {
        r.material.color = Color.cyan;
        isPainting = false;
    }

    public void OnTriggerEnter(Collider col)
    {
        if (col.tag == "bucket")
        {
            if (col.GetComponent<bucket>().playerNum==playerNum)
            {
                onBucket = true;
            }
        }
        if (col.tag == "cell")
        {
            onGrid = true;
        }
            
    }

    public void OnTriggerExit(Collider col)
    {
        if (col.tag == "bucket")
        {
            //Debug.Log("buckets");
            if (col.GetComponent<bucket>().playerNum == playerNum)
            {
                onBucket = false;
                if (ammo<maxAmmo && ammo>paintTypeThreshold)
                {
                    ammo = paintTypeThreshold;
                }
            }
        }
        if (col.tag == "cell")
        {
            onGrid = false;
        }
    }


}
