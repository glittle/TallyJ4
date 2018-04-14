
using TallyJ3.Code.Hubs;
using TallyJ3.Data;

namespace TallyJ3.Models
{
    public interface IAnalyzerFakes
  {
    ApplicationDbContext DbContext { get; }

    IStatusUpdateHub FakeHub { get; }
  }
}