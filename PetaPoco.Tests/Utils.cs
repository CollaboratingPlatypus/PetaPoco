using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PetaPoco.Tests
{
	class Utils
	{
		public static string LoadTextResource(string name)
		{
			// get a reference to the current assembly
			var a = System.Reflection.Assembly.GetExecutingAssembly();
			System.IO.StreamReader r = new System.IO.StreamReader(a.GetManifestResourceStream(name));
			string str = r.ReadToEnd();
			r.Close();

			return str;
		}

	    public static void SetDataDirectoryPath()
	    {
            var executable = Assembly.GetExecutingAssembly().Location;
            var path = Path.GetDirectoryName(executable);

            AppDomain.CurrentDomain.SetData("DataDirectory", path);
	    }
	}
}
