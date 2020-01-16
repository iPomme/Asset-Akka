using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Assets.Scripts;
using LanguageExt;
using Proto;
using Unity.Profiling;


public class Messages
{
    public interface IPingPong
    {
    }

    public class Ping : IPingPong
    {
        internal string name;

        public Ping()
        {
            this.name = "Ping";
        }
    }

    public class Pong : IPingPong
    {
        internal string name;

        public Pong()
        {
            this.name = "Pong";
        }
    }
}


public class PingPongActor : IActor
{
//    static ProfilerMarker s_PreparePerfMarker = new ProfilerMarker("AKKAUnity.OnReceive");

    private readonly Messages.IPingPong _matcher;
    private readonly Messages.IPingPong _answer;


    private int count = 0;
    private Option<PID> uiManager;

    private Stopwatch timeMessages = Stopwatch.StartNew();

    public PingPongActor(Messages.IPingPong matcher, Messages.IPingPong answer, PID uiManagerPath)
    {
        _matcher = matcher;
        _answer = answer;
        uiManager = uiManagerPath;
        UnityEngine.Debug.LogWarning("Starting " + matcher + " ==>" + answer);
    }

    public Task ReceiveAsync(IContext context)
    {
//        s_PreparePerfMarker.Begin();
//        UnityEngine.Debug.Log(Thread.CurrentThread.ManagedThreadId + ": received from '" +
//                              "' the message: " + context.Message?.ToString());

        if (context.Message.GetType() == _matcher.GetType())
        {
            context.Request(context.Sender, _answer);
            count += 1;
            if (count % 100000 == 0)
            {
                UnityEngine.Debug.Log(Thread.CurrentThread.ManagedThreadId + ": " + count.ToString("N0") +
                                      ": received " + context.Message?.ToString() + " answering " +
                                      _answer);

                int time = timeMessages.Elapsed.Milliseconds;
                timeMessages = Stopwatch.StartNew();
                UIMessages.SpeedInfo msg = new UIMessages.SpeedInfo(100000 / time * 1000);
                UnityEngine.Debug.LogFormat("about to send {0} to the UIManager {1}", msg, uiManager);
                uiManager.IfSome(a => a.Tell(msg));
            }
        }

//        s_PreparePerfMarker.End();
        return Actor.Done;
    }
}