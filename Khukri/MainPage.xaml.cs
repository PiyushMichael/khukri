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
        public MainPage()
        {
            this.InitializeComponent();
        }

		void Button_Click(Object sender, RoutedEventArgs e)
		{
			greetingOutput.Text = "Hello, " + nameInput.Text + " :)";
		}

		async void SecondButton_Click(Object sender, RoutedEventArgs e)
		{
			var picker = new Windows.Storage.Pickers.FileOpenPicker();
			picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
			picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Downloads;
			picker.FileTypeFilter.Add("*");

			Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
			if (file != null)
			{
				var x = await file.OpenSequentialReadAsync();
				var length = (uint)1024*64;
				var str = new Windows.Storage.Streams.Buffer(length);
				await x.ReadAsync(str, length, Windows.Storage.Streams.InputStreamOptions.ReadAhead);
				//dragBox.Text = str.Length.ToString();
				var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(str);
				var output = dataReader.ReadString(str.Length);
				dragBox.Text = "Here is the file you opened :) \n\n" + output;
			}
		}


		void Grid_DragOver(Object sender, DragEventArgs e)
		{
			e.AcceptedOperation = DataPackageOperation.Copy;
			e.DragUIOverride.Caption = "Parse excel sheet.. or something...";
			e.DragUIOverride.IsContentVisible = true;
			dragBox.Text = "Draggin";
		}


		async void Grid_Drop(Object sender, DragEventArgs e)
		{
			if (e.DataView.Contains(StandardDataFormats.StorageItems))
			{
				var items = await e.DataView.GetStorageItemsAsync();
				if (items.Count > 0)
				{
					dragBox.Text = "Dropped";
					//outputBox.Text = items[0].Path;
					var t = Task.Run(() => {
						FileStream SourceStream = File.Open(items[0].Path, FileMode.Open);
						StreamReader streamReader = new StreamReader(SourceStream);
						outputBox.Text = streamReader.ReadToEnd();
					});
				}
			}
		}


		void Grid_DragLeave(Object sender, DragEventArgs e)
		{
			dragBox.Text = "Awww... don't go.";
		}
	}
}
