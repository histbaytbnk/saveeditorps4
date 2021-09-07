
// Type: Ionic.FileSelector


// Hacked by SystemAce

using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace Ionic
{
  public class FileSelector
  {
    internal SelectionCriterion _Criterion;

    public string SelectionCriteria
    {
      get
      {
        if (this._Criterion == null)
          return (string) null;
        return this._Criterion.ToString();
      }
      set
      {
        if (value == null)
          this._Criterion = (SelectionCriterion) null;
        else if (value.Trim() == "")
          this._Criterion = (SelectionCriterion) null;
        else
          this._Criterion = FileSelector._ParseCriterion(value);
      }
    }

    public bool TraverseReparsePoints { get; set; }

    public FileSelector(string selectionCriteria)
      : this(selectionCriteria, true)
    {
    }

    public FileSelector(string selectionCriteria, bool traverseDirectoryReparsePoints)
    {
      if (!string.IsNullOrEmpty(selectionCriteria))
        this._Criterion = FileSelector._ParseCriterion(selectionCriteria);
      this.TraverseReparsePoints = traverseDirectoryReparsePoints;
    }

    private static string NormalizeCriteriaExpression(string source)
    {
      string[][] strArray = new string[11][]
      {
        new string[2]
        {
          "([^']*)\\(\\(([^']+)",
          "$1( ($2"
        },
        new string[2]
        {
          "(.)\\)\\)",
          "$1) )"
        },
        new string[2]
        {
          "\\((\\S)",
          "( $1"
        },
        new string[2]
        {
          "(\\S)\\)",
          "$1 )"
        },
        new string[2]
        {
          "^\\)",
          " )"
        },
        new string[2]
        {
          "(\\S)\\(",
          "$1 ("
        },
        new string[2]
        {
          "\\)(\\S)",
          ") $1"
        },
        new string[2]
        {
          "(=)('[^']*')",
          "$1 $2"
        },
        new string[2]
        {
          "([^ !><])(>|<|!=|=)",
          "$1 $2"
        },
        new string[2]
        {
          "(>|<|!=|=)([^ =])",
          "$1 $2"
        },
        new string[2]
        {
          "/",
          "\\"
        }
      };
      string input = source;
      for (int index = 0; index < strArray.Length; ++index)
      {
        string pattern = FileSelector.RegexAssertions.PrecededByEvenNumberOfSingleQuotes + strArray[index][0] + FileSelector.RegexAssertions.FollowedByEvenNumberOfSingleQuotesAndLineEnd;
        input = Regex.Replace(input, pattern, strArray[index][1]);
      }
      string pattern1 = "/" + FileSelector.RegexAssertions.FollowedByOddNumberOfSingleQuotesAndLineEnd;
      return Regex.Replace(Regex.Replace(input, pattern1, "\\"), " " + FileSelector.RegexAssertions.FollowedByOddNumberOfSingleQuotesAndLineEnd, "\x0006");
    }

    private static SelectionCriterion _ParseCriterion(string s)
    {
      if (s == null)
        return (SelectionCriterion) null;
      s = FileSelector.NormalizeCriteriaExpression(s);
      if (s.IndexOf(" ") == -1)
        s = "name = " + s;
      string[] strArray = s.Trim().Split(new char[2]
      {
        ' ',
        '\t'
      });
      if (strArray.Length < 3)
        throw new ArgumentException(s);
      SelectionCriterion selectionCriterion1 = (SelectionCriterion) null;
      Stack<FileSelector.ParseState> stack1 = new Stack<FileSelector.ParseState>();
      Stack<SelectionCriterion> stack2 = new Stack<SelectionCriterion>();
      stack1.Push(FileSelector.ParseState.Start);
      for (int startIndex = 0; startIndex < strArray.Length; ++startIndex)
      {
        string str1 = strArray[startIndex].ToLower();
        switch (str1)
        {
          case "and":
          case "xor":
          case "or":
            FileSelector.ParseState parseState1 = stack1.Peek();
            if (parseState1 != FileSelector.ParseState.CriterionDone)
              throw new ArgumentException(string.Join(" ", strArray, startIndex, strArray.Length - startIndex));
            if (strArray.Length <= startIndex + 3)
              throw new ArgumentException(string.Join(" ", strArray, startIndex, strArray.Length - startIndex));
            LogicalConjunction logicalConjunction = (LogicalConjunction) Enum.Parse(typeof (LogicalConjunction), strArray[startIndex].ToUpper(), true);
            selectionCriterion1 = (SelectionCriterion) new CompoundCriterion()
            {
              Left = selectionCriterion1,
              Right = (SelectionCriterion) null,
              Conjunction = logicalConjunction
            };
            stack1.Push(parseState1);
            stack1.Push(FileSelector.ParseState.ConjunctionPending);
            stack2.Push(selectionCriterion1);
            break;
          
          case ")":
            stack1.Pop();
            if (stack1.Peek() != FileSelector.ParseState.OpenParen)
              throw new ArgumentException(string.Join(" ", strArray, startIndex, strArray.Length - startIndex));
            int num1 = (int) stack1.Pop();
            stack1.Push(FileSelector.ParseState.CriterionDone);
            break;
          case "atime":
          case "ctime":
          case "mtime":
            if (strArray.Length <= startIndex + 2)
              throw new ArgumentException(string.Join(" ", strArray, startIndex, strArray.Length - startIndex));
            DateTime dateTime1;
            try
            {
              dateTime1 = DateTime.ParseExact(strArray[startIndex + 2], "yyyy-MM-dd-HH:mm:ss", (IFormatProvider) null);
            }
            catch (FormatException ex1)
            {
              try
              {
                dateTime1 = DateTime.ParseExact(strArray[startIndex + 2], "yyyy/MM/dd-HH:mm:ss", (IFormatProvider) null);
              }
              catch (FormatException ex2)
              {
                try
                {
                  dateTime1 = DateTime.ParseExact(strArray[startIndex + 2], "yyyy/MM/dd", (IFormatProvider) null);
                }
                catch (FormatException ex3)
                {
                  try
                  {
                    dateTime1 = DateTime.ParseExact(strArray[startIndex + 2], "MM/dd/yyyy", (IFormatProvider) null);
                  }
                  catch (FormatException ex4)
                  {
                    dateTime1 = DateTime.ParseExact(strArray[startIndex + 2], "yyyy-MM-dd", (IFormatProvider) null);
                  }
                }
              }
            }
            DateTime dateTime2 = DateTime.SpecifyKind(dateTime1, DateTimeKind.Local).ToUniversalTime();
            selectionCriterion1 = (SelectionCriterion) new TimeCriterion()
            {
              Which = (WhichTime) Enum.Parse(typeof (WhichTime), strArray[startIndex], true),
              Operator = (ComparisonOperator) EnumUtil.Parse(typeof (ComparisonOperator), strArray[startIndex + 1]),
              Time = dateTime2
            };
            startIndex += 2;
            stack1.Push(FileSelector.ParseState.CriterionDone);
            break;
          case "length":
          case "size":
            if (strArray.Length <= startIndex + 2)
              throw new ArgumentException(string.Join(" ", strArray, startIndex, strArray.Length - startIndex));
            string str2 = strArray[startIndex + 2];
            long num2 = !str2.ToUpper().EndsWith("K") ? (!str2.ToUpper().EndsWith("KB") ? (!str2.ToUpper().EndsWith("M") ? (!str2.ToUpper().EndsWith("MB") ? (!str2.ToUpper().EndsWith("G") ? (!str2.ToUpper().EndsWith("GB") ? long.Parse(strArray[startIndex + 2]) : long.Parse(str2.Substring(0, str2.Length - 2)) * 1024L * 1024L * 1024L) : long.Parse(str2.Substring(0, str2.Length - 1)) * 1024L * 1024L * 1024L) : long.Parse(str2.Substring(0, str2.Length - 2)) * 1024L * 1024L) : long.Parse(str2.Substring(0, str2.Length - 1)) * 1024L * 1024L) : long.Parse(str2.Substring(0, str2.Length - 2)) * 1024L) : long.Parse(str2.Substring(0, str2.Length - 1)) * 1024L;
            selectionCriterion1 = (SelectionCriterion) new SizeCriterion()
            {
              Size = num2,
              Operator = (ComparisonOperator) EnumUtil.Parse(typeof (ComparisonOperator), strArray[startIndex + 1])
            };
            startIndex += 2;
            stack1.Push(FileSelector.ParseState.CriterionDone);
            break;
          case "filename":
          
          case "attrs":
          case "attributes":
          case "":
            stack1.Push(FileSelector.ParseState.Whitespace);
            break;
          default:
            throw new ArgumentException("'" + strArray[startIndex] + "'");
        }
        FileSelector.ParseState parseState2 = stack1.Peek();
        if (parseState2 == FileSelector.ParseState.CriterionDone)
        {
          int num3 = (int) stack1.Pop();
          if (stack1.Peek() == FileSelector.ParseState.ConjunctionPending)
          {
            while (stack1.Peek() == FileSelector.ParseState.ConjunctionPending)
            {
              CompoundCriterion compoundCriterion = stack2.Pop() as CompoundCriterion;
              compoundCriterion.Right = selectionCriterion1;
              selectionCriterion1 = (SelectionCriterion) compoundCriterion;
              int num4 = (int) stack1.Pop();
              parseState2 = stack1.Pop();
              if (parseState2 != FileSelector.ParseState.CriterionDone)
                throw new ArgumentException("??");
            }
          }
          else
            stack1.Push(FileSelector.ParseState.CriterionDone);
        }
        if (parseState2 == FileSelector.ParseState.Whitespace)
        {
          int num5 = (int) stack1.Pop();
        }
      }
      return selectionCriterion1;
    }

    public override string ToString()
    {
      return "FileSelector(" + this._Criterion.ToString() + ")";
    }

    private bool Evaluate(string filename)
    {
      return this._Criterion.Evaluate(filename);
    }

    [Conditional("SelectorTrace")]
    private void SelectorTrace(string format, params object[] args)
    {
      if (this._Criterion == null || !this._Criterion.Verbose)
        return;
      Console.WriteLine(format, args);
    }

    public ICollection<string> SelectFiles(string directory)
    {
      return (ICollection<string>) this.SelectFiles(directory, false);
    }

    public ReadOnlyCollection<string> SelectFiles(string directory, bool recurseDirectories)
    {
      if (this._Criterion == null)
        throw new ArgumentException("SelectionCriteria has not been set");
      List<string> list = new List<string>();
      try
      {
        if (Directory.Exists(directory))
        {
          foreach (string filename in Directory.GetFiles(directory))
          {
            if (this.Evaluate(filename))
              list.Add(filename);
          }
          if (recurseDirectories)
          {
            foreach (string str in Directory.GetDirectories(directory))
            {
              if (this.TraverseReparsePoints || (File.GetAttributes(str) & FileAttributes.ReparsePoint) == (FileAttributes) 0)
              {
                if (this.Evaluate(str))
                  list.Add(str);
                list.AddRange((IEnumerable<string>) this.SelectFiles(str, recurseDirectories));
              }
            }
          }
        }
      }
      catch (UnauthorizedAccessException ex)
      {
      }
      catch (IOException ex)
      {
      }
      return list.AsReadOnly();
    }

    private bool Evaluate(ZipEntry entry)
    {
      return this._Criterion.Evaluate(entry);
    }

    public ICollection<ZipEntry> SelectEntries(ZipFile zip)
    {
      if (zip == null)
        throw new ArgumentNullException("zip");
      List<ZipEntry> list = new List<ZipEntry>();
      foreach (ZipEntry entry in zip)
      {
        if (this.Evaluate(entry))
          list.Add(entry);
      }
      return (ICollection<ZipEntry>) list;
    }

    public ICollection<ZipEntry> SelectEntries(ZipFile zip, string directoryPathInArchive)
    {
      if (zip == null)
        throw new ArgumentNullException("zip");
      List<ZipEntry> list = new List<ZipEntry>();
      string str = directoryPathInArchive == null ? (string) null : directoryPathInArchive.Replace("/", "\\");
      if (str != null)
      {
        while (str.EndsWith("\\"))
          str = str.Substring(0, str.Length - 1);
      }
      foreach (ZipEntry entry in zip)
      {
        if ((directoryPathInArchive == null || Path.GetDirectoryName(entry.FileName) == directoryPathInArchive || Path.GetDirectoryName(entry.FileName) == str) && this.Evaluate(entry))
          list.Add(entry);
      }
      return (ICollection<ZipEntry>) list;
    }

    private enum ParseState
    {
      Start,
      OpenParen,
      CriterionDone,
      ConjunctionPending,
      Whitespace,
    }

    private static class RegexAssertions
    {
      public static readonly string PrecededByOddNumberOfSingleQuotes = "(?<=(?:[^']*'[^']*')*'[^']*)";
      public static readonly string FollowedByOddNumberOfSingleQuotesAndLineEnd = "(?=[^']*'(?:[^']*'[^']*')*[^']*$)";
      public static readonly string PrecededByEvenNumberOfSingleQuotes = "(?<=(?:[^']*'[^']*')*[^']*)";
      public static readonly string FollowedByEvenNumberOfSingleQuotesAndLineEnd = "(?=(?:[^']*'[^']*')*[^']*$)";
    }
  }
}
