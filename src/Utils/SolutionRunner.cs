using Jint;
using System;
using System.IO;
using System.Diagnostics;
using Microsoft.Scripting.Hosting;
using AdventOfCode.Solutions;

namespace AdventOfCode.Utils
{
  public static class SolutionRunner
  {
    private static Stopwatch _timer;
    private static string _part1Answer;
    private static string _part2Answer;
    private static long _part1Time;
    private static long _part2Time;
    private static bool _part1Solved;
    private static bool _part2Solved;
    public delegate void DelReport(object solution);
    public static DelReport delPartOne = SubmitPartOne;
    public static DelReport delPartTwo = SubmitPartTwo;

    public static void RunSolver(int year, int day, Program.Language lang)
    {
      _timer = new Stopwatch();
      _part1Answer = string.Empty;
      _part2Answer = string.Empty;
      _part1Solved = false;
      _part2Solved = false;
      _part1Time = 0;
      _part2Time = 0;

      PuzzleInput input = new PuzzleInput(year, day);

      switch (lang)
      {
        case Program.Language.cs:
          SolveCSharp(input, year, day);
          break;
        case Program.Language.js:
          SolveJavaScript(input, year, day);
          break;
        case Program.Language.py:
          SolvePython(input, year, day);
          break;
      }
    }

    public static void SolveCSharp(PuzzleInput input, int year, int day)
    {
      // Get the name of the class for the given year and day
      string typeName = $"AdventOfCode.Solutions.Solution_{year}_{day:D2}";
      Type typeType = Type.GetType(typeName);
      CSharpSolution solution = (CSharpSolution)Activator.CreateInstance(typeType);

      _timer.Start();
      solution.Solve(input);
      _timer.Stop();
    }

    public static void SolveJavaScript(PuzzleInput input, int year, int day)
    {
      string scriptText = File.ReadAllText(Path.Combine(IOUtils.JSolutionPath(year, day)));
      Engine engine = new Engine();

      engine.SetValue("input", input.GetRaw());
      engine.SetValue("SubmitPartOne", delPartOne);
      engine.SetValue("SubmitPartTwo", delPartTwo);
      engine.Execute(scriptText);

      _timer.Start();
      engine.Invoke("solve");
      _timer.Stop();
    }

    public static void SolvePython(PuzzleInput input, int year, int day)
    {
      string scriptText = File.ReadAllText(Path.Combine(IOUtils.PSolutionPath(year, day)));
      ScriptEngine engine = IronPython.Hosting.Python.CreateEngine();
      ScriptScope scope = engine.CreateScope();
      engine.Execute(scriptText, scope);
      scope.SetVariable("submit_part_one", delPartOne);
      scope.SetVariable("submit_part_two", delPartTwo);
      dynamic solve = scope.GetVariable("solve");

      _timer.Start();
      solve(input.GetRaw());
      _timer.Stop();
    }

    public static (string solution, double time) GetPartOne()
    {
      if (_part1Solved)
      {
        return (_part1Answer, _part1Time);
      }
      else
      {
        return ("Unsolved", 0);
      }
    }

    public static (string solution, double time) GetPartTwo()
    {
      if (_part2Solved)
      {
        return (_part2Answer, _part2Time);
      }
      else
      {
        return ("Unsolved", 0);
      }
    }

    private static void SubmitPartOne(object answer)
    {
      _part1Time = _timer.ElapsedMilliseconds;
      _part1Answer = answer.ToString();
      _part1Solved = true;
    }

    private static void SubmitPartTwo(object answer)
    {
      _part2Time = _timer.ElapsedMilliseconds;
      _part2Answer = answer.ToString();
      _part2Solved = true;
    }
  }
}
