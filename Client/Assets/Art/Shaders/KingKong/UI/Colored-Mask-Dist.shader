Shader "KingKong/UI/Colored-Mask-Dist"
{
	Properties
	{
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "white" {}
		_Mask ("Mask Map", 2D) = "white" {}
		_WidthRate ("Sprite.width/Atlas.width", Float) = 1
		_HeightRate ("Sprite.height/Atlas.height", Float) = 1
		_XOffset ("Sprite.offset.x", Float) = 0
		_YOffset ("Sprite.offset.y", Float) = 0
		_DistRateMin ("Dist Rate Min", Float) = 0
		_DistRateMax ("Dis Rate Max", Float) = 0
		_DistLength ("Dist Length", Float) = 0
		_DistIntensity ("Dist Intensity", int) = 0
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"RenderType" = "Transparent"
			"IgnoreProjector" = "True"
		}

		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			ZTest LEqual
			Offset -1, -1
			Fog { Mode Off }
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			sampler2D _MainTex;
			sampler2D _Mask;
			fixed4 _MainTex_ST;
			fixed4 _Mask_ST;
			fixed _WidthRate;
			fixed _HeightRate;
			fixed _XOffset;
			fixed _YOffset;
			fixed _DistRateMin;
			fixed _DistRateMax;
			fixed _DistLength;
			fixed _DistIntensity;

			struct a2v
			{
				fixed4 vertex : POSITION;
				fixed2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};

			struct v2f
			{
				fixed4 vertex : SV_POSITION;
				fixed2 uv : TEXCOORD0;
				fixed4 color : COLOR;
			};

			v2f vert(a2v v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord;
				o.color = v.color;

				return o;
			}

			int random (int min, int max)
			{
				return min + (int)(_Time.w * 100000) % (max - min);
			}

			fixed4 frag(v2f i) : SV_Target
			{
				if (random(0, 100) < _DistIntensity)
				{
					i.uv.x += sin(i.uv.y * random(_DistRateMin, _DistRateMax)) * _DistLength;
				}

				fixed4 c = tex2D(_MainTex, i.uv) * i.color;

				i.uv = fixed2((i.uv.x - _XOffset) / _WidthRate, (i.uv.y - (1 - _YOffset)) / _HeightRate);
				fixed m = tex2D(_Mask, i.uv).a;
				c.a *= m;

				return c;
			}

			ENDCG
		}
	}
}