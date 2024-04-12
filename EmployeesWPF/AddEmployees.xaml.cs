/* Title:           Add Employees
 * Date:            5-1-17
 * Author:          Terry Holmes
 * 
 * Description:     This form is how you add an employee */

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
using DataValidationDLL;
using NewEmployeeDLL;
using NewEventLogDLL;
using OldEmployeesDLL;

namespace EmployeesWPF
{
    /// <summary>
    /// Interaction logic for AddEmployees.xaml
    /// </summary>
    public partial class AddEmployees : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        OldEmployeesClass TheOldEmployeesClass = new OldEmployeesClass();

        //setting up the data
        EmployeesDataSet TheVerifyEmployeeDataSet = new EmployeesDataSet();
        EmployeesDataSet TheEmployeesDataSet;
        EmployeeGroupDataSet TheEmployeeGroupsDataSet = new EmployeeGroupDataSet();
        OldEmployeesDataSet TheOldEmployeesDataSet = new OldEmployeesDataSet();

        public AddEmployees()
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

        private void btnMainMenu_Click(object sender, RoutedEventArgs e)
        {
            MainMenu MainMenu = new MainMenu();
            MainMenu.Show();
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;

            try
            {
                SetControlsReadOnly(true);

                TheEmployeeGroupsDataSet = TheEmployeeClass.GetEmployeeGroupInfo();

                TheEmployeesDataSet = TheEmployeeClass.GetEmployeesInfo();

                intNumberOfRecords = TheEmployeeGroupsDataSet.employeegroup.Rows.Count - 1;

                cboSelectGroup.Items.Add("Select Group");

                for(intCounter =0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectGroup.Items.Add(TheEmployeeGroupsDataSet.employeegroup[intCounter].GroupName);
                }

                cboWarehouse.Items.Add("Select Warehouse");

                intNumberOfRecords = MainWindow.TheWarehousesDataSet.FindWarehouses.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboWarehouse.Items.Add(MainWindow.TheWarehousesDataSet.FindWarehouses[intCounter].FirstName);
                }

                cboEmployeeType.Items.Add("Select Type");
                cboEmployeeType.Items.Add("EMPLOYEE");
                cboEmployeeType.Items.Add("CONTRACTOR");

                cboSelectGroup.SelectedIndex = 0;
                cboWarehouse.SelectedIndex = 0;
                cboEmployeeType.SelectedIndex = 0;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Employees WPF // Add Employees // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void SetControlsReadOnly(bool blnValueBoolean)
        {
            txtFirstName.IsReadOnly = blnValueBoolean;
            txtLastName.IsReadOnly = blnValueBoolean;
            txtPhoneNumber.IsReadOnly = blnValueBoolean;

            if(blnValueBoolean == false)
            {
                txtFirstName.Background = Brushes.White;
                txtLastName.Background = Brushes.White;
                txtPhoneNumber.Background = Brushes.White;
            }
            else
            {
                txtFirstName.Background = Brushes.LightGray;
                txtLastName.Background = Brushes.LightGray;
                txtPhoneNumber.Background = Brushes.LightGray;
            }
        }
        private void ClearControls()
        {
            txtEmployeeID.Text = "";
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtPhoneNumber.Text = "";
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            //this will add an employee
            //setting local variables
            bool blnFatalError = false;
            string strErrorMessage = "";
            string strFirstName;
            string strLastName;
            string strPhoneNumber;
            string strGroup;
            string strHomeOffice;
            string strEmployeeType;
            string strButtonValue;
            int intEmployeeID;
            int intRecordsReturned;
            bool blnThereIsAProblem = false;

            try
            {
                strButtonValue = btnAdd.Content.ToString();

                if(strButtonValue == "Add")
                {
                    //this will load the controls
                    SetControlsReadOnly(false);

                    intEmployeeID = TheEmployeeClass.CreateEmployeeID();

                    txtEmployeeID.Text = Convert.ToString(intEmployeeID);

                    btnAdd.Content = "Save";
                }
                else
                {
                    strEmployeeType = cboEmployeeType.SelectedItem.ToString();
                    if(strEmployeeType == "Select Type")
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Employee Type Was Not Selected\n";
                    }
                    strGroup = cboSelectGroup.SelectedItem.ToString();
                    if(strGroup == "Select Group")
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Group Was Not Selected\n";
                    }
                    strHomeOffice = cboWarehouse.SelectedItem.ToString();
                    if(strHomeOffice == "Select Warehouse")
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Warehouse Was Not Selected\n";
                    }
                    strFirstName = txtFirstName.Text;
                    if(strFirstName == "")
                    {
                        blnFatalError = true;
                        strErrorMessage += "The First Name Was Not Entered\n";
                    }
                    strLastName = txtLastName.Text;
                    if(strLastName == "")
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Last Name Was Not Entered\n";
                    }
                    strPhoneNumber = txtPhoneNumber.Text;
                    blnThereIsAProblem = TheDataValidationClass.VerifyPhoneNumberFormat(strPhoneNumber);
                    if (blnThereIsAProblem == true)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Phone Number Was Not Entered\n";
                    }

                    if(blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage(strErrorMessage);
                        return;
                    }

                    //checking to see if the employee exists
                    MainWindow.TheExistingEmployeeDataSet = TheEmployeeClass.VerifyEmployee(strFirstName, strLastName);

                    intRecordsReturned = MainWindow.TheExistingEmployeeDataSet.VerifyEmployee.Rows.Count;

                    MainWindow.gblnKeepNewEmployee = true;

                    if(intRecordsReturned > 0)
                    {
                        ExistingEmployees ExistingEmployees = new ExistingEmployees();
                        ExistingEmployees.ShowDialog();
                    }

                    if(MainWindow.gblnKeepNewEmployee == false)
                    {
                        return;
                    }

                    EmployeesDataSet.employeesRow NewEmployeeRow = TheEmployeesDataSet.employees.NewemployeesRow();

                    NewEmployeeRow.Active = true;
                    NewEmployeeRow.EmployeeID = Convert.ToInt32(txtEmployeeID.Text);
                    NewEmployeeRow.FirstName = strFirstName;
                    NewEmployeeRow.EmployeeGroup = strGroup;
                    NewEmployeeRow.HomeOffice = strHomeOffice;
                    NewEmployeeRow.LastName = strLastName;
                    NewEmployeeRow.PhoneNumber = strPhoneNumber;
                    NewEmployeeRow.EmployeeType = strEmployeeType;

                    TheEmployeesDataSet.employees.Rows.Add(NewEmployeeRow);
                    TheEmployeeClass.UpdateEmployeesDB(TheEmployeesDataSet);

                    OldEmployeesDataSet.employeesRow OldEmployeeRow = TheOldEmployeesDataSet.employees.NewemployeesRow();

                    OldEmployeeRow.Active = "YES";
                    OldEmployeeRow.EmployeeID = Convert.ToInt32(txtEmployeeID.Text);
                    OldEmployeeRow.FirstName = strFirstName;
                    OldEmployeeRow.Group = strGroup;
                    OldEmployeeRow.HomeOffice = strHomeOffice;
                    OldEmployeeRow.LastName = strLastName;
                    OldEmployeeRow.PhoneNumber = strPhoneNumber;
                    OldEmployeeRow.Type = strEmployeeType;
                    OldEmployeeRow.Department = "NOT PROVIDED";

                    TheOldEmployeesDataSet.employees.Rows.Add(OldEmployeeRow);
                    TheOldEmployeesClass.UpdateOldEmployeesDB(TheOldEmployeesDataSet);

                    ClearControls();
                    SetControlsReadOnly(true);
                    cboEmployeeType.SelectedIndex = 0;
                    cboSelectGroup.SelectedIndex = 0;
                    cboWarehouse.SelectedIndex = 0;
                    btnAdd.Content = "Add";
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Employees WPF // Add Employees // Add Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

        }
    }
}
