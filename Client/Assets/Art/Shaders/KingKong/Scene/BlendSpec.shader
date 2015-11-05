Shader "KingKong/Scene/BlendSpec"
{
	Properties
	{
		_MainTex("Base(RGB)", 2D) = "white" {}
		_BlendMap("Gloss(R) Illum(G)", 2D) = "black" {}
		_Shininess("Shininess", Range(0.01, 1)) = 0.078125
	}

	SubShader
	{
		LOD 200

		Tags
		{
			"RenderType" = "Opaque"
			"IgnoreProjector" = "True"
		}

		CGPROGRAM
		#pragma surface surf CustomBlinnPhong exclude_path:prepass nolightmap noforwardadd halfasview

		sampler2D _MainTex;
		sampler2D _BlendMap;
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

			o.Albedo = c.rgb;
			o.Alpha = c.a;
			o.Gloss = b.r;
			o.Specular = _Shininess;
			o.Emission = b.g * c.rgb * abs(_CosTime.w);
		}

		ENDCG
	}
	
	SubShader
	{
		LOD 100

		Tags
		{
			"RenderType" = "Opaque"
			"IgnoreProjector" = "True"
		}
		
		CGPROGRAM
		#pragma surface surf CustomLambert exclude_path:prepass nolightmap noforwardadd halfasview

		sampler2D _MainTex;
		sampler2D _BlendMap;
		fixed _Shininess;

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

			o.Albedo = c.rgb;
			o.Alpha = c.a;
			o.Emission = b.g * c.rgb * abs(_CosTime.w);
		}
		
		ENDCG
	}

	FallBack "Mobile/Diffuse"
}