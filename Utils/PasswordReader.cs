using System.Text;

namespace Utils
{
    public static class PasswordReader
    {
        public static string ReadPassword()
        {
            StringBuilder password = new StringBuilder();
            ConsoleKeyInfo keyInfo;

            do
            {
                keyInfo = Console.ReadKey(true);
                if (keyInfo.Key != ConsoleKey.Backspace && keyInfo.Key != ConsoleKey.Enter)
                {
                    password.Append(keyInfo.KeyChar);
                    Console.Write("*");
                }
                else if (keyInfo.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password.Remove(password.Length - 1, 1);
                    Console.Write("\b \b");
                }
                if(password.ToString() == Constants.Constants.BACK) return Constants.Constants.BACK;
            } while (keyInfo.Key != ConsoleKey.Enter);

            return password.ToString();
        }
    }
}
