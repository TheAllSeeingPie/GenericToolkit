using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;

namespace GenericToolkit.Core.EntityFramework
{
    public class GenericContext : DbContext
    {
        private readonly IEnumerable<Type> _entities;

        public GenericContext(IEnumerable<Type> entities, DbConnection existingConnection)
            : base(existingConnection, true)
        {
            _entities = entities;
        }

        public GenericContext(IEnumerable<Type> entities)
        {
            _entities = entities;
        }

        public override DbSet Set(Type entityType)
        {
            return base.Set(TypeGenerator.GetGeneratedType(entityType));
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var dbModelBuilder_Entity_Method = typeof (DbModelBuilder).GetMethod("Entity");

            foreach (var entity in _entities)
            {
                var instanceType = TypeGenerator.GetGeneratedType(entity);
                dbModelBuilder_Entity_Method.MakeGenericMethod(instanceType)
                    .Invoke(modelBuilder, new object[] {});
            }
        }
    }
}