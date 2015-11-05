﻿Shader "KingKong/Character/Blend"
{
	Properties
	{
		_MainTex("Base(RGB) Trans(A)", 2D) = "white" {}
		_BlendMap("Gloss(R) Illum(G) Mask(B)", 2D) = "black" {}
		_BumpMap("Normalmap", 2D) = "bump" {}
		_MaskColor("Mask Color", Color) = (1, 1, 1, 1)
		_OccColor("Occlusion Color", Color) = (1, 1, 1, 1)
		_OccPower("Occlusion Power", Range(0.0, 2.0)) = 0.5
		_Alpha("Alpha", Range(0, 1)) = 1
		_Shininess("Shininess", Range(0.01, 1)) = 0.5
	}

	SubShader
	{
		LOD 200

		UsePass "KingKong/Effect/Occlusion/BASE"
		Tags
		{
			"Queue" = "Geometry+1"
			"RenderType" = "Opaque"
			"IgnoreProjector" = "True"
		}
		Blend SrcAlpha OneMinusSrcAlpha
		AlphaTest Greater 0.1

		CGPROGRAM
		#pragma surface surf CustomBlinnPhong exclude_path:prepass nolightmap noforwardadd halfasview
		
		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _BlendMap;
		fixed3 _MaskColor;
		fixed _Alpha;
		fixed _Shininess;
		
		struct Input
		{
			fixed2 uv_MainTex;
		};

		inline fixed4 LightingCustomBlinnPhong(SurfaceOutput s, fixed3 lightDir, fixed3 halfDir, fixed atten)
		{
			fixed diff = max(0, dot(s.Normal, lightDir));
			fixed nh = max(0, dot(s.Normal, halfDir));
			fixed spec = pow(nh, s.Specular * 128) * s.Gloss;

			fixed4 c;
			c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * spec) * (atten * 2);
			c.a = s.Alpha;

			return c;
		}

		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			fixed3 b = tex2D(_BlendMap, IN.uv_MainTex);

			o.Albedo = lerp(c.rgb, _MaskColor.rgb, b.b);
			o.Alpha = c.a * _Alpha;
			o.Gloss = b.r;
			o.Specular = _Shininess;
			o.Emission = b.g * c.rgb * abs(_CosTime.w);
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
		}

		ENDCG
	}

	SubShader
	{
		LOD 100

		UsePass "KingKong/Effect/Occlusion/BASE"
		Tags
		{
			"Queue" = "Geometry+1"
			"RenderType" = "Opaque"
			"IgnoreProjector" = "True"
		}
		Blend SrcAlpha OneMinusSrcAlpha
		AlphaTest Greater 0.1
		
		CGPROGRAM
		#pragma surface surf CustomLambert exclude_path:prepass nolightmap noforwardadd halfasview
		
		sampler2D _MainTex;
		sampler2D _BlendMap;
		fixed3 _MaskColor;
		fixed _Alpha;

		struct Input
		{
			fixed2 uv_MainTex;
		};

		inline fixed4 LightingCustomLambert(SurfaceOutput s, fixed3 lightDir, fixed atten)
		{
			fixed diff = max(0, dot(s.Normal, lightDir));

			fixed4 c;
			c.rgb = (s.Albedo * _LightColor0.rgb * diff) * (atten * 2);
			c.a = s.Alpha;

			return c;
		}
		
		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			fixed3 b = tex2D(_BlendMap, IN.uv_MainTex);

			o.Albedo = lerp(c.rgb, _MaskColor.rgb, b.b);
			o.Alpha = c.a * _Alpha;
			o.Emission = b.g * c.rgb * abs(_CosTime.w);
		}
		
		ENDCG
	}

	FallBack "Mobile/Diffuse"
}