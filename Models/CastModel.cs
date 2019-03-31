using System;

namespace Model.Models
{
    public class CastModel
    {
        public PersonModel Person { get; set; }
    }
    public class PersonModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? Birthday { get; set; }
    }
}