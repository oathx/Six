Shader "KingKong/Effect/Rim"
{
	Properties
	{
		_BumpMap("Normalmap", 2D) = "bump" {}
		_RimColor("Rim Color", Color) =(0, 0, 0, 0)
		_RimPower("Rim Power", Range(0, 1)) = 1
	}
	
	SubShader
	{
		Tags
		{
			"Queue" = "Transparent+1"
			"RenderType" = "Opaque"
			"IgnoreProjector" = "True"
		}

		Pass
		{
			Name "BASE"

			Blend SrcAlpha OneMinusSrcAlpha
			Fog { Mode Off }
			Lighting Off
			ZWrite Off

			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			sampler2D _BumpMap;
			fixed4 _RimColor;
			fixed _RimPower;

			struct a2v
			{
				fixed4 vertex : POSITION;
				fixed3 normal : NORMAL;
				fixed4 tangent : TANGENT;
				fixed4 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				fixed4 pos : SV_POSITION;
				fixed2 uv : TEXCOORD0;
				fixed3 viewDir : TEXCOORD1;
			};

			v2f vert(a2v v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord.xy;

				TANGENT_SPACE_ROTATION;
				o.viewDir = normalize(mul(rotation, ObjSpaceViewDir(v.vertex)));

				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target
			{
				fixed3 n = UnpackNormal(tex2D(_BumpMap, i.uv));
				fixed rim = 1 - max(0, dot(n, i.viewDir));
				fixed4 c = _RimColor * pow(rim, _RimPower);

				return c;
			}

			ENDCG
		}
	}

	FallBack "Mobile/Diffuse"
}