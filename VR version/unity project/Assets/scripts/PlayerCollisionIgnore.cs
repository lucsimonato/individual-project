using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//class used to setup unwanted collisions
public class PlayerCollisionIgnore : MonoBehaviour
{
    [SerializeField] private GameObject collidersToIgnore;
    [SerializeField] private GameObject m_player;
    // Start is called before the first frame update
    void Start()
    {
        SetPlayerIgnoreCollision(true);
    }

    //makes colliders belonging to the player object ignore the collisions with the specified object
    private void SetPlayerIgnoreCollision(bool ignore)
	{
        Collider[] playerColliders = m_player.GetComponentsInChildren<Collider>();
        foreach (Collider pc in playerColliders)
        {
            Collider[] colliders = collidersToIgnore.GetComponentsInChildren<Collider>();
            foreach (Collider c in colliders)
            {
                Physics.IgnoreCollision(c, pc, ignore);
            }
		}
	}
}
