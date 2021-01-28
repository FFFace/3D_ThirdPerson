
Shader "Custom/test"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _BumpMap("NormalMap", 2D) = "white" {}
        _DissolveMap("DissolveMap", 2D) = "whtie" {}
        _DissolveColor("DissolveColor", Color) = (1,1,1,1)
        _DissolveAmount("DissolveAmount", Range(0,1)) = 0
        _DissolveWidth("DissolveWidth", Range(0,1)) = 0
        _AlphaTest("Alpha", Range(0,1)) = 0
        _OutlineBold("Outline Bold", Range(-1,1)) = 0.1
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "Queue" = "Geometry" "RenderType"="Transparent" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf _BandedLighting fullforwardshadows alphatest:_AlphaTest vertex:vertex
        //#pragma vertex vertex
        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _BumpMap;
        sampler2D _DissolveMap;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap;
            float2 uv_DissolveMap;
            float3 viewDir;
            float3 lightDir;
            float3 vertex;
            float3 normal;
        };

        half _Glossiness;
        half _Metallic;
        half _OutlineBold;
        half _DissolveAmount;
        half _DissolveWidth;
        fixed4 _Color;
        fixed4 _DissolveColor;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void vertex(inout appdata_base i, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.lightDir = WorldSpaceLightDir(i.vertex);
            o.vertex = UnityObjectToClipPos(i.vertex);
            o.normal = i.normal;
        }

        void surf (Input IN, inout SurfaceOutput o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            fixed4 mask = tex2D(_DissolveMap, IN.uv_DissolveMap);
            //o.Normal = UnpackScaleNormal(tex2D(_BumpMap, IN.uv_BumpMap), 0.5f);
            
            /*float x = ceil(IN.vertex * 5) / 5;
            col.x = x;*/

            float dissolve = ceil(mask.r - (_DissolveAmount + _DissolveWidth));

            float Dot = dot(IN.lightDir, IN.normal) * 0.5f + 0.5f;
            float tone = ceil(Dot * 3) / 3;

            float outline = dot(IN.viewDir, IN.normal) * 0.5f + 0.5f;

            outline -= _OutlineBold;
            outline = ceil(outline);

            //outline* tone
            o.Albedo = (c * tone * outline * dissolve) + (_DissolveColor * (ceil(mask.r) - dissolve));
            dissolve = ceil(mask.r - _DissolveAmount);
            o.Alpha = dissolve;            

            // Metallic and smoothness come from slider variables
            //o.Metallic = _Metallic;
            //o.Smoothness = _Glossiness;
            //o.Alpha = c.a;
            //o.Emission = c;
        }

        float4 Lighting_BandedLighting(SurfaceOutput s, float3 lightDir, float3 viewDir, float atten)
        {
            ////! 최종 컬러 출력
            float4 fFinalColor;
            fFinalColor.rgb = (s.Albedo);
            fFinalColor.a = s.Alpha;

            return fFinalColor;
        }

        ENDCG
    }
    FallBack "Diffuse"
}
