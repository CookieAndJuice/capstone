using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHittable          // 데미지를 입을 수 있는 오브젝트들이 공통적으로 가져야 하는 인터페이스
{
    public bool IsPlayer { get; }
    public bool IsMonster { get; }
    public void GetHit(float _damage);   // 데미지 받기
}

public interface IInteractable         // 상호작용이 가능한 오브젝트에 필수 부착
{
    public InteractScript InteractManager { get; }
    public void SetInteractScript(InteractScript _interact);
    public bool CanInteract { get; }
    public string InfoTxt { get; }          // 상호작용 정보 텍스트
    public void StartInteract();            // 상호작용 시작
    public void StopInteract();             // 상호작용 중단
}
