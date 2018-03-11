using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class _netGM : Photon.MonoBehaviour
{

    public GameObject cellPrefab;
    public GameObject brushPrefab;
    public GameObject inputHandlerPrefab;
    public GameObject p2testInputPrefab;
    public GameObject bucketPrefab;

    float cellSize;
    int hCellNum;
    int vCellNum;
    public List<GameObject> cellTemp;
    public GameObject[,] cells;
    public Vector2[,] celLox;
    public _netCell[,] gotCell;

    int timer;
    int maxTimer;

    Vector3 origin;
    GameObject newCell;

    public _netBrush p1Brush;
    public _netBrush p2Brush;
    public _netInputHandler p1Input;
    //public inputHandler p2Input;
    public p2testInputHandler p2Input;
    public _netBucket p1bucket;
    public _netBucket p2bucket;
    public bool p1oom;
    public bool newPaint;
    public bool newPaint2; //triggers frame after newpaint
    public bool frame1;
    public Color p1color;
    public Color p2color;

    public List<Vector2> cellQ;
    public List<float> qDist;

    public Vector2[] bfsSort;
    public float[] distsSort;

    public bool gotBoard;

    void Start()
    {
        Debug.Log("network mode");

        gotBoard = false;
        cellTemp = new List<GameObject>(625);
        cellQ = new List<Vector2>(625);
        qDist = new List<float>(625);
        cellSize = .2f;
        hCellNum = 25;
        vCellNum = 25;
        origin = new Vector3(-3, 3, 0);
        newPaint = false;
        newPaint2 = false;
        maxTimer = 60;
        Vector3 pos;
        //Quaternion angle = new Quaternion(0, 0, 0, 0);
        //Debug.Log(cellPrefab);



        GameObject backdrop = GameObject.CreatePrimitive(PrimitiveType.Cube);
        backdrop.transform.position = origin + 2.5f * Vector3.right + 2.5f * Vector3.down;
        backdrop.transform.localScale = new Vector3(5, 5, .01f);
        backdrop.GetComponent<Renderer>().material = new Material(Shader.Find("Sprites/Default"));
        backdrop.GetComponent<Renderer>().material.color = Color.white;

        backdrop.GetComponent<Renderer>().sortingLayerName = "Default";
        backdrop.GetComponent<Renderer>().sortingOrder = 2;

        cells = new GameObject[hCellNum, vCellNum];
        gotCell = new _netCell[hCellNum, vCellNum];
        celLox = new Vector2[hCellNum, vCellNum];
        PhotonNetwork.ConnectUsingSettings("alpha");
        Debug.Log("wait for connection?");



            /*
            for (int i = 0; i < hCellNum; i++)
            {
                for (int j = 0; j < vCellNum; j++)
                {
                    pos = origin + Vector3.right * cellSize * i + Vector3.down * cellSize * j;
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
                    newCell.AddComponent<_netCell>();
                    //newCell.AddComponent<Collider2D>();   CHECK THIS LATER

                    //moving to nontrigger paradigm

                    //newCell.GetComponent<BoxCollider>().isTrigger = true;
                    newCell.tag = "cell";
                    cells[i, j] = newCell;

                }

            }
            */

            /*
            celLox = new Vector2[hCellNum, vCellNum];

            for (int i = 0; i < hCellNum; i++)
            {
                for (int j = 0; j < vCellNum; j++)
                {
                    celLox[i, j] = cells[i, j].transform.position;
                    gotCell[i, j] = cells[i, j].GetComponent<_netCell>();
                }
            }
            */
            //p1

            p1Brush = Instantiate(brushPrefab).GetComponent<_netBrush>();
        //Debug.Log(p1Brush);
        p1Brush.playerNum = 1;
        p1Input = Instantiate(inputHandlerPrefab).GetComponent<_netInputHandler>();
        var initPos = new Vector3(-3, -3, 0);
        p1bucket = Instantiate(bucketPrefab, initPos, Quaternion.identity).GetComponent<_netBucket>();
        p1bucket.playerNum = 1;
        //p1oom = false;
        p1color = Color.blue;
        p1Brush.myColor = Color.blue;

        //p2

        p2Brush = Instantiate(brushPrefab).GetComponent<_netBrush>();
        //Debug.Log(p1Brush);
        p2Brush.playerNum = 2;
        p2Input = Instantiate(p2testInputPrefab).GetComponent<p2testInputHandler>();
        initPos = new Vector3(3, -3, 0);
        p2bucket = Instantiate(bucketPrefab, initPos, Quaternion.identity).GetComponent<_netBucket>();
        p2bucket.playerNum = 2;
        //p2oom = false;
        p2color = Color.magenta;
        p2Brush.myColor = Color.magenta;





        frame1 = true;

        cellQ.Clear();
        qDist.Clear();

        //bfsSort=
    }

    // Update is called once per frame
    void Update()
    {

        if (frame1)
        {
            p1Brush.playerNum = 1;
            p1bucket.playerNum = 1;

            p2Brush.playerNum = 2;
            p2bucket.playerNum = 2;


            //setAllCellBFS();
            //Debug.Log("clearing old values");
            /*
            unmarkAllCells();
            clearAllCellBFS();

            //Debug.Log("calling bfs");

            gotCell[0, 0].marked = true;
            cellQ.Clear();
            qDist.Clear();
            Vector2 v0 = Vector2.zero;

            //setAllCellBFS();
            setAllCellDistsTemp();
            sortCellDists();
            */
            /*
            for (int t=0; t < hCellNum * vCellNum - 1; t++)
            {
                Debug.Log(gotCell[5,5].bfs[t]);
            }
            */

            /*
            cellQ.Add(v0);
            qDist.Add(0);
            cellBFS(0,0, -1);
            */

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
            /*
            for (int i = 0; i < hCellNum; i++)
            {
                for (int j = 0; j < vCellNum; j++)
                {
                    p1Brush.cells[i, j] = cells[i, j];
                    p1Brush.celLox[i, j] = celLox[i, j];
                    p1Brush.gotCell[i, j] = cells[i, j].GetComponent<_netCell>();

                }
            }

            for (int i = 0; i < hCellNum; i++)
            {
                for (int j = 0; j < vCellNum; j++)
                {
                    p2Brush.cells[i, j] = cells[i, j];
                    p2Brush.celLox[i, j] = celLox[i, j];
                    p2Brush.gotCell[i, j] = cells[i, j].GetComponent<_netCell>();

                }
            }
            */
            frame1 = false;
        }

        if (!PhotonNetwork.isMasterClient)
        {
            if (!gotBoard)
            {
                getOnBoard();
            }
            
        }


        //P1

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
                newPaint = true;
                timer = maxTimer;
                p1Brush.ammo = 0;
                p1Brush.used = false;
            }
        }
        else
        {
            if (p1Brush.ammo > 0 && !p1Brush.onBucket)
            {
                if (p1Brush.drawType == "line")
                {
                    p1Brush.moveToXYcapped(v3.x, v3.y); // put a different move function here
                    if (p1Brush.lastFramePainting)
                    {
                        p1Brush.lastFramePainting = false;
                        newPaint = true;
                        timer = maxTimer;
                    }
                }
                if (p1Brush.drawType == "dot")
                {
                    //Debug.Log("dot");
                    p1Brush.dotPaint();
                    p1Brush.ammo = 0;
                    newPaint = true;
                    timer = maxTimer;
                }
            }

        }




        if ((!p1Input.mouseDown || ((p1Brush.ammo) <= 0) && !p1Brush.onBucket) && p1Brush.isPainting)
        {
            p1Brush.stopPainting();
            //Debug.Log("stop painting");
        }
        if (p1Input.mouseDown && !p1Brush.isPainting)
        {
            p1Brush.startPainting();
        }

        // END P1

        //P2

        p2Input.inputUpdate();

        v3 = Input.mousePosition;
        v3.z = 00.0f;
        v3 = Camera.main.ScreenToWorldPoint(v3);

        if (p2Brush.onBucket && p2Brush.isPainting)
        {
            if (p2Brush.ammo < p2Brush.maxAmmo)
            {
                //ammo++;
                p2Brush.ammo = p2Brush.ammo + 6;
                if (p2Brush.ammo == p2Brush.maxAmmo)
                {
                    Debug.Log("full charge");
                }
                if (p2Brush.ammo == p2Brush.paintTypeThreshold)
                {
                    Debug.Log("half charge");
                }
            }

        }


        if (!p2Brush.isPainting)
        {
            p2Brush.moveToXY(v3.x + 2, v3.y); // add 2 to offset players for testing purposes only
            if (p2Brush.used)
            {
                newPaint = true;
                timer = maxTimer;
                p2Brush.ammo = 0;
                p2Brush.used = false;
            }
        }
        else
        {
            if (p2Brush.ammo > 0 && !p2Brush.onBucket)
            {
                if (p2Brush.drawType == "line")
                {
                    p2Brush.moveToXYcapped(v3.x + 2, v3.y); // put a different move function here    // add 2 to offset players for testing purposes only
                    if (p2Brush.lastFramePainting)
                    {
                        p2Brush.lastFramePainting = false;
                        newPaint = true;
                        timer = maxTimer;
                    }
                }
                if (p2Brush.drawType == "dot")
                {
                    //Debug.Log("dot");
                    p2Brush.dotPaint();
                    p2Brush.ammo = 0;
                    newPaint = true;
                    timer = maxTimer;
                }
            }

        }




        if ((!p2Input.mouseDown || ((p2Brush.ammo) <= 0) && !p2Brush.onBucket) && p2Brush.isPainting)
        {
            p2Brush.stopPainting();
            //Debug.Log("stop painting");
        }
        if (p2Input.mouseDown && !p2Brush.isPainting)
        {
            p2Brush.startPainting();
        }

        //END P2


        if (newPaint)
        {
            //Debug.Log("newPaint");
            if (timer > 0)
            {
                timer--;
            }

            else
            {
                newPaint = false;
                {
                    for (int i = 0; i < hCellNum; i++)
                    {
                        for (int j = 0; j < vCellNum; j++)
                        {
                            updateClosestPlayer(i, j);
                            if (gotCell[i, j].closestPlayer == 1)
                            {
                                gotCell[i, j].updateClosestVisual(p1color);
                            }
                            if (gotCell[i, j].closestPlayer == 2)
                            {
                                gotCell[i, j].updateClosestVisual(p2color);
                            }
                        }
                    }
                }
                //Debug.Log(gotCell[0, 0].r.material.color.a);  
            }




        }






    }
    //END UPDATE


    //NETCODE

    private void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }

    void OnJoinedLobby()
    {
        Debug.Log("join lobby");
        PhotonNetwork.JoinRandomRoom();
    }

    void OnPhotonRandomJoinFailed()
    {
        Debug.Log("join room fail");
        PhotonNetwork.CreateRoom(null);
    }

    void OnJoinedRoom()
    {
        Debug.Log("join room");
        if (PhotonNetwork.isMasterClient)
        {
            gotBoard = true;
            SpawnBoard();
        }
        else
        {
            getOnBoard();
        }
        
    }

    void getOnBoard()
    {
        GameObject[] cellGetter;
        cellGetter = GameObject.FindGameObjectsWithTag("cell");
        //cellGetter = GameObject.FindObjectsOfType<_netCell>();
        int ij = 0;
        int q;
        int r;
        float it;
        float jt;
        //Debug.Log(cellGetter.Length);
        /*
        foreach (GameObject c in cellGetter)
        {
            q = 0;
            r = 0;
            while (hCellNum*(q+1)<ij+1)
            {
                q++;
            }
            r = ij - (q * hCellNum);
            //Debug.Log(ij);
            cells[r, q] = c;
            ij++;
        } 
        */
        foreach (GameObject c in cellGetter)
        {
            it = (c.GetPhotonView().transform.position.x +3.0f)*5.0f;
            jt = 24.0f-(c.GetPhotonView().transform.position.y +1.8f)/ cellSize;
            r = (int)(it+.01f);
            q = (int)(jt+.01f);
            //Debug.Log(it + ", " + jt + ", " + r + ", " + q);

            cells[r, q] = c;

        }


        //Debug.Log(cells[0,0]);
        if (cellGetter.Length==hCellNum*vCellNum)
        {
            //gotBoard = true;
        }
        for (int i = 0; i < hCellNum; i++)
        {
            for (int j = 0; j < vCellNum; j++)
            {
                //celLox[i, j] = cells[i, j].transform.position;
                //Debug.Log(i + ", " + j);
                celLox[i, j] = cells[i, j].GetPhotonView().transform.position;
                
                gotCell[i, j] = cells[i, j].GetComponent<_netCell>();

                

            }
        }

        //Debug.Log(gotCell[0, 0].bfs[0]);

        unmarkAllCells();
        clearAllCellBFS();

        //Debug.Log("calling bfs");

        gotCell[0, 0].marked = true;
        cellQ.Clear();
        qDist.Clear();
        Vector2 v0 = Vector2.zero;

        //setAllCellBFS();
        setAllCellDistsTemp();
        sortCellDists();


        //temp
        for (int i = 0; i < hCellNum; i++)
        {
            for (int j = 0; j < vCellNum; j++)
            {
                p1Brush.cells[i, j] = cells[i, j];
                p1Brush.celLox[i, j] = celLox[i, j];
                p1Brush.gotCell[i, j] = cells[i, j].GetComponent<_netCell>();

            }
        }

        for (int i = 0; i < hCellNum; i++)
        {
            for (int j = 0; j < vCellNum; j++)
            {
                p2Brush.cells[i, j] = cells[i, j];
                p2Brush.celLox[i, j] = celLox[i, j];
                p2Brush.gotCell[i, j] = cells[i, j].GetComponent<_netCell>();

            }
        }

        if (cellGetter.Length == hCellNum * vCellNum)
        {
            gotBoard = true;
        }
        Debug.Log("got board");



    }

    void SpawnBoard()
    {
        Debug.Log("spawn board");
        /*
        cells = new GameObject[hCellNum, vCellNum];
        gotCell = new _netCell[hCellNum, vCellNum];
        for (int i = 0; i < hCellNum; i++)
        {
            for (int j = 0; j < vCellNum; j++)
            {
                pos = origin + Vector3.right * cellSize * i + Vector3.down * cellSize * j;
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
                newCell.AddComponent<_netCell>();
                //newCell.AddComponent<Collider2D>();   CHECK THIS LATER

                //moving to nontrigger paradigm

                //newCell.GetComponent<BoxCollider>().isTrigger = true;
                newCell.tag = "cell";
                cells[i, j] = newCell;

            }

        }

        celLox = new Vector2[hCellNum, vCellNum];

        for (int i = 0; i < hCellNum; i++)
        {
            for (int j = 0; j < vCellNum; j++)
            {
                celLox[i, j] = cells[i, j].transform.position;
                gotCell[i, j] = cells[i, j].GetComponent<_netCell>();
            }
        }
        */

        cellTemp.Clear();
        for (int n = 0; n < hCellNum * vCellNum; n++)
        {
            PhotonNetwork.Instantiate("_netCell", Vector2.zero, Quaternion.identity, 0);
        }

        while (cellTemp[399] == null) { Debug.Log("tempcell = null"); }

        Vector3 pos;

        for (int i = 0; i < hCellNum; i++)
        {
            for (int j = 0; j < vCellNum; j++)
            {
                newCell = cellTemp[0];
                cells[i,j] = newCell;
                pos = origin + Vector3.right * cellSize * i + Vector3.down * cellSize * j;
                newCell.transform.position = pos;
                newCell.transform.localScale = new Vector3(cellSize, cellSize, cellSize);
                cellTemp.Remove(cellTemp[0]);
            }
        }

        for (int i = 0; i < hCellNum; i++)
        {
            for (int j = 0; j < vCellNum; j++)
            {
                celLox[i, j] = cells[i, j].transform.position;
                gotCell[i, j] = cells[i, j].GetComponent<_netCell>();
            }
        }

        //Debug.Log(gotCell[0, 0].bfs[0]);

        unmarkAllCells();
        clearAllCellBFS();

        //Debug.Log("calling bfs");

        gotCell[0, 0].marked = true;
        cellQ.Clear();
        qDist.Clear();
        Vector2 v0 = Vector2.zero;

        //setAllCellBFS();
        setAllCellDistsTemp();
        sortCellDists();


        //temp
        for(int i = 0; i < hCellNum; i++)
            {
            for (int j = 0; j < vCellNum; j++)
            {
                p1Brush.cells[i, j] = cells[i, j];
                p1Brush.celLox[i, j] = celLox[i, j];
                p1Brush.gotCell[i, j] = cells[i, j].GetComponent<_netCell>();

            }
        }

        for (int i = 0; i < hCellNum; i++)
        {
            for (int j = 0; j < vCellNum; j++)
            {
                p2Brush.cells[i, j] = cells[i, j];
                p2Brush.celLox[i, j] = celLox[i, j];
                p2Brush.gotCell[i, j] = cells[i, j].GetComponent<_netCell>();

            }
        }

        //gotBoard = true;
        Debug.Log("spawnboard complete");



    }
    //END NETCODE

    public void setAllCellDistsTemp()
    {
        for (int i = 0; i < hCellNum; i++)
        {
            for (int j = 0; j < vCellNum; j++)
            {
                tempCellDist(i, j, 0f);
            }
        }
    }

    public void tempCellDist(float i1, float j1, float dist)
    {
        int k = 0;
        for (int i2 = 0; i2 < hCellNum; i2++)
        {
            for (int j2 = 0; j2 < vCellNum; j2++)
            {
                if (!(i1 == i2 && j1 == j2))
                {
                    gotCell[(int)i1, (int)j1].bfs[k] = new Vector2(i2, j2);
                    int i = Math.Abs((int)i1 - (int)i2);
                    int j = Math.Abs((int)j1 - (int)j2);
                    gotCell[(int)i1, (int)j1].dists[k] = 1.4f * (float)Math.Min(i, j) + (Math.Max(i, j) - Math.Min(i, j));
                }
                else
                {
                    gotCell[(int)i1, (int)j1].bfs[k] = new Vector2(i2, j2);
                    gotCell[(int)i1, (int)j1].dists[k] = 0;
                }
                k++;
            }
        }
    }

    public void sortCellDists()
    {
        for (int i = 0; i < hCellNum; i++)
        {
            for (int j = 0; j < vCellNum; j++)
            {
                qs(0, hCellNum * vCellNum - 1, i, j);
            }
        }
    }

    public void qs(int lo, int hi, int i, int j)
    {
        if (lo < hi)
        {
            int p = part(lo, hi, i, j);
            qs(lo, p - 1, i, j);
            qs(p + 1, hi, i, j);
        }
    }

    public int part(int lo, int hi, int i, int j)
    {
        float pivot = gotCell[i, j].dists[hi];
        int k = lo - 1;
        float tempf;
        Vector2 tempv;
        for (int t = lo; t < hi; t++)
        {
            if (gotCell[i, j].dists[t] < pivot)
            {
                k++;
                tempf = gotCell[i, j].dists[t];
                gotCell[i, j].dists[t] = gotCell[i, j].dists[k];
                gotCell[i, j].dists[k] = tempf;

                tempv = gotCell[i, j].bfs[t];
                gotCell[i, j].bfs[t] = gotCell[i, j].bfs[k];
                gotCell[i, j].bfs[k] = tempv;
            }
        }
        if (gotCell[i, j].dists[hi] < gotCell[i, j].dists[k + 1])
        {
            tempf = gotCell[i, j].dists[hi];
            gotCell[i, j].dists[hi] = gotCell[i, j].dists[k + 1];
            gotCell[i, j].dists[k + 1] = tempf;

            tempv = gotCell[i, j].bfs[hi];
            gotCell[i, j].bfs[hi] = gotCell[i, j].bfs[k + 1];
            gotCell[i, j].bfs[k + 1] = tempv;
        }

        return k + 1;
    }

    public void cellBFSq(float i, float j, float dist)
    {
        //mark and put in p.queue all adjacent cells 
        if (i > 0)
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
            if (!gotCell[(int)i, (int)j - 1].marked && !gotCell[(int)i, (int)j - 1].impassable)
            {
                Vector2 v = new Vector2(i, j - 1);
                cellQ.Add(v);
                qDist.Add(dist + 1);
                gotCell[(int)i, (int)j - 1].marked = true;
            }

        }

        if (i < hCellNum - 1)
        {
            if (!gotCell[(int)i + 1, (int)j].marked && !gotCell[(int)i + 1, (int)j].impassable)
            {
                Vector2 v = new Vector2(i + 1, j);
                cellQ.Add(v);
                qDist.Add(dist + 1);
                gotCell[(int)i + 1, (int)j].marked = true;
            }

        }

        if (j < vCellNum - 1)
        {
            if (!gotCell[(int)i, (int)j + 1].marked && !gotCell[(int)i, (int)j + 1].impassable)
            {
                Vector2 v = new Vector2(i, j + 1);
                cellQ.Add(v);
                qDist.Add(dist + 1);
                gotCell[(int)i, (int)j + 1].marked = true;
            }

        }

        if (i > 0 && j > 0)
        {
            if (!gotCell[(int)i - 1, (int)j - 1].marked && !gotCell[(int)i - 1, (int)j - 1].impassable)
            {
                Vector2 v = new Vector2(i - 1, j - 1);
                cellQ.Add(v);
                qDist.Add(dist + 1.4f);
                gotCell[(int)i - 1, (int)j - 1].marked = true;
            }

        }


        if (i > 0 && j < vCellNum - 1)
        {
            if (!gotCell[(int)i - 1, (int)j + 1].marked && !gotCell[(int)i - 1, (int)j + 1].impassable)
            {
                Vector2 v = new Vector2(i - 1, j + 1);
                cellQ.Add(v);
                qDist.Add(dist + 1.4f);
                gotCell[(int)i - 1, (int)j + 1].marked = true;
            }

        }


        if (i < hCellNum - 1 && j > 0)
        {
            if (!gotCell[(int)i + 1, (int)j - 1].marked && !gotCell[(int)i + 1, (int)j - 1].impassable)
            {
                Vector2 v = new Vector2(i + 1, j - 1);
                cellQ.Add(v);
                qDist.Add(dist + 1.4f);
                gotCell[(int)i + 1, (int)j - 1].marked = true;
            }

        }

        if (i < hCellNum - 1 && j < vCellNum - 1)
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
        while (!(cellQ.Count == 0) && (cellQ.Count < 300))
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
        for (int i = 0; i < hCellNum; i++)
        {
            for (int j = 0; j < vCellNum; j++)
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
                for (int k = 0; k < hCellNum * vCellNum; k++)
                {
                    //Debug.Log(i + j + k);
                    gotCell[i, j].bfs[k] = Vector2.down;
                    gotCell[i, j].dists[k] = -1;
                }

            }
        }
    }


    public void setAllCellBFS()
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


    public void updateClosestPlayer(int i, int j)
    {
        int t = 0;
        int i1;
        int j1;

        while (t < hCellNum * vCellNum && (gotCell[i, j].dists[t] < gotCell[i, j].closestDist))
        {
            i1 = (int)gotCell[i, j].bfs[t].x;
            j1 = (int)gotCell[i, j].bfs[t].y;
            if (gotCell[i1, j1].closestDist == 0)
            {
                gotCell[i, j].closestPlayer = gotCell[i1, j1].painter;

                if (i == 0 && j == 0)
                {
                    Debug.Log("painter:" + gotCell[i1, j1].painter);
                }

                gotCell[i, j].closestDist = gotCell[i, j].dists[t];
            }
            t++;
        }
        while (t < hCellNum * vCellNum && (gotCell[i, j].dists[t] == gotCell[i, j].closestDist))
        {
            i1 = (int)gotCell[i, j].bfs[t].x;
            j1 = (int)gotCell[i, j].bfs[t].y;
            if ((gotCell[i1, j1].closestDist == 0) && (gotCell[i1, j1].painter != gotCell[i, j].closestPlayer) && (gotCell[i1, j1].painter != 0))
            {
                gotCell[i, j].closestPlayer = 0;
            }
            t++;
        }
        /*
        if ((i<1 && j<1) || (i>24 && j>24))
        {
            Debug.Log(i + " " + j + ": " + gotCell[i, j].closestPlayer + " " + gotCell[i, j].closestDist);
        }
        */


    }



}
