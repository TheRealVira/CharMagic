#region License

// Copyright (c) 2016, Vira
// All rights reserved.
// Solution: CharMagic
// Project: CharMagic
// Filename: MainWindow.xaml.cs
// Date - created:2016.08.31 - 11:40
// Date - current: 2016.09.01 - 12:25

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
using Clipboard = System.Windows.Clipboard;
using MessageBox = System.Windows.MessageBox;
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

            SplitterTxtBx.Text = " /[&\\/#,+()$~%.\'\":*!?<>{}]/g,\'_\'";
            _curses = new Dictionary<string, CharMagicAPI>();
            _curses.Add("Crucio", new Crucio());
            CursesCmbBx.Items.Add("Crucio");
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
                        // If not, than it gets a bit complicated:
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
            OutputTxtBx.Visibility = File.Exists(InputTxtBx.Text) ? Visibility.Visible : Visibility.Hidden;
            OutputhFileBtn.Visibility = File.Exists(InputTxtBx.Text) ? Visibility.Visible : Visibility.Hidden;

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
                OutputTxtBx.Text = fbd.SelectedPath + "\\" + Path.GetFileNameWithoutExtension(InputTxtBx.Text) +
                                   ".cursed";
            }
        }

        private void SplitterTxtBx_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if (SplitterTxtBx.Text.Length > 1)
            //{
            //    SplitterTxtBx.Text = SplitterTxtBx.Text.Substring(0, 1);
            //}
            //else if (SplitterTxtBx.Text.Length == 0)
            //{
            //    SplitterTxtBx.Text = " ";
            //}
        }

        private void CurseBtn_Click(object sender, RoutedEventArgs e)
        {
            CurseOrNot(true);
        }

        private void CurseFile(bool curse)
        {
            // Create File without locking
            using (
                var writeStream = File.Create(OutputTxtBx.Text))
            {
                writeStream.Flush();
                writeStream.Close();
                writeStream.Dispose();
            }

            using (
                var writer = new StreamWriter(OutputTxtBx.Text)
                )
            {
                using (var readStream = new StreamReader(InputTxtBx.Text))
                {
                    while (!readStream.EndOfStream)
                    {
                        var line = readStream.ReadLine();
                        var splitted = line.ToCharArray();
                        var toDisplay = string.Empty;

                        for (var i = 0; i < splitted.Length; i++)
                        {
                            var current = string.Empty;
                            var next = string.Empty;
                            while (i < splitted.Length)
                            {
                                if (SplitterTxtBx.Text.Contains(splitted[i]))
                                {
                                    next = splitted[i] + "";
                                    break;
                                }

                                current += splitted[i];
                                i++;
                            }

                            toDisplay += (curse
                                ? _curses[(string) CursesCmbBx.SelectedItem].Curse(current)
                                : _curses[(string) CursesCmbBx.SelectedItem].LiftCurse(current)) + next;
                        }

                        writer.WriteLine(toDisplay);
                    }

                    readStream.Close();
                    readStream.Dispose();
                }

                writer.Close();
                writer.Dispose();
            }
        }

        private void CurseString(bool curse)
        {
            if (_isBussy) return;

            _isBussy = true;
            var splitted = InputTxtBx.Text.ToCharArray();
            var output = splitted;
            var toDisplay = string.Empty;

            for (var i = 0; i < splitted.Length; i++)
            {
                var current = string.Empty;
                var next = string.Empty;
                while (i < splitted.Length)
                {
                    if (SplitterTxtBx.Text.Contains(splitted[i]))
                    {
                        next = splitted[i] + "";
                        break;
                    }

                    current += splitted[i];
                    i++;
                }

                toDisplay += (curse
                    ? _curses[(string) CursesCmbBx.SelectedItem].Curse(current)
                    : _curses[(string) CursesCmbBx.SelectedItem].LiftCurse(current)) + next;
            }


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
                            SpeedSlider.Dispatcher.Invoke(() =>
                                output[i1] +=
                                    Convert.ToChar(
                                        (int) Math.Ceiling((toDisplay[i1] - output[i1])/(SpeedSlider.Value/10)))
                                );
                            OutputTxtBlck.Dispatcher.Invoke(() => { OutputTxtBlck.Text = new string(output); },
                                DispatcherPriority.Render);
                            continue;
                        }

                        SpeedSlider.Dispatcher.Invoke(() =>
                            output[i1] -=
                                Convert.ToChar((int) Math.Ceiling((output[i1] - toDisplay[i1])/(SpeedSlider.Value/10)))
                            );
                        OutputTxtBlck.Dispatcher.Invoke(() => { OutputTxtBlck.Text = new string(output); },
                            DispatcherPriority.Render);
                    }
                }
            });

            _isBussy = false;
        }

        private void InputTxtBx_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CurseFileCkBx.IsChecked == true)
            {
                if (File.Exists(InputTxtBx.Text))
                {
                    OutputhFileBtn.Visibility = Visibility.Visible;
                    OutputTxtBx.Visibility = Visibility.Visible;
                }
                else
                {
                    OutputhFileBtn.Visibility = Visibility.Hidden;
                    OutputTxtBx.Visibility = Visibility.Hidden;
                }

                return;
            }

            OutputTxtBlck.Text = InputTxtBx.Text;
        }

        private void LiftCurseBtn_OnClickCurseBtn_Click(object sender, RoutedEventArgs e)
        {
            CurseOrNot(false);
        }

        private void CurseOrNot(bool curse)
        {
            if (CurseFileCkBx.IsChecked == true)
            {
                if (!File.Exists(InputTxtBx.Text))
                {
                    MessageBox.Show($"The input file \"{OutputTxtBx.Text}\" doesn't exist, or is blocked!");
                    return;
                }

                try
                {
                    if (!Directory.Exists(Path.GetDirectoryName(OutputTxtBx.Text)))
                    {
                        MessageBox.Show($"The output path \"{OutputTxtBx.Text}\" doesn't exist, or is blocked!");
                        return;
                    }
                }
                catch
                {
                    MessageBox.Show($"The output path \"{OutputTxtBx.Text}\" doesn't exist, or is blocked!");
                    return;
                }

                if (File.Exists(OutputTxtBx.Text))
                {
                    MessageBox.Show($"The output file \"{OutputTxtBx.Text}\" currently exists and won't be overwritten!");
                    return;
                }

                CurseFile(curse);

                return;
            }

            CurseString(curse);
        }

        private void CopyBtn_OnClickBTN_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(OutputTxtBlck.Text);
        }

        private void InfoBtn_OnClickBtn_OnClickBTN_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(_curses[(string) CursesCmbBx.SelectedItem].ToString());
        }
    }
}