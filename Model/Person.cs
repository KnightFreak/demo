using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace first_mvvm.Model
{
    public class Person : INotifyPropertyChanged  //notify to UI
    {
        private string fname, lname, fullname, email, dob, userid, number;

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

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string p)
        {
            PropertyChangedEventHandler ph = PropertyChanged;
            if (ph != null)
                ph(this, new PropertyChangedEventArgs(p));
        }
    }
}
