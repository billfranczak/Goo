using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;

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
    public Color p1color;
    public Color p2color;

    public List<Vector2> cellQ;
    public List<float> qDist;

    public Vector2[] bfsSort;
    public float[] distsSort;

    public bool gotBoard;
    public bool ready;

    public int cdTimer;
    public bool cd;
    public int gameClock;
    public bool addingAmmo;

    public _netDataShell nds;

    private Slider chargeSlider;
    private Slider ammoSlider;
    private Slider scoreSlider;
    private Canvas sliderCanvas;
    private RectTransform csrt;
    private RectTransform asrt;
    private RectTransform ssrt;
    private RectTransform scrt;
    public Image csFill;
    public Vector2 worldToCanvas;
    public Vector3 w2c;
    //public GameObject asFill;
    //public GameObject ssFill;
    public int score;
    public bool started;
    public bool displayScore;

    void Start()
    {
        //Debug.Log(Screen.width);
        //Debug.Log(Camera.main.fieldOfView);
        //Debug.Log(Camera.main);
        displayScore = false;
        started = false;
        sliderCanvas = GameObject.FindGameObjectWithTag("sliderCanvas").GetComponent<Canvas>();
        chargeSlider = GameObject.FindGameObjectWithTag("chargeSlider").GetComponent<Slider>();
        scoreSlider = GameObject.FindGameObjectWithTag("scoreSlider").GetComponent<Slider>();
        scrt = sliderCanvas.GetComponent<RectTransform>();
        csrt = chargeSlider.GetComponent<RectTransform>();
        
        //Debug.Log(GameObject.FindGameObjectWithTag("chargeSlider"));
        csFill = (chargeSlider as UnityEngine.UI.Slider).GetComponentsInChildren<UnityEngine.UI.Image>().FirstOrDefault(t => t.name == "csFill");

        ammoSlider = GameObject.FindGameObjectWithTag("ammoSlider").GetComponent<Slider>();

        

        Debug.Log("network mode");
        gotBoard = false;
        ready = false;
        cd = false;
        cdTimer = 0;

        cellTemp = new List<GameObject>(625);
        cellQ = new List<Vector2>(625);
        qDist = new List<float>(625);
        cellSize = .2f;
        hCellNum = 25;
        vCellNum = 25;
        origin = new Vector3(-3, 3, 0);
        newPaint = false;
        newPaint2 = false;
        addingAmmo = false;
        maxTimer = 60;
        //Vector3 pos;
        //Quaternion angle = new Quaternion(0, 0, 0, 0);
        //Debug.Log(cellPrefab);

        worldToCanvasConversion(scrt, Camera.main, origin + Vector3.up * cellSize * (vCellNum / 8) +Vector3.right*cellSize*(hCellNum/2)); //+ Vector3.up * .5f)
                                                                                                     
        scoreSlider.GetComponent<RectTransform>().anchoredPosition = w2c;


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
        //Debug.Log("wait for connection?");


        
        //p1

        p1Brush = Instantiate(brushPrefab).GetComponent<_netBrush>();
        //Debug.Log(p1Brush);
        p1Brush.playerNum = 1;
        p1Input = Instantiate(inputHandlerPrefab).GetComponent<_netInputHandler>();
        var initPos = new Vector3(-3, -3, 0);
        p1bucket = Instantiate(bucketPrefab, initPos, Quaternion.identity).GetComponent<_netBucket>();
        p1bucket.playerNum = 1;

        worldToCanvasConversion(scrt, Camera.main, initPos+Vector3.down*.6f); //+ Vector3.up * .5f)
        ammoSlider.GetComponent<RectTransform>().anchoredPosition = w2c;

        //p1oom = false;
        //p1color = Color.blue;
        //p1Brush.myColor = Color.blue;

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


        cellQ.Clear();
        qDist.Clear();

        //bfsSort=
    }

    // Update is called once per frame
    void Update()
    {
        if (!gotBoard)
        {
            StartCoroutine(getOnBoardDelay(3));
        }
        else
        {
            if (PhotonNetwork.isMasterClient)
            {
                if (ready)
                {
                    p1Update();
                    p2Update();
                    paintUpdate();
                }
                else
                {
                    if (nds.p1ready)
                    {
                        //waiting for p2
                        if (nds.p2ready)
                        {
                            if (!started)
                            {
                                started = true;
                                StartCoroutine(gameCountdown());
                            }                           
                        }
                        if (Input.GetKeyDown("space"))
                        {
                            if (!started)
                            {
                                started = true;
                                StartCoroutine(gameCountdown());
                            }
                            //TESTING PURPOSES ONLY
                        }
                    }
                }
            }
            else
            {
                if (ready)
                {
                    p1Update();
                    p2Update();
                    paintUpdate();
                }
                else
                {
                    if (nds.p2ready)
                    {
                        //waiting for p1
                        if (nds.p1ready)
                        {
                            if (!started)
                            {
                                started = true;
                                StartCoroutine(gameCountdown());
                            }
                        }
                    }
                }
            }
        }



        



    }
    //END UPDATE


    //GUI
    private void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());

        if(gotBoard)
        {
            if(PhotonNetwork.isMasterClient && !nds.p1ready)
            {
                if (GUI.Button(new Rect(300, 20, 100, 50), "Ready!"))
                {
                    nds.p1r();
                }
            }
            if (!PhotonNetwork.isMasterClient && !nds.p2ready)
            {
                if (GUI.Button(new Rect(300, 20, 100, 50), "Ready!"))
                {
                    nds.p2r();
                }
            }
            if (cd)
            {
                GUI.Label(new Rect(Screen.width/2, 20, 100, 20), cdTimer.ToString());
            }
            if (ready)
            {
                ammoSlider.value = (float) p1bucket.ammo / p1bucket.maxAmmo;
                chargeSlider.value = (float) p1Brush.ammo / p1Brush.maxAmmo;
                if ((float)p1Brush.ammo / p1Brush.maxAmmo > (float) p1Brush.paintTypeThreshold/p1Brush.maxAmmo)
                {
                    if((float)chargeSlider.value>.99f)
                    {
                        csFill.color = Color.cyan;
                    }
                    else
                    {
                        csFill.color = Color.green;
                    }
                }
                else
                {
                    csFill.color = Color.white;
                }
                //if (chargeSlider.value>0) { Debug.Log(chargeSlider.value); }
                GUI.Label(new Rect(Screen.width / 2, 20, 100, 20), ((int)gameClock).ToString());


                worldToCanvasConversion(scrt,Camera.main, p1Brush.transform.position - .5f * Vector3.right);
                csrt.anchoredPosition = w2c;
                //Debug.Log((float )csrt.anchoredPosition.x / p1Brush.transform.position.x); 

                updateScore();
                scoreSlider.value = (float)score / (hCellNum * vCellNum);
            }
            if(displayScore)
            {
                if (score > hCellNum * vCellNum / 2)
                {
                    GUI.Label(new Rect(Screen.width / 2, 20, 100, 20), "Player 1 Wins!");
                }
                else
                {
                    GUI.Label(new Rect(Screen.width / 2, 20, 100, 20), "Player 2 Wins!");
                }
            }
        }
    }
    
    //NETCODE

    void OnJoinedLobby()
    {
        //Debug.Log("join lobby");
        PhotonNetwork.JoinRandomRoom();
    }

    void OnPhotonRandomJoinFailed()
    {
        //Debug.Log("join room fail");
        PhotonNetwork.CreateRoom(null);
    }

    void OnJoinedRoom()
    {
        Debug.Log("joined room");
        if (PhotonNetwork.isMasterClient)
        {
            gotBoard = true;
            SpawnBoard();
        }
        else
        {
            //getOnBoard();
        }
        
    }

    void getOnBoard()
    {
        GameObject[] cellGetter;
        cellGetter = GameObject.FindGameObjectsWithTag("cell");
        //cellGetter = GameObject.FindObjectsOfType<_netCell>();
        //int ij = 0;
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
            //GENERALIZE!!!!!!!!!!
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

        nds = GameObject.FindGameObjectWithTag("nds").GetComponent<_netDataShell>();


        if (cellGetter.Length == hCellNum * vCellNum && (nds != null))
        {
            gotBoard = true;
        }
        //Debug.Log("got board");

        p1Brush.nds = nds;
        p2Brush.nds = nds;

    }

    void SpawnBoard()
    {
        //Debug.Log("spawn board");


        cellTemp.Clear();

        for (int n = 0; n < hCellNum * vCellNum; n++)
        {
            PhotonNetwork.Instantiate("_netCell", Vector2.zero, Quaternion.identity, 0);
        }

        //while (cellTemp[399] == null) { Debug.Log("tempcell = null"); }

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
        //Debug.Log("spawnboard complete");
        Debug.Log("You Are Player 1.");
        p1Brush.playerNum = 1;
        p1bucket.playerNum = 1;

        p2Brush.playerNum = 2;
        p2bucket.playerNum = 2;

        p1color = Color.blue;
        p1Brush.myColor = Color.blue;

        p2color = Color.magenta;
        p2Brush.myColor = Color.magenta;

        PhotonNetwork.Instantiate("_NetDataShell", Vector2.zero, Quaternion.identity, 0);

        p1Brush.nds = nds;
        p2Brush.nds = nds;
        /*
        for (int i = 0; i < hCellNum; i++)
        {
            for (int j = 0; j < vCellNum; j++)
            {
                nds.gotCell[i,j]=gotCell[i, j];
            }
        }
        */ //moved to gotboard
    }

    public void netNewPaint()
    {
        if (PhotonNetwork.isMasterClient)
        {
            nds.p1newPaint();
        }
        else
        {
            nds.p2newPaint();
        }
    }
    //END NETCODE



    public void p1Update()
    {
        //P1

        p1Input.inputUpdate();


        var v3 = Input.mousePosition;
        v3.z = 00.0f;
        v3 = Camera.main.ScreenToWorldPoint(v3);

        if (!addingAmmo)
        {
            if (p1Brush.onBucket && p1Brush.isPainting && p1bucket.ammo > 0)
            {
                if (p1Brush.ammo < p1Brush.maxAmmo)
                {
                    //ammo++;
                    //p1Brush.ammo = p1Brush.ammo + 6;
                    if (p1Brush.ammo < p1Brush.paintTypeThreshold)
                    {
                        p1bucket.ammo--;
                    }
                    addingAmmo = true;
                    StartCoroutine(ammoAdder());                   
                }
            }
        }

        if (p1Brush.onBucket && p1Brush.isPainting && p1bucket.ammo >0)
        {
            if (p1Brush.ammo < p1Brush.maxAmmo)
            {
                //ammo++;
                //p1Brush.ammo = p1Brush.ammo + 6;
                if(!addingAmmo)
                {
                    if (p1Brush.ammo < p1Brush.paintTypeThreshold)
                    {
                        p1bucket.ammo--;
                    }
                    addingAmmo = true;
                    StartCoroutine(ammoAdder());
                }
            }

        }


        if (!p1Brush.isPainting)
        {
            p1Brush.moveToXY(v3.x, v3.y);
            if (p1Brush.used)
            {
                newPaint = true;
                netNewPaint();
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
                        netNewPaint();
                        newPaint = true;
                        timer = maxTimer;
                    }
                }
                if (p1Brush.drawType == "dot")
                {
                    //Debug.Log("dot");
                    p1Brush.dotPaint();
                    //p1Brush.dotpaintRPCcaller();
                    p1Brush.ammo = 0;
                    newPaint = true;
                    netNewPaint();
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
    }

    public void p2Update() //for testing
    {
        //P2

        p2Input.inputUpdate();

        var v3 = Input.mousePosition;
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
                    //p2Brush.dotpaintRPCcaller();
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
    }

    public void paintUpdate()
    {
        if(PhotonNetwork.isMasterClient)
        {
            if(nds.netNewPaint2)
            {
                nds.netNewPaint2 = false;
                newPaint = true;
                timer = maxTimer;
            }
        }
        else
        {
            if (nds.netNewPaint1)
            {
                nds.netNewPaint1 = false;
                newPaint = true;
                timer = maxTimer;
            }
        }


        if (newPaint)
        {
            //Debug.Log("newPaint");
            if (timer > 0)
            {
                timer--;
            }

            else
            {
                //Debug.Log("Updating closest painter");
                newPaint = false;
                {
                    for (int i = 0; i < hCellNum; i++)
                    {
                        for (int j = 0; j < vCellNum; j++)
                        {
                            if (PhotonNetwork.isMasterClient)
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
                            else //flips colors for non-master client
                            {
                                updateClosestPlayer(i, j);
                                if (gotCell[i, j].closestPlayer == 1)
                                {
                                    gotCell[i, j].updateClosestVisual(p2color);
                                }
                                if (gotCell[i, j].closestPlayer == 2)
                                {
                                    gotCell[i, j].updateClosestVisual(p1color);
                                }
                            }
                        }
                    }
                }
                //Debug.Log(gotCell[0, 0].r.material.color.a);  
            }
        }
    }

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
                /*
                if (i == 0 && j == 0)
                {
                    Debug.Log("painter:" + gotCell[i1, j1].painter);
                }
                */
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

    public void updateScore ()
    {
        score = 0;
        for (int i = 0; i < hCellNum; i++)
        {
            for (int j = 0; j < vCellNum; j++)
            {
                if(gotCell[i,j].closestPlayer==1 || gotCell[i, j].painter == 1)
                {
                    score++;
                }
            }

        }
    }

    public void worldToCanvasConversion(RectTransform canvas, Camera camera, Vector3 position)
    {
        worldToCanvas = camera.WorldToViewportPoint(position);
        worldToCanvas.x *= canvas.sizeDelta.x;
        worldToCanvas.y *= canvas.sizeDelta.y;
        worldToCanvas.x -= canvas.sizeDelta.x * canvas.pivot.x;
        worldToCanvas.y -= canvas.sizeDelta.y * canvas.pivot.y;
        w2c = new Vector3(worldToCanvas.x, worldToCanvas.y, 0);
    }

    IEnumerator getOnBoardDelay(int i)
    {
        yield return new WaitForSeconds(i);

        if (!gotBoard && !PhotonNetwork.isMasterClient)
        {

            getOnBoard();
            Debug.Log("You Are Player 2.");
            p1Brush.playerNum = 2;
            p1bucket.playerNum = 2;

            p2Brush.playerNum = 1;
            p2bucket.playerNum = 1;

            p1color = Color.magenta;
            p1Brush.myColor = Color.magenta;

            p2color = Color.blue;
            p2Brush.myColor = Color.blue;

            //Debug.Log("only executing getboard once");
        }
    }

    IEnumerator gameCountdown()
    {
        cd = true;
        cdTimer = 3;
        yield return new WaitForSeconds(1);
        cdTimer = 2;
        yield return new WaitForSeconds(1);
        cdTimer = 1;
        yield return new WaitForSeconds(1);
        cd = false;
        ready = true;
        gameClock = 60;
        StartCoroutine(gameClockCounter());
    }

    IEnumerator gameClockCounter()
    {
        //Debug.Log("calling game clock");
        while (gameClock > 0)
        {
            gameClock--;
            yield return new WaitForSeconds(1);
        }
        ready = false;
        displayScore = true;
    }

    IEnumerator ammoAdder()
    {
        yield return new WaitForSeconds(.05f);
        p1Brush.ammo = p1Brush.ammo + 3;        
        if (p1Brush.ammo == p1Brush.maxAmmo)
        {
            p1bucket.ammo += 30;
            addingAmmo = false;
        }
        else
        {
            if (p1Brush.onBucket && p1Brush.isPainting && p1bucket.ammo > 0)
            {
                if (p1Brush.ammo < p1Brush.paintTypeThreshold)
                {
                    p1bucket.ammo--;
                }
                StartCoroutine(ammoAdder());
            }
            else
            {
                addingAmmo = false;
            }
        }

        
    }

    

}
