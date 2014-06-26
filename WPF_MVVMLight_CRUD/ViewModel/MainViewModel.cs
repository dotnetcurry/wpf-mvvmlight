using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using WPF_MVVMLight_CRUD.Model;
using WPF_MVVMLight_CRUD.Services;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using WPF_MVVMLight_CRUD.MessageInfrastructure;

namespace WPF_MVVMLight_CRUD.ViewModel
{
     
    public class MainViewModel : ViewModelBase
    {

         

        IDataAccessService _serviceProxy;

        ObservableCollection<EmployeeInfo> _Employees;

        public ObservableCollection<EmployeeInfo> Employees
        {
            get { return _Employees; }
            set
            {
                _Employees = value;
                RaisePropertyChanged("Employees");
            }
        }


        /// <summary>
        /// The declaration of the Employee object for Save and Messanger purpose
        /// </summary>
        EmployeeInfo _EmpInfo;

        public EmployeeInfo EmpInfo
        {
            get { return _EmpInfo; }
            set
            {
                _EmpInfo = value;
                RaisePropertyChanged("EmpInfo");
            }
        }

        /// <summary>
        /// The declaration used in case of search Employee
        /// </summary>
        string _EmpName;

        public string EmpName
        {
            get { return _EmpName; }
            set 
            {
                _EmpName = value;
                RaisePropertyChanged("EmpName");
            }
        }


        #region Command Object Declarations
        public RelayCommand ReadAllCommand { get; set; }
        public RelayCommand<EmployeeInfo> SaveCommand { get; set; }

        public RelayCommand SearchCommand { get; set; }

        public RelayCommand<EmployeeInfo> SendEmployeeCommand { get; set; } 
        #endregion

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// The class gets an instance of The DataAccessService
        /// </summary>
        public MainViewModel(IDataAccessService servPxy)
        {
            _serviceProxy = servPxy;
            Employees = new ObservableCollection<EmployeeInfo>();
            EmpInfo = new EmployeeInfo(); 
            ReadAllCommand = new RelayCommand(GetEmployees);
            SaveCommand = new RelayCommand<EmployeeInfo>(SaveEmployee);
            SearchCommand = new RelayCommand(SearchEmployee);
            SendEmployeeCommand = new RelayCommand<EmployeeInfo>(SendEmployeeInfo);
            ReceiveEmployeeInfo();
        }

        /// <summary>
        /// Method to Read All Employees
        /// </summary>
        void GetEmployees()
        {
            Employees.Clear();
            foreach (var item in _serviceProxy.GetEmployees())
            {
                Employees.Add(item);
            }
        }

        /// <summary>
        /// Method to Save Employees. Once the Employee is added in the database
        /// it will be added in the Employees observable collection and Property Changed will be raised
        /// </summary>
        /// <param name="emp"></param>
        void SaveEmployee(EmployeeInfo emp)
        {
            EmpInfo.EmpNo =  _serviceProxy.CreateEmployee(emp);
            if(EmpInfo.EmpNo!=0)
            { 
                Employees.Add(EmpInfo);
                RaisePropertyChanged("EmpInfo");
            }
        }

        /// <summary>
        /// The method to search Employee baseed upon the EmpName
        /// </summary>
        void SearchEmployee()
        {
            Employees.Clear();
            var Res = from e in _serviceProxy.GetEmployees()
                      where e.EmpName.StartsWith(EmpName)
                      select e;
            foreach (var item in Res)
            {
                Employees.Add(item);
            }
        }

        /// <summary>
        /// The method to send the selected Employee from the DataGrid on UI
        /// to the View Model
        /// </summary>
        /// <param name="emp"></param>
        void SendEmployeeInfo(EmployeeInfo emp)
        {
            if(emp!=null)
            { 
                Messenger.Default.Send<MessageCommunicator>(new MessageCommunicator() { 
                  Emp = emp
                });
            }
        }

        /// <summary>
        /// The Method used to Receive the send Employee from the DataGrid UI
        /// and assigning it the the EmpInfo Notifiable property so that
        /// it will be displayed on the other view
        /// </summary>
        void ReceiveEmployeeInfo()
        {
            if (EmpInfo != null)
            { 
                Messenger.Default.Register<MessageCommunicator>(this,(emp)=>{
                    this.EmpInfo = emp.Emp;
                });
            }
        }
        
    }
}