using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Utilities
{
    [SerializeField] private PlayerMovement playerScript;
    [SerializeField] private GameObject charcterPrefabs;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform liftDoor;
    private float liftDoorCloseHeight = 0;
    private float liftDoorOpenHeight;
    private bool openDoor;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        if (GameObject.FindGameObjectWithTag("Character") == null)
        {
            Instantiate(charcterPrefabs, spawnPoint.position, spawnPoint.rotation);
        }
        playerScript = GameObject.FindGameObjectWithTag("Character").GetComponent<PlayerMovement>();
        playerScript.FindSlider();
    }

    private void Update()
    {
        if (openDoor) OpeningDoor();
    }

    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void OpenDoor(float liftDoorOpenHeight)
    {
        this.liftDoorOpenHeight = liftDoorOpenHeight;
        openDoor = true;
    }

    private void OpeningDoor()
    {
        liftDoor.transform.localPosition = new Vector3(0, SmoothTransitionFloat(liftDoor.transform.localPosition.y, liftDoorOpenHeight), 0);
        if (liftDoor.transform.localPosition.y == liftDoorOpenHeight) openDoor = false;
    }
}
