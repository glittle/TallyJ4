using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TallyJ4Tests.Support
{
  public static class Extensions
  {

    public static void ShouldEqual<T>(this T actual, T expected)
    {
      ShouldEqual<T>(actual, expected, null);
    }

    public static void ShouldEqual<T>(this T actual, T expected, string comment)
    {
      Assert.AreEqual(expected, actual, comment);
    }

    public static void ShouldNotEqual<T>(this T actual, T expected)
    {
      ShouldNotEqual<T>(actual, expected, null);
    }

    public static void ShouldNotEqual<T>(this T actual, T expected, string comment)
    {
      Assert.AreNotEqual(expected, actual, comment);
    }
  }
}