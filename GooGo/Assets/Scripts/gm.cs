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
    
    Vector3 origin;
    GameObject newCell;

    public brush p1Brush;
    public brush p2Brush;
    public inputHandler p1Input;
    public inputHandler p2Input;
    public bucket p1bucket;
    public bucket p2bucket;

    void Start () {
        //Debug.Log("yo");
        cellSize = .1f; 
        hCellNum = 50;
        vCellNum = 50;
        origin = new Vector3(-3, 3,0);
        Vector3 pos;
        //Quaternion angle = new Quaternion(0, 0, 0, 0);
        //Debug.Log(cellPrefab);

        cells = new GameObject[hCellNum, vCellNum];
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
                newCell.GetComponent<BoxCollider>().isTrigger = true;
                cells[i,j] = newCell;
                
            }
            
        }


        p1Brush = Instantiate(brushPrefab).GetComponent<brush>();
        //Debug.Log(p1Brush);
        p1Brush.playerNum = 1;
        p1Input = Instantiate(inputHandlerPrefab).GetComponent<inputHandler>();
        var initPos = new Vector3(-3, -3, 0);
        p1bucket = Instantiate(bucketPrefab,initPos,Quaternion.identity).GetComponent<bucket>();
        p1bucket.playerNum = 1;
    }
	
	// Update is called once per frame
	void Update () {


        //p1

        p1Input.inputUpdate();

        var v3 = Input.mousePosition;
        v3.z = 10.0f;
        v3 = Camera.main.ScreenToWorldPoint(v3);
        p1Brush.moveToXY(v3.x, v3.y);

        if (p1Input.mouseDown && !p1Brush.isPainting)
        {
            p1Brush.startPainting();
        }
        if (!p1Input.mouseDown && p1Brush.isPainting)
        {
            p1Brush.stopPainting();
        }

    }

   

}
