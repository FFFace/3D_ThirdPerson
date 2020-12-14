Shader "Custom/trail"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_DissolveTex("Dissovle", 2D) = "white" {}
		_DissolveColor("DissolveColor", Color) = (1,1,1,1)
		_DissolveAmount("DissolveAmount", Range(0,1)) = 0
		_DissolveEmission("DissolveEmission", Range(0,1)) = 1
		_DissolveWidth("DissolveWidth", Range(0,0.1)) = 0.05
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
		sampler2D _DissolveTex;

        struct Input
        {
            float2 uv_MainTex;
			float2 uv_DissolveTex;
        };

		half _DissolveAmount;
		half _DissolveEmission;
		half _DissolveWidth;
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
		fixed4 _DissolveColor;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			fixed4 d = tex2D(_DissolveTex, IN.uv_DissolveTex);
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

			if (d.r < _DissolveAmount) discard;

			o.Albedo = c.rgb;
			o.Emission = c;

			if (d.r < _DissolveAmount + _DissolveWidth)
			{
				o.Albedo = _DissolveColor;
				o.Emission = _DissolveColor * _DissolveEmission;
			}

            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
