using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace rupload
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();
#if DEBUG
            args = new string[2] {"", @"C:\Dropbox\Desktop\powershell.txt"};
#endif
            if (args.Length > 1)
            {
                string filename = args[1];

                CloudStorageAccount account = CloudStorageAccount.Parse(System.Configuration.ConfigurationManager.AppSettings["StorageConnection"].ToString());
                CloudBlobClient blobClient = account.CreateCloudBlobClient();

                CloudBlobContainer container = blobClient.GetContainerReference("ruploads");
                container.CreateIfNotExists();

                BlobContainerPermissions containerPermissions = new BlobContainerPermissions();
                containerPermissions.PublicAccess = BlobContainerPublicAccessType.Blob;
                container.SetPermissions(containerPermissions);
                
                var blob = container.GetBlockBlobReference(System.IO.Path.GetFileName(filename));
                blob.UploadFromFile(filename, System.IO.FileMode.Open);

                Clipboard.SetText(blob.Uri.ToString());

                Application.Current.Shutdown();
            }
        }
    }
}
