using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _netDataShell : Photon.MonoBehaviour {

    public bool p1ready;
    public bool p2ready;
    public bool netNewPaint1;
    public bool netNewPaint2;

    public _netCell[,] gotCell;
    public float cellSize;

    public Vector3 breadCrumb;
    public Vector3 lookAhead;

    public int hCellNum;
    public int vCellNum;


    // Use this for initialization
    void Start () {

        this.photonView.RPC("ndsTagger", PhotonTargets.AllBuffered, gameObject.tag);
        p1ready = false;
        p2ready = false;
        netNewPaint1 = false;
        netNewPaint2 = false;
    }

    public void OnPhotonInstantiate()
    {
        hCellNum = 25;
        vCellNum = 25;
        cellSize = .2f;

        gotCell = new _netCell[hCellNum, vCellNum]; //UPDATE HCELLNUM / VCELLNUM
        _netGM ngm = FindObjectOfType<_netGM>();
        ngm.nds = this.gameObject.GetComponent<_netDataShell>();
        //FindObjectOfType<_netGM>().nds=this.gameObject.GetComponent<_netDataShell>();
        for (int i = 0; i < hCellNum; i++)
        {
            for (int j = 0; j < vCellNum; j++)
            {
                gotCell[i, j] = ngm.gotCell[i, j];
            }
        }

    }

    // Update is called once per frame
    void Update () {
		
	}

    public void p1r()
    {
        this.photonView.RPC("p1rRPC", PhotonTargets.AllBuffered);
    }

    public void p2r()
    {
        this.photonView.RPC("p2rRPC", PhotonTargets.AllBuffered);
    }

    public void p1newPaint()
    {
        this.photonView.RPC("p1newPaintRPC", PhotonTargets.AllBuffered);
    }

    public void p2newPaint()
    {
        this.photonView.RPC("p2newPaintRPC", PhotonTargets.AllBuffered);
    }

    [PunRPC]
    void p1rRPC()
    {
        this.p1ready = true;
    }

    [PunRPC]
    void p2rRPC()
    {
        this.p2ready = true;
    }
    
    [PunRPC]
    void ndsTagger(string text)
    {
        this.tag = text;
        //Debug.Log("here they come");
    }

    [PunRPC]
    void p1newPaintRPC()
    {
        this.netNewPaint1 = true;
    }

    [PunRPC]
    void p2newPaintRPC()
    {
        this.netNewPaint2 = true;
    }


    //painting

    public void Blob(int i0, int j0, float r, Color c, int pNum)
    {
        int t = 0;
        int i;
        int j;
        while (gotCell[i0, j0].dists[t] < r * (1 / cellSize))
        {
            //careful about int casting
            i = (int)gotCell[i0, j0].bfs[t].x;
            j = (int)gotCell[i0, j0].bfs[t].y;
            gotCell[i, j].colorNRPC(pNum, c);
            t++;
        }
    }

    public void bcLerpPaint(int pnum, Color c, Vector3 la, Vector3 bc, float dotsize)
    {

    }

    [PunRPC]
    public void dotPaintRPC(float r, float g, float b, float a, int pnum, int iOc, int jOc, float dotsize)
    {
        Color c = new Color(r, g, b, a);

        Blob(iOc, jOc, dotsize, c, pnum);
    }

    [PunRPC]
    public void bcLerpRPC(float r, float g, float b, float a, int pnum, Vector3 la,Vector3 bc, float linesize)
    {
        Color c = new Color(r, g, b, a);

        bcLerpPaint(pnum, c, la, bc, linesize);
    }


    

}
