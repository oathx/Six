Shader "Pixelstudio/Vertex Blend"
{
	properties {
		
		_SpecPower1 ("Specular power Splat1", Range(0.0,2.0)) = 1
		_SpecPower2 ("Specular power Splat2", Range(0.0,2.0)) = 1
		_SpecPower3 ("Specular power Splat3", Range(0.0,2.0)) = 1
		_SpecPower4 ("Specular power Splat4", Range(0.0,2.0)) = 1
		_SpecColor ("Specular Color", Color) = (1,1,1,1)
		
		_Shininess ("Shininess", Range(0.0,2.0)) = 1
		
		_Splat1 ("Texture 1 (R) (A = Spec)", 2D) = "white"{}
		_Splat1bump ("Texture 1 (R) bump", 2D) = "grey"{}
		_Splat2 ("Texture 2 (G) (A = Spec)", 2D) = "white"{}
		_Splat2bump ("Texture 2 (R) bump", 2D) = "grey"{}
		_Splat3 ("Texture 3 (B) (A = Spec)", 2D) = "white"{}
		_Splat3bump ("Texture 3 (R) bump", 2D) = "grey"{}
		_Splat4 ("Texture 4 (A) (A = Spec)", 2D) = "white"{}
		_Splat4bump ("Texture 4 (R) bump", 2D) = "grey"{}
	}
	
	subshader {
		Tags {"SplatCount" = "4" "RenderType" = "Opaque" }
		
		CGPROGRAM
		#pragma surface surf BlinnPhong vertex:vert 
		#pragma target 3.0
		
		
		sampler2D _Splat1;
		sampler2D _Splat2;
		sampler2D _Splat3;
		sampler2D _Splat4;
		
		sampler2D _Splat1bump;
		sampler2D _Splat2bump;
		sampler2D _Splat3bump;
		sampler2D _Splat4bump;
		
		float _SpecPower1;
		float _SpecPower2;
		float _SpecPower3;
		float _SpecPower4;
		float _Shininess;
		
		struct Input {
        	float2 uv_Splat1;
        	float2 uv_Splat2;
        	float2 uv_Splat3;
        	float2 uv_Splat4;
        	float4 vertexColor;
	    };
	    
	    void vert (inout appdata_full v, out Input o) {
	        o.vertexColor = v.color;
	    }
			
		void surf (Input IN, inout SurfaceOutput o) {
			
			float4 splat_control = IN.vertexColor;
			fixed3 albedo;
			float gloss;
			float2 uv1 = IN.uv_Splat1;
			float2 uv2 = IN.uv_Splat2;
			float2 uv3 = IN.uv_Splat3;
			float2 uv4 = IN.uv_Splat4;
			
			float4 Splat1 = tex2D(_Splat1, uv1);
			float4 Splat2 = tex2D(_Splat2, uv2);
			float4 Splat3 = tex2D(_Splat3, uv3);
			float4 Splat4 = tex2D(_Splat4, uv4);
			
			float4 tempNormal;
			float3 normal;
			
			// diffuse color
			albedo = Splat1.rgb * splat_control.r;
			albedo += Splat2.rgb * splat_control.g;
			albedo += Splat3.rgb * splat_control.b;
			albedo += Splat4.rgb * splat_control.a;
			
			//normals
			tempNormal = tex2D (_Splat1bump, uv1)  * splat_control.r;
			tempNormal += tex2D (_Splat2bump, uv2) * splat_control.g;
			tempNormal += tex2D (_Splat3bump, uv3) * splat_control.b;
			tempNormal += tex2D (_Splat4bump, uv4) * splat_control.a;
			normal = UnpackNormal(tempNormal);
			
			// specular based on alpha of the textures
			gloss = Splat1.a * splat_control.r * _SpecPower1;
			gloss += Splat2.a * splat_control.g * _SpecPower2;
			gloss += Splat3.a * splat_control.b * _SpecPower3;
			gloss += Splat4.a * splat_control.a * _SpecPower4;
			
			
			
			o.Normal = normal;
			o.Albedo = albedo;
			o.Gloss = gloss;
			o.Specular = _Shininess;
		}
		
		ENDCG
	}
	Fallback "Diffuse"
}