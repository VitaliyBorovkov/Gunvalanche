Shader "Skybox/Dynamic"
{
    Properties
    {
        [Header(Day Sky)]
        _DayZenith      ("Day Zenith",   Color) = (0.08, 0.37, 0.73, 1)
        _DayMidSky      ("Day Mid Sky",  Color) = (0.36, 0.64, 0.90, 1)
        _DayHorizon     ("Day Horizon",  Color) = (0.62, 0.82, 0.97, 1)
        _DayGround      ("Day Ground",   Color) = (0.25, 0.22, 0.18, 1)

        [Header(Night Sky)]
        _NightZenith    ("Night Zenith",  Color) = (0.005, 0.008, 0.025, 1)
        _NightHorizon   ("Night Horizon", Color) = (0.020, 0.030, 0.060, 1)
        _NightGround    ("Night Ground",  Color) = (0.008, 0.008, 0.010, 1)

        [Header(Sunset Sunrise)]
        _SunsetZenith   ("Sunset Zenith",  Color) = (0.25, 0.08, 0.18, 1)
        _SunsetHorizon  ("Sunset Horizon", Color) = (0.98, 0.30, 0.04, 1)
        _SunsetBlend    ("Sunset Blend",   Range(0,1)) = 0

        [Header(Sun)]
        _SunDir         ("Sun Direction (world)", Vector) = (0,1,0,0)
        _SunDiscColor   ("Sun Disc Color",  Color)  = (1.0, 0.97, 0.85, 1)
        _SunDiscSize    ("Sun Disc Size",   Range(0.001, 0.05)) = 0.020
        _SunGlowColor   ("Sun Glow Color",  Color)  = (1.0, 0.55, 0.10, 1)
        _SunGlowSpread  ("Sun Glow Spread", Range(0.01, 0.6)) = 0.25

        [Header(Moon)]
        _MoonDiscColor  ("Moon Disc Color", Color) = (0.82, 0.85, 0.92, 1)
        _MoonDiscSize   ("Moon Disc Size",  Range(0.001, 0.05)) = 0.014

        [Header(Stars)]
        _StarIntensity  ("Star Intensity",  Range(0,1)) = 0
        _StarDensity    ("Star Density",    Range(50,500)) = 180

        [Header(Clouds)]
        _CloudCoverage  ("Coverage",      Range(0,1))    = 0.55
        _CloudSharpness ("Sharpness",     Range(2,25))   = 10
        _CloudScale     ("Scale",         Range(0.5,10)) = 3
        _CloudSpeed     ("Speed",         Range(0,0.05)) = 0.005
        _CloudColorDay    ("Color Day",    Color) = (0.95, 0.95, 1.0, 1)
        _CloudColorSunset ("Color Sunset", Color) = (1.0, 0.55, 0.25, 1)
        _CloudColorNight  ("Color Night",  Color) = (0.18, 0.20, 0.28, 1)
        _CloudWarpStrength ("Warp Strength", Range(0,1.5)) = 0.5

        [Header(Blend)]
        _DayNightBlend  ("Day Night Blend (0=night 1=day)", Range(0,1)) = 1
        _HorizonBlend   ("Horizon Blend",                   Range(1,20)) = 7
    }

    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // Day
            half3 _DayZenith, _DayMidSky, _DayHorizon, _DayGround;
            // Night
            half3 _NightZenith, _NightHorizon, _NightGround;
            // Sunset
            half3 _SunsetZenith, _SunsetHorizon;
            float _SunsetBlend;
            // Sun
            float4 _SunDir;
            half3  _SunDiscColor, _SunGlowColor;
            float  _SunDiscSize, _SunGlowSpread;
            // Moon
            half3  _MoonDiscColor;
            float  _MoonDiscSize;
            // Stars
            float  _StarIntensity, _StarDensity;
            // Clouds
            float  _CloudCoverage, _CloudSharpness, _CloudScale, _CloudSpeed, _CloudWarpStrength;
            half3  _CloudColorDay, _CloudColorSunset, _CloudColorNight;
            // Blend
            float  _DayNightBlend, _HorizonBlend;

            struct appdata { float4 vertex : POSITION; };
            struct v2f     { float4 pos : SV_POSITION; float3 dir : TEXCOORD0; };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.dir = v.vertex.xyz;
                return o;
            }

            // ── 2D hash & value noise for clouds ────────────────────────────────────
            float hash21(float2 p)
            {
                p = frac(p * float2(234.34, 435.345));
                p += dot(p, p + 34.23);
                return frac(p.x * p.y);
            }

            float valueNoise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float2 u = f * f * (3.0 - 2.0 * f);
                return lerp(
                    lerp(hash21(i),              hash21(i + float2(1,0)), u.x),
                    lerp(hash21(i + float2(0,1)), hash21(i + float2(1,1)), u.x),
                    u.y);
            }

            // Two-octave noise with posterization → chunky low-poly cloud shapes
            float cloudNoise(float2 uv)
            {
                float n = valueNoise(uv)             * 0.65
                        + valueNoise(uv * 2.2 + 5.3) * 0.35;
                return floor(n * 5.0 + 0.5) / 5.0; // posterize → flat polygon regions
            }

            // ── Procedural stars ─────────────────────────────────────────────────────
            float hash13(float3 p)
            {
                p = frac(p * float3(443.897, 441.423, 437.195));
                p += dot(p, p.yzx + 19.19);
                return frac((p.x + p.y) * p.z);
            }

            float starField(float3 dir, float density)
            {
                float3 cell = floor(dir * density);
                float  h    = hash13(cell);
                if (h < 0.994) return 0;
                float3 center = normalize(cell + 0.5);
                float  d      = dot(normalize(dir), center);
                float  sz     = lerp(0.99994, 0.99999, h);
                return smoothstep(sz - 0.000005, sz, d) * pow(h, 3.0);
            }

            half4 frag(v2f i) : SV_Target
            {
                float3 dir = normalize(i.dir);
                float  y   = dir.y; // -1..1, positive = up

                // ── Sky gradient ─────────────────────────────────────────────────────
                float zenithT = pow(saturate(y), 1.0 / _HorizonBlend);
                float belowT  = saturate(-y * 3.0); // fade to ground below horizon

                // Day: horizon → midsky → zenith
                half3 daySky  = lerp(_DayHorizon, _DayMidSky, zenithT);
                daySky        = lerp(daySky, _DayZenith, pow(zenithT, 2.0));
                half3 dayFull = lerp(daySky, _DayGround, belowT);

                // Night: horizon → zenith
                half3 nightSky  = lerp(_NightHorizon, _NightZenith, zenithT);
                half3 nightFull = lerp(nightSky, _NightGround, belowT);

                half3 col = lerp(nightFull, dayFull, _DayNightBlend);

                // ── Sunset/sunrise horizon tint ──────────────────────────────────────
                if (_SunsetBlend > 0.001)
                {
                    float3 sunDir = normalize(_SunDir.xyz);
                    // Spread warm color along horizon in the sun's direction
                    float3 sunH = normalize(float3(sunDir.x, 0.0, sunDir.z) + 0.0001);
                    float3 dirH = normalize(float3(dir.x,   0.0, dir.z)   + 0.0001);
                    float  horzAlign    = saturate(dot(dirH, sunH));
                    float  horizonBand  = saturate(1.0 - abs(y) * 4.0);
                    half3  sunsetCol    = lerp(_SunsetHorizon, _SunsetZenith, zenithT);
                    float  sunsetFactor = _SunsetBlend * horizonBand * pow(horzAlign, 1.5)
                                       + _SunsetBlend * saturate(y) * 0.25;
                    col = lerp(col, sunsetCol, saturate(sunsetFactor));
                }

                // ── Sun disc + atmospheric glow ──────────────────────────────────────
                {
                    float3 sunDir = normalize(_SunDir.xyz);
                    float  sd     = dot(dir, sunDir);

                    // Atmospheric glow (wider halo)
                    float glowT = saturate((sd - (1.0 - _SunGlowSpread)) / _SunGlowSpread);
                    glowT = pow(glowT, 2.5);
                    float sunVis = saturate((sunDir.y + 0.15) / 0.15); // fade when below horizon
                    col += _SunGlowColor * glowT * _DayNightBlend * sunVis;

                    // Disc
                    float disc = smoothstep(1.0 - _SunDiscSize - 0.001, 1.0 - _SunDiscSize + 0.001, sd);
                    col = lerp(col, _SunDiscColor * 1.5, disc * _DayNightBlend * sunVis);
                }

                // ── Moon (opposite sun) ──────────────────────────────────────────────
                {
                    float3 moonDir = normalize(-_SunDir.xyz);
                    float  md      = dot(dir, moonDir);
                    float  moonVis = saturate((-_SunDir.y + 0.15) / 0.15);
                    float  disc    = smoothstep(1.0 - _MoonDiscSize - 0.001, 1.0 - _MoonDiscSize + 0.001, md);
                    col = lerp(col, _MoonDiscColor * 1.2, disc * (1.0 - _DayNightBlend) * moonVis);
                }

                // ── Clouds ───────────────────────────────────────────────────────────
                if (y > 0.02 && _CloudCoverage > 0.001)
                {
                    // Perspective projection onto cloud plane
                    float2 cloudUV = (dir.xz / max(dir.y, 0.07)) * _CloudScale;
                    float2 windOffset = _Time.y * _CloudSpeed * float2(1.0, 0.4);
                    cloudUV += windOffset;

                    // Domain warp: slow independent noise offsets the UV → shape morphing
                    if (_CloudWarpStrength > 0.001)
                    {
                        float2 warpUV = cloudUV * 0.5 + _Time.y * _CloudSpeed * 0.4;
                        float2 warp = float2(
                            valueNoise(warpUV + float2(1.7, 9.2)),
                            valueNoise(warpUV + float2(8.3, 2.8))
                        ) * 2.0 - 1.0;
                        cloudUV += warp * _CloudWarpStrength;
                    }

                    float n = cloudNoise(cloudUV);

                    // Coverage threshold with sharp edges for low-poly look
                    float cloud = saturate((n - (1.0 - _CloudCoverage)) * _CloudSharpness);

                    // Fade near horizon to avoid stretching artefacts
                    cloud *= saturate((y - 0.03) / 0.10);

                    // Color: day → sunset → night
                    half3 cloudColor = lerp(_CloudColorNight, _CloudColorDay, _DayNightBlend);
                    cloudColor = lerp(cloudColor, _CloudColorSunset, _SunsetBlend * 0.85);

                    // Soft shadow on cloud base
                    cloudColor *= lerp(0.70, 1.0, saturate(n * 3.0 - 0.5));

                    col = lerp(col, cloudColor, cloud);
                }

                // ── Stars ────────────────────────────────────────────────────────────
                if (_StarIntensity > 0.001 && y > -0.05)
                {
                    float s = starField(dir, _StarDensity);
                    col += s * _StarIntensity * (1.0 - _DayNightBlend);
                }

                return half4(col, 1.0);
            }
            ENDCG
        }
    }
}
