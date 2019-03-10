using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Lab2
{
    class Program
    {
        static AutoResetEvent[] waitHandles1 =
               {
                    new AutoResetEvent(false),
                    new AutoResetEvent(false),
               };

        static AutoResetEvent[] waitHandles2 =
               {
                    new AutoResetEvent(false),
                    new AutoResetEvent(false),
               };
        static AutoResetEvent[] go =
            {
                new AutoResetEvent(false),
                new AutoResetEvent(false)
            };

        static void Main(string[] args)
        {

         
            var thread = new List<Thread>();

            Thread one = new Thread(() => Wait(go, "1"));
            Thread two = new Thread(() => { Wait(waitHandles1, "2"); go[0].Set(); });
            Thread three = new Thread(() => { Wait(waitHandles2, "3"); go[1].Set(); });
            Thread four = new Thread(() => {Console.WriteLine("4"); waitHandles2[0].Set();});
            Thread seven = new Thread(() => { Console.WriteLine("7"); waitHandles2[1].Set(); });
            Thread five = new Thread(() => { Console.WriteLine("5"); waitHandles1[1].Set(); });
            Thread six = new Thread(() => { Console.WriteLine("6"); waitHandles1[0].Set(); });
            

            thread.Add(five);
            thread.Add(six);
            thread.Add(two);
            thread.Add(four);
            thread.Add(seven);
            thread.Add(three);
            thread.Add(one);


            foreach (Thread x in thread)
            {
                x.Start();
            }
          
            Console.ReadKey();
        }
 
        static void Wait(AutoResetEvent[] var, string number)
        {
            WaitHandle.WaitAll(var);
            Console.WriteLine(number);
        }      
    }
}