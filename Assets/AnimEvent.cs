using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEvent : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerScript;

    // Start is called before the first frame update
    void Start()
    {
        playerScript = GetComponentInParent<PlayerMovement>();
    }

    private void EndRolling()
    {
        playerScript.isRolling = false;
        playerScript.DefaultHeight();
    }

    private void Jump()
    {
        playerScript.Jump();
    }
}
