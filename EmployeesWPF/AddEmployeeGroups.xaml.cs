/* Title:           Add Employee Groups
 * Date:            4-25-17
 * Author:          Terry Holmes
 * 
 * Description:     This form will allow the for the adding of employee groups */

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
    /// Interaction logic for AddEmployeeGroups.xaml
    /// </summary>
    public partial class AddEmployeeGroups : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();

        EmployeeGroupDataSet TheEmployeeGroupDataSet;
        FindEmployeeGroupByGroupNameDataSet TheSearchedEmployeeGroupsDataSet = new FindEmployeeGroupByGroupNameDataSet();

        public AddEmployeeGroups()
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

        private void btnMainMenu_Click(object sender, RoutedEventArgs e)
        {
            MainMenu MainMenu = new MainMenu();
            MainMenu.Show();
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                TheEmployeeGroupDataSet = TheEmployeeClass.GetEmployeeGroupInfo();

                txtGroupName.IsReadOnly = true;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Employees WPF // Add Employee Groups // Window Loaded // " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void ClearTextBoxes()
        {
            txtGroupName.Text = "";
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            string strGroupName;
            string strButtonValue;
            int intRecordsReturned;
            bool blnFatalError;

            //getting the button value
            strButtonValue = btnAdd.Content.ToString();

            if(strButtonValue == "Add")
            {
                btnAdd.Content = "Save";
                txtGroupName.IsReadOnly = false;
            }
            else
            {
                //data validation
                strGroupName = txtGroupName.Text;
                if(strGroupName == "")
                {
                    TheMessagesClass.ErrorMessage("Group Name Was Not Entered");
                    return;
                }

                //checking to see if the group name exists
                TheSearchedEmployeeGroupsDataSet = TheEmployeeClass.FindEmployeeGroupByName(strGroupName);

                intRecordsReturned = TheSearchedEmployeeGroupsDataSet.FindEmployeeGroupByGroupName.Rows.Count;

                if(intRecordsReturned != 0)
                {
                    TheMessagesClass.ErrorMessage("Group Name Already Exists");
                    return;
                }

                blnFatalError = TheEmployeeClass.CreateEmployeeGroup(strGroupName);

                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("There Was A Problem, Contact IT");
                    return;
                }
                else
                {
                    ClearTextBoxes();
                    btnAdd.Content = "Add";
                    TheMessagesClass.InformationMessage("Group Name Was Saved");
                    txtGroupName.IsReadOnly = true;
                }
            }
        }
    }
}
