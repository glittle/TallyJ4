using System.Collections;

namespace TallyJ3.Code.Session
{
  public interface ICurrentContext
  {
    IDictionary Items { get; }

    ISessionWrapper Session { get; }
  }
}