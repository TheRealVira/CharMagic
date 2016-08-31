#region License

// Copyright (c) 2016, Vira
// All rights reserved.
// Solution: CharMagic
// Project: CharMagic
// Filename: MainWindow.xaml.cs
// Date - created:2016.08.31 - 11:40
// Date - current: 2016.08.31 - 15:08

#endregion

#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Threading;
using CharMagicLib;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

#endregion

namespace CharMagic
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly Dictionary<string, CharMagicAPI> _curses;

        private bool _isBussy;

        public MainWindow()
        {
            InitializeComponent();

            SplitterTxtBx.Text = " ";
            _curses = new Dictionary<string, CharMagicAPI>();

            Loadplugin();
        }

        /// <summary>
        ///     Will load all *.dll in the Plugins directory and add the matching algorithms to their matching combobox.
        /// </summary>
        private void Loadplugin()
        {
            if (!Directory.Exists("Plugins"))
            {
                Directory.CreateDirectory("Plugins");
                return;
            }

            // NOTE: .AsParallel().ForAll creates an endless break here! Please don't ask me why...

            foreach (var file in Directory.GetFiles("Plugins").Where(x => x.EndsWith(".dll")))
            {
                UltimateFactory<CharMagicAPI>.Compute(Assembly.LoadFrom(file)).ToList().ForEach(x =>
                {
                    _curses.TryAdd(x.Key, x.Value);

                    if (CursesCmbBx.Dispatcher.CheckAccess())
                        // Check if the thread allows us to access the control
                    {
                        CursesCmbBx.Items.Add(x.Key); // If so, than we just need to add the new item
                    }
                    else
                    {
                        // If not, than it gets a bit complicater:
                        // Now we have to invoke the dispatcher of the control from its current thread to access it.
                        CursesCmbBx.Dispatcher.Invoke(DispatcherPriority.Normal,
                            new Action(() => CursesCmbBx.Items.Add(x.Key)));
                    }
                });
            }

            CursesCmbBx.SelectedIndex = 0;
        }

        private void CurseFileCkBx_Checked(object sender, RoutedEventArgs e)
        {
            SearchFileBtn.Visibility = Visibility.Visible;
            OutputTxtBx.Visibility = Visibility.Visible;
            OutputhFileBtn.Visibility = Visibility.Visible;

            OutputCard.Visibility = Visibility.Hidden;
        }

        private void CurseFileCkBx_Unchecked(object sender, RoutedEventArgs e)
        {
            SearchFileBtn.Visibility = Visibility.Hidden;
            OutputTxtBx.Visibility = Visibility.Hidden;
            OutputhFileBtn.Visibility = Visibility.Hidden;

            OutputCard.Visibility = Visibility.Visible;
        }

        private void SearchFileBtn_OnClickBTN_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of the open file dialog box.
            var openFileDialog1 = new OpenFileDialog {Multiselect = false};

            // Call the ShowDialog method to show the dialog box.
            var userClickedOk = openFileDialog1.ShowDialog();

            // Process input if the user clicked OK.
            if (userClickedOk == true)
            {
                InputTxtBx.Text = openFileDialog1.FileName;
            }
        }

        private void SearchFolderBtn_OnClickBTN_Click(object sender, RoutedEventArgs e)
        {
            var fbd = new FolderBrowserDialog();

            var result = fbd.ShowDialog();

            if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                OutputTxtBx.Text = fbd.SelectedPath;
            }
        }

        private void SplitterTxtBx_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SplitterTxtBx.Text.Length > 1)
            {
                SplitterTxtBx.Text = SplitterTxtBx.Text.Substring(0, 1);
            }
            else if (SplitterTxtBx.Text.Length == 0)
            {
                SplitterTxtBx.Text = " ";
            }
        }

        private void CurseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurseFileCkBx.IsChecked == true)
            {
                if (File.Exists(InputTxtBx.Text) && Directory.Exists(OutputTxtBx.Text))
                {
                    CurseFile();
                }

                return;
            }

            CurseString();
        }

        private void CurseFile()
        {
            // TODO
        }

        private void CurseString()
        {
            if (_isBussy) return;

            _isBussy = true;
            var splitted = InputTxtBx.Text.Split(Convert.ToChar(SplitterTxtBx.Text));
            var toDisplay = string.Empty;

            for (var i = 0; i < splitted.Length; i++)
            {
                toDisplay += _curses[(string) CursesCmbBx.SelectedItem].Curse(splitted[i]) +
                             (i != splitted.Length - 1 ? SplitterTxtBx.Text : "");
            }

            var output = new char[toDisplay.Length];

            Task.Factory.StartNew(() =>
            {
                for (var i = 0; i < toDisplay.Length; i++)
                {
                    var i1 = i;
                    // Note, that we MUST create a copy of the variable, because it'd count upwords in EVERY task!!!
                    while (true)
                    {
                        if (output[i1] == toDisplay[i1])
                        {
                            break;
                        }

                        if (output[i1] < toDisplay[i1])
                        {
                            output[i1] += Convert.ToChar((int) Math.Ceiling((toDisplay[i1] - output[i1])/2f));
                            OutputTxtBlck.Dispatcher.Invoke(() => { OutputTxtBlck.Text = new string(output); },
                                DispatcherPriority.Render);
                            continue;
                        }

                        output[i1] -= Convert.ToChar((int) Math.Ceiling((output[i1] - toDisplay[i1])/2f));
                        OutputTxtBlck.Dispatcher.Invoke(() => { OutputTxtBlck.Text = new string(output); },
                            DispatcherPriority.Render);
                    }
                }
            });

            _isBussy = false;
        }

        private void InputTxtBx_TextChanged(object sender, TextChangedEventArgs e)
        {
            OutputTxtBlck.Text = InputTxtBx.Text;
        }
    }
}