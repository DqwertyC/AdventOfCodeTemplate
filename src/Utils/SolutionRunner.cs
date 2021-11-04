using Jint;
using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using AdventOfCode.Solutions;
using Python.Runtime;
using Python;

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
    public delegate void cDelSubmit(object solution);
    public static cDelSubmit cDelOne = CSubmitPartOne;
    public static cDelSubmit cDelTwo = CSubmitPartTwo;
    public delegate void pDelSubmit(PyObject solution);
    public static pDelSubmit pDelOne = PSubmitPartOne;
    public static pDelSubmit pDelTwo = PSubmitPartTwo;

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
      engine.SetValue("SubmitPartOne", cDelOne);
      engine.SetValue("SubmitPartTwo", cDelTwo);
      engine.Execute(scriptText);

      _timer.Start();
      engine.Invoke("solve");
      _timer.Stop();
    }

    public static void SolvePython(PuzzleInput input, int year, int day)
    {
      string solutionPath = IOUtils.PSolutionPath(year, day);

      Runtime.PythonDLL = (string)IOUtils.ConfigObject()["pydll"];

      using (Py.GIL())
      {
        PythonEngine.Initialize();
        using (PyModule scope = Py.CreateScope())
        {
          dynamic importlib = Py.Import("importlib.util");
          dynamic pySpec = importlib.spec_from_file_location("solution", solutionPath);
          dynamic pyModule = importlib.module_from_spec(pySpec);
          pySpec.loader.exec_module(pyModule);

          pyModule.init(pDelOne, pDelTwo);
          pyModule.solve(input.GetRaw());
        }
      }
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

    private static void CSubmitPartOne(object answer)
    {
      _part1Time = _timer.ElapsedMilliseconds;
      _part1Answer = answer.ToString();
      _part1Solved = true;
    }

    private static void CSubmitPartTwo(object answer)
    {
      _part2Time = _timer.ElapsedMilliseconds;
      _part2Answer = answer.ToString();
      _part2Solved = true;
    }

    private static void PSubmitPartOne(PyObject answer)
    {
      _part1Time = _timer.ElapsedMilliseconds;
      _part1Solved = true;

      using (Py.GIL())
      {
        _part1Answer = answer.ToString();
      }
    }

    private static void PSubmitPartTwo(PyObject answer)
    {
      _part2Time = _timer.ElapsedMilliseconds;
      _part2Solved = true;

      using (Py.GIL())
      {
        _part2Answer = answer.ToString();
      }
    }
  }
}
