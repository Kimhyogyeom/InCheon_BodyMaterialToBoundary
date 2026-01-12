Shader "Custom/ScrollingSprite"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _ScrollX ("Scroll X", Float) = 0
        _ScrollY ("Scroll Y", Float) = 0

        [Header(Glow Settings)]
        _GlowColor ("Glow Color", Color) = (0, 1, 1, 1)
        _GlowIntensity ("Glow Intensity", Range(0, 5)) = 1.5
        _GlowPulseSpeed ("Pulse Speed", Range(0, 10)) = 2
        _GlowPulseMin ("Pulse Min", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
        }

        Blend SrcAlpha OneMinusSrcAlpha
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
            float _ScrollX;
            float _ScrollY;

            float4 _GlowColor;
            float _GlowIntensity;
            float _GlowPulseSpeed;
            float _GlowPulseMin;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv.x += _ScrollX;
                o.uv.y += _ScrollY;
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;

                // 펄스 효과 (시간에 따라 밝기 변화)
                float pulse = lerp(_GlowPulseMin, 1.0, (sin(_Time.y * _GlowPulseSpeed) + 1.0) * 0.5);

                // 야광 효과 추가
                float3 glow = _GlowColor.rgb * _GlowIntensity * pulse;
                col.rgb += col.rgb * glow;

                return col;
            }
            ENDCG
        }
    }
}
