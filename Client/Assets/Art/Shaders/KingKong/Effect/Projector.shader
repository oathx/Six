Shader "KingKong/Effect/Projector"
{
	Properties
	{
		_ShadowTex("Shadow Texture", 2D) = "black" { TexGen ObjectLinear }
		_FalloffTex("FallOff", 2D) = "" { TexGen ObjectLinear }
	}

	Subshader
	{
		Tags
		{
			"Queue" = "Transparent+1"
			"RenderType" = "Opaque"
		}

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			AlphaTest Greater 0
			Fog{ Mode Off }
			Lighting Off
			ZWrite Off
			Offset -1, -1

			SetTexture [_ShadowTex]
			{
				constantColor(0, 0, 0, 1)
				combine texture * constant
				Matrix [_Projector]
			}

			SetTexture [_FalloffTex]
			{
				constantColor(0, 0, 0, 0)
				combine previous lerp(texture) constant
				Matrix [_ProjectorClip]
			}
		}
	}
}
