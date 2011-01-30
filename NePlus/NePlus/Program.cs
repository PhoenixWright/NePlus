namespace NePlus
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (NePlusGame game = new NePlusGame())
            {
                game.Run();
            }
        }
    }
#endif
}