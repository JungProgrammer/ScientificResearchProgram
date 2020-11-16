using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResearchProgram
{
    public class Job
    {
<<<<<<< HEAD
        public int Id { get; set; }

        public string Title { get; set; }

        public int Salary { get; set; }
=======
        // Id в БД
        public int Id;

        // Название работы
        public string Title;

        // Оклад
        public float Salary;

        // Ставка
        public float SalaryRate;
>>>>>>> 4f0c2ddc71648269d9222e06aa1bc6e1009f3a83

        public override string ToString()
        {
            return Title;
        }
    }
}
