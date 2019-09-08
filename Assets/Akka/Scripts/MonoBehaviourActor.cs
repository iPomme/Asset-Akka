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
            system = AkkaSystem.Instance.GetActorSystem();
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
            switch (message)
            {
                case CreateChildActor createRequest:
                    Debug.LogFormat("{0} creating child actor: {1}", Self.Path, createRequest.ChildName);
                    var child = Context.ActorOf(createRequest.ChildProps, createRequest.ChildName);
                    createRequest.ActorToNotify.ForEach(a => a.Tell(new ReadyForWork(child, createRequest.ChildName)));
                    break;
                default:
                    _unity.OnReceive(message, Sender);
                    break;
            }
            
        }

        public static Props Props(string name, MonoBehaviourActor unity)
        {
            return Akka.Actor.Props.Create(() => new InternalActor(name, unity));
        }
    }
    
    #region Messages
    /// <summary>
    /// Command to create ChildProps actor and notify another actor about it.
    /// </summary>
    public class CreateChildActor
    {
        public CreateChildActor(string childName, Props childProps, Option<IActorRef> actorToNotify)
        {
            ChildName = childName;
            ActorToNotify = actorToNotify;
            ChildProps = childProps;
        }

        public CreateChildActor(string childName, Props childProps)
            : this(childName, childProps, new None())
        {
        }

        public Props ChildProps { get; private set; }
        public string ChildName { get; private set; }
        public Option<IActorRef> ActorToNotify { get; private set; }
    }
    /// <summary>
    /// Report to another actor when ready.
    /// </summary>
    public class ReadyForWork
    {
        public ReadyForWork(IActorRef worker, string name)
        {
            Worker = worker;
            Name = name;
        }

        public IActorRef Worker { get; private set; }
        public string Name { get; private set; }
    }
    #endregion
}