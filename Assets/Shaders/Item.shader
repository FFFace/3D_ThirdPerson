Shader "Custom/Item"
{
    Properties
    {
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		//_NormalMap("Normal Map", 2D) = "bump" {}
		_NormalStrenght("Normal Strength", Range(0, 1.5)) = 0.5
		_Color("Color", Color) = (1,1,1,1)
    }
    SubShader
	{
			Tags { "Queue" = "Geometry" }
			cull back
			CGPROGRAM
			#pragma surface surf _BandedLighting fullforwardshadows   //! 커스텀 라이트 사용
			#pragma target 3.0

			sampler2D _MainTex;
			//sampler2D _NormalMap;

			half _NormalStrenght;
			fixed4 _Color;

			struct Input
			{
				float2 uv_MainTex;
				//float2 uv_NormalMap;
			};

			void surf(Input IN, inout SurfaceOutput o)
			{
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

				o.Albedo = c;
				o.Emission = c;

				o.Alpha = _Color.a;
				//o.Normal = UnpackScaleNormal(tex2D(_NormalMap, IN.uv_NormalMap), _NormalStrenght);
			}

			//! 커스텀 라이트 함수
			float4 Lighting_BandedLighting(SurfaceOutput s, float3 lightDir, float3 viewDir, float atten)
			{
				//! BandedDiffuse 조명 처리 연산
				float3 fBandedDiffuse;
				float fNDotL = dot(s.Normal, lightDir) * 0.5f + 0.5f;    //! Half Lambert 공식

				//! 0~1로 이루어진 fNDotL값을 3개의 값으로 고정함 <- Banded Lighting 작업
				float fBandNum = 3.0f;
				fBandedDiffuse = ceil(fNDotL * fBandNum) / fBandNum;


				//! 최종 컬러 출력
				float4 fFinalColor;
				//fFinalColor.rgb = (s.Albedo) * fBandedDiffuse * atten * _Color.rgb * _LightColor0.rgb;
				fFinalColor.rgb = (1, 0, 0);
				fFinalColor.a = s.Alpha;

				return fFinalColor;
			}
			ENDCG
	}
    FallBack "Diffuse"
}
