using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using wifi.Helpers;
using wifi.Models;

namespace wifi
{
    /// <summary>
    /// Окно для редактирования существующего места в базе данных.
    /// Позволяет изменить основную информацию — тип, адрес, описание, качество Wi-Fi и другие поля.
    /// </summary>
    public partial class EditPlaceWindow : Window
    {
        /// <summary>
        /// Объект выбранного места, переданный из главного окна.
        /// </summary>
        private readonly Place place;

        /// <summary>
        /// Конструктор окна редактирования.
        /// Инициализирует интерфейс и заполняет поля текущими данными выбранного места.
        /// </summary>
        /// <param name="selectedPlace">Объект места, выбранного для редактирования.</param>
        public EditPlaceWindow(Place selectedPlace)
        {
            InitializeComponent();
            place = selectedPlace;
            LoadData();
        }

        private void LoadData()
        {
            NameBox.Text = place.Name;
            AddressBox.Text = place.Address;

            ContactBox.Text = place.Contact;


            foreach (ComboBoxItem item in TypeBox.Items)
                if (item.Content.ToString() == place.Type) { TypeBox.SelectedItem = item; break; }

            foreach (ComboBoxItem item in WiFiBox.Items)
                if (item.Content.ToString() == place.WiFiQuality) { WiFiBox.SelectedItem = item; break; }
            foreach (ComboBoxItem item in WorkBox.Items)
                if (item.Content.ToString() == place.WorkHours) { WorkBox.SelectedItem = item; break; }
        }

        /// <summary>
        /// Сохраняет внесённые изменения в базу данных.
        /// Выполняет валидацию данных и формирует запрос <c>UPDATE</c> к таблице <c>places</c>.
        /// </summary>
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var name = NameBox.Text.Trim();
            var type = (TypeBox.SelectedItem as ComboBoxItem)?.Content?.ToString()?.Trim() ?? "";
            var address = AddressBox.Text.Trim();
            var wifi = (WiFiBox.SelectedItem as ComboBoxItem)?.Content?.ToString()?.Trim() ?? "";
            var hours = (WorkBox.SelectedItem as ComboBoxItem)?.Content?.ToString()?.Trim() ?? "";
            var contact = ContactBox.Text.Trim();
            

            // Проверка на заполнение всех полей
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(type) ||
                string.IsNullOrWhiteSpace(address) || string.IsNullOrWhiteSpace(wifi) ||
                string.IsNullOrWhiteSpace(hours) || string.IsNullOrWhiteSpace(contact))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

        
            // Подготовка SQL-запроса
            string query = @"UPDATE places SET 
                                name=@n,
                                type=@t,
                                address=@a,
                                wifi_quality=@w,
                                work_hours=@h,
                                contact=@c
                             WHERE id=@id";

            var parameters = new Dictionary<string, object>
            {
                { "@n", name },
                { "@t", type },
                { "@a", address },
                { "@w", wifi },
                { "@h", hours },
                { "@c", contact },
                { "@id", place.Id }
            };

            // Выполнение запроса обновления
            DatabaseHelper.ExecuteNonQuery(query, parameters);
            MessageBox.Show("Изменения успешно сохранены!");
            Close();
        }
    }
}
