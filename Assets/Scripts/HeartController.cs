using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeartController : MonoBehaviour
{
    public float systolic = 120f;
    public float diastolic = 80f;
    public float tgtSystolic = 120f;
    public float beatStrength = 15f;
    public float normalBPM = 20f;
    private float targetBPM;

    public float health = 100f;
    public float safeRange = 0.4f;
    public float recoveryRate = 2f;
    public float damageRate = 0.05f;

    private float scale = 1.0f;
    private float lastBeat = 0.0f;

    public float money = 0.0f;
    public float maxEarnRate = 100f;

    private List<float> beats;

    // Upgrades
    public int AutoPacer = 0;
    public int BrainInterface = 0;
    public int BeatModulator = 0;
    public int MalpracticeDefenseLawyers = 0;
    public int PerfluorocarbonSurogate = 0;
    public int IntravanousLubricant = 0;
    public int EmergencyReleaseValve = 0;
    public int RevisedTermsOfService = 0;

    public float modulation = 0.15f;            // Percent modulation per level of modulator.
    public float surrogateResistance = 0.015f;  // Damage resistance per level of perfluorocarbon.
    public float lubricantBuffer = 0.1f;        // Percent added to safeRange per level of lubricant.
    public float mineRate = 5f;                 // $ minned per beat per level of TOS.

    // Start is called before the first frame update
    void Start()
    {
        targetBPM = normalBPM;
        beats = new List<float>();
        beats.Add(0f);
        beats.Add(0f);
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
        float decay = (2*targetBPM*(Time.time - lastBeat));
        systolic -= decay * Time.deltaTime;
        if (systolic < 0f) {
            systolic = 0f;
        }
        diastolic = Mathf.Max(systolic - 40f, 0f);

        // Score
        List<float> beatLengths = new List<float>();
        for (int i = 1; i < beats.Count; ++i) {
            beatLengths.Add(beats[i] - beats[i-1]);
        }
        float consitency = Mathf.Max(-(beatLengths.Max() - beatLengths.Min()) / 1.5f + 1f, 0.3f);
        float earnRate = Mathf.Max(maxEarnRate - 10f * Mathf.Abs(systolic - tgtSystolic), 0);
        money += Time.deltaTime * earnRate * consitency;

        float bpm = 1f / (beatLengths.Average() / 60f);

        if (beats.Count > 10) { // Only read last 10 beats.
            beats.RemoveAt(0);
        }

        // Health
        float maxSafe = tgtSystolic * (1f + safeRange + lubricantBuffer * (float) IntravanousLubricant);
        float minSafe = tgtSystolic * (1f - safeRange + lubricantBuffer * (float) IntravanousLubricant);
        if (systolic > maxSafe) {
            health -= (systolic - maxSafe) * Time.deltaTime * (damageRate - surrogateResistance * (float) PerfluorocarbonSurogate);
        } else if (systolic < minSafe) {
            health -= (minSafe - systolic) * Time.deltaTime * (damageRate - surrogateResistance * (float) PerfluorocarbonSurogate);
        } else {
            health = Mathf.Min(health + recoveryRate * Time.deltaTime, 100f);
        }
    }

    void OnMouseDown() {
        lastBeat = Time.time;
        beats.Add(Time.time);
        systolic += beatStrength;
        float mod = beatStrength * modulation * (float) BeatModulator;
        if (systolic > tgtSystolic + mod) {
            systolic -= mod;
        } else if (systolic < tgtSystolic - mod) {
            systolic += mod;
        }
        scale += .25f;
        money += RevisedTermsOfService * mineRate;
    }

    public void Normalize() {
        targetBPM = normalBPM;
        Debug.Log("Normalize");
    }

    public void SetTargetBPM(float newBPM) {
        targetBPM = newBPM;
        Debug.Log(newBPM);
    }
}
