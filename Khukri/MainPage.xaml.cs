using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using Windows.Storage;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Khukri
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
	
    public sealed partial class MainPage : Page
    {
		private List<String> urls = new List<String>();
		private List<String> Articles = new List<String>();
		private List<List<string>> parsedResult = new List<List<string>>();
		private Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();


		public MainPage()
        {
            this.InitializeComponent();
			Windows.UI.ViewManagement.ApplicationView.PreferredLaunchViewSize = new Size(768,800);
			Windows.UI.ViewManagement.ApplicationView.PreferredLaunchWindowingMode = Windows.UI.ViewManagement.ApplicationViewWindowingMode.PreferredLaunchViewSize;
		}

		async void Run_Click(Object sender, RoutedEventArgs e)
		{
			richTextBox.IsEnabled = Run.IsEnabled = plusButton.IsEnabled = minusButton.IsEnabled = false;
			dropText.Text = "Working...";
			Loader1.Visibility = Visibility.Visible;
			urls.Clear();
			Articles.Clear();
			foreach (var item in textFields.Children)
			{
				TextBox x = item as TextBox;
				if (x.Text != "")
				{
					urls.Add(x.Text);
				}
			}

			foreach (var url in urls)
			{
				try
				{
					Uri requestUri = new Uri(url);
					var response = await httpClient.GetAsync(requestUri);
					var text = await response.Content.ReadAsStringAsync();
					Articles.Add(text.ToLower());
				}
				catch (Exception) {}
			}

			richTextBox.IsEnabled = Run.IsEnabled = plusButton.IsEnabled = minusButton.IsEnabled = true;
			dropText.Text = "Drop files here...";
			Loader1.Visibility = Visibility.Collapsed;
			//dragBox.Text = urls.Count.ToString() + ' ' + Articles.Count.ToString();
			KeywordAnalysis();
		}

		void Handle_Input(Object sender, RoutedEventArgs e)
		{
			dragBox.Text = e.ToString();
		}

		delegate void HandleInput(Object sender, RoutedEventArgs e);

		void Grid_DragOver(Object sender, DragEventArgs e)
		{
			e.AcceptedOperation = DataPackageOperation.Copy;
			e.DragUIOverride.Caption = "Drop keyword spreadsheet here.";
			e.DragUIOverride.IsContentVisible = true;
		}

		void GenerateKeywords(string text, List<List<string>> results)
		{
			var records = text.Split('\n');
			foreach (var record in records)
			{
				var fields = record.Split(',');
				// dragBox.Text += fields.Length.ToString() + '\n';
				var recordItem = new List<string>();
				int last;
				foreach (var field in fields)
				{
					last = recordItem.Count - 1;
					if (recordItem.Count > 1) {
						if (recordItem[last].Contains('"') && field.Contains('"'))
						{
							recordItem[last] = (recordItem[last] + field).Replace('"', ' ').Trim();
						}
						else
						{
							recordItem.Add(field);
						}
					}
					else
					{
						recordItem.Add(field);
					}
				}
				results.Add(recordItem);
			}
			dragBox.Text = "Records: " + results.Count.ToString() + '\n';
		}

		void KeywordAnalysis()
		{
			foreach (var entry in parsedResult)
			{
				var keyword = entry[0];
				var maxSearches = entry[4];
				var competition = entry[6];
				var bidding = entry[8];
				var line = keyword + " | " + maxSearches + " | " + competition + " | " + bidding + '\n';
				/*var line = "";
				for (int i = 0; i < entry.Count; i++)
				{
					line += entry[i] + " | ";
				}
				line += "|\n";*/
				dragBox.Text += line;
			}
		}

		async void Grid_Drop(Object sender, DragEventArgs e)
		{
			if (e.DataView.Contains(StandardDataFormats.StorageItems))
			{
				var items = await e.DataView.GetStorageItemsAsync();
				if (items.Count > 0)
				{
					StorageFile file = items[0] as StorageFile;
					if (file != null)
					{
						if (items[0].Path.Contains(".csv")) {
							var x = await file.OpenSequentialReadAsync();
							var length = (uint)1024 * 64;
							var str = new Windows.Storage.Streams.Buffer(length);
							await x.ReadAsync(str, length, Windows.Storage.Streams.InputStreamOptions.ReadAhead);
							var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(str);
							var output = dataReader.ReadString(str.Length);
							GenerateKeywords(output, parsedResult);
						} else {
							var messageDialog = new Windows.UI.Popups.MessageDialog("Invalid file type. Only drop .csv files.");
							await messageDialog.ShowAsync();
						}
						
					}
				}
			}
		}


		void Grid_DragLeave(Object sender, DragEventArgs e)
		{
			// dragBox.Text = "Awww... don't go.";
		}

		void Add_Click(object sender, RoutedEventArgs e)
		{
			TextBox field = new TextBox();
			field.Width = 200;
			field.PlaceholderText = "paste link...";
			field.HorizontalAlignment = HorizontalAlignment.Left;
			field.Margin = new Thickness(0, 0, 0, 10);
			textFields.Children.Add(field);
		}

		private void Subtract_Click(object sender, RoutedEventArgs e)
		{
			if (textFields.Children.Count > 0) textFields.Children.RemoveAt(textFields.Children.Count - 1);
		}
	}
}
