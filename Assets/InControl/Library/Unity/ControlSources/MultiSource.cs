using System;
using UnityEngine;


namespace InControl
{
	public class MultiSource : InputControlSource
	{
		InputControlSource[] sources;


		public MultiSource( params InputControlSource[] sources )
		{
			this.sources = sources;
		}


		public override float GetValue( InputDevice inputDevice )
		{
			// CAVEAT: multiple input sources could be giving values, but we only want one
			//   thus, we'll take the first one in the list
			//   so the user can control priority simply by ordering the list
			foreach(InputControlSource source in sources)
			{
				var thisValue = source.GetValue( inputDevice );
				if(thisValue != 0f)
				{
					return thisValue;
				}
			}

			return 0f;
		}


		public override bool GetState( InputDevice inputDevice )
		{
			foreach(InputControlSource source in sources)
			{
				if(source.GetState( inputDevice ))
				{
					return true;
				}
			}

			return false;
		}
	}
}

