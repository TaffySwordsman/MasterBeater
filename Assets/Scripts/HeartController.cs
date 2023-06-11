using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeartController : MonoBehaviour
{
    [Header("Cardio")]
    public float systolic = 120f;
    public float diastolic = 80f;
    public float tgtSystolic = 120f;
    public float beatStrength = 15f;
    public float normalBPM = 20f;
    private float targetBPM;
    public float BPM;

    [Header("Health")]
    public float health = 100f;
    public float safeRange = 0.4f;
    public float recoveryRate = 2f;
    public float damageRate = 0.05f;

    private float scale = 1.0f;
    private float lastBeat = 0.0f;


    [Header("Money")]
    public float money = 0.0f;
    public float maxEarnRate = 100f;

    private List<float> beats;
    private float lastHumanBeat = 0f;
    private float lastRelease = -50f;              // Time of last release.

    private RectTransform rt;

    [Header("Upgrades")]
    public int AutoPacer = 0;
    public int BrainInterface = 0;
    public int BeatModulator = 0;
    public int MalpracticeDefenseLawyers = 0;
    public int PerfluorocarbonSurogate = 0;
    public int IntravanousLubricant = 0;
    public int EmergencyReleaseValve = 0;
    public int RevisedTermsOfService = 0;

    [Header("Upgrade Affects")]
    public float autoBeatRevUp = 5f;            // Time before autobeat kicks in. 
    public float autoBeatRevReduction = 1.0f;   // Reduction of revup per pacer level.
    public float autoBeatRate = 1f;             // Rate of auto beat.
    public float modulation = 0.15f;            // Percent modulation per level of modulator.
    public float surrogateResistance = 0.015f;  // Damage resistance per level of perfluorocarbon.
    public float lubricantBuffer = 0.1f;        // Percent added to safeRange per level of lubricant.
    public float refractoryPeriod = 20f;        // Cool down for emergency valve.
    public float refractoryReduction = 3f;      // Refractory reduction per level of release valve.
    public float releaseRate = 50f;             // Amount of pressure released over the one second of valve opening.
    public float releasePenalty = 10f;          // Penalty to beat strength for emergency release.
    public float releasePenaltyRecution = 2f;   // Reduction of the penalty per level of emergency valve.
    public float mineRate = 5f;                 // $ minned per beat per level of TOS.

    // Start is called before the first frame update
    void Start()
    {
        rt = gameObject.GetComponent<RectTransform>();
        targetBPM = normalBPM;
        beats = new List<float>();
        beats.Add(0f);
        beats.Add(0f);

        EventDispatch.current.OnNormalize += () => Normalize();
        EventDispatch.current.OnSetBPM += (newBPM) => SetTargetBPM(newBPM);

    }

    // Update is called once per frame
    void Update()
    {
        // Autobeat
        // Beat if revup period is over and it's past autoBeatRate seconds.
        if (AutoPacer > 0 &&
            Time.time - lastHumanBeat > autoBeatRevUp - autoBeatRevReduction * AutoPacer && 
            Time.time - lastBeat > autoBeatRate) {
            Beat();
        }

        // Beat Scale
        if (scale < 1f) {
            scale += Time.deltaTime * 1.0f;
        } else if (scale < 0.5f) {
            scale = 0.5f;
        } else {
            scale = 1f;
        }
        rt.localScale = new Vector3(scale, scale, scale);

        // Pressure Range
        float maxSafe = tgtSystolic * (1f + safeRange + lubricantBuffer * (float) IntravanousLubricant);
        float minSafe = tgtSystolic * (1f - safeRange + lubricantBuffer * (float) IntravanousLubricant);
        float overPressure = tgtSystolic * 1.85f;

        // Update Pressure
        float decay = (2*targetBPM*(Time.time - lastBeat));
        if (systolic > overPressure && EmergencyReleaseValve > 0 && Time.time - lastRelease > refractoryPeriod) {  // Emergency Valve
            lastRelease = Time.time;
            Debug.Log("OVERPRESSURE!");
        }
        if (Time.time - lastRelease <= 1.0f && systolic > minSafe) {
            decay += releaseRate;
            Debug.Log("Decay: " + decay);
        }
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

        BPM = 1f / (beatLengths.Average() / 60f);

        if (beats.Count > 10) { // Only read last 10 beats.
            beats.RemoveAt(0);
        }

        // Health
        if (systolic > maxSafe) {
            health -= (systolic - maxSafe) * Time.deltaTime * (damageRate - surrogateResistance * (float) PerfluorocarbonSurogate);
        } else if (systolic < minSafe) {
            health -= (minSafe - systolic) * Time.deltaTime * (damageRate - surrogateResistance * (float) PerfluorocarbonSurogate);
        } else {
            health = Mathf.Min(health + recoveryRate * Time.deltaTime, 100f);
        }
    }

    public void MouseDown() {
        lastHumanBeat = Time.time;
        Beat();
    }

    void Beat() {
        lastBeat = Time.time;
        beats.Add(Time.time);

        float beat = beatStrength;
        
        // Release Penalty
        float timeSinceRelease = Time.time - lastRelease;
        beat *= Mathf.Min((1 - timeSinceRelease / refractoryPeriod) * (releasePenalty - releasePenaltyRecution * EmergencyReleaseValve), 1.0f);

        systolic += beatStrength;
        float mod = beatStrength * modulation * (float) BeatModulator;
        if (systolic > tgtSystolic + mod) {
            systolic -= mod;
        } else if (systolic < tgtSystolic - mod) {
            systolic += mod;
        }
        scale -= .25f;
        money += RevisedTermsOfService * mineRate;
    }

    public void Normalize() {
        targetBPM = normalBPM;
    }

    public void SetTargetBPM(float newBPM) {
        targetBPM = newBPM;
    }
}
