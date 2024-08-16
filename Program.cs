using PrimerProyecto;

internal class Program
{
    private static void Main(string[] args)
    {
        using (LetraT game = new LetraT())
        {
            game.Run();
        }
    }
}