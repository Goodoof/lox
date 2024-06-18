using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;

namespace Защитник.Windows
{
    /// <summary>
    /// Логика взаимодействия для AccessManagementWindow.xaml
    /// </summary>
    public partial class AccessManagementWindow : Window
    {
        private bool isWindowBlocked = false;
        private StrazhnikEntities _context;

        public AccessManagementWindow(string userName, string userSurname, string userPatronymic)
        {
            InitializeComponent();
            // Сохраняем информацию о пользователе
            _context = StrazhnikEntities.GetContext();
            UserInformationTextBox.Text = $"{Environment.NewLine}{userSurname} {userName} {userPatronymic}";
        }

        private void ChoosePhotoButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg;*.gif)|*.png;*.jpeg;*.jpg;*.gif|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    BitmapImage bitmap = new BitmapImage(new Uri(openFileDialog.FileName));
                    SelectedImage.Source = bitmap;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}");
                }
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка наличия пустых полей
            if (string.IsNullOrWhiteSpace(Name.Text) ||
                string.IsNullOrWhiteSpace(Patronymic.Text) ||
                string.IsNullOrWhiteSpace(Surname.Text) ||
                string.IsNullOrWhiteSpace(JobTitle.Text))
            {
                await BlockWindow();
                return;
            }

            try
            {
                // Создание нового объекта Пользователь
                Пользователь user = new Пользователь
                {
                    Имя = Name.Text,
                    Отчество = Patronymic.Text,
                    Фамилия = Surname.Text,
                    Должность = JobTitle.Text,
                    // Заполните остальные свойства объекта в соответствии с вашими данными
                };

                // Добавление нового пользователя в контекст данных
                _context.Пользователь.Add(user);

                // Сохранение изменений в базе данных
                _context.SaveChanges();

                MessageBox.Show("Данные сохранены.", "Успех");

                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения данных: {ex.Message}", "Ошибка");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
            MessageBox.Show("Данные очищены.", "Отмена");
        }

        private async Task BlockWindow()
        {
            // Проверка наличия пустых полей перед блокировкой окна
            if (string.IsNullOrWhiteSpace(Name.Text) ||
                string.IsNullOrWhiteSpace(Patronymic.Text) ||
                string.IsNullOrWhiteSpace(Surname.Text) ||
                string.IsNullOrWhiteSpace(JobTitle.Text))
            {
                MessageBox.Show("Необходимо заполнить все поля перед сохранением.", "Предупреждение");
                return;
            }

            isWindowBlocked = true;
            SaveButton.IsEnabled = false;
            CancelButton.IsEnabled = false;

            await Task.Delay(TimeSpan.FromSeconds(10)); // Блокировка на 10 секунд

            isWindowBlocked = false;
            SaveButton.IsEnabled = true;
            CancelButton.IsEnabled = true;
        }

        private void ClearFields()
        {
            Name.Text = string.Empty;
            Patronymic.Text = string.Empty;
            Surname.Text = string.Empty;
            JobTitle.Text = string.Empty;
            // Очистка выбранного изображения
            SelectedImage.Source = null;
        }
    }
}