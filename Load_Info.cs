using first_mvvm.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace first_mvvm
{
    class Load_Info
    {
        public static ObservableCollection<Person> load_complete_info()
        {
            XmlSerializer xs2 = new XmlSerializer(typeof(ObservableCollection<Person>));
            ObservableCollection<Person> complete_info_list = null;
            try
            {
                using (Stream s = File.OpenRead("data1.xml"))
                {
                    complete_info_list = xs2.Deserialize(s) as ObservableCollection<Person>;
                }
            }
            catch
            {
                complete_info_list = new ObservableCollection<Person>();
            }
            return complete_info_list;
        }
    }
}
