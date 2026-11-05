using UnityEngine;

/// <summary>
/// Controls the day/night cycle: rotates the sun Directional Light and updates
/// all SkyboxDynamic shader properties each frame.
///
/// Setup:
///   1. Add this component to any GameObject in the scene.
///   2. Assign Sky Material (material using Skybox/Dynamic shader).
///   3. Assign Sun (Directional Light).
///   4. Assign the material to Lighting > Environment > Skybox Material.
///   5. Set Sun Source in Lighting to None (shader handles it).
/// </summary>
[ExecuteAlways]
public class TimeOfDay : MonoBehaviour
{
    // ── Time ──────────────────────────────────────────────────────────────────
    [Header("Time")]
    [Range(0f, 24f)] public float timeOfDay          = 12f;
    public bool  autoAdvance                          = true;
    public float dayDurationSeconds                   = 180f; // real seconds for the day phase (sunrise→sunset)
    public float nightDurationSeconds                 = 90f;  // real seconds for the night phase (sunset→sunrise)

    // ── References ────────────────────────────────────────────────────────────
    [Header("References")]
    public Material skyMaterial;
    public Light    sun;
    public Light    moon;

    // ── Sun arc ───────────────────────────────────────────────────────────────
    [Header("Sun Arc")]
    [Range(0f, 12f)]     public float sunriseHour = 6f;
    [Range(12f, 24f)]    public float sunsetHour  = 18f;
    [Tooltip("Azimuth offset of the sun's path (Y rotation)")]
    [Range(-180f, 180f)] public float sunAzimuth  = -30f;

    // ── Day sky ───────────────────────────────────────────────────────────────
    [Header("Day Sky")]
    public Color dayZenith  = new Color(0.08f, 0.37f, 0.73f);
    public Color dayMidSky  = new Color(0.36f, 0.64f, 0.90f);
    public Color dayHorizon = new Color(0.62f, 0.82f, 0.97f);
    public Color dayGround  = new Color(0.25f, 0.22f, 0.18f);

    // ── Night sky ─────────────────────────────────────────────────────────────
    [Header("Night Sky")]
    public Color nightZenith  = new Color(0.005f, 0.008f, 0.025f);
    public Color nightHorizon = new Color(0.020f, 0.030f, 0.060f);
    public Color nightGround  = new Color(0.008f, 0.008f, 0.010f);

    // ── Sunset / sunrise ──────────────────────────────────────────────────────
    [Header("Sunset / Sunrise")]
    public Color sunsetZenith  = new Color(0.25f, 0.08f, 0.18f);
    public Color sunsetHorizon = new Color(0.98f, 0.30f, 0.04f);

    // ── Sun appearance ────────────────────────────────────────────────────────
    [Header("Sun Appearance")]
    public Color sunDiscColor   = new Color(1.00f, 0.97f, 0.85f);
    public Color sunGlowDay     = new Color(1.00f, 0.80f, 0.40f);
    public Color sunGlowSunset  = new Color(1.00f, 0.35f, 0.05f);

    // ── Lighting ──────────────────────────────────────────────────────────────
    [Header("Moon Light")]
    [Range(0f, 1f)] public float moonIntensity = 0.08f;
    public Color moonColor = new Color(0.55f, 0.65f, 0.85f);

    [Header("Ambient Light")]
    public Color ambientDay    = new Color(0.50f, 0.55f, 0.60f);
    public Color ambientSunset = new Color(0.60f, 0.40f, 0.30f);
    public Color ambientNight  = new Color(0.03f, 0.03f, 0.06f);

    [Header("Fog")]
    public Color fogDay   = new Color(0.62f, 0.82f, 0.97f);
    public Color fogNight = new Color(0.02f, 0.03f, 0.06f);

    [Header("Clouds")]
    [Range(0f, 1f)]   public float cloudCoverage  = 0.55f;
    [Range(2f, 25f)]  public float cloudSharpness = 10f;
    [Range(0.5f, 10f)] public float cloudScale    = 3f;
    [Range(0f, 0.05f)]  public float cloudSpeed         = 0.005f;
    [Range(0f, 1.5f)]  public float cloudWarpStrength = 0.5f;
    public Color cloudColorDay    = new Color(0.95f, 0.95f, 1.00f);
    public Color cloudColorSunset = new Color(1.00f, 0.55f, 0.25f);
    public Color cloudColorNight  = new Color(0.18f, 0.20f, 0.28f);

    // ─────────────────────────────────────────────────────────────────────────

    void Update()
    {
        if (autoAdvance && Application.isPlaying)
        {
            float daySpan   = sunsetHour - sunriseHour;      // e.g. 12h
            float nightSpan = 24f - daySpan;                  // e.g. 12h
            bool  isDay     = timeOfDay >= sunriseHour && timeOfDay < sunsetHour;
            float rate      = isDay
                ? daySpan   / dayDurationSeconds
                : nightSpan / nightDurationSeconds;
            timeOfDay = (timeOfDay + Time.deltaTime * rate) % 24f;
        }

        Apply();
    }

    void Apply()
    {
        if (skyMaterial == null || sun == null) return;

        // ── Rotate sun ────────────────────────────────────────────────────────
        // pitch: 6h → 0° (horizon), 12h → 90° (zenith/nadir for light), 18h → 180°, 0h → -90°
        // Directional Light forward points WHERE the light shines (toward ground at noon).
        // Euler(90,..) → forward=(0,-1,0) = shines down = sun overhead. ✓
        float pitch = (timeOfDay - sunriseHour) * 15f;
        sun.transform.localRotation = Quaternion.Euler(pitch, sunAzimuth, 0f);

        // ── Blend values ──────────────────────────────────────────────────────
        // sunY > 0 → sun above horizon (day); sunY < 0 → below horizon (night)
        float sunY = -sun.transform.forward.y; // forward.y=-1 at noon → sunY=1

        // Wide transition zone for natural sunrise/sunset (over ~2.5h each side)
        float dayBlend = Mathf.Clamp01((sunY + 0.25f) / 0.5f);

        // duskBlend peaks when sun is near the horizon (sunrise/sunset)
        float duskBlend = Mathf.Clamp01(1f - Mathf.Abs(sunY) / 0.25f) * Mathf.Clamp01(dayBlend * 6f);

        // ── Shader properties ─────────────────────────────────────────────────
        skyMaterial.SetColor("_DayZenith",     dayZenith);
        skyMaterial.SetColor("_DayMidSky",     dayMidSky);
        skyMaterial.SetColor("_DayHorizon",    dayHorizon);
        skyMaterial.SetColor("_DayGround",     dayGround);
        skyMaterial.SetColor("_NightZenith",   nightZenith);
        skyMaterial.SetColor("_NightHorizon",  nightHorizon);
        skyMaterial.SetColor("_NightGround",   nightGround);
        skyMaterial.SetColor("_SunsetZenith",  sunsetZenith);
        skyMaterial.SetColor("_SunsetHorizon", sunsetHorizon);
        skyMaterial.SetColor("_SunDiscColor",  sunDiscColor);
        skyMaterial.SetColor("_SunGlowColor",  Color.Lerp(sunGlowDay, sunGlowSunset, duskBlend));

        skyMaterial.SetVector("_SunDir",       -sun.transform.forward);
        skyMaterial.SetFloat("_DayNightBlend", dayBlend);
        skyMaterial.SetFloat("_SunsetBlend",   duskBlend);
        skyMaterial.SetFloat("_StarIntensity", 1f - dayBlend);

        // ── Sun light ─────────────────────────────────────────────────────────
        sun.color     = Color.Lerp(sunGlowSunset, sunDiscColor, dayBlend);
        sun.intensity = Mathf.Clamp01(sunY * 4f + 0.1f);

        // ── Moon light ────────────────────────────────────────────────────────
        if (moon != null)
        {
            moon.transform.forward = -sun.transform.forward;
            moon.color     = moonColor;
            moon.intensity = moonIntensity * (1f - dayBlend);
        }

        // ── Ambient / fog ─────────────────────────────────────────────────────
        Color ambient = Color.Lerp(ambientNight, Color.Lerp(ambientDay, ambientSunset, duskBlend), dayBlend);
        RenderSettings.ambientMode  = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = ambient;

        if (RenderSettings.fog)
            RenderSettings.fogColor = Color.Lerp(fogNight, fogDay, dayBlend);

        // ── Clouds ────────────────────────────────────────────────────────────────
        skyMaterial.SetFloat("_CloudCoverage",   cloudCoverage);
        skyMaterial.SetFloat("_CloudSharpness",  cloudSharpness);
        skyMaterial.SetFloat("_CloudScale",      cloudScale);
        skyMaterial.SetFloat("_CloudSpeed",      cloudSpeed);
        skyMaterial.SetColor("_CloudColorDay",    cloudColorDay);
        skyMaterial.SetColor("_CloudColorSunset", cloudColorSunset);
        skyMaterial.SetColor("_CloudColorNight",   cloudColorNight);
        skyMaterial.SetFloat("_CloudWarpStrength", cloudWarpStrength);
    }
}
