using Eto.Drawing;
using Eto.Forms;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace DomainScanner;

public class MainForm : Form
{
    private void OpenUrl(string url)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            url = url.Replace("&", "^&");
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Process.Start("xdg-open", url);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Process.Start("open", url);
        }
    }
    
    public MainForm()
    {
        Title = "Website checker";
        MinimumSize = new Size(600, 600);

        TextBox siteName = new()
        {
            PlaceholderText = "Site name"
        };
        FilePicker domainList = new()
        {
            FileAction = Eto.FileAction.OpenFile,
            FilePath = "Path to domain file (each domain is separated by a newline character, no dots)"
        };
        Button button = new()
        {
            Text = "Scan",
        };
        ListBox listBox = new()
        {
            Width = 450,
        };
        listBox.Activated += (_, _) =>
        {
            var websiteName = listBox.SelectedValue.ToString();
            OpenUrl(websiteName);
        };

        button.Click += async (_, _) =>
        {
            var websiteName = siteName.Text;
            if (websiteName.Length == 0)
            {
                MessageBox.Show(this, "Site name is not set");
                return;
            }

            var domainFilePath = domainList.FilePath;
            if (!File.Exists(domainFilePath))
            {
                MessageBox.Show(this, "Domain file list not found");
                return;
            }
                
            listBox.Items.Clear();
            siteName.Enabled = false;
            button.Enabled = false;
            domainList.Enabled = false;
            var domains = File.ReadAllLines(domainFilePath).ToList();

            HttpClient client = new();
            client.Timeout = new TimeSpan(0, 0, 5);
            Scanner scanner = new(client, domains);
            var tasks = scanner.GetDomainsFound(websiteName).ToList();
            while (tasks.Any())
            {
                var finishedTask = await Task.WhenAny(tasks);
                tasks.Remove(finishedTask);
                var resultingString = await finishedTask;
                if (resultingString.Length > 0)
                {
                    listBox.Items.Add(await finishedTask);
                }
            }
            MessageBox.Show(this, "Scanned all websites!");
            siteName.Enabled = true;
            button.Enabled = true;
            domainList.Enabled = true;
        };

        TableLayout layout = new() { Padding = new Padding(10), Spacing = new Size(5, 5) };
        layout.Rows.Add(siteName);
        layout.Rows.Add(domainList);
        layout.Rows.Add(button);
        layout.Rows.Add(listBox);

        Content = layout;
    }
}