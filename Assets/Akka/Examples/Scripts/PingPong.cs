using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Akka.Actor;
using Akka.Configuration;
using Akka.Event;

public class PingPong : MonoBehaviour
{
    private readonly ActorSystem _system = ActorSystem.Create("PingPong");


    // Use this for initialization
    void Start()
    {
        var ponger = _system.ActorOf(PingPongActor.Props("ping", "pong"), "ponger");
        var pinger = _system.ActorOf(PingPongActor.Props("pong", "ping"), "pinger");

        pinger.Tell("pong", ponger);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnDisable()
    {
        _system.Terminate();
    }
}


public class PingPongActor : UntypedActor
{
    private readonly string _matcher;
    private readonly string _answer;

    private int count = 0;

    public PingPongActor(string matcher, string answer)
    {
        _matcher = matcher;
        _answer = answer;
        UnityEngine.Debug.LogWarning("Starting " + matcher + " ==>" + answer);
    }

    protected override void OnReceive(object message)
    {
        if (message.ToString() == _matcher)
        {
            Sender.Tell(_answer);
            count += 1;
            if (count % 100000 == 0)
                UnityEngine.Debug.Log(count.ToString("N0") + ": received " + message.ToString() + " answering " + _answer);
        }
    }

    public static Props Props(string matcher, string answer)
    {
        return Akka.Actor.Props.Create(() => new PingPongActor(matcher, answer));
    }
}