
using TallyJ4.Code.Hubs;
using TallyJ4.Data;

namespace TallyJ4.Models
{
    public interface IAnalyzerFakes
  {
    ApplicationDbContext DbContext { get; }

    IStatusUpdateHub FakeHub { get; }
  }
}