using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _netDataShell : Photon.MonoBehaviour {

    public bool p1ready;
    public bool p2ready;
    public bool netNewPaint1;
    public bool netNewPaint2;

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
        FindObjectOfType<_netGM>().nds=this.gameObject.GetComponent<_netDataShell>();
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


}
