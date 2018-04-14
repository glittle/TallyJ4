using System;

namespace TallyJ4.Code.Misc
{
  public class AssertAtRuntime
  {
    public static void That(bool trueCondition, string exceptionMessage = "Invalid condition in code")
    {
      if (!trueCondition)
      {
        throw new ApplicationException(exceptionMessage);
      }
    }
  }
}