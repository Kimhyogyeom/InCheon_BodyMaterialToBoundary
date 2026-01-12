Shader "Custom/IntenseGlowSprite"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        [Header(Intense Glow Settings)]
        _GlowColor ("Glow Color", Color) = (0, 1, 1, 1)
        _GlowIntensity ("Glow Intensity", Range(0, 20)) = 5
        _GlowPulseSpeed ("Pulse Speed", Range(0, 15)) = 3
        _GlowPulseMin ("Pulse Min", Range(0, 1)) = 0.3

        [Header(Outer Glow)]
        _OuterGlowSize ("Outer Glow Size", Range(0, 0.1)) = 0.02
        _OuterGlowIntensity ("Outer Glow Intensity", Range(0, 10)) = 3

        [Header(Bloom Effect)]
        _BloomStrength ("Bloom Strength", Range(0, 5)) = 2
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
        }

        Blend SrcAlpha OneMinusSrcAlpha  // 일반 알파 블렌딩
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;

            float4 _GlowColor;
            float _GlowIntensity;
            float _GlowPulseSpeed;
            float _GlowPulseMin;

            float _OuterGlowSize;
            float _OuterGlowIntensity;
            float _BloomStrength;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv) * i.color;

                // 강한 펄스 효과
                half pulse = lerp(_GlowPulseMin, 1.0, (sin(_Time.y * _GlowPulseSpeed) + 1.0) * 0.5);
                half pulse2 = lerp(0.7, 1.0, (sin(_Time.y * _GlowPulseSpeed * 1.7) + 1.0) * 0.5);

                // 외곽 글로우 샘플링
                half outerGlow = 0;
                float2 offsets[8] = {
                    float2(1, 0), float2(-1, 0), float2(0, 1), float2(0, -1),
                    float2(1, 1), float2(-1, 1), float2(1, -1), float2(-1, -1)
                };

                for (int j = 0; j < 8; j++)
                {
                    float2 offset = offsets[j] * _OuterGlowSize;
                    outerGlow += tex2D(_MainTex, i.uv + offset).a;
                }
                outerGlow /= 8.0;

                // 메인 야광 효과 (HDR - 1.0 이상 출력)
                half3 glow = _GlowColor.rgb * _GlowIntensity * pulse;

                // 외곽 글로우 추가
                half3 outerGlowColor = _GlowColor.rgb * outerGlow * _OuterGlowIntensity * pulse2;

                // 블룸 효과 (밝은 부분 더 밝게)
                half brightness = dot(col.rgb, half3(0.299, 0.587, 0.114));
                half3 bloom = col.rgb * brightness * _BloomStrength * pulse;

                // 최종 색상 합성 (HDR 값 허용)
                col.rgb = col.rgb + col.rgb * glow + outerGlowColor + bloom;

                return col;
            }
            ENDCG
        }
    }
}
