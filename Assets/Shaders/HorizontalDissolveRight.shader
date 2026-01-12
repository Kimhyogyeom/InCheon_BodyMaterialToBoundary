Shader "Custom/HorizontalDissolveRight"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DissolveAmount ("Dissolve Amount", Range(0, 1)) = 0
        _ContentStart ("Content Start X", Range(0, 0.5)) = 0.2  // 여백 시작점
        _ContentEnd ("Content End X", Range(0.5, 1)) = 0.8      // 여백 끝점
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _DissolveAmount;
            float _ContentStart;
            float _ContentEnd;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // UV를 실제 컨텐츠 영역으로 리매핑
                // 원래 UV: 0~1 → 컨텐츠 UV: ContentStart~ContentEnd를 0~1로 변환
                float remappedUV = (i.uv.x - _ContentStart) / (_ContentEnd - _ContentStart);
                remappedUV = saturate(remappedUV); // 0~1 범위로 클램프
                
                // 왼쪽부터 사라짐 (오른쪽만 남음)
                if (remappedUV < _DissolveAmount)
                {
                    discard; // 왼쪽 부분 제거
                }
                
                return col;
            }
            ENDCG
        }
    }
}
