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
using System.Text.RegularExpressions;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Khukri
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>

	public class KeywordCount
	{
		public string keyword { get; set; }
		public string maxSearches { get; set; }
		public string competition { get; set; }
		public string maxBidding { get; set; }
		public List<int> counts { get; set; }
		public int Min { get; set; }
		public int Max { get; set; }
		public int Avg { get; set; }

		public void CalculateAggregates()
		{
			List<int> countsExcept1st = (counts.ToArray()).Skip(1).ToList();
			Min = countsExcept1st.Min();
			Max = countsExcept1st.Max();
			Avg = Convert.ToInt32(countsExcept1st.Average());
		}
	}

	public sealed partial class MainPage : Page
    {
		private List<String> urls = new List<String>();
		private List<String> Articles = new List<String>();
		private List<List<string>> parsedResult = new List<List<string>>();
		private List<KeywordCount> searchMatrix = new List<KeywordCount>();
		private Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();


		public MainPage()
        {
            this.InitializeComponent();
			Windows.UI.ViewManagement.ApplicationView.PreferredLaunchViewSize = new Size(710,740);
			Windows.UI.ViewManagement.ApplicationView.PreferredLaunchWindowingMode = Windows.UI.ViewManagement.ApplicationViewWindowingMode.PreferredLaunchViewSize;
			this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
		}

		async void Run_Click(Object sender, RoutedEventArgs e)
		{
			urls.Clear();
			foreach (var item in textFields.Children)
			{
				TextBox x = item as TextBox;
				if (x.Text != "")
				{
					urls.Add(x.Text);
				}
			}

			if (parsedResult.Count != 0 && urls.Count != 0)
			{
				richTextBox.IsEnabled = Run.IsEnabled = plusButton.IsEnabled = minusButton.IsEnabled = false;
				dropText.Text = "Working...";
				Loader1.Visibility = Visibility.Visible;
				Articles.Clear();
				richTextBox.Document.GetText(Windows.UI.Text.TextGetOptions.None, out string rtb);
				Articles.Add(rtb.ToLower());
				int i = 1;

				foreach (var url in urls)
				{
					try
					{
						Uri requestUri = new Uri(url);
						var response = await httpClient.GetAsync(requestUri);
						var text = await response.Content.ReadAsStringAsync();
						Articles.Add(Regex.Replace(text.ToLower(), "<.*?>", ""));
						i++;
					}
					catch (Exception) { }
				}

				richTextBox.IsEnabled = Run.IsEnabled = plusButton.IsEnabled = minusButton.IsEnabled = true;
				dropText.Text = "Drop files here...";
				Loader1.Visibility = Visibility.Collapsed;
				KeywordAnalysis();
				this.Frame.Navigate(typeof(KeywordReport), searchMatrix);
			} else
			{
				var message = "Add csv file of keywords first";
				if (urls.Count == 0) message = "Add competitior URLs first";
				var messageDialog = new Windows.UI.Popups.MessageDialog(message);
				await messageDialog.ShowAsync();
			}
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
			results.Clear();
			var records = text.Split('\n');
			var keywordMarker = false;

			foreach (var record in records)
			{
				var fields = record.Split(',');
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
				if (keywordMarker) results.Add(recordItem);
				if (recordItem.First() == "Keyword") keywordMarker = true;
			}
			dragBox.Text = results.Count.ToString() + " keywords added";
		}

		void KeywordAnalysis()
		{
			searchMatrix.Clear();

			foreach (var entry in parsedResult)
			{
				var keyword = entry[0];
				var maxSearches = entry[4];
				var competition = entry[6];
				var bidding = entry[8];
				List<int> counts = new List<int>();
				foreach (var article in Articles)
				{
					counts.Add(Regex.Matches(article, keyword).Count);
				}

				var search = new KeywordCount();
				search.keyword = keyword;
				search.maxSearches = maxSearches;
				search.maxBidding = bidding;
				search.competition = competition;
				search.counts = counts;
				search.CalculateAggregates();
				searchMatrix.Add(search);
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
							try {
								var output = dataReader.ReadString(str.Length);
								GenerateKeywords(output, parsedResult);
							} catch(Exception) {
								var messageDialog = new Windows.UI.Popups.MessageDialog("Invalid csv file. Save file as CSV (MS-DOS) (*.csv) format.");
								await messageDialog.ShowAsync();
							}
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
			field.Width = 400;
			field.PlaceholderText = "paste link...";
			field.HorizontalAlignment = HorizontalAlignment.Left;
			field.Margin = new Thickness(20, 0, 0, 10);
			textFields.Children.Add(field);
		}

		private void Subtract_Click(object sender, RoutedEventArgs e)
		{
			if (textFields.Children.Count > 0) textFields.Children.RemoveAt(textFields.Children.Count - 1);
		}

		private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			richTextBox.Width = e.NewSize.Width - 60;
		}
	}
}
