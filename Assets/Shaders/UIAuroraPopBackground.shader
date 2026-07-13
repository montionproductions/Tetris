Shader "UI/Aurora Pop Background"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _DeepColor ("Deep Color", Color) = (0.035,0.012,0.16,1)
        _VioletColor ("Violet Color", Color) = (0.31,0.055,0.62,1)
        _CyanColor ("Cyan Color", Color) = (0.04,0.78,1,1)
        _PinkColor ("Pink Color", Color) = (1,0.12,0.68,1)
        _Speed ("Animation Speed", Range(0,2)) = 0.22
        _AuroraStrength ("Aurora Strength", Range(0,2)) = 0.85
        _SparkleStrength ("Sparkle Strength", Range(0,1)) = 0.28
        _Vignette ("Vignette", Range(0,1)) = 0.48

        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "AuroraPop"

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed4 _DeepColor;
            fixed4 _VioletColor;
            fixed4 _CyanColor;
            fixed4 _PinkColor;
            float _Speed;
            float _AuroraStrength;
            float _SparkleStrength;
            float _Vignette;
            float4 _ClipRect;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.worldPosition = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color * _Color;
                return o;
            }

            float hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float time = _Time.y * _Speed;

                float vertical = smoothstep(0.0, 1.0, uv.y);
                fixed3 color = lerp(_DeepColor.rgb, _VioletColor.rgb, vertical * 0.58);

                float waveA = sin(uv.x * 5.2 + time + sin(uv.y * 3.1 - time * 0.7));
                float bandA = exp(-24.0 * abs(uv.y - (0.57 + waveA * 0.12)));
                float waveB = sin(uv.x * 4.0 - time * 0.65 + 1.7);
                float bandB = exp(-31.0 * abs(uv.y - (0.34 + waveB * 0.10)));
                color += _CyanColor.rgb * bandA * _AuroraStrength * 0.52;
                color += _PinkColor.rgb * bandB * _AuroraStrength * 0.42;

                float2 starGrid = floor(uv * float2(34.0, 52.0));
                float starSeed = hash21(starGrid);
                float2 starUV = frac(uv * float2(34.0, 52.0)) - 0.5;
                float star = smoothstep(0.09, 0.0, length(starUV));
                star *= step(0.91, starSeed);
                star *= 0.55 + 0.45 * sin(time * 4.0 + starSeed * 18.0);
                color += lerp(_CyanColor.rgb, _PinkColor.rgb, starSeed) * star * _SparkleStrength;

                float2 centered = uv * 2.0 - 1.0;
                float vignette = smoothstep(1.2, 0.25, dot(centered, centered));
                color *= lerp(1.0 - _Vignette, 1.0, vignette);

                fixed textureAlpha = tex2D(_MainTex, uv).a;
                fixed4 result = fixed4(color, textureAlpha * i.color.a);

                #ifdef UNITY_UI_CLIP_RECT
                    result.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                    clip(result.a - 0.001);
                #endif

                return result;
            }
            ENDCG
        }
    }
}
