using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyManager.ClientClass
{
    public class SlideModel: INotifyPropertyChanged
    {
        string _couleur;
        string _contenu;
        public string Couleur { get { return _couleur; } set { _couleur = value; OnPropertyChanged("Couleur"); } }
        public string Contenu { get { return _contenu; } set { _contenu = value; OnPropertyChanged("Contenu"); } }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
    }
}
