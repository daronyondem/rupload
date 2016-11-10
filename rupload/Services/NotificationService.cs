using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace rupload.Services
{
    public class NotificationService : INotificationService
    {
        public async Task CreateToast(ToastTemplateType templateType, string text, string title = null, string imgPath = null)
        {
            await Task.Run(() =>
            {
                var toastTemplate = ToastNotificationManager.GetTemplateContent(templateType);
                var stringElements = toastTemplate.GetElementsByTagName("text");
                if (!string.IsNullOrEmpty(title))
                {
                    stringElements[0].AppendChild(toastTemplate.CreateTextNode(title));
                }
                stringElements[1].AppendChild(toastTemplate.CreateTextNode(text));
                if (!string.IsNullOrEmpty(imgPath))
                {
                    // Specify the absolute path to an image
                    //var imagePath = "file:///" + Path.GetFullPath("toastImageAndText.png");
                    var imageElements = toastTemplate.GetElementsByTagName("image");
                    imageElements[0].Attributes.GetNamedItem("src").NodeValue = imgPath;
                }
                // Show the toast. Be sure to specify the AppUserModelId on your application's shortcut!
                var applicationId = typeof(App).Namespace;
                ToastNotificationManager.CreateToastNotifier(applicationId).Show(new ToastNotification(toastTemplate));
            });
        }
    }
}
