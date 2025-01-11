using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeWall : MonoBehaviour
{
    private void Start()
    {
        DestroyOverlapMaze();
    }

    private void DestroyOverlapMaze()
    {
        Vector3 halfExtents = this.GetComponent<BoxCollider>().size;
        Collider[] colliders = Physics.OverlapBox(this.gameObject.transform.position, halfExtents);

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject != this.gameObject && collider.gameObject.tag == "Wall")
            {
                // Debug.Log("Destroyed overlapping maze wall!");
                Destroy(gameObject);
                break;
            }
        }
    }
}
