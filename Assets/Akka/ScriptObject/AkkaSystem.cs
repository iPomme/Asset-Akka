using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Akka.Actor;
using Akka.Configuration;

public class AkkaSystem : ScriptableObject
{
    private static readonly ActorSystem _system =
        ActorSystem.Create("AkkaSystem", ConfigurationFactory.ParseString(@""));

    private static AkkaSystem _inst;

    public static AkkaSystem Instance
    {
        get
        {
            if (!_inst)
            {
                var classes = Resources.FindObjectsOfTypeAll<AkkaSystem>();
                if (classes.Length > 0)
                    _inst = classes.First();
            }

            if (!_inst)
                _inst = CreateInstance<AkkaSystem>();
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