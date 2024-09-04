namespace Miksvel.TestProject.ProviderTwo.Model
{
    // HTTP POST http://provider-two/api/v1/search
    public class ProviderTwoSearchRequest
    {
        // Mandatory
        // Start point of route, e.g. Moscow 
        public string Departure { get; set; }

        // Mandatory
        // End point of route, e.g. Sochi
        public string Arrival { get; set; }

        // Mandatory
        // Start date of route
        public DateTime DepartureDate { get; set; }

        // Optional
        // Minimum value of timelimit for route
        public DateTime? MinTimeLimit { get; set; }
    }
}
