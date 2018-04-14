
using TallyJ4.Data.DbModel;
using TallyJ4.Extensions;

namespace TallyJ4.Code.Helper
{
  public static class PersonHelper
  {
    public const string WordSeparator = " ";

    public static string MakeCombinedInfo(this Person person)
    {
      return new[]
      {
        person.FirstName,
        person.LastName,
        person.BahaiId,
        person.OtherNames,
        person.OtherLastNames,
        person.OtherInfo,
        //person.Area,
      }
        .JoinedAsString(WordSeparator, true)
        .ReplacePunctuation(WordSeparator[0])
        .WithoutDiacritics(true);
    }
  }
}