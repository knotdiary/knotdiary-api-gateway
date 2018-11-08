using MongoDB.Bson;
using MongoDB.Driver;
using KnotDiary.Common;
using KnotDiary.Common.Extensions;
using KnotDiary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace KnotDiary.Data
{
    public abstract class MongoDbGenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : IBaseEntity
    {
        protected readonly IMongoDatabase _db;
        protected readonly IConfigurationHelper _configurationHelper;

        public MongoDbGenericRepository(IConfigurationHelper configurationHelper)
        {
            _configurationHelper = configurationHelper;

            var clientString = _configurationHelper.GetAppSettings("MongoDbClientString");
            var dbName = _configurationHelper.GetAppSettings("MongoDbDatabaseName");

            var client = new MongoClient(clientString);
            _db = client.GetDatabase(dbName);
        }

        public async Task<IList<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> filter = null, Expression<Func<TEntity, object>> orderBy = null, int skip = 0, int take = 10)
        {
            var collectionName = GetCollectionName();
            if (string.IsNullOrEmpty(collectionName)) throw new InvalidOperationException("Invalid collection name.");

            var collection = _db.GetCollection<TEntity>(collectionName);
            var hasFilter = false;
            var hasOrder = false;

            if (filter != null)
            {
                hasFilter = true;
            }

            if (orderBy != null)
            {
                hasOrder = true;
            }

            if (hasFilter && hasOrder)
            {
                return await collection.Find(Builders<TEntity>.Filter.Where(filter)).Sort(Builders<TEntity>.Sort.Descending(orderBy)).Skip(0).Limit(take).ToListAsync();
            }

            if (hasFilter && !hasOrder)
            {
                return await collection.Find(Builders<TEntity>.Filter.Where(filter)).Skip(0).Limit(take).ToListAsync();
            }

            if (!hasFilter && hasOrder)
            {
                return await collection.Find(a => a.Id != null).Sort(Builders<TEntity>.Sort.Descending(orderBy)).Skip(0).Limit(take).ToListAsync();
            }

            return await collection.Find(a => a.Id != null).Skip(0).Limit(take).ToListAsync();
        }

        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            var collectionName = GetCollectionName();
            if (string.IsNullOrEmpty(collectionName)) throw new InvalidOperationException("Invalid collection name.");

            var collection = _db.GetCollection<TEntity>(collectionName);
            var dbFilter = Builders<TEntity>.Filter.Where(filter);
            var result = await collection.FindAsync(dbFilter);

            return await result.FirstOrDefaultAsync();
        }

        public async Task<TEntity> Insert(TEntity entity)
        {
            var collectionName = GetCollectionName();
            if (string.IsNullOrEmpty(collectionName)) throw new InvalidOperationException("Invalid collection name.");

            var collection = _db.GetCollection<TEntity>(collectionName);
            entity.CreatedDate = DateTime.UtcNow;
            entity.ModifiedDate = DateTime.UtcNow;

            await collection.InsertOneAsync(entity);

            return entity;
        }

        public async Task<TEntity> Update(TEntity entity)
        {
            var collectionName = GetCollectionName();
            if (string.IsNullOrEmpty(collectionName)) throw new InvalidOperationException("Invalid collection name.");

            var collection = _db.GetCollection<TEntity>(collectionName);
            entity.ModifiedDate = DateTime.UtcNow;

            await collection.ReplaceOneAsync(a => a.Id == entity.Id, entity);

            return entity;
        }

        public async Task<bool> Delete(object id)
        {
            var entityId = (ObjectId)id;
            if (entityId == null) throw new InvalidCastException("Invalid entity");

            var collection = _db.GetCollection<BaseEntity>(typeof(TEntity).Name.ToCamelCase());
            var filter = Builders<BaseEntity>.Filter.Eq(e => e.Id, entityId);
            var result = await collection.DeleteOneAsync(filter);

            return result?.DeletedCount == 1;
        }

        protected string GetCollectionName(IEnumerable<Type> typeHierarchy = null)
        {
            if (typeHierarchy == null)
            {
                typeHierarchy = typeof(TEntity).GetInheritanceHierarchy();
            }

            foreach (var type in typeHierarchy)
            {
                var typeNameCollection = _configurationHelper.GetAppSettings($"MongoTypeMapping:{type.Name}");
                var excludedTypes = _configurationHelper.GetAppSettingsList("ExcludedMongoTypeMapping");

                if (!string.IsNullOrEmpty(typeNameCollection) && !excludedTypes.Contains(typeNameCollection.ToLower()))
                {
                    return typeNameCollection;
                }
            }

            return null;
        }
    }
}
