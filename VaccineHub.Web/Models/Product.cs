using VaccineHub.Web.Types;

namespace VaccineHub.Web.Models {
    public class Product {
        public string Id { get; set; }

        public string Name { get; set; }

        public decimal? Cost { get; set; }

        public Currency? Currency { get; set; }

        public int? Doses { get; set; }

        // https://www.hse.ie/eng/health/immunisation/hcpinfo/covid19vaccineinfo4hps/faqscovidvacc/
        public int? MaxIntervalInDays { get; set; }

        public int? MinIntervalInDays { get; set; }
    }
}