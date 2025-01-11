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

    public string InfoTxt { get { return "��ȭ"; } }

    public void StartInteract()             // ��ȣ�ۿ� ����
    {
        Debug.Log(this.name + " ��ȣ�ۿ� ����");
        // Door.AddTriggerObject(this);
    }

    public void StopInteract()               // ��ȣ�ۿ� �ߴ�
    {
        Debug.Log(this.name + " ��ȣ�ۿ� ����");
    }
}
