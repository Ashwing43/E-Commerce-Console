namespace CustomLogger
{
    public static class Logger
    {
        public static void LogError(Exception ex)
        {
            string filePath = Constants.Constants.logPath;

            string str = "*********************************************************************\n";
            File.AppendAllText(filePath, str);
            
            File.AppendAllText(filePath, "Exception occured at: ");
            File.AppendAllText(filePath, DateTime.Now.ToString());
            
            File.AppendAllText(filePath, "\n");
            File.AppendAllText(filePath, ex.ToString());
            
            File.AppendAllText(filePath, "\nMessage: ");
            File.AppendAllText(filePath, "ex.Message");
            
            File.AppendAllText(filePath, "\n\n");
        }
    }
}
