using VaccineHub.Service.Abstractions;

namespace VaccineHub.Web.Scheduler.Visitable
{
    public interface IVisitable
    {
        void Accept(IVisitor visitor);
    }
}