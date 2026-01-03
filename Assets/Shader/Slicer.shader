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
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float side =
                    dot(i.worldPos.xy - _CutPoint.xy, _CutNormal.xy);

                if (side * _Side < 0)
                    discard;

                return tex2D(_MainTex, i.uv);
            }
            ENDHLSL
        }
    }
}
