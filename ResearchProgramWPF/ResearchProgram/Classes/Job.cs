using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ResearchProgram
{
    public class Job: INotifyPropertyChanged
    {
        // Id в БД
        public int Id { get; set; }

        private string _title;
        // Название работы
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        // Оклад
        public float Salary { get; set; }

        // Ставка
        public float SalaryRate { get; set; }

        /// <summary>
        /// Возвращает список окладов всех работ
        /// </summary>
        /// <param name="jobs"></param>
        /// <returns></returns>
        public static List<string> GetSalariesFromPerson(List<Job> jobs)
        {
            List<string> salaries = new List<string>();

            foreach(Job job in jobs)
            {
                salaries.Add(job.Salary.ToString());
            }

            return salaries;
        }

        /// <summary>
        /// Возвращает список ставок всех работ
        /// </summary>
        /// <param name="jobs"></param>
        /// <returns></returns>
        public static List<string> GetSalaryRatesFromPerson(List<Job> jobs)
        {
            List<string> salaryRates = new List<string>();


            foreach (Job job in jobs)
            {
                salaryRates.Add(job.SalaryRate.ToString());
            }

            return salaryRates;
        }

        public override string ToString()
        {
            return Title;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
