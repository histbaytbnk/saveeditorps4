
// Type: Ionic.NameCriterion


// Hacked by SystemAce

using Ionic.Zip;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Ionic
{
  internal class NameCriterion : SelectionCriterion
  {
    private Regex _re;
    private string _regexString;
    internal ComparisonOperator Operator;
    private string _MatchingFileSpec;

    internal virtual string MatchingFileSpec
    {
      set
      {
        this._MatchingFileSpec = !Directory.Exists(value) ? value : ".\\" + value + "\\*.*";
        this._regexString = "^" + Regex.Escape(this._MatchingFileSpec).Replace("\\\\\\*\\.\\*", "\\\\([^\\.]+|.*\\.[^\\\\\\.]*)").Replace("\\.\\*", "\\.[^\\\\\\.]*").Replace("\\*", ".*").Replace("\\?", "[^\\\\\\.]") + "$";
        this._re = new Regex(this._regexString, RegexOptions.IgnoreCase);
      }
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("name ").Append(EnumUtil.GetDescription((Enum) this.Operator)).Append(" '").Append(this._MatchingFileSpec).Append("'");
      return stringBuilder.ToString();
    }

    internal override bool Evaluate(string filename)
    {
      return this._Evaluate(filename);
    }

    private bool _Evaluate(string fullpath)
    {
      bool flag = this._re.IsMatch(this._MatchingFileSpec.IndexOf('\\') == -1 ? Path.GetFileName(fullpath) : fullpath);
      if (this.Operator != ComparisonOperator.EqualTo)
        flag = !flag;
      return flag;
    }

    internal override bool Evaluate(ZipEntry entry)
    {
      return this._Evaluate(entry.FileName.Replace("/", "\\"));
    }
  }
}
