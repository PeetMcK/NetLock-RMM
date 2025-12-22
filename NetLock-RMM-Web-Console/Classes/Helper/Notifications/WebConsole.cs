using MudBlazor;

namespace NetLock_RMM_Web_Console.Classes.Helper.Notifications
{
    /// <summary>
    /// Helper class for displaying beautiful notifications in the Web Console
    /// Uses MudBlazor Snackbar to show toast notifications in the bottom-right corner
    /// </summary>
    public static class WebConsole
    {
        /// <summary>
        /// Shows a success notification (green)
        /// </summary>
        /// <param name="snackbar">MudBlazor ISnackbar instance</param>
        /// <param name="message">Message to display</param>
        /// <param name="duration">Duration in milliseconds (default: 3000ms)</param>
        public static void Success(ISnackbar snackbar, string message, int duration = 3000)
        {
            snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
            snackbar.Configuration.SnackbarVariant = Variant.Filled;
            snackbar.Configuration.VisibleStateDuration = duration;
            snackbar.Configuration.ShowCloseIcon = true;
            snackbar.Configuration.MaxDisplayedSnackbars = 5;
            
            snackbar.Add(message, Severity.Success);
        }

        /// <summary>
        /// Shows an info notification (blue)
        /// </summary>
        /// <param name="snackbar">MudBlazor ISnackbar instance</param>
        /// <param name="message">Message to display</param>
        /// <param name="duration">Duration in milliseconds (default: 3000ms)</param>
        public static void Info(ISnackbar snackbar, string message, int duration = 3000)
        {
            snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
            snackbar.Configuration.SnackbarVariant = Variant.Filled;
            snackbar.Configuration.VisibleStateDuration = duration;
            snackbar.Configuration.ShowCloseIcon = true;
            snackbar.Configuration.MaxDisplayedSnackbars = 5;
            
            snackbar.Add(message, Severity.Info);
        }

        /// <summary>
        /// Shows a warning notification (orange)
        /// </summary>
        /// <param name="snackbar">MudBlazor ISnackbar instance</param>
        /// <param name="message">Message to display</param>
        /// <param name="duration">Duration in milliseconds (default: 4000ms)</param>
        public static void Warning(ISnackbar snackbar, string message, int duration = 4000)
        {
            snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
            snackbar.Configuration.SnackbarVariant = Variant.Filled;
            snackbar.Configuration.VisibleStateDuration = duration;
            snackbar.Configuration.ShowCloseIcon = true;
            snackbar.Configuration.MaxDisplayedSnackbars = 5;
            
            snackbar.Add(message, Severity.Warning);
        }

        /// <summary>
        /// Shows an error notification (red)
        /// </summary>
        /// <param name="snackbar">MudBlazor ISnackbar instance</param>
        /// <param name="message">Message to display</param>
        /// <param name="duration">Duration in milliseconds (default: 5000ms)</param>
        public static void Error(ISnackbar snackbar, string message, int duration = 5000)
        {
            snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
            snackbar.Configuration.SnackbarVariant = Variant.Filled;
            snackbar.Configuration.VisibleStateDuration = duration;
            snackbar.Configuration.ShowCloseIcon = true;
            snackbar.Configuration.MaxDisplayedSnackbars = 5;
            
            snackbar.Add(message, Severity.Error);
        }

        /// <summary>
        /// Shows a normal notification (default gray)
        /// </summary>
        /// <param name="snackbar">MudBlazor ISnackbar instance</param>
        /// <param name="message">Message to display</param>
        /// <param name="duration">Duration in milliseconds (default: 3000ms)</param>
        public static void Normal(ISnackbar snackbar, string message, int duration = 3000)
        {
            snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
            snackbar.Configuration.SnackbarVariant = Variant.Filled;
            snackbar.Configuration.VisibleStateDuration = duration;
            snackbar.Configuration.ShowCloseIcon = true;
            snackbar.Configuration.MaxDisplayedSnackbars = 5;
            
            snackbar.Add(message, Severity.Normal);
        }

        /// <summary>
        /// Shows a custom notification with emoji
        /// </summary>
        /// <param name="snackbar">MudBlazor ISnackbar instance</param>
        /// <param name="message">Message to display</param>
        /// <param name="emoji">Emoji to add (e.g., "✅", "🚀", "🔔")</param>
        /// <param name="severity">Severity level</param>
        /// <param name="duration">Duration in milliseconds (default: 3000ms)</param>
        public static void Custom(ISnackbar snackbar, string message, string emoji, Severity severity = Severity.Normal, int duration = 3000)
        {
            snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
            snackbar.Configuration.SnackbarVariant = Variant.Filled;
            snackbar.Configuration.VisibleStateDuration = duration;
            snackbar.Configuration.ShowCloseIcon = true;
            snackbar.Configuration.MaxDisplayedSnackbars = 5;
            
            snackbar.Add($"{emoji} {message}", severity);
        }

        /// <summary>
        /// Shows an auto-refresh notification (for background updates)
        /// </summary>
        /// <param name="snackbar">MudBlazor ISnackbar instance</param>
        /// <param name="itemType">Type of items refreshed (e.g., "Events", "Devices")</param>
        /// <param name="count">Number of items refreshed</param>
        public static void AutoRefresh(ISnackbar snackbar, string itemType, int count)
        {
            snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
            snackbar.Configuration.SnackbarVariant = Variant.Filled;
            snackbar.Configuration.VisibleStateDuration = 2000;
            snackbar.Configuration.ShowCloseIcon = false;
            snackbar.Configuration.MaxDisplayedSnackbars = 5;
            
            snackbar.Add($"🔄 {itemType} refreshed ({count})", Severity.Info);
        }
    }
}

