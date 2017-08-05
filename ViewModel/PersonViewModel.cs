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

namespace first_mvvm.ViewModel
{
    class PersonViewModel : INotifyPropertyChanged
    {
        bool add_update_flg = false;

        public PersonViewModel()
       {
           Person = new Person();
           Persons = Load_Info.load_complete_info();
       }
       
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
            var values = (object[])parameter;
            var passwordBox1 = values[0] as PasswordBox;
            var passworda = passwordBox1.Password;
            var passwordBox2 = values[1] as PasswordBox;
            var passwordb = passwordBox2.Password;
            Console.WriteLine(passworda);
            Console.WriteLine(passwordb);
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

            /******/
            //var passwordBox = parameter as PasswordBox;
            //var password = passwordBox.Password;
            //Console.WriteLine(password);
            /******/
             
            /********************************************************************************************************/
            //LOAD DATA FROM XML DOCUMENT TO CHECK WHETHER SAME INFO EXISTS
            for (int i = 0; i < Persons.Count; i++)
            {
                if (Persons[i].Fname == Person.Fname && Persons[i].Lname == Person.Lname
                    && Persons[i].Email_id == Person.Email_id && Persons[i].Dob == Person.Dob && Persons[i].Number == Person.Number)
                {
                    MessageBox.Show(" This entry already exists");
                    return;
                }
            }

            if (!add_update_flg)
            {
                Person.UserID = DateTime.Now.ToString("yyyyMMddHHmmss");
                Persons.Add(Person);
            }

            if(add_update_flg) // perform update function
            {
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
            }

            
            
            /*********************************************************************************************************/
            //SAVE DATA TO XML DOCUMENT
            XmlSerializer xmls = new XmlSerializer(typeof(ObservableCollection<Person>));
            //File.Delete("data1.xml");
            using (Stream s = File.OpenWrite("data1.xml"))
            {
                xmls.Serialize(s, Persons);
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
            for (int i = 0; i < Persons.Count; i++)
            {
                if (Persons[i].UserID == SelectedPerson.UserID)
                {
                    Persons.RemoveAt(i);
                    File.Delete("data1.xml");
                    XmlSerializer xmlser = new XmlSerializer(typeof(ObservableCollection<Person>));
                    using (Stream s = File.OpenWrite("data1.xml"))
                    {
                        xmlser.Serialize(s, Persons);
                    }
                    ButtonText = "Add";
                    Person = new Person();
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
            //var passwordBox = parameter as PasswordBox;
            //var password = passwordBox.Password;

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
