Shader "Unlit/uvAnim"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex("Noise", 2D) = "white" {}
        _Amount("Amount", Range(0,0.1)) = 0
        _Speed("Speed", Float) = 0
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
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float4 _MainTex_ST;
            float4 _NoiseTex_ST;

            float x;
            float y;
            half _Speed;
            half _Amount;

            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                x += unity_DeltaTime.x;
                y += -unity_DeltaTime.x;
                half timex = floor(x) / 4;
                half timey = ceil(y / 4) / 4;


                // sample the texture
                fixed4 noise = tex2D(_NoiseTex, float2(i.uv + (_Speed * _Time.y)));
               /* fixed4 col = tex2D(_MainTex, i.uv + float2(x,y));*/
                fixed4 col = tex2D(_MainTex, i.uv + (noise * _Amount) + float2(timex, timey));
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
