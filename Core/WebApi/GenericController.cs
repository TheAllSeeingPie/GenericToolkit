﻿using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using GenericToolkit.Core.EntityFramework;

namespace GenericToolkit.Core.WebApi
{
    public class GenericController<TEntity, TGet, TPost, TPut> : ApiController
        where TEntity : class, IEntity
        where TGet : class
        where TPost : class
        where TPut : class, IEntity
    {
        private readonly GenericContext _context;

        static GenericController()
        {
            Mapping.Register(new[] {typeof (TEntity)}, new[] {typeof (TGet), typeof (TPost), typeof (TPut)},
                (type, type1) => type1.Name.StartsWith(type.Name));
        }

        public GenericController(GenericContext context)
        {
            _context = context;
        }

        public IEnumerable<TGet> Get()
        {
            var dbSet = _context.Set(typeof (TEntity));

            foreach (var entity in dbSet)
            {
                yield return Mapper.Map((TEntity)entity, TypeGenerator.Generate<TGet>());
            }
        }

        public async Task<TGet> Get(int id)
        {
            var dbSet = _context.Set(typeof (TEntity));
            var entity = await dbSet.FindAsync(id);
            return Mapper.Map<TGet>(entity);
        }

        public async Task<IHttpActionResult> Post(TPost dto)
        {
            var entity = Mapper.Map(dto, TypeGenerator.Generate<TEntity>());
            var dbSet = _context.Set(typeof (TEntity));
            dbSet.Add(entity);
            var id = await _context.SaveChangesAsync();

            return Ok(id);
        }

        public async Task<IHttpActionResult> Put(TPut dto)
        {
            var updatedEntity = Mapper.Map(dto, TypeGenerator.Generate<TEntity>());
            var dbSet = _context.Set(typeof (TEntity));
            var entity = await dbSet.FindAsync(updatedEntity.Id);

            foreach (var property in entity.GetType().GetProperties())
            {
                property.SetValue(entity, property.GetValue(updatedEntity));
            }

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}