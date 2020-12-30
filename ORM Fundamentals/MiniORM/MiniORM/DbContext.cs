using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace MiniORM
{
    public class DbContext
    {
        private readonly DatabaseConnection connection;
        private readonly Dictionary<Type, PropertyInfo> dbsetProperties;

        internal static readonly Type[] AllowedSqlTypes =
        {
            typeof(string),
            typeof(int),
            typeof(double),
            typeof(decimal),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(bool),
            typeof(DateTime)
        };

        protected DbContext(string connectionString)
        {
            this.connection = new DatabaseConnection(connectionString);

            this.dbsetProperties = this.DiscoverDbSets();

            using (new ConnectionManager(connection))
            {
                this.InitializeDbSets();
            }

            this.MapAllRelations();
        }

        public void SaveChanges()
        {
            //we get all the generic types in our tables (DbSets)
            var dbSets = this.dbsetProperties
                .Select(x => x.Value.GetValue(this))
                .ToArray();

            //now we need to ensure that each entity(row) is valid

            //if any are invalid we throw an InvalidOperationException
            foreach (IEnumerable<object> dbSet in dbSets)
            {
                var invalidEntities = dbSet.Where(x => !IsObjectValid(x)).ToArray();

                if (invalidEntities.Any())
                {
                    throw new InvalidOperationException($"{invalidEntities.Length} Invalid Entities found in {dbSet.GetType().Name}!");
                }
            }
            //now we connect to the the database
            using(new ConnectionManager(connection))
            {
                //we create another using block for starting a db transaction
                //that way, if anything goes wrong, no entities will be inserted/modified/deleted 
                using (var transaction = this.connection.StartTransaction())
                {
                    foreach (IEnumerable dbSet in dbSets)
                    {
                        //now we need to find the entity type of each dbSet
                        var dbSetType = dbSet.GetType().GetGenericArguments().First();

                        //we make a generic version of that method using the DBSetType
                        var persistMethod = typeof(DbContext)
                            .GetMethod("Persist", BindingFlags.Instance | BindingFlags.NonPublic)
                            .MakeGenericMethod(dbSetType);
                        //we invoke try catch with the known exceptions
                        try
                        {
                            persistMethod.Invoke(this, new object[] {dbSet});
                        }
                        //If the invoked method throws an exception
                        catch (TargetInvocationException e)
                        {
                            //Consequently, this block throws the inner exception, because this is the actual exception,
                            //which occurred within the method invocation,
                            //which the second and third catch blocks will handle.
                            throw e.InnerException;
                        }
                        catch (InvalidOperationException)
                        {
                            transaction.Rollback();
                        }
                        catch (SqlException)
                        {
                            transaction.Rollback();
                        }
                    }
                    transaction.Commit();
                }
            }
        }

        private void InitializeDbSets()
        {
            //For each DB set, we will invoke the PopulateDbSet(dbSetProperty) method dynamically,
            //because we are providing the generic type parameter at runtime,
            //since we don’t know what the framework user’s DB sets are.
            foreach (var dbset in dbsetProperties)
            {
                var dbSetType = dbset.Key;
                var dbSetProperty = dbset.Value;

                var populateDbSetGeneric = typeof(DbContext)
                    .GetMethod("PopulateDbSet", BindingFlags.Instance | BindingFlags.NonPublic)
                    .MakeGenericMethod(dbSetType);

                populateDbSetGeneric.Invoke(this, new object[] {dbSetProperty});
            }
        }

        private void Persist<TEntity>(DbSet<TEntity> dbSet)
            where TEntity: class, new()
        {
            var tableName = GetTableName(typeof(TEntity));

            var columns = this.connection.FetchColumnNames(tableName).ToArray();

            //we check the dbSet’s ChangeTracker for any added entities, and if there are any, we use the InsertEntities() method
            if (dbSet.ChangeTracker.AddedCollection.Any())
            {
                this.connection.InsertEntities(dbSet.ChangeTracker.AddedCollection, tableName, columns);
            }

            //now we need to get the modified entities and we update them
            var modifiedEntities = dbSet.ChangeTracker.GetModifiedEntities(dbSet).ToArray();
            if (modifiedEntities.Any())
            {
                this.connection.UpdateEntities(modifiedEntities, tableName,columns);
            }

            //now we check if there are any removed entities and we remove them
            var removedEntities = dbSet.ChangeTracker.RemovedCollection;
            if (removedEntities.Any())
            {
                this.connection.DeleteEntities(removedEntities, tableName, columns);
            }
        }

        private void PopulateDbSet<TEntity>(PropertyInfo dbSet)
            where TEntity : class, new()
        {
            //We retrieve the entities from the database, using the LoadTableEntities<TEntity>() method
            var entities = LoadTableEntities<TEntity>();

            //we need to replace the actual DbSet property in the current DbContext instance with the one we just created.
            var dbSetInstance = new DbSet<TEntity>(entities);

            //Since the Db sets don’t have a setter, we need to replace its backing field, by using the ReflectionHelper.ReplaceBackingField() method
            //This works, because every auto-property has a private, autogenerated backing field.
            ReflectionHelper.ReplaceBackingField(this, dbSet.Name,dbSetInstance);
        }

        private IEnumerable<TEntity> LoadTableEntities<TEntity>() 
            where TEntity : class, new()
        {
            var table = typeof(TEntity);
            var columns = GetEntityColumnNames(table);
            var tableName = GetTableName(table);
            var fetchedRows = this.connection.FetchResultSet<TEntity>(tableName, columns).ToArray();
            return fetchedRows;
        }

        private string GetTableName(Type tableType)
        {
            //we check if the table has a table attribute
            //and if it has one, we can get it ez
            //if it doesn't have one, we take the name from the dbSet properties
            var tableName = ((TableAttribute)Attribute.GetCustomAttribute(tableType, typeof(TableAttribute)))?.Name;
            if (tableName == null)
            {
                tableName = this.dbsetProperties[tableType].Name;
            }

            return tableName;
        }

        private string[] GetEntityColumnNames(Type table)
        {
            //which returns an array of strings with the column names and accepts the table type as a parameter

            var tableName = this.GetTableName(table);

            var dbColumns = this.connection.FetchColumnNames(tableName);

            var columns = table.GetProperties()
                .Where(pi => dbColumns.Contains(pi.Name) &&
                             !pi.HasAttribute<NotMappedAttribute>() &&
                             AllowedSqlTypes.Contains(pi.PropertyType))
                .Select(pi => pi.Name)
                .ToArray();
            return columns;
        }

        private static bool IsObjectValid(object e)
        {
            /*Since the Validator class is part of System.Data.Annotations, which is very old,
             we need to write a bunch of boilerplate code to use it. 
             So instead of writing this everywhere we need to validate an object,
             we can just extract it into a method*/

            var validationContext = new ValidationContext(e);
            var validationErrors = new List<ValidationResult>();

            var validationResult =
                Validator.TryValidateObject(e, validationContext, validationErrors, validateAllProperties: true);
            return validationResult;
        }
        
        private void MapAllRelations()
        {
            //All this method will do is call MapRelations() dynamically for each DB set property
            foreach (var dbsetProperty in dbsetProperties)
            {
                var dbSetType = dbsetProperty.Key;

                var mapRelationsGeneric = typeof(DbContext)
                    .GetMethod("MapRelations", BindingFlags.Instance | BindingFlags.NonPublic)
                    ?.MakeGenericMethod(dbSetType);

                var dbSet = dbsetProperty.Value.GetValue(this);

                if (mapRelationsGeneric != null) mapRelationsGeneric.Invoke(this, new[] {dbSet});
            }
        }

        private void MapCollection<TDbSet, TCollection>(DbSet<TDbSet> dbSet, PropertyInfo collectionProperty)
            where TDbSet: class, new()
            where TCollection: class, new()
        {
            //we need to get the primary and the foreign keys
            var entityType = typeof(TDbSet);
            var collectionType = typeof(TCollection);

            /*The primary ones we find by getting all the properties
            which have a [Key] attribute in the collectionType variable we declared before*/
            var primaryKeys = collectionType.GetProperties()
                .Where(x => x.HasAttribute<KeyAttribute>())
                .ToArray();
            var primaryKey = primaryKeys.First();

            //We can get the foreign key in the same way but using the entityType variable
            var foreignKey = entityType.GetProperties()
                .First(x => x.HasAttribute<KeyAttribute>());

            /*We check if we are dealing with a many-to-many relation,
            which is only true if we have 2 or more primary keys*/
            var isManyToMany = primaryKeys.Length >= 2;
            if (isManyToMany)
            {
                /*we can get the foreign key by finding the first type property,
                whose name is equal to the foreign key attribute’s name
                and has the same property type as the entity type.*/

                primaryKey = collectionType.GetProperties()
                    .First(x => collectionType
                        .GetProperty(x.GetCustomAttribute<ForeignKeyAttribute>().Name)
                        .PropertyType == entityType);
            }

            /*Now we get the collection’s DB set, which we will filter with a where clause
            and extract all of the entities whose foreign keys are equal
            to the primary key of the current entity.*/

            var navigationDbSet = (DbSet<TCollection>) this.dbsetProperties[collectionType].GetValue(this);

            foreach (var entity in dbSet)
            {
                var primaryKeyValue = foreignKey.GetValue(entity);
                var navigationEntities = navigationDbSet
                    .Where(navigationEntity => primaryKey.GetValue(navigationEntity).Equals(primaryKeyValue))
                    .ToArray();

                //we replace the null collection with a populated collection
                ReflectionHelper.ReplaceBackingField(entity, collectionProperty.Name, navigationEntities);
            }
        }

        private void MapNavigationProperties<TEntity>(DbSet<TEntity> dbSet)
            where TEntity: class, new()
        {
            //this method finds the entity’s foreign keys (there could be multiple) and iterates over them
            var entityType = typeof(TEntity);

            var foreignKeys = entityType.GetProperties()
                .Where(x => x.HasAttribute<ForeignKeyAttribute>())
                .ToArray();

            foreach (var foreignKey in foreignKeys)
            {
                /*for each entity in that DB set, we find the first entity,
                 whose primary key value is equal to the foreign key value in our TEntity*/
                var navigationPropertyName =
                    foreignKey.GetCustomAttribute<ForeignKeyAttribute>().Name;
                var navigationProperty = entityType.GetProperty(navigationPropertyName);

                var navigationDbSet = this.dbsetProperties[navigationProperty.PropertyType]
                    .GetValue(this);

                var navigationPrimaryKey = navigationProperty.PropertyType.GetProperties()
                    .First(x => x.HasAttribute<KeyAttribute>());

                foreach (var entity in dbSet)
                {
                    var foreignKeyValue = foreignKey.GetValue(entity);

                    var navigationPropertyValue = ((IEnumerable<object>) navigationDbSet)
                        .First(currentNavigationProperty =>
                            navigationPrimaryKey.GetValue(currentNavigationProperty).Equals(foreignKeyValue));
                    /*we replace the navigation property’s value (which is currently null) with the entity we found.*/
                    navigationProperty.SetValue(entity, navigationPropertyValue);
                }
            }
        }

        private Dictionary<Type, PropertyInfo> DiscoverDbSets()
        {
            /*used that method in our constructor to populate our dbSetProperties field,
             which is a Dictionary with а Type as a key and a PropertyInfo as a value*/
            var dbSets = this.GetType().GetProperties()
                .Where(pi => pi.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                .ToDictionary(pi => pi.PropertyType.GetGenericArguments().First(),pi=>pi);

            //we get all the info about the columns
            return dbSets;
        }
    }
}