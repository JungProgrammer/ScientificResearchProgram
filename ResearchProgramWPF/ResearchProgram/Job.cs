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
        public int Id { get; set; }

        // Название работы
        public string Title { get; set; }

        // Оклад
        public float Salary { get; set; }

        // Ставка
        public float SalaryRate { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}
