﻿using Microsoft.Toolkit.Uwp.UI.Controls;
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
        public List<List<int>> searchMatrix;
        private List<int> max = new List<int>();
        private List<int> min = new List<int>();
        private List<int> avg = new List<int>();
        public List<ExpandoObject> Personses;

        public KeywordReport()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            searchMatrix = e.Parameter as List<List<int>>;
            minMaxAvg();
            DrawTable();
            outputBox.Text += searchMatrix.Last().Count.ToString() + ", " + min.Count.ToString() + ", " + max.Count.ToString() + ", " + avg.Count.ToString();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        void minMaxAvg()
        {
            max.Clear();
            min.Clear();
            avg.Clear();

            foreach (var searchRow in searchMatrix)
            {
                max.Add(searchRow.Max());
                min.Add(searchRow.Min());
                avg.Add(Convert.ToInt32(searchRow.Average()));
            }
        }

        void DrawTable()
        {
            TextBlock text;
            StackPanel stack;

            stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;
            stack.HorizontalAlignment = HorizontalAlignment.Left;

            text = new TextBlock();
            text.Text = "Own Article";
            text.Width = 80;
            stack.Children.Add(text); 
            
            for (int i = 1; i < searchMatrix.Last().Count; i++)
            {
                text = new TextBlock();
                text.Text = "Article " + i;
                text.Width = 80;
                stack.Children.Add(text);
            }

            text = new TextBlock();
            text.Text = "Min";
            text.Width = 80;
            stack.Children.Add(text);

            text = new TextBlock();
            text.Text = "Max";
            text.Width = 80;
            stack.Children.Add(text);

            text = new TextBlock();
            text.Text = "Avg";
            text.Width = 80;
            stack.Children.Add(text);

            tableStack.Children.Add(stack);

            int records = searchMatrix.Count < 20 ? searchMatrix.Count : 20;
            for (int i = 0; i < records; i++)
            {
                stack = new StackPanel();
                stack.Orientation = Orientation.Horizontal;
                stack.HorizontalAlignment = HorizontalAlignment.Left;

                foreach (var entry in searchMatrix[i]) {
                    text = new TextBlock();
                    text.Text = entry.ToString();
                    text.Width = 80;
                    stack.Children.Add(text);
                }

                text = new TextBlock();
                text.Text = min[i].ToString();
                text.Width = 80;
                stack.Children.Add(text);

                text = new TextBlock();
                text.Text = max[i].ToString();
                text.Width = 80;
                stack.Children.Add(text);

                text = new TextBlock();
                text.Text = avg[i].ToString();
                text.Width = 80;
                stack.Children.Add(text);

                tableStack.Children.Add(stack);
            }
        }
    }
}
