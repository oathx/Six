using UnityEngine;
using System.Collections;

public class WUrl
{
	public static string	Url
	{
		get{
#if UNITY_STANDALONE || UNITY_EDITOR
			return "file://" + Application.dataPath;
#elif UNITY_ANDROID
			return	"jar:file://" + Application.persistentDataPath;
#elif UNITY_IPHONE
			return "file://" + Application.persistentDataPath;
#endif
		}
	}
}
