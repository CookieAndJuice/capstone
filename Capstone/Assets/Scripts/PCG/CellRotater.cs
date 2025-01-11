using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CellRotater : MonoBehaviour
{
    public bool rotate = true;
    void Update()
    {
        if (rotate) transform.Rotate(new Vector3(0, 0.5f, 0));
    }
}
