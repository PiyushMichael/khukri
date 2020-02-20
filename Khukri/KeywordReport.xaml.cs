using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Khukri
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>  

    public sealed partial class KeywordReport : Page
    {
        public List<KeywordCount> searchMatrix;
        public int recordCount = 20;

        public KeywordReport()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            searchMatrix = e.Parameter as List<KeywordCount>;
            DrawTable();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        void DrawTable()
        {
            TextBlock text;
            StackPanel stack;
            tableStack.Children.Clear();

            stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;
            stack.HorizontalAlignment = HorizontalAlignment.Left;

            text = new TextBlock();
            text.Text = "Keywords";
            text.Width = 160;
            text.FontWeight = FontWeights.SemiBold;
            stack.Children.Add(text);

            text = new TextBlock();
            text.Text = "Density";
            text.Width = 70;
            text.FontWeight = FontWeights.SemiBold;
            stack.Children.Add(text);

            text = new TextBlock();
            text.Text = "Compt.";
            text.Width = 70;
            text.FontWeight = FontWeights.SemiBold;
            stack.Children.Add(text);

            text = new TextBlock();
            text.Text = "Own Article";
            text.Width = 90;
            text.FontWeight = FontWeights.SemiBold;
            stack.Children.Add(text);

            for (int i = 1; i < searchMatrix.Last().counts.Count; i++)
            {
                text = new TextBlock();
                text.Text = "Article " + i;
                text.Width = 80;
                text.FontWeight = FontWeights.SemiBold;
                stack.Children.Add(text);
            }

            text = new TextBlock();
            text.Text = "Min";
            text.Width = 80;
            text.FontWeight = FontWeights.SemiBold;
            stack.Children.Add(text);

            text = new TextBlock();
            text.Text = "Max";
            text.Width = 80;
            text.FontWeight = FontWeights.SemiBold;
            stack.Children.Add(text);

            text = new TextBlock();
            text.Text = "Avg";
            text.Width = 80;
            text.FontWeight = FontWeights.SemiBold;
            stack.Children.Add(text);

            tableStack.Children.Add(stack);

            int records = searchMatrix.Count < recordCount ? searchMatrix.Count : recordCount;
            for (int i = 0; i < records; i++)
            {
                stack = new StackPanel();
                stack.Orientation = Orientation.Horizontal;
                stack.HorizontalAlignment = HorizontalAlignment.Left;

                text = new TextBlock();
                text.Text = searchMatrix[i].keyword;
                text.Width = 160;
                stack.Children.Add(text);

                text = new TextBlock();
                text.Text = searchMatrix[i].maxSearches;
                text.Width = 70;
                stack.Children.Add(text);

                text = new TextBlock();
                text.Text = searchMatrix[i].competition;
                text.Width = 70;
                stack.Children.Add(text);

                int articleIndex = 0;
                foreach (var entry in searchMatrix[i].counts) {
                    text = new TextBlock();
                    text.Text = entry.ToString();
                    if (articleIndex == 0)
                    {
                        text.Width = 90;
                        if (entry <= searchMatrix[i].Min || entry >= searchMatrix[i].Avg)
                        {
                            text.Foreground = new SolidColorBrush(Colors.Red);
                        }
                    } else
                    {
                        text.Width = 80;
                    }
                    stack.Children.Add(text);
                    articleIndex++;
                }

                text = new TextBlock();
                text.Text = searchMatrix[i].Min.ToString();
                text.Width = 80;
                stack.Children.Add(text);

                text = new TextBlock();
                text.Text = searchMatrix[i].Max.ToString();
                text.Width = 80;
                stack.Children.Add(text);

                text = new TextBlock();
                text.Text = searchMatrix[i].Avg.ToString();
                text.Width = 80;
                stack.Children.Add(text);

                tableStack.Children.Add(stack);
            }
        }

        private async void Show_Click(object sender, RoutedEventArgs e)
        {
           try
            {
                recordCount = Convert.ToInt32(recordCountPicker.Text);
                DrawTable();
            }
            catch(Exception)
            {
                var messageDialog = new Windows.UI.Popups.MessageDialog("Record Count must be an integer");
                await messageDialog.ShowAsync();
            }
        }
    }
}
