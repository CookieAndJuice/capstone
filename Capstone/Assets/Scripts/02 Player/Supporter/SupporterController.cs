using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupporterController : MonoBehaviour
{
    [SerializeField]
    private PlayerController player;
    private Animation animation;

    private void Update()
    {
        MatchPlayerPos();
        SupporterAnimation();
    }

    private void MatchPlayerPos()
    {
        Vector3 newPos = new Vector3((PlayManager.PlayerPos.x + 1), transform.position.y, (PlayManager.PlayerPos.z - 1));    
        transform.position = newPos;   
    }

    private void SupporterAnimation()
    {
        if (player.IsMove)
        {
            animation.Play("Walk");
            animation.Stop("Run");

            if (player.IsRun)
            {
                animation.Play("Run");
                animation.Stop("Walk");
            }
        }
    }


    private void Start()
    {
        animation = GetComponent<Animation>();
    }
}
