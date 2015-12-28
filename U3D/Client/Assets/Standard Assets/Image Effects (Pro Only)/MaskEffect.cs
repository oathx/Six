using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/MaskEffect")]
[RequireComponent (typeof(Camera))]
public class MaskEffect : MonoBehaviour {
	
	public static float TotalChangeTime = 1.0f;
	//public Texture  textureMask;
	public Color    JustColor = Color.white;
	public float	Range = 1;
	public float    Power = 1;
	private float 	CurTime = 0.0f;
	public Material	UseMaterial;
	
	protected void Start ()
	{
		// Disable if we don't support image effects
		if (!SystemInfo.supportsImageEffects) {
			enabled = false;
			return;
		}
		
		UseMaterial.SetColor("_Color",JustColor);
		UseMaterial.SetFloat("_Range",Range);
		UseMaterial.SetFloat("_Power",Power);
	}

	// Called by camera to apply image effect
	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		CurTime += Time.deltaTime ;
		float Rate = CurTime /TotalChangeTime;
		if(Rate > 1.0f)
			Rate	= 1.0f;
		
		//if(textureMask != null)
		//	UseMaterial.SetTexture ("_MaskTex", textureMask);		

		UseMaterial.SetFloat("_Rate",Rate);
		
		Graphics.Blit (source, destination, UseMaterial);
	}
	
	protected void OnDisable() {
		CurTime	= 0.0f;
	}
}
