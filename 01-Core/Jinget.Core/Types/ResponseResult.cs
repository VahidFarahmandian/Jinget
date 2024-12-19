using Mapster;

namespace Jinget.Core.Types;

public class ResponseResult<TResponseModel>
{
    private static ResponseResult<TResponseModel>? _instance;
    public static ResponseResult<TResponseModel> Empty => _instance ??= new ResponseResult<TResponseModel>();

    /// <summary>
    /// if the <typeparamref name="TResponseModel"></typeparamref> is <see cref="ProblemDetails"/> then false will be returned
    /// otherwise true will be returned
    /// </summary>
    public bool IsSuccess => typeof(TResponseModel) != typeof(ProblemDetails);

    /// <summary>
    /// if the <typeparamref name="TResponseModel"></typeparamref> is <see cref="ProblemDetails"/> then true will be returned
    /// otherwise false will be returned
    /// </summary>
    public bool IsFailure => !IsSuccess;

    public long EffectedRowsCount { get; }
    public List<TResponseModel> Data { get; }

    public ResponseResult()
    {
        Data = [];
    }

    public ResponseResult(TResponseModel data) : this()
    {
        if (data == null)
        {
            Data.AddRange([]);
            EffectedRowsCount = 0;
        }
        else
        {
            Data.Add(data);
            EffectedRowsCount = 1;
        }
    }

    public ResponseResult(IEnumerable<TResponseModel>? data) : this()
    {
        if (data == null)
        {
            Data.AddRange([]);
            EffectedRowsCount = 0;
        }
        else
        {
            Data.AddRange(data);
            EffectedRowsCount = data.Count();
        }
    }

    public ResponseResult(TResponseModel data, long effectedRowsCount) : this(data) =>
        EffectedRowsCount = effectedRowsCount;

    public ResponseResult(IEnumerable<TResponseModel> data, long effectedRowsCount) : this(data) =>
        EffectedRowsCount = effectedRowsCount;

    /// <summary>
    /// Maps a <see cref="ResponseResult`TSource`"/> to a <see cref="ResponseResult`TDestination`"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of the source data.</typeparam>
    /// <typeparam name="TDestination">The type of the destination data.</typeparam>
    /// <param name="input">The input <see cref="ResponseResult{TSource}"/> to be mapped.</param>
    /// <returns>A new <see cref="ResponseResult{TDestination}"/> with the mapped data.</returns>
    public static ResponseResult<TDestination> MapTo<TDestination>(ResponseResult<TResponseModel> input)
    {
        if (input == null)
        {
            throw new ArgumentNullException(nameof(input), "Input cannot be null");
        }

        try
        {
            var mappedData = input.Data.Select(x => x.Adapt<TDestination>()).ToList();
            return new ResponseResult<TDestination>(mappedData, input.EffectedRowsCount);
        }
        catch (Exception ex)
        {
            // Handle or log the exception as needed
            throw new InvalidOperationException("An error occurred while mapping the data", ex);
        }
    }
}