using System;
using System.Collections.Generic;

namespace Model.Models
{
    public class TvShowModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<PersonModel> Cast { get; set; }
    }
}