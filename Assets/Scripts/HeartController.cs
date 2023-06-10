using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartController : MonoBehaviour
{
    public float systolic = 120f;
    public float diastolic = 80f;
    public float tgtBpm = 60f;
    public float beatStrength = 15f;
    
    private float scale = 1.0f;
    private float lastBeat = 0.0f;

    // Components


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Beat Scale
        if (scale > 1f) {
            scale -= Time.deltaTime * 1.0f;
        } else if (scale > 1.5f) {
            scale = 1.5f;
        } else {
            scale = 1f;
        }
        gameObject.transform.localScale = new Vector3(scale, scale, scale);

        // Update Pressure
        Debug.Log("Blood Pressure: " + systolic + "/" + diastolic + "\tDecay: " + (-2*tgtBpm*(Time.time - lastBeat)));
        systolic -= 2 * tgtBpm * (Time.time - lastBeat) * Time.deltaTime;
        if (systolic < 0f) {
            systolic = 0f;
        }
        diastolic = systolic - 80f;
    }

    void OnMouseDown() {
        lastBeat = Time.time;
        systolic += beatStrength;
        // diastolic += 5;
        scale += .25f;
    }
}
