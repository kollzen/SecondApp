using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Second
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
            int readerCount = 10;
            int writerCount = 3;
            
            Task[] readers = new Task[readerCount];
            for (int i = 0; i < readerCount; i++)
            {
                readers[i] = Task.Run(() =>
                {
                    for (int j = 0; j < 5; j++) 
                    {
                        int value = Server.GetCount();
                        Console.WriteLine($"Читатель {Task.CurrentId}: значение count = {value}");
                        Thread.Sleep(100); 
                    }
                });
            }

            // Задачи для писателей
            Task[] writers = new Task[writerCount];
            for (int i = 0; i < writerCount; i++)
            {
                writers[i] = Task.Run(() =>
                {
                    for (int j = 0; j < 5; j++) 
                    {
                        int increment = new Random().Next(1, 10);
                        Server.AddToCount(increment);
                        Console.WriteLine($"Писатель {Task.CurrentId}: добавил {increment}, новое значение count = {Server.GetCount()}");
                        Thread.Sleep(150); 
                    }
                });
            }

            // Ожидаем завершения всех задач
            Task.WaitAll(readers);
            Task.WaitAll(writers);

            Console.WriteLine("Все задачи завершены");
            Console.WriteLine($"Итоговое значение count = {Server.GetCount()}");

        }
        public static class Server {
            private static int count = 0;
            private static ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();

            // Метод для чтения значения переменной count
            public static int GetCount()
            {
                rwLock.EnterReadLock(); 
                try
                {
                    return count;
                }
                finally
                {
                    rwLock.ExitReadLock(); 
                }
            }

            // Метод для добавления значения к переменной count
            public static void AddToCount(int value)
            {
                rwLock.EnterWriteLock(); 
                try
                {
                    count += value;
                }
                finally
                {
                    rwLock.ExitWriteLock(); 
                }
            }
        } 
    }
}

