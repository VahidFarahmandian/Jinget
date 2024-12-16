using Mapster;

namespace Jinget.Core.Utilities;

public class ResponseResultUtility
{
    /// <summary>
    /// Convert ResponseResult<TSource> to ResponseResult<TDestination>
    /// </summary>
    public static ResponseResult<TDestination> MapTo<TSource, TDestination>(ResponseResult<TSource> input)
    {
        ResponseResult<TDestination> result = new(
            input.Data.Select(x => x.Adapt<TDestination>()), input.EffectedRowsCount);

        return result;
    }
}
