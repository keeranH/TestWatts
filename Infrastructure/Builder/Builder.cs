using System;
using System.Collections.Generic;
using System.Linq;
using Econocom.Model;
using Econocom.Model.Interfaces;
using Model.Interfaces;
using Omu.ValueInjecter;
//
//
//
//using Omu.AwesomeDemo.Data;

namespace Infrastructure.Builder
{
    public class Builder<TEntity, TInput> : IBuilder<TEntity, TInput>
        where TEntity : class, new()
        where TInput : new()
    {
        private readonly IRepository<TEntity> repo;

        public Builder(IRepository<TEntity> repo)
        {
            this.repo = repo;
        }

        public TInput BuildInput(TEntity entity)
        {
            var input = new TInput();
            input.InjectFrom(entity)
                .InjectFrom<NullDateInjection>(entity)
                .InjectFrom<EntityToNullInt>(entity)
                .InjectFrom<EntitiesToInts>(entity);
            MakeInput(entity, ref input);
            return input;
        }

        protected virtual void MakeInput(TEntity entity, ref TInput input)
        {
        }

        public TEntity BuildEntity(TInput input, int? id)
        {
            var entity = id.HasValue ? repo.Find(id.Value) : new TEntity();
            if (entity == null)
                throw new EconocomException("this entity doesn't exist anymore");

            ////creating a clone, since my Db is a static class, changing the entity directly will change it in the Db in my case
            //var e = new TEntity();
            //e.InjectFrom(entity);
            ////not needed for a real app (with DB)

            entity.InjectFrom(input);
            entity.InjectFrom<NullDateInjection>(input);
            entity.InjectFrom<NullIntToEntity>(input);
            entity.InjectFrom<IntsToEntities>(input);
            MakeEntity(ref entity, input);
            return entity;
        }

        protected virtual void MakeEntity(ref TEntity entity, TInput input)
        {
            
        }

        public TInput RebuildInput(TInput input, int? id)
        {
            return BuildInput(BuildEntity(input, id));
        }
    }

    public class IntsToEntities : LoopValueInjection
    {
        protected override bool TypesMatch(Type s, Type t)
        {
            if (!s.IsGenericType || !t.IsGenericType
                || s.GetGenericTypeDefinition() != typeof(IEnumerable<>)
                || t.GetGenericTypeDefinition() != typeof(IEnumerable<>)) return false;
            return s.GetGenericArguments()[0] == (typeof(int))
                && (t.GetGenericArguments()[0].IsSubclassOf(typeof(Entity)));
        }

        protected override object SetValue(object v)
        {
            if (v == null) return null;

            dynamic repo = IoC.Resolve(typeof(IRepository<>).MakeGenericType(TargetPropType.GetGenericArguments()[0]));
            dynamic list = Activator.CreateInstance(typeof (List<>).MakeGenericType(TargetPropType.GetGenericArguments()[0]));

            foreach (var i in (v as IEnumerable<int>))
                list.Add(repo.Find(i));
            return list;
        }
    }

    public class EntitiesToInts : LoopValueInjection
    {
        protected override bool TypesMatch(Type s, Type t)
        {
            if (!s.IsGenericType || !t.IsGenericType
                || s.GetGenericTypeDefinition() != typeof(IEnumerable<>)
                || t.GetGenericTypeDefinition() != typeof(IEnumerable<>)) return false;
            return t.GetGenericArguments()[0] == (typeof(int))
                && (s.GetGenericArguments()[0].IsSubclassOf(typeof(Entity)));
        }

        protected override object SetValue(object v)
        {
            if (v == null) return null;
            return (v as IEnumerable<Entity>).Select(o => o.Id);
        }
    }

}