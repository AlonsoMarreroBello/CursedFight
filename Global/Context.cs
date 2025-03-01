namespace CursedFight.Global
{
  public static class Context
  {
    private static string _actual_stage = "fight_club";

    public static void SetActualStage(string stageName)
    {
      _actual_stage = stageName;
    }

    public static string GetActualStage()
    {
      return _actual_stage;
    }
  }
}

