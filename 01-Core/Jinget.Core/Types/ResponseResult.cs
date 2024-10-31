namespace Jinget.Core.Types;

public class ResponseResult<TResponseModel>
{
    private static ResponseResult<TResponseModel>? _instance;
    public static ResponseResult<TResponseModel> Empty => _instance ??= new ResponseResult<TResponseModel>();
    public long EffectedRowsCount { get; }

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

    public List<TResponseModel> Data { get; }

    public ResponseResult()
    {
        Data = [];
    }

    public ResponseResult(TResponseModel data) : this() => Data.Add(data);
    public ResponseResult(IEnumerable<TResponseModel> data) : this() => Data.AddRange(data);

    public ResponseResult(TResponseModel data, long effectedRowsCount) : this(data) =>
        EffectedRowsCount = effectedRowsCount;

    public ResponseResult(IEnumerable<TResponseModel> data, long effectedRowsCount) : this(data) =>
        EffectedRowsCount = effectedRowsCount;
}