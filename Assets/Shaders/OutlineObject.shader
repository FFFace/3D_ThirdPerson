Shader "Custom/OutlineObject"
{
	Properties
	{
		_Outline_Bold("Outline_Bold", Range(0, 10)) = 1
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		//_NormalMap("Normal Map", 2D) = "bump" {}
		//_NormalStrenght("Normal Strength", Range(0, 1.5)) = 0.5
		_Color("Color", Color) = (1,1,1,1)
		_Bright("Bright", Color) = (1,1,1,1)
	}

		SubShader
		{
		//	Tags { "Queue" = "Transparent" "RenderType" = "Opaque"}
		//	//blend SrcAlpha OneMinusSrcAlpha
		//	cull front
		//	Pass
		//	{
		//		CGPROGRAM
		//		#pragma vertex _VertexFuc
		//		#pragma fragment _FragmentFuc
		//		#include "UnityCG.cginc"

		//	float _Outline_Bold;
		//	fixed4 _Color;
		//	struct ST_VertexInput
		//	{
		//		float4 vertex : POSITION;
		//		float3 normal : NORMAL;
		//	};

		//	struct ST_VertexOutput
		//	{
		//		float4 vertex : SV_POSITION;
		//		float3 normal : NORMAL;
		//	};

		//	ST_VertexOutput _VertexFuc(ST_VertexInput stInput)
		//	{
		//		ST_VertexOutput stOutput;

		//		stOutput.vertex = UnityObjectToClipPos(stInput.vertex);

		//		float3 fNormalized_Normal = normalize(stInput.normal);
		//		float3 fOutline_Position = stInput.vertex + fNormalized_Normal * (_Outline_Bold * 0.1f) * stOutput.vertex.w;

		//		stOutput.vertex = UnityObjectToClipPos(fOutline_Position);
		//		stOutput.normal = UnityObjectToWorldNormal(stInput.normal);

		//		return stOutput;
		//	}

		//	fixed4 _FragmentFuc(ST_VertexOutput i) : SV_Target
		//	{
		//		fixed4 col = (0,0,0,1);
		//		col.a = _Color.a;
		//		return 0.0;
		//	}
		//	ENDCG
		//}

		Tags { "Queue" = "Transparent" "RenderType" = "Opaque"}
		cull back
		blend SrcAlpha OneMinusSrcAlpha
		CGPROGRAM
			#pragma surface surf _BandedLighting noforwardadd noambient keepalpha //! 커스텀 라이트 사용
			#pragma target 3.0

			sampler2D _MainTex;
			//sampler2D _NormalMap;

			half _NormalStrenght;
			fixed4 _Color;
			fixed4 _Bright;
			struct Input
			{
				float2 uv_MainTex;
				//float2 uv_NormalMap;
			};

			void surf(Input IN, inout SurfaceOutput o)
			{
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex); //* _Color;

				o.Albedo = c;
				o.Emission = c * _Bright;
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
				fFinalColor.rgb = (s.Albedo) * fBandedDiffuse * atten * _Color.rgb * _LightColor0.rgb;
				fFinalColor.a = s.Alpha;
				return fFinalColor;
			}
			ENDCG
		}
}
