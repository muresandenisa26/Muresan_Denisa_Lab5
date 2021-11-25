using AutoGeistModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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

namespace Muresan_Denisa_Lab5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    enum ActionState { New, Edit, Delete, Nothing}
    public partial class MainWindow : Window
    {
        ActionState action = ActionState.Nothing;
        //using AutoGeistModel
        AutoGeistEntitiesModel ctx = new AutoGeistEntitiesModel();
        CollectionViewSource carViewSource;
        CollectionViewSource customersViewSource;
        CollectionViewSource carOrdersViewSource;

        Binding bodyStyleTextBoxBinding = new Binding();
        Binding modelTextBoxBinding = new Binding();
        Binding makeTextBoxBinding = new Binding();


        Binding firstNameTextBoxBinding = new Binding();
        Binding lastNameTextBoxBinding = new Binding();
        Binding purchaseDatePickerBinding = new Binding();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            modelTextBoxBinding.Path = new PropertyPath("Model");
            makeTextBoxBinding.Path = new PropertyPath("Make");
            bodyStyleTextBoxBinding.Path = new PropertyPath("BodyStyle");
            modelTextBox.SetBinding(TextBox.TextProperty, modelTextBoxBinding);
            makeTextBox.SetBinding(TextBox.TextProperty, makeTextBoxBinding);
            bodyStyleTextBox.SetBinding(TextBox.TextProperty, bodyStyleTextBoxBinding);

            firstNameTextBoxBinding.Path = new PropertyPath("FirstName");
            lastNameTextBoxBinding.Path = new PropertyPath("LastName");
            purchaseDatePickerBinding.Path = new PropertyPath("PurchaseDatePicker");
            firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameTextBoxBinding);
            lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameTextBoxBinding);
            purchaseDatePicker.SetBinding(DatePicker.SelectedDateProperty, purchaseDatePickerBinding);


        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            System.Windows.Data.CollectionViewSource carViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("carViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            // carViewSource.Source = [generic data source]
            System.Windows.Data.CollectionViewSource customerViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            // customerViewSource.Source = [generic data source]

            carViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("carViewSource")));
            carViewSource.Source = ctx.Car.Local;
            ctx.Car.Load();

            customersViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("carViewSource")));
            customersViewSource.Source = ctx.Customer.Local;
            ctx.Customer.Load();

            carOrdersViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("carOrdersViewSource")));
            //carOrdersViewSource.Source = ctx.Order.Local;
            ctx.Order.Load();
            BindDataGrid();
            cbCars.ItemsSource = ctx.Car.Local;
            //cbCars.DisplayMemberPath = "Make";
            cbCars.SelectedValuePath = "CarId";
            cbCustomers.ItemsSource = ctx.Customer.Local;

        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            TabItem ti = tbCtrlAutoGeist.SelectedItem as TabItem;
            switch (ti.Header)
            {
                case "Cars":
                    BindingOperations.ClearBinding(bodyStyleTextBox, TextBox.TextProperty);
                    BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);
                    BindingOperations.ClearBinding(modelTextBox, TextBox.TextProperty);
                    bodyStyleTextBox.Text = "";
                    makeTextBox.Text = "";
                    modelTextBox.Text = "";
                    Keyboard.Focus(bodyStyleTextBox);
                    SetValidationBinding();
                    break;
                case "Customers":
                    BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
                    BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
                    firstNameTextBox.Text = "";
                    lastNameTextBox.Text = "";
                    Keyboard.Focus(firstNameTextBox);
                    SetValidationBinding();
                    break;
                case "Orders":
                    break;
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            SetValidationBinding();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            carViewSource.View.MoveCurrentToNext();
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            carViewSource.View.MoveCurrentToPrevious();
        }

        private void SaveCars()
        {
            Car car = null;
            if (action == ActionState.New)
            {
                try
                {
                    car = new Car()
                    {
                        Make = makeTextBox.Text.Trim(),
                        Model = modelTextBox.Text.Trim(),
                        BodyStyle = bodyStyleTextBox.Text.Trim(),
                    };
                    ctx.Car.Add(car);
                    carViewSource.View.Refresh();
                    carViewSource.View.MoveCurrentTo(car);
                    ctx.SaveChanges();
                }
                // using System.Data;
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                makeTextBox.SetBinding(TextBox.TextProperty, makeTextBoxBinding);
                modelTextBox.SetBinding(TextBox.TextProperty, modelTextBoxBinding);
                bodyStyleTextBox.SetBinding(TextBox.TextProperty,
               bodyStyleTextBoxBinding);
            }
            else if (action == ActionState.Edit)
            {
                try
                {
                    car = (Car)carDataGrid.SelectedItem;
                    car.Make = makeTextBox.Text.Trim();
                    car.Model = modelTextBox.Text.Trim();
                    car.BodyStyle = bodyStyleTextBox.Text.Trim();
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                carViewSource.View.Refresh();
                carViewSource.View.MoveCurrentTo(car);
                makeTextBox.SetBinding(TextBox.TextProperty, makeTextBoxBinding);
                modelTextBox.SetBinding(TextBox.TextProperty, modelTextBoxBinding);
                bodyStyleTextBox.SetBinding(TextBox.TextProperty,
               bodyStyleTextBoxBinding);
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    car = (Car)carDataGrid.SelectedItem;
                    ctx.Car.Remove(car);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                carViewSource.View.Refresh();
                makeTextBox.SetBinding(TextBox.TextProperty, makeTextBoxBinding);
                modelTextBox.SetBinding(TextBox.TextProperty, modelTextBoxBinding);
                bodyStyleTextBox.SetBinding(TextBox.TextProperty,
                bodyStyleTextBoxBinding);
            }
        }

        private void btnPrevious1_Click(object sender, RoutedEventArgs e)
        {
            customersViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNext1_Click(object sender, RoutedEventArgs e)
        {
            customersViewSource.View.MoveCurrentToNext();
        }

        private void SaveCustomers()
        {
            Customer customer = null;
            if (action == ActionState.New)
            {
                try
                {
                    customer = new Customer()
                    {
                        FirstName = firstNameTextBox.Text.Trim(),
                        LastName = lastNameTextBox.Text.Trim(),

                    };
                    ctx.Customer.Add(customer);
                    customersViewSource.View.Refresh();
                    customersViewSource.View.MoveCurrentTo(customer);
                    ctx.SaveChanges();
                }
                // using System.Data;
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameTextBoxBinding);
                lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameTextBoxBinding);

            }
            else if (action == ActionState.Edit)
            {
                try
                {
                    customer = (Customer)customerDataGrid.SelectedItem;
                    customer.FirstName = firstNameTextBox.Text.Trim();
                    customer.LastName = lastNameTextBox.Text.Trim();
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customersViewSource.View.Refresh();
                customersViewSource.View.MoveCurrentTo(customer);
                firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameTextBoxBinding);
                lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameTextBoxBinding);

            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    customer = (Customer)carDataGrid.SelectedItem;
                    ctx.Customer.Remove(customer);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customersViewSource.View.Refresh();
                firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameTextBoxBinding);
                lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameTextBoxBinding);


            }
        }

        private void gbOperations_Click(object sender, RoutedEventArgs e)
        {
            Button SelectedButton = (Button)e.OriginalSource;
            Panel panel = (Panel)SelectedButton.Parent;
            foreach (Button B in panel.Children.OfType<Button>())
            {
                if (B != SelectedButton)
                    B.IsEnabled = false;
            }
            gbActions.IsEnabled = true;

        }

        private void ReInitializare()
        {
            Panel panel = gbOperations.Content as Panel;
            foreach (Button B in panel.Children.OfType<Button>())
            {
                B.IsEnabled = true;
            }
            gbActions.IsEnabled = false;

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            ReInitializare();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            TabItem ti = tbCtrlAutoGeist.SelectedItem as TabItem;
            switch (ti.Header)
            {
                case "Cars":
                    SaveCars();
                    break;
                case "Customers":
                    SaveInventory();
                    break;
                case "Orders":
                    break;
            }
            ReInitializare();
        }

        private void SaveInventory()
        {
            throw new NotImplementedException();
        }

        private void SaveOrders()
        {
            Order order = null;
            if (action == ActionState.New)
            {
                try
                {
                    Car car = (Car)cbCars.SelectedItem;
                    Customer customer = (Customer)cbCustomers.SelectedItem;
                    //instantiem Order entity
                    order = new Order()
                    {
                        CarId = car.CarId,
                        CustId = customer.CustId
                    };
                    //adaugam entitatea nou creata in context
                    ctx.Order.Add(order);
                    ctx.SaveChanges();
                    BindDataGrid();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else if (action == ActionState.Edit)
            {
                dynamic selectedOrder = orderDataGrid.SelectedItem;
                try
                {
                    int curr_id = selectedOrder.OrderId;
                    var editedOrder = ctx.Order.FirstOrDefault(s => s.OrderId ==
                    curr_id);
                    if (editedOrder != null)
                    {
                        editedOrder.CarId =
                       Convert.ToInt32(cbCars.SelectedValue.ToString());
                        editedOrder.CustId =
                       Int32.Parse(cbCustomers.SelectedValue.ToString());
                        //salvam modificarile
                        ctx.SaveChanges();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                BindDataGrid();
                // pozitionarea pe item-ul curent
                carOrdersViewSource.View.MoveCurrentTo(selectedOrder);
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    dynamic selectedOrder = orderDataGrid.SelectedItem;
                    int curr_id = selectedOrder.OrderId;
                    var deletedOrder = ctx.Order.FirstOrDefault(s => s.OrderId == curr_id);
                    if (deletedOrder != null)
                    {
                        ctx.Order.Remove(deletedOrder);
                        ctx.SaveChanges();
                        MessageBox.Show("Order Deleted Successfully", "Message");
                        BindDataGrid();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void BindDataGrid()
        {
            var queryOrder = from ord in ctx.Order
                             join cust in ctx.Customer on ord.CustId equals cust.CustId
                             join car in ctx.Car on ord.CarId equals car.CarId
                             select new
                             {
                                 ord.OrderId,
                                 ord.CarId,
                                 ord.CustId,
                                 cust.FirstName,
                                 cust.LastName,
                                 car.Make,
                                 car.Model
                             };
            carOrdersViewSource.Source = queryOrder.ToList();
        }

        private void SetValidationBinding()
        {
            Binding firstNameValidationBinding = new Binding();
            firstNameValidationBinding.Source = customersViewSource;
            firstNameValidationBinding.Path = new PropertyPath("FirstName");
            firstNameValidationBinding.NotifyOnValidationError = true;
            firstNameValidationBinding.Mode = BindingMode.TwoWay;
            firstNameValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //string required
            firstNameValidationBinding.ValidationRules.Add(new StringNotEmpty());
            firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameValidationBinding);


            Binding lastNameValidationBinding = new Binding();
            lastNameValidationBinding.Source = customersViewSource;
            lastNameValidationBinding.Path = new PropertyPath("LastName");
            lastNameValidationBinding.NotifyOnValidationError = true;
            lastNameValidationBinding.Mode = BindingMode.TwoWay;
            lastNameValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //string min length validator
            lastNameValidationBinding.ValidationRules.Add(new StringMinLength());
            lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameValidationBinding); //setare binding nou


            Binding bodyStyleValidationBinding = new Binding();
            bodyStyleValidationBinding.Source = carViewSource;
            bodyStyleValidationBinding.Path = new PropertyPath("BodyStyle");
            bodyStyleValidationBinding.NotifyOnValidationError = true;
            bodyStyleValidationBinding.Mode = BindingMode.TwoWay;
            bodyStyleValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //string required
            bodyStyleValidationBinding.ValidationRules.Add(new StringNotEmpty());
            bodyStyleTextBox.SetBinding(TextBox.TextProperty, bodyStyleValidationBinding);


            Binding makeValidationBinding = new Binding();
            makeValidationBinding.Source = carViewSource;
            makeValidationBinding.Path = new PropertyPath("Make");
            makeValidationBinding.NotifyOnValidationError = true;
            makeValidationBinding.Mode = BindingMode.TwoWay;
            makeValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //string min lenght validator
            makeValidationBinding.ValidationRules.Add(new StringMinLength());
            makeTextBox.SetBinding(TextBox.TextProperty, makeValidationBinding);
        }


    }
}
