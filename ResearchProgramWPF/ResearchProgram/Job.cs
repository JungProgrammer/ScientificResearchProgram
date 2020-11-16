using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResearchProgram
{
    public class Job
    {
        // Id в БД
        public int Id;

        // Название работы
        public string Title;

        // Оклад
        public float Salary;

        // Ставка
        public float SalaryRate;

        public override string ToString()
        {
            return Title;
        }
    }
}
