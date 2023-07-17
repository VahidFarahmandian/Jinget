namespace Jinget.Handlers.ExternalServiceHandlers.Tests.DefaultServiceHandler.SampleType
{
    public class SampleGetResponse
    {
        public int id { get; set; }
        public required string name { get; set; }
        public required string username { get; set; }
        public required string email { get; set; }
        public required Address address { get; set; }
        public required string phone { get; set; }
        public required string website { get; set; }
        public required Company company { get; set; }


        public class Address
        {
            public required string street { get; set; }
            public required string suite { get; set; }
            public required string city { get; set; }
            public required string zipcode { get; set; }
            public required Geo geo { get; set; }
        }

        public class Geo
        {
            public string lat { get; set; }
            public string lng { get; set; }
        }

        public class Company
        {
            public string name { get; set; }
            public string catchPhrase { get; set; }
            public string bs { get; set; }
        }

    }
}