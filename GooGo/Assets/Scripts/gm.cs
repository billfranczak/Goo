using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
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

    public List<Vector2> cellQ;
    public List<float> qDist;



    void Start () {
        cellQ = new List<Vector2>(400);
        qDist = new List<float>(400);
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

        cellQ.Clear();
        qDist.Clear();

        Debug.Log("fuckballs");

    }
	
	// Update is called once per frame
	void Update () {

        if (frame1)
        {

            //setAllCellBFS();
            Debug.Log("clearing old values");
            unmarkAllCells();
            clearAllCellBFS();
            Debug.Log("calling bfs");
            gotCell[0, 0].marked = true;
            cellQ.Clear();
            qDist.Clear();
            Vector2 v0 = Vector2.zero;
            cellQ.Add(v0);
            qDist.Add(0);
            cellBFS(0,0, -1);
            /*
            Debug.Log(gotCell[0,0].bfs[0]);
            Debug.Log(gotCell[0, 0].bfs[1]);
            Debug.Log(gotCell[0, 0].bfs[2]);
            Debug.Log(gotCell[0, 0].bfs[3]);
            Debug.Log(gotCell[0, 0].bfs[4]);
            Debug.Log(gotCell[0, 0].bfs[5]);
            Debug.Log(gotCell[0, 0].bfs[6]);
            Debug.Log(gotCell[0, 0].bfs[7]);
            Debug.Log(gotCell[0, 0].bfs[8]);
            Debug.Log(gotCell[0, 0].bfs[9]);
            */

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
        
        if (p1Brush.onBucket && p1Brush.isPainting)
        {
            if (p1Brush.ammo < p1Brush.maxAmmo)
            {
                //ammo++;
                p1Brush.ammo = p1Brush.ammo + 6;
                if (p1Brush.ammo == p1Brush.maxAmmo)
                {
                    Debug.Log("full charge");
                }
                if (p1Brush.ammo == p1Brush.paintTypeThreshold)
                {
                    Debug.Log("half charge");
                }
            }

        }
        

        if (!p1Brush.isPainting)
        {
            p1Brush.moveToXY(v3.x, v3.y);
            if (p1Brush.used)
            {
                p1Brush.ammo = 0;
                p1Brush.used = false;
            }
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


    public void cellBFSq(float i, float j, float dist)
    {
        //mark and put in p.queue all adjacent cells 
        if (i>0)
        {
            if (!gotCell[(int)i - 1, (int)j].marked && !gotCell[(int)i - 1, (int)j].impassable)
            {
                Vector2 v = new Vector2(i - 1, j);
                cellQ.Add(v);
                qDist.Add(dist + 1);
                gotCell[(int)i - 1, (int)j].marked = true;
            }
            
        }

        if (j > 0)
        {
            if (!gotCell[(int)i , (int)j-1].marked && !gotCell[(int)i , (int)j - 1].impassable)
            {
                Vector2 v = new Vector2(i, j - 1);
                cellQ.Add(v);
                qDist.Add(dist + 1);
                gotCell[(int)i, (int)j - 1].marked = true;
            }
            
        }

        if (i < hCellNum-1)
        {
            if (!gotCell[(int)i + 1, (int)j].marked && !gotCell[(int)i + 1, (int)j ].impassable)
            {
                Vector2 v = new Vector2(i + 1, j);
                cellQ.Add(v);
                qDist.Add(dist + 1);
                gotCell[(int)i + 1, (int)j].marked = true;
            }
            
        }

        if (j < vCellNum-1)
        {
            if (!gotCell[(int)i, (int)j+1].marked && !gotCell[(int)i, (int)j + 1].impassable)
            {
                Vector2 v = new Vector2(i, j + 1);
                cellQ.Add(v);
                qDist.Add(dist + 1);
                gotCell[(int)i, (int)j + 1].marked = true;
            }
           
        }

        if (i > 0 && j>0)
        {
            if (!gotCell[(int)i - 1, (int)j-1].marked && !gotCell[(int)i - 1, (int)j - 1].impassable)
            {
                Vector2 v = new Vector2(i - 1, j-1);
                cellQ.Add(v);
                qDist.Add(dist + 1.4f);
                gotCell[(int)i - 1, (int)j-1].marked = true;
            }

        }


        if (i > 0 && j < vCellNum-1)
        {
            if (!gotCell[(int)i - 1, (int)j + 1].marked && !gotCell[(int)i - 1, (int)j + 1].impassable)
            {
                Vector2 v = new Vector2(i - 1, j + 1);
                cellQ.Add(v);
                qDist.Add(dist + 1.4f);
                gotCell[(int)i - 1, (int)j + 1].marked = true;
            }

        }


        if (i < hCellNum-1 && j > 0)
        {
            if (!gotCell[(int)i + 1, (int)j - 1].marked && !gotCell[(int)i + 1, (int)j - 1].impassable)
            {
                Vector2 v = new Vector2(i + 1, j - 1);
                cellQ.Add(v);
                qDist.Add(dist + 1.4f);
                gotCell[(int)i + 1, (int)j - 1].marked = true;
            }

        }

        if (i < hCellNum-1 && j < vCellNum-1)
        {
            if (!gotCell[(int)i + 1, (int)j + 1].marked && !gotCell[(int)i + 1, (int)j + 1].impassable)
            {
                Vector2 v = new Vector2(i + 1, j + 1);
                cellQ.Add(v);
                qDist.Add(dist + 1.4f);
                gotCell[(int)i + 1, (int)j + 1].marked = true;
            }

        }

        
    }

    public void cellBFS(int i0, int j0, int itr)
    {
        /*
        if (!(cellQ.Count == 0) && (cellQ.Count<180))
        {
            //Debug.Log(itr+": "+ cellQ.Count);
            itr++;
            //find index of min dist, put in the linearized bfs for cell i0,j0, and enqueue the adjacent cells
            float m = qDist.Min();
            int index = qDist.FindIndex(0,qDist.Count, x => x==m);
            //Debug.Log("index: " + index);
            gotCell[i0, j0].bfs[itr] = (cellQ[index]);
            gotCell[i0, j0].dists[itr] = m;
            cellBFSq(cellQ[index].x, cellQ[index].y, m);
            cellQ.RemoveAt(index);
            qDist.RemoveAt(index);
            cellBFS(i0, j0, itr);
        }
        else
        {
            Debug.Log("exited at "+cellQ.Count+" : iteration depth : "+itr);

        }
        */
        while (!(cellQ.Count == 0) && (cellQ.Count < 200))
        {
            itr++;
            //find index of min dist, put in the linearized bfs for cell i0,j0, and enqueue the adjacent cells
            float m = qDist.Min();
            int index = qDist.FindIndex(0, qDist.Count, x => x == m);
            //Debug.Log("index: " + index);
            gotCell[i0, j0].bfs[itr] = (cellQ[index]);
            gotCell[i0, j0].dists[itr] = m;
            cellBFSq(cellQ[index].x, cellQ[index].y, m);
            cellQ.RemoveAt(index);
            qDist.RemoveAt(index);
            //cellBFS(i0, j0, itr);
        }
        Debug.Log("exited at " + cellQ.Count + " : iteration depth : " + itr);
    }

    public void unmarkAllCells()
    {
        for (int i=0; i<hCellNum; i++)
        {
            for (int j=0; j<vCellNum; j++)
            {
                gotCell[i, j].marked = false;
            }
        }
    }

    public void clearAllCellBFS()
    {
        for (int i = 0; i < hCellNum; i++)
        {
            for (int j = 0; j < vCellNum; j++)
            {
                for (int k=0; k<hCellNum*vCellNum;k++)
                {
                    gotCell[i, j].bfs[k] = Vector2.down;
                    gotCell[i, j].dists[k] = -1;
                }
                
            }
        }
    }


    public void setAllCellBFS ()
    {
        
        clearAllCellBFS();
        for (int i = 0; i < hCellNum; i++)
        {
            for (int j = 0; j < vCellNum; j++)
            {
                unmarkAllCells();
                gotCell[i, j].marked = true;
                Vector2 v0 = new Vector2(i, j);
                cellQ.Add(v0);
                qDist.Add(0);
                cellBFS(i, j, -1);
            }
        }
    }
    
}
