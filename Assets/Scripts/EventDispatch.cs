using System.Collections;
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
    [SerializeField] private UnityEvent normalize;
    [SerializeField] private UnityEvent exersise;

    public Queue<Dispatch> dispatchQueue;

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
}
