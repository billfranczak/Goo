using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class gm : MonoBehaviour {

    public GameObject cellPrefab;
    public GameObject brushPrefab;
    public GameObject inputHandlerPrefab;
    public GameObject bucketPrefab;

    float cellSize;
    int hCellNum;
    int vCellNum;
    public GameObject[,] cells;
    public Vector2[,] celLox;
    public cell[,] gotCell;
    
    Vector3 origin;
    GameObject newCell;

    public brush p1Brush;
    public brush p2Brush;
    public inputHandler p1Input;
    public inputHandler p2Input;
    public bucket p1bucket;
    public bucket p2bucket;
    public bool p1oom;
    public bool frame1;

    void Start () {
        cellSize = .05f; 
        hCellNum = 100;
        vCellNum = 100;
        origin = new Vector3(-3, 3,0);
        Vector3 pos;
        //Quaternion angle = new Quaternion(0, 0, 0, 0);
        //Debug.Log(cellPrefab);

        cells = new GameObject[hCellNum, vCellNum];
        gotCell = new cell[hCellNum, vCellNum];
        for (int i = 0; i < hCellNum; i++)
        {
            for (int j = 0; j < vCellNum; j++)
            {
                pos = origin + Vector3.right * cellSize*i + Vector3.down * cellSize*j;
                //newCell = Instantiate(cellPrefab, pos, angle) as UnityEngine.Object;
                //Debug.Log(newCell.GetComponent<cell>());
                //GameObject c = GameObject.FindGameObjectWithTag("newCell");
                //Debug.Log(newCell.GetType());
                //cells[i][j] = newCell;
                newCell = GameObject.CreatePrimitive(PrimitiveType.Cube);
                newCell.transform.position = pos;
                newCell.transform.localScale = new Vector3(cellSize, cellSize, cellSize);
                newCell.GetComponent<Renderer>().material = new Material(Shader.Find("Sprites/Default"));
                //Debug.Log(newCell.GetType());
                newCell.AddComponent<cell>();
                //newCell.AddComponent<Collider2D>();   CHECK THIS LATER

                //moving to nontrigger paradigm

                //newCell.GetComponent<BoxCollider>().isTrigger = true;
                newCell.tag = "cell";
                cells[i,j] = newCell;
                
            }
            
        }

        celLox = new Vector2[hCellNum, vCellNum];

        for (int i = 0; i < hCellNum; i++)
        {
            for (int j = 0; j < vCellNum; j++)
            {
                celLox[i, j] = cells[i, j].transform.position;
                gotCell[i, j] = cells[i, j].GetComponent<cell>();
            }
        }


        p1Brush = Instantiate(brushPrefab).GetComponent<brush>();
        //Debug.Log(p1Brush);
        p1Brush.playerNum = 1;
        p1Input = Instantiate(inputHandlerPrefab).GetComponent<inputHandler>();
        var initPos = new Vector3(-3, -3, 0);
        p1bucket = Instantiate(bucketPrefab,initPos,Quaternion.identity).GetComponent<bucket>();
        p1bucket.playerNum = 1;
        p1oom = false;

        frame1 = true;

        
        

    }
	
	// Update is called once per frame
	void Update () {

        if (frame1)
        {
            for (int i = 0; i < hCellNum; i++)
            {
                for (int j = 0; j < vCellNum; j++)
                {
                    p1Brush.cells[i, j] = cells[i, j];
                    p1Brush.celLox[i, j] = celLox[i, j];
                    p1Brush.gotCell[i, j] = cells[i, j].GetComponent<cell>();

                }
            }
            frame1 = false;
        }


        //p1

        p1Input.inputUpdate();

        var v3 = Input.mousePosition;
        v3.z = 00.0f;
        v3 = Camera.main.ScreenToWorldPoint(v3);
        if (!p1Brush.isPainting)
        {
            p1Brush.moveToXY(v3.x, v3.y);
        }
        else
        {
            if (p1Brush.ammo>0 && !p1Brush.onBucket)
            {
                if (p1Brush.drawType == "line")
                {
                    p1Brush.moveToXYcapped(v3.x, v3.y); // put a different move function here
                }
                if (p1Brush.drawType == "dot")
                {
                    //Debug.Log("dot");
                    p1Brush.dotPaint();
                    p1Brush.ammo = 0;
                }
            }
            
        }



        
        if ((!p1Input.mouseDown || ((p1Brush.ammo)<=0)&& !p1Brush.onBucket) && p1Brush.isPainting)
        {
            p1Brush.stopPainting();
            //Debug.Log("stop painting");
        }
        if (p1Input.mouseDown && !p1Brush.isPainting)
        {
            p1Brush.startPainting();
        }

    }

   

}
