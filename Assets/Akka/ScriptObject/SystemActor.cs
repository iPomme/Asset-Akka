using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Akka.Actor;

public class SystemActor : ScriptableObject {

	private readonly ActorSystem _system = ActorSystem.Create("SystemActor");
 
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
	
}
