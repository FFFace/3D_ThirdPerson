Shader "Unlit/ItemShpere"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    _RimColor("Rim Color", Color) = (1, 1, 1, 1)
        _Color("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
         blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
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
                //float4 posWorld : TEXCOORD1;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 color : COLOR;
                //float3 normal : NORMAL;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed4 _RimColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
                float dotProduct = 1 - dot(v.normal, viewDir);
                float rimWidth = 0.7;
                o.color = smoothstep(1 - rimWidth, 1.0, dotProduct);

                o.color *= _RimColor;

                UNITY_TRANSFER_FOG(o,o.vertex);
                //o.normal = UnityObjectToWorldNormal(v.normal);
                //o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture
                //fixed4 col = tex2D(_MainTex, i.uv) *_Color;
                // apply fog
                //float num1 = (cos(i.uv.y * 3.141592 * 2)+1)*0.5;
                //float num2 = sin(i.uv.y * 3.141592);
                //col.rgb = col.rgb + num1/2;
                //col.a = num2;

               /* float viewDir = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
            float rim = 1 - saturate(dot(viewDir, i.normal));*/

                //fixed4 col = (1, 1, 1,1);
                //col.rbg *= rim;
                //UNITY_APPLY_FOG(i.fogCoord, col);
                //return col;

                    float4 texcol = tex2D(_MainTex, i.uv);
                    texcol *= _Color;
                    texcol.rgb += i.color;
                    return texcol;
            }
            ENDCG
        }
    }
}
