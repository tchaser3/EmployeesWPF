/* Title:           Edit Employee
 * Date:            5-25-17
 * Author:          Terry Holmes
 * 
 * Description:     This is the form for editing an employee */

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
using NewEmployeeDLL;
using NewEventLogDLL;
using DataValidationDLL;

namespace EmployeesWPF
{
    /// <summary>
    /// Interaction logic for EditEmployee.xaml
    /// </summary>
    public partial class EditEmployee : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();

        //setting up the data
        EmployeesDataSet TheEmployeesDataSet = new EmployeesDataSet();
        FindAllEmployeesByLastNameDataSet TheFindAllEmployeesByLastNameDataSet = new FindAllEmployeesByLastNameDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeID = new FindEmployeeByEmployeeIDDataSet();
        EmployeeComboBoxDataSet TheEmployeeComboBoxDataSet = new EmployeeComboBoxDataSet();
        EmployeeGroupDataSet TheEmployeeGroupDataSet;

        //setting global variables
        bool gblnActive;
        string gstrHomeOffice;
        string gstrEmployeeType;
        string gstrGroup;

        public EditEmployee()
        {
            InitializeComponent();
        }

        private void btnMainMenu_Click(object sender, RoutedEventArgs e)
        {
            MainMenu MainMenu = new MainMenu();
            MainMenu.Show();
            Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            //this will close the program
            TheMessagesClass.CloseTheProgram();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //local variables
            int intCounter;
            int intNumberOfRecords;

            try
            {
                cboSelectActive.Items.Add("Select Active");
                cboSelectActive.Items.Add("YES");
                cboSelectActive.Items.Add("NO");
                cboSelectEmployeetype.Items.Add("Select Type");
                cboSelectEmployeetype.Items.Add("EMPLOYEE");
                cboSelectEmployeetype.Items.Add("CONTRACTOR");
                cboSelectHomeOffice.Items.Add("Select Warehouse");
                cboSelectGroup.Items.Add("Select Group");

                intNumberOfRecords = MainWindow.TheWarehousesDataSet.FindWarehouses.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectHomeOffice.Items.Add(MainWindow.TheWarehousesDataSet.FindWarehouses[intCounter].FirstName);
                }

                cboSelectHomeOffice.SelectedIndex = 0;

                TheEmployeeGroupDataSet = TheEmployeeClass.GetEmployeeGroupInfo();

                intNumberOfRecords = TheEmployeeGroupDataSet.employeegroup.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectGroup.Items.Add(TheEmployeeGroupDataSet.employeegroup[intCounter].GroupName);
                }

                cboSelectGroup.SelectedIndex = 0;
                cboSelectActive.SelectedIndex = 0;
                cboSelectEmployeetype.SelectedIndex = 0;
                SetControlsReadOnly(true);
                btnEdit.IsEnabled = false;
                SetComboBoxCondition(false);
                
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Employee WPF // Edit Employee // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

        }
        private void SetComboBoxCondition(bool blnValueBoolean)
        {
            cboSelectActive.IsEnabled = blnValueBoolean;
            cboSelectEmployeetype.IsEnabled = blnValueBoolean;
            cboSelectGroup.IsEnabled = blnValueBoolean;
            cboSelectHomeOffice.IsEnabled = blnValueBoolean;
        }
        private void txtEnterLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            //this will call the like search
            string strLastName;
            int intRecordsReturn;
            int intCounter;
            int intEmployeeID;
            string strFirstName;
            string strFullName;
            

            try
            {
                cboSelectEmployee.Items.Clear();
                TheEmployeeComboBoxDataSet.employees.Rows.Clear();

                strLastName = txtEnterLastName.Text;

                TheFindAllEmployeesByLastNameDataSet = TheEmployeeClass.FindAllEmployeesByLastName(strLastName);

                intRecordsReturn = TheFindAllEmployeesByLastNameDataSet.FindAllEmployeeByLastName.Rows.Count - 1;

                if(intRecordsReturn == -1)
                {
                    TheMessagesClass.ErrorMessage("No Employees Found");
                    ResetControls();
                }
                else
                {
                    cboSelectEmployee.Items.Add("Select Employees");

                    for (intCounter = 0; intCounter <= intRecordsReturn; intCounter++)
                    {
                        strFirstName = TheFindAllEmployeesByLastNameDataSet.FindAllEmployeeByLastName[intCounter].FirstName;
                        strLastName = TheFindAllEmployeesByLastNameDataSet.FindAllEmployeeByLastName[intCounter].LastName;
                        intEmployeeID = TheFindAllEmployeesByLastNameDataSet.FindAllEmployeeByLastName[intCounter].EmployeeID;

                        strFullName = strFirstName + " " + strLastName;

                        EmployeeComboBoxDataSet.employeesRow NewEmployeeEntry = TheEmployeeComboBoxDataSet.employees.NewemployeesRow();

                        NewEmployeeEntry.EmployeeID = intEmployeeID;
                        NewEmployeeEntry.FullName = strFullName;

                        TheEmployeeComboBoxDataSet.employees.Rows.Add(NewEmployeeEntry);

                        cboSelectEmployee.Items.Add(strFullName);
                    }

                    cboSelectEmployee.SelectedIndex = 0;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Employees WPF // Edit Employee // Enter Last Name Event " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //setting local variables
            int intSelectedIndex;
            string strFullName;
            int intCounter;
            int intNumberOfRecords;
            int intEmployeeID = 0;

            try
            {
                intSelectedIndex = cboSelectEmployee.SelectedIndex;

                if(intSelectedIndex > 0)
                {
                    strFullName = cboSelectEmployee.SelectedItem.ToString();

                    intNumberOfRecords = TheEmployeeComboBoxDataSet.employees.Rows.Count - 1;

                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        if(strFullName == TheEmployeeComboBoxDataSet.employees[intCounter].FullName)
                        {
                            intEmployeeID = TheEmployeeComboBoxDataSet.employees[intCounter].EmployeeID;
                            break;
                        }
                    }

                    TheFindEmployeeByEmployeeID = TheEmployeeClass.FindEmployeeByEmployeeID(intEmployeeID);

                    //loading the controls
                    txtEmployeeID.Text = Convert.ToString(TheFindEmployeeByEmployeeID.FindEmployeeByEmployeeID[0].EmployeeID);
                    txtFirstName.Text = TheFindEmployeeByEmployeeID.FindEmployeeByEmployeeID[0].FirstName;
                    txtLastName.Text = TheFindEmployeeByEmployeeID.FindEmployeeByEmployeeID[0].LastName;
                    txtPhoneNumber.Text = TheFindEmployeeByEmployeeID.FindEmployeeByEmployeeID[0].PhoneNumber;
                    if (TheFindEmployeeByEmployeeID.FindEmployeeByEmployeeID[0].Active == true)
                        cboSelectActive.SelectedIndex = 1;
                    else if (TheFindEmployeeByEmployeeID.FindEmployeeByEmployeeID[0].Active == false)
                        cboSelectActive.SelectedIndex = 2;

                    intNumberOfRecords = cboSelectEmployeetype.Items.Count - 1;

                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        cboSelectEmployeetype.SelectedIndex = intCounter;

                        if (cboSelectEmployeetype.SelectedItem.ToString() == TheFindEmployeeByEmployeeID.FindEmployeeByEmployeeID[0].EmployeeType)
                        { 
                            intSelectedIndex = intCounter;
                        }
                    }

                    cboSelectEmployeetype.SelectedIndex = intSelectedIndex;

                    intNumberOfRecords = cboSelectGroup.Items.Count - 1;

                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        cboSelectGroup.SelectedIndex = intCounter;

                        if(cboSelectGroup.SelectedItem.ToString() == TheFindEmployeeByEmployeeID.FindEmployeeByEmployeeID[0].EmployeeGroup)
                        {
                            intSelectedIndex = intCounter;
                        }
                    }

                    cboSelectGroup.SelectedIndex = intSelectedIndex;

                    intNumberOfRecords = cboSelectHomeOffice.Items.Count;

                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        cboSelectHomeOffice.SelectedIndex = intCounter;

                        if(cboSelectHomeOffice.SelectedItem.ToString() == TheFindEmployeeByEmployeeID.FindEmployeeByEmployeeID[0].HomeOffice)
                        {
                            intSelectedIndex = intCounter;
                        }
                    }

                    cboSelectHomeOffice.SelectedIndex = intSelectedIndex;
                    btnEdit.IsEnabled = true;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Employee WPF // Edit Employee // cboSelectEmployee Selection Changed " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void SetControlsReadOnly(bool blnValueBoolean)
        {
            txtFirstName.IsReadOnly = blnValueBoolean;
            txtLastName.IsReadOnly = blnValueBoolean;
            txtPhoneNumber.IsReadOnly = blnValueBoolean;
        }
        private void ResetControls()
        {
            txtEmployeeID.Text = "";
            txtEnterLastName.Text = "";
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtPhoneNumber.Text = "";
            cboSelectActive.SelectedIndex = 0;
            cboSelectEmployee.Items.Clear();
            cboSelectEmployeetype.SelectedIndex = 0;
            cboSelectGroup.SelectedIndex = 0;
            cboSelectHomeOffice.SelectedIndex = 0;
            btnEdit.IsEnabled = false;
            SetComboBoxCondition(false);
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            //this will allow the user to edit an employee
            bool blnFatalError = false;
            string strErrorMesage = "";
            string strButtonValue;
            int intCounter;
            int intNumberOfRecords;
            int intSelectedIndex;
            string strFirstName;
            string strLastname;
            string strPhoneNumber;
            int intEmployeeID;

            try
            {
                strButtonValue = btnEdit.Content.ToString();

                if(strButtonValue == "Edit")
                {
                    SetControlsReadOnly(false);
                    SetComboBoxCondition(true);

                    btnEdit.Content = "Save";
                }
                else
                {
                    //beginning data validation
                    intSelectedIndex = cboSelectActive.SelectedIndex;
                    if(intSelectedIndex < 1)
                    {
                        blnFatalError = true;
                        strErrorMesage += "Active Was Not Selected\n";
                    }
                    intSelectedIndex = cboSelectEmployeetype.SelectedIndex;
                    if (intSelectedIndex < 1)
                    {
                        blnFatalError = true;
                        strErrorMesage += "Employee Type Was Not Selected\n";
                    }
                    intSelectedIndex = cboSelectGroup.SelectedIndex;
                    if (intSelectedIndex < 1)
                    {
                        blnFatalError = true;
                        strErrorMesage += "Group Was Not Selected\n";
                    }
                    intSelectedIndex = cboSelectHomeOffice.SelectedIndex;
                    if (intSelectedIndex < 1)
                    {
                        blnFatalError = true;
                        strErrorMesage += "Home Office Was Not Selected\n";
                    }
                    intEmployeeID = Convert.ToInt32(txtEmployeeID.Text);
                    strFirstName = txtFirstName.Text;
                    if(strFirstName == "")
                    {
                        blnFatalError = true;
                        strErrorMesage += "First Name Was Not Entered\n";
                    }
                    strLastname = txtLastName.Text;
                    if(strLastname == "")
                    {
                        blnFatalError = true;
                        strErrorMesage += "Last Name Was Not Entered\n";
                    }
                    strPhoneNumber = txtPhoneNumber.Text;
                    if(strPhoneNumber == "")
                    {
                        blnFatalError = true;
                        strErrorMesage += "The Phone Number Was Not Entered\n";
                    }

                    if(blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage(strErrorMesage);
                        return;
                    }

                    //updating the data
                    TheEmployeesDataSet = TheEmployeeClass.GetEmployeesInfo();

                    intNumberOfRecords = TheEmployeesDataSet.employees.Rows.Count - 1;

                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        if(intEmployeeID == TheEmployeesDataSet.employees[intCounter].EmployeeID)
                        {
                            TheEmployeesDataSet.employees[intCounter].Active = gblnActive;
                            TheEmployeesDataSet.employees[intCounter].FirstName = strFirstName;
                            TheEmployeesDataSet.employees[intCounter].EmployeeGroup = gstrGroup;
                            TheEmployeesDataSet.employees[intCounter].HomeOffice = gstrHomeOffice;
                            TheEmployeesDataSet.employees[intCounter].LastName = strLastname;
                            TheEmployeesDataSet.employees[intCounter].PhoneNumber = strPhoneNumber;
                            TheEmployeesDataSet.employees[intCounter].EmployeeType = gstrEmployeeType;

                            TheEmployeeClass.UpdateEmployeesDB(TheEmployeesDataSet);
                            break;
                        }
                    }

                    btnEdit.Content = "Edit";
                    SetComboBoxCondition(false);
                    SetControlsReadOnly(true);
                    ResetControls();
                    TheMessagesClass.InformationMessage("Employee Updated");
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Employees WPF // Edit Employees // Edit Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //setting variables
            int intSelectedIndex;

            intSelectedIndex = cboSelectGroup.SelectedIndex;

            if(intSelectedIndex > 0)
            {
                gstrGroup = cboSelectGroup.SelectedItem.ToString();
            }
        }

        private void cboSelectHomeOffice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectHomeOffice.SelectedIndex;

            if(intSelectedIndex > 0)
            {
                gstrHomeOffice = cboSelectHomeOffice.SelectedItem.ToString();
            }
                    
        }

        private void cboSelectEmployeetype_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectEmployeetype.SelectedIndex;

            if(intSelectedIndex > 0)
            {
                gstrEmployeeType = cboSelectEmployeetype.SelectedItem.ToString();
            }
        }

        private void cboSelectActive_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectActive.SelectedIndex;

            if(intSelectedIndex > 0)
            {
                if (intSelectedIndex == 1)
                    gblnActive = true;
                else if (intSelectedIndex == 2)
                    gblnActive = false;
            }
        }
    }
}
