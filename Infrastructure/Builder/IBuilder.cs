namespace Infrastructure.Builder
{
    public interface IBuilder<TEntity, TInput> where TEntity : class, new() where TInput : new()
    {
        TInput BuildInput(TEntity entity);
        TEntity BuildEntity(TInput input, int? id = null);
        TInput RebuildInput(TInput input, int? id = null);
    }
}