using System;

namespace ParallelProgramming_lab2
{
    public class Ticket
    {
        public int Id { get; set; }

        public string Arrival { get; set; }

        public DateTime Date { get; set; }

        public int NumberOfSeatsLeft { get; set; }

        public override string ToString()
        {
            return $"ID Билета: {Id}; Прибытие в: {Arrival}; Дата полёта: {Date}; Осталось мест: {NumberOfSeatsLeft};";
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is Ticket)) return false;
            Ticket ticket = (Ticket)obj;
            return ticket.Arrival == this.Arrival && ticket.Date == this.Date;
        }
    }
}