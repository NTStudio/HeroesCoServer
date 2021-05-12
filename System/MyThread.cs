using System;
using System.Threading;

namespace Server
{
    public delegate void Execute();
    public class MyThread
    {
        public event Execute Execute;
        Thread T;
        DateTime DT;
        uint interval;
        bool closed = false;

        public void Start(uint Interval)
        {
            interval = Interval;
            T = new Thread(new ThreadStart(Run));
            T.Start();
            Program.ThreaderCount++;
        }
        public void Close()
        {
            closed = true;
        }
        void Run()
        {
            if (closed)
                return;
            while (true)
            {
                if (DateTime.Now > DT.AddMilliseconds(interval))
                {
                    DT = DateTime.Now;
                    try
                    {
                        Execute.Invoke();
                    }
                    catch { }
                    
                }
                Thread.Sleep(100);
            }
        }
    }
}
