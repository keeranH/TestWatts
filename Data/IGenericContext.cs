using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Econocom.Data
{
    public interface IGenericContext
    {
        DbSet<T> DBSet<T>() where T : Type;
    }
}
