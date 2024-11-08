using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Function
{
    restartLevel,
    changeLevel
}

public class LevelController : Utilities
{
    [SerializeField] private Function function;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Transform liftDoor;
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private bool tes;
    private GameObject character;
    private int sceneIndex;
    private float liftDoorOpenHeight;
    private bool isClosingDoor;
    // Start is called before the first frame update
    void Start()
    {
        if (function == Function.changeLevel)
        {
            liftDoorOpenHeight = liftDoor.localPosition.y;
            gameManager.OpenDoor(liftDoorOpenHeight);
        }
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        character = GameObject.FindGameObjectWithTag("Character");
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    // Update is called once per frame
    void Update()
    {
        if (isClosingDoor) CloseDoor();
        if (tes) gameManager.LoadScene(sceneIndex);
        if (character == null) character = GameObject.FindGameObjectWithTag("Character"); ;
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (function)
        {
            case Function.restartLevel:
                Destroy(character);
                gameManager.LoadScene(sceneIndex);
                break;
            case Function.changeLevel:
                isClosingDoor = true;
                break;
        }
    }

    private void CloseDoor()
    {
        liftDoor.localPosition = new Vector3(0, SmoothTransitionFloat(liftDoor.localPosition.y, 0f), 0);
        DontDestroyOnLoad(character);
        DontDestroyOnLoad(mainCamera);
        if (liftDoor.localPosition.y == 0) gameManager.LoadScene(sceneIndex + 1);
    }
}
