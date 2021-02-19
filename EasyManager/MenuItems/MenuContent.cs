using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace EasyManager.MenuItems
{
    public class MenuContent:INotifyPropertyChanged
    {
        private object _name;
        private object _content;
        private bool _visible;
        private string _icon="Sms";
        private string _tooltip="";
        private ScrollBarVisibility _horizontalScrollBarVisibilityRequirement;
        private ScrollBarVisibility _verticalScrollBarVisibilityRequirement;
        private Thickness _marginRequirement = new Thickness(16);
        private Visibility _showtext = Visibility.Collapsed;
        public MenuContent() { }

        public bool Visible
        {
            get => _visible;
            set => this.MutateVerbose(ref _visible, value, RaisePropertyChanged());
        }

        public MenuContent(string name,object content,bool visible=true)
        {
            _visible = visible;
            _name = name;
            _content = content;
        }
        
        public MenuContent(string icon,string tooltip="", string name="", object content = null, Visibility visibility=Visibility.Collapsed, bool visible = true)
        {
            _visible = visible;
            _name = name;
            _content = content;
            _icon = icon;
            _showtext = visibility;
            _tooltip = tooltip;
        }

        public MenuContent(string icon,string name = "", object content = null, Visibility visibility = Visibility.Visible, bool visible = true)
        {
            _visible = visible;
            _name = name;
            _content = content;
            _icon = icon;
            _showtext = visibility;
            _tooltip = "";
        }

        public object Name
        {
            get { return _name; }
            set { this.MutateVerbose(ref _name, value, RaisePropertyChanged()); }
        }

        public object Content
        {
            get { return _content; }
            set { this.MutateVerbose(ref _content, value, RaisePropertyChanged()); }
        }

        public string Icon
        {
            get { return _icon; }
            set { _icon = value; Console.WriteLine(value); OnPropertyChanged("Icon"); }
        }

        public string ToolTip
        {
            get { return _tooltip; }
            set { _tooltip = value; Console.WriteLine(value); OnPropertyChanged("ToolTip"); }
        }

        public System.Windows.Visibility ShowText
        {
            get => _showtext;
            set { _showtext = value; OnPropertyChanged("ShowText"); }
        }

        public ScrollBarVisibility HorizontalScrollBarVisibilityRequirement
        {
            get { return _horizontalScrollBarVisibilityRequirement; }
            set { this.MutateVerbose(ref _horizontalScrollBarVisibilityRequirement, value, RaisePropertyChanged()); }
        }

        public ScrollBarVisibility VerticalScrollBarVisibilityRequirement
        {
            
            get { return _verticalScrollBarVisibilityRequirement; }
            set { this.MutateVerbose(ref _verticalScrollBarVisibilityRequirement, value, RaisePropertyChanged()); }
        }

        public Thickness MarginRequirement
        {
            get { return _marginRequirement; }
            set { this.MutateVerbose(ref _marginRequirement, value, RaisePropertyChanged()); }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }



    public static class NotifyPropertyChangedExtension
    {
        public static void MutateVerbose<TField>(this INotifyPropertyChanged instance, ref TField field, TField newValue, Action<PropertyChangedEventArgs> raise, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<TField>.Default.Equals(field, newValue)) return;
            field = newValue;
            raise?.Invoke(new PropertyChangedEventArgs(propertyName));
        }
    }
}
