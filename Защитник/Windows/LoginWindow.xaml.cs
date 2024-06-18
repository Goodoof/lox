using System;
using System.Linq;
using System.Windows;

namespace Защитник.Windows
{
    public partial class LoginWindow : Window
    {
        private readonly StrazhnikEntities _context;
        public string CurrentUserName { get; private set; }
        public string CurrentUserSurname { get; private set; }
        public string CurrentUserPatronymic { get; private set; }

        public LoginWindow()
        {
            InitializeComponent();
            _context = StrazhnikEntities.GetContext();
            InitializeUserTypes();
        }
        private void InitializeUserTypes()
        {
            try
            {
                // Получаем список уникальных ролей пользователей из базы данных
                var userTypes = _context.Пользователь.Select(u => u.ТипПользователя).Distinct().ToList();

                // Очищаем ComboBox перед добавлением новых элементов
                userTypeComboBox.Items.Clear();

                // Добавляем полученные роли в ComboBox
                foreach (var userType in userTypes)
                {
                    userTypeComboBox.Items.Add(userType);
                }

                // Устанавливаем выбранным первый элемент, если он есть
                if (userTypes.Any())
                {
                    userTypeComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при инициализации типов пользователей: {ex.Message}");
            }
        }
        private void LoginSuccess()
        {
            string login = LoginInput.Password; // Получаем логин из PasswordBox
            string password = PasswordInput.Password; // Получаем пароль из PasswordBox
            string secretWord = WordInput.Password; // Получаем секретное слово из PasswordBox
            string userType = userTypeComboBox.SelectedItem as string; // Получаем выбранный тип пользователя из ComboBox

            // Проверяем, что все поля заполнены
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(secretWord) || string.IsNullOrWhiteSpace(userType))
            {
                MessageBox.Show("Необходимо заполнить все поля");
                return;
            }

            // Проверяем аутентификацию пользователя
            var user = _context.Пользователь.FirstOrDefault(u => u.Логин == login && u.Пароль == password && u.СекретноеСлово == secretWord && u.ТипПользователя == userType);
            if (user != null)
            {
                // Установка значений для переменных CurrentUserName, CurrentUserSurname и CurrentUserPatronymic
                CurrentUserName = user.Имя;
                CurrentUserSurname = user.Фамилия;
                CurrentUserPatronymic = user.Отчество;

                // Проверяем роль пользователя и переходим на соответствующую страницу
                switch (user.ТипПользователя)
                {
                    case "Администратор":
                        // Создаем экземпляр AccessManagementWindow и передаем информацию о пользователе через конструктор
                        AccessManagementWindow accessManagementWindow = new AccessManagementWindow(CurrentUserName, CurrentUserSurname, CurrentUserPatronymic);
                        accessManagementWindow.Show();
                        break;
                    case "Служащий ИБ":
                        // Создаем экземпляр Verification и передаем информацию о пользователе через конструктор
                        Verification verificationWindow = new Verification(CurrentUserName, CurrentUserSurname, CurrentUserPatronymic);

                        verificationWindow.Show();
                        break;
                    default:
                        MessageBox.Show("Неизвестный тип пользователя");
                        break;
                }

                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин, пароль, секретное слово или тип пользователя");
            }
        }

        private void ButtonEnterApp_Click(object sender, RoutedEventArgs e)
        {
            LoginSuccess();
        }
    }
}