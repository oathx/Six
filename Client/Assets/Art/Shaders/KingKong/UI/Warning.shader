Shader "KingKong/UI/Warning"
{
	Properties
	{
		_Color("Main Color", Color) = (1, 1, 1, 1)
		_Power ("Power", Range(0, 2)) = 0.5
		_Rate ("Rate", Range(0, 2)) = 0.1
		_Multipy ("Multipy", Range(0, 5)) = 1
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
			
			fixed4 _Color;
			fixed _Power;
			fixed _Rate;
			fixed _Multipy;
	
			struct a2v
			{
				fixed4 vertex : POSITION;
				fixed2 texcoord : TEXCOORD0;
			};
			
			struct v2f
			{
				fixed4 vertex : SV_POSITION;
				fixed2 uv : TEXCOORD0;
			};
			
			v2f vert(a2v v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord;
				
				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 c = _Color;
				fixed2 uv = i.uv.xy - fixed2(0.5f, 0.5f);
				fixed a = distance(uv, fixed2(0, 0)) * 2;
				a -= _Power + abs(_SinTime.w) * _Rate;
				a *= _Multipy;

				c.a *= a;
				
				return c;
			}
			
			ENDCG
		}
	}
}