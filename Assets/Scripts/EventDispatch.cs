using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random=System.Random;


public class Dispatch 
{
    public UnityEvent ev;
    public float runAtTime;

    public Dispatch(UnityEvent uevent, float time) {
        ev = uevent;
        runAtTime = time;
    }

    // Invoke if it's time for the event.
    public bool Invoke(float time) {
        if (time >= runAtTime) {
            ev.Invoke();
            return true;
        }
        return false;
    }
}

public class EventDispatch : MonoBehaviour
{
    private static EventDispatch _current;

    // OnEnable and Awake can happen simultaneously, causing errors
    // This is a hotfix to make calling current Find this object if it is null
    // Was a major issue in Level 1
    // TODO: Actual fix, this was a hotfix by Brandon
    public static EventDispatch current
    {
        get
        {
            if (_current == null) {
                _current = FindObjectOfType<EventDispatch>();
            }
            return _current;
        }
        set => _current = value;
    }

    private void Awake()
    {
        current = this;
    }

    public float averageWait = 40;
    public float waitRange = 20;
    private float nextWait;

    [SerializeField] public GameObject warning;
    [SerializeField] public GameObject danger;   
    
    private float lastTime = 0f;
    
    private Random rng;

    [SerializeField] private UnityEvent normalize;
    [SerializeField] private UnityEvent exersise;

    void Start() {
        rng = new Random();
        SetWait();
        // StartCoroutine(RunBPMEvent(500, 3, 5));

        // dispatchQueue = new Queue<Dispatch>();
        // dispatchQueue.Enqueue(new Dispatch(exersise, 5));
        // dispatchQueue.Enqueue(new Dispatch(normalize, 10));
    }

    // Update is called once per frame
    void Update()
    {
        // if (Time.time - lastTime > nextWait) {
        //     SetWait();
            
        //     if (rng.NextDouble() > 0.5) {
        //         StartCoroutine(RunBPMEvent(500, 3, 5));
        //     } else {
        //         StartCoroutine(RunStrengthEvent(500, 3, 5));
        //     }
        // }
    }

    IEnumerator RunBPMEvent(float targetBPM, float warning, float length) {
        Debug.Log("BPM Event Coming");
        yield return new WaitForSeconds(warning);
        SetBPM(targetBPM);
        Debug.Log("BPM Event Started");
        yield return new WaitForSeconds(length);
        SetNormalize();
        Debug.Log("BPM Event Ended");
    }

    IEnumerator RunStrengthEvent(float newStrength, float warning, float length) {
        Debug.Log("Strength Event Coming");
        yield return new WaitForSeconds(warning);
        SetBeatStrength(newStrength);
        Debug.Log("Strength Event Started");
        yield return new WaitForSeconds(length);
        SetNormalize();
        Debug.Log("Strength Event Ended");
    }


    private void SetWait() {
        lastTime = Time.time;
        nextWait = averageWait + waitRange * 2f * ((float) rng.NextDouble() - 0.5f);
        Debug.Log("Next wait is " + nextWait);
    }

    /* ----------------------------------------------------------------------------------------- */

    #region HUD
    
    #endregion

    /* ----------------------------------------------------------------------------------------- */

    #region Mechanics
    
    public event Action OnNormalize;

    public void SetNormalize()
    {
        OnNormalize?.Invoke();
    }

    public event Action<float> OnSetBPM;

    public void SetBPM(float targetBPM)
    {
        OnSetBPM?.Invoke(targetBPM);
    }

    public event Action<float> OnSetBeatStrength;

    public void SetBeatStrength(float beatStrength)
    {
        OnSetBeatStrength?.Invoke(beatStrength);
    }
    #endregion

    /* ----------------------------------------------------------------------------------------- */

    #region Game State

    public event Action<bool> OnPauseGame;

    public void PauseGame(bool isPaused)
    {
        OnPauseGame?.Invoke(isPaused);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    #endregion

}