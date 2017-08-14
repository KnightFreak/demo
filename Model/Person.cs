using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using System.Data.SQLite;

namespace first_mvvm.Model
{
    public class Person : INotifyPropertyChanged  //notify to UI
    {
        private string fname, lname, fullname, email, dob, userid, number, psw1;

        public String Fname
        {
            get
            { return fname;}
            set
            {
                fname = value;
                //OnPropertyChanged(Fname);
            }
        }

        public String Lname
        {
            get
            { return lname;}
            set
            {
                lname = value;
                //OnPropertyChanged(Lname);
            }
        }

        public String FullName
        {
            get
            { return fullname = Fname + " " + Lname; }
            set
            {
                //if (fullname != value)
                    fullname = value;
                //OnPropertyChanged(FullName);
            }
        }

        public String Email_id
        {
            get
            { return email; }
            set
            {
                email = value;
                //OnPropertyChanged(Email_id);
            }
        }

        public String Dob
        {
            get
            { return dob; }
            set
            {
                dob = value;
                //OnPropertyChanged(Dob);
            }
        }

        public String UserID
        {
            get
            { return userid; }
            set
            {
                userid = value;
                //OnPropertyChanged(UserID);
            }
        }

        public String Number
        {
            get
            { return number; }
            set
            {
                number = value;
                //OnPropertyChanged(Number);
            }
        }

        public String Password
        {
            get
            { return psw1; }
            set
            {
                psw1 = value;
                //OnPropertyChanged(Number);
            }
        }
        
        //public void create_table()
        //{
        //    if (!System.IO.File.Exists("Registration.db"))
        //    {
        //        Console.WriteLine("Just entered to create database");
        //        SQLiteConnection.CreateFile("Registration.db");
        //        SQLiteConnection db_connection = new SQLiteConnection("Data Source=Registration.db;Version=3;");
        //        db_connection.Open();
        //        string sql = "create table PatientInfo(FirstName varchar(20), LastName varchar(20), FullName varchar(20), EmailId varchar(30), Dob varchar(20), ContactNumber varchar(10), UserId varchar(20), Password varchar(20))";
        //        SQLiteCommand command = new SQLiteCommand(sql, db_connection);
        //        command.ExecuteNonQuery();
        //        db_connection.Close();
        //    }
        //}       

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string p)
        {
            PropertyChangedEventHandler ph = PropertyChanged;
            if (ph != null)
                ph(this, new PropertyChangedEventArgs(p));
        }
    }
}
