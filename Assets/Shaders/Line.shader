
Shader "Custom/XLockBillborad"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _LightningTime1("LightningTime1", Range(0,1)) = 1
        _LightningTime2("LightningTime2", Range(0,1)) = 0
        _Bright("Bright", Range(0, 1)) = 0
    }
        SubShader
        {
            Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane"}
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            LOD 100

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_fog

                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                    float3 normal : NORMAL;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                half _LightningTime1;
                half _LightningTime2;
                half _Bright;

                v2f vert(appdata v)
                {
                    v2f o;

                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    float4 pos = v.vertex;
                    pos.y = 0;

                    pos = mul(UNITY_MATRIX_MV, pos);

                    pos.y += v.vertex.y * UNITY_MATRIX_M._m11;

                    o.vertex = mul(UNITY_MATRIX_P, pos);

                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 col = tex2D(_MainTex, i.uv) * _Bright;

                    return col;
                }
                ENDCG
            }
        }
}
