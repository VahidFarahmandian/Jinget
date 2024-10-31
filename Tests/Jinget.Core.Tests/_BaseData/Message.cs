using Jinget.Core.Contracts;

namespace Jinget.Core.Tests._BaseData
{
    public class GenericRequestSampleMessage : IOrderBy<List<OrderBy<GenericRequestSampleMessage>>>, IPaginated
    {
        public List<OrderBy<GenericRequestSampleMessage>> OrderBy { get; set; }
        public GenericRequestSampleMessage()
        {
            PagingConfig = new Paging
            {
                PageNumber = 1,
                PageSize = 10,
            };
            OrderBy = [
            new OrderBy<GenericRequestSampleMessage>()
            {
                Name = x=>x.Property1,
                Direction=Enumerations.OrderByDirection.Descending
            }
        ];
        }
        public string Property1 { get; set; }
        public string Property2 { get; set; }
        public Paging PagingConfig { get; set; }
    }

    public class NonGenericRequestSampleMessage : IOrderBy, IPaginated
    {
        public List<OrderBy> OrderBy { get; set; }
        public NonGenericRequestSampleMessage()
        {
            PagingConfig = new Paging
            {
                PageNumber = 1,
                PageSize = 10,
            };
            OrderBy =
            [
                new OrderBy()
            {
                Name = x=>((NonGenericRequestSampleMessage )x).Property1,
                Direction=Enumerations.OrderByDirection.Descending
            }
            ];
        }
        public string Property1 { get; set; }
        public string Property2 { get; set; }
        public Paging PagingConfig { get; set; }
    }
}
