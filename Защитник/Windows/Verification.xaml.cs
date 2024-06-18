using System;
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
using System.Windows.Shapes;

namespace Защитник.Windows
{
    /// <summary>
    /// Логика взаимодействия для Verification.xaml
    /// </summary>
    public partial class Verification : Window
    {
        private StrazhnikEntities _context;
        public Verification(string userName, string userSurname, string userPatronymic)
        {
            InitializeComponent();
            // Сохраняем информацию о пользователе
            _context = StrazhnikEntities.GetContext();
            UserInformationTextBox.Text = $"{Environment.NewLine}{userSurname} {userName} {userPatronymic}";
            LoadData(); // Вызываем метод LoadData для загрузки данных
        }

        public Verification()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            // Загрузка данных в DataGrid из базы данных
            DataGridVerification.ItemsSource = _context.Пользователь.ToList();
            DataGridMandat.ItemsSource = _context.Пользователь.ToList();
            //ТипUser.ItemsSource = _context.Пользователь.ToList();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Обновление записей в базе данных
                foreach (Пользователь user in DataGridVerification.ItemsSource)
                {
                    var existingUser = _context.Пользователь.FirstOrDefault(u => u.КодПользователя == user.КодПользователя);
                    if (existingUser != null)
                    {
                        existingUser.Фамилия = user.Фамилия;
                        existingUser.Имя = user.Имя;
                        existingUser.Отчество = user.Отчество;
                        existingUser.Должность = user.Должность;
                        existingUser.ТипПользователя = user.КодПользователя.ToString();
                        existingUser.Логин = user.Логин;
                        existingUser.Пароль = user.Пароль;
                        existingUser.СекретноеСлово = user.СекретноеСлово;
                        existingUser.Одобрен = user.Одобрен;
                    }
                }

                _context.SaveChanges();

                MessageBox.Show("Данные успешно сохранены.", "Успех");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения данных: {ex.Message}", "Ошибка");
            }
        }
    }
}