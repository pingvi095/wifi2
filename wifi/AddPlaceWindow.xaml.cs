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
    /// <summary>
    /// Окно для добавления нового места с Wi-Fi в базу данных.
    /// Содержит логику для валидации данных, выбора изображения и сохранения информации.
    /// </summary>
    public partial class AddPlaceWindow : Window
    {
        /// <summary>
        /// Путь к выбранной фотографии для добавляемого места.
        /// </summary>
        private string photoPath = "";

        /// <summary>
        /// Конструктор окна инициализирует визуальные компоненты.
        /// </summary>
        public AddPlaceWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Открывает диалог выбора изображения и сохраняет путь к нему.
        /// </summary>
        private void ChoosePhoto_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp" };
            if (dlg.ShowDialog() == true)
            {
                photoPath = dlg.FileName;
                PhotoPathText.Text = System.IO.Path.GetFileName(photoPath);
            }
        }

        /// <summary>
        /// Проверяет введённые данные и сохраняет новое место в базе данных.
        /// </summary>
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var name = NameBox.Text.Trim();
            var type = (TypeBox.SelectedItem as ComboBoxItem)?.Content?.ToString()?.Trim() ?? "";
            var address = AddressBox.Text.Trim();
            var wifi = (WiFiBox.SelectedItem as ComboBoxItem)?.Content?.ToString()?.Trim() ?? "";
            var hours = (WorkBox.SelectedItem as ComboBoxItem)?.Content?.ToString()?.Trim() ?? "";
            var contact = ContactBox.Text.Trim();
            
            // Проверка на заполненность всех полей
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(type) ||
                string.IsNullOrWhiteSpace(address) || string.IsNullOrWhiteSpace(wifi) ||
                string.IsNullOrWhiteSpace(hours) || string.IsNullOrWhiteSpace(contact))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

           

            // Сохранение фото в папку проекта
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

            // SQL-запрос для вставки данных в таблицу places
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
    }
}
