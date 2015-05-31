using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.Results;
using AutoMapper;
using GenericToolkit.Core.EntityFramework;

namespace GenericToolkit.Core.WebApi
{
    public class GenericController<TEntity, TGet, TPost, TPut> : ApiController, IGenericController
        where TEntity : class, IEntity
        where TGet : class
        where TPost : class
        where TPut : class, IEntity
    {
        private readonly GenericContext _context;
        private readonly string _name = GetName();

        static GenericController()
        {
            Mapping.Register(new[] {typeof (TEntity)}, new[] {typeof (TGet), typeof (TPost), typeof (TPut)},
                (type, type1) => type1.Name.StartsWith(type.Name));
        }

        public GenericController(GenericContext context = null)
        {
            //Doing poor mans DI here due to not wanting to tie consumers of this into having an IoC framework they don't like
            _context = context ?? new GenericContext(BootStrapper.Entities);
        }

        public GenericController()
        {
            _context = new GenericContext(BootStrapper.Entities);
        }

        public string Name
        {
            get { return _name; }
        }

        public IEnumerable<TGet> Get()
        {
            var dbSet = _context.Set(typeof (TEntity));

            foreach (var entity in dbSet)
            {
                yield return Mapper.Map((TEntity) entity, TypeGenerator.Generate<TGet>());
            }
        }

        public async Task<TGet> Get(int id)
        {
            var dbSet = _context.Set(typeof (TEntity));
            var entity = await dbSet.FindAsync(id);
            return Mapper.Map((TEntity) entity, TypeGenerator.Generate<TGet>());
        }

        public async Task<IHttpActionResult> Post([ModelBinder(typeof(GenericTypeModelBinder))]TPost dto)
        {
            var entity = Mapper.Map(dto, TypeGenerator.Generate<TEntity>());
            var dbSet = _context.Set(typeof (TEntity));
            dbSet.Add(entity);
            var id = await _context.SaveChangesAsync();

            return Ok(id);
        }

        public async Task<IHttpActionResult> Put([ModelBinder(typeof(GenericTypeModelBinder))]TPut dto)
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

        public async Task<IHttpActionResult> Delete(int id)
        {
            var dbSet = _context.Set(typeof(TEntity));
            var entity = await dbSet.FindAsync(id);
            dbSet.Remove(entity);

            await _context.SaveChangesAsync();

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing)
            {
                _context.Dispose();
            }
        }

        private static string GetName()
        {
            return string.Format("{0}Controller", typeof (TEntity).GetClassName());
        }
    }
}