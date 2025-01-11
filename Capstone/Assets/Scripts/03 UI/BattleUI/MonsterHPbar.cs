using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterHPbar : MonoBehaviour
{
    private Camera cam;

    private MonsterScript monster;
    private Slider HPBar;

    public void SetHPValue(float _hp)
    {
        HPBar.value = _hp;
    }

    private void LateUpdate()
    {
        Vector3 direction = cam.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(-direction, Vector3.up);
    }

    public void SetComps()
    {
        cam = Camera.main;
        HPBar = GetComponentInChildren<Slider>();

        Transform monsterObj = this.transform.parent;
        monster = monsterObj.GetComponent<MonsterScript>();
        
        HPBar.maxValue = monster.MaxHP;
        HPBar.value = monster.MaxHP;

        this.gameObject.SetActive(false);
    }
}
