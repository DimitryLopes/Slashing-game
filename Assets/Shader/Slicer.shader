Shader "Unlit/Slicer"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CutPoint ("Cut Point", Vector) = (0,0,0,0)
        _CutNormal ("Cut Normal", Vector) = (1,0,0,0)
        _Side ("Side", Float) = 1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _CutPoint;
            float4 _CutNormal;
            float _Side;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 localPos : TEXCOORD1;
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.localPos = v.vertex.xyz; // 🔑 LOCAL space
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float side = dot(i.localPos.xy - _CutPoint.xy, _CutNormal.xy);
                clip(side * _Side);

                return tex2D(_MainTex, i.uv);
            }
            ENDHLSL
        }
    }
}
