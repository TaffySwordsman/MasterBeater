using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Random=System.Random;

public class HeartController : MonoBehaviour
{
    public RankDisplay rankDisplay;
    public PostProcessVolume volume;
    public News news;
    private Vignette vignette;
    private AudioSource audioData;


    [Header("Events")]
    public string[] bpmEventMessages;
    public float minElevatedBPM = 40f;
    public float maxElevatedBPM = 80f;
    public string[] strengthEventMessages;
    public float minStengthBoost = 20f;
    public float maxStrengthBoost = 60f;

    [Header("Cardio")]
    public float systolic = 120f;
    public float diastolic = 80f;
    public float tgtSystolic = 120f;
    public float normalBeatStrength = 15f;
    public float beatStrength;
    public float normalBPM = 20f;
    private float targetBPM;
    public float BPM;
    public float averageWait = 40f;
    public float waitRange = 20f;

    [Header("Health")]
    public float health = 100f;
    public float safeRange = 0.4f;
    public float recoveryRate = 2f;
    public float damageRate = 0.05f;

    private float scale = 1.0f;
    private float lastBeat = 0.0f;

    [Header("Score")]
    public float xp = 0f;
    public float maxEarnRate = 100f;
    public float lossRate = 100f;
    public string[] ranks;
    public string[] rankNewsMessages;
    public float[] rankXp;
    public int currentRank = 0;
    public string currentRankStr;
    public GameObject star;

    private List<float> beats;
    private float lastHumanBeat = 0f;
    private float lastRelease = -50f; 
    public string name;             // Time of last release.

    private RectTransform rt;

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
    public float lawyerDefense = 20f;           // $ minned per beat per level of TOS.
    public float earlyWarningLevel = 3f;
    [SerializeField] public GameObject warningObj;
    [SerializeField] public GameObject dangerObj; 

    private Random rng;

    // Start is called before the first frame update
    void Start()
    {
        rng = new Random();
        rt = gameObject.GetComponent<RectTransform>();
        Normalize();
        beats = new List<float>();
        beats.Add(0f);
        beats.Add(0f);
        currentRankStr = ranks[currentRank];
        rankDisplay.UpdateRank(currentRankStr, name);
        audioData = GetComponent<AudioSource>();

        EventDispatch.current.OnNormalize += () => Normalize();
        EventDispatch.current.OnSetBPM += (newBPM) => SetTargetBPM(newBPM);
        EventDispatch.current.OnSetBeatStrength += (newStrength) => SetBeatStrength(newStrength);

        vignette = ScriptableObject.CreateInstance<Vignette>();
        vignette.enabled.Override(true);
        vignette.intensity.Override(0f);
        volume = PostProcessManager.instance.QuickVolume(gameObject.layer, 100f, vignette);
    }

    void OnDestroy() {
        RuntimeUtilities.DestroyVolume(volume, true, true);
    }

    // Update is called once per frame
    void Update()
    {

        if (beats.Count < 3) {
            return;
        }

        // Autobeat
        // Beat if revup period is over and it's past autoBeatRate seconds.
        if (CashController.current.AutoPacer > 0 &&
            Time.time - lastHumanBeat > autoBeatRevUp - autoBeatRevReduction * CashController.current.AutoPacer && 
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
        float maxSafe = tgtSystolic * (1f + safeRange + lubricantBuffer * (float) CashController.current.IntravanousLubricant);
        float minSafe = tgtSystolic * (1f - safeRange + lubricantBuffer * (float) CashController.current.IntravanousLubricant);
        float overPressure = tgtSystolic * 1.85f;

        // Update Pressure
        float decay = (2*targetBPM*(Time.time - lastBeat));
        if (systolic > overPressure && CashController.current.EmergencyReleaseValve > 0 && Time.time - lastRelease > refractoryPeriod) {  // Emergency Valve
            lastRelease = Time.time;
        }
        if (Time.time - lastRelease <= 1.0f && systolic > minSafe) {
            decay += releaseRate;
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
        Earn(Time.deltaTime * earnRate * consitency);

        BPM = 1f / (beatLengths.Average() / 60f);

        if (beats.Count > 10) { // Only read last 10 beats.
            beats.RemoveAt(0);
        }

        if (health < 1f) { // Death Penalty
            float realLoss = lossRate - CashController.current.MalpracticeDefenseLawyers * lawyerDefense;
            CashController.current.money = Mathf.Max(CashController.current.money - lossRate * Time.deltaTime, 0f);
        }

        // Health
        if (systolic > maxSafe) {
            health -= (systolic - maxSafe) * Time.deltaTime * (damageRate - surrogateResistance * (float) CashController.current.PerfluorocarbonSurogate);
        } else if (systolic < minSafe) {
            health -= (minSafe - systolic) * Time.deltaTime * (damageRate - surrogateResistance * (float) CashController.current.PerfluorocarbonSurogate);
        } else {
            health = Mathf.Min(health + recoveryRate * Time.deltaTime, 100f);
        }
        health = Mathf.Max(health, 0f);
        vignette.intensity.value = 0.53f - health / 100f;

        // Event loop
        if (Time.time - lastTime > nextWait) {
            SetWait();
            
            if (rng.NextDouble() > 0.5) {
                string message = bpmEventMessages[rng.Next(0, bpmEventMessages.Count())] + "  Blood pressure decreasing.";
                float bpm = (maxElevatedBPM - minElevatedBPM) * (float) rng.NextDouble() + minElevatedBPM;
                StartCoroutine(RunBPMEvent(message, bpm, CashController.current.BrainInterface * earlyWarningLevel, (float) rng.NextDouble() * 10f + 5f));
            } else {
                string message = strengthEventMessages[rng.Next(0, strengthEventMessages.Count())] + "  Beat strength increasing.";
                float str = (maxStrengthBoost - minStengthBoost) * (float) rng.NextDouble() + minStengthBoost;
                StartCoroutine(RunStrengthEvent(message, str, CashController.current.BrainInterface * earlyWarningLevel, (float) rng.NextDouble() * 10f + 5f));
            }
        }
    }

    public void MouseDown() {
        lastHumanBeat = Time.time;
        Beat();
    }

    void Beat() {
        audioData.Play();
        lastBeat = Time.time;
        beats.Add(Time.time);

        float beat = beatStrength;
        
        // Release Penalty
        float timeSinceRelease = Time.time - lastRelease;
        beat *= Mathf.Min((1 - timeSinceRelease / refractoryPeriod) * (releasePenalty - releasePenaltyRecution * CashController.current.EmergencyReleaseValve), 1.0f);

        systolic += beatStrength;
        float mod = beatStrength * modulation * (float) CashController.current.BeatModulator;
        if (systolic > tgtSystolic + mod) {
            systolic -= mod;
        } else if (systolic < tgtSystolic - mod) {
            systolic += mod;
        }
        scale -= .25f;
        CashController.current.EarnBTC(CashController.current.RevisedTermsOfService * mineRate);
    }

    public void Normalize() {
        targetBPM = normalBPM;
        beatStrength = normalBeatStrength;
    }

    public void SetTargetBPM(float newBPM) {
        targetBPM = newBPM;
    }

    public void SetBeatStrength(float newStrength) {
        beatStrength = newStrength;
    }

    public void Earn(float addedMoney) {
        xp += CashController.current.EarnWithTax(addedMoney);
        if (currentRank < ranks.Count() - 1) {
            if (xp >= rankXp[currentRank+1]) {
                currentRank += 1;
                currentRankStr = ranks[currentRank];
                rankDisplay.UpdateRank(currentRankStr, name);
                news.RunHeadline(rankNewsMessages[currentRank]);
            }
            if(currentRank == ranks.Count() - 1)
                star.SetActive(true);
        }
    }

    // ---------------------- Events

    private float lastTime = 0f;
    private float nextWait = 70f;

    private void SetWait() {
        lastTime = Time.time;
        nextWait = averageWait + waitRange * 2f * ((float) rng.NextDouble() - 0.5f);
        Debug.Log("Next wait is " + nextWait);
    }

    IEnumerator RunBPMEvent(string message, float bpm, float warning, float length) {
        Debug.Log("BPM Event Coming");
        news.RunHeadline(message);
        warningObj.SetActive(true);
        yield return new WaitForSeconds(warning);
        warningObj.SetActive(false);
        dangerObj.SetActive(true);
        targetBPM = bpm;
        yield return new WaitForSeconds(length);
        dangerObj.SetActive(false);
        Normalize();
        Debug.Log("BPM Event Ended");
    }

    IEnumerator RunStrengthEvent(string message, float newStrength, float warning, float length) {
        Debug.Log("Strength Event Coming");
        news.RunHeadline(message);
        warningObj.SetActive(true);
        yield return new WaitForSeconds(warning);
        warningObj.SetActive(false);
        dangerObj.SetActive(true);
        SetBeatStrength(newStrength);
        yield return new WaitForSeconds(length);
        dangerObj.SetActive(false);
        Normalize();
        Debug.Log("Strength Event Ended");
    }

}
