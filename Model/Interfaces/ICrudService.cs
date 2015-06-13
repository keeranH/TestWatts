using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Omu.Awesome.Core;

namespace Econocom.Model.Interfaces
{
    public interface ICrudService<T> where T : Entity, new()
    {
        T Create(T e);
        void Save(T e);
        IPageable<T> GetPageable(int page, int pageSize);
        void Delete(int id);
        T Find(int id);
        IEnumerable<T> All();
        int Count();
    }
}
