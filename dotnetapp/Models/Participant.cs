namespace dotnetapp.Models
{
    public class Participant
    {
        public int ParticipantID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int WorkshopID { get; set; }
        public Workshop Workshop { get; set; }
    }
}