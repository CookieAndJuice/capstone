using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveButton : MonoBehaviour, IInteractable
{
    [SerializeField] private DoorScript Door;
    public InteractScript InteractManager { get; private set;  }
    public int buttonNumber { get; }

    public void SetInteractScript(InteractScript _script)
    {
        InteractManager = _script;
    }

    public bool CanInteract { get { return true; } }

    public string InfoTxt { get { return "대화"; } }

    public void StartInteract()             // 상호작용 시작
    {
        Debug.Log(this.name + " 상호작용 시작");
        // Door.AddTriggerObject(this);
    }

    public void StopInteract()               // 상호작용 중단
    {
        Debug.Log(this.name + " 상호작용 종료");
    }
}
