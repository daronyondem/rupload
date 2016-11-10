using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace rupload.Services
{
    public interface INotificationService
    {
        Task CreateToast(ToastTemplateType templateType, string text, string title = null, string imgPath = null);
    }
}
