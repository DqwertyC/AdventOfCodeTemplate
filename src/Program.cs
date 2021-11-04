using AdventOfCode.Utils;
using AdventOfCode.Solutions;
using System;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace AdventOfCode
{
  public class Program
  {
    public enum Language
    {
      cs,
      js,
      py
    }

    static void Main(string[] args)
    {
      Language lang = Language.cs;
      int year = 0;
      int day = 0;

      bool yearSet = false;
      bool daySet = false;
      bool helpSet = false;
      bool initSet = false;
      bool langSet = false;

      try
      {
        for (int i = 0; i < args.Length; i++)
        {
          if (args[i].Equals("--year") || args[i].Equals("--y"))
          {
            if (yearSet)
            {
              Console.Error.WriteLine("Year specified multiple times!");
              throw new Exception();
            }

            yearSet = true;
            i++;

            if (!int.TryParse(args[i], out year))
            {
              Console.Error.WriteLine("Year must be an integer!");
              throw new Exception();
            }

            if (year < 2015)
            {
              Console.Error.Write("Year must be at least 2015!");
              throw new Exception();
            }
          }
          else if (args[i].Equals("--day") || args[i].Equals("--d"))
          {
            if (daySet)
            {
              Console.Error.WriteLine("Day specified multiple times!");
              throw new Exception();
            }

            daySet = true;
            i++;

            if (!int.TryParse(args[i], out day))
            {
              Console.Error.WriteLine("Day must be an integer!");
              throw new Exception();
            }

            if (day < 1 || day > 25)
            {
              Console.Error.WriteLine("Day must be between 1 and 25 (inclusive)!");
              throw new Exception();
            }
          }
          else if (args[i].Equals("--lang") || args[i].Equals("--l"))
          {
            if (langSet)
            {
              Console.Error.WriteLine("Language specified multiple times!");
              throw new Exception();
            }

            langSet = true;
            i++;

            lang = IOUtils.GetLanguage(args[i]);
          }
          else if (args[i].Equals("--help") || args[i].Equals("--h"))
          {
            helpSet = true;
          }
          else if (args[i].Equals("--init"))
          {
            initSet = true;
          }
        }
      }
      catch (Exception e)
      {
        Console.Error.WriteLine(e.Message);
        Console.Error.PrintUsage();
      }

      // Now that we've parsed all the inputs...
      if (helpSet)
      {
        Console.Out.PrintUsage();
      }

      JObject config = IOUtils.ConfigObject();

      if (!yearSet)
      {
        if (config.ContainsKey("year"))
        {
          year = (int)config["year"];
          yearSet = true;
        }
        else
        {
          Console.Error.WriteLine("Please specify a year!");
        }
      }

      if (!daySet && config.ContainsKey("day"))
      {
        day = (int)config["day"];
        daySet = true;
      }

      if (!langSet)
      {
        if (config.ContainsKey("lang"))
        {
          lang = IOUtils.GetLanguage((string)config["lang"]);
          langSet = true;
        }
        else
        {
          Console.Error.WriteLine("Please specify a language!");
        }
      }

      var pid = Process.GetCurrentProcess().Id;
      File.WriteAllText(IOUtils.PIDPath(), $"{pid}");

      if (Language.py == lang)
      {
        Console.WriteLine("ready for python debugger");
      }
      else if (Language.js == lang)
      {
        Console.WriteLine("ready for javascript debugger");
      }

      if (initSet)
      {
        if (!Directory.Exists(lang.SolutionPath(year)))
        {
          Directory.CreateDirectory(lang.SolutionPath(year));
        }

        if (!Directory.Exists(IOUtils.InputPath(year)))
        {
          Directory.CreateDirectory(IOUtils.InputPath(year));
        }

        string template = File.ReadAllText(lang.TemplatePath());

        for (int i = 1; i <= 25; i++)
        {
          string solutionPath = lang.SolutionPath(year, i);

          if (!File.Exists(solutionPath))
          {
            string newFile = template;

            newFile = newFile.Replace("20YY", $"{year}");
            newFile = newFile.Replace("DD", $"{i:D2}");
            File.WriteAllText(solutionPath, newFile);
          }
        }
      }
      else
      {
        Console.Out.WriteLine("----------------------------------------");
        for (int i = 1; i <= 25; i++)
        {
          if (day == 0 || day == i)
          {
            if (File.Exists(lang.SolutionPath(year, i)))
            {
              Console.Out.WriteLine($"{year} day {i:D2}");
              SolutionRunner.RunSolver(year, i, lang);
              Console.Out.PrintSolution(SolutionRunner.GetPartOne(), SolutionRunner.GetPartTwo());
              Console.Out.WriteLine("----------------------------------------");
            }
          }
        }
      }
    }
  }
}
