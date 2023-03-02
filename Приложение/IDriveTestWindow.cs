using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Приложение
{
    public interface IDriveTestWindow
    {
        public void Command(ref TextBox textBox, ref ListBox listBox, ref ObservableCollection<DQuest> questions);
    }
}
