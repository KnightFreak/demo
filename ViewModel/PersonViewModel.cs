using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Text.RegularExpressions;
using first_mvvm.Model;
using first_mvvm.Command;
using System.Windows;
using System.Xml.Serialization;
using System.IO;
using System.Data.SQLite;
using System.Windows.Controls;
using System.Windows.Data;
using System.Security.Cryptography;

namespace first_mvvm.ViewModel
{
    class PersonViewModel : INotifyPropertyChanged
    {
        private string _LoginEmail;
        public String LoginEmail
        {
            get
            { return _LoginEmail; }
            set
            {
                _LoginEmail = value;
                OnPropertyChanged("LoginEmail");
            }
        }

        /*************************************************************************************************/   
        bool add_update_flg = false;
        /*************************************************************************************************/
        public PersonViewModel()
        {
           Person = new Person();
           if (!System.IO.File.Exists("Registration.db"))
           {
               Console.WriteLine("Just entered to create database");
               SQLiteConnection.CreateFile("Registration.db");
               SQLiteConnection db_connection = new SQLiteConnection("Data Source=Registration.db;Version=3;");
               db_connection.Open();
               string sql = "create table PatientInfo(FirstName varchar(20), LastName varchar(20), FullName varchar(20), EmailId varchar(30), Dob varchar(20), ContactNumber varchar(10), UserId varchar(20), Password varchar(20))";
               SQLiteCommand command = new SQLiteCommand(sql, db_connection);
               command.ExecuteNonQuery();
               db_connection.Close();
           }
           SQLiteConnection connection = new SQLiteConnection("Data Source=Registration.db;Version=3;");
           string query = "select * from PatientInfo";
           connection.Open();
           SQLiteCommand commandtxt = new SQLiteCommand(query, connection);
           SQLiteDataReader reader = commandtxt.ExecuteReader();

           Persons = new ObservableCollection<Person>();
           while(reader.Read())
           {
               Person p = new Person { Fname = (string)reader["FirstName"], Lname = (string)reader["LastName"], Email_id = (string)reader["EmailId"], Dob = (string)reader["Dob"], UserID = (string)reader["UserId"], Number = (string)reader["ContactNumber"], Password = (string)reader["Password"]};// 
               Persons.Add(p);
           }
           connection.Close();
       }
         
        /************************************************************************************************/       
        private Person _person;
        public Person Person
       {
           get { return _person; }
           set
           {
               _person = value;
               OnPropertyChanged("Person");
           }
       }

        /************************************************************************************************/
        private ObservableCollection<Person> _persons;
        public ObservableCollection<Person> Persons
       {
           get { return _persons; }
           set
           {
               _persons = value;
               OnPropertyChanged("Persons");
           }
       }

        /*************************************************************************************************/
        public Person _Selectedperson;
        public Person SelectedPerson
        {
            get { return _Selectedperson; }
            set 
            {
                
                _Selectedperson = value;
                if (SelectedPerson != null)
                {
                    ButtonText = "Update";

                    for (int i = 0; i < Persons.Count; i++)
                    {
                        if (Persons[i].UserID == SelectedPerson.UserID)
                        {
                            Person = new Person{ Fname = Persons[i].Fname, Lname = Persons[i].Lname, Email_id = Persons[i].Email_id, Dob = Persons[i].Dob, UserID = Persons[i].UserID, Number = Persons[i].Number};
                            break;
                        }

                    }
                    //Change the flag value for ADD_UPDATE function. Update => true. Add => false.
                    add_update_flg = true;
                    
                }
            }
        }

        /*************************************************************************************************/
        private ICommand _AddCommand;
        private string _ButtonText = "Add";
        public String ButtonText
        {
            get { return _ButtonText ?? (_ButtonText = "Add"); }
            set
            {
                _ButtonText = value;
                OnPropertyChanged("ButtonText");
            }
        }

        /**************************************************************************************************/
        public ICommand AddCommand
        {
            get
            {
                if(_AddCommand == null)
                {
                    _AddCommand = new RelayCommand(AddExecute, CanAddExecute /*, false*/);
                }
                return _AddCommand;
            }
        }

        private void AddExecute(object parameter)
        {
            
            //START VALIDATION
            if (String.IsNullOrEmpty(Person.Fname) || !Regex.IsMatch(Person.Fname, @"^[a-zA-Z]+$"))
            {
                MessageBox.Show("Enter valid First Name");
                return;
            }
            if (String.IsNullOrEmpty(Person.Lname) || !Regex.IsMatch(Person.Lname, @"^[a-zA-Z]+$"))
            {
                MessageBox.Show("Enter valid Last Name");
                return;
            }
            if (String.IsNullOrEmpty(Person.Dob))
            {
                MessageBox.Show("Enter valid Date of birth");
                return;
            }
            if (!validate_email.IsValidEmail(Person.Email_id))
            {
                MessageBox.Show("Enter valid Email ID");
                return;
            }
            if (String.IsNullOrEmpty(Person.Number) || !Regex.IsMatch(Person.Number, @"^[0-9]{10}$"))
            {
                MessageBox.Show("Enter valid Contact Number");
                return;
            }

            var pswBoxes = parameter as List<object>;
            PasswordBox pb1, pb2;
            pb1 = pswBoxes[0] as PasswordBox;
            string psw0 = pb1.Password;
            pb2 = pswBoxes[1] as PasswordBox;
            string psw1 = pb2.Password;

            if (psw0 != psw1)
            {
                MessageBox.Show("Passwords didn't match. Enter again!");
                return;
            }
            else
            {
                Person.Password = SHA.GenerateSHA256String(psw0);
            }     

            /********************************************************************************************************/
            //CHECK WHETHER SAME INFO EXISTS
            for (int i = 0; i < Persons.Count; i++)
            {
                if (Persons[i].Fname == Person.Fname && Persons[i].Lname == Person.Lname
                    && Persons[i].Email_id == Person.Email_id && Persons[i].Dob == Person.Dob && Persons[i].Number == Person.Number)
                {
                    MessageBox.Show(" This entry already exists");
                    return;
                }
            }

            if (!add_update_flg)//perform add function
            {
                Person.UserID = DateTime.Now.ToString("yyyyMMddHHmmss");
                Persons.Add(Person);
                SQLiteConnection db_connection = new SQLiteConnection("Data Source=Registration.db;Version=3;");
                db_connection.Open();
                string Query = "insert into PatientInfo(FirstName, LastName, FullName, EmailId, Dob, ContactNumber, UserId, Password) values('" + Person.Fname + "','" + Person.Lname + "','" + Person.FullName + "','" + Person.Email_id + "', '" + Person.Dob + "', '" + Person.Number + "','" + Person.UserID + "', '" + Person.Password + "')";
                SQLiteCommand command = new SQLiteCommand(Query, db_connection);
                command.ExecuteNonQuery();
                db_connection.Close();
                pb1.Password = "";
                pb2.Password = "";
            }

            if(add_update_flg) // perform update function
            {
                var temp = Person.UserID;

                //Update Collection
                for (int i = 0; i < Persons.Count; i++)
                {
                    if (Persons[i].UserID == Person.UserID)
                    {
                        Person.UserID = DateTime.Now.ToString("yyyyMMddHHmmss");
                        Persons.RemoveAt(i);
                        Persons.Add(Person);
                        break;
                    }
                }
                SQLiteConnection db_connection = new SQLiteConnection("Data Source=Registration.db;Version=3;");
                db_connection.Open();
                string Query = "update PatientInfo set FirstName = '" + Person.Fname + "',LastName = '" + Person.Lname + "',FullName = '" + Person.FullName + "',EmailId = '" + Person.Email_id + "', Dob = '" + Person.Dob + "', ContactNumber = '" + Person.Number + "',UserId = '" + Person.UserID + "' where UserId = '" + temp + "' ";
                SQLiteCommand command = new SQLiteCommand(Query, db_connection);
                command.ExecuteNonQuery();
                db_connection.Close();
            }

            /**********************************************************************************************************/

            if(add_update_flg)
            {
                MessageBox.Show("Data Updated Successfully");
                ButtonText = "Add";
                add_update_flg = false;
            }

            Person = new Person();
        }

        private bool CanAddExecute(object parameter)
        {
            return true;     
        }

        /***************************************************************************************************/
        private ICommand _DelCommand;
        public ICommand DelCommand
        {
            get
            {
                if (_DelCommand == null)
                {
                    _DelCommand = new RelayCommand(DelExecute, CanDelExecute /*, false*/);
                }
                return _DelCommand;
            }
        }

        private bool CanDelExecute(object parameter)
        {
            if (SelectedPerson != null)
                return true;
            else
                return false;
        }

        private void DelExecute(object parameter)
        {          
            SQLiteConnection db_connection = new SQLiteConnection("Data Source=Registration.db;Version=3;");
            db_connection.Open();
            string Query = "delete from PatientInfo where UserId = '" + SelectedPerson.UserID + "'";
            SQLiteCommand command = new SQLiteCommand(Query, db_connection);
            command.ExecuteNonQuery();
            MessageBox.Show("Deleted Successfully");
            db_connection.Close();

            for (int i = 0; i < Persons.Count; i++)
            {
                if (Persons[i].UserID == SelectedPerson.UserID)
                {
                    Persons.RemoveAt(i);
                    ButtonText = "Add";
                    Person = new Person();
                    add_update_flg = false;
                    break;
                }
            }
        }

        /******************************************************************************************************/
        private ICommand _RegNewCommand;
        public ICommand RegNewCommand
        {
            get
            {
                if (_RegNewCommand == null)
                {
                    _RegNewCommand = new RelayCommand(RegNewExecute, CanRegNewExecute /*, false*/);
                }
                return _RegNewCommand;
            }
        }

        private void RegNewExecute(object parameter)
        {
            Person = new Person();
            SelectedPerson = null;
            //update the flg for Add function
            ButtonText = "Add";
            add_update_flg = false;
            //Reset Password boxes
            var pswBoxes = parameter as List<object>;
            PasswordBox pb1, pb2;
            pb1 = pswBoxes[0] as PasswordBox;
            pb2 = pswBoxes[1] as PasswordBox;
            pb1.Password = "";
            pb2.Password = "";
        }

        private bool CanRegNewExecute(object parameter)
        {
            return true;
        }

        /******************************************************************************************************/
        private ICommand _LoginCommand;
        public ICommand LoginCommand
        {
            get
            {
                if (_LoginCommand == null)
                {
                    _LoginCommand = new RelayCommand(LoginExecute, CanLoginExecute /*, false*/);
                }
                return _LoginCommand;
            }
        }

        private void LoginExecute(object parameter)
        {

            var passwordBox = parameter as PasswordBox;
            var password = passwordBox.Password;
            var hashed_password = SHA.GenerateSHA256String(password);


            for (int i = 0; i < Persons.Count; i++)
            {
                if (Persons[i].Email_id == LoginEmail && Persons[i].Password == hashed_password)
                {
                    MessageBox.Show("Successfully logged in!");
                    LoginEmail = "";
                    passwordBox.Password = "";
                    return;
                }  
            }
            MessageBox.Show("Incorrect Username or Password");
            passwordBox.Password = "";
            return;   
        }

        private bool CanLoginExecute(object parameter)
        {
            return true;
        }

        /******************************************************************************************************/
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                 PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        
    }

    public class MyConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return values;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
