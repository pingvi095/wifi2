using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using wifi.Helpers;
using wifi.Models;

namespace wifi
{
    public partial class AddPlaceWindow : Window
    {
        private string photoPath = "";
        public AddPlaceWindow()
        {
            InitializeComponent();
        }
        private bool PlaceExists(string name, string address)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM places WHERE LOWER(TRIM(name)) = LOWER(TRIM(@name)) AND LOWER(TRIM(address)) = LOWER(TRIM(@address))";
                var parameters = new Dictionary<string, object>
                {
                    { "@name", name },
                    { "@address", address }
                };

                var result = DatabaseHelper.ExecuteScalar(query, parameters);
                return Convert.ToInt32(result) > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при проверке данных: " + ex.Message);
                return true;
            }
        }
        private void ChoosePhoto_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp" };
            if (dlg.ShowDialog() == true)
            {
                photoPath = dlg.FileName;
                PhotoPathText.Text = System.IO.Path.GetFileName(photoPath);
            }
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var name = NameBox.Text.Trim();
            var type = (TypeBox.SelectedItem as ComboBoxItem)?.Content?.ToString()?.Trim() ?? "";
            var address = AddressBox.Text.Trim();
            var wifi = (WiFiBox.SelectedItem as ComboBoxItem)?.Content?.ToString()?.Trim() ?? "";
            var hours = (WorkBox.SelectedItem as ComboBoxItem)?.Content?.ToString()?.Trim() ?? "";
            var contact = ContactBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(type) ||
                string.IsNullOrWhiteSpace(address) || string.IsNullOrWhiteSpace(wifi) ||
                string.IsNullOrWhiteSpace(hours) || string.IsNullOrWhiteSpace(contact))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }
            if (PlaceExists(name, address))
            {
                MessageBox.Show("Место с таким названием и адресом уже существует!\nИзмените название или адрес.");
                NameBox.Focus();
                return;
            }
            string storedPath = "";
            if (!string.IsNullOrEmpty(photoPath) && File.Exists(photoPath))
            {
                var imagesDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images");
                Directory.CreateDirectory(imagesDir);
                var fname = $"{Guid.NewGuid()}{System.IO.Path.GetExtension(photoPath)}";
                var dest = System.IO.Path.Combine(imagesDir, fname);
                File.Copy(photoPath, dest, true);
                storedPath = dest;
            }
            var query = @"INSERT INTO places (name,type,address,wifi_quality,work_hours,photo_path,contact)
                          VALUES (@n,@t,@a,@w,@h,@p,@c)";

            var parameters = new Dictionary<string, object>
            {
                { "@n", name },
                { "@t", type },
                { "@a", address },
                { "@w", wifi },
                { "@h", hours },
                { "@p", storedPath },
                { "@c", contact }
            };

            DatabaseHelper.ExecuteNonQuery(query, parameters);
            MessageBox.Show("Место успешно добавлено!");
            Close();
        }

        private void ContactBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                string newText = new string(textBox.Text.Where(char.IsDigit).ToArray());
                if (textBox.Text != newText)
                {
                    textBox.Text = newText;
                    textBox.CaretIndex = newText.Length;
                }
            }
        }
        private void OnlyNum(object s, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text, 0);
        }
    }
}