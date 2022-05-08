namespace IFP.Utils
{
    static class SiteNav
    {
        /// <summary>
        /// Uses explorer.exe to open link in the default browser 
        /// </summary>
        /// <param name="url"></param>
        public static void GoToSite(string url)
        {
            System.Diagnostics.Process.Start("explorer.exe", url);
        }
    }
}
