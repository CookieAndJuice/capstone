using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerController
{
    [SerializeField]
    private InteractScript interactableObject;  // 상호작용 누르면 상호작용 할 대상
    private bool Interacting { get; set; }

    private void DetectObject()
    {
        if (Interacting) { return; }

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit[] hits = Physics.RaycastAll(ray, 10, ValueDefinition.INTERACT_LAYER);

        InteractScript interact = null;         // 상호작용 대상 탐색
        for (int i = 0; i < hits.Length; i++)   // 근처 대상 확인
        {
            GameObject obj = hits[i].collider.gameObject;
            InteractScript script = obj.GetComponentInParent<InteractScript>()
                ?? obj.GetComponentInChildren<InteractScript>();
            if (script == null || !script.CanInteract) { continue; }               // 스크립트가 없거나 상호작용 불가능인 경우
            interact = script;
        }

        if (interact != null && interact != interactableObject)   // 대상이 바뀐 경우
        {
            if (interactableObject != null)
                interactableObject.DisableInteract();

            interactableObject = interact;
            interactableObject.AbleInteract();
        }
        if (interactableObject != null && !interactableObject.CanInteract)  // 대상이 사라진 경우
        {
            interactableObject.DisableInteract();
            interactableObject = null;
        }
    }

    public void StartInteract()                                                     // 상호작용 시작
    {
        Interacting = true;
    }
    public void StopInteract()                                                      // 상호작용 중단
    {
        interactableObject.StopInteract();
        interactableObject = null;
        Interacting = false;
    }
    public void StopInteract(InteractScript _interact)
    {
        if (interactableObject != _interact) { return; }
    }

    public void PlayerInteract()
    {
        DetectObject();
        if (interactableObject != null && PlayerInput.Interact.triggered)
        {
            StartInteract();
            interactableObject.StartInteract();
        }
    }
}
