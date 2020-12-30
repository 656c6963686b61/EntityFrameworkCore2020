using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace MiniORM
{
    //Table = DbSet<Entity>
    //Entity = Class = Table Row

    internal class ChangeTracker<T>
    where T: class, new()
    {
        //the allEntities field will store clones for all rows(entities) in the table(DbSet)
        private readonly List<T> allEntities;
        private readonly List<T> added;
        //all deleted rows(entities)
        private readonly List<T> removed;

        //we make our collections readonly so nobody can modify them 
        public IReadOnlyCollection<T> AllEntities => this.allEntities.AsReadOnly();
        public IReadOnlyCollection<T> AddedCollection => this.added.AsReadOnly();
        public IReadOnlyCollection<T> RemovedCollection => this.removed.AsReadOnly();

        //add a row
        public void Add(T entity)
        {
            added.Add(entity);
        }
        //delete a row
        public void Remove(T entity)
        {
            removed.Add(entity);
        }

        //initialize tracker and lists
        public ChangeTracker(IEnumerable<T> entities)
        {
            //we initialize the added and removed lists
            added = new List<T>();
            removed = new List<T>();

            //we clone the rows(entities) in the allEntities(all rows) field
            allEntities = CloneEntities(entities);
        }

        //we clone the entities(rows) in order to see the modifications made on them
        private static List<T> CloneEntities(IEnumerable<T> entities)
        {
            var clonedEntities = new List<T>();

            //we get all the properties(columns) from the entity(row)
            var propertiesToClone = typeof(T) //type of the column => int, varchar, byte
                .GetProperties()
                //they should be part of the allowedSqlTypes, described in the DbContext class 
                .Where(x => DbContext.AllowedSqlTypes.Contains(x.PropertyType))
                .ToArray();

            //we go through all rows(entities) and create an instance from the same type
            foreach (var entity in entities)
            {
                //the instance (only from the type) without parameters
                //ex. clone = new Student();
                var clone = Activator.CreateInstance<T>();

                //now my original entity and my clone are from the same type => they have the same properties

                //here we get all the properties from propertiesToClone and insert them into the clone
                foreach (var property in propertiesToClone)
                {
                    var value = property.GetValue(entity);
                    //we transfer the entity value into the clone
                    property.SetValue(clone, value);
                }
                clonedEntities.Add(clone);
                //now our clone has the same properties as the original entity
            }
            return clonedEntities;
        }
        
        public IEnumerable<T> GetModifiedEntities(DbSet<T> table)
        {
            var modifiedEntities = new List<T>();
            var entityType = typeof(T);
            //we get the primary keys
            var primaryKeys = entityType.
                GetProperties().
                Where(x => x.HasAttribute<KeyAttribute>())
                .ToArray();

            //we compare the primary keys of the changed entity with the original one
            foreach (var proxyEntity in this.AllEntities)
            {
               object[] primaryKeyValues = GetPrimaryKeyValues(primaryKeys, proxyEntity).ToArray();

               T entity = table.Entities
                   .Single(e => GetPrimaryKeyValues(primaryKeys, e).SequenceEqual(primaryKeyValues));

               bool isModified = IsModified(entity, proxyEntity);

               if (isModified)
               {
                   modifiedEntities.Add(entity);
               }
            }

            return modifiedEntities;
        }

        private static IEnumerable<object> GetPrimaryKeyValues(IEnumerable<PropertyInfo> primaryKeys, T entity)
        {
            return primaryKeys.Select(x => x.GetValue(entity));
        }

        //takes the original and proxy entities as parameters
        //they are of the same type
        private static bool IsModified(T entity, T proxyEntity)
        {
            var monitoredProperties = typeof(T).GetProperties()
                .Where(x => DbContext.AllowedSqlTypes.Contains(x.PropertyType));
            var modifiedProperties = monitoredProperties
                .Where(x => !Equals(x.GetValue(entity), x.GetValue(proxyEntity))).ToArray();
            var isModified = modifiedProperties.Any();
            return isModified;
        }
    }
}