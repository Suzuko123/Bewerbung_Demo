Shader "Test_Shader/Fog"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Intensitaet("_Intensitaet", Range( 0 , 100)) = 0.4197272
		_MaxIntensitaet("_MaxIntensitaet", Range( 0 , 1)) = 0
		_Emission("_Emission", Color) = (0.2058824,0.2058824,0.2058824,0)
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma surface surf Standard alpha:fade keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 
		struct Input
		{
			float4 screenPos;
		};

		uniform float4 _Emission;
		uniform sampler2D _CameraDepthTexture;
		uniform float _Intensitaet;
		uniform float _MaxIntensitaet;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Emission = _Emission.rgb;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPos3 = ase_screenPos;
			float eyeDepth1 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos3))));
			o.Alpha = clamp( ( abs( ( eyeDepth1 - ase_screenPos3.w ) ) * (0.005 + (_Intensitaet - 0.0) * (0.02 - 0.005) / (1.0 - 0.0)) ) , 0.0 , _MaxIntensitaet );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}