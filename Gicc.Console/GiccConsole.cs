﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using Gicc;

namespace Gicc.Console
{
  public class GiccConsole
  {
    static void Main(string[] args)
    {
      if (args.Length == 0)
      {
        WriteLine(Usage.Main);
        return;
      }

      ClearCase cc;

      switch (args[0].ToLower())
      {
        case "clone":
          new Gicc(Environment.CurrentDirectory, args[1], args[2], args[3]).Clone();
          break;

        case "pull":
          new Gicc(Environment.CurrentDirectory, true).Pull();
          break;

        case "push":
          new Gicc(Environment.CurrentDirectory, true).Push();
          break;

        case "list":
          if (args.Length < 2)
          {
            WriteLine(Usage.List);
            return;
          }
          cc = new ClearCase(CreateCCInfo(args[1], Environment.CurrentDirectory));
          cc.FindAllFilesInBranch().ForEach(file => WriteLine(file));
          break;

        case "tree":
          if (args.Length < 2)
          {
            WriteLine(Usage.Tree);
            return;
          }
          cc = new ClearCase(CreateCCInfo(args[1], Environment.CurrentDirectory));
          cc.FindAllFilesInBranch().ForEach(filePath => cc.ViewVersionTree(filePath));
          break;

        case "label":
          Label(args);
          break;

        case "cs":
          ConfigSpec(args);
          break;

        default:
          WriteLine(Usage.Main);
          return;
      }
    }

    static private void Label(string[] args)
    {
      if (args.Length < 4)
      {
        WriteLine(Usage.Label);
        return;
      }

      string labeledBranch;

      switch (args[1].ToLower())
      {
        case "-main": case "-m":
          labeledBranch = "main";
          break;

        case "-branch": case "-b":
          labeledBranch = "main\\" + args[2];
          break;

        default:
          WriteLine(Usage.Label);
          return;
      }

      ClearCase cc = new ClearCase(CreateCCInfo(labeledBranch, Environment.CurrentDirectory));
      cc.LabelLastElements(labeledBranch, args[3]);
    }

    static private void ConfigSpec(string[] args)
    {
      switch (args.Length)
      {
        case 2:
          new ClearCase(CreateCCInfo("", Environment.CurrentDirectory)).CatCS()
            .ForEach(line => WriteLine(line));
          break;
        /*
        case 3:
          new Gicc(Environment.CurrentDirectory, args[2]).SetBranchCS();
          break;
        */
        default:
          WriteLine(Usage.CS);
          return;
      }
    }

    /// <summary>
    /// Git 과 상관 없이 CC 를 실행 할 때 사용하는 생성자 정보
    /// </summary>
    static private ClearCaseConstructInfo CreateCCInfo(string branchName, string executingPath)
    {
      return new ClearCaseConstructInfo()
      {
        BranchName = branchName,
        ExecutingPath = executingPath,
        OutPath = Path.Combine(executingPath, "giccout.txt"),
        LogPath = Path.Combine(executingPath, "gicclog.txt")
      };
    }

    static void WriteLine(string value)
    {
      System.Console.WriteLine(value);
    }
  }
}
