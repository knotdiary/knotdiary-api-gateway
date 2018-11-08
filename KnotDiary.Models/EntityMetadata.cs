using System;
using System.Collections.Generic;

namespace KnotDiary.Models
{
    public class EntityMetadata<TEntity, TErrorType> where TErrorType : struct
    {
        public EntityMetadata() { }

        public EntityMetadata(TEntity data)
        {
            if (!typeof(TErrorType).IsEnum)
            {
                throw new ArgumentException("Argument must be an enum type");
            }

            Data = data;
        }

        public EntityMetadata(List<ErrorInfo<TErrorType>> errors)
        {
            if (!typeof(TErrorType).IsEnum)
            {
                throw new ArgumentException("Argument must be an enum type");
            }

            Errors = errors;
        }

        public EntityMetadata(TEntity data, List<ErrorInfo<TErrorType>> errors)
        {
            if (!typeof(TErrorType).IsEnum)
            {
                throw new ArgumentException("Argument must be an enum type");
            }

            Data = data;
            Errors = errors;
        }

        public TEntity Data { get; set; }

        public List<ErrorInfo<TErrorType>> Errors{ get; set; }
    }
}
