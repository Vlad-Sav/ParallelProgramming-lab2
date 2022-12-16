using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelProgramming_lab2
{
    internal class Program
    {
        private static readonly Airport _airport = new Airport();
        private static Random _random = new Random();

        private static void Main(string[] args)
        {
              var thread1 = new Thread(() => { ActionsInAirport("Клиент1"); });
              thread1.Start();

              var thread2 = new Thread(() => { ActionsInAirport("Клиент2"); });
              thread2.Start();

              var thread3 = new Thread(() => { ActionsInAirport("Клиент3"); });
              thread3.Start();

              var thread4 = new Thread(() => { ActionsInAirport("Клиент4"); });
              thread4.Start();

              var thread5 = new Thread(() => { ActionsInAirport("Клиент5"); });
              thread5.Start();

            Console.ReadKey();
        }

        private static void ActionsInAirport(string threadName)
        {
            
            var cityName = Airport.Cities[_random.Next(1, Airport.Cities.Count())];
            for (int i = 0; i < 20; i++)
            {
                var flightDay = Airport.Days[_random.Next(0, Airport.Days.Count() - 1)];
            
                Console.WriteLine(threadName + $" спрашивает, есть ли рейс в {cityName} на дату {flightDay}.11.2022");
                if (_airport.AskIfThereIsTicket(cityName, flightDay))
                {
                    Console.WriteLine(threadName + $": получает ответ, что есть рейс в {cityName} на дату {flightDay}.11.2022");
                    var ticket = _airport.BuyTicket(cityName, flightDay);

                    if (string.IsNullOrEmpty(ticket))
                    {
                        Console.WriteLine(threadName + $": хочет приобрести билет на рейс в {cityName} на дату {flightDay}.11.2022, но билетов нет в наличии");
                    }
                    else
                    {
                        Console.WriteLine(threadName + $": приобретает билет на рейс в {cityName} на дату {flightDay}.11.2022");
                        break;
                    }
                }
                else
                {
                    Console.WriteLine(threadName + $": получает ответ, что рейса в {cityName} на дату {flightDay}.11.2022 нет");
                }
                
            }
        }
    }
}
