using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
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

    public class Department
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
    }

    public class Person
    {
        public int PersonId { get; set; }
        public int DepartmentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Position { get; set; }
    }


    public sealed partial class KeywordReport : Page
    {
        public List<List<int>> searchMatrix;
        public List<Department> Departments { get; set; }
        public List<Person> Persons { get; set; }

        public KeywordReport()
        {
            this.InitializeComponent();

            Departments = new List<Department>
            {
                new Department {DepartmentId = 1, DepartmentName = "R&D"},
                new Department {DepartmentId = 2, DepartmentName = "Finance"},
                new Department {DepartmentId = 3, DepartmentName = "IT"}
            };

            Persons = new List<Person>
            {
                new Person
                {
                    PersonId = 1, DepartmentId = 3, FirstName = "Ronald", LastName = "Rumple",
                    Position = "Network Administrator"
                },
                new Person
                {
                    PersonId = 2, DepartmentId = 1, FirstName = "Brett", LastName = "Banner",
                    Position = "Software Developer"
                },
                new Person
                {
                    PersonId = 3, DepartmentId = 2, FirstName = "Alice", LastName = "Anderson",
                    Position = "Accountant"
                }
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //navResult.Text = e.Parameter.ToString();
            searchMatrix = e.Parameter as List<List<int>>;
            foreach (var search in searchMatrix)
            {
                outputBox.Text += search.Count.ToString() + ", ";
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }
    }
}
