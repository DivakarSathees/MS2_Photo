using System;
using System.Collections.Generic;

namespace dotnetapp.Models
{
    public class Workshop
    {
        public int WorkshopID { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public int Capacity { get; set; }
        public ICollection<Participant> Participants { get; set; }
    }
}