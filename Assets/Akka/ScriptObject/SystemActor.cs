using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Akka.Actor;
using Akka.Configuration;

public class SystemActor : ScriptableObject {

	private static readonly ActorSystem _system = ActorSystem.Create("SystemActor", ConfigurationFactory.ParseString(@""));
 
	private static SystemActor _inst;

	public static SystemActor Instance
	{
		get
		{
			if (!_inst)
			{
				var classes = Resources.FindObjectsOfTypeAll<SystemActor>();
				if (classes.Length > 0)
					_inst = classes.First();
			}

			if (!_inst)
				_inst = CreateInstance<SystemActor>();
			return _inst;
		}
	}

	public ActorSystem GetActorSystem()
	{
		return _system;
	}
	
	public void OnDestroy()
	{
		Debug.Log("Destrowing the system Actor ....");
		_system.Terminate();
	}
}
