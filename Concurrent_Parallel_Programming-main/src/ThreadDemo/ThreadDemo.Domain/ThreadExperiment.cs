using Microsoft.VisualBasic;
using System.ComponentModel.Design;
using System.Diagnostics.Metrics;

namespace ThreadDemo.Domain;

public class ThreadExperiments
{
    private static int counter = 0;
    private static object gate = new();

    public void RunSequential()
    {
        Console.WriteLine("Esecuzione sequenziale");
        work("a");
        work("b");
    }
    //funzione per simulare del lavoro
    private static void work(string name)
    {
        for (int i = 0; i < 5; i++) 
        {
            Console.WriteLine($"{name}; Thread: {Thread.CurrentThread.ManagedThreadId}; index {i}");
            Thread.Sleep(100);
        }
        
    }
    
    public void ShowThreadProperties()
    {
        Thread currentThread = Thread.CurrentThread;
        Console.WriteLine($"Thread ID: {currentThread.ManagedThreadId}");
        Console.WriteLine($"Thread Name: {currentThread.Name}");
        Console.WriteLine($"Priority: {currentThread.Priority}");
        Console.WriteLine($"Is Background: {currentThread.IsBackground}");
        
    }
    //avere una risorsa condivisa su cui piu thread devono agire. può essere pericoloso
    //in queanto potrebbero esserci problemi sull'aggiornamento della variabile condivisa
    //1° test i thread contano sulla stesssa variabile condivisa senza sincronizzazione
    //2° test i thread contano sulla stesssa variabile condivisa con sincronizzazione
    //il metodo ritorna una tupla (expexted,acrual)
    public (int expected, int actual) RunCounter(bool useLock, int iteration= 100_000)
    {
        
        counter = 0;

        Thread t1 = new Thread(() => Increment(iteration, useLock));
        Thread t2 = new Thread(() => Increment(iteration, useLock));

        t1.Start();
        t2.Start();

        t1.Join();
        t2.Join();

        return (expected: iteration * 2, actual: counter);

    }

    void Increment(int iteration, bool useLock)
    {
        for (int i = 0; i < iteration; i++)
        {
            //mi serve un accesso in mutua esclusione per ottenere uan soluzione deterministica
            if (useLock)
            {
                lock (gate)
                {
                    counter++;
                }
            }
            else
                counter++;
        }
    }


    //private static void IncrementCounter()
    //{
    //    for (int i = 0; i <=
    //         iteration; i++) 
    //    {

        //    }
        //}

    public void runbackgroundThread()
    {
        Thread backgroundThread = new Thread(() =>
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine($"Background thread: {i}");
                Thread.Sleep(500);
            }
        });
        backgroundThread.IsBackground = true;
        backgroundThread.Start();
        Console.WriteLine("Main thread is doing some work...");
        Thread.Sleep(2000);
        Console.WriteLine("Main thread is exiting.");
    }


    public void runThreads()
    {
        int milliseconds = 5000;
        //lambda expression per creare un thread che esegue la funzione work con parametro "a"
        Thread thread1 = new Thread(() => work("A")) 
        {

            Name = "Thread A"

        };
        Thread thread2 = new Thread(() => work("B")) 
        {

            Name = "Thread B"

        };
        
        Console.WriteLine("Starting Thread...");
        
        //THIS IS A NON DETERMINISTIC RUN
        thread1.Start();
        Thread.Sleep(milliseconds);
        thread2.Start();

        Console.WriteLine("This line is written by main thread");

        //this is a deterministic run
        thread1.Join();//attende che il thread1 finisca prima di continuare
        thread2.Join();
    }
}
