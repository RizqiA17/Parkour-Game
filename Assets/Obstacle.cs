using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;

public enum ObstacleVariant
{
    Move,
    Blinking,
    Fall
}

[Serializable]
public struct Move
{
    [SerializeField] private GameObject[] movePoint;
    [SerializeField] private float speed;
}
[Serializable]
public struct Blinking
{
    [SerializeField] private float appearTimer;
    public float AppearTimer { get { return appearTimer; } }
    [SerializeField] private float disappearTimer;
    public float DisappearTimer { get { return disappearTimer; } }
    [SerializeField] private float blinkingTimes;
    public float BlinkingTimes { get { return blinkingTimes; } }
    [SerializeField] private bool appearFirst;
    public bool AppearFirst { get { return appearFirst; } }
    [SerializeField] private float blinkinDelay;
    public float BlinkingDelay { get { return blinkinDelay; } }
    public bool isAppear;
    public bool isBlinking;
    [HideInInspector] public int blinkingTotal;
}
[Serializable]
public struct Fall
{
    [SerializeField] private float fallTimer;
}
public class Obstacle : MonoBehaviour
{
    [SerializeField] private ObstacleVariant obstacleVariant;
    private IEnumerator IE_Countdown = null;
    private MeshRenderer mesh;
    private BoxCollider coll;

    public Move move = new Move();

    public Blinking blinking = new Blinking();

    public Fall fall = new Fall();
    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        coll = GetComponent<BoxCollider>();
        switch (obstacleVariant)
        {
            case ObstacleVariant.Move:
                Move();
                break;
            case ObstacleVariant.Blinking:
                mesh.enabled = blinking.AppearFirst;
                coll.enabled = blinking.AppearFirst;
                blinking.isAppear = blinking.AppearFirst;
                break;
            case ObstacleVariant.Fall:
                Fall();
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (obstacleVariant)
        {
            case ObstacleVariant.Move:
                Move();
                break;
            case ObstacleVariant.Blinking:
                Blinking();
                break;
            case ObstacleVariant.Fall:
                Fall();
                break;
        }
    }

    private void Move()
    {

    }

    private void Blinking()
    {
        if (blinking.isAppear) StartCoroutine(Appear());
        else StartCoroutine(Disappear());
    }

    private void Fall()
    {

    }

    IEnumerator Appear()
    {
        yield return new WaitForSeconds(blinking.AppearTimer - blinking.BlinkingTimes * blinking.BlinkingDelay * 2);
        StartCoroutine(blink());
    }

    IEnumerator Disappear()
    {
        yield return new WaitForSeconds(blinking.DisappearTimer - blinking.BlinkingTimes * blinking.BlinkingDelay * 2);
        StartCoroutine(blink());
    }

    IEnumerator blink()
    {
        StopCoroutine(Appear());
        StopCoroutine(Disappear());
        yield return new WaitForSeconds(blinking.BlinkingDelay);
        Blink();
    }

    private int Blink()
    {
        if (blinking.blinkingTotal < blinking.BlinkingTimes * 2)
        {
            mesh.enabled = !mesh.enabled;
            blinking.blinkingTotal++;
            StopAllCoroutines();
            StartCoroutine(blink());
            return 0;
        }
        else
        {
            StopAllCoroutines();
            blinking.blinkingTotal = 0;
            mesh.enabled = coll.enabled = blinking.isAppear = !blinking.isAppear;
            blinking.isBlinking = false;
            return 0;
        }
    }
}
