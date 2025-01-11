using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressureSwitch : MonoBehaviour
{
    public OrderDoorScript orderDoorScript;
    public int switchNumber { get; set; }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        Debug.Log("¹â´Â Áß!");
        orderDoorScript.AddTriggerObject(this);
    }

    //private void OnCollisionExit(Collision collision)
    //{
    //    Debug.Log(collision.gameObject.name);
    //    Debug.Log("¶¼´Â Áß!");
    //    Door.RemoveTriggerObject(this);
    //}
}
