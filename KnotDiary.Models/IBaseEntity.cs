using MongoDB.Bson;
using System;

namespace KnotDiary.Models
{
    public interface IBaseEntity
    {
        ObjectId Id { get; set; }

        DateTime CreatedDate { get; set; }

        DateTime ModifiedDate { get; set; }

        string CreatedBy { get; set; }

        string ModifiedBy { get; set; }
    }
}
