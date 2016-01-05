using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;
using System.IO;
using System.Xml;
using LitJson;
using System.Data;
using Mono.Data.Sqlite;
using System.Text;

public class UnitStruct : XmlStruct
{
	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="UnitStruct"/> split skin mesh render.
	/// </summary>
	/// <value><c>true</c> if split skin mesh render; otherwise, <c>false</c>.</value>
	public bool				SplitSkinMeshRender
	{ get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="UnitStruct"/> remove animation component.
	/// </summary>
	/// <value><c>true</c> if remove animation component; otherwise, <c>false</c>.</value>
	public bool				RemoveAnimationComponent
	{ get; set; }

	/// <summary>
	/// Gets or sets the path.
	/// </summary>
	/// <value>The path.</value>
	public DirectoryStruct	SkinPath
	{ get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="UnitStruct"/> class.
	/// </summary>
	public UnitStruct()
	{
		SkinPath 				= new DirectoryStruct();
		SplitSkinMeshRender		= true;
	}
}

/// <summary>
/// Zip xml struct.
/// </summary>
public class UnitXmlStruct : XmlStruct
{
	/// <summary>
	/// Gets or sets the generate clicked.
	/// </summary>
	/// <value>The generate clicked.</value>
	[ButtonField("Generate")]
	public ButtonClicked	GenerateClicked
	{ get; set; }

	/// <summary>
	/// Gets or sets the add unit clicked.
	/// </summary>
	/// <value>The add unit clicked.</value>
	[ButtonField("Add Unit")]
	public ButtonClicked	AddUnitClicked
	{ get; set; }

	/// <summary>
	/// The unit.
	/// </summary>
	public List<UnitStruct>	
		Unit = new List<UnitStruct>();

	/// <summary>
	/// Initializes a new instance of the <see cref="UnitXmlStruct"/> class.
	/// </summary>
	public UnitXmlStruct()
	{
		GenerateClicked = new ButtonClicked(OnGenerateClicked);
		AddUnitClicked	= new ButtonClicked(OnAddUnitClicked);
	}

	/// <summary>
	/// Generates the animatior.
	/// </summary>
	/// <returns>The animatior.</returns>
	/// <param name="unit">Unit.</param>
	/// <param name="szFileName">Size file name.</param>
	/// <param name="aryClip">Ary clip.</param>
	public GameObject		GenerateAnimatior(UnitStruct unit, string szFileName, List<string> aryClip, List<string> arySkin)
	{
		UnityEngine.Object resource = AssetDatabase.LoadAssetAtPath(szFileName, typeof(GameObject));
		if (!resource)
			throw new System.NullReferenceException();
		
		string[] arySplit = unit.SkinPath.Path.Split('/');
		if (arySplit.Length == 0)
			throw new System.NullReferenceException();

		string szOutPath = string.Format("{0}/{1}", unit.SkinPath.FullPath, arySplit[arySplit.Length - 1]);
		if (Directory.Exists(szOutPath))
		{
			// clear old file
			string[] aryDelete = System.IO.Directory.GetFiles(
				szOutPath, "*.*", SearchOption.AllDirectories
				);
			foreach(string delete in aryDelete)
			{
				File.Delete(delete);
			}
		}
		else
		{
			Directory.CreateDirectory(szOutPath);
		}
		
		// extract skeleton
		string szSkeletonPath = string.Format("{0}/{1}.prefab", 
		                                      szOutPath, arySplit[arySplit.Length - 1]);
		GameObject skeleton = XmlEditorHelper.ExtractSkeleton(resource, szSkeletonPath, unit.SplitSkinMeshRender);
		if (!skeleton)
			throw new System.NullReferenceException();

		// create the unit animator
		XmlEditorHelper.CreateAnimator(skeleton, aryClip, szFileName);

		// extract skin mesh
		if (unit.SplitSkinMeshRender && arySkin.Count > 0)
		{
			// extract skin
			List<UnityEngine.Object> 
				aryResource = new List<UnityEngine.Object>();
			foreach(string skin in arySkin)
			{
				aryResource.Add
					(AssetDatabase.LoadAssetAtPath(skin, typeof(GameObject))
					 );
			}
			
			// extract skin mesh
			string szMeshPath = string.Format("{0}/{1}", 
			                                  szOutPath, arySplit[arySplit.Length - 1]);
			XmlEditorHelper.ExtractSkinMesh(
				aryResource.ToArray(), szOutPath
				);
		}

		return skeleton;

	}

	/// <summary>
	/// Invalidate the specified animator and arySkin.
	/// </summary>
	/// <param name="animator">Animator.</param>
	/// <param name="arySkin">Ary skin.</param>
	public void 			Invalidate(Animator refAnimatior, List<string> arySkin)
	{ 
		foreach(string skin in arySkin)
		{
			if (skin.ToLower().Contains(SearchFileType.prefab.ToString()))
			{
				// Extract the model skeleton
				UnityEngine.GameObject resource = AssetDatabase.LoadAssetAtPath(skin, typeof(GameObject)) as UnityEngine.GameObject;
				if (!resource)
					throw new System.NullReferenceException();
				
				Animator animator = resource.GetComponent<Animator>();
				if (!animator)
					animator = resource.AddComponent<Animator>();
				
				animator.runtimeAnimatorController 	= refAnimatior.runtimeAnimatorController;
				animator.avatar						= refAnimatior.avatar;
				
				Animation old = resource.GetComponent<Animation>();
				if (old)
					GameObject.DestroyImmediate(old, true);
			}
		}
	}
	
	/// <summary>
	/// Raises the generate clicked event.
	/// </summary>
	/// <param name="target">Target.</param>
	/// <param name="p">P.</param>
	public void 			OnGenerateClicked(object target, PropertyInfo p)
	{
		foreach(UnitStruct unit in Unit)
		{
			List<string> 
				arySkin = new List<string>();
			
			List<string> 
				aryClip = new List<string>();

			foreach(string cur in unit.SkinPath.FileArray)
			{
				if (cur.Contains("@"))
				{
					aryClip.Add(cur);
				}
				else
				{
					arySkin.Add(cur);
				}
			}

			// if current have a skin
			if (arySkin.Count > 0)
			{
				GameObject skeleton = default(GameObject);

				foreach(string skin in arySkin)
				{
					string x = skin.ToLower();
					if (skin.ToLower().Contains(SearchFileType.fbx.ToString()))
					{
						skeleton = GenerateAnimatior(unit, skin, aryClip, arySkin);
						break;
					}
				}

				if (skeleton)
				{
					Animator refAnimatior = skeleton.GetComponent<Animator>();
					if (!refAnimatior)
						throw new System.NullReferenceException();

					Invalidate(refAnimatior, arySkin);
				}
			}
		}
	}

	/// <summary>
	/// Raises the add unit clicked event.
	/// </summary>
	/// <param name="target">Target.</param>
	/// <param name="p">P.</param>
	public void 			OnAddUnitClicked(object target, PropertyInfo p)
	{
		Unit.Add(new UnitStruct());
	}
}

/// <summary>
/// Zip xml editor.
/// </summary>
public class UnitXmlEditor : XmlEditorWindow<UnitXmlStruct>
{
	[MenuItem("Custom/Unit Window")]
	public static void CreateXmlEditor()
	{
		UnitXmlEditor window = EditorWindow.GetWindow<UnitXmlEditor> ();
		if (window) 
		{
			window.Show();
			window.Focus();
		}
	}
}



