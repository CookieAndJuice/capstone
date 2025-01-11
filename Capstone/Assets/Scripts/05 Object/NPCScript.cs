using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCScript : MonoBehaviour, IInteractable
{
    public InteractScript InteractManager { get; private set; }

    public bool CanInteract { get { return true; } }

    public string InfoTxt { get { return "��ȭ"; } }

    public void SetInteractScript(InteractScript _script)
    {
        InteractManager = _script;
    }

    public virtual void StartInteract()
    {
        GameManager.SetControlMode(EControlMode.UI_CONTROL);
        // if(this.CompareTag(ValueDefinition.NPC_TAG))
        PlayManager.OpenDialogue(this);
        Debug.Log(this.name + " ��ȣ�ۿ� ����");
    }

    public virtual void StopInteract()
    {
        GameManager.SetControlMode(EControlMode.FIRST_PERSON);
        PlayManager.CloseDialogue();
        Debug.Log(this.name + " ��ȣ�ۿ� ����");
    }

    private void Start()
    {
        
    }
}
