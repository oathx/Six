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
	/// Gets or sets a value indicating whether this <see cref="UnitXmlStruct"/> out skeleton.
	/// </summary>
	/// <value><c>true</c> if out skeleton; otherwise, <c>false</c>.</value>
	public bool				OutSkeleton
	{ get; set; }
	
	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="UnitXmlStruct"/> out skin.
	/// </summary>
	/// <value><c>true</c> if out skin; otherwise, <c>false</c>.</value>
	public bool				OutSkin
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
		SkinPath 	= new DirectoryStruct();
		OutSkin		= true;
		OutSkeleton	= true;
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
				// Extract the model skeleton
				UnityEngine.Object resource = AssetDatabase.LoadAssetAtPath(arySkin[0], typeof(GameObject));
				if (!resource)
					throw new System.NullReferenceException();
		
				string[] arySplit = unit.SkinPath.Path.Split('/');
				if (arySplit.Length != 0)
				{
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
					GameObject skeleton = XmlEditorHelper.ExtractSkeleton(resource, szSkeletonPath);
					if (skeleton)
					{
						XmlEditorHelper.CreateAnimator(skeleton, aryClip, arySkin[0]);
					}

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



