using System.Collections.Generic;

namespace Jinget.Core.Types
{
    public class ResponseResult<TResponseModel>
    {
        private static ResponseResult<TResponseModel>? _instance;
        public static ResponseResult<TResponseModel> Empty => _instance ??= new ResponseResult<TResponseModel>();
        public long EffectedRowsCount { get; }

        public List<TResponseModel> Data { get; }

        public ResponseResult() { Data = new List<TResponseModel>(); }

        public ResponseResult(TResponseModel data) : this() => Data.Add(data);
        public ResponseResult(IEnumerable<TResponseModel> data) : this() => Data.AddRange(data);


        public ResponseResult(TResponseModel data, long effectedRowsCount) : this(data) => EffectedRowsCount = effectedRowsCount;
        public ResponseResult(IEnumerable<TResponseModel> data, long effectedRowsCount) : this(data) => EffectedRowsCount = effectedRowsCount;

    }
}
