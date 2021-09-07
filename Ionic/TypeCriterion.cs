
// Type: Ionic.TypeCriterion


// Hacked by SystemAce

using Ionic.Zip;
using System;
using System.IO;
using System.Text;

namespace Ionic
{
  internal class TypeCriterion : SelectionCriterion
  {
    private char ObjectType;
    internal ComparisonOperator Operator;

    internal string AttributeString
    {
      get
      {
        return this.ObjectType.ToString();
      }
      set
      {
        if (value.Length != 1 || (int) value[0] != 68 && (int) value[0] != 70)
          throw new ArgumentException("Specify a single character: either D or F");
        this.ObjectType = value[0];
      }
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("type ").Append(EnumUtil.GetDescription((Enum) this.Operator)).Append(" ").Append(this.AttributeString);
      return stringBuilder.ToString();
    }

    internal override bool Evaluate(string filename)
    {
      bool flag = (int) this.ObjectType == 68 ? Directory.Exists(filename) : File.Exists(filename);
      if (this.Operator != ComparisonOperator.EqualTo)
        flag = !flag;
      return flag;
    }

    internal override bool Evaluate(ZipEntry entry)
    {
      bool flag = (int) this.ObjectType == 68 ? entry.IsDirectory : !entry.IsDirectory;
      if (this.Operator != ComparisonOperator.EqualTo)
        flag = !flag;
      return flag;
    }
  }
}
