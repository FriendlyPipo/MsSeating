namespace Seats.Core.Database
{
    public interface ISeatsDbContextTransactionProxy : IDisposable
    {
        void Commit();
        void Rollback();
    }
}
