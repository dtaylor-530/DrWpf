using DrWPF.Windows.Data;
using System;
using System.Collections;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DrWpf.DemoApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            _observableButtonStyles = FindResource("ButtonStyles") as ObservableStyleDictionary;
            ReloadDictionary(null, null);
        }

        void OnToolBarButtonClick(object sender, RoutedEventArgs e)
        {
            int index = int.Parse((e.OriginalSource as Button).Content.ToString());
            llb.SelectedIndex = index;
            (llb.ItemContainerGenerator.ContainerFromItem(llb.Items[index]) as ListBoxItem).Focus(); // .BringIntoView();
        }


        private ObservableStyleDictionary _observableButtonStyles = null;


        private void ReloadDictionary(object sender, RoutedEventArgs e)
        {
            // cache the selected index
            int index = StyleSelector.SelectedIndex;

            ResourceDictionary buttonStyles = Application.LoadComponent(
                new Uri("/DrWpf;component/Themes/ButtonStyles.xaml", UriKind.Relative)) as ResourceDictionary;
            _observableButtonStyles.Clear();
            foreach (DictionaryEntry de in buttonStyles)
            {
                if (!(de.Value is Style)) continue;

                if (de.Key == typeof(Button))
                {
                    _observableButtonStyles["Default Style"] = de.Value as Style;
                }
                else if (de.Key is string)
                {
                    _observableButtonStyles[de.Key as string] = de.Value as Style;
                }
            }

            // restore the selected index, if still applicable; otherwise select the first index
            if (index > -1 && StyleSelector.Items.Count > index)
            {
                StyleSelector.SelectedIndex = index;
            }
            else
            {
                StyleSelector.SelectedIndex = 0;
            }
        }

        private void AddOrRemoveButton(object sender, RoutedEventArgs e)
        {
            if (!(e.OriginalSource is Button)) return;

            Style selectedStyle = (e.OriginalSource as Button).Style;
            DependencyObject parentLVI = VisualTreeHelper.GetParent(e.OriginalSource as DependencyObject);
            while (parentLVI != null && !(parentLVI is ListViewItem))
                parentLVI = VisualTreeHelper.GetParent(parentLVI);
            if (parentLVI != null)
            {
                int index = StyleListView.ItemContainerGenerator.IndexFromContainer(parentLVI);
                if (index % 2 == 0)
                {
                    // if an even index, remove the style from the dictionary
                    _observableButtonStyles.Remove(((KeyValuePair<string, Style>)StyleListView.Items[index]).Key as string);
                }
                else
                {
                    // if an odd index, duplicate the style in the dictionary
                    string newKey = "New Style ";
                    int i = 1;
                    while (_observableButtonStyles.ContainsKey(newKey + i.ToString())) i++;
                    _observableButtonStyles[newKey + i.ToString()] = selectedStyle;
                }
            }
        }
    }
}
