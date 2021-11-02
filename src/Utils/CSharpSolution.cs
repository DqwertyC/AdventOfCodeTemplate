using AdventOfCode.Utils;

namespace AdventOfCode.Solutions
{
  public abstract class CSharpSolution
  {
    public abstract void Solve(PuzzleInput input);
    protected static SolutionRunner.DelReport SubmitPartOne = SolutionRunner.delPartOne;
    protected static SolutionRunner.DelReport SubmitPartTwo = SolutionRunner.delPartTwo;
  }
}
