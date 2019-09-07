using System;
using UnityEngine;
using Akka.Actor;
using Akka.Util.Internal;
using Akka;
using UnityEngine.SceneManagement;
using LaYumba.Functional;
using LaYumba.Functional.Option;
using static LaYumba.Functional.F;


namespace Jorand.AkkaUnity
{
    public abstract class MonoBehaviourActor : MonoBehaviour
    {
        protected ActorSystem system;

        public abstract void OnReceive(object message, IActorRef sender);

        protected IActorRef internalActor;


        public MonoBehaviourActor()
        {
        }


        protected void Start(string name)
        {
            string nameWithScene = SceneManager.GetActiveScene().name + "-" + name;
            string nameCleaned =
                nameWithScene.Replace(" ", "").Replace("(", "_").Replace(")", "_"); // TODO: improve the cleanup
            Debug.Log(nameCleaned + " Starting ....");
            system = SystemActor.Instance.GetActorSystem();
            internalActor = system.ActorOf(InternalActor.Props(this.GetType().Name, this), nameCleaned);
            UnityThread.initUnityThread();
        }

        protected void OnDisable()
        {
            Debug.Log(this.name + " OnDisable ....");
//            system.Terminate();
        }

        private void OnDestroy()
        {
            internalActor.Tell(PoisonPill.Instance);
        }
        
        
        public static Option<IActorRef> getActorRef(string path, ActorSystem system)
        {
            try
            {
                return Some(system.ActorSelection(path).ResolveOne(TimeSpan.FromSeconds(2)).Result);
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString() + ": Cannot ge the reference to the actor with path: ");
                return new None();
            }
        }
    }

    public class InternalActor : UntypedActor
    {
        private string _name;
        private MonoBehaviourActor _unity;

        public InternalActor(string name, MonoBehaviourActor unity)
        {
            UnityEngine.Debug.Log("Starting Actor " + name);
            _name = name;
            _unity = unity;
        }

        protected override void OnReceive(object message)
        {
            _unity.OnReceive(message, Sender);
        }

        public static Props Props(string name, MonoBehaviourActor unity)
        {
            return Akka.Actor.Props.Create(() => new InternalActor(name, unity));
        }
        
       
    }

}