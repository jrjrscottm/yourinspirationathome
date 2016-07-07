using System;

namespace Helium.Distributors
{
    public class Volume
    {
        public Volume(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }

        public decimal PersonalRetailVolume { get; set; }
        public decimal PersonalCommissionableVolume { get; set; }
        public decimal GroupRetailVolume { get; set; }
        public decimal GroupCommissionableVolume { get; set; }
        public decimal BonusVolume { get; set; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
    }
}