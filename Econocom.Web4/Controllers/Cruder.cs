using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Econocom.Model;
using Econocom.Model.Interfaces;
using Infrastructure.Builder;
using Infrastructure.DTO;
using Model.Interfaces;

namespace Econocom.Web4.Controllers
{
    /// <summary>
    /// generic crud controller for entities where there is no difference between the edit and create view
    /// </summary>
    /// <typeparam name="TEntity">the entity</typeparam>
    /// <typeparam name="TInput"> viewmodel </typeparam>
    public class Cruder<TEntity, TInput> : Crudere<TEntity, TInput, TInput>
        where TInput : EntityEditInput, new()
        where TEntity : Entity, new()
    {
        public Cruder(ICrudService<TEntity> s, IBuilder<TEntity, TInput> v)
            : base(s, v, v)
        {
        }

        protected override string EditView
        {
            get { return "create"; }
        }
    }
}