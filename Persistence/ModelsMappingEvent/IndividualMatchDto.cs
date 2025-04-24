using Microsoft.AspNetCore.Identity.Data;

namespace DB.SportHive.Domain
{
    public class IndividualMatchDto
    {
        public string NameEvent {get;set;} = null!;
        public string loginFirstAthlete{get;set;} = null!;
        public string loginSecondAthlete {get;set;} = null!;
        public DateTime DateStart{get;set;}
        public TimeSpan TimeStart{get;set;}

        public LocationDto location{get;set;} = null!;
        public int tour{get;set;}
        public string AddInformation { get; set; } = null!;
    }

}