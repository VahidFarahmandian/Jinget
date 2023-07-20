namespace Jinget.Handlers.ExternalServiceHandlers.Tests.DefaultServiceHandler.SampleType
{
    public class SampleGetResponse
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required Address_ Address { get; set; }
        public required string Phone { get; set; }
        public required string Website { get; set; }
        public required Company_ Company { get; set; }


        public class Address_
        {
            public required string Street { get; set; }
            public required string Suite { get; set; }
            public required string City { get; set; }
            public required string Zipcode { get; set; }
            public required Geo_ Geo { get; set; }
        }

        public class Geo_
        {
            public string? Lat { get; set; }
            public string? Lng { get; set; }
        }

        public class Company_
        {
            public string? Name { get; set; }
            public string? CatchPhrase { get; set; }
            public string? Bs { get; set; }
        }

    }
}