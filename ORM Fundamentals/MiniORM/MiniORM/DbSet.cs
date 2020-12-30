using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MiniORM
{
    //represents the collection of all entities in the context, or that can be queried from the database, of a given type
    public class DbSet<TEntity> : ICollection<TEntity>
    where TEntity: class, new()
    {
        //changeTracker provides access to features of the context that deal with change tracking of entities
        internal ChangeTracker<TEntity> ChangeTracker { get; set; }
        //collection of the entities
        internal IList<TEntity> Entities { get; set; }

        internal DbSet(IEnumerable<TEntity> entities)
        {
            this.Entities = entities.ToList();
            this.ChangeTracker = new ChangeTracker<TEntity>(entities);
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return this.Entities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(TEntity item)
        {
            if (item == null)
            {
                throw new ArgumentException(nameof(item),"Item cannot be null");
            }
            this.Entities.Add(item);
            this.ChangeTracker.Add(item);
        }

        public void Clear()
        {
            foreach (var entity in this.Entities)
            {
                this.Entities.Remove(entity);
            }
        }

        public bool Contains(TEntity item) => this.Entities.Contains(item);

        public void CopyTo(TEntity[] array, int arrayIndex) => this.Entities.CopyTo(array, arrayIndex);

        public bool Remove(TEntity item)
        {
            if (item == null)
            {
                throw new ArgumentException(nameof(item), "item cannot be null");
            }

            var removedSuccessfully = this.Entities.Remove(item);
            if (removedSuccessfully)
            {
                this.ChangeTracker.Remove(item);
            }

            return removedSuccessfully;
        }

        public int Count => this.Entities.Count;
        public bool IsReadOnly => this.Entities.IsReadOnly;

        public void RemoveRange(IEnumerable<TEntity> entitiesToRemove)
        {
            foreach (var entity in entitiesToRemove.ToArray())
            {
                this.Remove(entity);
            }
        }
    }
}