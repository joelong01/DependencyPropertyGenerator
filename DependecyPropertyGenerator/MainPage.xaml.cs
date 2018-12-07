using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DependecyPropertyGenerator
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }
        //<!--public static readonly DependencyProperty StateDescriptionProperty = DependencyProperty.Register("StateDescription", typeof(string), typeof(MainPage), new PropertyMetadata("Hit Start"));-->
        //public string ActivePlayerBackground
        //{
        //    get
        //    {
        //        return (string)GetValue(ActivePlayerBackgroundProperty);
        //    }
        //    set
        //    {
        //        SetValue(ActivePlayerBackgroundProperty, value);
        //        ActivePlayerForeground = StaticHelpers.BackgroundToForegroundDictionary[value];
        //    }
        //}
        private void OnGenerateAndCopy(object sender, RoutedEventArgs e)
        {

            if (_chkDependencyProperty.IsChecked == true)
            {
                GenerateDependencyProp();
            }
            else if (_chkTypescript.IsChecked == true)
            {
                GenerateTypeScriptProp();
            }
            else
            {
                GenerateFieldProperty();
            }


        }

        private void GenerateFieldProperty()
        {
            //
            //  int _score = 0;
            //  public int Score
            //  {
            //    get
            //    {
            //        return _score;
            //    }
            //    set
            //    {
            //        if (_score != value)
            //        {
            //            _score = value;
            //            NotifyPropertyChanged();
            //        }
            //    }
            //}

            if (_chkUnderscoreField.IsChecked == true)
            {
                _txtFieldName.Text = "_" + _txtPropName.Text;
            }

            string s = string.Format($"{_txtPropType.Text} {_txtFieldName.Text} = {_txtDefault.Text};\npublic {_txtPropType.Text} {_txtPropName.Text}\n[\n\tget\n\t[\n\t\treturn {_txtFieldName.Text};\n\t]");
            s += String.Format($"\n\tset\n\t[\n\t\tif({_txtFieldName.Text} != value)\n\t\t[\n\t\t\t{_txtFieldName.Text}=value;\n\t\t\tNotifyPropertyChanged();\n\t\t]\n\t]\n] ");
            s = s.Replace(']', '}');
            s = s.Replace('[', '{');
            _txtCode.Text = s;

        }

        private void GenerateTypeScriptProp()
        {

            if (_chkUnderscoreField.IsChecked == true)
            {
                _txtFieldName.Text = "_" + _txtPropName.Text;
            }
            string skeleton =
@"private __VAR__NAME: __PROP_TYPE__; 
get __PROP_NAME__(): __PROP_TYPE__ {
    return this.__VAR__NAME;
}

set __PROP_NAME__(value: string) {
    if (value !== this.__VAR__NAME) {

        this.__VAR__NAME = value;
        __NOTIFY__
    }
}";

            string notify = "this.NotifyPropertyChanged(\"__PROP_NAME__\")";

            string s = skeleton.Replace("__PROP_NAME__", _txtPropName.Text);
            s = s.Replace("__PROP_TYPE__", _txtPropType.Text);
            s = s.Replace("__VAR__NAME", _txtFieldName.Text);
            if (_chkNotificationFunction.IsChecked == true)
            {
                notify = notify.Replace("__PROP_NAME__", _txtPropName.Text);
                
            }
            else
            {
                notify = "";
            }

            s = s.Replace("__NOTIFY__", notify);
            s = s.Replace("\t", "");
            s = s.Trim();
            _txtCode.Text = s;
        }

        private void GenerateDependencyProp()
        {
            string nl = "\n";


            bool hasNotifyFunc = (bool)_chkNotificationFunction.IsChecked;


            string s = string.Format($"public static readonly DependencyProperty {_txtPropName.Text}Property = DependencyProperty.Register(\"{_txtPropName.Text}\", typeof({_txtPropType.Text}), typeof({_txtClasstype.Text}), new PropertyMetadata({_txtDefault.Text}");
            if (hasNotifyFunc)
            {
                s += string.Format($", {_txtPropName.Text}Changed)); {nl}");
            }
            else
            {
                s += string.Format($"));{nl}");
            }

            string tab = "\t";
            s += String.Format($"public {_txtPropType.Text} {_txtPropName.Text}{tab}{nl}[{nl}");
            s += String.Format($"{tab}get => ({_txtPropType.Text})GetValue({_txtPropName.Text}Property); {nl}");
            s += String.Format($"{tab}set => SetValue({_txtPropName.Text}Property,value);{nl}]");
            if (hasNotifyFunc)
            {
                //private static void SelectedCardChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
                //{
                //    var depPropClass = d as GameGeneratorPage;
                //    var depPropValue = (CardCtrl)e.NewValue;
                //    depPropClass?.SetSelectedCard(depPropValue);
                //}

                s += String.Format($"private static void {_txtPropName.Text}Changed (DependencyObject d, DependencyPropertyChangedEventArgs e){nl}[{nl}{tab}var depPropClass = d as {_txtClasstype.Text};{nl}");
                s += String.Format($"{tab}var depPropValue = ({_txtPropType.Text})e.NewValue;{nl}");
                s += String.Format($"{tab}depPropClass?.Set{_txtPropName.Text}(depPropValue);{nl}]{nl}");

                //private void SetPirateTile(TileCtrl tile)

                s += String.Format($"private void Set{_txtPropName.Text}({_txtPropType.Text} value){nl}[{nl}{nl}]{nl}");

            }

            s = s.Replace(']', '}');
            s = s.Replace('[', '{');


            _txtCode.Text = s;
        }

        private void TextBox_GoFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            tb.SelectAll();
        }

        private void _chkDependencyProperty_Checked(object sender, RoutedEventArgs e)
        {
           
        }

        private void DependencyOptionClicked(object sender, RoutedEventArgs e)
        {
            if (_chkDependencyProperty.IsChecked == true)
            {
                _fieldStackPanel.Visibility = Visibility.Collapsed;
                _spClassType.Visibility = Visibility.Visible;
            }
            else
            {
                _fieldStackPanel.Visibility = Visibility.Visible;
                _spClassType.Visibility = Visibility.Collapsed;
            }
        }

        private void UnderscoreFieldNameClicked(object sender, RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            if (chk.IsChecked == true)
            {
                _fieldStackPanel.Visibility = Visibility.Collapsed;
            }
            else if (_chkDependencyProperty.IsChecked != true)
            {
                _fieldStackPanel.Visibility = Visibility.Visible;
            }
        }
    }
}
