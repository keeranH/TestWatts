using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Design.PluralizationServices;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Econocom.Model;
using Econocom.Model.Interfaces;
using Econocom.Model.Models.Benchmark;
using Model.Interfaces;
using Omu.Awesome.Core;
using System.Data.Entity;


namespace Econocom.Data
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    public class Repository<TObject> : IRepository<TObject> where TObject :BaseEntity, new()
    {
        protected EconocomContext Context;        
        private bool shareContext = false;

        /// <summary>
        /// 
        /// </summary>
        public Repository()
        {
            Context = new EconocomContext();
            Context.Configuration.ProxyCreationEnabled = false;         
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unitOfWork"></param>
        public Repository(IUnitOfWork unitOfWork)
        {
            Context = unitOfWork as EconocomContext;
            Context.Configuration.ProxyCreationEnabled = false; 
            shareContext = true;
        }

        public DbSet<TObject> DbSet
        {
            get
            {
                return Context.Set<TObject>();
            }
        }

        public void Dispose()
        {
            if (shareContext && (Context != null))
                Context.Dispose();
        }

        public virtual IQueryable<TObject> All()
        {
            return DbSet.AsQueryable();
        }

        public virtual IQueryable<TObject>
        Filter(Expression<Func<TObject, bool>> predicate)
        {
            return DbSet.Where(predicate).AsQueryable<TObject>();
        }

        public virtual IQueryable<TObject> Filter(Expression<Func<TObject, bool>> filter,
         out int total, int index = 0, int size = 50)
        {
            int skipCount = index * size;
            var _resetSet = filter != null ? DbSet.Where(filter).AsQueryable() :
                DbSet.AsQueryable();
            _resetSet = skipCount == 0 ? _resetSet.Take(size) :
                _resetSet.Skip(skipCount).Take(size);
            total = _resetSet.Count();
            return _resetSet.AsQueryable();
        }

        public bool Contains(Expression<Func<TObject, bool>> predicate)
        {
            return DbSet.Count(predicate) > 0;
        }

        public virtual TObject Find(params object[] keys)
        {
            return DbSet.Find(keys);
        }

        public virtual TObject Find(Expression<Func<TObject, bool>> predicate)
        {
            return DbSet.FirstOrDefault(predicate);
        }

        public virtual TObject Create(TObject TObject)
        {
            var newEntry = DbSet.Add(TObject);
            if (!shareContext)
                Context.SaveChanges();
            return newEntry;
        }

        public virtual int Count
        {
            get
            {
                return DbSet.Count();
            }
        }

        public virtual int Delete(TObject TObject)
        {
            DbSet.Remove(TObject);
            if (!shareContext)
                return Context.SaveChanges();
            return 0;
        }

        public virtual int Update(TObject TObject)
        {
            var entry = Context.Entry(TObject);
            DbSet.Attach(TObject);
            entry.State = EntityState.Modified;
            if (!shareContext)
                return Context.SaveChanges();
            return 0;
        }

        public virtual int Delete(Expression<Func<TObject, bool>> predicate)
        {           
            var objects = Filter(predicate);
            foreach (var obj in objects)
                DbSet.Remove(obj);
            if (!shareContext)
                return Context.SaveChanges();
            return 0;
        }


        public IQueryable<TObject> Filter<Key>(Expression<Func<TObject, bool>> filter, out int total, int index = 0, int size = 50)
        {
            throw new NotImplementedException();
        }

       
        public virtual IQueryable<TObject> GetAllLazyLoad(Expression<Func<TObject, bool>> filter, params Expression<Func<TObject, object>>[] children)
        {
            children.ToList().ForEach(x => DbSet.Include(x).Load());
            return DbSet;
        }

        public IPageable<TObject> GetPageable(int page, int pageSize)
        {
            throw new NotImplementedException();
        }


        void IRepository<TObject>.Delete(TObject t)
        {
            DbSet.Remove(t);
            //if (!shareContext)
            Context.SaveChanges();
        }

        public int ExecuteCommand(string command, params object[] parameters)
        {
            var sql = @command;
            return Context.Database.ExecuteSqlCommand(sql, parameters);
        }
    }  

}
