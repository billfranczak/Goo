using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _netBucket : MonoBehaviour
{

    public int playerNum;
    void Start()
    {
        playerNum = 0;
        GetComponent<Renderer>().sortingLayerName = "LayerName";
        GetComponent<Renderer>().sortingOrder = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
