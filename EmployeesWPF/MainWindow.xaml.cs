/* Title:           Main Window Employees WPF
 * Date:            4-24-17
 * Author:          Terry Holmes
 * 
 * Description:     This is the main window for WPF*/

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
using System.Windows.Navigation;
using System.Windows.Shapes;
using DataValidationDLL;
using NewEmployeeDLL;
using NewEventLogDLL;

namespace EmployeesWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();

        //setting up public data
        public static VerifyLogonDataSet TheEmployeeSignedInDataSet = new VerifyLogonDataSet();
        public static FindWarehousesDataSet TheWarehousesDataSet = new FindWarehousesDataSet();
        public static VerifyEmployeeDataSet TheExistingEmployeeDataSet = new VerifyEmployeeDataSet();

        public static bool gblnKeepNewEmployee;

        //setting up public variables
        int gintNoOfMisses;
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            //this will close the program
            TheMessagesClass.CloseTheProgram();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                gintNoOfMisses = 0;

                TheWarehousesDataSet = TheEmployeeClass.FindWarehouses();

                PleaseWait PleaseWait = new PleaseWait();
                PleaseWait.Show();

                PleaseWait.Close();

                pbxEmployeeID.Focus();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Employees WPF // Main Window // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
            
        }
        private void LogonFailed()
        {
            gintNoOfMisses++;

            if(gintNoOfMisses == 3)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "There Have Been Three Attempts to Sign Into Employees WPF");

                TheMessagesClass.ErrorMessage("There Have Been Three Attempts To Sign In\nThe Program Will Shut Down");

                Application.Current.Shutdown();
            }
            else
            {
                TheMessagesClass.InformationMessage("You Have Failed The Sign In Process");
                return;
            }
        }

        private void btnSignOn_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            string strValueForValidation;
            int intEmployeeID = 0;
            string strLastName;
            string strErrorMessage = "";
            bool blnThereIsAProblem = false;
            bool blnFatalError = false;
            int intRecordsReturned;

            try
            {
                //data validation
                strValueForValidation = pbxEmployeeID.Password;
                blnThereIsAProblem = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Employee ID Is Not An Integer\n";
                }
                else
                {
                    intEmployeeID = Convert.ToInt32(strValueForValidation);
                }
                strLastName = txtLastName.Text;
                if (strLastName == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "The Last Name Was Not Entered";
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                //getting the data
                TheEmployeeSignedInDataSet = TheEmployeeClass.VerifyLogon(intEmployeeID, strLastName);

                intRecordsReturned = TheEmployeeSignedInDataSet.VerifyLogon.Rows.Count;

                if(intRecordsReturned == 0)
                {
                    LogonFailed();
                }
                else
                {
                    if((TheEmployeeSignedInDataSet.VerifyLogon[0].EmployeeGroup != "ADMIN") && (TheEmployeeSignedInDataSet.VerifyLogon[0].EmployeeGroup != "IT"))
                    {
                        LogonFailed();
                    }
                    else
                    {
                        MainMenu MainMenu = new MainMenu();
                        MainMenu.Show();
                        Hide();
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Employee WPF // Main Window // Sign On Button // " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
