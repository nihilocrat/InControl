using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using InControl;
using System.IO;


namespace InControl
{
	public class DeviceProfileSerializer
	{
		protected static string DeviceProfilesPath
		{
			get
			{
				return Application.dataPath + "/../";
			}
		}

		static public UnityInputDeviceProfile GetCustomProfile(string joystickName)
		{
			if(FileExists("controls.ini"))
			{
				return Load("controls.ini", joystickName);
			}

			return null;
		}
		
		static public bool FileExists(string filename)
		{
			return File.Exists(DeviceProfilesPath + filename);
		}
		
		static public UnityInputDeviceProfile Load(string profileFile, string joystickName)
		{
			var data = IniFile.LoadFile(profileFile);

			if(!(bool)data["General"]["Enabled"])
			{
				return null;
			}

			// use the unknown device defaults
			var profile = new UnityUnknownDeviceProfile(joystickName);
			profile.Name = "Custom Profile: " + profileFile;
			
			profile.Sensitivity = (float)data["General"]["Sensitivity"];
			profile.LowerDeadZone = (float)data["General"]["LowerDeadZone"];
			profile.UpperDeadZone = (float)data["General"]["UpperDeadZone"];

			var analogs = data["AnalogMappings"];
			var buttons = data["ButtonMappings"];
			//Hashtable analogs = data["AnalogMappings"] as Hashtable;
			//Hashtable buttons = data["ButtonMappings"] as Hashtable;

			//var analogs = data.AnalogMappings;
			//var buttons = data.ButtonMappings;

			profile.AnalogMappings = new InputControlMapping[analogs.Count];

			int index;
			index = 0;
			foreach(DictionaryEntry entry in analogs)
			{
				string targetName = entry.Key.ToString();
				string sourceName = entry.Value.ToString();
				profile.AnalogMappings[index] = CreateMapping(targetName, targetName, sourceName);

				index += 1;
			}

			index = 0;
			foreach(DictionaryEntry entry in buttons)
			{
				string targetName = entry.Key.ToString();
				string sourceName = entry.Value.ToString();
				profile.ButtonMappings[index] = CreateMapping(targetName, targetName, sourceName);

				index += 1;
			}

			return profile;
		}
		

		protected static InputControlMapping CreateMapping(string handleName, string targetName, string sourceName)
		{
			bool isAnalog = sourceName.StartsWith("Analog");

			InputControlType targetEnum = (InputControlType)System.Enum.Parse(typeof(InputControlType), targetName);

			InputControlSource source;
			if(isAnalog)
			{
				source = CreateAnalog(sourceName);
			}
			else
			{
				source = CreateButton(sourceName);
			}


			var mapping = new InputControlMapping
			{
				Handle = handleName,
				Target = targetEnum,
				Source = source
			};

			return mapping;
		}

		protected static InputControlSource CreateAnalog( string analogName )
		{
			string indexString = analogName.Substring(analogName.Length-1);
			int index = int.Parse(indexString);
			
			return new UnityAnalogSource( index );
		}
		
		protected static InputControlSource CreateButton( string buttonName )
		{
			string indexString = buttonName.Substring(buttonName.Length-1);
			int index = int.Parse(indexString);
			
			return new UnityButtonSource( index );
		}	
	}
}