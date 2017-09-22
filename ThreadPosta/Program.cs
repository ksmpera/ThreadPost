using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadPosta
{
    class Program
    {
        static SemaphoreSlim PostOffice = new SemaphoreSlim(3);
        static Dictionary<Thread, int> Citizens = new Dictionary<Thread, int>();
        static void Main(string[] args)
        {
            bool postistworking = true;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Postu otvoriti pritiskom space.");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Svaki pojedinacni gradjanin ulazi sa enter.");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Kraj rada poste pritiskom escape.");
            bool postopened = false;
            int i = 0;

            while (postistworking)
            {
                ConsoleKeyInfo keypress = Console.ReadKey();
                if (keypress.Key == ConsoleKey.Spacebar)
                {
                    if (!postopened)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Posta je otvorena.");
                        postopened = true;
                    }
                }
                if (keypress.Key == ConsoleKey.Enter)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("gradjanin broj {0} zeli da udje u postu.", ++i);
                    Citizens.Add(new Thread(EnterinPost), i);
                }
                if (keypress.Key == ConsoleKey.Escape)
                {
                    postopened = false;
                    foreach (var j in Citizens)
                    {
                        if (j.Key.IsAlive || j.Key.ThreadState == ThreadState.Running)
                        {
                            j.Key.Join();
                        }
                    }
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(" Posta je zatvorena. ");
                    postistworking = true;
                }

                if (postopened)
                {
                    foreach (var j in Citizens)
                    {
                        if (j.Key.ThreadState == ThreadState.Unstarted)
                        {
                            j.Key.Start();
                        }
                    }
                }
                Thread.Sleep(100);
            }
            Console.ReadLine();
        }
        static void EnterinPost()
        {
            int i;
            Citizens.TryGetValue(Thread.CurrentThread, out i);
            PostOffice.Wait();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("gradjanin broj " + i + " je usao.");
            Thread.Sleep(1000 * i);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("gradjanin broj " + i + " je izasao.");
            PostOffice.Release();
        }
    }
}
