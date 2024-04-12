/* Title:           Main Menu
 * Date:            4-24-17
 * Author:          Terry Holmes
 * 
 * Description:     This form is the main menu */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EmployeesWPF
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();

        public MainMenu()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void btnCreateEmployeeGroup_Click(object sender, RoutedEventArgs e)
        {
            AddEmployeeGroups AddEmployeeGroups = new AddEmployeeGroups();
            AddEmployeeGroups.Show();
            Close();
        }

        private void btnTerminateEmployee_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.UnderDevelopment();
        }

        private void btnReports_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.UnderDevelopment();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            AddEmployees AddEmployees = new AddEmployees();
            AddEmployees.Show();
            Close();
        }

        private void btnEditEmployee_Click(object sender, RoutedEventArgs e)
        {
            EditEmployee EditEmployee = new EditEmployee();
            EditEmployee.Show();
            Close();
        }
    }
}
