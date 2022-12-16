using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ParallelProgramming_lab2
{
    public class Airport
    {
        private readonly List<Ticket> _ticketsInAirport;
        public static Dictionary<int, string> Cities = new Dictionary<int, string>
        {
            { 1, "Москва" },
            { 2, "Грозный" },
            { 3, "Новосибирск" },
            { 4, "Красноярск" },
            { 5, "Екатеринбург" },
            { 6, "Санкт-Петербург" }
        };
        public static List<int> Days = new List<int>
        {
            10, 11, 12, 13, 14, 15
        };
        public static int Month = 11;
        public static int Year = 2022;
        private int _readers;
        private int _waitingReaders;
        private int _waitingWriters;

        //0 = false, 1=true
        private int _isWriting;

        private readonly object _canRead = new object();
        private readonly object _canWrite = new object();

        public Airport()
        {
            _ticketsInAirport = new List<Ticket>
            {
                new Ticket
                {
                    Id = 1,
                    Date = new System.DateTime(Year, Month, Days[1]),
                    Arrival = Cities[5],
                    NumberOfSeatsLeft = 3
                },
                new Ticket
                {
                    Id = 2,
                    Date = new System.DateTime(Year, Month, Days[3]),
                    Arrival = Cities[5],
                    NumberOfSeatsLeft = 5
                },
                new Ticket
                {
                    Id = 3,
                    Date = new System.DateTime(Year, Month, Days[5]),
                    Arrival = Cities[5],
                    NumberOfSeatsLeft = 8
                },
                new Ticket
                {
                    Id = 1,
                    Date = new System.DateTime(Year, Month, Days[0]),
                    Arrival = Cities[1],
                    NumberOfSeatsLeft = 2
                },
                new Ticket
                {
                    Id = 1,
                    Date = new System.DateTime(Year, Month, Days[4]),
                    Arrival = Cities[1],
                    NumberOfSeatsLeft = 10
                },
                new Ticket
                {
                    Id = 1,
                    Date = new System.DateTime(Year, Month, Days[3]),
                    Arrival = Cities[4],
                    NumberOfSeatsLeft = 10
                },
                new Ticket
                {
                    Id = 1,
                    Date = new System.DateTime(Year, Month, Days[2]),
                    Arrival = Cities[6],
                    NumberOfSeatsLeft = 10
                },
                new Ticket
                {
                    Id = 1,
                    Date = new System.DateTime(Year, Month, Days[4]),
                    Arrival = Cities[6],
                    NumberOfSeatsLeft = 10
                },
                new Ticket
                {
                    Id = 1,
                    Date = new System.DateTime(Year, Month, Days[3]),
                    Arrival = Cities[3],
                    NumberOfSeatsLeft = 10
                },
                new Ticket
                {
                    Id = 1,
                    Date = new System.DateTime(Year, Month, Days[0]),
                    Arrival = Cities[2],
                    NumberOfSeatsLeft = 10
                },

            };
        }

        private void StartReading()
        {
            if (Interlocked.Add(ref _isWriting, 0) == 1 || Interlocked.Add(ref _waitingWriters, 0) > 0)
            {
                Interlocked.Increment(ref _waitingReaders);
                lock (_canRead) Monitor.Wait(_canRead);
                Interlocked.Decrement(ref _waitingReaders);
            }

            Interlocked.Increment(ref _readers);
            lock (_canRead) Monitor.Pulse(_canRead);
        }

        private void EndReading()
        {
            if (Interlocked.Decrement(ref _readers) == 0)
            {
                lock (_canWrite) Monitor.Pulse(_canWrite);
            }
        }

        private void StartWriting()
        {
            if (Interlocked.Add(ref _readers, 0) > 0 || Interlocked.Add(ref _isWriting, 0) == 1)
            {
                Interlocked.Increment(ref _waitingWriters);
                lock (_canWrite) Monitor.Wait(_canWrite);
                Interlocked.Decrement(ref _waitingWriters);
            }

            Interlocked.CompareExchange(ref _isWriting, 1, 0);
        }

        private void EndWriting()
        {
            Interlocked.CompareExchange(ref _isWriting, 0, 1);
            if(Interlocked.Add(ref _waitingReaders, 0) > 0) lock(_canRead) Monitor.Pulse(_canRead);
            else lock(_canWrite) Monitor.Pulse(_canWrite);
        }

        public bool AskIfThereIsTicket(string city, int day)
        {
            StartReading();
            var ticket = new Ticket { Arrival = city, Date = new System.DateTime(Year, Month, day) };
            var desiredTicket = _ticketsInAirport.FirstOrDefault(book => book.Equals(ticket));
            bool isThere = false;
            if(desiredTicket == null)
            {
                isThere = false;
            }
            else if(desiredTicket.NumberOfSeatsLeft == 0)
            {
                isThere = false;
            }
            else
            {
                isThere = true;
            }

            EndReading();

            return isThere;
        }

        public string BuyTicket(string city, int day)
        {
            StartWriting();
            var ticket = new Ticket { Arrival = city, Date = new System.DateTime(Year, Month, day) };
            var desiredTicket = _ticketsInAirport.FirstOrDefault(book => book.Equals(ticket));

            if (desiredTicket != null)
            {
                var index = _ticketsInAirport.FindIndex(book => book.Equals(ticket));
                desiredTicket.NumberOfSeatsLeft -= 1;
                _ticketsInAirport[index] = desiredTicket;
            }

            EndWriting();

            return desiredTicket?.ToString();
        }
    }
}