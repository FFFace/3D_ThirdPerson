Shader "Unlit/cartoon"
{
	Properties
	{
		_Outline_Bold("Outline_Bold", Range(0, 10)) = 1
		_OutLineColor("Outline_Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_NormalMap("Normal Map", 2D) = "bump" {}
		_NormalStrenght("Normal Strength", Range(0, 1.5)) = 0.5
		_DissolveMap("Dissolve Map", 2D) = "white" {}
		_DissolveAmount("DissolveAmount", Range(0,1)) = 0
		_DissolveColor("DissolveColor", Color) = (1,1,1,1)
		_DissolveEmission("DissolveEmission", Range(0,1)) = 1
		_DissolveWidth("DissolveWidth", Range(0,0.1)) = 0.05
		_Color("Color", Color) = (1,1,1,1)
		_Bright("Bright", Range(0, 1)) = 0
		_Specular("Specular", Range(0,1)) = 0.0
		_SpecularColor("Specular Color", Color) = (1,1,1,1)
	}

		SubShader
		{
			Tags {"Queue" = "Transparent" "RenderType" = "Opaque"}

			Pass
			{
				Tags {"Queue" = "Overlay" "LightMode" = "Always" }
				Cull front
				ZWrite Off
				ZTest Always
				ColorMask RGB

				Blend DstColor Zero

				Stencil
				{
					ref 2
					Comp always
					Pass Replace
					Zfail keep
					Fail keep
				}

				CGPROGRAM
				#pragma vertex _VertexFuc
				#pragma fragment _FragmentFuc
				#include "UnityCG.cginc"

				sampler2D _DissolveMap;
				float4 _DissolveMap_ST;
				fixed4 _OutLineColor;

				float _Outline_Bold;
				half _DissolveAmount;

				struct ST_VertexInput
				{
					float4 vertex : POSITION;
					float3 normal : NORMAL;
					float2 uv : TEXCOORD0;
				};

				struct ST_VertexOutput
				{
					float4 vertex : SV_POSITION;
					float3 normal : NORMAL;
					float2 uv : TEXCOORD0;
				};

				ST_VertexOutput _VertexFuc(ST_VertexInput stInput)
				{
					ST_VertexOutput stOutput;

					stOutput.vertex = UnityObjectToClipPos(stInput.vertex);

					float3 fNormalized_Normal = normalize(stInput.normal);
					float3 fOutline_Position = stInput.vertex + fNormalized_Normal * (_Outline_Bold * 0.1f) * stOutput.vertex.w;

					stOutput.vertex = UnityObjectToClipPos(fOutline_Position);
					stOutput.uv = TRANSFORM_TEX(stInput.uv, _DissolveMap);
					stOutput.normal = UnityObjectToWorldNormal(stInput.normal);

					return stOutput;
				}

				fixed4 _FragmentFuc(ST_VertexOutput i) : SV_Target
				{
					fixed4 col = tex2D(_DissolveMap, i.uv);
					if (col.r < _DissolveAmount) discard;
					return _OutLineColor;
				}
				ENDCG
			}
			Tags { "RenderType" = "Opaque" }
			cull back
			Stencil
			{
				ref 2
				Comp Equal
				Pass keep
				Zfail keep
				Fail decrWrap
			}
			CGPROGRAM
			#pragma surface surf _BandedLighting fullforwardshadows  //! 커스텀 라이트 사용
			#pragma target 3.0

			sampler2D _MainTex;
			sampler2D _NormalMap;
			sampler2D _DissolveMap;

			half _DissolveAmount;
			half _NormalStrenght;
			half _DissolveEmission;
			half _DissolveWidth;
			half _Bright;
			half _Glossiness;
			half _Specular;
			fixed4 _Color;
			fixed4 _DissolveColor;
			fixed4 _SpecularColor;

			struct Input
			{
				float2 uv_MainTex;
				float2 uv_NormalMap;
				float2 uv_DissolveMap;
			};

			void surf(Input IN, inout SurfaceOutput o)
			{
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				fixed4 mask = tex2D(_DissolveMap, IN.uv_DissolveMap);

				if (mask.r < _DissolveAmount)
					discard;

				o.Albedo = c;
				o.Emission = c * _Bright;
				o.Specular = _Specular;
				o.Gloss = _Glossiness;

				if (mask.r < _DissolveAmount + _DissolveWidth) {
					o.Albedo = _DissolveColor;
					o.Emission = _DissolveColor * _DissolveEmission;
				}

				o.Alpha = _Color.a;
				o.Normal = UnpackScaleNormal(tex2D(_NormalMap, IN.uv_NormalMap), _NormalStrenght);
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

				//float3 fSpecularColor;
				//float3 fReflectVector = reflect(-lightDir, s.Normal);
				//float fRDotV = saturate(dot(fReflectVector, viewDir));
				//fSpecularColor = pow(fRDotV, _Specular) * _SpecularColor.rgb;

				//float3 fSpecular;
				//float3 fHalfVector = normalize(lightDir + viewDir);
				//float fHDotN = saturate(dot(fHalfVector, s.Normal));
				//fSpecular = pow(fHDotN, _Specular);

				float3 fSpecular;
				float3 fHalfVector = normalize(lightDir + viewDir);
				float fHDotN = saturate(dot(fHalfVector, s.Normal));
				fSpecular = pow(fHDotN, _Specular);

				//! 최종 컬러 출력
				float4 fFinalColor;
				fFinalColor.rgb = (s.Albedo) * (fBandedDiffuse + fSpecular) * _Color.rgb *_LightColor0.rgb;
				fFinalColor.a = s.Alpha;

				return fFinalColor;
			}
			ENDCG
		}
}
		