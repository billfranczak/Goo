using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _netCell : MonoBehaviour
{


    public int painter;
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

    public Renderer r;
    void Start()
    {
        hCellNum = 25;
        vCellNum = 25;
        impassable = false;
        r = GetComponent<Renderer>();
        r.material.SetFloat("_Mode", 3);
        r.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        r.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        r.material.SetInt("_ZWrite", 0);
        r.material.DisableKeyword("_ALPHATEST_ON");
        r.material.EnableKeyword("_ALPHABLEND_ON");
        r.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        r.material.renderQueue = 3000;
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

        GetComponent<Renderer>().sortingLayerName = "all";
        GetComponent<Renderer>().sortingOrder = 1;

    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
        {
            timer--;
        }
        if (timer == 1)
        {
            isPaintable = false;
            GetComponent<Collider>().isTrigger = false;
            closestDist = 0;
            //Debug.Log("disabling cell");
        }
    }

    /* deprocate and move to GM
    public void updateClosestPlayer()
    {
        int t = 0;
        while (dists[t] < closestDist)
        {
            if ()
            {

            }
            t++;
        }
    } */

    public void color(int i, Color c)
    {
        timer = maxTime;
        if (painter == 0)
        {
            painter = i;
            r.material.color = c;
            //Debug.Log("painted by" + i);
        }
        else if (painter != i)
        {
            painter = 3;
            r.material.color = Color.black;
            //Debug.Log("double paint!!!!");
        }
    }

    public void updateClosestVisual(Color c)
    {
        //Debug.Log("yo");
        if (painter == 0)
        {
            Color c1 = c;
            c1.a = .3f * Mathf.Max((((hCellNum) - closestDist) / (hCellNum)), 0);
            //Debug.Log(c1.a);
            r.material.color = c1;
            //Debug.Log(r.material.color.a);
        }
    }

}
