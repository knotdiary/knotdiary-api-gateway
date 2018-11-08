using MongoDB.Bson;
using System;

namespace KnotDiary.Models
{
    public class BaseEntity : IBaseEntity
    {
        public ObjectId Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }
    }
}
