using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Econocom.Model;
using Econocom.Model.Interfaces;
using Model.Interfaces;
using Omu.Awesome.Core;

namespace Econocom.Business
{
    public class CrudService<T> : ICrudService<T> where T : Entity, new()
    {
        protected IRepository<T> repo;

        public CrudService(IRepository<T> repo)
        {
            this.repo = repo;
        }

        public IEnumerable<T> All()
        {
            return repo.All();
        }

        public int Count()
        {
            return repo.Count;
        }

        public IPageable<T> GetPageable(int page, int pageSize)
        {
            return repo.GetPageable(page, pageSize);
        }

        public T Find(int id)
        {
            return repo.Find(id);
        }

        public virtual T Create(T e)
        {
            return repo.Create(e);
        }

        public virtual void Save(T e)
        {
            repo.Update(e);
        }

        public void Delete(int id)
        {
            repo.Delete(repo.Find(id));
        }
    }
}
