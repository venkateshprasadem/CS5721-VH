namespace VaccineHub.Service.Models {
    public class Product {
        public string Id { get; set; }

        public string Name { get; set; }

        public decimal? Cost { get; set; }

        public Currency? Currency { get; set; }

        public int? Doses { get; set; }

        public int? MaxIntervalInDays { get; set; }

        public int? MinIntervalInDays { get; set; }
    }
}