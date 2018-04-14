using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using TallyJ3.Extensions;

namespace TallyJ3.Data.DbModel
{
    //[MetadataType(typeof(PersonMetadata))]
    public partial class Person
    {
        //enum ExtraSettingKey
        //{
        //    // keep names as short as possible
        //    RegLog, // registration log
        //}

        public string FullName
        {
            get
            {
                return new[]
                {
          LastName,
          OtherLastNames.SurroundContentWith(" [", "]"),
          FirstName.SurroundContentWith(", ", ""),
          OtherNames.SurroundContentWith(" [", "]"),
          OtherInfo.SurroundContentWith(" (", ")")
        }.JoinedAsString("", true);
            }
        }

        public string FullNameAndArea
        {
            get { return FullNameFL + Area.SurroundContentWith(" (", ")"); }
        }

        public string FullNameFL
        {
            get
            {
                return new[]
                {
          FirstName.SurroundContentWith("", " "),
          LastName,
          OtherNames.SurroundContentWith(" [", "]"),
          OtherLastNames.SurroundContentWith(" [", "]"),
          OtherInfo.SurroundContentWith(" (", ")")
        }.JoinedAsString("", true);
            }
        }

        private class PersonMetadata
        {
            [DebuggerDisplay("Local = {RegistrationTime.ToLocalTime()}, UTC = {RegistrationTime}")]
            public object RegistrationTime { get; set; }
        }

        /// <summary>
        /// </summary>
        /// <remarks>
        /// The values in the string must not contain <see cref="SplitChar"/> ~ or <see cref="ArraySplit"/> |
        /// </remarks>
        [NotMapped]
        public List<string> RegistrationLogAsList
        {
            get
            {
                return RegistrationLog.HasContent() ? RegistrationLog.Split(ArraySplit).ToList() : new List<string>();
            }
            set
            {
                RegistrationLog = value.JoinedAsString(ArraySplit);
            }
        }


        //const char FlagChar = '~';
        //const char SplitChar = '~';
        const char ArraySplit = '|'; // for when the value is an array or List<string>

        //private Dictionary<ExtraSettingKey, string> _extraDict;
        //private Dictionary<ExtraSettingKey, string> ExtraSettings
        //{
        //    get
        //    {
        //        if (_extraDict != null)
        //        {
        //            return _extraDict;
        //        }
        //        // column contents...  ~Flag=1;FlagB=hello

        //        if (string.IsNullOrWhiteSpace(CombinedSoundCodes) || CombinedSoundCodes[0] != FlagChar)
        //        {
        //            _extraDict = new Dictionary<ExtraSettingKey, string>();
        //        }
        //        else
        //        {
        //            _extraDict = CombinedSoundCodes
        //                .Substring(1) // skip flag char
        //                .Trim()
        //                .Split(SplitChar)
        //                .Select(s => s.Split('='))
        //                .Where(a => Enum.IsDefined(typeof(ExtraSettingKey), a[0]))
        //                // any that are not recognized are ignored and lost
        //                .ToDictionary(a => (ExtraSettingKey)Enum.Parse(typeof(ExtraSettingKey), a[0]), a => a[1]);
        //        }

        //        return _extraDict;
        //    }
        //}


        //private string GetExtraSetting(ExtraSettingKey setting)
        //{
        //    string value;
        //    if (ExtraSettings.TryGetValue(setting, out value))
        //    {
        //        return value;
        //    }
        //    return null;
        //}

        //private void SetExtraSettting(ExtraSettingKey setting, string value)
        //{
        //    var s = value ?? "";
        //    if (s.Contains("=") || s.Contains(SplitChar))
        //    {
        //        throw new ApplicationException("Invalid value for extra settings: " + s);
        //    }

        //    var dict = ExtraSettings;

        //    if (s == "")
        //    {
        //        if (dict.ContainsKey(setting))
        //        {
        //            dict.Remove(setting);
        //        }
        //    }
        //    else
        //    {
        //        dict[setting] = s;
        //    }

        //    if (dict.Count == 0)
        //    {
        //        CombinedSoundCodes = null;
        //    }
        //    else
        //    {
        //        CombinedSoundCodes = FlagChar + dict.Select(kvp => kvp.Key + "=" + kvp.Value).JoinedAsString(SplitChar);
        //    }

        //    _extraDict = dict;
        //}
    }
}