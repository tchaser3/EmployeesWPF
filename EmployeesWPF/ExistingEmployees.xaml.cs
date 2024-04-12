/* Title:           Existing Employees
 * Date:            5-24-17
 * Author:          Terry Holmes
 * 
 * Description:     This form will display an existing employee(s) that match the same name */

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
using NewEventLogDLL;

namespace EmployeesWPF
{
    /// <summary>
    /// Interaction logic for ExistingEmployees.xaml
    /// </summary>
    public partial class ExistingEmployees : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        public ExistingEmployees()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dgrResults.ItemsSource = MainWindow.TheExistingEmployeeDataSet.VerifyEmployee;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void rdoYes_Checked(object sender, RoutedEventArgs e)
        {
            MainWindow.gblnKeepNewEmployee = true;
        }

        private void rdoNo_Checked(object sender, RoutedEventArgs e)
        {
            MainWindow.gblnKeepNewEmployee = false;
        }
    }
}
