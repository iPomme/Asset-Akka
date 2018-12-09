using Akka.Actor;
using System;

namespace Jorand.AkkaUnity
{
    public interface AkkaCommand
    {
    }

    public interface AkkaEvent
    {
        string getValue();
    }

    public class Register : AkkaCommand
    {
        public IActorRef actorRef { get; private set; }
        public string evt { get; }

        public Register(IActorRef actor, string eventType)
        {
            actorRef = actor;
            evt = eventType;
        }
    }

    public class UnRegister : AkkaCommand
    {
        private IActorRef _actor;
        public string evt { get; }

        public UnRegister(IActorRef actor, string eventType)
        {
            _actor = actor;
            evt = eventType;
        }
    }

    public class PrintListeners : AkkaCommand
    {
    }


    [Serializable]
    public class DebugMessage : AkkaEvent
    {
        public string msg { get; private set; }


        public DebugMessage(string m)
        {
            msg = m;
        }

        public string getValue()
        {
            return msg;
        }
    }
}