namespace FS.TimeTracking.Shared.Interfaces.Application.Converters
{
    /// <summary>
    /// DTO to model converter
    /// </summary>
    /// <typeparam name="TModel">The type of the <typeparamref name="TModel"/> model.</typeparam>
    /// <typeparam name="TDto">The type of the <typeparamref name="TDto"/> dto.</typeparam>
    public interface IModelConverter<TModel, TDto>
    {
        /// <summary>
        /// Converts  <typeparamref name="TModel"/> to <typeparamref name="TDto"/>.
        /// </summary>
        /// <param name="model">The model.</param>
        TDto ToDto(TModel model);

        /// <summary>
        /// Converts  <typeparamref name="TDto"/> to <typeparamref name="TModel"/> .
        /// </summary>
        /// <param name="model">The model.</param>
        TModel FromDto(TDto model);
    }
}
