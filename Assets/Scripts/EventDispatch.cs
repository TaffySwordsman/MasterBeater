using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

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

    public Queue<Dispatch> dispatchQueue;

    [SerializeField] private UnityEvent normalize;
    [SerializeField] private UnityEvent exersise;

    void Start() {
        dispatchQueue = new Queue<Dispatch>();
        dispatchQueue.Enqueue(new Dispatch(exersise, 5));
        dispatchQueue.Enqueue(new Dispatch(normalize, 10));
    }

    // Update is called once per frame
    void Update()
    {
        if (dispatchQueue.Count > 0) {
            if (dispatchQueue.Peek().Invoke(Time.time)) {
                dispatchQueue.Dequeue();
            }
        }
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