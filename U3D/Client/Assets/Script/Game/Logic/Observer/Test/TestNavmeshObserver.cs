using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using org.critterai.nav;
using org.critterai.nav.u3d;

/// <summary>
/// Logic plugin.
/// </summary>
public class TestNavmeshObserver : IEventObserver
{
	/// <summary>
	/// Raises the render object event.
	/// </summary>
	protected void OnRenderObject ()
	{
		NavGroup group = SceneSupport.GetSingleton().GetNavGroup();
		if (group.mesh != default(Navmesh))
		{
			NavDebug.Draw(group.mesh, true);
		}
	}
}
