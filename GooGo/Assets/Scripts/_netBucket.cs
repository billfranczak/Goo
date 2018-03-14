using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _netBucket : MonoBehaviour
{

    public int playerNum;
    public int ammo;
    public int maxAmmo;

    void Start()
    {
        playerNum = 0;
        maxAmmo = 520;
        GetComponent<Renderer>().sortingLayerName = "LayerName";
        GetComponent<Renderer>().sortingOrder = 0;
        ammo = maxAmmo;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
