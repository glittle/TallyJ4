using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TallyJ4.Extensions;
using TallyJ4Tests.Support;

namespace TallyJ4Tests.FrameworkTests
{
  [TestClass]
  public class ReflectionHelperTests
  {

    [TestMethod]
    public void GetNameForObject_Test()
    {
      var sut = new TestPropertyNames();

      ReflectionHelper.GetName(() => sut.D2).ShouldEqual("D2");
      ReflectionHelper.GetName(() => sut.S2).ShouldEqual("S2");
      ReflectionHelper.GetName(() => sut.I2).ShouldEqual("I2");
    }

    [TestMethod]
    public void GetNameForClass_Test()
    {
      var sut = default(TestPropertyNames);
      ReflectionHelper.GetName(() => sut.D2).ShouldEqual("D2");
      ReflectionHelper.GetName(() => sut.S2).ShouldEqual("S2");
      ReflectionHelper.GetName(() => sut.I2).ShouldEqual("I2");
    }

    [TestMethod]
    public void GetNameExt_Test()
    {
      ((TestPropertyNames)null).GetPropertyName(x => x.D2).ShouldEqual("D2");

      var obj = new TestPropertyNames();
      obj.GetPropertyName(x => x.D2).ShouldEqual("D2");
    }


  }

  internal class TestPropertyNames
  {
#pragma warning disable 0649
    public string S1;
    public string S2;
    public int I2;
    public DateTime D2;
#pragma warning restore 0649
  }
}