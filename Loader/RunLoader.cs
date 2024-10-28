namespace Loader
{
	public static class Loader
	{
		public static void RunLogout()
		{
			Console.Write("Logging out");
			Thread.Sleep(500);
			Console.Write(".");
			Thread.Sleep(500);
			Console.Write(".");
			Thread.Sleep(500);
			Console.WriteLine(".");
		}

		public static void RunExit()
		{
			Console.Write("Exiting");
			Thread.Sleep(500);
			Console.Write(".");
			Thread.Sleep(500);
			Console.Write(".");
			Thread.Sleep(500);
			Console.WriteLine(".");
		}

		public static void PressAnyKeyToExit()
		{
			Console.WriteLine("\nPress any key to return to menu");
			Console.ReadKey();
			Console.Clear();
		}
	}
}
