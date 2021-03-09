namespace ResearchProgram.Classes
{
    public class WorkCategories
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public override string ToString()
        {
            return Title;
        }

    }
}
