using System;
using Proto;
using UnityEngine;

namespace Assets.Scripts
{
    public class ActorSystem : MonoBehaviour
    {
        public static readonly RootContext Context = new RootContext();

        private void Start()
        {
            var props = Props.FromProducer(() => new UIManagerActor());
            var actor = Context.SpawnNamed(props, "UIManagerActor");
            UnityEngine.Debug.Log("PingPong Starting ....");
//            var ret = Context.RequestAsync<object>(actor, new Hello() {Who = "Nicolas"});
            // Create the ping and the pong actor
            var pongerActor = Context.Spawn(Props.FromProducer(() =>
                new PingPongActor(new Messages.Ping(), new Messages.Pong(), actor)));
            var pingerActor = Context.Spawn(Props.FromProducer(() =>
                new PingPongActor(new Messages.Pong(), new Messages.Ping(), actor)));
            
            Context.Request(pingerActor, new Messages.Pong(), pongerActor);
            
        }
    }
}