using System;

namespace Model.DbModels
{
    public class Cast : BaseDbModel
    {
        public int TvMazeCastId { get; set; }
        public string Name { get; set; }
        public DateTime? Birthday { get; set; }
        public int TvShowId { get; set; }
        public TvShow TvShow { get; set; }
    }
}
