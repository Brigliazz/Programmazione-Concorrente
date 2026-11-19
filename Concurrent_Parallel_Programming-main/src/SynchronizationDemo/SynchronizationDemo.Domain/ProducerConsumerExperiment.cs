namespace SynchronizationDemo.Domain
{
    public class ProducerConsumerExperiment
    {
        private const int BufferSize = 4; // grandezza della tavola in cui mettere i piatti con il cibo

        private const int NumProducers = 2; // numero di produttori

        private const int NumConsumers = 2; // numero di consumatori


        public void run()
        {

            //mi serve una struttura dati per posizionare i prodotti creati e consumati
            Queue<int> buffer = new();

            var spaziDisponibili = new SemaphoreSlim(BufferSize, BufferSize); // semaforo per i produttori
            var elementiDisponibili = new SemaphoreSlim(0, BufferSize); // semaforo per i consumatori
            var accessoTavola = new SemaphoreSlim(1, 1); // semaforo per l'accesso alla tavola in maniera mutualmente esclusiva
            
            Thread[] produttori = new Thread[NumProducers];
            Thread[] consumatori = new Thread[NumConsumers];

            int nextId = 0;

            //TODO: implementazione dei metodi per produrre e consumare

            void ConsumeSomeProduct(int consumerId)
            {
                
            }

            void ProduceSomeProduct(int producerId)
            {
                

                //per sempre finche il tread non viene killato o fermato per altri motivi di syncronizzation
                while (true)
                {
                    //per evitare il fatto che più thread creino item identici devo garantire l'unicità
                    int foodItem = Interlocked.Increment(ref nextId); // Incrementa in modo atomico l'ID del prodotto

                    spaziDisponibili.Wait(); // aspetta che ci sia spazio disponibile nel buffer

                    try
                    {
                        try
                        {
                            accessoTavola.Wait(); // se ho questo permesso posso accedere alla tavola
                            buffer.Enqueue(foodItem); // aggiunge un prodotto alla tavola
                            // Interlocked.Increment(ref nextId); // Incrementa in modo atomico l'ID del prodotto
                            Console.WriteLine($"ProducerId: [{producerId}],item: [{foodItem}], bufferCount: [{buffer.Count}]");
                        }
                        finally
                        {
                            accessoTavola.Release(); // rilascia il permesso per accedere alla tavola
                        }
                    }
                    catch
                    {

                        spaziDisponibili.Release(); // Rilascia il permesso per lo spazio disponibile in caso di eccezione

                    }

                    //segnalare che è possibile d'ora in poi poter accedere a nuovi fooditems ai consumatori
                }
            }

            for (int i = 1; i < NumConsumers; i++)
            {
                //i corrisponde all'id del tread corrente
                produttori[i] = new Thread(() => ConsumeSomeProduct(i));
                produttori[i].Start();
            }

            for (int i = 1; i < NumProducers; i++)
            {
                //i corrisponde all'id del tread corrente
                produttori[i] = new Thread(() => ProduceSomeProduct(i));
                produttori[i].Start();
            }

            foreach (Thread consumatore in consumatori)
            {
                consumatore.Join();
            }

            foreach (Thread produttore in produttori)
            {
                produttore.Join();
            }


        }
    }
}
