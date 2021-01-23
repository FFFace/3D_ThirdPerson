// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/test"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _BumpMap("NormalMap", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf _BandedLighting fullforwardshadows vertex:vertex
        //#pragma vertex vertex
        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _BumpMap;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap;
            float3 viewDir;
            float3 lightDir;
            float3 vertex;
            float3 normal;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

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
            //o.Normal = UnpackScaleNormal(tex2D(_BumpMap, IN.uv_BumpMap), 0.5f);
            
            /*float x = ceil(IN.vertex * 5) / 5;
            col.x = x;*/

            float Dot = dot(IN.lightDir, IN.normal);
            float cel = ceil(Dot * 2) / 2;
            //float ol = saturate(dot(IN.viewDir, IN.normal));
            //ol = ceil(ol * 2) / 2;

            //float cel = ceil(Dot * 3) / 3;

            o.Albedo = c * cel;
            // Metallic and smoothness come from slider variables
            //o.Metallic = _Metallic;
            //o.Smoothness = _Glossiness;
            //o.Alpha = c.a;
            //o.Emission = c;
        }

        float4 Lighting_BandedLighting(SurfaceOutput s, float3 lightDir, float3 viewDir, float atten)
        {
            //! BandedDiffuse 조명 처리 연산
            //float3 fBandedDiffuse;
            //float fNDotL = dot(s.Normal, lightDir) * 0.5f + 0.5f;    //! Half Lambert 공식

            ////! 0~1로 이루어진 fNDotL값을 3개의 값으로 고정함 <- Banded Lighting 작업
            //float fBandNum = 3.0f;
            //fBandedDiffuse = ceil(fNDotL * fBandNum) / fBandNum;

            //float3 fSpecularColor;
            //float3 fReflectVector = reflect(lightDir, s.Normal);
            //float fRDotV = saturate(dot(fReflectVector, viewDir));
            //fSpecularColor = pow(fRDotV, _Specular) * _SpecularColor.rgb;

            //float3 fSpecular;
            //float3 fHalfVector = normalize(lightDir + viewDir);
            //float fHDotN = saturate(dot(fHalfVector, s.Normal));
            //fSpecular = pow(fHDotN, _Specular);

            //float3 fSpecular;
            //float3 fHalfVector = normalize(lightDir + viewDir);
            //float fHDotN = dot(fHalfVector, s.Normal);
            //fSpecular = pow(fHDotN, _Specular);

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
