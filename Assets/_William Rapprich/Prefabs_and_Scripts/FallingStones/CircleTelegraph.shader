//Author: William Rapprich
//Last edited: 7.12.2017 by: William
Shader "Telegraph/CircleTelegraph"
{
	Properties
	{
		_Color ("Color", Color) = (1.0, 0.0, 0.0, 0.3)
		_FillPercent("Fill percent", Range(0,1)) = 0
	}
	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent"}
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Back
		ZWrite Off
		Lighting Off

		Pass
		{
			Stencil
			{
				Ref 200
				Comp greater
				Pass keep
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 screenUV : TEXCOORD1;
				float3 ray : TEXCOORD2;
			};

			fixed4 _Color;
			fixed _FillPercent;
			sampler2D_float _CameraDepthTexture;
			
			//http://theorangeduck.com/page/avoiding-shader-conditionals
			fixed or(fixed a, fixed b)
			{
				return min(a + b, 1.0);
			}
			fixed and(fixed a, fixed b)
			{
				return a * b;
			}
			fixed greater(half x, half y)
			{
				return max(sign(x - y), 0.0);
			}
			fixed less(half x, half y)
			{
				return max(sign(y - x), 0.0);
			}

			v2f vert (appdata v)
			{
				v2f o;

				o.pos = UnityObjectToClipPos(v.vertex);
				o.screenUV = ComputeScreenPos(o.pos);
				o.ray = UnityObjectToViewPos(v.vertex).xyz * float3(-1,-1,1);

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				i.ray *= (_ProjectionParams.z / i.ray.z);
				float depth = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.screenUV.xy / i.screenUV.w));
				
				float4 vpos = float4(i.ray * depth,1);
				float3 wpos = mul (unity_CameraToWorld, vpos).xyz;
				float3 opos = mul (unity_WorldToObject, float4(wpos,1)).xyz;

				clip(float3(0.5, 0.5, 0.5) - abs(opos));

				_FillPercent *= 0.5;
				half dist = length(opos.xz);
				return _Color * or( less(dist, _FillPercent), and(less(dist, 0.5), greater(dist, 0.48)) );
			}
			ENDCG
		}
	}
}
