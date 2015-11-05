// yy36day谢谢你的支持，希望你会喜欢，改得不好请多原谅。//
//感谢www.cgjoy.com平台//
Shader "FX/Displacement01" {
Properties {
	    _DispMap("Displacement Map ", 2D) = "white"{}
		_MaskTex (" Mask ", 2D) = "white" {}
		_StrengthX ("Displacement Strength X", Float) =  1
		_StrengthY ("Displacement Strength X", Float) = -1	
	}
Category{
    Tags{"Queue"="Transparent+99" "RenderType" ="Transparent"}
    Blend SrcAlpha OneMinusSrcAlpha
    Cull Off Lighting Off ZWrite Off ZTest Always
    BindChannels {
        Bind "Color", color
        Bind "Vertex", vertex
        Bind "TexCoord", texcoord
        }  
	SubShader {
	    GrabPass{
	        Name "BASE"
		    Tags { "lightMode" = "Always" }
		}
		
		Pass {
		     Name "Base"
		     Tags {"LightMode" = "Always"}

		
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"


struct appdata_t{
        float4 vertex : POSITION;
        fixed4 color: COLOR;
        float2 texcoord: TEXCOORD0;
        float2 param: TEXCOORD1;
};

struct v2f {
        float4 vertex :POSITION;
        fixed4 color: COLOR;
        float2 uvmain: TEXCOORD0;
        float2 param: TEXCOORD1;
        float4 uvgrab: TEXCOORD2;
};
uniform half _StrengthX;
uniform half _StrengthY;
uniform float4 _DispMap_ST;
uniform sampler2D _DispMap;
uniform sampler2D _MaskTex;
v2f vert (appdata_t v)
{
  v2f o;
  o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
  #if UNITY_UV_STARTS_AT_TOP
  float scale = -1.0;
  #else
  float scale = 1.0;
  #endif
  o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
  o.uvgrab.zw = o.vertex.zw;
  o.uvmain = TRANSFORM_TEX( v.texcoord, _DispMap );
  o.color = v.color;
  o.param = v.param;
  return o;
}
sampler2D _GrabTexture;
half4 frag( v2f i ):COLOR
{
  half4 offsetColor = tex2D(_DispMap, i.uvmain);
  half oftX = offsetColor.a * _StrengthX * i.param.x;
  half oftY = offsetColor.a * _StrengthY * i.param.x;
  i.uvgrab.x += oftX;
  i.uvgrab.y += oftY;
  half4 col = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.uvgrab));
  col.a = i.color.a;
  fixed4 tint = tex2D(_MaskTex, i.uvmain);
  col.a *= tint.a;
  return col;
}
ENDCG
		}
	} 
	SubShader{
	Blend SrcAlpha OneMinusSrcAlpha
	Pass{
	    Name "BASE"
	    SetTexture [_MainTex] {combine texture * primary double, texture * primary}
	}
	}
	}
}
