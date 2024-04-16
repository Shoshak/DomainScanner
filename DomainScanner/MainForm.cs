using Eto.Drawing;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DomainScanner
{
    public partial class MainForm : Form
    {
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
            listBox.Activated += (sender, e) =>
            {
                string websiteName = listBox.SelectedValue.ToString();
                Process.Start(new ProcessStartInfo(websiteName) { UseShellExecute = true });
            };

            button.Click += async (sender, e) =>
            {
                string websiteName = siteName.Text;
                if (websiteName.Length == 0)
                {
                    MessageBox.Show(this, "Site name is not set");
                    return;
                }

                string domainFilePath = domainList.FilePath;
                if (!File.Exists(domainFilePath))
                {
                    MessageBox.Show(this, "Domain file list not found");
                    return;
                }
                
                listBox.Items.Clear();
                siteName.Enabled = false;
                button.Enabled = false;
                domainList.Enabled = false;
                List<string> domains = File.ReadAllLines(domainFilePath).ToList();

                HttpClient client = new();
                client.Timeout = new TimeSpan(0, 0, 5);
                Scanner scanner = new(client, domains);
                List<Task<string>> tasks = scanner.GetDomainsFound(websiteName).ToList();
                while (tasks.Any())
                {
                    Task<string> finishedTask = await Task.WhenAny(tasks);
                    tasks.Remove(finishedTask);
                    string resultingString = await finishedTask;
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
}
