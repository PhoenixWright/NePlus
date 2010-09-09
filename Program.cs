using System;

namespace NePlus
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (NePlus game = new NePlus())
            {
                game.Run();
            }
        }
    }
}

