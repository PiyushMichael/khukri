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
			greetingOutput.Text = "Hello, " + nameInput.Text + "!";
		}

		void SecondButton_Click(Object sender, RoutedEventArgs e)
		{
			greetingOutput.Text = "Hello, " + nameInput.Text + " from second.";
		}


		void Grid_DragOver(Object sender, DragEventArgs e)
		{
			/*e.AcceptedOperation = DataPackageOperation.Copy;
			if (e.DragUIOverride)
			{
				e.DragUIOverride.Caption = "Parse excel sheet.. or something...";
				e.DragUIOverride.IsContentVisible = true;
			}
			dragBox->Text = "Draggin";*/
		}


		void Grid_Drop(Object sender, DragEventArgs e)
		{
			/*if (e.DataView.Contains(StandardDataFormats.StorageItems))
			{
				// String^ items = e->DataView->GetTextAsync()->GetResults();
				// dragBox->Text = items;
				auto def = e->GetDeferral();
				auto _this = this;
				create_task(e->DataView->GetTextAsync()).then([def, _this, e])(String ^ s)
				{
					auto wsText = s->Data();
					e->AcceptedOperation = DataPackageOperation::Copy;
					def->Complete();
				}
			}*/
		}


		void Grid_DragLeave(Object sender, DragEventArgs e)
		{
			dragBox.Text = "Awww... don't go.";
		}
	}
}
