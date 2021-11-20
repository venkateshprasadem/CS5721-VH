using System;

namespace VaccineHub.Web.Models
{
    public class JobScheduleDto
    {
        public Type JobType { get; }
        public string CronExpression { get; }
        public JobScheduleDto(Type jobType, string cronExpression)
        {
            JobType = jobType;
            CronExpression = cronExpression;
        }
       
    }
}